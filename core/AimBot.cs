using CS16Cheat.LWOperations;
using CS16Cheat.Overlay;

namespace CS16Cheat.core;

static class Aimbot
{
    internal static bool IsEnabled;
    internal static bool IsAimingOn { get; set; }

    internal static void Run()
    {
        if (!IsAimingOn)
            return;

        int closestEntIndex = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < GameData.Entities.Length; i++)
        {
            ref var entity = ref GameData.Entities[i];

            if (entity.Team == 0)
                continue;
            if (entity.Team == GameData.LocalPlayer.Team)
                continue;
            if (entity.Health <= 1)
                continue;

            if (entity.Distance < minDistance)
            {
                minDistance = entity.Distance;
                closestEntIndex = i;
            }
        }

        if (closestEntIndex != -1)
        {
            ref var target = ref GameData.Entities[closestEntIndex];
            var newAngle = MathCalc.CalculateAngles(GameData.LocalPlayer.Position, target.Position);

            if (float.IsNaN(newAngle.X) || float.IsNaN(newAngle.Y))
            {
                Renderer.HasError = true;
                Renderer.LastErrorMessage = "Aimbot: Invalid angles calculated";
                return;
            }

            nint viewAnglesAddress = ModuleManager.GetBaseAddress(Modules.hw) + Offsets.viewAngles;

            if (!Memory.WriteVec2(viewAnglesAddress, newAngle))
            {
                Renderer.HasError = true;
                Renderer.LastErrorMessage =
                    "Couldn't handle with writing memory in process for aimbot processing...";
            }
        }
    }
}
