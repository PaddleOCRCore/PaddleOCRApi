import ctypes
import os
import time

# 获取当前工作目录的绝对路径
def get_current_directory():
    return os.getcwd()

# 加载DLL文件
root_dir = get_current_directory()
dll_path = os.path.join(root_dir, "PaddleOCR.dll")
ocr_dll = ctypes.CDLL(dll_path)

# 定义DLL中的函数
findimage_func = ocr_dll.FindImage

# 跨平台内存释放函数（替代Windows特有的CoTaskMemFree）
free_result_buffer_func = ocr_dll.FreeResultBuffer
free_result_buffer_func.argtypes = [ctypes.c_void_p]
free_result_buffer_func.restype = None

image_dir = ".\\"
start_time = time.time()
# 大图
bigimage_path = image_dir + "\\bgg.png"
# 小图
smallimage_path = image_dir + "\\tgg.png"
# 匹配阈值 [0, 1]，默认0.8
threshold = 0.8
#是否转换为灰度图进行匹配，默认true
toGray = true
print("以图找图:", bigimage_path)
findimage_func.restype = ctypes.c_void_p
result_ptr = findimage_func(ctypes.c_char_p(bigimage_path.encode('utf-8')),ctypes.c_char_p(smallimage_path.encode('utf-8')),threshold,toGray)
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