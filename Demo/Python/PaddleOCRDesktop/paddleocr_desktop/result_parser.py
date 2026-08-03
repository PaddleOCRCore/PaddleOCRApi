from __future__ import annotations

import json
from dataclasses import dataclass
from typing import Any, Iterable


@dataclass(frozen=True)
class RecognitionBox:
    label: str
    text: str
    score: float | None
    points: tuple[tuple[float, float], ...]


@dataclass(frozen=True)
class RecognitionResult:
    text: str
    markdown: str
    boxes: tuple[RecognitionBox, ...]
    raw: Any
    raw_json: str
    visualization_path: str = ""


def _value(data: dict[str, Any], *names: str, default: Any = None) -> Any:
    lowered = {str(key).lower(): value for key, value in data.items()}
    for name in names:
        if name in data:
            return data[name]
        if name.lower() in lowered:
            return lowered[name.lower()]
    return default


def _points(value: Any) -> tuple[tuple[float, float], ...]:
    if not isinstance(value, list):
        return ()
    if len(value) == 4 and all(isinstance(item, (int, float)) for item in value):
        x1, y1, x2, y2 = map(float, value)
        return ((x1, y1), (x2, y1), (x2, y2), (x1, y2))
    result: list[tuple[float, float]] = []
    for item in value:
        if isinstance(item, dict):
            x, y = _value(item, "x"), _value(item, "y")
        elif isinstance(item, (list, tuple)) and len(item) >= 2:
            x, y = item[0], item[1]
        else:
            continue
        if isinstance(x, (int, float)) and isinstance(y, (int, float)):
            result.append((float(x), float(y)))
    return tuple(result)


def parse_text_result(raw_json: str) -> RecognitionResult:
    payload = json.loads(raw_json)
    if not isinstance(payload, list):
        raise ValueError("文本识别结果必须是 JSON 数组")
    lines: list[str] = []
    boxes: list[RecognitionBox] = []
    for item in payload:
        if not isinstance(item, dict):
            continue
        text = str(_value(item, "Words", "words", default="") or "")
        if text:
            lines.append(text)
        score = _value(item, "Score", "score")
        boxes.append(
            RecognitionBox(
                label="text",
                text=text,
                score=float(score) if isinstance(score, (int, float)) else None,
                points=_points(_value(item, "Location", "location", default=[])),
            )
        )
    return RecognitionResult(
        text="\n".join(lines),
        markdown="",
        boxes=tuple(boxes),
        raw=payload,
        raw_json=json.dumps(payload, ensure_ascii=False, indent=2),
    )


def _layout_boxes(payload: dict[str, Any]) -> Iterable[RecognitionBox]:
    overall = _value(payload, "overall_ocr_res", default={})
    if isinstance(overall, dict):
        polygons = _value(overall, "dt_polys", default=[])
        texts = _value(overall, "rec_texts", default=[])
        scores = _value(overall, "rec_scores", default=[])
        if isinstance(polygons, list) and polygons:
            for index, polygon in enumerate(polygons):
                yield RecognitionBox(
                    "text",
                    str(texts[index]) if isinstance(texts, list) and index < len(texts) else "",
                    float(scores[index]) if isinstance(scores, list) and index < len(scores) else None,
                    _points(polygon),
                )
            return
        boxes = _value(overall, "rec_boxes", default=[])
        if isinstance(boxes, list) and boxes:
            for index, box in enumerate(boxes):
                yield RecognitionBox(
                    "text",
                    str(texts[index]) if isinstance(texts, list) and index < len(texts) else "",
                    float(scores[index]) if isinstance(scores, list) and index < len(scores) else None,
                    _points(box),
                )
            return
    blocks = _value(payload, "parsing_res_list", default=[])
    if isinstance(blocks, list) and blocks:
        for block in blocks:
            if not isinstance(block, dict):
                continue
            yield RecognitionBox(
                str(_value(block, "block_label", default="layout") or "layout"),
                str(_value(block, "block_content", default="") or ""),
                float(_value(block, "score")) if isinstance(_value(block, "score"), (int, float)) else None,
                _points(_value(block, "polygon_points", "block_bbox", default=[])),
            )
        return
    layout = _value(payload, "layout_det_res", default={})
    if isinstance(layout, dict):
        for box in _value(layout, "boxes", default=[]) or []:
            if isinstance(box, dict):
                yield RecognitionBox(
                    str(_value(box, "label", default="layout")),
                    "",
                    float(_value(box, "score")) if isinstance(_value(box, "score"), (int, float)) else None,
                    _points(_value(box, "coordinate", default=[])),
                )


def parse_layout_result(raw_json: str) -> RecognitionResult:
    payload = json.loads(raw_json)
    if not isinstance(payload, dict):
        raise ValueError("版面识别结果必须是 JSON 对象")
    blocks = _value(payload, "parsing_res_list", default=[])
    ordered = sorted(
        (item for item in blocks if isinstance(item, dict)),
        key=lambda item: int(_value(item, "block_order", default=0) or 0),
    ) if isinstance(blocks, list) else []
    text = "\n\n".join(
        f"[{_value(item, 'block_label', default='layout')}]\n{_value(item, 'block_content', default='')}"
        for item in ordered
    )
    return RecognitionResult(
        text=text,
        markdown=str(_value(payload, "markdown", default="") or ""),
        boxes=tuple(_layout_boxes(payload)),
        raw=payload,
        raw_json=json.dumps(payload, ensure_ascii=False, indent=2),
        visualization_path=str(_value(payload, "vis_path", default="") or ""),
    )