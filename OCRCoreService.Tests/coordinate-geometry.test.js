const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const vm = require("node:vm");

const scriptPath = path.resolve(__dirname, "..", "OCRCoreService", "wwwroot", "js", "home-index.js");
const source = fs.readFileSync(scriptPath, "utf8");
const start = source.indexOf("function createCoordinateGeometry");
const end = source.indexOf("function updateCoordinateCanvasScale");
assert.notEqual(start, -1);
assert.notEqual(end, -1);

const context = {
    getBlockText: box => box.text ?? "",
    getBlockLabel: box => box.label ?? ""
};
vm.createContext(context);
vm.runInContext(source.slice(start, end), context);

assert.equal(context.getCoordinateBlockFontSize({ height: 43 }, "paragraph_title", "标题"), 43 * 0.9);
assert.equal(context.getCoordinateBlockFontSize({ height: 60 }, "doc_title", "标题"), 60 * 0.9);
assert.equal(context.getCoordinateBlockFontSize({ height: 26 }, "vision_footnote", "脚注"), 26 * 0.9);
assert.equal(context.getCoordinateBlockFontSize({ height: 30 }, "figure_title", "图题"), 30 * 0.9);
assert.equal(context.getCoordinateBlockFontSize({ height: 404 }, "text", "一\n二\n三"), 18);
assert.equal(context.getCoordinateBlockFontSize({ height: 24 }, "text", "普通文字"), 18);
assert.equal(context.getCoordinateBlockLineHeight("paragraph_title", "标题"), "1");
assert.equal(context.getCoordinateBlockLineHeight("vision_footnote", "脚注"), "1");
assert.equal(context.getCoordinateBlockLineHeight("text", "一\n二\n三"), "1.35");
assert.equal(context.getCoordinateBlockLineHeight("text", "普通文字"), "1.2");
assert.equal(context.getCoordinateTableHtml("text", "<table><tr><td>内容</td></tr></table>"), "");
assert.ok(Math.abs(context.getCoordinateVerticalFontSize(
    { width: 188, height: 547 },
    "否极泰来如覆手陶步自此凌烟霄"
) - 70.32857142857144) < 0.0001);

const verticalOcr = context.createCoordinateGeometry({
    text: "ODM OEM",
    isTextLine: true,
    points: [
        { x: 403, y: 224 },
        { x: 418, y: 224 },
        { x: 418, y: 292 },
        { x: 403, y: 292 }
    ]
}, 0);
assert.deepEqual(
    [
        verticalOcr.x,
        verticalOcr.y,
        verticalOcr.width,
        verticalOcr.height,
        verticalOcr.angle,
        verticalOcr.isVertical,
        verticalOcr.isVerticalWriting
    ],
    [403, 224, 15, 68, 0, false, true]
);

const horizontalOcr = context.createCoordinateGeometry({
    text: "horizontal",
    isTextLine: true,
    points: [
        { x: 10, y: 20 },
        { x: 110, y: 30 },
        { x: 106, y: 60 },
        { x: 6, y: 50 }
    ]
}, 1);
assert.equal(horizontalOcr.isVertical, false);
assert.ok(Math.abs(horizontalOcr.angle - 5.710593) < 0.000001);

const verticalStructureBlock = context.createCoordinateGeometry({
    text: "ODM OEM",
    label: "image",
    isTextLine: false,
    x: 371.3277587890625,
    y: 96.31705474853516,
    width: 73.656432,
    height: 245.938622
}, 2);
assert.equal(verticalStructureBlock.isVertical, true);
assert.equal(verticalStructureBlock.angle, 90);

const verticalWritingBlock = context.createCoordinateGeometry({
    text: "否极泰来如覆手陶步自此凌烟霄",
    label: "vertical_text",
    isTextLine: false,
    x: 12,
    y: 17,
    width: 188,
    height: 547
}, 3);
assert.deepEqual(
    [
        verticalWritingBlock.x,
        verticalWritingBlock.y,
        verticalWritingBlock.width,
        verticalWritingBlock.height,
        verticalWritingBlock.angle,
        verticalWritingBlock.isVertical,
        verticalWritingBlock.isVerticalWriting
    ],
    [12, 17, 188, 547, 0, false, true]
);

console.log("coordinate geometry tests passed");