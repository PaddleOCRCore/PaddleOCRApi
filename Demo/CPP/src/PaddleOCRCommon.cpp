#include "PaddleOCRCommon.h"

#include <iostream>
#include <windows.h>

#include <PaddleOCR.h>

using namespace std;

void GetFileList(const string& directoryPath, vector<string>& files) {
    WIN32_FIND_DATAA ffd;
    string pattern = directoryPath + "/*";

    HANDLE hFind = FindFirstFileA(pattern.c_str(), &ffd);
    if (hFind == INVALID_HANDLE_VALUE) {
        cerr << "FindFirstFile failed (" << GetLastError() << "): " << directoryPath << endl;
        return;
    }

    do {
        string name = ffd.cFileName;
        if ((ffd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0) {
            if (name != "." && name != "..") {
                GetFileList(directoryPath + "\\" + name, files);
            }
        } else {
            files.push_back(directoryPath + "\\" + name);
        }
    } while (FindNextFileA(hFind, &ffd) != 0);

    FindClose(hFind);
}

string TakeResultAndFree(const char* buffer) {
    if (buffer == nullptr) {
        return "";
    }

    string text(buffer);
    FreeResultBuffer(const_cast<void*>(static_cast<const void*>(buffer)));
    return text;
}

string GetLastErrorAndFree() {
    char* err = GetError();
    if (err == nullptr) {
        return "";
    }

    string text(err);
    FreeResultBuffer(err);
    return text;
}

static bool DirectoryExists(const string& path) {
    DWORD attributes = GetFileAttributesA(path.c_str());
    return attributes != INVALID_FILE_ATTRIBUTES && (attributes & FILE_ATTRIBUTE_DIRECTORY) != 0;
}

static bool FileExists(const string& path) {
    DWORD attributes = GetFileAttributesA(path.c_str());
    return attributes != INVALID_FILE_ATTRIBUTES && (attributes & FILE_ATTRIBUTE_DIRECTORY) == 0;
}

static bool IsPathRooted(const string& path) {
    if (path.size() >= 2 && path[1] == ':') {
        return true;
    }

    return path.size() >= 2 &&
        ((path[0] == '\\' && path[1] == '\\') || (path[0] == '/' && path[1] == '/'));
}

string CombinePath(const string& left, const string& right) {
    if (left.empty()) {
        return right;
    }

    char last = left[left.size() - 1];
    if (last == '\\' || last == '/') {
        return left + right;
    }

    return left + "\\" + right;
}

string GetExecutableDirectory() {
    char path[MAX_PATH] = { 0 };
    DWORD length = GetModuleFileNameA(nullptr, path, MAX_PATH);
    if (length == 0 || length == MAX_PATH) {
        return "";
    }

    string fullPath(path);
    size_t slash = fullPath.find_last_of("\\/");
    if (slash == string::npos) {
        return "";
    }

    return fullPath.substr(0, slash);
}

static bool PrependPathEnvironment(const string& directory) {
    DWORD length = GetEnvironmentVariableA("PATH", nullptr, 0);
    string oldPath;
    if (length > 0) {
        oldPath.resize(length);
        DWORD written = GetEnvironmentVariableA("PATH", &oldPath[0], length);
        oldPath.resize(written);
    }

    if (oldPath.find(directory) != string::npos) {
        return true;
    }

    string newPath = directory + ";" + oldPath;
    return SetEnvironmentVariableA("PATH", newPath.c_str()) != 0;
}

void ConfigureNativeDllDirectory(const string& currentDirectory) {
    vector<string> candidates;
    candidates.push_back(CombinePath(currentDirectory, "runtimes\\win-x64\\native"));
    candidates.push_back(CombinePath(GetExecutableDirectory(), "runtimes\\win-x64\\native"));
    candidates.push_back(currentDirectory);
    candidates.push_back(GetExecutableDirectory());
    candidates.push_back(CombinePath(currentDirectory, "lib"));

    for (const auto& candidate : candidates) {
        if (!candidate.empty() && DirectoryExists(candidate)) {
            SetDllDirectoryA(candidate.c_str());
            PrependPathEnvironment(candidate);
            cout << "Native DLL directory: " << candidate << endl;
            return;
        }
    }

    cerr << "Warning: native DLL directory was not found. Expected runtimes\\win-x64\\native." << endl;
}

bool ActivateLicenseIfExists(const string& baseDirectory, const string& licensePath) {
    static bool licenseActivated = false;
    if (licenseActivated) {
        return true;
    }

    if (licensePath.empty()) {
        return false;
    }

    string resolvedLicensePath = IsPathRooted(licensePath)
        ? licensePath
        : CombinePath(baseDirectory, licensePath);

    if (!FileExists(resolvedLicensePath)) {
        return false;
    }

    cout << "licensePath:" << resolvedLicensePath << endl;
    licenseActivated = ActivateLicense(resolvedLicensePath.c_str());
    if (licenseActivated) {
        cout << "License activated!" << endl;
    } else {
        cerr << "License activation failed: " << GetLastErrorAndFree() << endl;
    }

    return licenseActivated;
}
