package main

/*
#cgo linux LDFLAGS: -ldl
#include <stdbool.h>
#include <stdlib.h>

#ifdef _WIN32
#include <windows.h>
typedef HMODULE lib_handle;
#define OCR_CALL __stdcall

static int add_library_dir(const char* path) {
	return SetDllDirectoryA(path) ? 1 : 0;
}

static lib_handle load_library(const char* path) {
	return LoadLibraryA(path);
}

static void* load_symbol(lib_handle handle, const char* name) {
	return (void*)GetProcAddress(handle, name);
}
#else
#include <dlfcn.h>
#define OCR_CALL
typedef void* lib_handle;

static int add_library_dir(const char* path) {
	return 1;
}

static lib_handle load_library(const char* path) {
	return dlopen(path, RTLD_NOW | RTLD_GLOBAL);
}

static void* load_symbol(lib_handle handle, const char* name) {
	return dlsym(handle, name);
}
#endif

typedef bool (OCR_CALL *initjson_fn)(const char*, const char*, const char*, const char*);
typedef const char* (OCR_CALL *detect_fn)(const char*);
typedef void (OCR_CALL *free_result_buffer_fn)(void*);
typedef void (OCR_CALL *enable_json_result_fn)(bool);

static bool call_initjson(void* fn, const char* det, const char* cls, const char* rec, const char* json) {
	return ((initjson_fn)fn)(det, cls, rec, json);
}

static const char* call_detect(void* fn, const char* image) {
	return ((detect_fn)fn)(image);
}

static void call_free_result_buffer(void* fn, void* buffer) {
	((free_result_buffer_fn)fn)(buffer);
}

static void call_enable_json_result(void* fn, bool enable) {
	((enable_json_result_fn)fn)(enable);
}
*/
import "C"

import (
	"bufio"
	"fmt"
	"os"
	"path/filepath"
	"runtime"
	"sort"
	"strings"
	"time"
	"unsafe"
)

func getCurrentDirectory() string {
	dir, err := os.Getwd()
	if err != nil {
		fmt.Println("get current directory failed:", err)
	}
	return dir
}

func currentRID() string {
	switch runtime.GOOS + "/" + runtime.GOARCH {
	case "windows/amd64":
		return "win-x64"
	case "linux/amd64":
		return "linux-x64"
	default:
		return runtime.GOOS + "-" + runtime.GOARCH
	}
}

func nativeLibraryFileName() string {
	if runtime.GOOS == "windows" {
		return "PaddleOCR.dll"
	}
	return "PaddleOCR.so"
}

func executableDirectory() string {
	exe, err := os.Executable()
	if err != nil {
		return ""
	}
	return filepath.Dir(exe)
}

func findNativeLibrary(rootDir string) (string, string, error) {
	fileName := nativeLibraryFileName()
	rid := currentRID()
	candidates := []string{
		filepath.Join(rootDir, "runtimes", rid, "native", fileName),
		filepath.Join(executableDirectory(), "runtimes", rid, "native", fileName),
		filepath.Join(rootDir, fileName),
		filepath.Join(executableDirectory(), fileName),
		filepath.Join(rootDir, "lib", fileName),
	}

	for _, candidate := range candidates {
		if candidate == "" {
			continue
		}
		if _, err := os.Stat(candidate); err == nil {
			return candidate, filepath.Dir(candidate), nil
		}
	}

	return "", "", fmt.Errorf("cannot find %s under current directory, executable directory, or runtimes/%s/native", fileName, rid)
}

func prependNativeSearchPath(nativeDir string) {
	if nativeDir == "" {
		return
	}

	if runtime.GOOS == "windows" {
		oldPath := os.Getenv("PATH")
		if !strings.Contains(oldPath, nativeDir) {
			_ = os.Setenv("PATH", nativeDir+string(os.PathListSeparator)+oldPath)
		}
	} else {
		oldPath := os.Getenv("LD_LIBRARY_PATH")
		if !strings.Contains(oldPath, nativeDir) {
			_ = os.Setenv("LD_LIBRARY_PATH", nativeDir+string(os.PathListSeparator)+oldPath)
		}
	}

	cNativeDir := C.CString(nativeDir)
	defer C.free(unsafe.Pointer(cNativeDir))
	C.add_library_dir(cNativeDir)
}

func loadSymbol(handle C.lib_handle, name string) (unsafe.Pointer, error) {
	cName := C.CString(name)
	defer C.free(unsafe.Pointer(cName))

	proc := C.load_symbol(handle, cName)
	if proc == nil {
		return nil, fmt.Errorf("cannot find symbol: %s", name)
	}
	return proc, nil
}

