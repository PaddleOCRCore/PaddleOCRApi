import ctypes
import os
import time
import sys
from pathlib import Path
from typing import Optional


# Fixed runtime config (edit here if needed).
DLL_REL_PATHS = [
    "llamaocr-vl.dll",
    "runtimes/win-x64/native/llamaocr-vl.dll",
]
CONFIG_REL_PATH = "configs/PaddleOCR-VL-1.5.yaml"
IMAGE_REL_PATH = "images/text.jpg"
PROMPT_TEXT = "OCR:"


def _b(text: Optional[str]) -> Optional[bytes]:
    if text is None:
        return None
    return text.encode("utf-8")


def _decode_cstr(ptr: Optional[int]) -> str:
    if not ptr:
        return ""
    raw = ctypes.string_at(ptr)
    try:
        return raw.decode("utf-8")
    except UnicodeDecodeError:
        return raw.decode("gbk", errors="replace")


def _consume_heap_cstr(dll: ctypes.CDLL, ptr: Optional[int]) -> str:
    if not ptr:
        return ""
    try:
        return _decode_cstr(ptr)
    finally:
        dll.FreeResultBuffer(ptr)


def _activate_license(dll: ctypes.CDLL, root: Path, license_file: str = "paddleocr.lic") -> bool:
    license_path = Path(license_file)
    if not license_path.is_absolute():
        license_path = root / license_path
    if not license_path.exists():
        print(f"License file not found, skip activation: {license_path}")
        return False
    ok = dll.ActivateLicense(_b(str(license_path)))
    print("License activation success:" if ok else "License activation failed:", license_path)
    return bool(ok)


def _print_license_status(dll: ctypes.CDLL) -> None:
    result_ptr = dll.GetLicenseStatus()
    if not result_ptr:
        print("GetLicenseStatus returned null")
        return
    print("License Status:")
    print(_consume_heap_cstr(dll, result_ptr))


def _resolve_dll_path(root: Path) -> Path:
    for rel_path in DLL_REL_PATHS:
        dll_path = (root / rel_path).resolve()
        if dll_path.exists():
            return dll_path
    raise FileNotFoundError("DLL not found: " + " or ".join(str((root / p).resolve()) for p in DLL_REL_PATHS))


def _register_native_dll_directory(dll_dir: Path) -> None:
    dll_dir_text = str(dll_dir)
    os.environ["PATH"] = dll_dir_text + os.pathsep + os.environ.get("PATH", "")
    if hasattr(os, "add_dll_directory"):
        os.add_dll_directory(dll_dir_text)
    if os.name == "nt":
        try:
            ctypes.windll.kernel32.SetDllDirectoryW(dll_dir_text)
        except Exception:
            pass


def main() -> int:
    root = Path(__file__).resolve().parent
    try:
        dll_path = _resolve_dll_path(root)
    except FileNotFoundError as exc:
        print(f"[ERROR] {exc}", file=sys.stderr)
        return 1

    config_path = (root / CONFIG_REL_PATH).resolve()
    image_path = (root / IMAGE_REL_PATH).resolve()
    if not config_path.exists():
        print(f"[ERROR] Config not found: {config_path}", file=sys.stderr)
        return 1
    if not image_path.exists():
        print(f"[ERROR] Image not found: {image_path}", file=sys.stderr)
        return 1

    _register_native_dll_directory(dll_path.parent)

    dll = ctypes.CDLL(str(dll_path))

    dll.Init.argtypes = [ctypes.c_char_p]
    dll.Init.restype = ctypes.c_int

    dll.Chat.argtypes = [ctypes.c_char_p, ctypes.c_char_p]
    dll.Chat.restype = ctypes.c_void_p

    dll.FreeResultBuffer.argtypes = [ctypes.c_void_p]
    dll.FreeResultBuffer.restype = None

    dll.FreeEngine.argtypes = []
    dll.FreeEngine.restype = None

    dll.GetError.argtypes = []
    dll.GetError.restype = ctypes.c_void_p

    dll.ActivateLicense.argtypes = [ctypes.c_char_p]
    dll.ActivateLicense.restype = ctypes.c_bool

    dll.GetLicenseStatus.argtypes = []
    dll.GetLicenseStatus.restype = ctypes.c_void_p

    _activate_license(dll, root)
    _print_license_status(dll)

    ok = dll.Init(_b(str(config_path)))

    if ok != 1:
        err = _decode_cstr(dll.GetError())
        print(f"[ERROR] Init failed: {err}", file=sys.stderr)
        return 1

    result_ptr = None
    try:
        start_time = time.time()
        result_ptr = dll.Chat(_b(PROMPT_TEXT), _b(str(image_path)))
        if not result_ptr:
            err = _decode_cstr(dll.GetError())
            print(f"[ERROR] Chat failed: {err}", file=sys.stderr)
            return 1

        text = _decode_cstr(result_ptr)
        print("="*20 + " OCR Result Start" + "="*20)
        print(text)
        print("="*20 + " OCR Result End" + "="*20)
        # 计算识别时间
        elapsed_time = time.time() - start_time
        print(f"OCR-VL耗时: {elapsed_time * 1000:.2f}ms")
        return 0
    finally:
        if result_ptr:
            dll.FreeResultBuffer(result_ptr)
        dll.FreeEngine()


if __name__ == "__main__":
    raise SystemExit(main())

