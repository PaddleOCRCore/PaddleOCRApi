import ctypes
import os
import time
import base64
try:
    import cv2
    HAS_CV2 = True
except ImportError:
    HAS_CV2 = False
    print("cv2 not available, skipping DetectMat tests")

# 获取当前工作目录的绝对路径
def get_current_directory():
    return os.getcwd()

# 安全的Detect函数调用，自动处理内存释放
def call_detect_function(func, free_result_func, *args):
    """
    安全调用Detect函数，使用DLL提供的FreeResultBuffer释放内存（跨平台）
    
    Args:
        func: DLL中的Detect函数
        free_result_func: DLL中的FreeResultBuffer函数
        *args: 函数参数
        
    Returns:
        str: 识别结果字符串
    """
    try:
        # 设置返回类型为指针
        func.restype = ctypes.c_void_p
        result_ptr = func(*args)
        #print(f"DEBUG: func call returned ptr: {result_ptr}")  # 调试输出
        
        if result_ptr:
            try:
                # 从指针读取字符串
                result_str = ctypes.cast(result_ptr, ctypes.c_char_p).value.decode('utf-8')
                print(f"DEBUG: decoded string length: {len(result_str)}")  # 调试输出
            finally:
                # 使用DLL提供的FreeResultBuffer释放内存（跨平台支持）
                free_result_func(result_ptr)
                #print("DEBUG: memory freed")  # 调试输出
            return result_str
        else:
            print("DEBUG: result_ptr is None or empty")  # 调试输出
            return ""
    except Exception as e:
        print(f"DEBUG: Exception in call_detect_function: {e}")  # 调试输出
        return ""

# 加载DLL文件
root_dir = get_current_directory()
dll_path = os.path.join(root_dir, "PaddleOCR.dll")
ocr_dll = ctypes.CDLL(dll_path)

# 定义DLL中的函数
init_func = ocr_dll.Initjson
detect_func = ocr_dll.Detect
detect_mat_func = ocr_dll.DetectMat if HAS_CV2 else None
detect_byte_func = ocr_dll.DetectByte
detect_base64_func = ocr_dll.DetectBase64
detect_table_func = ocr_dll.DetectTable
detect_table_byte_func = ocr_dll.DetectTableByte
detect_table_base64_func = ocr_dll.DetectTableBase64
init_table_func = ocr_dll.InitTablejson
enable_json_func = ocr_dll.EnableJsonResult
enable_log_func = ocr_dll.EnableLog
free_engine_func = ocr_dll.FreeEngine
freetable_engine_func = ocr_dll.FreeTableEngine

# 跨平台内存释放函数（替代Windows特有的CoTaskMemFree）
free_result_buffer_func = ocr_dll.FreeResultBuffer
free_result_buffer_func.argtypes = [ctypes.c_void_p]
free_result_buffer_func.restype = None

# 设置返回结果格式
enable_json_func(0)  # 0: 返回纯字符串结果, 1: 返回JSON字符串结果
enable_log_func(1)  # 0: 不输出日志, 1: 输出日志

# 初始化OCR
root_dir = get_current_directory()
init_func(
    ctypes.c_char_p(("models\\PP-OCRv5_mobile_det_infer").encode('utf-8')),
    ctypes.c_char_p(("models\\PP-LCNet_x1_0_textline_ori").encode('utf-8')),
    ctypes.c_char_p(("models\\PP-OCRv5_mobile_rec_infer").encode('utf-8')),
    ctypes.c_char_p(b'{"use_gpu": false,"return_word_box":false,"cpu_threads": 30,"gpu_id": 0,"gpu_mem": 4000,"cpu_mem": 0,"enable_mkldnn": true,"rec_img_h": 48,"rec_img_w": 320,"cls":true,"det":true,"use_angle_cls":true}')
)
# 初始化Table OCR
init_table_func(
    ctypes.c_char_p(("models\\PP-OCRv5_mobile_det_infer").encode('utf-8')),
    ctypes.c_char_p(("models\\PP-LCNet_x1_0_textline_ori").encode('utf-8')),
    ctypes.c_char_p(("models\\PP-OCRv5_mobile_rec_infer").encode('utf-8')),
    ctypes.c_char_p(("models\\PP-SLANet_plus_infer").encode('utf-8')),
    ctypes.c_char_p(b'{"use_gpu": false,"cpu_threads": 30,"gpu_id": 0,"gpu_mem": 4000,"cpu_mem": 0,"enable_mkldnn": true,"rec_img_h": 48,"rec_img_w": 320,"cls":true,"det":true,"use_angle_cls":true}')
)

# 读取图片目录
image_dir = root_dir + "\\images"
images = os.listdir(image_dir)

# 选择第一张图片进行测试
if images:
    image_name = images[0]
    image_path = os.path.join(image_dir, image_name)
    
    # 读取图片数据
    with open(image_path, 'rb') as f:
        image_bytes = f.read()
    image_size = len(image_bytes)
    
    # 转换为base64
    image_base64 = base64.b64encode(image_bytes).decode('utf-8')
    
    # 测试每个Detect函数10次
    functions_to_test = [
        ("Detect", detect_func, lambda: ctypes.c_char_p(image_path.encode('utf-8'))),
        ("DetectByte", detect_byte_func, lambda: (ctypes.c_char_p(image_bytes), ctypes.c_size_t(image_size))),
        ("DetectBase64", detect_base64_func, lambda: ctypes.c_char_p(image_base64.encode('utf-8'))),
        ("DetectTable", detect_table_func, lambda: ctypes.c_char_p(image_path.encode('utf-8'))),
        ("DetectTableByte", detect_table_byte_func, lambda: (ctypes.c_char_p(image_bytes), ctypes.c_size_t(image_size))),
        ("DetectTableBase64", detect_table_base64_func, lambda: ctypes.c_char_p(image_base64.encode('utf-8'))),
    ]
    totaltimes=10
    for func_name, func, arg_func in functions_to_test:
        print(f"\n测试 {func_name} 函数 {totaltimes} 次:")
        total_time = 0
        for i in range(totaltimes):
            start_time = time.time()
            args = arg_func()
            if isinstance(args, tuple):
                result = call_detect_function(func, free_result_buffer_func, *args)
            else:
                result = call_detect_function(func, free_result_buffer_func, args)
            elapsed_time = time.time() - start_time
            total_time += elapsed_time
			
            print(f"\n{func_name}第{i+1}次: {result} \n 耗时: {elapsed_time * 1000:.2f}ms")
                
        avg_time = total_time / totaltimes
        print(f"\n{func_name} 平均耗时: {avg_time * 1000:.2f}ms")
else:
    print("没有找到图片文件")
free_engine_func()
freetable_engine_func()
input("按回车键退出...")