func preloadLinuxDependencies(nativeDir string, entryLibrary string) {
	if runtime.GOOS != "linux" {
		return
	}

	entries, err := os.ReadDir(nativeDir)
	if err != nil {
		return
	}

	var libraries []string
	for _, entry := range entries {
		name := entry.Name()
		if entry.IsDir() || name == entryLibrary || !strings.Contains(name, ".so") {
			continue
		}
		libraries = append(libraries, filepath.Join(nativeDir, name))
	}
	sort.Strings(libraries)

	pending := append([]string(nil), libraries...)
	for len(pending) > 0 {
		progress := false
		next := pending[:0]
		for _, library := range pending {
			cLibrary := C.CString(library)
			handle := C.load_library(cLibrary)
			C.free(unsafe.Pointer(cLibrary))
			if handle != nil {
				progress = true
				continue
			}
			next = append(next, library)
		}
		if !progress {
			return
		}
		pending = next
	}
}

func cString(value string) *C.char {
	return C.CString(value)
}

func main() {
	rootDir := getCurrentDirectory()
	libraryPath, nativeDir, err := findNativeLibrary(rootDir)
	if err != nil {
		fmt.Println(err)
		return
	}
	prependNativeSearchPath(nativeDir)
	preloadLinuxDependencies(nativeDir, filepath.Base(libraryPath))

	cLibraryPath := C.CString(libraryPath)
	defer C.free(unsafe.Pointer(cLibraryPath))

	ocrLibrary := C.load_library(cLibraryPath)
	if ocrLibrary == nil {
		fmt.Println("load native library failed:", libraryPath)
		return
	}

	initFunc, err := loadSymbol(ocrLibrary, "Initjson")
	if err != nil {
		fmt.Println(err)
		return
	}
	detectFunc, err := loadSymbol(ocrLibrary, "Detect")
	if err != nil {
		fmt.Println(err)
		return
	}
	freeResultBufferFunc, err := loadSymbol(ocrLibrary, "FreeResultBuffer")
	if err != nil {
		fmt.Println(err)
		return
	}
	enableJsonFunc, err := loadSymbol(ocrLibrary, "EnableJsonResult")
	if err != nil {
		fmt.Println(err)
		return
	}

	detModel := filepath.Join(rootDir, "models", "PP-OCRv6_tiny_det_infer")
	clsModel := filepath.Join(rootDir, "models", "PP-LCNet_x1_0_textline_ori")
	recModel := filepath.Join(rootDir, "models", "PP-OCRv6_tiny_rec_infer")
	configJson := `{"use_gpu": false,"cpu_math_library_num_threads": 30,"gpu_id": 0,"gpu_mem": 4000,"cpu_mem": 4000,"enable_mkldnn": true,"rec_img_h": 48,"rec_img_w": 320,"cls":false,"det":true,"use_angle_cls":false,"visualize":true}`

	cDetModel := cString(detModel)
	cClsModel := cString(clsModel)
	cRecModel := cString(recModel)
	cConfigJson := cString(configJson)
	defer C.free(unsafe.Pointer(cDetModel))
	defer C.free(unsafe.Pointer(cClsModel))
	defer C.free(unsafe.Pointer(cRecModel))
	defer C.free(unsafe.Pointer(cConfigJson))

	if !bool(C.call_initjson(initFunc, cDetModel, cClsModel, cRecModel, cConfigJson)) {
		fmt.Println("Initjson failed. Check models and native dependencies.")
		return
	}

	C.call_enable_json_result(enableJsonFunc, C.bool(false))

	imageDir := filepath.Join(rootDir, "images")
	images, err := os.ReadDir(imageDir)
	if err != nil {
		fmt.Println("read images directory failed:", err)
		return
	}

	for _, image := range images {
		if image.IsDir() {
			continue
		}

		fmt.Println("Image:", image.Name())
		startTime := time.Now()

		imagePath := filepath.Join(imageDir, image.Name())
		cImagePath := cString(imagePath)
		resultPtr := C.call_detect(detectFunc, cImagePath)
		C.free(unsafe.Pointer(cImagePath))

		if resultPtr == nil {
			fmt.Println("Detect failed: empty result pointer")
			continue
		}

		elapsedTime := time.Since(startTime)
		fmt.Printf("OCR time: %.2fms\n", float64(elapsedTime)/float64(time.Millisecond))

		result := C.GoString(resultPtr)
		C.call_free_result_buffer(freeResultBufferFunc, unsafe.Pointer(resultPtr))
		fmt.Println("Result:", result)
	}

	fmt.Println("Press Enter to exit...")
	bufio.NewScanner(os.Stdin).Scan()
}
