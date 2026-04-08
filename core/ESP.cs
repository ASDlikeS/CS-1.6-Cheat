using System.Numerics;
using CS16Cheat.LWOperations;
using CS16Cheat.Overlay;
using ImGuiNET;

namespace CS16Cheat.core;

static class ESP
{
    const int _lengthViewMatrix = 16;
    internal static bool isEnabled;

    internal static void Draw()
    {
        if (!isEnabled)
            return;

        float[]? viewMatrix = Memory.ReadMatrix(
            ModuleManager.GetBaseAddress(Modules.hw) + Offsets.viewMatrix,
            _lengthViewMatrix
        );

        if (viewMatrix == null)
        {
            Renderer.SetError("Couldn't read viewMatrix memory...");
            return;
        }

        var drawList = ImGui.GetBackgroundDrawList();

        for (int i = 0; i < GameData.Entities.Length; i++)
        {
            ref var entity = ref GameData.Entities[i];

            if (entity.Address == nint.Zero)
                break;

            if (entity.Health <= 1)
                continue;

            if (MathCalc.W2S(viewMatrix, entity.Position, out Vector2 screenPos))
            {
                DrawBox(drawList, screenPos, entity);
            }
        }
    }

    private static void DrawBox(ImDrawListPtr drawList, Vector2 screenPos, Entity entity)
    {
        const float BASE_DISTANCE = 500f;
        const float BASE_WIDTH = 40f;
        const float BASE_HEIGHT = 80f;

        float scale = BASE_DISTANCE / entity.Distance;
        float boxWidth = BASE_WIDTH * scale;
        float boxHeight = BASE_HEIGHT * scale;

        float x = screenPos.X - boxWidth / 2;
        float y = screenPos.Y - boxHeight / 3;

        Vector4 color =
            (entity.Team != GameData.LocalPlayer.Team)
                ? new Vector4(1, 0, 0, 1)
                : new Vector4(0, 1, 0, 1);

        uint uintColor = ImGui.ColorConvertFloat4ToU32(color);

        drawList.AddRect(new Vector2(x, y), new Vector2(x + boxWidth, y + boxHeight), uintColor, 0);

        float healthPercent = Math.Clamp(entity.Health / 100.0f, 0, 1);
        float healthHeight = boxHeight * healthPercent;

        drawList.AddRectFilled(
            new Vector2(x - 6, y + boxHeight - healthHeight),
            new Vector2(x - 2, y + boxHeight),
            ImGui.ColorConvertFloat4ToU32(new Vector4(0, 1, 0, 1))
        );
    }
}
