using System.Diagnostics;
using System.Runtime.InteropServices;
using CS16Cheat.utils;

namespace CS16Cheat.LWOperations;

static class ProcessManager
{
    private static readonly string[] processNames = ["hl", "cstrike", "cs"];

    private const uint PROCESS_VM_READ = 0x0010;
    private const uint PROCESS_VM_WRITE = 0x0020;
    private const uint PROCESS_VM_OPERATION = 0x0008;
    private const uint PROCESS_QUERY_INFORMATION = 0x0400;

    private static uint DesiredAccess =>
        PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION | PROCESS_QUERY_INFORMATION;

    internal static nint Handle { get; private set; }
    internal static nint ProcessWindowHandle { get; private set; }
    internal static Process? GameProcess { get; private set; }
    internal static int ProcessId => GameProcess?.Id ?? 0;

    [DllImport("kernel32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern nint OpenProcess(
        uint dwDesiredAccess,
        bool bInheritHandle,
        int dwProcessId
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static extern bool CloseHandle(IntPtr hObject);

    internal static void Initialize()
    {
        Console.WriteLine("[*] Initializing process manager...");

        GameProcess = FindExistingProcess();

        if (GameProcess != null)
        {
            Console.WriteLine(
                $"[+] Found existing game: {GameProcess.ProcessName}.exe (PID: {GameProcess.Id})"
            );
        }
        else
        {
            Console.WriteLine("[*] Starting game...");
            if (!LaunchGame())
            {
                Console.WriteLine("WARNING: Couldn't start game process automatically.");
                Console.WriteLine("Please start the game manually and press any key when ready...");
                Console.ReadKey();

                GameProcess = FindExistingProcess();

                if (GameProcess == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[X] Game process not found. Exiting...");
                    Console.ResetColor();
                    Console.ReadKey();
                    Console.WriteLine("Press any key to exit...");
                    Environment.Exit(1);
                }
            }
        }

        ProcessWindowHandle = GetWindowHandle(GameProcess!);
        Handle = OpenGameProcess(GameProcess!.Id);
        Console.WriteLine("[+] Process manager initialized successfully");
    }

    private static Process? FindExistingProcess()
    {
        foreach (var procName in processNames)
        {
            var processes = Process.GetProcessesByName(procName);
            if (processes.Length > 0)
            {
                return processes[0];
            }
        }
        return null;
    }

    private static nint GetWindowHandle(Process process)
    {
        for (int i = 0; i < 30; i++)
        {
            process.Refresh();
            if (process.MainWindowHandle != IntPtr.Zero)
            {
                Console.WriteLine($"[+] Window handle: 0x{process.MainWindowHandle:X}");
                return process.MainWindowHandle;
            }
            Thread.Sleep(100);
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[!] Warning: Could not get window handle. ESP overlay may not work.");
        Console.ResetColor();
        return IntPtr.Zero;
    }

    private static bool LaunchGame()
    {
        string gamePath = Config.ConfigData.GamePath;

        if (string.IsNullOrEmpty(gamePath) || !File.Exists(gamePath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[X] Game not found at: {gamePath}");
            Console.ResetColor();
            return false;
        }

        string gameDirectory = Path.GetDirectoryName(gamePath) ?? string.Empty;

        ProcessStartInfo startInfo = new()
        {
            FileName = gamePath,
            WorkingDirectory = gameDirectory,
            Arguments = "-windowed -noborder",
            UseShellExecute = false,
        };

        try
        {
            Process? gameProc = Process.Start(startInfo);

            if (gameProc == null)
                return false;

            gameProc.WaitForInputIdle(5000);
            Thread.Sleep(500);

            gameProc.Refresh();

            Console.WriteLine($"[+] Game launched (PID: {gameProc.Id})");
            GameProcess = gameProc;
            return true;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[X] Launch failed: {ex.Message}");
            Console.ResetColor();
            return false;
        }
    }

    private static nint OpenGameProcess(int processId)
    {
        nint handle = OpenProcess(DesiredAccess, false, processId);

        if (handle == IntPtr.Zero)
        {
            int error = Marshal.GetLastWin32Error();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[X] Failed to open process. Error: {error}");

            if (error == 5)
            {
                Console.WriteLine("[!] Try running the cheat as Administrator.");
            }

            Console.ResetColor();
            Environment.Exit(1);
        }

        Console.WriteLine($"[+] Process handle: 0x{handle:X}");
        return handle;
    }

    internal static void Dispose()
    {
        if (Handle != IntPtr.Zero)
        {
            CloseHandle(Handle);
            Handle = IntPtr.Zero;
        }
    }
}
