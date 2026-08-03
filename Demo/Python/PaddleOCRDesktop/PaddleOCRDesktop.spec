from pathlib import Path


project_dir = Path(SPEC).resolve().parent

analysis = Analysis(
    [str(project_dir / "desktop_app.py")],
    pathex=[str(project_dir)],
    binaries=[],
    datas=[],
    hiddenimports=["fitz", "PIL.Image", "PIL.ImageGrab", "PIL.ImageTk"],
    hookspath=[],
    hooksconfig={},
    runtime_hooks=[],
    excludes=[],
    noarchive=False,
    optimize=0,
)
pyz = PYZ(analysis.pure)

executable = EXE(
    pyz,
    analysis.scripts,
    analysis.binaries,
    analysis.datas,
    [],
    name="PaddleOCRDesktop",
    debug=False,
    bootloader_ignore_signals=False,
    strip=False,
    upx=True,
    console=False,
    disable_windowed_traceback=False,
    argv_emulation=False,
    target_arch=None,
    codesign_identity=None,
    entitlements_file=None,
)
