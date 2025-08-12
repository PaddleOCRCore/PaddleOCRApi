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
init_func = ocr_dll.Initjson
detect_func = ocr_dll.Detect
enable_json_func = ocr_dll.EnableJsonResult
enable_log_func = ocr_dll.EnableLog

# 初始化OCR
root_dir = get_current_directory()
init_func(
    ctypes.c_char_p((root_dir + "\\models\\ch_PP-OCRv4_det_infer").encode('utf-8')),
    ctypes.c_char_p((root_dir + "\\models\\ch_ppocr_mobile_v2.0_cls_infer").encode('utf-8')),
    ctypes.c_char_p((root_dir + "\\models\\ch_PP-OCRv4_rec_infer").encode('utf-8')),
    ctypes.c_char_p((root_dir + "\\models\\ppocr_keys.txt").encode('utf-8')),
    ctypes.c_char_p(b'{"use_gpu": false,"cpu_threads": 30,"gpu_id": 0,"gpu_mem": 4000,"cpu_mem": 0,"enable_mkldnn": true,"rec_img_h": 48,"rec_img_w": 320,"cls":false,"det":true,"use_angle_cls":false}')
)

# 设置返回结果格式
enable_json_func(0)  # 0: 返回纯字符串结果, 1: 返回JSON字符串结果
enable_log_func(1)  # 0: 不输出日志, 1: 输出日志
# 读取图片目录
image_dir = root_dir + "\\images"
images = os.listdir(image_dir)

# 处理每张图片
for image_name in images:
    print("处理图片:", image_name)
    start_time = time.time()

    # 调用OCR检测函数
    image_path = image_dir + "\\" + image_name
    detect_func.restype = ctypes.c_char_p
    c_string = detect_func(ctypes.c_char_p(image_path.encode('utf-8'))).decode('utf-8')
    # 计算识别时间
    elapsed_time = time.time() - start_time
    print(f"OCR耗时: {elapsed_time * 1000:.2f}ms")
    # 检查返回值是否为空指针
    if c_string:
        print("识别结果:", c_string)
    else:
        print("识别失败，返回空指针。")

# 等待用户输入以退出程序
input("按回车键退出...")