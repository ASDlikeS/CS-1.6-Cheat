using System.Runtime.InteropServices;
using CS16Cheat.LWOperations;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using static CS16Cheat.Overlay.DLLImports;

namespace CS16Cheat.Overlay;

public static class WindowFollowing
{
    static RECT _rect;
    static RECT _chacheRect;
    static bool _chacheIsVisible;
    public static nint WindowHnd { get; set; }

    public static void FollowUpGameWnd(IWindow window)
    {
        if (
            IsIconic(ProcessManager.ProcessHwnd)
            || GetForegroundWindow() != ProcessManager.ProcessHwnd
        )
        {
            if (_chacheIsVisible)
            {
                ShowWindow(WindowHnd, SW_HIDE);
                _chacheIsVisible = false;
                return;
            }
        }

        if (!GetWindowRect(ProcessManager.ProcessHwnd, out _rect))
        {
            Console.WriteLine("cant get window rectangle");
            return;
        }

        int width = _rect.Right - _rect.Left;
        int height = _rect.Bottom - _rect.Top;

        if (width <= 0 || height <= 0)
        {
            return;
        }

        if (_chacheRect != _rect)
        {
            // SetWindowPos(WindowHnd, HWND_TOPMOST, _rect.Left, _rect.Top, width, height, 0x2000);
            if (window == null)
                return;
            window.Position = new Vector2D<int>(width, height);
            _chacheRect = _rect;
        }
        _chacheIsVisible = true;
    }

    public static void OnLoading(IWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(window.Native);
        ArgumentNullException.ThrowIfNull(window.Native.Win32);
        WindowHnd = window.Native.Win32.Value.Hwnd;
        int exStyle = GetWindowLong(WindowHnd, GWL_EXSTYLE);
        _ = SetWindowLong(WindowHnd, GWL_EXSTYLE, exStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);
        ShowWindow(WindowHnd, SW_HIDE);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct RECT(int left, int top, int right, int bottom) : IEquatable<RECT>
{
    public int Left = left;
    public int Top = top;
    public int Right = right;
    public int Bottom = bottom;

    public readonly int Width => Right - Left;
    public readonly int Height => Bottom - Top;

    public override readonly bool Equals(object? obj)
    {
        return obj is RECT other && Equals(other);
    }

    public readonly bool Equals(RECT other)
    {
        return Left == other.Left
            && Top == other.Top
            && Right == other.Right
            && Bottom == other.Bottom;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Left, Top, Right, Bottom);
    }

    public static bool operator ==(RECT left, RECT right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RECT left, RECT right)
    {
        return !(left == right);
    }
}
