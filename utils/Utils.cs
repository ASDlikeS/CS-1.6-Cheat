using CS16Cheat.LWOperations;

namespace CS16Cheat.utils;

internal static class Utils
{
    internal static int HOTKEY_AIM = 0x06;

    internal static void SetupCancelHandler()
    {
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            Console.WriteLine("\n[!] Ctrl+C pressed. Exiting...");

            if (ProcessManager.Handle != IntPtr.Zero)
            {
                ProcessManager.CloseHandle(ProcessManager.Handle);
                Console.WriteLine("[+] Process handle closed.");
            }
            Environment.Exit(0);
        };
    }

    internal static void GetChooseMessage()
    {
        var answer = Console.ReadLine()?.ToLower();
        if (answer != "y")
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
