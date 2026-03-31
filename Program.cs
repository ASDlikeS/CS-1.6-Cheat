using CS16Cheat.core;
using CS16Cheat.LWOperations;
using CS16Cheat.Overlay;
using CS16Cheat.utils;

namespace CS16Cheat;

static class Program
{
    internal static async Task<int> Main()
    {
        try
        {
            ProcessManager.Initialize();
            Utils.SetupCancelHandler();
            ModuleManager.Initialize();
            Info.ShowStartHandle();

            GameLoop.Start();

            using var renderer = new Renderer();
            renderer.Run();

            GameLoop.Stop();
            return 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"HANDLED ERROR: {ex.Message}");
            Console.WriteLine(new string('=', Console.WindowWidth));
            Console.WriteLine($"STACK TRACE: {ex.StackTrace}");
            Console.ResetColor();
            if (ProcessManager.Handle != IntPtr.Zero)
                ProcessManager.CloseHandle(ProcessManager.Handle);
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return 1;
        }
    }
}
