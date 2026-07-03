import com.sun.jna.Library;
import com.sun.jna.Native;
import com.sun.jna.NativeLibrary;
import com.sun.jna.Pointer;
import com.sun.jna.win32.StdCallLibrary;
import com.sun.jna.win32.W32APIOptions;

import java.io.File;
import java.util.Arrays;
import java.util.Iterator;
import java.util.LinkedHashSet;
import java.util.Scanner;
import java.util.Set;

public class OCRJavaDemo {

    private interface Kernel32 extends StdCallLibrary {
        Kernel32 INSTANCE = Native.load("kernel32", Kernel32.class, W32APIOptions.UNICODE);

        boolean SetDllDirectory(String path);
    }

    public interface PaddleOCR extends Library {
        PaddleOCR INSTANCE = Native.load(configureNativeRuntime(), PaddleOCR.class);

        void EnableLog(boolean useLog);

        void EnableJsonResult(boolean enable);

        boolean Initjson(String detInfer, String clsInfer, String recInfer, String parameterJson);

        Pointer Detect(String imageFile);

        void FreeResultBuffer(Pointer resultPtr);

        void FreeEngine();
    }

    private static boolean isWindows() {
        return System.getProperty("os.name").toLowerCase().contains("win");
    }

    private static boolean isLinux() {
        return System.getProperty("os.name").toLowerCase().contains("linux");
    }

    private static String currentRid() {
        String arch = System.getProperty("os.arch").toLowerCase();
        boolean x64 = arch.equals("amd64") || arch.equals("x86_64");
        if (isWindows() && x64) {
            return "win-x64";
        }
        if (isLinux() && x64) {
            return "linux-x64";
        }
        return System.getProperty("os.name").toLowerCase().replaceAll("[^a-z0-9]+", "-") + "-" + arch;
    }

    private static String nativeLibraryFileName() {
        return isWindows() ? "PaddleOCR.dll" : "PaddleOCR.so";
    }

    private static File executableDirectory() {
        try {
            return new File(OCRJavaDemo.class.getProtectionDomain().getCodeSource().getLocation().toURI()).getParentFile();
        } catch (Exception ignored) {
            return new File(System.getProperty("user.dir"));
        }
    }

    private static String configureNativeRuntime() {
        String rootDir = System.getProperty("user.dir");
        String rid = currentRid();
        String fileName = nativeLibraryFileName();
        File exeDir = executableDirectory();

        File[] candidates = new File[] {
                new File(rootDir, "runtimes" + File.separator + rid + File.separator + "native" + File.separator + fileName),
                new File(exeDir, "runtimes" + File.separator + rid + File.separator + "native" + File.separator + fileName),
                new File(rootDir, fileName),
                new File(exeDir, fileName),
                new File(rootDir, "lib" + File.separator + fileName)
        };

        for (File candidate : candidates) {
            if (candidate.exists()) {
                File nativeDir = candidate.getParentFile();
                String nativeDirPath = nativeDir.getAbsolutePath();

                NativeLibrary.addSearchPath("PaddleOCR", nativeDirPath);
                String oldJnaPath = System.getProperty("jna.library.path", "");
                if (!oldJnaPath.contains(nativeDirPath)) {
                    System.setProperty("jna.library.path",
                            nativeDirPath + File.pathSeparator + oldJnaPath);
                }

                if (isWindows()) {
                    try {
                        Kernel32.INSTANCE.SetDllDirectory(nativeDirPath);
                    } catch (Throwable ignored) {
                        // Absolute loading still works for PaddleOCR.dll; SetDllDirectory helps its dependencies.
                    }
                } else if (isLinux()) {
                    preloadLinuxDependencies(nativeDir, candidate.getName());
                }

                return candidate.getAbsolutePath();
            }
        }

        throw new UnsatisfiedLinkError(
                "Cannot find " + fileName + " under current directory, executable directory, or runtimes/"
                        + rid + "/native");
    }

