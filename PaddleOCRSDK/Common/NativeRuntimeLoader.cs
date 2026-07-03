using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace PaddleOCRSDK
{
    internal static class NativeRuntimeLoader
    {
        private static readonly object SyncRoot = new object();
        private static bool _loaded;

        internal static void EnsureLoaded()
        {
            if (_loaded)
            {
                return;
            }

            lock (SyncRoot)
            {
                if (_loaded)
                {
                    return;
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string nativeDirectory = ResolveNativeDirectory(AppContext.BaseDirectory, Directory.GetCurrentDirectory());
                    if (!string.IsNullOrWhiteSpace(nativeDirectory))
                    {
                        RegisterWindowsNativeDirectory(nativeDirectory);
                    }
                }

                _loaded = true;
            }
        }

        internal static string ResolveNativeDirectory(string baseDirectory, string currentDirectory)
        {
            foreach (string candidate in GetCandidateDirectories(baseDirectory, currentDirectory))
            {
                if (Directory.Exists(candidate) && ContainsPaddleNativeLibrary(candidate))
                {
                    return candidate;
                }
            }

            return string.Empty;
        }

        private static IEnumerable<string> GetCandidateDirectories(string baseDirectory, string currentDirectory)
        {
            if (!string.IsNullOrWhiteSpace(baseDirectory))
            {
                yield return Path.Combine(baseDirectory, "runtimes", "win-x64", "native");
                yield return baseDirectory;
            }

            if (!string.IsNullOrWhiteSpace(currentDirectory))
            {
                yield return Path.Combine(currentDirectory, "runtimes", "win-x64", "native");
                yield return currentDirectory;
            }
        }

        private static bool ContainsPaddleNativeLibrary(string directory)
        {
            return File.Exists(Path.Combine(directory, "PaddleOCR.dll"))
                || File.Exists(Path.Combine(directory, "llamaocr-vl.dll"));
        }

        private static void RegisterWindowsNativeDirectory(string nativeDirectory)
        {
            string fullPath = Path.GetFullPath(nativeDirectory);
            string currentPath = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            if (!PathContainsDirectory(currentPath, fullPath))
            {
                Environment.SetEnvironmentVariable("PATH", fullPath + Path.PathSeparator + currentPath);
            }

            SetDllDirectory(fullPath);
        }

        private static bool PathContainsDirectory(string pathValue, string directory)
        {
            string[] parts = pathValue.Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                string normalizedPart;
                try
                {
                    normalizedPart = Path.GetFullPath(part.Trim());
                }
                catch
                {
                    continue;
                }

                if (string.Equals(normalizedPart.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                    directory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        [DllImport("kernel32", EntryPoint = "SetDllDirectoryW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);
    }
}
