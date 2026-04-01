using CS16Cheat.LWOperations;

namespace CS16Cheat.core;

static class GameLoop
{
    private static bool _running = true;
    private static Thread _gameThread = new(Run) { IsBackground = true, Name = "GameLoopThread" };

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
                if (Aimbot.IsEnabled)
                {
                    Aimbot.Run();
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"GameLoop error: {ex.Message}");
                Environment.Exit(1);
            }
        }
    }
}
