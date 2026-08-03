from __future__ import annotations

from dataclasses import dataclass
from io import BytesIO
from pathlib import Path

from PIL import Image, ImageGrab


SUPPORTED_IMAGES = {".png", ".jpg", ".jpeg", ".bmp", ".tif", ".tiff", ".webp"}


@dataclass
class ImageInput:
    image: Image.Image
    png_bytes: bytes
    display_name: str


def _from_image(image: Image.Image, display_name: str) -> ImageInput:
    owned = image.convert("RGB") if image.mode not in ("RGB", "RGBA") else image.copy()
    stream = BytesIO()
    owned.save(stream, format="PNG")
    return ImageInput(owned, stream.getvalue(), display_name)


def load_image(path: str | Path) -> ImageInput:
    source = Path(path)
    if source.suffix.lower() not in SUPPORTED_IMAGES:
        raise ValueError(f"不支持的图片格式：{source.suffix}")
    with Image.open(source) as image:
        return _from_image(image, source.name)


def load_pdf_first_page(path: str | Path, dpi: int = 200, max_pixels: int = 50_000_000) -> ImageInput:
    import fitz

    source = Path(path)
    with fitz.open(source) as document:
        if document.page_count < 1:
            raise ValueError("PDF 没有可识别页面")
        page = document.load_page(0)
        matrix = fitz.Matrix(dpi / 72.0, dpi / 72.0)
        pixmap = page.get_pixmap(matrix=matrix, alpha=False)
        if pixmap.width * pixmap.height > max_pixels:
            raise ValueError("PDF 首页像素过大，请降低 DPI")
        with Image.open(BytesIO(pixmap.tobytes("png"))) as image:
            return _from_image(image, f"{source.stem}_page1.png")


def load_clipboard() -> ImageInput:
    value = ImageGrab.grabclipboard()
    if isinstance(value, Image.Image):
        return _from_image(value, "clipboard.png")
    if isinstance(value, list) and len(value) == 1:
        path = Path(value[0])
        if path.suffix.lower() == ".pdf":
            return load_pdf_first_page(path)
        return load_image(path)
    raise ValueError("剪贴板中没有图片或单个受支持文件")