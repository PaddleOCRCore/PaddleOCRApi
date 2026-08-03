from __future__ import annotations

import tkinter as tk
from concurrent.futures import ThreadPoolExecutor
from pathlib import Path
from tkinter import filedialog, messagebox, ttk
from typing import Callable

from .config import AppPaths, EngineOptions, LAYOUT_MODELS, TEXT_MODELS
from .inputs import ImageInput, load_clipboard, load_image, load_pdf_first_page
from .native_ocr import NativeOcr
from .preview_canvas import PreviewCanvas
from .result_parser import RecognitionResult, parse_layout_result, parse_text_result


class MainWindow(ttk.Frame):
    def __init__(self, master: tk.Tk) -> None:
        super().__init__(master, padding=6)
        self.master = master
        self.paths = AppPaths()
        self.executor = ThreadPoolExecutor(max_workers=1, thread_name_prefix="paddleocr")
        self.native: NativeOcr | None = None
        self.current_input: ImageInput | None = None
        self.current_result: RecognitionResult | None = None
        self.busy = False
        self._closing = False
        self._build_variables()
        self._build_ui()
        self.pack(fill="both", expand=True)
        self.master.protocol("WM_DELETE_WINDOW", self.close)
        self._refresh_buttons()

    def _build_variables(self) -> None:
        self.text_model = tk.StringVar(value=next(iter(TEXT_MODELS)))
        self.layout_model = tk.StringVar(value=next(iter(LAYOUT_MODELS)))
        self.use_layout = tk.BooleanVar(value=False)
        self.use_gpu = tk.BooleanVar(value=False)
        self.use_tensorrt = tk.BooleanVar(value=False)
        self.return_word_box = tk.BooleanVar(value=False)
        self.cpu_threads = tk.IntVar(value=30)
        self.cpu_mem = tk.IntVar(value=0)
        self.gpu_id = tk.IntVar(value=0)
        self.gpu_mem = tk.IntVar(value=4000)
        self.instance_count = tk.IntVar(value=1)
        self.repeat_count = tk.IntVar(value=1)
        self.status = tk.StringVar(value="状态：就绪")

    def _build_ui(self) -> None:
        self.master.title("PaddleOCR 识别 Demo V4.5.3.0")
        self.master.geometry("1000x620")
        self.master.minsize(1000, 620)
        style = ttk.Style(self.master)
        if "vista" in style.theme_names():
            style.theme_use("vista")

        options = ttk.LabelFrame(self, text="OCR 文本识别 / 版面识别", padding=8)
        options.pack(fill="x")
        actions = ttk.Frame(options)
        actions.pack(side="right", fill="y", padx=(14, 0))
        parameters = ttk.Frame(options)
        parameters.pack(side="left", anchor="nw")

        ttk.Checkbutton(parameters, text="启用 GPU", variable=self.use_gpu, command=self._refresh_buttons).grid(row=0, column=0, sticky="w")
        self.tensorrt_check = ttk.Checkbutton(parameters, text="TensorRT", variable=self.use_tensorrt)
        self.tensorrt_check.grid(row=0, column=1, sticky="w")
        ttk.Label(parameters, text="CPU 线程").grid(row=0, column=2, sticky="e")
        ttk.Spinbox(parameters, from_=1, to=128, textvariable=self.cpu_threads, width=7).grid(row=0, column=3, sticky="w")
        ttk.Label(parameters, text="CPU 内存").grid(row=0, column=4, sticky="e")
        ttk.Spinbox(parameters, from_=-1, to=65536, textvariable=self.cpu_mem, width=8).grid(row=0, column=5, sticky="w")
        ttk.Label(parameters, text="GPU ID").grid(row=0, column=6, sticky="e")
        ttk.Spinbox(parameters, from_=0, to=16, textvariable=self.gpu_id, width=6).grid(row=0, column=7, sticky="w")

        ttk.Checkbutton(parameters, text="生成单字坐标", variable=self.return_word_box).grid(row=1, column=0, columnspan=2, sticky="w")
        ttk.Label(parameters, text="实例数").grid(row=1, column=2, sticky="e")
        ttk.Spinbox(parameters, from_=1, to=10, textvariable=self.instance_count, width=7).grid(row=1, column=3, sticky="w")
        ttk.Label(parameters, text="循环次数").grid(row=1, column=4, sticky="e")
        ttk.Spinbox(parameters, from_=1, to=100, textvariable=self.repeat_count, width=7).grid(row=1, column=5, sticky="w")
        ttk.Label(parameters, text="GPU 显存").grid(row=1, column=6, sticky="e")
        ttk.Spinbox(parameters, from_=256, to=65536, textvariable=self.gpu_mem, width=8).grid(row=1, column=7, sticky="w")
        ttk.Checkbutton(parameters, text="启用版面识别", variable=self.use_layout).grid(row=1, column=8, columnspan=2, sticky="w")

        ttk.Label(parameters, text="识别方案").grid(row=2, column=0, sticky="e", pady=(5, 0))
        self.text_combo = ttk.Combobox(parameters, textvariable=self.text_model, values=list(TEXT_MODELS), state="readonly", width=22)
        self.text_combo.grid(row=2, column=1, columnspan=3, sticky="ew", padx=3, pady=(5, 0))
        ttk.Label(parameters, text="版面模型").grid(row=2, column=4, sticky="e", pady=(5, 0))
        self.layout_combo = ttk.Combobox(parameters, textvariable=self.layout_model, values=list(LAYOUT_MODELS), state="readonly", width=27)
        self.layout_combo.grid(row=2, column=5, columnspan=5, sticky="ew", padx=3, pady=(5, 0))

        for column in range(3):
            actions.columnconfigure(column, weight=1, uniform="ocr_action")
        self.init_button = ttk.Button(actions, text="初始化 OCR", command=self.initialize)
        self.init_button.grid(row=0, column=0, sticky="ew", padx=3, pady=3)
        self.free_button = ttk.Button(actions, text="释放 OCR", command=self.release)
        self.free_button.grid(row=1, column=0, sticky="ew", padx=3, pady=3)
        self.text_button = ttk.Button(actions, text="OCR 文本识别", command=self.recognize_text_file)
        self.text_button.grid(row=0, column=1, sticky="ew", padx=3, pady=3)
        self.clipboard_button = ttk.Button(actions, text="剪贴板识别", command=self.recognize_clipboard)
        self.clipboard_button.grid(row=1, column=1, sticky="ew", padx=3, pady=3)
        self.layout_button = ttk.Button(actions, text="OCR 版面识别", command=self.recognize_layout_file)
        self.layout_button.grid(row=0, column=2, sticky="ew", padx=3, pady=3)
        self.pdf_button = ttk.Button(actions, text="PDF 版面识别", command=self.recognize_pdf)
        self.pdf_button.grid(row=1, column=2, sticky="ew", padx=3, pady=3)

        self.body = ttk.Panedwindow(self, orient="horizontal")
        self.body.pack(fill="both", expand=True, pady=(6, 0))
        preview_frame = ttk.LabelFrame(self.body, text="原始图片", padding=3)
        result_frame = ttk.LabelFrame(self.body, text="结果预览", padding=3)
        self.body.add(preview_frame, weight=1)
        self.body.add(result_frame, weight=1)
        self.body.bind("<Configure>", self._balance_body)
        self.preview = PreviewCanvas(preview_frame)
        self.preview.pack(fill="both", expand=True)
        self.notebook = ttk.Notebook(result_frame)
        self.notebook.pack(fill="both", expand=True)
        self.result_text = self._add_text_tab("文本")
        self.markdown_text = self._add_text_tab("Markdown")
        self.json_text = self._add_text_tab("JSON")
        result_actions = ttk.Frame(result_frame)
        result_actions.pack(fill="x", pady=(4, 0))
        ttk.Button(result_actions, text="保存当前结果", command=self.save_result).pack(side="right")
        ttk.Separator(self).pack(fill="x", pady=(5, 2))
        ttk.Label(self, textvariable=self.status, anchor="w").pack(fill="x")

    def _balance_body(self, event: tk.Event) -> None:
        if event.width > 1:
            self.body.sashpos(0, event.width // 2)

    def _add_text_tab(self, title: str) -> tk.Text:
        frame = ttk.Frame(self.notebook)
        text = tk.Text(frame, wrap="word", font=("Microsoft YaHei UI", 10), undo=False)
        scrollbar = ttk.Scrollbar(frame, orient="vertical", command=text.yview)
        text.configure(yscrollcommand=scrollbar.set)
        scrollbar.pack(side="right", fill="y")
        text.pack(fill="both", expand=True)
        self.notebook.add(frame, text=title)
        return text

    def _engine_options(self) -> EngineOptions:
        return EngineOptions(
            text_model=self.text_model.get(),
            layout_model=self.layout_model.get(),
            use_layout=self.use_layout.get(),
            use_gpu=self.use_gpu.get(),
            use_tensorrt=self.use_tensorrt.get(),
            cpu_threads=self.cpu_threads.get(),
            cpu_mem=self.cpu_mem.get(),
            gpu_id=self.gpu_id.get(),
            gpu_mem=self.gpu_mem.get(),
            instance_count=self.instance_count.get(),
            return_word_box=self.return_word_box.get(),
        )

    def _submit(self, message: str, work: Callable[[], object], done: Callable[[object], None]) -> None:
        if self.busy:
            return
        self.busy = True
        self.status.set(f"状态：{message}")
        self._refresh_buttons()
        future = self.executor.submit(work)

        def finish() -> None:
            if self._closing:
                return
            self.busy = False
            try:
                done(future.result())
                self.status.set("状态：就绪")
            except Exception as error:
                self.status.set("状态：操作失败")
                messagebox.showerror("PaddleOCR", str(error), parent=self.master)
            self._refresh_buttons()

        future.add_done_callback(lambda _future: self.master.after(0, finish))

    def initialize(self) -> None:
        options = self._engine_options()

        def work() -> NativeOcr:
            native = NativeOcr(self.paths)
            try:
                native.initialize(options)
                return native
            except Exception:
                native.close()
                raise

        def done(value: object) -> None:
            self.native = value if isinstance(value, NativeOcr) else None
            self._append_result("OCR 引擎初始化成功。\n")

        self._submit("正在初始化引擎，请稍候...", work, done)

    def release(self) -> None:
        native = self.native
        if native is None:
            return
        self._submit("正在释放引擎...", native.close, lambda _value: self._released())

    def _released(self) -> None:
        self.native = None
        self._append_result("OCR 引擎已释放。\n")

    def recognize_text_file(self) -> None:
        path = filedialog.askopenfilename(filetypes=[("图片", "*.png *.jpg *.jpeg *.bmp *.tif *.tiff *.webp"), ("所有文件", "*.*")])
        if path:
            self._recognize_input(lambda: load_image(path), layout=False)

    def recognize_layout_file(self) -> None:
        path = filedialog.askopenfilename(filetypes=[("图片", "*.png *.jpg *.jpeg *.bmp *.tif *.tiff *.webp"), ("所有文件", "*.*")])
        if path:
            self._recognize_input(lambda: load_image(path), layout=True)

    def recognize_pdf(self) -> None:
        path = filedialog.askopenfilename(filetypes=[("PDF", "*.pdf")])
        if path:
            self._recognize_input(lambda: load_pdf_first_page(path), layout=True)

    def recognize_clipboard(self) -> None:
        self._recognize_input(load_clipboard, layout=False)

    def _recognize_input(self, loader: Callable[[], ImageInput], layout: bool) -> None:
        native = self.native
        if native is None or not native.text_ready or (layout and not native.layout_ready):
            messagebox.showwarning("PaddleOCR", "请先初始化对应的 OCR 引擎。", parent=self.master)
            return
        repeats = max(1, self.repeat_count.get())

        def work() -> tuple[ImageInput, RecognitionResult]:
            image_input = loader()
            raw = ""
            for _ in range(repeats):
                raw = native.detect_layout(image_input.png_bytes) if layout else native.detect_text(image_input.png_bytes)
            result = parse_layout_result(raw) if layout else parse_text_result(raw)
            return image_input, result

        def done(value: object) -> None:
            if not isinstance(value, tuple) or len(value) != 2:
                raise ValueError("识别结果无效")
            image_input, result = value
            if not isinstance(image_input, ImageInput) or not isinstance(result, RecognitionResult):
                raise ValueError("识别结果无效")
            self.current_input = image_input
            self.preview.set_content(image_input.image, result.boxes)
            self._show_result(result)

        self._submit("正在进行版面识别..." if layout else "正在进行文本识别...", work, done)

    def _show_result(self, value: object) -> None:
        if not isinstance(value, RecognitionResult):
            raise ValueError("识别结果无效")
        self.current_result = value
        self.preview.set_boxes(value.boxes)
        self._set_text(self.result_text, value.text)
        self._set_text(self.markdown_text, value.markdown)
        self._set_text(self.json_text, value.raw_json)
        self.notebook.select(1 if value.markdown else 0)

    @staticmethod
    def _set_text(widget: tk.Text, value: str) -> None:
        widget.delete("1.0", "end")
        widget.insert("1.0", value)

    def _append_result(self, value: str) -> None:
        self.result_text.insert("end", value)
        self.result_text.see("end")

    def save_result(self) -> None:
        if self.current_result is None:
            return
        selected = self.notebook.index(self.notebook.select())
        suffix, content = ((".txt", self.current_result.text), (".md", self.current_result.markdown), (".json", self.current_result.raw_json))[selected]
        path = filedialog.asksaveasfilename(defaultextension=suffix, filetypes=[(suffix.upper()[1:], f"*{suffix}")])
        if path:
            Path(path).write_text(content, encoding="utf-8")

    def _refresh_buttons(self) -> None:
        ready = self.native is not None and self.native.text_ready
        layout_ready = self.native is not None and self.native.layout_ready
        self.init_button.configure(state="disabled" if self.busy or ready else "normal")
        self.free_button.configure(state="normal" if not self.busy and ready else "disabled")
        self.text_button.configure(state="normal" if not self.busy and ready else "disabled")
        self.clipboard_button.configure(state="normal" if not self.busy and ready else "disabled")
        self.layout_button.configure(state="normal" if not self.busy and layout_ready else "disabled")
        self.pdf_button.configure(state="normal" if not self.busy and layout_ready else "disabled")
        self.tensorrt_check.configure(state="normal" if not self.busy and not ready and self.use_gpu.get() else "disabled")
        self.text_combo.configure(state="disabled" if self.busy or ready else "readonly")
        self.layout_combo.configure(state="disabled" if self.busy or ready else "readonly")

    def close(self) -> None:
        if self.busy:
            messagebox.showinfo("PaddleOCR", "当前原生操作完成后才能退出。", parent=self.master)
            return
        self._closing = True
        if self.native is not None:
            self.native.close()
        self.executor.shutdown(wait=False, cancel_futures=True)
        self.master.destroy()


def run() -> None:
    root = tk.Tk()
    MainWindow(root)
    root.mainloop()