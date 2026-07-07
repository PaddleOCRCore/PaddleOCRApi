#pragma once

#include <string>
#include <vector>

/// <summary>
/// 递归获取目录下所有文件路径，用于批量执行Demo识别。
/// </summary>
/// <param name="directoryPath">要扫描的图片目录。</param>
/// <param name="files">输出文件路径集合。</param>
void GetFileList(const std::string& directoryPath, std::vector<std::string>& files);

/// <summary>
/// 将PaddleOCR返回的字符串缓冲区复制为std::string，并立即释放原生缓冲区。
/// </summary>
/// <param name="buffer">Detect/DetectLayout等接口返回的原生字符串指针。</param>
/// <returns>复制后的字符串；buffer为空时返回空字符串。</returns>
std::string TakeResultAndFree(const char* buffer);

/// <summary>
/// 获取PaddleOCR最近一次错误信息，并释放错误字符串缓冲区。
/// </summary>
/// <returns>错误信息；没有错误信息时返回空字符串。</returns>
std::string GetLastErrorAndFree();

/// <summary>
/// 拼接Windows路径，自动补齐路径分隔符。
/// </summary>
/// <param name="left">左侧路径。</param>
/// <param name="right">右侧路径。</param>
/// <returns>拼接后的路径。</returns>
std::string CombinePath(const std::string& left, const std::string& right);

/// <summary>
/// 获取当前可执行文件所在目录。
/// </summary>
/// <returns>可执行文件目录；失败时返回空字符串。</returns>
std::string GetExecutableDirectory();

/// <summary>
/// 配置PaddleOCR原生DLL搜索目录，优先查找NuGet运行时目录。
/// </summary>
/// <param name="currentDirectory">进程当前工作目录。</param>
void ConfigureNativeDllDirectory(const std::string& currentDirectory);

/// <summary>
/// 初始化前自动激活授权文件。若授权文件不存在则跳过，不影响CPU模式初始化。
/// </summary>
/// <param name="baseDirectory">相对授权路径的基准目录，通常为当前工作目录。</param>
/// <param name="licensePath">授权文件路径，支持绝对路径或相对baseDirectory路径。</param>
/// <returns>授权文件存在且激活成功返回true；文件不存在或激活失败返回false。</returns>
bool ActivateLicenseIfExists(
    const std::string& baseDirectory,
    const std::string& licensePath = "models\\paddleocr.lic");
