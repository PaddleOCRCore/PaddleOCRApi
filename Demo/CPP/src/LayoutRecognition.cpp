#include "LayoutRecognition.h"

#include <chrono>
#include <iostream>

#include <PaddleOCR.h>

#include "PaddleOCRCommon.h"

using namespace std;

bool RunLayoutRecognitionDemo(const vector<string>& images, const string& baseDirectory) {
    if (images.empty()) {
        cerr << "No image found for layout demo." << endl;
        return false;
    }

    string det_infer = CombinePath(baseDirectory, "models\\PP-OCRv6_tiny_det_infer");
    string rec_infer = CombinePath(baseDirectory, "models\\PP-OCRv6_tiny_rec_infer");
    string cls_infer = CombinePath(baseDirectory, "models\\PP-LCNet_x1_0_textline_ori");
    string layout_model_dir = CombinePath(baseDirectory, "models\\PP-DocLayoutV3_infer");
    string table_cls_model_dir = CombinePath(baseDirectory, "models\\PP-LCNet_x1_0_table_cls_infer");
    string wired_table_model_dir = CombinePath(baseDirectory, "models\\SLANeXt_wired_infer");
    string wireless_table_model_dir = CombinePath(baseDirectory, "models\\SLANeXt_wireless_infer");
    string wired_table_cell_det_model_dir = CombinePath(baseDirectory, "models\\RT-DETR-L_wired_table_cell_det_infer");
    string wireless_table_cell_det_model_dir = CombinePath(baseDirectory, "models\\RT-DETR-L_wireless_table_cell_det_infer");
    string doc_cls_infer = CombinePath(baseDirectory, "models\\PP-LCNet_x1_0_doc_ori_infer");
    string formula_model_dir = CombinePath(baseDirectory, "models\\LaTeX_OCR_rec_infer");
    string seal_model_dir = CombinePath(baseDirectory, "models\\PP-OCRv4_mobile_seal_det_infer");
    string doc_unwarp_model = CombinePath(baseDirectory, "models\\UVDoc_infer");
    string region_model_dir = CombinePath(baseDirectory, "models\\PP-DocBlockLayout");

    LayoutParameter layout_param;
    layout_param.use_gpu = false;
    layout_param.cpu_threads = 8;
    layout_param.enable_mkldnn = true;
    layout_param.use_table_recognition = true;
    layout_param.use_table_cells_detection = false;
    layout_param.use_formula_recognition = true;
    layout_param.use_seal_recognition = false;
    layout_param.output_markdown = true;
    layout_param.format_block_content = false;
    layout_param.visualize = false;

    EnableLog(false);
    EnableJsonResult(true);
    ActivateLicenseIfExists(baseDirectory);

    cout << "======= Layout =========" << endl;
    if (!InitStructure(
        det_infer.c_str(),
        cls_infer.c_str(),
        rec_infer.c_str(),
        layout_model_dir.c_str(),
        table_cls_model_dir.c_str(),
        wired_table_model_dir.c_str(),
        wireless_table_model_dir.c_str(),
        wired_table_cell_det_model_dir.c_str(),
        wireless_table_cell_det_model_dir.c_str(),
        formula_model_dir.c_str(),
        seal_model_dir.c_str(),
        doc_cls_infer.c_str(),
        doc_unwarp_model.c_str(),
        region_model_dir.c_str(),
        layout_param)) {
        cerr << "InitStructure failed: " << GetLastErrorAndFree() << endl;
        return false;
    }

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
    return true;
}
