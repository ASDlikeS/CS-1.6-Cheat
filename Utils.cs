using CS16Cheat.core;

namespace CS16Cheat;

internal static class Utils
{
    internal static void CancelCB()
    {
        Console.WriteLine("[*] Initialization cancelling callback function...");

        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("\nExitting...");
            Initialization.CloseHandle(Initialization._handle);
            Environment.Exit(0);
        };
    }
}
