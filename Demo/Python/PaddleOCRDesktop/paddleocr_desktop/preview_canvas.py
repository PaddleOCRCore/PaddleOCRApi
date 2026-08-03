from __future__ import annotations

import tkinter as tk
from typing import Iterable

from PIL import Image, ImageTk

from .result_parser import RecognitionBox


class PreviewCanvas(tk.Canvas):
    def __init__(self, master: tk.Misc, **kwargs) -> None:
        super().__init__(master, background="#f4f5f6", highlightthickness=0, **kwargs)
        self._image: Image.Image | None = None
        self._photo: ImageTk.PhotoImage | None = None
        self._boxes: tuple[RecognitionBox, ...] = ()
        self._zoom = 1.0
        self._offset = [0.0, 0.0]
        self._drag_origin: tuple[int, int] | None = None
        self.bind("<Configure>", lambda _event: self._draw())
        self.bind("<MouseWheel>", self._on_wheel)
        self.bind("<ButtonPress-1>", self._on_press)
        self.bind("<B1-Motion>", self._on_drag)
        self.bind("<Double-Button-1>", lambda _event: self.reset_view())

    def set_content(self, image: Image.Image, boxes: Iterable[RecognitionBox] = ()) -> None:
        self._image = image.copy()
        self._boxes = tuple(boxes)
        self.reset_view()

    def set_boxes(self, boxes: Iterable[RecognitionBox]) -> None:
        self._boxes = tuple(boxes)
        self._draw()

    def reset_view(self) -> None:
        self._zoom = 1.0
        self._offset = [0.0, 0.0]
        self._draw()

    def _fit_scale(self) -> float:
        if self._image is None:
            return 1.0
        width = max(1, self.winfo_width() - 20)
        height = max(1, self.winfo_height() - 20)
        return min(width / self._image.width, height / self._image.height)

    def _draw(self) -> None:
        self.delete("all")
        if self._image is None:
            self.create_text(
                max(1, self.winfo_width()) / 2,
                max(1, self.winfo_height()) / 2,
                text="请选择图片、PDF 或读取剪贴板",
                fill="#687078",
                font=("Microsoft YaHei UI", 11),
            )
            return
        scale = self._fit_scale() * self._zoom
        width = max(1, round(self._image.width * scale))
        height = max(1, round(self._image.height * scale))
        rendered = self._image.resize((width, height), Image.Resampling.LANCZOS)
        self._photo = ImageTk.PhotoImage(rendered)
        origin_x = (self.winfo_width() - width) / 2 + self._offset[0]
        origin_y = (self.winfo_height() - height) / 2 + self._offset[1]
        self.create_image(origin_x, origin_y, image=self._photo, anchor="nw")
        for box in self._boxes:
            if len(box.points) < 2:
                continue
            coords = [coordinate for x, y in box.points for coordinate in (origin_x + x * scale, origin_y + y * scale)]
            self.create_polygon(*coords, outline="#e33d3d", fill="", width=2)

    def _on_wheel(self, event: tk.Event) -> None:
        self._zoom = min(8.0, max(0.2, self._zoom * (1.12 if event.delta > 0 else 0.89)))
        self._draw()

    def _on_press(self, event: tk.Event) -> None:
        self._drag_origin = (event.x, event.y)

    def _on_drag(self, event: tk.Event) -> None:
        if self._drag_origin is None:
            return
        self._offset[0] += event.x - self._drag_origin[0]
        self._offset[1] += event.y - self._drag_origin[1]
        self._drag_origin = (event.x, event.y)
        self._draw()