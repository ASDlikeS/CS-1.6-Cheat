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

    private static void DrawUI()
    {
        var displaySize = ImGui.GetIO().DisplaySize;

        ImGui.SetNextWindowPos(new Vector2(10, 10), ImGuiCond.Always, new Vector2(0, 0));

        ImGui.Begin(
            $"CS 1.6 ({Info.CurrentVersion}) 2026 Cheat",
            ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize
        );

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

    private void OnLoad()
    {
        _gl = _window.CreateOpenGL();
        _input = _window.CreateInput();
        _imGuiController = new ImGuiController(_gl, _window, _input);
        WindowFollowing.OnLoading(_window);
        InputController.Initialize();
    }

    private void OnUpdating(double deltaTime)
    {
        WindowFollowing.FollowUpGameWnd(_window);
        WindowFollowing.UpdateTransparentWindow();
    }

    private void OnRender(double deltaTime)
    {
        _imGuiController.Update((float)deltaTime);
        _gl.ClearColor(0f, 0f, 0f, 0f);
        _gl.Clear(ClearBufferMask.ColorBufferBit);
        DrawUI();
        _imGuiController.Render();
    }

    private void OnFramebufferResize(Vector2D<int> size)
    {
        _gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
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
