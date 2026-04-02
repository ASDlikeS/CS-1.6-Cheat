using System.Runtime.InteropServices;

namespace CS16Cheat.Overlay;

public static class DLLImports
{
    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern int GetWindowLong(nint hWnd, int nIndex);

    public const int GWL_EXSTYLE = -20;

    //========================================================================================
    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    public const int TOP_MOST = 0x8;
    public const int WS_EX_LAYERED = 0x80000;
    public const int WS_EX_TRANSPARENT = 0x20;

    //========================================================================================
    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern short GetAsyncKeyState(int vKey);

    //========================================================================================
    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern nint GetForegroundWindow();

    //========================================================================================
    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern bool IsIconic(IntPtr hWnd);

    //========================================================================================
    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern bool SetWindowPos(
        nint hWnd,
        nint hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags
    );

    public static readonly nint HWND_TOPMOST = new(-1);
    public const uint SWP_NOACTIVATE = 0x10;
    public const uint SWP_SHOWWINDOW = 0x40;

    //========================================================================================
    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern bool ShowWindow(nint hWnd, int nIndex);

    public const int SW_HIDE = 0;
    public const int SW_SHOW = 5;

    //========================================================================================

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern bool GetWindowRect(nint hWnd, out RECT lpRect);
}
