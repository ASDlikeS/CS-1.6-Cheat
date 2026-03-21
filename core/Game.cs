using System.Diagnostics;
using System.Runtime.InteropServices;
using static CS16Cheat.core.Memory;

namespace CS16Cheat.core;

internal static class Game
{
    internal static string[] GAME_MODULES = ["hw.dll", "client.dll"];

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(
        uint dwDesiredAccess,
        bool bInheritHandle,
        int dwProcessId
    );

    internal static Process? FindGameProcess(string processName)
    {
        Process[] processes = Process.GetProcessesByName(processName);

        if (processes.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Process {processName}.exe not found");
            Console.ResetColor();
            return null;
        }

        Process gameProcess = processes[0];
        Console.WriteLine($"Process found: {gameProcess.ProcessName}.exe (PID: {gameProcess.Id})");

        return gameProcess;
    }

    internal static IntPtr OpenGameProcess(int processId)
    {
        IntPtr handle = OpenProcess(
            Program.PROCESS_VM_READ | Program.PROCESS_QUERY_INFORMATION,
            false,
            processId
        );

        if (handle == IntPtr.Zero)
        {
            int errorCode = Marshal.GetLastWin32Error();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(
                $"Error of opening process PID: {processId} | ERROR CODE: {errorCode}"
            );
            Console.ResetColor();
            Console.WriteLine("Try to restart utility with admin rights...");
            return IntPtr.Zero;
        }
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Process opened successfully! Handle: {handle}");
        Console.ResetColor();
        return handle;
    }

    internal static nint GetModuleBaseAddress(Process process, string moduleName)
    {
        foreach (ProcessModule module in process.Modules)
        {
            if (module.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(
                    $"[+] Module {moduleName} found at 0x{module.BaseAddress.ToInt64():X}"
                );
                Console.ResetColor();
                return module.BaseAddress;
            }
        }
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[X] Module {moduleName} not found!");
        Console.ResetColor();
        return IntPtr.Zero;
    }

    internal static nint FollowPointerChain(nint processHandle, nint startAddress, int[] offsets)
    {
        nint currentAddress = startAddress;

        byte[] buffer = new byte[4];

        for (int i = 0; i < offsets.Length - 1; i++)
        {
            if (!ReadProcessMemory(processHandle, currentAddress, buffer, buffer.Length, out _))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[X] Failed to read at 0x{currentAddress.ToInt64():X}");
                Console.ResetColor();
                return IntPtr.Zero;
            }

            currentAddress = (nint)BitConverter.ToInt32(buffer, 0);
            currentAddress += offsets[i];
        }

        if (offsets.Length > 0)
        {
            currentAddress += offsets[^1];
        }

        return currentAddress;
    }
}
