using CS16Cheat.LWOperations;
using CS16Cheat.utils;

namespace CS16Cheat.core;

static class GameLoop
{
    private static bool _running = true;
    private static readonly Thread _gameThread = new(Run)
    {
        IsBackground = true,
        Name = "GameLoopThread",
    };

    public static void Start()
    {
        _gameThread.Start();
    }

    public static void Stop()
    {
        Console.WriteLine("See ya later...");
        _running = false;
        _gameThread?.Join(1000);
        ProcessManager.CloseHandle(ProcessManager.Handle);
    }

    private static void Run()
    {
        while (_running)
        {
            try
            {
                GameData.UpdateGameData();
                if (Aimbot.isEnabled)
                {
                    Aimbot.Run();
                }
                Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                Utils.WriteErrorMessage($"[X] CORE ERROR: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }
    }
}
