using System.Diagnostics;
using System.Runtime.InteropServices;
using static CS16Cheat.core.Game;

namespace CS16Cheat.core;

internal static class Initialization
{
    internal static nint InitProcAndGetHandle()
    {
        Console.WriteLine("[*] Initialization process...");
        Process? game = FindGameProcess("hl");
        if (game == null)
        {
            Console.WriteLine("Start CS 1.6 and try again\nPress any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);
        }
        Console.WriteLine("[+] Process initialized");
        return OpenGameProcess(game.Id);
    }
}
