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
            ProcessManager.CloseHandle(ProcessManager.Handle);
            Environment.Exit(0);
        };
    }
}
