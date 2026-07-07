#pragma once

#include <string>
#include <vector>

/// <summary>
/// 执行版面识别Demo：初始化结构化文档识别引擎，遍历图片并输出版面分析结果。
/// </summary>
/// <param name="images">要分析的图片路径集合。</param>
/// <param name="baseDirectory">模型和授权相对路径的基准目录。</param>
/// <returns>全部流程正常返回true；初始化失败或没有图片返回false。</returns>
bool RunLayoutRecognitionDemo(const std::vector<std::string>& images, const std::string& baseDirectory);
