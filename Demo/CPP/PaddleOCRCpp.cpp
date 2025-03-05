#include <iostream>
#include <string.h>
#include <windows.h>
#include <PaddleOCR.h>
#include <locale>
#include <codecvt>
#include <string>
#include <filesystem>
#include <vector>
#include <chrono>
using namespace std;

void GetFileList(string directoryPath, vector<string>& files)
{
    WIN32_FIND_DATA ffd;
    HANDLE hFind = INVALID_HANDLE_VALUE;
    string pt;
    // 打开目录句柄
    hFind = FindFirstFile(pt.assign(directoryPath).append("/*").c_str(), &ffd);
    if (INVALID_HANDLE_VALUE == hFind)
    {
        cerr << "FindFirstFile failed (" << GetLastError() << ")" << endl;
    }
    // 遍历目录
    do
    {
        if (ffd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
        {
            // 跳过"."和".."目录
            if (strcmp(ffd.cFileName, ".") != 0 && strcmp(ffd.cFileName, "..") != 0)
            {
                GetFileList(pt.assign(directoryPath).append("\\").append(ffd.cFileName), files);
            }
            else
            {
                continue;
            }
        }
        else
        {
            files.push_back(pt.assign(directoryPath).append("\\") + ffd.cFileName);
        }
    } while (FindNextFile(hFind, &ffd) != 0);
    // 关闭目录句柄
    FindClose(hFind);
}
string ConvertWStringToString(const wstring& wstr) {
    wstring_convert<codecvt_utf8<wchar_t>> converter;
    return converter.to_bytes(wstr);
}
int main()
{
    SetConsoleOutputCP(CP_UTF8);//解决控制台中文乱码，使用UTF-8编码
    char path[MAX_PATH];
    GetCurrentDirectoryA(MAX_PATH, path);
    string det_infer(path);
    //请将PaddleOCRSDK项目中PaddleOCRRuntime下面所有文件复制到c++的生成Release运行目录
    det_infer += "/models/ch_PP-OCRv4_det_infer";
    string rec_infer(path);
    rec_infer += "/models/ch_PP-OCRv4_rec_infer";
    string cls_infer(path);
    cls_infer += "/models/ch_ppocr_mobile_v2.0_cls_infer";
    string keys(path);
    keys += "/models/ppocr_keys.txt";
    OCRParameter parameter;
    parameter.use_gpu = false;
    parameter.cpu_threads = 30;
    parameter.cpu_mem = 4000;
    parameter.enable_mkldnn = true;
    parameter.cls = false;
    parameter.det = true;
    parameter.use_angle_cls = false;
    parameter.det_db_score_mode = true;
    parameter.max_side_len = 960;
    parameter.rec_img_h = 48;
    parameter.rec_img_w = 320;
    parameter.visualize = true;

    string imagespath(path);
    imagespath += "\\images";//请将图片放至此目录
    vector<string> images;
    GetFileList(imagespath, images);
    EnableJsonResult(false);

    Init(const_cast<char*>(det_infer.c_str()), const_cast<char*>(cls_infer.c_str()), 
        const_cast<char*>(rec_infer.c_str()), const_cast<char*>(keys.c_str()), parameter);
    for (const auto image : images) 
    {
        cout << "images:" << image << endl;
        auto	starttime = chrono::steady_clock::now();
        string cstr = ConvertWStringToString(Detect(const_cast<char*>(image.c_str())));
        auto	endtime = chrono::steady_clock::now();
        auto duration = chrono::duration_cast<chrono::milliseconds>(endtime - starttime);
        std::cout << "Detect:" << duration.count() << "ms" << endl;
        cout << cstr << endl;
    }
    try
    {
        FreeEngine();
    }
    catch (const exception& e)
    {
        wcout << e.what();
    }
    system("pause");
    return 0;
}
