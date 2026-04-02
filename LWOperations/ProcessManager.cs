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
    internal static nint ProcessHwnd { get; private set; }
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
            Utils.WriteSuccessMessage(
                $"[+] Found existing game: {GameProcess.ProcessName}.exe (PID: {GameProcess.Id})"
            );
        }
        else
        {
            Console.WriteLine("[*] Starting game...");
            if (!LaunchGame())
            {
                Utils.WriteWarningMessage("[!] Couldn't start game process automatically.");
                Console.WriteLine(
                    "Waiting starting game manually.\nPress any key if game launched..."
                );
                Console.ReadKey();

                GameProcess = FindExistingProcess();

                if (GameProcess == null)
                {
                    Utils.WriteErrorMessage("[X] Game process not found. Exiting...");
                    Console.ReadKey();
                    Console.WriteLine("Press any key to exit...");
                    Environment.Exit(1);
                }
            }
        }

        ProcessHwnd = GetWindowHandle(GameProcess!);
        Handle = OpenGameProcess(GameProcess!.Id);
        Utils.WriteSuccessMessage("[+] Process manager initialized successfully");
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
                Utils.WriteSuccessMessage($"[+] Window handle: 0x{process.MainWindowHandle:X}");
                return process.MainWindowHandle;
            }
            Thread.Sleep(100);
        }

        throw new DllNotFoundException(
            "[X] Couldn't get window handle for overlay... | Try restart the game"
        );
    }

    private static bool LaunchGame()
    {
        string? gamePath = Config.ConfigData?.GamePath;

        if (gamePath == null)
        {
            Utils.WriteErrorMessage($"[X] Game not found.\nYou have to start the game manually...");
            Console.ReadKey();
            Environment.Exit(1);
        }

        string? gameDirectory = Path.GetDirectoryName(gamePath);

        if (string.IsNullOrEmpty(gameDirectory))
        {
            Utils.WriteErrorMessage("[X] Incorrect game path");
            Config.ChangeGamePath();
        }

        ProcessStartInfo startInfo = new()
        {
            FileName = gamePath,
            WorkingDirectory = gameDirectory,
            Arguments = "-windowed -noforcemparms -noforcemaccel -stretchaspect",
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
            Utils.WriteErrorMessage($"[X] Failed to open process. Error: {error}");

            if (error == 5)
            {
                Utils.WriteWarningMessage("[!] Try running the cheat as Administrator.");
            }
            Environment.Exit(1);
        }

        Utils.WriteSuccessMessage($"[+] Process handle: 0x{handle:X}");
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
