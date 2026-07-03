import ctypes
import os
import time

# 获取当前工作目录的绝对路径
def get_current_directory():
    return os.getcwd()

def resolve_dll_path(root_dir, dll_name):
    candidates = [
        os.path.join(root_dir, dll_name),
        os.path.join(root_dir, "runtimes", "win-x64", "native", dll_name),
    ]
    for candidate in candidates:
        if os.path.exists(candidate):
            return candidate
    raise FileNotFoundError("DLL not found: " + " or ".join(candidates))

def register_native_dll_directory(dll_dir):
    os.environ["PATH"] = dll_dir + os.pathsep + os.environ.get("PATH", "")
    if hasattr(os, "add_dll_directory"):
        os.add_dll_directory(dll_dir)
    if os.name == "nt":
        try:
            ctypes.windll.kernel32.SetDllDirectoryW(dll_dir)
        except Exception:
            pass

# 加载DLL文件
root_dir = get_current_directory()
dll_path = resolve_dll_path(root_dir, "PaddleOCR.dll")
register_native_dll_directory(os.path.dirname(dll_path))
ocr_dll = ctypes.CDLL(dll_path)

# 定义DLL中的函数
findimage_func = ocr_dll.FindImage
findimage_func.argtypes = [ctypes.c_char_p, ctypes.c_char_p, ctypes.c_double, ctypes.c_bool, ctypes.c_bool]
findimage_func.restype = ctypes.c_void_p
free_result_buffer_func = ocr_dll.FreeResultBuffer
free_result_buffer_func.argtypes = [ctypes.c_void_p]
free_result_buffer_func.restype = None

image_dir = "images"
start_time = time.time()
# 大图
bigimage_path = image_dir + "\\bg.png"
# 小图
smallimage_path = image_dir + "\\tg.png"
# 匹配阈值 [0, 1]，默认0.8
threshold = 0.2
#是否转换为灰度图进行匹配，默认true
toGray = True
#是否使用滑块专用匹配（边缘检测），默认false
useSlideMatch = True
print("以图找图:", bigimage_path)
findimage_func.restype = ctypes.c_void_p
result_ptr = findimage_func(ctypes.c_char_p(bigimage_path.encode('utf-8')),ctypes.c_char_p(smallimage_path.encode('utf-8')),threshold,toGray,useSlideMatch)
c_string = ctypes.string_at(result_ptr).decode('utf-8')
free_result_buffer_func(result_ptr)
# 计算识别时间
elapsed_time = time.time() - start_time
print(f"耗时: {elapsed_time * 1000:.2f}ms")
# 检查返回值是否为空指针
if c_string:
	print("找图结果:", c_string)
else:
	print("找图失败，返回空指针。")
# 等待用户输入以退出程序
input("按回车键退出...")
