using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CS16Cheat.core;

internal static class ProcessManager
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(
        uint dwDesiredAccess,
        bool bInheritHandle,
        int dwProcessId
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool CloseHandle(IntPtr hObject);

    internal const uint PROCESS_VM_READ = 0x0010;
    internal const uint PROCESS_VM_WRITE = 0x0020;
    internal const uint PROCESS_VM_OPERATION = 0x0008;
    internal const uint PROCESS_QUERY_INFORMATION = 0x0400;

    internal static IntPtr Handle { get; private set; }
    internal static Process GameProcess { get; private set; } = null!;

    internal static void Initialize(string processName)
    {
        GameProcess = FindProcess(processName);
        Handle = OpenGameProcess(GameProcess.Id);
    }

    internal static bool CloseHandleP()
    {
        return CloseHandle(Handle);
    }

    private static Process FindProcess(string processName)
    {
        var processes = Process.GetProcessesByName(processName);
        if (processes.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[X] Process {processName}.exe not found");
            Console.ResetColor();
            Environment.Exit(1);
        }

        var process = processes[0];
        Console.WriteLine($"[+] Process found: {process.ProcessName}.exe (PID: {process.Id})");
        return process;
    }

    private static IntPtr OpenGameProcess(int processId)
    {
        uint access =
            PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION | PROCESS_QUERY_INFORMATION;
        IntPtr handle = OpenProcess(access, false, processId);

        if (handle == IntPtr.Zero)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[-] Failed to open process. Error: {Marshal.GetLastWin32Error()}");
            Console.ResetColor();
            Environment.Exit(1);
        }

        Console.WriteLine($"[+] Process opened successfully");
        return handle;
    }
}
