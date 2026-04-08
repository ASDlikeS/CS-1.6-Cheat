using System.Numerics;
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
#pragma warning disable CA2213
    readonly IWindow _window;
#pragma warning restore CA2213
    IInputContext _input = null!;
    GL _gl = null!;
    ImGuiController _imGuiController = null!;
    bool _disposed;
    static bool _hasError;
    static bool _hasWarn;
    static double _warnDuration;
    static double _errorDuration;
    const double _WarnAndErrDelay = 2.0;

    internal static string? _lastErrorMessage;
    internal static string? _lastWarnMessage;

    public Renderer()
    {
        var options = WindowOptions.Default;

        options.WindowBorder = WindowBorder.Hidden;
        options.TransparentFramebuffer = true;
        options.VSync = false;
        options.FramesPerSecond = 0;
        options.TopMost = true;

        _window = Window.Create(options);

        _window.Load += OnLoad;
        _window.Render += OnRender;
        _window.Closing += OnClosing;
        _window.Update += OnUpdating;
        _window.FramebufferResize += OnFramebufferResize;
    }

    static void DrawUI()
    {
        ImGui.SetNextWindowPos(new Vector2(10, 10), ImGuiCond.Always, new Vector2(0, 0));

        ImGui.Begin($"CS 1.6 ({Info.CurrentVersion}) 2026 Cheat", ImGuiWindowFlags.NoMove);

        if (_hasError)
        {
            ImGui.TextColored(new Vector4(1, 0, 0, 1), $"ERROR: {_lastErrorMessage}");
        }
        else
        {
            ImGui.TextColored(new Vector4(0, 1, 0, 1), "Execute correct.");
        }
        if (_hasWarn)
        {
            ImGui.TextColored(new Vector4(1, 1, 0, 1), $"WARNING: {_lastWarnMessage}");
        }

        ImGui.Checkbox("Aimbot", ref Aimbot.isEnabled);
        if (Aimbot.isEnabled)
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

        ImGui.Checkbox("ESP", ref ESP.isEnabled);

        ImGui.End();
    }

    void OnLoad()
    {
        _gl = _window.CreateOpenGL();
        _input = _window.CreateInput();
        _imGuiController = new ImGuiController(_gl, _window, _input);
        WindowFollowing.OnLoading(_window);
        InputController.Initialize();
    }

    void OnUpdating(double deltaTime)
    {
        WindowFollowing.FollowUpGameWnd(_window);
        WindowFollowing.UpdateTransparentWindow();
        if (_hasError)
        {
            _errorDuration += deltaTime;

            if (_errorDuration > _WarnAndErrDelay)
            {
                _hasError = false;
                _errorDuration = 0;
            }
        }
        if (_hasWarn)
        {
            _warnDuration += deltaTime;

            if (_warnDuration > _WarnAndErrDelay)
            {
                _hasWarn = false;
                _warnDuration = 0;
            }
        }
    }

    void OnRender(double deltaTime)
    {
        _imGuiController.Update((float)deltaTime);
        _gl.ClearColor(0f, 0f, 0f, 0f);
        _gl.Clear(ClearBufferMask.ColorBufferBit);
        ESP.Draw();
        DrawUI();
        _imGuiController.Render();
    }

    public static void SetError(string errorMessage)
    {
        _hasError = true;
        _lastErrorMessage = errorMessage;
    }

    public static void SetWarning(string warningMessage)
    {
        _hasWarn = true;
        _lastWarnMessage = warningMessage;
    }

    void OnFramebufferResize(Vector2D<int> size)
    {
        _gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
    }

    void OnClosing()
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
                InputController.ShutDown();
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
