using System.Runtime.InteropServices;
using CS16Cheat.core;

namespace CS16Cheat;

class Program
{
    const string csProcessName = "hl";

    internal static int Main(string[] args)
    {
        ProcessManager.Initialize(csProcessName);
        ModuleManager.InitializeAll();
        while (true)
        {
            if (Console.KeyAvailable)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    break;
            }

            nint healthAddr = PointerResolver.FollowPointerChain(
                ModuleManager.GetBaseAddress(Offsets.GetModuleFor("health")),
                Offsets.Health
            );
        }
        //---------------------------------------------------------
        if (!ProcessManager.CloseHandleP())
        {
            Console.WriteLine(
                $"[X] Error of closing process_handle | ERROR_CODE: {Marshal.GetLastWin32Error()}"
            );
        }
        Console.WriteLine("\nPress eny key to escape...");
        Console.ReadKey();
        return 0;
    }
}
