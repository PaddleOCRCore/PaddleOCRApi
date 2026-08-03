from __future__ import annotations

import ctypes
import json
import os
from pathlib import Path
from typing import Any

from .config import AppPaths, EngineOptions, LAYOUT_MODELS, STRUCTURE_MODELS, TEXT_MODELS


class NativeOcrError(RuntimeError):
    pass


class NativeOcr:
    def __init__(self, paths: AppPaths | None = None, dll: Any | None = None) -> None:
        self.paths = paths or AppPaths()
        self._dll_directory = None
        self._dll = dll or self._load_dll()
        self.text_ready = False
        self.layout_ready = False
        self._declare_signatures()

    def _load_dll(self) -> Any:
        if os.name != "nt":
            raise NativeOcrError("本程序仅支持 Windows x64")
        if not self.paths.dll.exists():
            raise NativeOcrError(f"未找到 PaddleOCR.dll：{self.paths.dll}")
        os.environ["PATH"] = str(self.paths.native) + os.pathsep + os.environ.get("PATH", "")
        if hasattr(os, "add_dll_directory"):
            self._dll_directory = os.add_dll_directory(str(self.paths.native))
        return ctypes.CDLL(str(self.paths.dll))

    def _declare_signatures(self) -> None:
        c_char_p = ctypes.c_char_p
        c_void_p = ctypes.c_void_p
        self._dll.Initjson.argtypes = [c_char_p, c_char_p, c_char_p, c_char_p]
        self._dll.Initjson.restype = ctypes.c_bool
        self._dll.InitStructurejson.argtypes = [c_char_p] * 15
        self._dll.InitStructurejson.restype = ctypes.c_bool
        self._detect_text_function = "DetectPtr" if hasattr(self._dll, "DetectPtr") else "DetectByte"
        self._detect_layout_function = "DetectLayoutPtr" if hasattr(self._dll, "DetectLayoutPtr") else "DetectLayoutByte"
        for name in (self._detect_text_function, self._detect_layout_function):
            function = getattr(self._dll, name)
            size_type = ctypes.c_int if name.endswith("Ptr") else ctypes.c_size_t
            function.argtypes = [c_void_p, size_type]
            function.restype = c_void_p
        for name in ("GetError", "GetLicenseStatus"):
            function = getattr(self._dll, name)
            function.argtypes = []
            function.restype = c_void_p
        self._dll.ActivateLicense.argtypes = [c_char_p]
        self._dll.ActivateLicense.restype = ctypes.c_bool
        self._dll.FreeResultBuffer.argtypes = [c_void_p]
        self._dll.FreeResultBuffer.restype = None
        for name in ("EnableLog", "EnableJsonResult", "EnableASCIIResult"):
            function = getattr(self._dll, name)
            function.argtypes = [ctypes.c_bool]
            function.restype = None
        for name in ("FreeEngine", "FreeStructureEngine"):
            function = getattr(self._dll, name)
            function.argtypes = []
            function.restype = ctypes.c_int

    @staticmethod
    def _utf8(value: str | Path) -> bytes:
        return str(value).encode("utf-8")

    def _consume(self, pointer: int | None) -> str:
        if not pointer:
            return ""
        try:
            return ctypes.string_at(pointer).decode("utf-8", errors="replace")
        finally:
            self._dll.FreeResultBuffer(pointer)

    def error_message(self) -> str:
        return self._consume(self._dll.GetError()) or "PaddleOCR 返回未知错误"

    def _model_path(self, name: str) -> Path:
        return self.paths.models / name

    def _require_models(self, names: list[str]) -> None:
        missing = [str(self._model_path(name)) for name in names if not self._model_path(name).is_dir()]
        if missing:
            raise NativeOcrError("缺少模型目录：\n" + "\n".join(missing))

    def _activate_gpu_license(self, options: EngineOptions) -> None:
        if not options.use_gpu:
            return
        if not self.paths.license.is_file():
            raise NativeOcrError(f"GPU 模式需要授权文件：{self.paths.license}")
        if not self._dll.ActivateLicense(self._utf8(self.paths.license)):
            raise NativeOcrError("GPU 授权文件激活失败")

    def initialize(self, options: EngineOptions) -> None:
        self.paths.prepare_writable_dirs()
        self._activate_gpu_license(options)
        self._dll.EnableLog(True)
        self._dll.EnableJsonResult(True)
        self._dll.EnableASCIIResult(False)
        detection, recognition = TEXT_MODELS[options.text_model]
        text_names = [detection, STRUCTURE_MODELS["cls"], recognition]
        self._require_models(text_names)
        result = self._dll.Initjson(
            *(self._utf8(self._model_path(name)) for name in text_names),
            self._utf8(json.dumps(options.ocr_parameters(), ensure_ascii=False)),
        )
        if not result:
            raise NativeOcrError(self.error_message())
        self.text_ready = True
        if options.use_layout:
            self._initialize_layout(options, detection, recognition)

    def _initialize_layout(self, options: EngineOptions, detection: str, recognition: str) -> None:
        names = [
            detection,
            STRUCTURE_MODELS["cls"],
            recognition,
            LAYOUT_MODELS[options.layout_model],
            STRUCTURE_MODELS["table_cls"],
            STRUCTURE_MODELS["wired_table"],
            STRUCTURE_MODELS["wireless_table"],
            STRUCTURE_MODELS["wired_cell"],
            STRUCTURE_MODELS["wireless_cell"],
            STRUCTURE_MODELS["formula"],
            STRUCTURE_MODELS["seal"],
            STRUCTURE_MODELS["doc_cls"],
            STRUCTURE_MODELS["doc_unwarp"],
            STRUCTURE_MODELS["region"],
        ]
        self._require_models(names)
        result = self._dll.InitStructurejson(
            *(self._utf8(self._model_path(name)) for name in names),
            self._utf8(json.dumps(options.layout_parameters(), ensure_ascii=False)),
        )
        if not result:
            raise NativeOcrError(self.error_message())
        self.layout_ready = True

    def _detect(self, function_name: str, image_bytes: bytes) -> str:
        if not image_bytes:
            raise ValueError("图片数据为空")
        if len(image_bytes) > 2_147_483_647:
            raise ValueError("图片数据超过原生接口长度限制")
        buffer = (ctypes.c_ubyte * len(image_bytes)).from_buffer_copy(image_bytes)
        pointer = getattr(self._dll, function_name)(ctypes.cast(buffer, ctypes.c_void_p), len(image_bytes))
        result = self._consume(pointer)
        if not result:
            raise NativeOcrError(self.error_message())
        return result

    def detect_text(self, image_bytes: bytes) -> str:
        if not self.text_ready:
            raise NativeOcrError("文本识别引擎尚未初始化")
        return self._detect(self._detect_text_function, image_bytes)

    def detect_layout(self, image_bytes: bytes) -> str:
        if not self.layout_ready:
            raise NativeOcrError("版面识别引擎尚未初始化")
        return self._detect(self._detect_layout_function, image_bytes)

    def close(self) -> None:
        if self.layout_ready:
            self._dll.FreeStructureEngine()
            self.layout_ready = False
        if self.text_ready:
            self._dll.FreeEngine()
            self.text_ready = False
        if self._dll_directory is not None:
            self._dll_directory.close()
            self._dll_directory = None