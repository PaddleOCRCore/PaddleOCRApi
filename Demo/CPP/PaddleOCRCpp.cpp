#include <iostream>
#include <string>
#include <vector>
#include <chrono>
#include <windows.h>

#include <PaddleOCR.h>

using namespace std;

void GetFileList(const string& directoryPath, vector<string>& files) {
    WIN32_FIND_DATAA ffd;
    HANDLE hFind = INVALID_HANDLE_VALUE;
    string pattern = directoryPath + "/*";

    hFind = FindFirstFileA(pattern.c_str(), &ffd);
    if (hFind == INVALID_HANDLE_VALUE) {
        cerr << "FindFirstFile failed (" << GetLastError() << "): " << directoryPath << endl;
        return;
    }

    do {
        string name = ffd.cFileName;
        if ((ffd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0) {
            if (name != "." && name != "..") {
                GetFileList(directoryPath + "\\" + name, files);
            }
        } else {
            files.push_back(directoryPath + "\\" + name);
        }
    } while (FindNextFileA(hFind, &ffd) != 0);

    FindClose(hFind);
}

static string TakeResultAndFree(const char* buffer) {
    if (buffer == nullptr) {
        return "";
    }

    string text(buffer);
    FreeResultBuffer(const_cast<void*>(static_cast<const void*>(buffer)));
    return text;
}

static string GetLastErrorAndFree() {
    char* err = GetError();
    if (err == nullptr) {
        return "";
    }

    string text(err);
    FreeResultBuffer(err);
    return text;
}

int main() {
    SetConsoleOutputCP(CP_UTF8);

    char cwd[MAX_PATH] = { 0 };
    GetCurrentDirectoryA(MAX_PATH, cwd);

    string det_infer = "models/PP-OCRv5_mobile_det_infer";
    string rec_infer = "models/PP-OCRv5_mobile_rec_infer";
    string cls_infer = "models/PP-LCNet_x1_0_textline_ori";

    string layout_model_dir = "models/PP-DocLayoutV2_infer";
    string table_model_dir = "models/PP-SLANet_plus_infer";
    string doc_cls_infer = "PP-LCNet_x1_0_doc_ori_infer";//文档图像方向分类模块
    string formula_model_dir = "LaTeX_OCR_rec_infer";//公式识别模型
    string seal_model_dir = "PP-OCRv4_mobile_seal_det_infer";//印章检测模型  
    string doc_unwarp_model = "UVDoc_infer";//文档矫正模型
    string region_model_dir = "PP-DocBlockLayout_infer";

    OCRParameter ocr_param;
    ocr_param.use_gpu = false;
    ocr_param.cpu_threads = 8;
    ocr_param.enable_mkldnn = true;
    ocr_param.det = true;
    ocr_param.rec = true;
    ocr_param.cls = false;
    ocr_param.use_angle_cls = false;
    ocr_param.visualize = false;
    ocr_param.ocr_instance_count = 1;

    LayoutParameter layout_param;
    layout_param.use_gpu = false;
    layout_param.cpu_threads = 8;
    layout_param.enable_mkldnn = true;
    layout_param.use_table_recognition = true;
    layout_param.use_formula_recognition = true;
    layout_param.use_seal_recognition = false;
    layout_param.output_markdown = true;
    layout_param.format_block_content = false;

    string imagespath(cwd);
    imagespath += "\\images";

    vector<string> images;
    GetFileList(imagespath, images);
    if (images.empty()) {
        cerr << "No image found under: " << imagespath << endl;
        system("pause");
        return 1;
    }

    EnableJsonResult(true);

    if (!Init(det_infer.c_str(), cls_infer.c_str(), rec_infer.c_str(), ocr_param)) {
        cerr << "Init OCR failed: " << GetLastErrorAndFree() << endl;
        system("pause");
        return 1;
    }

    cout << "========== OCR ==========" << endl;
    for (const auto& image : images) {
        cout << "Image: " << image << endl;
        auto start = chrono::steady_clock::now();

        cv::Mat imgMat = cv::imread(image, cv::IMREAD_COLOR);
        const char* raw = nullptr;
        if (!imgMat.empty()) {
            raw = DetectMat(imgMat);
        } else {
            raw = Detect(image.c_str());
        }

        auto end = chrono::steady_clock::now();
        auto duration = chrono::duration_cast<chrono::milliseconds>(end - start);
        cout << "Detect: " << duration.count() << "ms" << endl;

        if (raw == nullptr) {
            cerr << "Detect failed: " << GetLastErrorAndFree() << endl;
            continue;
        }

        cout << TakeResultAndFree(raw) << endl << endl;
    }

    cout << "======= Layout =========" << endl;
    if (!InitStructure(
        det_infer.c_str(),
        cls_infer.c_str(),
        rec_infer.c_str(),
        layout_model_dir.c_str(),
        table_model_dir.c_str(),
        formula_model_dir.c_str(),  // formula_model_dir
        seal_model_dir.c_str(),  // seal_model_dir
        doc_cls_infer.c_str(),  // doc_cls_infer
        doc_unwarp_model.c_str(),  // doc_unwarp_model
        region_model_dir.c_str(),  // region_model_dir
        layout_param)) {
        cerr << "InitStructure failed: " << GetLastErrorAndFree() << endl;
    } else {
        for (const auto& image : images) {
            cout << "Layout Image: " << image << endl;
            auto start = chrono::steady_clock::now();
            const char* raw = DetectLayout(image.c_str());
            auto end = chrono::steady_clock::now();
            auto duration = chrono::duration_cast<chrono::milliseconds>(end - start);
            cout << "DetectLayout: " << duration.count() << "ms" << endl;

            if (raw == nullptr) {
                cerr << "DetectLayout failed: " << GetLastErrorAndFree() << endl;
                continue;
            }

            cout << TakeResultAndFree(raw) << endl << endl;
        }

        FreeStructureEngine();
    }

    FreeEngine();
    system("pause");
    return 0;
}
