using CS16Cheat.core;
using CS16Cheat.LWOperations;
using static CS16Cheat.Overlay.DLLImports;

namespace CS16Cheat.Overlay;

static class InputController
{
    const int VK_XBUTTON1 = 0x05;
    const int VK_HOME = 0x24;
    static bool _isOverlayOpened;

    static bool IsXButton1Pressed => (GetAsyncKeyState(VK_XBUTTON1) & 0x8000) != 0;
    static bool IsHomePressed => (GetAsyncKeyState(VK_HOME) & 0x8000) != 0;

    static Thread? _pollThread;
    static volatile bool _running;

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

            if (
                IsIconic(ProcessManager.ProcessHwnd)
                || GetForegroundWindow() != ProcessManager.ProcessHwnd
            )
            {
                Thread.Sleep(1);
                continue;
            }

            if (IsHomePressed)
            {
                _isOverlayOpened = !_isOverlayOpened;

                if (_isOverlayOpened)
                    ShowWindow(WindowFollowing.WindowHnd, SW_SHOW);
                else
                    ShowWindow(WindowFollowing.WindowHnd, SW_HIDE);

                Thread.Sleep(100);
            }
            Thread.Sleep(1);
        }
    }

    public static void ShutDown()
    {
        _running = false;
        _pollThread?.Join(500);
    }
}
