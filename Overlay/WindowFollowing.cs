using System.Runtime.InteropServices;
using CS16Cheat.LWOperations;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using static CS16Cheat.Overlay.DLLImports;

namespace CS16Cheat.Overlay;

public static class WindowFollowing
{
    static RECT _rect;
    static RECT _cachedRect;
    static bool _isTransparentMode;

    public static nint WindowHnd { get; set; }
    public static int WindowHeight { get; private set; }
    public static int WindowWidth { get; private set; }

    public static void FollowUpGameWnd(IWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        bool minimized = IsIconic(ProcessManager.ProcessHwnd);
        nint foreground = GetForegroundWindow();
        bool gameActive =
            !minimized && (foreground == ProcessManager.ProcessHwnd || foreground == WindowHnd);

        bool shouldShow = gameActive && InputController.IsOverlayOpen;

        if (window.IsVisible != shouldShow)
            window.IsVisible = shouldShow;

        if (!shouldShow)
            return;

        if (!GetWindowRect(ProcessManager.ProcessHwnd, out _rect))
            return;

        WindowWidth = _rect.Right - _rect.Left;
        WindowHeight = _rect.Bottom - _rect.Top;
        if (WindowWidth <= 0 || WindowHeight <= 0)
            return;

        if (_cachedRect != _rect)
        {
            window.Position = new Vector2D<int>(_rect.Left, _rect.Top);
            window.Size = new Vector2D<int>(WindowWidth, WindowHeight);
            _cachedRect = _rect;
        }
    }

    public static void UpdateTransparentWindow()
    {
        bool shouldBeTransparent = !ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow);

        if (shouldBeTransparent == _isTransparentMode)
            return;

        int exStyle = GetWindowLong(WindowHnd, GWL_EXSTYLE);

        if (shouldBeTransparent)
        {
            exStyle |= WS_EX_LAYERED | WS_EX_TRANSPARENT;
        }
        else
        {
            exStyle &= ~WS_EX_LAYERED & ~WS_EX_TRANSPARENT;
        }

        _ = SetWindowLong(WindowHnd, GWL_EXSTYLE, exStyle);
        _isTransparentMode = shouldBeTransparent;
    }

    public static void OnLoading(IWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(window.Native);
        ArgumentNullException.ThrowIfNull(window.Native.Win32);

        WindowHnd = window.Native.Win32.Value.Hwnd;

        int exStyle = GetWindowLong(WindowHnd, GWL_EXSTYLE);
        exStyle |= WS_EX_LAYERED | WS_EX_TRANSPARENT;
        _ = SetWindowLong(WindowHnd, GWL_EXSTYLE, exStyle);

        _isTransparentMode = true;

        window.IsVisible = false;
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

    public override readonly bool Equals(object? obj) => obj is RECT other && Equals(other);

    public readonly bool Equals(RECT other) =>
        Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;

    public override readonly int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);

    public static bool operator ==(RECT left, RECT right) => left.Equals(right);

    public static bool operator !=(RECT left, RECT right) => !(left == right);
}
