import com.sun.jna.Native;
import com.sun.jna.win32.StdCallLibrary;
import java.io.File;
import java.util.Scanner;

/**
 * PaddleOCR Java 调用示例
 * 需要引入 JNA 库 (jna-jpms.jar 和 jna-platform-jpms.jar)
 */
public class OCRJavaDemo {

    // 定义 DLL 接口，对应 PaddleOCR.dll 中的导出函数
    public interface PaddleOCR extends StdCallLibrary {
        // 加载 PaddleOCR.dll
        PaddleOCR INSTANCE = Native.load("PaddleOCR.dll", PaddleOCR.class);

        // 开启/关闭日志
        void EnableLog(boolean useLog);
        
        // 设置返回结果格式 (true: JSON, false: String)
        void EnableJsonResult(boolean enable);
        
        // 初始化引擎 (JSON 配置方式)
        boolean Initjson(String det_infer, String cls_infer, String rec_infer, String parameterjson);
        
        // 识别图片
        String Detect(String imageFile);
        
        // 释放引擎
        void FreeEngine();
    }

    public static void main(String[] args) {
        try {
            // 获取当前工作目录
            String rootDir = System.getProperty("user.dir");
            
            // 模型路径 (请根据实际存放位置修改)
            String detModel = rootDir + "\\models\\PP-OCRv5_mobile_det_infer";
            String clsModel = rootDir + "\\models\\PP-LCNet_x1_0_textline_ori";
            String recModel = rootDir + "\\models\\PP-OCRv5_mobile_rec_infer";
            
            // 初始化参数 JSON
            String configJson = "{" +
                    "\"use_gpu\": false," +
                    "\"return_word_box\": false," +
                    "\"cpu_math_library_num_threads\": 10," +
                    "\"gpu_id\": 0," +
                    "\"gpu_mem\": 4000," +
                    "\"cpu_mem\": 4000," +
                    "\"enable_mkldnn\": true," +
                    "\"rec_img_h\": 48," +
                    "\"rec_img_w\": 320," +
                    "\"cls\": false," +
                    "\"det\": true," +
                    "\"use_angle_cls\": false," +
                    "\"visualize\": true" +
                    "}";

            System.out.println("=== PaddleOCR Java Demo ===");
            System.out.println("正在初始化 OCR 引擎...");
            
            // 初始化
            boolean inited = PaddleOCR.INSTANCE.Initjson(detModel, clsModel, recModel, configJson);
            
            if (!inited) {
                System.err.println("OCR 初始化失败！请确认以下事项：");
                System.err.println("1. PaddleOCR.dll 及其依赖项在系统路径或当前目录下");
                System.err.println("2. 模型路径正确: " + detModel);
                return;
            }

            // 设置返回格式为纯文本
            PaddleOCR.INSTANCE.EnableJsonResult(false);

            // 遍历 images 目录下的图片
            File imageDir = new File(rootDir + "\\images");
            if (imageDir.exists() && imageDir.isDirectory()) {
                File[] images = imageDir.listFiles((dir, name) -> {
                    String lower = name.toLowerCase();
                    return lower.endsWith(".jpg") || lower.endsWith(".png") || lower.endsWith(".bmp");
                });

                if (images != null && images.length > 0) {
                    for (File img : images) {
                        System.out.println("\n处理图片: " + img.getName());
                        long startTime = System.currentTimeMillis();
                        
                        // 执行 OCR 识别
                        String result = PaddleOCR.INSTANCE.Detect(img.getAbsolutePath());
                        
                        long endTime = System.currentTimeMillis();
                        System.out.println("OCR 耗时: " + (endTime - startTime) + "ms");
                        System.out.println("识别内容: \n" + result);
                    }
                } else {
                    System.out.println("在 images 目录下未找到图片文件。");
                }
            } else {
                System.err.println("未找到图片目录: " + imageDir.getAbsolutePath());
            }

            System.out.println("\n程序运行完毕，按回车键退出...");
            new Scanner(System.in).nextLine();

            // 释放引擎
            // PaddleOCR.INSTANCE.FreeEngine();

        } catch (UnsatisfiedLinkError e) {
            System.err.println("无法加载 DLL: " + e.getMessage());
            System.err.println("请确保 PaddleOCR.dll 及其依赖库在当前目录或 PATH 中。");
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
