using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CS16Cheat;

class Program
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(
        uint dwDesiredAccess,
        bool bInheritHandle,
        int dwProcessId
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(
        IntPtr hProcess,
        IntPtr lpBaseAddress,
        byte[] lpBuffer,
        int dwSize,
        out int lpNumberOfBytesRead
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    private const uint PROCESS_VM_READ = 0x0010;
    private const uint PROCESS_QUERY_INFORMATION = 0x0400;
    private static nint _handle;

    internal static int Main(string[] args)
    {
        Console.WriteLine("🎯 CS 1.6 Memory Reader (WinAPI Version)");
        Console.WriteLine("=".PadRight(50, '='));

        Process? game = FindGameProcess("hl");
        if (game == null)
        {
            Console.WriteLine("Start CS 1.6 and try again...");
            return 1;
        }

        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("\nExitting...");
            CloseHandle(_handle);
            Environment.Exit(0);
        };

        _handle = OpenGameProcess(game.Id);
        if (_handle == IntPtr.Zero)
        {
            return 1;
        }

        nint ptrHealth = new(0x059868EC);
        nint ptrAmmo = new(0x05989FC0);
        nint ptrPosX = new(0x1758F444);
        nint ptrPosY = new(0x1758F270);
        nint ptrPosZ = new(0x057C5864);
        Console.WriteLine(
            $"Read health, ammo and player's position from address: {ptrHealth.ToInt64():X}"
        );

        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }

            var health = ReadInt(_handle, ptrHealth);
            var ammo = ReadShort(_handle, ptrAmmo);
            var x = ReadFloat(_handle, ptrPosX);
            var y = ReadFloat(_handle, ptrPosY);
            var z = ReadFloat(_handle, ptrPosZ);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(
                $"Player health: {health ?? 0}\nPlayer ammo: {ammo ?? 0}\nPlayer position: (x:{x ?? 0}|y:{y ?? 0}|z:{z ?? 0})"
            );
            Console.ResetColor();

            Thread.Sleep(100);
        }

        CloseHandle(_handle);
        Console.WriteLine("Handle closed successfully");

        Console.WriteLine("\nPress ENTER to escape...");
        Console.ReadKey();
        return 0;
    }

    private static Process? FindGameProcess(string processName)
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

    private static IntPtr OpenGameProcess(int processId)
    {
        IntPtr handle = OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, processId);

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

    private static int? ReadInt(IntPtr processHandle, IntPtr address)
    {
        var buffer = new byte[4];

        bool success = ReadProcessMemory(
            processHandle,
            address,
            buffer,
            buffer.Length,
            out int bytesRead
        );

        if (!success || bytesRead != 4)
        {
            int errorCode = Marshal.GetLastWin32Error();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error of reading memory in 4 bytes. | ERROR_CODE: {errorCode}");
            Console.ResetColor();
            return null;
        }

        int value = BitConverter.ToInt32(buffer, 0);
        return value;
    }

    private static short? ReadShort(IntPtr processHandle, IntPtr address)
    {
        var buffer = new byte[2];

        bool success = ReadProcessMemory(
            processHandle,
            address,
            buffer,
            buffer.Length,
            out int bytesRead
        );

        if (!success || bytesRead < 2)
        {
            int errorCode = Marshal.GetLastWin32Error();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error of reading memory in 2 bytes. | ERROR_CODE: {errorCode}");
            Console.ResetColor();
            return null;
        }
        short value = BitConverter.ToInt16(buffer, 0);
        return value;
    }

    private static float? ReadFloat(nint processHandle, nint address)
    {
        var buffer = new byte[4];
        bool success = ReadProcessMemory(
            processHandle,
            address,
            buffer,
            buffer.Length,
            out int bytesRead
        );

        if (!success || bytesRead < 4)
        {
            int errorCode = Marshal.GetLastWin32Error();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(
                $"Error of reading memory in 4 bytes(float). | ERROR_CODE: {errorCode}"
            );
            Console.ResetColor();
            return null;
        }

        float value = BitConverter.ToSingle(buffer, 0);
        return value;
    }
}
