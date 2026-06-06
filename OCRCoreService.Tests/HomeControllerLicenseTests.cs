using System.Collections;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using OCRCoreService;
using OCRCoreService.Controllers;
using OCRCoreService.Services;
using PaddleOCRSDK;
using PaddleOCRSDK.Models;
using Xunit;

namespace OCRCoreService.Tests;

public class HomeControllerLicenseTests
{
    [Fact]
    public void Index_UsesOcrVlYamlFileNameForModelOption()
    {
        FakeOcrService ocrService = new();
        OCREngine ocrEngine = new(
            ocrService,
            new OCRConfig { OCRLicense = "missing.lic", det_infer = "", rec_infer = "", cls_infer = "" },
            new LayoutConfig());
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddSingleton(new OCRVLConfig { enabled = true, yaml_path = "configs/PaddleOCR-VL-1.5.yaml" })
            .BuildServiceProvider();
        HomeController controller = new(
            NullLogger<HomeController>.Instance,
            ocrEngine,
            ocrService,
            serviceProvider,
            new OCRVLConfig { enabled = true, yaml_path = "configs/PaddleOCR-VL-1.5.yaml" });

        Assert.IsType<ViewResult>(controller.Index());

        Assert.Equal("PaddleOCR-VL-1.5", controller.ViewData["OcrVlModelName"]);
        Assert.Equal("paddleocr-vl-1.5", controller.ViewData["OcrVlModelValue"]);
    }

    [Fact]
    public void GetLicenseStatus_WhenOcrVlDisabled_DoesNotReportOcrVl()
    {
        FakeOcrService ocrService = new();
        OCREngine ocrEngine = new(
            ocrService,
            new OCRConfig { OCRLicense = "missing.lic", det_infer = "", rec_infer = "", cls_infer = "" },
            new LayoutConfig());
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddSingleton(new OCRVLConfig { enabled = false })
            .BuildServiceProvider();
        HomeController controller = new(
            NullLogger<HomeController>.Instance,
            ocrEngine,
            ocrService,
            serviceProvider,
            new OCRVLConfig { enabled = false });

        Microsoft.AspNetCore.Mvc.JsonResult jsonResult =
            Assert.IsType<Microsoft.AspNetCore.Mvc.JsonResult>(controller.GetLicenseStatus());
        ApiResult apiResult = Assert.IsType<ApiResult>(jsonResult.Value);

        Assert.Equal(HttpStatusCode.OK, apiResult.Status);
        IEnumerable modules = Assert.IsAssignableFrom<IEnumerable>(GetProperty(apiResult.Data, "modules"));
        object[] moduleItems = modules.Cast<object>().ToArray();
        Assert.Single(moduleItems);
        Assert.Equal("PaddleOCR", GetProperty<string>(moduleItems[0], "Module"));
    }

    [Fact]
    public async Task UploadLicense_WhenOcrVlDisabled_DoesNotValidateOcrVl()
    {
        FakeOcrService ocrService = new();
        OCREngine ocrEngine = new(
            ocrService,
            new OCRConfig { OCRLicense = "missing.lic", det_infer = "", rec_infer = "", cls_infer = "" },
            new LayoutConfig());
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddSingleton(new OCRVLConfig { enabled = false })
            .BuildServiceProvider();
        HomeController controller = new(
            NullLogger<HomeController>.Instance,
            ocrEngine,
            ocrService,
            serviceProvider,
            new OCRVLConfig { enabled = false });

        await using MemoryStream stream = new(new byte[] { 1, 2, 3 });
        FormFile file = new(stream, 0, stream.Length, "file", "paddleocr.lic");

        Microsoft.AspNetCore.Mvc.JsonResult jsonResult =
            Assert.IsType<Microsoft.AspNetCore.Mvc.JsonResult>(await controller.UploadLicense(file));
        ApiResult apiResult = Assert.IsType<ApiResult>(jsonResult.Value);

        Assert.Equal(HttpStatusCode.OK, apiResult.Status);
        IEnumerable modules = Assert.IsAssignableFrom<IEnumerable>(GetProperty(apiResult.Data, "modules"));
        object[] moduleItems = modules.Cast<object>().ToArray();
        Assert.Single(moduleItems);
        Assert.Equal("PaddleOCR", GetProperty<string>(moduleItems[0], "Module"));
    }

    private static object? GetProperty(object source, string propertyName)
    {
        PropertyInfo property = source.GetType().GetProperty(propertyName)
            ?? throw new InvalidOperationException($"Property '{propertyName}' was not found.");
        return property.GetValue(source);
    }

    private static T GetProperty<T>(object source, string propertyName)
    {
        return Assert.IsType<T>(GetProperty(source, propertyName));
    }

    private sealed class FakeOcrService : IOCRService
    {
        public string InitDefaultOCREngine(string modelsPath, bool useV5) => "";
        public string InitDefaultStructureEngine(string modelsPath, bool useV5) => "";
        public bool Init(InitParamater para) => true;
        public string GetLicenseRequestCode() => "request-code";
        public bool ActivateLicense(string licenseFile) => true;
        public string GetLicenseStatus() => "";
        public LicenseStatus GetLicenseStatusInfo() => new() { Activated = true, MachineMatch = true };
        public OCRResult Detect(string imagefile) => new();
        public OCRResult Detect(byte[] imagebyte) => new();
        public OCRResult DetectMat(IntPtr ptr_cvmat) => new();
        public OCRResult DetectBase64(string base64) => new();
        public OCRResult DetectScreenShot(byte[] screenshotData) => new();
        public string GetError() => "";
        public void EnableLog(bool useLog) { }
        public void EnableASCIIResult(bool useANSI) { }
        public void EnableJsonResult(bool enableJson) { }
        public string DetectLayout(string imagefile) => "";
        public string DetectLayoutByte(byte[] imagebyte) => "";
        public string DetectLayoutBase64(string base64) => "";
        public string DetectLayoutMat(IntPtr ptr_cvmat) => "";
        public void FreeEngine() { }
        public void FreeStructureEngine() { }
        public FindImageResult FindImage(string bigImagePath, string smallImagePath, double threshold = 0.8, bool toGray = true, bool useSlideMatch = false) => new();
        public LayoutDetectResult ParseLayoutResult(string json) => new();
        public LayoutDetectResult DetectLayoutParsed(string imagefile) => new();
        public LayoutDetectResult DetectLayoutByteParsed(byte[] imagebyte) => new();
        public LayoutDetectResult DetectLayoutBase64Parsed(string base64) => new();
        public LayoutDetectResult DetectLayoutMatParsed(IntPtr ptr_cvmat) => new();
    }
}
