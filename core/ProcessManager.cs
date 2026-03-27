using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CS16Cheat.core;

internal static class ProcessManager
{
    private static readonly string[] processNames = ["hl", "cs"];

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint OpenProcess(
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

    internal static nint Handle { get; private set; }
    internal static Process GameProcess { get; private set; } = null!;

    internal static void Initialize()
    {
        Console.WriteLine("[*] Process hl.exe initialization...");
        GameProcess = FindProcess(processNames);
        Handle = OpenGameProcess(GameProcess.Id);
    }

    internal static bool CloseHandleP()
    {
        try
        {
            return CloseHandle(Handle);
        }
        catch
        {
            return false;
        }
    }

    private static Process FindProcess(string[] processNames)
    {
        Process process = new();
        foreach(var proc in processNames)
        {
            var findProcList = Process.GetProcessesByName(proc);
            if(findProcList.Length > 0)
            {
                process = findProcList[0];
                break;
            }
        }
        if (process == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[X] There's no processes from process list here...");
            Console.ResetColor();
            Console.ReadKey();
            Console.WriteLine("Press any key to exit...");  
            Environment.Exit(1);
        }

        Console.WriteLine($"[+] Process found: {process.ProcessName}.exe (PID: {process.Id})");
        return process;
    }

    private static nint OpenGameProcess(int processId)
    {
        uint access =
            PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION | PROCESS_QUERY_INFORMATION;
        nint handle = OpenProcess(access, false, processId);

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
