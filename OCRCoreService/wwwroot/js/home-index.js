const state = {
    file: null,
    objectUrl: "",
    result: null,
    pageIndex: 1,
    pageCount: 1,
    isPdf: false,
    markdownText: "",
    tab: "doc",
    busy: false,
    activeBlockId: null
};

const app = document.getElementById("app");
const fileInput = document.getElementById("fileInput");
const dropzone = document.getElementById("dropzone");
const fileList = document.getElementById("fileList");
const sourceName = document.getElementById("sourceName");
const sourceSize = document.getElementById("sourceSize");
const modelSelect = document.getElementById("modelSelect");
const docView = document.getElementById("docView");
const jsonView = document.getElementById("jsonView");
const docTab = document.getElementById("docTab");
const jsonTab = document.getElementById("jsonTab");
const copyButton = document.getElementById("copyButton");
const refreshButton = document.getElementById("refreshButton");
const newButton = document.getElementById("newButton");
const licenseCodeButton = document.getElementById("licenseCodeButton");
const licenseStatusButton = document.getElementById("licenseStatusButton");
const uploadLicenseButton = document.getElementById("uploadLicenseButton");
const licenseInput = document.getElementById("licenseInput");
const statusPill = document.getElementById("statusPill");
const toast = document.getElementById("toast");
const placeholder = document.getElementById("placeholder");
const imageWrap = document.getElementById("imageWrap");
const previewImage = document.getElementById("previewImage");
const overlay = document.getElementById("overlay");
const pager = document.getElementById("pager");
const prevPageButton = document.getElementById("prevPageButton");
const nextPageButton = document.getElementById("nextPageButton");
const pageInput = document.getElementById("pageInput");
const pageCount = document.getElementById("pageCount");
const placeholderMark = placeholder.querySelector(".placeholder-mark");
const initialSourceName = sourceName.textContent;
const initialFileListHtml = fileList.innerHTML;
const initialDocText = docView.textContent;
const initialStatusText = statusPill.textContent;
const initialPlaceholderMark = placeholderMark ? placeholderMark.textContent : "";

fileInput.addEventListener("change", () => {
    const file = fileInput.files && fileInput.files[0];
    if (file) {
        setFile(file);
    }
});

dropzone.addEventListener("dragover", event => {
    event.preventDefault();
    dropzone.classList.add("dragging");
});

dropzone.addEventListener("dragleave", () => {
    dropzone.classList.remove("dragging");
});

dropzone.addEventListener("drop", event => {
    event.preventDefault();
    dropzone.classList.remove("dragging");
    const file = event.dataTransfer.files && event.dataTransfer.files[0];
    if (file) {
        setFile(file);
    }
});

modelSelect.addEventListener("change", () => {
    if (state.file) {
        analyze();
    }
});

refreshButton.addEventListener("click", () => analyze());
newButton.addEventListener("click", () => startNewAnalysis());
licenseCodeButton.addEventListener("click", () => getLicenseRequestCode());
licenseStatusButton.addEventListener("click", () => getLicenseStatus());
uploadLicenseButton.addEventListener("click", () => {
    if (!state.busy) {
        licenseInput.click();
    }
});
licenseInput.addEventListener("change", () => {
    const file = licenseInput.files && licenseInput.files[0];
    if (file) {
        uploadLicense(file);
    }
});
prevPageButton.addEventListener("click", () => changePage(state.pageIndex - 1));
nextPageButton.addEventListener("click", () => changePage(state.pageIndex + 1));
pageInput.addEventListener("change", () => changePage(Number.parseInt(pageInput.value, 10)));
pageInput.addEventListener("keydown", event => {
    if (event.key === "Enter") {
        event.preventDefault();
        changePage(Number.parseInt(pageInput.value, 10));
    }
});

docTab.addEventListener("click", () => setTab("doc"));
jsonTab.addEventListener("click", () => setTab("json"));

copyButton.addEventListener("click", async () => {
    const text = state.tab === "json" ? jsonView.textContent : (state.markdownText || docView.textContent);
    if (!text || text === "等待上传文件") {
        showToast("暂无可复制内容");
        return;
    }

    await navigator.clipboard.writeText(text);
    showToast("已复制");
});

