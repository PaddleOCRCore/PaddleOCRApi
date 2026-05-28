// Copyright (c) 2026 PaddleOCRCore All Rights Reserved.
// https://github.com/PaddleOCRCore/PaddleOCRApi.git
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using PaddleOCRSDK;

namespace OCRCoreService.Services
{
    /// <summary>
    /// OCR-VL引擎依赖注入
    /// </summary>
    public class OCRVLEngine
    {
        private readonly IOCRVLService _ocrvlService;
        private readonly OCRVLConfig _ocrvlConfig;
        private readonly OCRConfig _ocrConfig;
        private readonly ILogger<OCRVLEngine> _logger;
        private readonly object _syncRoot = new object();
        private bool _licenseActivated;

        public IOCRVLService OcrVlService => _ocrvlService;

        public OCRVLEngine(
            IOCRVLService ocrvlService,
            OCRVLConfig ocrvlConfig,
            OCRConfig ocrConfig,
            ILogger<OCRVLEngine> logger)
        {
            _ocrvlService = ocrvlService;
            _ocrvlConfig = ocrvlConfig;
            _ocrConfig = ocrConfig;
            _logger = logger;
            Initialize();
        }

        /// <summary>
        /// 初始化前自动激活GPU授权文件
        /// </summary>
        /// <returns></returns>
        public bool ActivateLicenseIfExists()
        {
            if (_licenseActivated)
            {
                return true;
            }

            string licensePath = ResolvePath(_ocrConfig.OCRLicense);
            if (string.IsNullOrWhiteSpace(licensePath) || !File.Exists(licensePath))
            {
                return false;
            }

            lock (_syncRoot)
            {
                if (_licenseActivated)
                {
                    return true;
                }

                _licenseActivated = _ocrvlService.ActivateLicense(licensePath);
                if (_licenseActivated)
                {
                    _logger.LogInformation("OCR-VL授权激活成功: {LicensePath}", licensePath);
                }
                else
                {
                    _logger.LogWarning("OCR-VL授权激活失败: {LicensePath}", licensePath);
                }

                return _licenseActivated;
            }
        }

        private void Initialize()
        {
            if (!_ocrvlConfig.enabled)
            {
                _logger.LogInformation("OCR-VL服务未启用");
                return;
            }

            _logger.LogInformation("OCR-VL服务已启用");
            ActivateLicenseIfExists();

            string yamlPath = ResolvePath(_ocrvlConfig.yaml_path);
            try
            {
                _ocrvlService.Init(yamlPath);
                _logger.LogInformation("OCR-VL引擎初始化成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OCR-VL引擎初始化失败: {Message}", ex.Message);
            }

            try
            {
                _ocrvlService.InitDoc(yamlPath);
                _logger.LogInformation("OCR-VL版面分析引擎初始化成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OCR-VL版面分析引擎初始化失败: {Message}", ex.Message);
            }
        }

        private static string ResolvePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            return Path.IsPathRooted(path)
                ? path
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }
    }
}
