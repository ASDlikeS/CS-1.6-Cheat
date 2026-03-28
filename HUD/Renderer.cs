using System.Numerics;
using ClickableTransparentOverlay;
using ImGuiNET;

namespace CS16Cheat.HUD;

public class Renderer : Overlay
{
    private bool wantKeepDemoWindow = true;
    public bool enableAim = false;
    public bool enableFocusOnTarget = false;

    public Renderer()
        : base(1920, 1080) { }

    protected override void Render()
    {
        ImGui.ShowDemoWindow(ref wantKeepDemoWindow);
        ImGui.Begin("Awesome skill by ASD");
        ImGui.Checkbox("Aimbot", ref enableAim);

        if (enableAim && enableFocusOnTarget)
        {
            ImGui.TextColored(new Vector4(0, 1, 0, 1), "Aimbot: aiming");
        }
        else if (enableAim)
        {
            ImGui.TextColored(new Vector4(1, 0, 0, 1), "Aimbot: disabled");
        }

        if (!this.wantKeepDemoWindow)
        {
            this.Close();
        }
        ImGui.End();
    }
}
