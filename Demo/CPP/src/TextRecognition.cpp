#include "TextRecognition.h"

#include <chrono>
#include <iostream>

#include <PaddleOCR.h>

#include "PaddleOCRCommon.h"

using namespace std;

bool RunTextRecognitionDemo(const vector<string>& images, const string& baseDirectory) {
    if (images.empty()) {
        cerr << "No image found for OCR demo." << endl;
        return false;
    }

    string det_infer = CombinePath(baseDirectory, "models\\PP-OCRv6_tiny_det_infer");
    string rec_infer = CombinePath(baseDirectory, "models\\PP-OCRv6_tiny_rec_infer");
    string cls_infer = CombinePath(baseDirectory, "models\\PP-LCNet_x1_0_textline_ori");

    OCRParameter ocr_param;
    ocr_param.use_gpu = false;
    ocr_param.cpu_threads = 8;
    ocr_param.enable_mkldnn = true;
    ocr_param.det = true;
    ocr_param.rec = true;
    ocr_param.cls = false;
    ocr_param.use_angle_cls = false;
    ocr_param.visualize = true;
    ocr_param.ocr_instance_count = 1;

    EnableJsonResult(true);
    ActivateLicenseIfExists(baseDirectory);

    if (!Init(det_infer.c_str(), cls_infer.c_str(), rec_infer.c_str(), ocr_param)) {
        cerr << "Init OCR failed: " << GetLastErrorAndFree() << endl;
        return false;
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

    FreeEngine();
    return true;
}
