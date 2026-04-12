using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinFormsApp
{
    internal sealed class NativeConsoleRedirector
    {
        private const int StdOutputHandle = -11;
        private const int StdErrorHandle = -12;
        private const int OText = 0x4000;
        private const uint DuplicateSameAccess = 0x00000002;
        private const int SwHide = 0;
        private const uint WaitObject0 = 0x00000000;
        private const uint WaitTimeout = 0x00000102;
        private const uint Infinite = 0xFFFFFFFF;
        private const uint PageReadWrite = 0x04;
        private const uint FileMapRead = 0x0004;
        private static readonly IntPtr InvalidFileHandle = new IntPtr(-1);
        private const string DbWinBufferReadyEventName = "DBWIN_BUFFER_READY";
        private const string DbWinDataReadyEventName = "DBWIN_DATA_READY";
        private const string DbWinBufferName = "DBWIN_BUFFER";
        private static readonly IntPtr InvalidHandleValue = new IntPtr(-1);
        private const uint HandleFlagInherit = 0x00000001;

        private readonly object syncLock = new object();
        private IntPtr originalStdOut = IntPtr.Zero;
        private IntPtr originalStdErr = IntPtr.Zero;
        private IntPtr pipeWrite = IntPtr.Zero;
        private SafeFileHandle? pipeRead;
        private CancellationTokenSource? cts;
        private Task? readTask;
        private CancellationTokenSource? monitorCts;
        private Task? monitorTask;
        private CancellationTokenSource? debugCts;
        private Task? debugTask;
        private Action<string>? lineHandler;
        private bool started;
        private int savedStdOutFd = -1;
        private int savedStdErrFd = -1;
        private int pipeWriteFd = -1;
        private int savedStdOutFdMsvcrt = -1;
        private int savedStdErrFdMsvcrt = -1;
        private int pipeWriteFdMsvcrt = -1;
        private bool msvcrtRedirected;
        private bool allocatedConsole;

        public void Start(Action<string> onLine)
        {
            if (onLine == null)
            {
                throw new ArgumentNullException(nameof(onLine));
            }

            lock (syncLock)
            {
                lineHandler = onLine;

                if (started)
                {
                    // Native libraries may overwrite process std handles after startup.
                    // Rebind to our pipe to keep capture stable.
                    RebindToPipeHandles();
                    EnsureReaderTaskRunning();
                    return;
                }

                EnsureHiddenConsoleReady();

                originalStdOut = GetStdHandle(StdOutputHandle);
                originalStdErr = GetStdHandle(StdErrorHandle);

                SECURITY_ATTRIBUTES securityAttributes = new SECURITY_ATTRIBUTES
                {
                    nLength = Marshal.SizeOf<SECURITY_ATTRIBUTES>(),
                    bInheritHandle = true,
                    lpSecurityDescriptor = IntPtr.Zero
                };

                if (!CreatePipe(out IntPtr readHandle, out IntPtr writeHandle, ref securityAttributes, 0))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "CreatePipe failed.");
                }

                // Parent read end should not be inheritable.
                SetHandleInformation(readHandle, HandleFlagInherit, 0);

                bool stdOutSwitched = false;
                bool stdErrSwitched = false;
                bool crtRedirected = false;
                try
                {
                    if (!SetStdHandle(StdOutputHandle, writeHandle))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "SetStdHandle(STDOUT) failed.");
                    }

                    stdOutSwitched = true;

                    if (!SetStdHandle(StdErrorHandle, writeHandle))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "SetStdHandle(STDERR) failed.");
                    }

                    stdErrSwitched = true;
                    RedirectCrtStdHandles(writeHandle);
                    crtRedirected = true;
                    pipeRead = new SafeFileHandle(readHandle, ownsHandle: true);
                    pipeWrite = writeHandle;
                    cts = new CancellationTokenSource();
                    readTask = Task.Run(() => ReadLoop(onLine, cts.Token));
                    monitorCts = new CancellationTokenSource();
                    monitorTask = Task.Run(() => MonitorLoop(monitorCts.Token));
                    EnsureDebugTaskRunning();
                    started = true;
                }
                catch
                {
                    if (crtRedirected)
                    {
                        RestoreCrtStdHandles();
                    }

                    if (stdErrSwitched && originalStdErr != IntPtr.Zero)
                    {
                        SetStdHandle(StdErrorHandle, originalStdErr);
                    }

                    if (stdOutSwitched && originalStdOut != IntPtr.Zero)
                    {
                        SetStdHandle(StdOutputHandle, originalStdOut);
                    }

                    CloseHandle(readHandle);
                    CloseHandle(writeHandle);
                    throw;
                }
            }
        }

        public void Stop()
        {
            Task? readerToWait = null;
            Task? monitorToWait = null;
            Task? debugToWait = null;
            lock (syncLock)
            {
                if (!started)
                {
                    return;
                }

                RestoreCrtStdHandles();

                if (originalStdOut != IntPtr.Zero)
                {
                    SetStdHandle(StdOutputHandle, originalStdOut);
                }

                if (originalStdErr != IntPtr.Zero)
                {
                    SetStdHandle(StdErrorHandle, originalStdErr);
                }

                cts?.Cancel();
                monitorCts?.Cancel();
                debugCts?.Cancel();

                if (pipeWrite != IntPtr.Zero)
                {
                    CloseHandle(pipeWrite);
                    pipeWrite = IntPtr.Zero;
                }

                readerToWait = readTask;
                monitorToWait = monitorTask;
                debugToWait = debugTask;

                readTask = null;
                cts?.Dispose();
                cts = null;
                monitorTask = null;
                monitorCts?.Dispose();
                monitorCts = null;
                debugTask = null;
                debugCts?.Dispose();
                debugCts = null;
                pipeRead?.Dispose();
                pipeRead = null;
                lineHandler = null;

                if (allocatedConsole)
                {
                    FreeConsole();
                    allocatedConsole = false;
                }

                started = false;
            }

            if (readerToWait != null)
            {
                try
                {
                    readerToWait.Wait(300);
                }
                catch
                {
                }
            }

            if (monitorToWait != null)
            {
                try
                {
                    monitorToWait.Wait(300);
                }
                catch
                {
                }
            }

            if (debugToWait != null)
            {
                try
                {
                    debugToWait.Wait(300);
                }
                catch
                {
                }
            }
        }

        private void EnsureReaderTaskRunning()
        {
            if (!started || pipeRead == null || pipeRead.IsInvalid)
            {
                return;
            }

            if (lineHandler == null)
            {
                return;
            }

            if (readTask != null && !readTask.IsCompleted)
            {
                return;
            }

            if (cts == null || cts.IsCancellationRequested)
            {
                cts?.Dispose();
                cts = new CancellationTokenSource();
            }

            readTask = Task.Run(() => ReadLoop(lineHandler, cts.Token));
        }

        private void EnsureDebugTaskRunning()
        {
            if (!started || lineHandler == null)
            {
                return;
            }

            if (debugTask != null && !debugTask.IsCompleted)
            {
                return;
            }

            if (debugCts == null || debugCts.IsCancellationRequested)
            {
                debugCts?.Dispose();
                debugCts = new CancellationTokenSource();
            }

            debugTask = Task.Run(() => DebugOutputLoop(lineHandler, debugCts.Token));
        }

        private async Task MonitorLoop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    lock (syncLock)
                    {
                        if (!started)
                        {
                            return;
                        }

                        if (pipeWrite != IntPtr.Zero)
                        {
                            IntPtr stdOut = GetStdHandle(StdOutputHandle);
                            IntPtr stdErr = GetStdHandle(StdErrorHandle);
                            if (stdOut != pipeWrite || stdErr != pipeWrite)
                            {
                                RebindToPipeHandles();
                            }
                        }

                        EnsureReaderTaskRunning();
                        EnsureDebugTaskRunning();
                    }

                    await Task.Delay(700, token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch
            {
            }
        }

        private void DebugOutputLoop(Action<string> onLine, CancellationToken token)
        {
            IntPtr hBufferReady = IntPtr.Zero;
            IntPtr hDataReady = IntPtr.Zero;
            IntPtr hMap = IntPtr.Zero;
            IntPtr pBuf = IntPtr.Zero;

            try
            {
                hBufferReady = CreateEvent(IntPtr.Zero, false, false, DbWinBufferReadyEventName);
                hDataReady = CreateEvent(IntPtr.Zero, false, false, DbWinDataReadyEventName);
                hMap = CreateFileMapping(InvalidFileHandle, IntPtr.Zero, PageReadWrite, 0, 4096, DbWinBufferName);

                if (hBufferReady == IntPtr.Zero || hDataReady == IntPtr.Zero || hMap == IntPtr.Zero)
                {
                    return;
                }

                pBuf = MapViewOfFile(hMap, FileMapRead, 0, 0, new UIntPtr(4096));
                if (pBuf == IntPtr.Zero)
                {
                    return;
                }

                while (!token.IsCancellationRequested)
                {
                    SetEvent(hBufferReady);
                    uint wait = WaitForSingleObject(hDataReady, 400);
                    if (wait == WaitTimeout)
                    {
                        continue;
                    }

                    if (wait != WaitObject0)
                    {
                        continue;
                    }

                    int pid = Marshal.ReadInt32(pBuf);
                    if (pid != 0 && pid != Environment.ProcessId)
                    {
                        continue;
                    }

                    IntPtr textPtr = IntPtr.Add(pBuf, 4);
                    string? msg = Marshal.PtrToStringAnsi(textPtr);
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        TryEmitLine(onLine, $"[DebugOut] {msg.Trim()} ");
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (pBuf != IntPtr.Zero)
                {
                    UnmapViewOfFile(pBuf);
                }

                if (hMap != IntPtr.Zero)
                {
                    CloseHandle(hMap);
                }

                if (hDataReady != IntPtr.Zero)
                {
                    CloseHandle(hDataReady);
                }

                if (hBufferReady != IntPtr.Zero)
                {
                    CloseHandle(hBufferReady);
                }
            }
        }

        private void RedirectCrtStdHandles(IntPtr writeHandle)
        {
            IntPtr process = GetCurrentProcess();
            if (!DuplicateHandle(process, writeHandle, process, out IntPtr crtWriteHandle, 0, true, DuplicateSameAccess))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "DuplicateHandle failed.");
            }

            pipeWriteFd = OpenOsfHandle(crtWriteHandle, OText);
            if (pipeWriteFd < 0)
            {
                CloseHandle(crtWriteHandle);
                throw new Win32Exception(Marshal.GetLastWin32Error(), "_open_osfhandle failed.");
            }

            savedStdOutFd = Dup(1);
            savedStdErrFd = Dup(2);
            if (savedStdOutFd < 0 || savedStdErrFd < 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "_dup stdout/stderr failed.");
            }

            if (Dup2(pipeWriteFd, 1) != 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "_dup2 stdout failed.");
            }

            if (Dup2(pipeWriteFd, 2) != 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "_dup2 stderr failed.");
            }

            TryRedirectMsvcrt(writeHandle);
        }

        private void TryRedirectMsvcrt(IntPtr writeHandle)
        {
            try
            {
                IntPtr process = GetCurrentProcess();
                if (!DuplicateHandle(process, writeHandle, process, out IntPtr crtWriteHandle, 0, true, DuplicateSameAccess))
                {
                    return;
                }

                pipeWriteFdMsvcrt = OpenOsfHandleMsvcrt(crtWriteHandle, OText);
                if (pipeWriteFdMsvcrt < 0)
                {
                    CloseHandle(crtWriteHandle);
                    return;
                }

                savedStdOutFdMsvcrt = DupMsvcrt(1);
                savedStdErrFdMsvcrt = DupMsvcrt(2);
                if (savedStdOutFdMsvcrt < 0 || savedStdErrFdMsvcrt < 0)
                {
                    return;
                }

                if (Dup2Msvcrt(pipeWriteFdMsvcrt, 1) != 0)
                {
                    return;
                }

                if (Dup2Msvcrt(pipeWriteFdMsvcrt, 2) != 0)
                {
                    return;
                }

                msvcrtRedirected = true;
            }
            catch
            {
            }
        }

        private void EnsureHiddenConsoleReady()
        {
            IntPtr stdOut = GetStdHandle(StdOutputHandle);
            IntPtr stdErr = GetStdHandle(StdErrorHandle);
            bool hasStdOut = stdOut != IntPtr.Zero && stdOut != InvalidHandleValue;
            bool hasStdErr = stdErr != IntPtr.Zero && stdErr != InvalidHandleValue;

            if (hasStdOut && hasStdErr)
            {
                return;
            }

            if (!AllocConsole())
            {
                return;
            }

            allocatedConsole = true;
            IntPtr consoleWindow = GetConsoleWindow();
            if (consoleWindow != IntPtr.Zero)
            {
                ShowWindow(consoleWindow, SwHide);
            }
        }

        private void RestoreCrtStdHandles()
        {
            if (savedStdOutFd >= 0)
            {
                Dup2(savedStdOutFd, 1);
                CloseFd(savedStdOutFd);
                savedStdOutFd = -1;
            }

            if (savedStdErrFd >= 0)
            {
                Dup2(savedStdErrFd, 2);
                CloseFd(savedStdErrFd);
                savedStdErrFd = -1;
            }

            if (pipeWriteFd >= 0)
            {
                CloseFd(pipeWriteFd);
                pipeWriteFd = -1;
            }

            if (msvcrtRedirected)
            {
                if (savedStdOutFdMsvcrt >= 0)
                {
                    Dup2Msvcrt(savedStdOutFdMsvcrt, 1);
                    CloseFdMsvcrt(savedStdOutFdMsvcrt);
                    savedStdOutFdMsvcrt = -1;
                }

                if (savedStdErrFdMsvcrt >= 0)
                {
                    Dup2Msvcrt(savedStdErrFdMsvcrt, 2);
                    CloseFdMsvcrt(savedStdErrFdMsvcrt);
                    savedStdErrFdMsvcrt = -1;
                }

                if (pipeWriteFdMsvcrt >= 0)
                {
                    CloseFdMsvcrt(pipeWriteFdMsvcrt);
                    pipeWriteFdMsvcrt = -1;
                }

                msvcrtRedirected = false;
            }
        }

        private void RebindToPipeHandles()
        {
            if (pipeWrite == IntPtr.Zero)
            {
                return;
            }

            SetStdHandle(StdOutputHandle, pipeWrite);
            SetStdHandle(StdErrorHandle, pipeWrite);

            if (pipeWriteFd >= 0)
            {
                Dup2(pipeWriteFd, 1);
                Dup2(pipeWriteFd, 2);
            }

            if (pipeWriteFdMsvcrt >= 0)
            {
                Dup2Msvcrt(pipeWriteFdMsvcrt, 1);
                Dup2Msvcrt(pipeWriteFdMsvcrt, 2);
            }
        }

        private void ReadLoop(Action<string> onLine, CancellationToken token)
        {
            SafeFileHandle? readHandle = pipeRead;
            if (readHandle == null || readHandle.IsInvalid)
            {
                return;
            }

            try
            {
                using FileStream stream = new FileStream(readHandle, FileAccess.Read, 4096, isAsync: false);
                byte[] buffer = new byte[2048];
                StringBuilder pending = new StringBuilder();
                while (!token.IsCancellationRequested)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                    {
                        break;
                    }

                    string chunk = Encoding.UTF8.GetString(buffer, 0, read)
                        .Replace("\r\n", "\n")
                        .Replace('\r', '\n');

                    pending.Append(chunk);

                    // Some native modules flush output without trailing newlines.
                    // Emit the partial text immediately to avoid apparent log loss.
                    if (chunk.IndexOf('\n') < 0 && pending.Length > 0)
                    {
                        string partial = pending.ToString().Trim();
                        TryEmitLine(onLine, partial);

                        pending.Clear();
                        continue;
                    }

                    while (true)
                    {
                        int newlineIndex = pending.ToString().IndexOf('\n');
                        if (newlineIndex < 0)
                        {
                            break;
                        }

                        string line = pending.ToString(0, newlineIndex);
                        pending.Remove(0, newlineIndex + 1);
                        TryEmitLine(onLine, line);
                    }
                }

                if (pending.Length > 0)
                {
                    string rest = pending.ToString().Trim();
                    TryEmitLine(onLine, rest);
                }
            }
            catch
            {
            }
        }

        private static void TryEmitLine(Action<string> onLine, string? line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return;
            }

            try
            {
                onLine(line);
            }
            catch
            {
                // Ignore per-line callback errors to keep native log pump alive.
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CreatePipe(out IntPtr hReadPipe, out IntPtr hWritePipe, ref SECURITY_ATTRIBUTES lpPipeAttributes, int nSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetHandleInformation(IntPtr hObject, uint dwMask, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetStdHandle(int nStdHandle, IntPtr hHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DuplicateHandle(
            IntPtr hSourceProcessHandle,
            IntPtr hSourceHandle,
            IntPtr hTargetProcessHandle,
            out IntPtr lpTargetHandle,
            uint dwDesiredAccess,
            bool bInheritHandle,
            uint dwOptions);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("ucrtbase.dll", EntryPoint = "_open_osfhandle", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern int OpenOsfHandle(IntPtr osfhandle, int flags);

        [DllImport("ucrtbase.dll", EntryPoint = "_dup", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern int Dup(int fd);

        [DllImport("ucrtbase.dll", EntryPoint = "_dup2", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern int Dup2(int sourceFd, int targetFd);

        [DllImport("ucrtbase.dll", EntryPoint = "_close", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern int CloseFd(int fd);

        [DllImport("msvcrt.dll", EntryPoint = "_open_osfhandle", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern int OpenOsfHandleMsvcrt(IntPtr osfhandle, int flags);

        [DllImport("msvcrt.dll", EntryPoint = "_dup", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern int DupMsvcrt(int fd);

        [DllImport("msvcrt.dll", EntryPoint = "_dup2", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern int Dup2Msvcrt(int sourceFd, int targetFd);

        [DllImport("msvcrt.dll", EntryPoint = "_close", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern int CloseFdMsvcrt(int fd);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, UIntPtr dwNumberOfBytesToMap);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetEvent(IntPtr hEvent);

        [StructLayout(LayoutKind.Sequential)]
        private struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            [MarshalAs(UnmanagedType.Bool)]
            public bool bInheritHandle;
        }
    }
}
