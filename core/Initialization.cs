using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CS16Cheat.core;

internal static class Initialization
{
    internal const uint PROCESS_VM_READ = 0x0010;
    internal const uint PROCESS_QUERY_INFORMATION = 0x0400;
    internal static Process _process = null!;
    internal static nint _handle = IntPtr.Zero;

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool CloseHandle(IntPtr hObject);

    internal static void InitProcess()
    {
        Console.WriteLine("[*] Initialization process...");
        _process = Game.FindGameProcess("hl");
        Console.WriteLine("Put process handle in memory...");
        _handle = Game.OpenGameProcess(_process.Id);
    }

    internal static bool InitModuleBaseAddress()
    {
        Console.WriteLine("[*] Initialization base addresses...");

        var module = Game.baseAddresses;
        var keys = module.Keys.ToList();

        Modules[] notInitialized = [];

        foreach (var m in keys)
        {
            nint address = Game.GetModuleBaseAddress(_process, $"{m}.dll");
            if (address == IntPtr.Zero)
            {
                notInitialized.Append(m);
            }
            module[m] = address;
        }

        if (notInitialized.Length > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[!] Initialization end with errors...");
            foreach (var n in notInitialized)
            {
                Console.WriteLine($"[x] Module {n}.dll wasn't found in process...");
            }
            Console.Write(
                "You're sure you want to continue? Some features may NOT work, this will affect the gaming experience [y/N]: "
            );
            Console.ResetColor();
            string answer = Console.ReadLine()?.ToLower() ?? "";
            if (answer == "y")
            {
                return false;
            }
            Console.WriteLine(
                "Perhaps if you notice any bugs and malfunctions, or want to fix this problem, contact the issue on my github: https://github.com/ASDlikeS/CS-1.6-Cheat/issues/new"
            );
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[+] Initialization base addresses complete successfully!");
            Console.ResetColor();
        }
        return true;
    }
}
