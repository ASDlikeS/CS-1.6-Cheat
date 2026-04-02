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
            Config.Initialize();
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
            Utils.WriteErrorMessage($"HANDLED ERROR: {ex.Message}");
            Console.WriteLine(new string('=', Console.WindowWidth));
            Utils.WriteErrorMessage($"STACK TRACE: {ex.StackTrace}");
            if (ProcessManager.Handle != IntPtr.Zero)
                ProcessManager.CloseHandle(ProcessManager.Handle);
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return 1;
        }
    }
}
