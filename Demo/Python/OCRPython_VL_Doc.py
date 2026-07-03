"""
python_doc_analyser.py
======================
Python wrapper for llamaocr-vl.dll document analysis API.

Usage:
    python python_doc_analyser.py --config configs/PaddleOCR-VL-1.5.yaml <image_path>
"""

import argparse
import base64
import ctypes
import json
import os
import sys
import time


if os.name == "nt":
    try:
        ctypes.windll.kernel32.SetConsoleCP(65001)
        ctypes.windll.kernel32.SetConsoleOutputCP(65001)
    except Exception:
        pass

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
if hasattr(sys.stderr, "reconfigure"):
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")


SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
PROJECT_ROOT = SCRIPT_DIR

DLL_PATHS = [
    os.path.join(PROJECT_ROOT, "llamaocr-vl.dll"),
    os.path.join(PROJECT_ROOT, "runtimes", "win-x64", "native", "llamaocr-vl.dll"),
]
DEFAULT_PIPELINE_CONFIG = os.path.join(PROJECT_ROOT, "configs", "PaddleOCR-VL-1.5.yaml")
IMAGE_REL_PATH = os.path.join(PROJECT_ROOT, "images", "text.jpg")

def resolve_dll_path() -> str:
    for dll_path in DLL_PATHS:
        if os.path.exists(dll_path):
            return dll_path
    return DLL_PATHS[0]

def register_native_dll_directory(dll_dir: str) -> None:
    os.environ["PATH"] = dll_dir + os.pathsep + os.environ.get("PATH", "")
    if hasattr(os, "add_dll_directory"):
        os.add_dll_directory(dll_dir)
    if os.name == "nt":
        try:
            ctypes.windll.kernel32.SetDllDirectoryW(dll_dir)
        except Exception:
            pass

def load_dll(dll_path: str) -> ctypes.CDLL:
    if not os.path.exists(dll_path):
        raise FileNotFoundError(f"DLL not found: {dll_path}")

    dll_dir = os.path.dirname(os.path.abspath(dll_path))
    register_native_dll_directory(dll_dir)

    lib = ctypes.CDLL(dll_path)
    lib.InitDoc.restype = ctypes.c_int
    lib.InitDoc.argtypes = [ctypes.c_char_p]
    lib.DocChat.restype = ctypes.c_void_p
    lib.DocChat.argtypes = [ctypes.c_char_p]
    lib.DocChatBase64.restype = ctypes.c_void_p
    lib.DocChatBase64.argtypes = [ctypes.c_char_p]
    lib.FreeDocAnalyser.restype = None
    lib.FreeDocAnalyser.argtypes = []
    lib.FreeResultBuffer.restype = None
    lib.FreeResultBuffer.argtypes = [ctypes.c_void_p]
    lib.GetError.restype = ctypes.c_char_p
    lib.GetError.argtypes = []
    lib.ActivateLicense.restype = ctypes.c_bool
    lib.ActivateLicense.argtypes = [ctypes.c_char_p]
    lib.GetLicenseStatus.restype = ctypes.c_void_p
    lib.GetLicenseStatus.argtypes = []
    return lib


def _consume_heap_cstring(lib: ctypes.CDLL, ptr: int) -> str:
    if not ptr:
        return ""
    try:
        return ctypes.string_at(ptr).decode("utf-8", errors="replace")
    finally:
        lib.FreeResultBuffer(ctypes.c_void_p(ptr))


def activate_license(lib: ctypes.CDLL, license_file: str = "paddleocr.lic") -> bool:
    license_path = license_file if os.path.isabs(license_file) else os.path.join(PROJECT_ROOT, license_file)
    if not os.path.exists(license_path):
        print(f"[PY] License file not found, skip activation: {license_path}")
        return False
    ok = lib.ActivateLicense(license_path.encode("utf-8"))
    print("[PY] License activation success:" if ok else "[PY] License activation failed:", license_path)
    return bool(ok)


def print_license_status(lib: ctypes.CDLL) -> None:
    result_ptr = lib.GetLicenseStatus()
    if not result_ptr:
        print("[PY] GetLicenseStatus returned null")
        return
    print("[PY] License Status:")
    print(_consume_heap_cstring(lib, result_ptr))


def init_doc_analyser(lib: ctypes.CDLL, config_path: str) -> bool:
    if not config_path or not os.path.isfile(config_path):
        print(f"[PY] ERROR: config file not found: {config_path}")
        return False
    print(f"[PY] Initialising DocAnalyser from config path: {config_path}")
    rc = lib.InitDoc(config_path.encode("utf-8"))
    if rc == 0:
        err = lib.GetError()
        print(f"[PY] InitDoc FAILED: {err.decode() if err else 'unknown error'}")
        return False
    print("[PY] DocAnalyser initialised successfully.")
    return True


def analyse_image(lib: ctypes.CDLL, image_path: str) -> dict:
    raw_ptr = lib.DocChat(image_path.encode("utf-8"))
    if not raw_ptr:
        err = lib.GetError()
        raise RuntimeError(f"DocChat failed: {err.decode() if err else 'unknown'}")
    raw_text = _consume_heap_cstring(lib, raw_ptr)
    print(f"[PY] DocChat returned: {raw_text}")
    return raw_text


def analyse_image_base64(lib: ctypes.CDLL, image_path: str) -> dict:
    with open(image_path, "rb") as fh:
        b64 = base64.b64encode(fh.read()).decode()
    raw_ptr = lib.DocChatBase64(b64.encode("utf-8"))
    if not raw_ptr:
        err = lib.GetError()
        raise RuntimeError(f"DocChatBase64 failed: {err.decode() if err else 'unknown'}")
    raw_text = _consume_heap_cstring(lib, raw_ptr)
    return raw_text



def main() -> None:
    parser = argparse.ArgumentParser(description="Document layout analysis using llamaocr-vl.dll")
    parser.add_argument("--config", default=DEFAULT_PIPELINE_CONFIG, help="Pipeline config YAML path")
    parser.add_argument("image", nargs="?", default=IMAGE_REL_PATH, help="Image path to analyse")
    parser.add_argument("--dll", default=resolve_dll_path(), help="Path to llamaocr-vl.dll")
    parser.add_argument("--base64", action="store_true", help="Read image and send as Base64")
    args = parser.parse_args()

    if not os.path.isfile(args.image):
        print(f"[PY] ERROR: Image file not found: {args.image}")
        sys.exit(1)

    lib = load_dll(args.dll)
    print(f"[PY] Loaded DLL: {args.dll}")
    activate_license(lib)
    print_license_status(lib)
    if not init_doc_analyser(lib, args.config):
        sys.exit(1)

    try:
        for i in range(1):
            t0 = time.time()
            if args.base64:
                result = analyse_image_base64(lib, args.image)
            else:
                result = analyse_image(lib, args.image)
            elapsed_ms = (time.time() - t0) * 1000
            print("[PY] Analysis result:")
            print(result)
            print(f"\n图片识别耗时: {elapsed_ms:.2f}ms")
    finally:
        lib.FreeDocAnalyser()
        print("\n[PY] DocAnalyser released.")


if __name__ == "__main__":
    main()
