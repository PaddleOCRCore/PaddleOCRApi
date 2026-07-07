#include <iostream>
#include <string>
#include <vector>
#include <windows.h>

#include "LayoutRecognition.h"
#include "PaddleOCRCommon.h"
#include "TextRecognition.h"

using namespace std;

/// <summary>
/// 输出Demo命令行参数说明。默认同时运行普通文本识别和版面识别。
/// </summary>
/// <param name="exeName">当前可执行文件名称。</param>
static void PrintUsage(const char* exeName) {
    cout << "Usage: " << exeName << " [ocr|layout|all]" << endl;
}

/// <summary>
/// C++ Demo入口：准备运行目录、加载图片，并按参数调度普通文本识别或版面识别Demo。
/// </summary>
/// <param name="argc">命令行参数数量。</param>
/// <param name="argv">命令行参数数组，支持ocr、layout、all。</param>
/// <returns>所有选中的Demo运行成功返回0，否则返回1。</returns>
int main(int argc, char* argv[]) {
    SetConsoleOutputCP(CP_UTF8);

    char cwd[MAX_PATH] = { 0 };
    GetCurrentDirectoryA(MAX_PATH, cwd);
    string baseDirectory(cwd);
    ConfigureNativeDllDirectory(baseDirectory);

    string mode = argc > 1 ? argv[1] : "all";
    if (mode != "ocr" && mode != "layout" && mode != "all") {
        PrintUsage(argv[0]);
        system("pause");
        return 1;
    }

    string imagesPath = CombinePath(baseDirectory, "images");
    vector<string> images;
    GetFileList(imagesPath, images);
    if (images.empty()) {
        cerr << "No image found under: " << imagesPath << endl;
        system("pause");
        return 1;
    }

    bool success = true;
    if (mode == "ocr" || mode == "all") {
        success = RunTextRecognitionDemo(images, baseDirectory) && success;
    }

    if (mode == "layout" || mode == "all") {
        success = RunLayoutRecognitionDemo(images, baseDirectory) && success;
    }

    system("pause");
    return success ? 0 : 1;
}
