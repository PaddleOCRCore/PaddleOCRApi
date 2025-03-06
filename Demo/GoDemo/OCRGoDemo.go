package main

import (
	"bufio"
	"fmt"
	"io/ioutil"
	"os"
	"syscall"
	"time"
	"unsafe"
)

// 获取字符串的指针
func stringToPointer(s string) uintptr {
	return uintptr(unsafe.Pointer(syscall.StringBytePtr(s)))
}

// 获取当前工作目录的绝对路径
func getCurrentDirectory() string {
	dir, err := os.Getwd()
	if err != nil {
		fmt.Println("获取当前目录失败:", err)
	}
	return dir
}

func main() {
	// 加载DLL文件
	ocrDLL, err := syscall.LoadDLL("PaddleOCR.dll")
	if err != nil {
		fmt.Println("加载DLL失败:", err)
		return
	}

	// 获取DLL中的函数
	initFunc, err := ocrDLL.FindProc("Initjson")
	if err != nil {
		fmt.Println("获取Initjson函数失败:", err)
		return
	}
	detectFunc, err := ocrDLL.FindProc("Detect")
	if err != nil {
		fmt.Println("获取Detect函数失败:", err)
		return
	}
	enableJsonFunc, err := ocrDLL.FindProc("EnableJsonResult")
	if err != nil {
		fmt.Println("获取EnableJsonResult函数失败:", err)
		return
	}

	// 初始化OCR
	rootDir := getCurrentDirectory()
	initFunc.Call(
		stringToPointer(rootDir+"\\models\\ch_PP-OCRv4_det_infer"),
		stringToPointer(rootDir+"\\models\\ch_ppocr_mobile_v2.0_cls_infer"),
		stringToPointer(rootDir+"\\models\\ch_PP-OCRv4_rec_infer"),
		stringToPointer(rootDir+"\\models\\ppocr_keys.txt"),
		stringToPointer("{\"use_gpu\": false,\"cpu_math_library_num_threads\": 30,\"gpu_id\": 0,\"gpu_mem\": 4000,\"cpu_mem\": 4000,\"enable_mkldnn\": true,\"rec_img_h\": 48,\"rec_img_w\": 320,\"cls\":false,\"det\":true,\"use_angle_cls\":false,\"visualize\":true}"),
	)

	// 设置返回结果格式
	enableJsonFunc.Call(0) // 0: 返回纯字符串结果, 1: 返回JSON字符串结果

	// 读取图片目录
	imageDir := rootDir + "\\images"
	images, err := ioutil.ReadDir(imageDir)
	if err != nil {
		fmt.Println("读取图片目录失败:", err)
		return
	}

	// 处理每张图片
	for _, image := range images {
		fmt.Println("处理图片:", image.Name())
		startTime := time.Now()

		// 调用OCR检测函数
		imagePath := imageDir + "\\" + image.Name()
		cWString, _, _ := detectFunc.Call(stringToPointer(imagePath))

		// 计算识别时间
		elapsedTime := time.Since(startTime)
		fmt.Printf("OCR耗时: %.2fms\n", float64(elapsedTime)/float64(time.Millisecond))

		// 将返回的wchar_t指针转换为Go字符串
		wstr := (*[1 << 30]uint16)(unsafe.Pointer(cWString))
		var result string
		for i := 0; wstr[i] != 0; i++ {
			result += string(rune(wstr[i]))
		}
		fmt.Println("识别结果:", result)
	}

	// 等待用户输入以退出程序
	fmt.Println("按回车键退出...")
	bufio.NewScanner(os.Stdin).Scan()
}