previewImage.addEventListener("load", () => {
    previewImage.dataset.fallbackUsed = "";
    renderBoxes();
});
previewImage.addEventListener("error", () => {
    const fallback = previewImage.dataset.fallbackSource || "";
    const primary = previewImage.dataset.primarySource || "";
    if (fallback && previewImage.dataset.fallbackUsed !== "true" && fallback !== primary) {
        previewImage.dataset.fallbackUsed = "true";
        previewImage.src = fallback;
        return;
    }

    overlay.innerHTML = "";
    previewImage.removeAttribute("src");
    imageWrap.hidden = true;
    placeholder.hidden = false;
});
window.addEventListener("resize", () => renderBoxes());

function setFile(file) {
    const validation = validateFile(file);
    if (validation) {
        showToast(validation);
        return;
    }

    state.file = file;
    state.result = null;
    state.pageIndex = 1;
    state.pageCount = 1;
    state.isPdf = isPdfFile(file);
    state.markdownText = "";
    dropzone.hidden = true;
    updateFileSummary(file);
    renderLocalPreview(file);
    setResultText("解析中...");
    analyze();
}

function startNewAnalysis() {
    if (state.busy) {
        return;
    }

    state.file = null;
    state.result = null;
    state.pageIndex = 1;
    state.pageCount = 1;
    state.isPdf = false;
    state.markdownText = "";
    fileInput.value = "";
    dropzone.hidden = false;
    dropzone.classList.remove("dragging");
    overlay.innerHTML = "";
    pager.hidden = true;
    pageInput.value = "1";
    pageCount.textContent = "1";
    placeholder.hidden = false;
    if (placeholderMark) {
        placeholderMark.textContent = initialPlaceholderMark;
    }
    imageWrap.hidden = true;
    previewImage.removeAttribute("src");
    sourceName.textContent = initialSourceName;
    sourceSize.textContent = "";
    fileList.innerHTML = initialFileListHtml;
    setResultText(initialDocText);
    setTab("doc");
    setStatus(initialStatusText);

    if (state.objectUrl) {
        URL.revokeObjectURL(state.objectUrl);
        state.objectUrl = "";
    }
}

async function analyze() {
    if (!state.file || state.busy) {
        return;
    }

    setBusy(true);
    setResultText("解析中...");
    overlay.innerHTML = "";

    const form = new FormData();
    form.append("file", state.file);
    form.append("model", modelSelect.value);
    form.append("pageIndex", String(state.pageIndex || 1));

    try {
        const response = await fetch("/OCRDemo/Analyze", {
            method: "POST",
            body: form
        });
        const payload = await response.json();
        if (String(payload.status) !== "200") {
            throw new Error(payload.errorMessage || "解析失败");
        }

        state.result = payload.data;
        applyResult(state.result);
        setStatus("解析完成");
    } catch (error) {
        state.result = null;
        setResultText(error.message || "解析失败");
        jsonView.textContent = "{}";
        setStatus("解析失败");
        showToast(error.message || "解析失败");
    } finally {
        setBusy(false);
    }
}

async function getLicenseRequestCode() {
    if (state.busy) {
        return;
    }

    setBusy(true, "获取授权申请码中...");
    try {
        const payload = await fetchJson("/Home/GetLicenseRequestCode");
        const requestCode = payload.data && payload.data.requestCode;
        if (!requestCode) {
            throw new Error("未获取到GPU授权申请码。");
        }

        setResultText(requestCode);
        jsonView.textContent = formatJson(payload.data);
        setStatus("授权申请码已生成");
        try {
            await navigator.clipboard.writeText(requestCode);
            showToast("授权申请码已复制");
        } catch {
            showToast("授权申请码已生成");
        }
    } catch (error) {
        setResultText(error.message || "获取授权申请码失败");
        setStatus("获取授权申请码失败");
        showToast(error.message || "获取授权申请码失败");
    } finally {
        setBusy(false);
    }
}

async function getLicenseStatus() {
    if (state.busy) {
        return;
    }

    setBusy(true, "查看授权状态中...");
    try {
        const payload = await fetchJson("/Home/GetLicenseStatus");
        const data = payload.data || {};
        setResultText(data.statusText || "未获取到授权状态。");
        jsonView.textContent = formatJson(data.modules || data.module || data);
        setStatus("授权状态已更新");
        showToast("授权状态已更新");
    } catch (error) {
        setResultText(error.message || "获取授权状态失败");
        setStatus("获取授权状态失败");
        showToast(error.message || "获取授权状态失败");
    } finally {
        setBusy(false);
    }
}