    private static void preloadLinuxDependencies(File nativeDir, String entryLibraryName) {
        File[] libraries = nativeDir.listFiles((dir, name) -> !name.equals(entryLibraryName) && name.contains(".so"));
        if (libraries == null || libraries.length == 0) {
            return;
        }

        Arrays.sort(libraries, (left, right) -> left.getName().compareToIgnoreCase(right.getName()));
        Set<File> pending = new LinkedHashSet<>(Arrays.asList(libraries));
        while (!pending.isEmpty()) {
            boolean progress = false;
            Iterator<File> iterator = pending.iterator();
            while (iterator.hasNext()) {
                File library = iterator.next();
                try {
                    System.load(library.getAbsolutePath());
                    iterator.remove();
                    progress = true;
                } catch (UnsatisfiedLinkError ignored) {
                    // Some libraries depend on others in this directory; retry after more preloads succeed.
                }
            }
            if (!progress) {
                return;
            }
        }
    }

    private static String modelPath(String rootDir, String name) {
        return new File(new File(rootDir, "models"), name).getAbsolutePath();
    }

    public static void main(String[] args) {
        try {
            String rootDir = System.getProperty("user.dir");

            String detModel = modelPath(rootDir, "PP-OCRv6_tiny_det_infer");
            String clsModel = modelPath(rootDir, "PP-LCNet_x1_0_textline_ori");
            String recModel = modelPath(rootDir, "PP-OCRv6_tiny_rec_infer");

            String configJson = "{"
                    + "\"use_gpu\": false,"
                    + "\"return_word_box\": false,"
                    + "\"cpu_math_library_num_threads\": 10,"
                    + "\"gpu_id\": 0,"
                    + "\"gpu_mem\": 4000,"
                    + "\"cpu_mem\": 4000,"
                    + "\"enable_mkldnn\": true,"
                    + "\"rec_img_h\": 48,"
                    + "\"rec_img_w\": 320,"
                    + "\"cls\": false,"
                    + "\"det\": true,"
                    + "\"use_angle_cls\": false,"
                    + "\"visualize\": true"
                    + "}";

            System.out.println("=== PaddleOCR Java Demo ===");
            System.out.println("Initializing OCR engine...");

            boolean inited = PaddleOCR.INSTANCE.Initjson(detModel, clsModel, recModel, configJson);
            if (!inited) {
                System.err.println("OCR initialization failed.");
                System.err.println("1. Check native dependencies under runtimes/" + currentRid() + "/native");
                System.err.println("2. Check model path: " + detModel);
                return;
            }

            PaddleOCR.INSTANCE.EnableJsonResult(false);

            File imageDir = new File(rootDir, "images");
            if (imageDir.exists() && imageDir.isDirectory()) {
                File[] images = imageDir.listFiles((dir, name) -> {
                    String lower = name.toLowerCase();
                    return lower.endsWith(".jpg") || lower.endsWith(".png") || lower.endsWith(".bmp");
                });

                if (images != null && images.length > 0) {
                    for (File image : images) {
                        System.out.println("\nImage: " + image.getName());
                        long startTime = System.currentTimeMillis();

                        Pointer resultPtr = PaddleOCR.INSTANCE.Detect(image.getAbsolutePath());
                        String result = "";
                        if (resultPtr != null) {
                            result = resultPtr.getString(0, "UTF-8");
                            PaddleOCR.INSTANCE.FreeResultBuffer(resultPtr);
                        }

                        long endTime = System.currentTimeMillis();
                        System.out.println("OCR time: " + (endTime - startTime) + "ms");
                        if (!result.isEmpty()) {
                            System.out.println("Result:\n" + result);
                        } else {
                            System.out.println("Detect failed: empty result pointer.");
                        }
                    }
                } else {
                    System.out.println("No image files found under: " + imageDir.getAbsolutePath());
                }
            } else {
                System.err.println("Image directory not found: " + imageDir.getAbsolutePath());
            }

            System.out.println("\nPress Enter to exit...");
            new Scanner(System.in).nextLine();
        } catch (UnsatisfiedLinkError e) {
            System.err.println("Cannot load native library: " + e.getMessage());
            System.err.println("Expected path: runtimes/" + currentRid() + "/native/" + nativeLibraryFileName());
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
