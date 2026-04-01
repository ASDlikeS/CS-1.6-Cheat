using System.Numerics;
using System.Runtime.InteropServices;
using CS16Cheat.core;
using CS16Cheat.utils;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;

namespace CS16Cheat.Overlay;

public class Renderer : IDisposable
{
    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern int GetWindowLong(nint hWnd, int nIndex);

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern bool SetLayeredWindowAttributes(
        nint hWnd,
        uint crKey,
        byte bAlpha,
        uint dwFlags
    );

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern bool SetWindowPos(
        nint hWnd,
        nint hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags
    );

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern bool ShowWindow(nint hWnd, int nIndex);

    private readonly nint HWND_TOPMOST = new(-1);
    private const int GWL_EXSTYLE = -20; // Sets a new extended window style.
    private const int WS_EX_LAYERED = 0x80000; // FLAG: window supports alpha-channel(opacity)
    private const int WS_EX_TOPMOST = 0x8;
    private const uint LWA_COLORKEY = 0x1; // MODE: opacity by color
    private const uint SWP_NOMOVE = 0x2;
    private const uint SWP_NOSIZE = 0x1;

#pragma warning disable CA2213
    private readonly IWindow _window;
#pragma warning restore CA2213
    private IInputContext _input = null!;
    private GL _gl = null!;
    private ImGuiController _imGuiController = null!;
    private bool _disposed;

    internal static bool HasError { get; set; }
    internal static bool HasWarn { get; set; }
    internal static string? LastErrorMessage { get; set; }
    internal static string? LastWarnMessage { get; set; }
    private static bool _isOverlayOpened = true;

    public Renderer()
    {
        var options = WindowOptions.Default;

        options.Size = new Vector2D<int>(1920, 1080);
        options.WindowBorder = WindowBorder.Hidden;
        options.TransparentFramebuffer = false;
        options.VSync = false;
        options.FramesPerSecond = 0;

        _window = Window.Create(options);

        _window.Load += OnLoad;
        _window.Render += OnRender;
        _window.Closing += OnClosing;
    }

    private static void DrawUI()
    {
        ImGui.Begin($"CS 1.6 ({Info.CurrentVersion}) 2026 Cheat");

        if (HasError)
        {
            ImGui.TextColored(new Vector4(1, 0, 0, 1), LastErrorMessage);
        }
        else
        {
            ImGui.TextColored(new Vector4(0, 1, 0, 1), "Execute correct.");
        }
        if (HasWarn)
        {
            ImGui.TextColored(new Vector4(1, 1, 0, 1), LastWarnMessage);
        }

        ImGui.Checkbox("Aimbot", ref Aimbot.IsEnabled);
        if (Aimbot.IsEnabled)
        {
            if (Aimbot.IsAimingOn)
            {
                ImGui.TextColored(new Vector4(0, 1, 0, 1), "Aiming: active");
            }
            else
            {
                ImGui.TextColored(new Vector4(1, 0, 0, 1), "Aiming: idle");
            }
        }

        ImGui.End();
    }

    private void MakeOverlay()
    {
        if (_window.Native != null && _window.Native.Win32.HasValue)
        {
            nint hWnd = _window.Native.Win32.Value.Hwnd;
            ShowWindow(hWnd, 0x0);

            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);

            int newStyle = exStyle | WS_EX_LAYERED | WS_EX_TOPMOST;
            _ = SetWindowLong(hWnd, GWL_EXSTYLE, newStyle);

            SetLayeredWindowAttributes(hWnd, 0x000000, 255, LWA_COLORKEY);
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            ShowWindow(hWnd, 0x5);
        }
    }

    private void OnLoad()
    {
        _gl = _window.CreateOpenGL();
        _input = _window.CreateInput();
        _imGuiController = new ImGuiController(_gl, _window, _input);
        MouseController.Initialize();
        MakeOverlay();
    }

    private void OnRender(double deltaTime)
    {
        _imGuiController.Update((float)deltaTime);

        if (_input.Keyboards[0].IsKeyPressed(Key.Home))
        {
            _isOverlayOpened = !_isOverlayOpened;
            Thread.Sleep(100);
        }

        _gl.ClearColor(0f, 0f, 0f, 0f);
        _gl.Clear(ClearBufferMask.ColorBufferBit);

        if (_isOverlayOpened)
            DrawUI();
        _imGuiController.Render();
    }

    private void OnClosing()
    {
        Dispose(true);
    }

    public void Run() => _window.Run();

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                MouseController.ShutDown();
                _imGuiController?.Dispose();
                _gl?.Dispose();
                _input?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
