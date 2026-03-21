namespace CS16Cheat;

internal static class Utils
{
    internal static void CancelCB(nint handle)
    {
        Console.WriteLine("[*] Initialization cancelling callback function...");

        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("\nExitting...");
            Program.CloseHandle(handle);
            Environment.Exit(0);
        };
    }
}