async function uploadLicense(file) {
    if (state.busy) {
        return;
    }

    const validation = validateLicenseFile(file);
    if (validation) {
        licenseInput.value = "";
        showToast(validation);
        return;
    }

    setBusy(true, "上传授权文件中...");
    const form = new FormData();
    form.append("file", file);
    try {
        const payload = await fetchJson("/Home/UploadLicense", {
            method: "POST",
            body: form
        });

        const data = payload.data || {};
        setResultText(data.statusText || "授权文件已保存到Models目录。");
        jsonView.textContent = formatJson(data);
        setStatus("授权文件有效");
        showToast("授权文件已保存");
    } catch (error) {
        setResultText(error.message || "授权文件无效");
        setStatus("授权文件无效");
        showToast(error.message || "授权文件无效");
    } finally {
        licenseInput.value = "";
        setBusy(false);
    }
}

async function fetchJson(url, options) {
    const response = await fetch(url, options);
    const payload = await response.json();
    if (String(payload.status) !== "200") {
        throw new Error(payload.errorMessage || "请求失败");
    }

    return payload;
}

function applyResult(result) {
    const markdown = result.markdown || result.content || "未识别到内容";
    state.markdownText = markdown;
    renderMarkdownResult(markdown);
    jsonView.textContent = formatJson(result.jsonText || result.raw || result);

    const previewSource = getPreviewImageSource(result);
    if (previewSource) {
        setPreviewImageSource(previewSource, "");
        placeholder.hidden = true;
        imageWrap.hidden = false;
    }

    state.pageIndex = result.pageIndex || state.pageIndex || 1;
    state.pageCount = result.pageCount || 1;
    updatePager();
    renderBoxes();
}

function getPreviewImageSource(result) {
    return result.previewImage || state.objectUrl || "";
}

function setPreviewImageSource(primarySource, fallbackSource) {
    const primary = normalizeImageSource(primarySource);
    const fallback = normalizeImageSource(fallbackSource);
    previewImage.dataset.primarySource = primary;
    previewImage.dataset.fallbackSource = fallback;
    previewImage.dataset.fallbackUsed = "";
    previewImage.src = primary;
}

