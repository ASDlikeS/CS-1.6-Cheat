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
    static RECT _chachedRect;
    static int _cachedExStyle; // ← Добавить кеш стиля
    static bool _isTransparentMode;
    public static nint WindowHnd { get; set; }

    public static void FollowUpGameWnd(IWindow window)
    {
        var windowS = GetWindowRect(ProcessManager.ProcessHwnd, out _rect);

        if (_chachedRect != _rect)
        {
            int width = _rect.Right - _rect.Left;
            int height = _rect.Bottom - _rect.Top;
            ArgumentNullException.ThrowIfNull(window);
            window.Position = new Vector2D<int>(_rect.Left, _rect.Top);
            window.Size = new Vector2D<int>(width, height);
            _chachedRect = _rect;
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
            Console.WriteLine("SET TRANSPARENT!");
        }
        else
        {
            Console.WriteLine("SET CLICKABLE!");
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
        exStyle |= WS_EX_LAYERED | WS_EX_TRANSPARENT | 0x80;
        _ = SetWindowLong(WindowHnd, GWL_EXSTYLE, exStyle);
        _isTransparentMode = true;
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
