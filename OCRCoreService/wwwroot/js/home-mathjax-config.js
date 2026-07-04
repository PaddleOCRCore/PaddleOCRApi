window.MathJax = {
    tex: {
        inlineMath: [["\\(", "\\)"], ["$", "$"]],
        displayMath: [["\\[", "\\]"], ["$$", "$$"]],
        processEscapes: true
    },
    options: {
        enableMenu: false,
        enableExplorer: false,
        skipHtmlTags: ["script", "noscript", "style", "textarea", "pre", "code"]
    },
    sre: {
        speech: "none"
    },
    a11y: {
        speech: false,
        voicing: false,
        braille: false,
        brailleSpeech: false,
        explorer: false,
        assistiveMml: false
    },
    startup: {
        typeset: false
    }
};
