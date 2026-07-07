#pragma once

#include <string>
#include <vector>

/// <summary>
/// 执行普通文本识别Demo：初始化OCR引擎，遍历图片并输出OCR结果。
/// </summary>
/// <param name="images">要识别的图片路径集合。</param>
/// <param name="baseDirectory">模型和授权相对路径的基准目录。</param>
/// <returns>全部流程正常返回true；初始化失败或没有图片返回false。</returns>
bool RunTextRecognitionDemo(const std::vector<std::string>& images, const std::string& baseDirectory);
