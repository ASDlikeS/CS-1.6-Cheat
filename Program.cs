using CS16Cheat.core;
using CS16Cheat.HUD;
using CS16Cheat.LWOperations;
using CS16Cheat.utils;

namespace CS16Cheat;

static class Program
{
    private static volatile bool _isRunning = true;

    internal static async Task<int> Main()
    {
        try
        {
            ProcessManager.Initialize();
            ModuleManager.Initialize();
            Info.ShowStartHandle();
            Utils.SetupCancelHandler();

            var renderer = new Renderer();
            var renderThread = new Thread(new ThreadStart(renderer.Start().Wait));

            var localPlayer = new Entity();
            List<Entity> entities = [];

            while (_isRunning)
            {
                nint entityList = Memory.ReadPointer(
                    ModuleManager.GetBaseAddress(Modules.hw) + Offsets.entityList
                );
                localPlayer.Address = Memory.ReadPointer(entityList + Offsets.initialEntity);
                localPlayer.Team = Memory.ReadInt32(localPlayer.Address + Offsets.team);
                nint localData = Memory.ReadPointer(localPlayer.Address + Offsets.localData);
                localPlayer.Position = Memory.ReadVec3(localData + Offsets.position);

                Console.WriteLine(
                    $"Addr: {localPlayer.Address:X}, Position: ({localPlayer.Position.X} | {localPlayer.Position.Y} | {localPlayer.Position.Z}), Team: {(localPlayer.Team == 2 ? "SWAT" : "Terrorists")}"
                );

                Thread.Sleep(1000);
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: {ex.Message}");
            Console.WriteLine($"STACK: {ex.StackTrace}");
            Console.ResetColor();
            if (ProcessManager.Handle != IntPtr.Zero)
                ProcessManager.CloseHandle(ProcessManager.Handle);
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return 1;
        }
    }
}
