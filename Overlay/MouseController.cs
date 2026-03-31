using System.Runtime.InteropServices;
using CS16Cheat.core;

namespace CS16Cheat.Overlay;

static class MouseController
{
    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern short GetAsyncKeyState(int vKey);

    private const int VK_XBUTTON1 = 0x05;

    public static bool IsXButton1Pressed => (GetAsyncKeyState(VK_XBUTTON1) & 0x8000) != 0;

    private static Thread? _pollThread;
    private static volatile bool _running;

    public static void Initialize()
    {
        _running = true;
        _pollThread = new Thread(PollLoop) { IsBackground = true, Name = "MousePollThread" };
        _pollThread.Start();
    }

    private static void PollLoop()
    {
        while (_running)
        {
            Aimbot.IsAimingOn = IsXButton1Pressed;
            Thread.Sleep(1);
        }
    }

    public static void ShutDown()
    {
        _running = false;
        _pollThread?.Join(500);
    }
}
