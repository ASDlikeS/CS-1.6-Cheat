using CS16Cheat.core;
using CS16Cheat.LWOperations;
using static CS16Cheat.Overlay.DLLImports;

namespace CS16Cheat.Overlay;

static class InputController
{
    const int VK_XBUTTON1 = 0x05;
    const int VK_HOME = 0x24;

    static bool IsXButton1Pressed => (GetAsyncKeyState(VK_XBUTTON1) & 0x8000) != 0;
    static bool IsHomePressed => (GetAsyncKeyState(VK_HOME) & 0x8000) != 0;

    static bool _homePrev;
    static Thread? _pollThread;
    static volatile bool _running;

    public static volatile bool IsOverlayOpen;

    public static void Initialize()
    {
        _running = true;
        _pollThread = new Thread(PollLoop) { IsBackground = true, Name = "InputPollThread" };
        _pollThread.Start();
    }

    static void PollLoop()
    {
        while (_running)
        {
            Aimbot.IsAimingOn = IsXButton1Pressed;

            bool homeCurrent = IsHomePressed;

            if (homeCurrent && !_homePrev)
                IsOverlayOpen = !IsOverlayOpen;

            _homePrev = homeCurrent;

            Thread.Sleep(1);
        }
    }

    public static void ShutDown()
    {
        _running = false;
        _pollThread?.Join(500);
    }
}
