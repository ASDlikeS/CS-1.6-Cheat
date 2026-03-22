using System.Diagnostics;
using System.Runtime.InteropServices;
using static CS16Cheat.core.Memory;

namespace CS16Cheat.core;

internal enum Modules
{
    hw,
    client,
}

internal static class Game
{
    internal static Dictionary<Modules, nint> baseAddresses = new()
    {
        { Modules.hw, IntPtr.Zero },
        { Modules.client, IntPtr.Zero },
    };

    internal static Dictionary<string, Modules> valuesAndModules = new()
    {
        { "health", Modules.hw },
        { "currentSlotAmmo", Modules.hw },
        { "money", Modules.client },
    };

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(
        uint dwDesiredAccess,
        bool bInheritHandle,
        int dwProcessId
    );

    internal static Process FindGameProcess(string processName)
    {
        Process[] processes = Process.GetProcessesByName(processName);

        if (processes.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[X] Process {processName}.exe not found");
            Console.ResetColor();
            Console.WriteLine("Start CS 1.6 and try again\nPress any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);
        }
        var process = processes[0];

        Console.WriteLine($"[+] Process found: {process.ProcessName}.exe (PID: {process.Id})");
        return process;
    }

    internal static IntPtr OpenGameProcess(int processId)
    {
        IntPtr handle = OpenProcess(
            Initialization.PROCESS_VM_READ | Initialization.PROCESS_QUERY_INFORMATION,
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
        return IntPtr.Zero;
    }

    internal static nint FollowPointerChain(nint startAddress, int[] offsets) // client.dll + 0x1234 + 0x7D + 0x1230
    {
        nint currentAddress = startAddress + offsets[0];

        byte[] buffer = new byte[4];

        for (int i = 1; i < offsets.Length; i++)
        {
            if (
                !ReadProcessMemory(
                    Initialization._handle,
                    currentAddress,
                    buffer,
                    buffer.Length,
                    out _
                )
            )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[X] Failed to read at 0x{currentAddress.ToInt64():X}");
                Console.ResetColor();
                return IntPtr.Zero;
            }

            currentAddress = (nint)BitConverter.ToInt32(buffer, 0);
            currentAddress += offsets[i];
        }

        return currentAddress;
    }
}
