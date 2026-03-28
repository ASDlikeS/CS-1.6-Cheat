using System.Numerics;
using ClickableTransparentOverlay;
using ImGuiNET;

namespace CS16Cheat.HUD;

public class Renderer : Overlay
{
    public bool enableAim = false;
    public bool enableFocusOnTarget = false;

    protected override void Render()
    {
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
        ImGui.End();
    }
}