function parseJsonValue(value) {
    if (typeof value !== "string") {
        return value;
    }

    const trimmed = value.trim();
    if (!trimmed || !/^[\[{]/.test(trimmed)) {
        return value;
    }

    try {
        return JSON.parse(trimmed);
    } catch {
        return value;
    }
}

function normalizeImageSource(source) {
    const value = String(source || "").trim().replaceAll("\\", "/");
    if (/^(?:data:|https?:\/\/|blob:|\/)/i.test(value)) {
        return value;
    }

    const outputIndex = value.toLowerCase().lastIndexOf("/output/");
    if (outputIndex >= 0) {
        return `/${value.slice(outputIndex + 1)}`;
    }

    if (value.toLowerCase().startsWith("output/")) {
        return `/${value}`;
    }

    return value;
}

function renderLocalPreview(file) {
    overlay.innerHTML = "";
    pager.hidden = true;
    pageInput.value = "1";
    pageCount.textContent = "1";

    if (state.objectUrl) {
        URL.revokeObjectURL(state.objectUrl);
        state.objectUrl = "";
    }

    if (isPdfFile(file)) {
        placeholder.hidden = true;
        imageWrap.hidden = true;
        return;
    }

    state.objectUrl = URL.createObjectURL(file);
    setPreviewImageSource(state.objectUrl, "");
    placeholder.hidden = true;
    imageWrap.hidden = false;
}

function renderBoxes() {
    overlay.innerHTML = "";
    if (!state.result || !Array.isArray(state.result.boxes) || !previewImage.naturalWidth || !previewImage.naturalHeight) {
        return;
    }

    const displayWidth = previewImage.clientWidth;
    const displayHeight = previewImage.clientHeight;
    if (!displayWidth || !displayHeight) {
        return;
    }

    const scaleX = displayWidth / previewImage.naturalWidth;
    const scaleY = displayHeight / previewImage.naturalHeight;
    for (let index = 0; index < state.result.boxes.length; index++) {
        const item = state.result.boxes[index];
        if (!item || item.width <= 0 || item.height <= 0) {
            continue;
        }

        const box = document.createElement("div");
        box.className = "box";
        const blockId = getBlockId(item, index);
        box.dataset.blockId = blockId;
        box.style.left = `${item.x * scaleX}px`;
        box.style.top = `${item.y * scaleY}px`;
        box.style.width = `${item.width * scaleX}px`;
        box.style.height = `${item.height * scaleY}px`;

        const label = document.createElement("div");
        label.className = "box-label";
        label.textContent = `${item.label || "block"} #${blockId}`;
        box.appendChild(label);
        box.addEventListener("mouseenter", () => setActiveBlock(blockId));
        box.addEventListener("mouseleave", () => setActiveBlock(null));
        overlay.appendChild(box);
    }
    updateActiveBlock();
}

function getBlockId(item, fallbackIndex) {
    const value = item && (item.blockId ?? item.BlockId ?? item.block_id ?? item.id);
    return value !== undefined && value !== null ? String(value) : String(fallbackIndex);
}

function getBlockOrder(item, fallbackIndex) {
    const value = item && (item.blockOrder ?? item.BlockOrder ?? item.block_order);
    const number = Number(value);
    return Number.isFinite(number) ? number : fallbackIndex;
}

function getBlockLabel(item) {
    return (item && (item.label ?? item.Label ?? item.block_label)) || "block";
}

function getBlockText(item) {
    return (item && (item.text ?? item.Text ?? item.block_content)) || "";
}

function getOrderedBoxes() {
    const boxes = state.result && Array.isArray(state.result.boxes) ? state.result.boxes : [];
    return boxes
        .map((box, index) => ({ box, index }))
        .filter(item => item.box && getBlockText(item.box))
        .sort((a, b) => {
            const ao = getBlockOrder(a.box, a.index);
            const bo = getBlockOrder(b.box, b.index);
            return ao - bo;
        });
}

function getOrderedResultBlocks() {
    const boxes = getOrderedBoxes().map(item => ({
        source: item.box,
        index: item.index
    }));
    if (boxes.length) {
        return boxes;
    }

    const raw = parseJsonValue(state.result && (state.result.raw || state.result.jsonText));
    const blocks = raw && (Array.isArray(raw.parsing_res_list) ? raw.parsing_res_list : raw.parsingResList);
    const blockList = Array.isArray(blocks) ? blocks : [];
    return blockList
        .map((block, index) => ({ source: block, index }))
        .filter(item => item.source && getBlockText(item.source))
        .sort((a, b) => getBlockOrder(a.source, a.index) - getBlockOrder(b.source, b.index));
}

function renderBlockMarkdown(blocks) {
    const fragment = document.createDocumentFragment();
    for (const item of blocks) {
        const block = item.source;
        const blockId = getBlockId(block, item.index);
        const label = getBlockLabel(block);
        const content = formatBlockMarkdown(getBlockText(block), label);
        const wrapper = document.createElement("div");
        wrapper.className = "markdown-block";
        wrapper.dataset.blockId = blockId;
        wrapper.dataset.blockLabel = `${label} #${blockId}`;
        wrapper.innerHTML = sanitizeMarkdownHtml(parseMarkdown(content));
        wrapper.addEventListener("mouseenter", () => setActiveBlock(blockId, true));
        wrapper.addEventListener("mouseleave", () => setActiveBlock(null));
        fragment.appendChild(wrapper);
    }

    docView.replaceChildren(fragment);
}

function formatBlockMarkdown(content, label) {
    const text = String(content || "").trim();
    if (!text) {
        return "";
    }

    if (/^\s{0,3}#{1,6}\s/.test(text) || startsWithRenderableHtml(text)) {
        return text;
    }

    if (label === "doc_title") {
        return `# ${text}`;
    }

    if (label === "paragraph_title") {
        return `# ${text}`;
    }

    return text;
}

function setActiveBlock(blockId, scrollPreview = false) {
    state.activeBlockId = blockId == null ? null : String(blockId);
    updateActiveBlock();

    if (scrollPreview && state.activeBlockId) {
        const activeBox = overlay.querySelector(`.box[data-block-id="${cssEscape(state.activeBlockId)}"]`);
        if (activeBox) {
            activeBox.scrollIntoView({ block: "nearest", inline: "nearest" });
        }
    }

    if (!scrollPreview && state.activeBlockId) {
        const activeBlock = docView.querySelector(`.markdown-block[data-block-id="${cssEscape(state.activeBlockId)}"]`);
        if (activeBlock) {
            activeBlock.scrollIntoView({ block: "nearest", inline: "nearest" });
        }
    }
}

function updateActiveBlock() {
    const activeId = state.activeBlockId;
    for (const box of overlay.querySelectorAll(".box")) {
        box.classList.toggle("active", !!activeId && box.dataset.blockId === activeId);
    }
    for (const block of docView.querySelectorAll(".markdown-block")) {
        block.classList.toggle("active", !!activeId && block.dataset.blockId === activeId);
    }
}

function cssEscape(value) {
    if (window.CSS && typeof window.CSS.escape === "function") {
        return window.CSS.escape(value);
    }
    return String(value).replace(/["\\]/g, "\\$&");
}

function updateFileSummary(file) {
    sourceName.textContent = file.name;
    sourceSize.textContent = formatSize(file.size);
    fileList.innerHTML = "";

    const item = document.createElement("div");
    item.className = "file-item";
    item.innerHTML = `
        <div class="file-icon">${getExtensionLabel(file.name)}</div>
        <div>
            <div class="file-name" title="${escapeHtml(file.name)}">${escapeHtml(file.name)}</div>
            <div class="file-meta">${new Date().toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })} · ${formatSize(file.size)}</div>
        </div>`;
    fileList.appendChild(item);
}

function setResultText(text) {
    state.markdownText = "";
    clearMath(docView);
    docView.classList.add("plain-text");
    docView.classList.remove("markdown-body");
    docView.textContent = text;
    jsonView.textContent = "{}";
}

function renderMarkdownResult(markdown) {
    clearMath(docView);
    docView.classList.remove("plain-text");
    docView.classList.add("markdown-body");
    const blocks = getOrderedResultBlocks();
    if (blocks.length) {
        renderBlockMarkdown(blocks);
    } else {
        docView.innerHTML = sanitizeMarkdownHtml(parseMarkdown(markdown));
    }
    renderMath(docView);
}

function parseMarkdown(markdown) {
    const normalizedSource = normalizeBareOptionMath(
        normalizeMathDelimiters(normalizeRenderableHtml(String(markdown || "")))
    );
    const protectedSource = protectMath(normalizedSource);
    const source = protectedSource.text;
    let html = "";
    if (window.marked && typeof window.marked.parse === "function") {
        html = window.marked.parse(source, { gfm: true, breaks: true });
        return restoreMath(html, protectedSource.tokens);
    }

    if (typeof window.marked === "function") {
        html = window.marked(source, { gfm: true, breaks: true });
        return restoreMath(html, protectedSource.tokens);
    }

    html = `<pre><code>${escapeHtml(source)}</code></pre>`;
    return restoreMath(html, protectedSource.tokens);
}

function normalizeMathDelimiters(source) {
    return String(source || "")
        .replace(/\\{2}([()[\]])/g, "\\$1");
}

function normalizeBareOptionMath(source) {
    return String(source || "")
        .split(/\n/)
        .map(normalizeBareOptionMathLine)
        .join("\n");
}

function normalizeBareOptionMathLine(line) {
    if (!/[\\](?:d?frac|leq?|geq?|sqrt|times|cdot|div|infty|pi)|[<>≤≥]/.test(line)) {
        return line;
    }

    const optionPattern = /(^|\s+)([A-D])\.\s*([\s\S]*?)(?=(?:\s+[A-D]\.\s*)|$)/g;
    let matched = false;
    const normalized = line.replace(optionPattern, (match, prefix, label, body) => {
        matched = true;
        const trimmed = body.trim();
        if (!shouldWrapBareOptionMath(trimmed)) {
            return match;
        }

        return `${prefix}${label}. ${buildMathSource(trimmed, false)}`;
    });

    return matched ? normalized : line;
}

function shouldWrapBareOptionMath(value) {
    const text = String(value || "").trim();
    if (!text || /\\\(|\\\[|\$\$|\$/.test(text)) {
        return false;
    }

    return /\\(?:d?frac|leq?|geq?|sqrt|times|cdot|div|infty|pi)\b/.test(text)
        || /[≤≥]/.test(text)
        || /(?:^|[\s(])-?[\dA-Za-z]+(?:\s*[<>]\s*-?[\dA-Za-z\\{}\/]+)+/.test(text);
}

function protectMath(source) {
    const tokens = [];
    const text = source.replace(/(\$\$[\s\S]+?\$\$|\\\[[\s\S]+?\\\]|\\\([\s\S]+?\\\)|\$[^\n$]+?\$)/g, match => {
        const key = `@@MATH_${tokens.length}@@`;
        tokens.push(createMathToken(match));
        return key;
    });

    return { text, tokens };
}

function createMathToken(source) {
    let tex = source;
    let display = false;

    if (source.startsWith("$$") && source.endsWith("$$")) {
        tex = source.slice(2, -2);
        display = true;
    } else if (source.startsWith("\\[") && source.endsWith("\\]")) {
        tex = source.slice(2, -2);
        display = true;
    } else if (source.startsWith("\\(") && source.endsWith("\\)")) {
        tex = source.slice(2, -2);
    } else if (source.startsWith("$") && source.endsWith("$")) {
        tex = source.slice(1, -1);
    }

    tex = normalizeMathTex(decodeHtmlEntities(tex));

    return {
        source,
        tex,
        normalizedSource: buildMathSource(tex, display),
        display,
        valid: isRenderableMath(tex)
    };
}

function buildMathSource(tex, display) {
    return display ? `\\[${tex}\\]` : `\\(${tex}\\)`;
}

function normalizeMathTex(tex) {
    return String(tex || "")
        .replace(/\\{2}(?=[A-Za-z{}])/g, "\\")
        .replace(/&(?!(?:amp|lt|gt|quot|apos|nbsp|#\d+|#x[0-9a-f]+);)/gi, "\\&");
}

function isRenderableMath(tex) {
    const value = String(tex || "").trim();
    if (!value) {
        return false;
    }

    let depth = 0;
    for (let index = 0; index < value.length; index++) {
        const char = value[index];
        if (char === "\\") {
            index++;
            continue;
        }
        if (char === "{") {
            depth++;
            continue;
        }
        if (char === "}") {
            depth--;
            if (depth < 0) {
                return false;
            }
        }
    }

    return depth === 0;
}

function restoreMath(html, tokens) {
    return tokens.reduce((result, token, index) => {
        const replacement = token.valid
            ? `<span class="mathjax-source">${escapeHtml(token.normalizedSource)}</span>`
            : escapeHtml(token.source);
        return result.replaceAll(`@@MATH_${index}@@`, replacement);
    }, html);
}

let mathRenderToken = 0;
function renderMath(element) {
    if (!element) {
        return;
    }

    const token = ++mathRenderToken;
    if (window.MathJax && typeof MathJax.whenReady === "function") {
        MathJax.whenReady(() => {
            if (token !== mathRenderToken || !element.isConnected) {
                return Promise.resolve();
            }

            return renderMathContent(element);
        })
            .catch(error => {
                console.warn("MathJax render failed", error);
            });
        return;
    }

    scheduleMathRender(element, token, 0);
}

function scheduleMathRender(element, token, attempt) {
    window.setTimeout(() => {
        if (token !== mathRenderToken || !element.isConnected) {
            return;
        }

        if (window.MathJax && typeof MathJax.whenReady === "function") {
            MathJax.whenReady(() => renderMathContent(element)).catch(error => {
                console.warn("MathJax render failed", error);
            });
            return;
        }

        if (window.MathJax && typeof MathJax.typesetPromise === "function") {
            renderMathContent(element);
            return;
        }

        if (attempt < 20) {
            scheduleMathRender(element, token, attempt + 1);
            return;
        }

        console.warn("MathJax is not ready; formulas remain as TeX text.");
    }, attempt === 0 ? 0 : 100);
}

function renderMathContent(element) {
    const sources = Array.from(element.querySelectorAll(".mathjax-source"));
    if (sources.length) {
        for (const source of sources) {
            source.replaceWith(document.createTextNode(source.textContent || ""));
        }
    }

    return typesetMath(element);
}

function clearMath(element) {
    if (window.MathJax && typeof MathJax.typesetClear === "function") {
        MathJax.typesetClear([element]);
    }
}

function typesetMath(element) {
    clearMath(element);

    if (typeof MathJax.texReset === "function") {
        MathJax.texReset();
    }

    if (typeof MathJax.typesetPromise === "function") {
        return MathJax.typesetPromise([element]).catch(error => {
            console.warn("MathJax render failed", error);
        });
    }

    return Promise.resolve();
}

function normalizeRenderableHtml(source) {
    return source
        .replace(/```(?:html|HTML)?\s*\n([\s\S]*?)```/g, (match, code) => {
            const decoded = decodeHtmlEntities(code.trim());
            return startsWithRenderableHtml(decoded) ? decoded : match;
        })
        .replace(/&lt;\/?(?:table|thead|tbody|tfoot|tr|th|td|caption|colgroup|col|div|p|ul|ol|li|h[1-6]|blockquote|pre|code|br|hr|strong|em|b|i|u|span|a)\b[\s\S]*?&gt;/gi, value => decodeHtmlEntities(value));
}

function startsWithRenderableHtml(value) {
    return /^\s*<(?:table|thead|tbody|tfoot|tr|th|td|caption|div|p|ul|ol|li|h[1-6]|blockquote|pre)\b/i.test(value);
}

function decodeHtmlEntities(value) {
    const textarea = document.createElement("textarea");
    textarea.innerHTML = value;
    return textarea.value;
}

function sanitizeMarkdownHtml(html) {
    const template = document.createElement("template");
    template.innerHTML = String(html || "");
    const allowedTags = new Set([
        "A", "B", "BLOCKQUOTE", "BR", "CAPTION", "CODE", "COL", "COLGROUP", "DEL", "DIV", "EM", "H1", "H2", "H3", "H4", "H5", "H6",
        "HR", "I", "LI", "OL", "P", "PRE", "S", "SPAN", "STRONG", "SUB", "SUP", "TABLE", "TBODY",
        "TD", "TFOOT", "TH", "THEAD", "TR", "U", "UL"
    ]);
    const allowedAttributes = new Map([
        ["A", new Set(["href", "title", "target", "rel"])],
        ["OL", new Set(["start", "type", "reversed"])],
        ["LI", new Set(["value"])],
        ["DIV", new Set(["class", "data-block-id", "data-block-label"])],
        ["P", new Set(["class", "data-block-id", "data-block-label"])],
        ["H1", new Set(["class", "data-block-id", "data-block-label"])],
        ["H2", new Set(["class", "data-block-id", "data-block-label"])],
        ["H3", new Set(["class", "data-block-id", "data-block-label"])],
        ["H4", new Set(["class", "data-block-id", "data-block-label"])],
        ["H5", new Set(["class", "data-block-id", "data-block-label"])],
        ["H6", new Set(["class", "data-block-id", "data-block-label"])],
        ["SPAN", new Set(["class", "data-math-tex", "data-math-display", "data-block-id", "data-block-label"])],
        ["TD", new Set(["colspan", "rowspan", "align"])],
        ["TH", new Set(["colspan", "rowspan", "align"])]
    ]);

    const cleanNode = node => {
        for (const child of Array.from(node.childNodes)) {
            if (child.nodeType === Node.ELEMENT_NODE) {
                if (!allowedTags.has(child.tagName)) {
                    if (child.tagName === "SCRIPT" || child.tagName === "STYLE" || child.tagName === "IFRAME" || child.tagName === "OBJECT") {
                        child.remove();
                    } else {
                        const fragment = document.createDocumentFragment();
                        while (child.firstChild) {
                            fragment.appendChild(child.firstChild);
                        }
                        child.replaceWith(fragment);
                        cleanNode(node);
                    }
                    continue;
                }

                const allowedForTag = allowedAttributes.get(child.tagName) || new Set();
                for (const attribute of Array.from(child.attributes)) {
                    const name = attribute.name.toLowerCase();
                    if (name.startsWith("on") || name === "style" || !allowedForTag.has(name)) {
                        child.removeAttribute(attribute.name);
                        continue;
                    }

                    if ((name === "href" || name === "src") && !isSafeUrl(attribute.value)) {
                        child.removeAttribute(attribute.name);
                    }
                }

                if (child.tagName === "A" && child.hasAttribute("href")) {
                    child.setAttribute("target", "_blank");
                    child.setAttribute("rel", "noopener noreferrer");
                }

                cleanNode(child);
            } else if (child.nodeType !== Node.TEXT_NODE) {
                child.remove();
            }
        }
    };

    cleanNode(template.content);
    return template.innerHTML;
}

function isSafeUrl(value) {
    const url = String(value || "").trim();
    return url.startsWith("#")
        || url.startsWith("/")
        || /^https?:\/\//i.test(url)
        || /^mailto:/i.test(url);
}

function setTab(tab) {
    state.tab = tab;
    docTab.classList.toggle("active", tab === "doc");
    jsonTab.classList.toggle("active", tab === "json");
    docView.style.display = tab === "doc" ? "block" : "none";
    jsonView.style.display = tab === "json" ? "block" : "none";
}

function changePage(pageIndex) {
    if (!state.file || !state.isPdf || state.busy) {
        updatePager();
        return;
    }

    const targetPage = clampPage(pageIndex);
    if (targetPage === state.pageIndex) {
        updatePager();
        return;
    }

    state.pageIndex = targetPage;
    overlay.innerHTML = "";
    setResultText("解析中...");
    analyze();
}

function updatePager() {
    const total = state.pageCount || 1;
    const current = clampPage(state.pageIndex || 1);
    state.pageIndex = current;
    pager.hidden = !state.isPdf || total <= 1;
    pageInput.value = String(current);
    pageCount.textContent = String(total);
    prevPageButton.disabled = state.busy || current <= 1;
    nextPageButton.disabled = state.busy || current >= total;
    pageInput.disabled = state.busy;
}

function clampPage(pageIndex) {
    const total = Math.max(1, state.pageCount || 1);
    const page = Number.isFinite(pageIndex) ? pageIndex : state.pageIndex || 1;
    return Math.min(Math.max(1, page), total);
}

function isPdfFile(file) {
    return file.type === "application/pdf" || file.name.toLowerCase().endsWith(".pdf");
}

function setBusy(busy, text) {
    state.busy = busy;
    app.classList.toggle("busy", busy);
    updatePager();
    setStatus(busy ? (text || "解析中") : "就绪");
}

function setStatus(text) {
    statusPill.textContent = text;
}

function validateFile(file) {
    const name = file.name.toLowerCase();
    const isPdf = name.endsWith(".pdf") || file.type === "application/pdf";
    const supported = [".pdf", ".png", ".jpg", ".jpeg", ".bmp", ".tif", ".tiff"].some(ext => name.endsWith(ext));
    if (!supported) {
        return "仅支持 PDF、PNG、JPG、BMP、TIF 文件";
    }

    if (isPdf && file.size > 200 * 1024 * 1024) {
        return "PDF 文件不能超过 200MB";
    }

    if (!isPdf && file.size > 10 * 1024 * 1024) {
        return "单张图片不能超过 10MB";
    }

    return "";
}

function validateLicenseFile(file) {
    if (!file.name.toLowerCase().endsWith(".lic")) {
        return "请上传.lic授权文件";
    }

    if (file.size > 1024 * 1024) {
        return "授权文件不能超过1MB";
    }

    return "";
}

function formatJson(value) {
    if (!value) {
        return "{}";
    }

    try {
        const parsed = typeof value === "string" ? JSON.parse(value) : value;
        return JSON.stringify(parsed, null, 2);
    } catch {
        return typeof value === "string" ? value : JSON.stringify(value, null, 2);
    }
}

function formatSize(size) {
    if (size < 1024) {
        return `${size}B`;
    }
    if (size < 1024 * 1024) {
        return `${(size / 1024).toFixed(1)}KB`;
    }
    return `${(size / 1024 / 1024).toFixed(2)}MB`;
}

function getExtensionLabel(name) {
    const ext = name.split(".").pop() || "DOC";
    return ext.slice(0, 3).toUpperCase();
}

function escapeHtml(value) {
    return String(value || "")
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;");
}

function escapeAttribute(value) {
    return escapeHtml(value).replaceAll("'", "&#39;");
}

let toastTimer = 0;
function showToast(message) {
    toast.textContent = message;
    toast.classList.add("show");
    clearTimeout(toastTimer);
    toastTimer = setTimeout(() => toast.classList.remove("show"), 2400);
}

setTab("doc");
updatePager();
