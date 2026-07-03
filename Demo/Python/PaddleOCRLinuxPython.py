import os
import ctypes
from ctypes import *
import json
from datetime import datetime
import time

def get_runtime_root():
    return os.path.dirname(os.path.abspath(__file__))

def resolve_library_path(root_dir):
    candidates = [
        os.path.join(root_dir, "PaddleOCR.so"),
        os.path.join(root_dir, "runtimes", "linux-x64", "native", "PaddleOCR.so"),
        os.path.join(root_dir, "runtimes", "linux-x64", "native", "libPaddleOCR.so"),
        os.path.join(os.getcwd(), "PaddleOCR.so"),
        os.path.join(os.getcwd(), "runtimes", "linux-x64", "native", "PaddleOCR.so"),
        os.path.join(os.getcwd(), "runtimes", "linux-x64", "native", "libPaddleOCR.so"),
    ]
    for candidate in candidates:
        if os.path.exists(candidate):
            return os.path.abspath(candidate)
    raise FileNotFoundError("PaddleOCR Linux library not found: " + " or ".join(candidates))

def register_native_library_directory(lib_dir):
    os.environ["LD_LIBRARY_PATH"] = lib_dir + os.pathsep + os.environ.get("LD_LIBRARY_PATH", "")

runtime_root = get_runtime_root()
lib_path = resolve_library_path(runtime_root)
register_native_library_directory(os.path.dirname(lib_path))
ocr_dll = ctypes.CDLL(lib_path, mode=ctypes.RTLD_GLOBAL)

init_func = ocr_dll.Initjson
detect_func = ocr_dll.Detect
enable_json_func = ocr_dll.EnableJsonResult
enablelog_func = ocr_dll.EnableLog
free_engine_func = ocr_dll.FreeEngine
activate_license_func = ocr_dll.ActivateLicense
activate_license_func.argtypes = [ctypes.c_char_p]
activate_license_func.restype = ctypes.c_bool
get_license_status_func = ocr_dll.GetLicenseStatus
get_license_status_func.argtypes = []
get_license_status_func.restype = ctypes.c_void_p

# 跨平台内存释放函数（替代Windows特有的CoTaskMemFree）
free_result_buffer_func = ocr_dll.FreeResultBuffer
free_result_buffer_func.argtypes = [ctypes.c_void_p]
free_result_buffer_func.restype = None

def activate_license(license_file="paddleocr.lic"):
    license_path = license_file if os.path.isabs(license_file) else os.path.join(runtime_root, license_file)
    if not os.path.exists(license_path):
        print("License file not found, skip activation:", license_path)
        return False
    ok = activate_license_func(ctypes.c_char_p(license_path.encode("utf-8")))
    print("License activation success:" if ok else "License activation failed:", license_path)
    return ok

def print_license_status():
    result_ptr = get_license_status_func()
    if not result_ptr:
        print("GetLicenseStatus returned null")
        return
    try:
        status = ctypes.string_at(result_ptr).decode("utf-8", errors="replace")
    finally:
        free_result_buffer_func(result_ptr)
    print("License Status:")
    print(status)

free_engine_func()
# 初始化OCR
root_dir = runtime_root + "/"
activate_license()
print_license_status()
init_func(
    ctypes.c_char_p((root_dir + "models/PP-OCRv6_tiny_det_infer").encode('utf-8')),
    ctypes.c_char_p((root_dir + "models/PP-LCNet_x1_0_textline_ori").encode('utf-8')),
    ctypes.c_char_p((root_dir + "models/PP-OCRv6_tiny_rec_infer").encode('utf-8')),
    ctypes.c_char_p(b'{"use_gpu": false,"ocr_instance_count":3,"return_word_box":false,"cpu_threads": 30,"gpu_id": 0,"gpu_mem": 4000,"cpu_mem": 0,"enable_mkldnn": true,"rec_img_h": 48,"rec_img_w": 320,"cls":true,"det":true,"use_angle_cls":true}')
)

# 设置返回结果格式
enable_json_func(0)  # 0: 返回纯字符串结果, 1: 返回JSON字符串结果
enablelog_func(1)
# 读取图片目录
image_dir = root_dir + "images"
images = os.listdir(image_dir)

# 处理每张图片
for image_name in images:
    for i in range(10):
        start_time = time.time()
        # 调用OCR检测函数
        image_path = image_dir + "/" + image_name
        detect_func.restype = ctypes.c_void_p
        result_ptr = detect_func(ctypes.c_char_p(image_path.encode('utf-8')))
        c_string = ctypes.string_at(result_ptr).decode('utf-8')
        free_result_buffer_func(result_ptr)
        # 计算识别时间
        elapsed_time = time.time() - start_time
        print(f"OCR耗时: {elapsed_time * 1000:.2f}ms")
        # 检查返回值是否为空指针
        if c_string:
            print("识别结果:", c_string)
        else:
            print("识别失败，返回空指针。")
free_engine_func()
# 等待用户输入以退出程序
input("按回车键退出...")
   
