using System.Numerics;
using System.Runtime.InteropServices;
using CS16Cheat.core;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;

namespace CS16Cheat.Graphic;

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

    private const int GWL_EXSTYLE = -20; // Sets a new extended window style.
    private const int WS_EX_LAYERED = 0x80000; // FLAG: window supports alpha-channel(opacity)
    private const int WS_EX_TRANSPARENT = 0x20; // FLAG: input work throw window
    private const int WS_EX_TOPMOST = 0x8; // FLAG: window opens on the topmost place
    private const int WS_EX_TOOLWINDOW = 0x80;
    private const uint LWA_COLORKEY = 0x1; // MODE: opacity by color
    private static readonly nint HWND_TOPMOST = new(-1);
    private const uint SWP_NOMOVE = 0x2;
    private const uint SWP_NOSIZE = 0x1;
    private const int SW_HIDE = 0x0;
    private const int SW_SHOW = 0x5;
#pragma warning disable CA2213
    private readonly IWindow _window;
#pragma warning restore  CA2213
    private IInputContext _input = null!;
    private GL _gl = null!;
    private ImGuiController _imGuiController = null!;
    private bool _disposed;

    internal static bool _hasError;
    internal static bool _hasWarn;
    internal static string? LastErrorMessage { get; set; }
    internal static string? LastWarnMessage { get; set; }

    internal static IMouse Mouse { get; private set; }

    public Renderer()
    {
        var options = WindowOptions.Default;

        options.Size = new Vector2D<int>(1920, 1080);
        options.WindowBorder = WindowBorder.Hidden;
        options.TransparentFramebuffer = true;
        options.VSync = false;
        options.FramesPerSecond = 0;

        _window = Window.Create(options);

        _window.Load += OnLoad;
        _window.Render += OnRender;
        _window.Closing += OnClosing;
    }

    private static void DrawUI()
    {
        ImGui.Begin("Entity List");

        if (ImGui.BeginTable("EntitiesTable", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
        {
            // Заголовки
            ImGui.TableSetupColumn("Team", ImGuiTableColumnFlags.WidthFixed, 60);
            ImGui.TableSetupColumn("Health", ImGuiTableColumnFlags.WidthFixed, 80);
            ImGui.TableSetupColumn("Position X", ImGuiTableColumnFlags.WidthFixed, 100);
            ImGui.TableSetupColumn("Position Y", ImGuiTableColumnFlags.WidthFixed, 100);
            ImGui.TableHeadersRow();

            for (int i = 0; i < GameData.Entities.Length; i++)
            {
                var entity = GameData.Entities[i];
                if (entity.Team == 0)
                    break;

                ImGui.TableNextRow();

                // Team
                ImGui.TableSetColumnIndex(0);
                ImGui.Text(entity.Team.ToString());

                // Health (с цветом)
                ImGui.TableSetColumnIndex(1);
                Vector4 healthColor =
                    entity.Health < 30 ? new Vector4(1, 0, 0, 1) : new Vector4(0, 1, 0, 1);
                ImGui.TextColored(healthColor, entity.Health.ToString());

                // Position X
                ImGui.TableSetColumnIndex(2);
                ImGui.Text(entity.Position.X.ToString());

                // Position Y
                ImGui.TableSetColumnIndex(3);
                ImGui.Text(entity.Position.Y.ToString());
            }

            ImGui.EndTable();
        }

        ImGui.End();
    }

    private void MakeOverlay()
    {
        if (_window.Native != null && _window.Native.Win32.HasValue)
        {
            nint hWnd = _window.Native.Win32.Value.Hwnd;
            ShowWindow(hWnd, SW_HIDE);
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            var err = SetWindowLong(
                hWnd,
                GWL_EXSTYLE,
                exStyle | WS_EX_TOPMOST | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW | WS_EX_LAYERED
            );
            SetLayeredWindowAttributes(hWnd, 0x000000, 255, LWA_COLORKEY);
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            ShowWindow(hWnd, SW_SHOW);
        }
    }

    private void OnLoad()
    {
        _gl = _window.CreateOpenGL();
        _input = _window.CreateInput();
        _imGuiController = new ImGuiController(_gl, _window, _input);
        MakeOverlay();
        if (_input.Mice.Count > 0)
        {
            Mouse = _input.Mice[0];
            // Mouse.MouseDown += OnMouse TODO MOUSE HANDLELING
        }
    }

    private void OnRender(double deltaTime)
    {
        _imGuiController.Update((float)deltaTime);

        _gl.ClearColor(0f, 0f, 0f, 0f);
        _gl.Clear(ClearBufferMask.ColorBufferBit);
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
