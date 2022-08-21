using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace MemoryTools.Native
{
    public static class NativeFunctions
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImport("user32", SetLastError = true)]
        public static extern bool EnableWindow(IntPtr handle, bool enable);
        [DllImport("kernel32", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessRight dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenThread(ThreadAcessRight dwDesiredAccess, bool bInheritHandle, int dwThreadId);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, AllocationType flAllocationType, MemoryProtectionConstants flProtect);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr GetModuleHandleA(string namelpModuleName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "CreateRemoteThreadEx")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, int dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, ThreadCreationFlags dwCreationFlags, IntPtr lpAttributeList, out int lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern WaitForSingleObjectReponse WaitForSingleObjectEx(IntPtr hHandle, int dwMilliseconds, bool alertabAlertable);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, FreeType dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ResumeThread(IntPtr handle);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SuspendThread(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DuplicateHandle(int hSourceProcessHandle, int hSourceHandle, int hTargetProcessHandle, out int lpTargetHandle, ProcessAccessRight dwDesiredAccess, bool bInheritHandle, HandleOptions dwOptions);
        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern NtStatus NtQueryInformationThread(IntPtr ThreadHandle, ThreadInfoClass ThreadInformationClass, IntPtr ThreadInformation, int ThreadInformationLength, IntPtr ReturnLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetThreadContext(IntPtr hThread, ref Context lpContext);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetThreadContext(IntPtr hThread, ref Context lpContext);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint VirtualQueryEx(IntPtr Handle, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PeekMessageA(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PeekMessageW(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetMessageA(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetMessageW(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, MemoryProtectionConstants flNewProtect, out MemoryProtectionConstants lpflOldProtect);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern int MsgWaitForMultipleObjects(
            int nCount,
            IntPtr[] pHandles,
            bool bWaitAll,
            uint dwMilliseconds,
            uint dwWakeMask
            );
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr RegisterWindowMessage(string lpString);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetCurrentThreadId();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    }
}
