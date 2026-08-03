from __future__ import annotations

import os
import sys
from dataclasses import dataclass, field
from pathlib import Path
from typing import Any


TEXT_MODELS = {
    "PP-OCRv6 tiny": ("PP-OCRv6_tiny_det_infer", "PP-OCRv6_tiny_rec_infer"),
    "PP-OCRv6 small": ("PP-OCRv6_small_det_infer", "PP-OCRv6_small_rec_infer"),
    "PP-OCRv6 medium": ("PP-OCRv6_medium_det_infer", "PP-OCRv6_medium_rec_infer"),
    "PP-OCRv5 mobile": ("PP-OCRv5_mobile_det_infer", "PP-OCRv5_mobile_rec_infer"),
    "PP-OCRv5 server": ("PP-OCRv5_server_det_infer", "PP-OCRv5_server_rec_infer"),
    "PP-OCRv4 mobile": ("PP-OCRv4_mobile_det_infer", "PP-OCRv4_mobile_rec_infer"),
}

LAYOUT_MODELS = {
    "PP-DocLayoutV3 infer": "PP-DocLayoutV3_infer",
    "PP-DocLayoutV2 infer": "PP-DocLayoutV2_infer",
    "PP-DocLayout plus-L infer": "PP-DocLayout_plus-L_infer",
    "PP-DocBlockLayout infer": "PP-DocBlockLayout_infer",
}

STRUCTURE_MODELS = {
    "cls": "PP-LCNet_x1_0_textline_ori",
    "table_cls": "PP-LCNet_x1_0_table_cls_infer",
    "wired_table": "SLANeXt_wired_infer",
    "wireless_table": "SLANeXt_wireless_infer",
    "wired_cell": "RT-DETR-L_wired_table_cell_det_infer",
    "wireless_cell": "RT-DETR-L_wireless_table_cell_det_infer",
    "formula": "LaTeX_OCR_rec_infer",
    "seal": "PP-OCRv4_mobile_seal_det_infer",
    "doc_cls": "PP-LCNet_x1_0_doc_ori_infer",
    "doc_unwarp": "UVDoc_infer",
    "region": "PP-DocBlockLayout",
}


def application_root() -> Path:
    if getattr(sys, "frozen", False):
        return Path(sys.executable).resolve().parent
    return Path(__file__).resolve().parents[1]


@dataclass(frozen=True)
class AppPaths:
    root: Path = field(default_factory=application_root)

    @property
    def models(self) -> Path:
        return self.root / "models"

    @property
    def native(self) -> Path:
        return self.root / "runtimes" / "win-x64" / "native"

    @property
    def dll(self) -> Path:
        return self.native / "PaddleOCR.dll"

    @property
    def output(self) -> Path:
        return self.root / "output"

    @property
    def uploads(self) -> Path:
        return self.root / "uploads"

    @property
    def license(self) -> Path:
        return self.models / "paddleocr.lic"

    def prepare_writable_dirs(self) -> None:
        self.output.mkdir(parents=True, exist_ok=True)
        self.uploads.mkdir(parents=True, exist_ok=True)


@dataclass(frozen=True)
class EngineOptions:
    text_model: str = "PP-OCRv6 tiny"
    layout_model: str = "PP-DocLayoutV3 infer"
    use_layout: bool = True
    use_gpu: bool = False
    use_tensorrt: bool = False
    cpu_threads: int = min(30, max(1, os.cpu_count() or 1))
    cpu_mem: int = 0
    gpu_id: int = 0
    gpu_mem: int = 4000
    instance_count: int = 1
    return_word_box: bool = False

    def ocr_parameters(self) -> dict[str, Any]:
        return {
            "use_gpu": self.use_gpu,
            "use_tensorrt": self.use_tensorrt if self.use_gpu else False,
            "gpu_id": self.gpu_id,
            "gpu_mem": self.gpu_mem,
            "cpu_mem": self.cpu_mem,
            "cpu_threads": self.cpu_threads,
            "enable_mkldnn": True,
            "rec_batch_num": 6,
            "cls": True,
            "det": True,
            "use_angle_cls": True,
            "det_db_score_mode": False,
            "max_side_len": 960,
            "rec_img_h": 48,
            "rec_img_w": 320,
            "det_db_thresh": 0.3,
            "det_db_box_thresh": 0.5,
            "det_db_unclip_ratio": 1.6,
            "visualize": True,
            "ocr_instance_count": self.instance_count,
            "return_word_box": self.return_word_box,
        }

    def layout_parameters(self) -> dict[str, Any]:
        return {
            "use_gpu": self.use_gpu,
            "use_tensorrt": self.use_tensorrt if self.use_gpu else False,
            "gpu_id": self.gpu_id,
            "gpu_mem": self.gpu_mem,
            "cpu_mem": self.cpu_mem,
            "cpu_threads": self.cpu_threads,
            "enable_mkldnn": True,
            "visualize": True,
            "use_doc_preprocessor": True,
            "use_doc_orientation_classify": True,
            "use_doc_unwarping": False,
            "use_layout_detection": True,
            "use_region_detection": False,
            "layout_nms": True,
            "layout_unclip_ratio_w": 1.0,
            "layout_unclip_ratio_h": 1.0,
            "run_ocr_after_layout": True,
            "text_det_thresh": 0.3,
            "text_rec_score_thresh": 0.5,
            "use_textline_orientation": True,
            "max_side_len": 960,
            "use_table_recognition": True,
            "use_table_cells_detection": False,
            "use_seal_recognition": True,
            "use_formula_recognition": True,
            "format_block_content": False,
            "output_markdown": True,
        }