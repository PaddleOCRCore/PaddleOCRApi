using System.Reflection;
using PaddleOCRSDK;
using Xunit;

namespace OCRCoreService.Tests;

public class NativeRuntimeLoaderTests
{
    [Fact]
    public void ResolveNativeDirectory_PrefersRidNativeDirectory()
    {
        string root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        string nativeDir = Path.Combine(root, "runtimes", "win-x64", "native");
        Directory.CreateDirectory(nativeDir);
        File.WriteAllText(Path.Combine(nativeDir, "PaddleOCR.dll"), "");

        try
        {
            Assembly sdkAssembly = typeof(OCRService).Assembly;
            Type loaderType = sdkAssembly.GetType("PaddleOCRSDK.NativeRuntimeLoader", throwOnError: true)!;
            MethodInfo method = loaderType.GetMethod(
                "ResolveNativeDirectory",
                BindingFlags.Static | BindingFlags.NonPublic)!;

            string? actual = (string?)method.Invoke(null, new object[] { root, Path.Combine(root, "other") });

            Assert.Equal(nativeDir, actual);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
