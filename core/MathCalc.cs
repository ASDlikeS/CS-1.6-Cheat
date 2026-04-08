using System.Numerics;
using CS16Cheat.Overlay;

namespace CS16Cheat.core;

static class MathCalc
{
    internal static Vector2 CalculateAngles(Vector3 from, Vector3 to)
    {
        Vector3 delta = to - from;

        float horizontalDistance = MathF.Sqrt(delta.X * delta.X + delta.Y * delta.Y);

        float yaw = MathF.Atan2(delta.Y, delta.X) * (180f / MathF.PI);
        float pitch = -(MathF.Atan2(delta.Z, horizontalDistance) * (180f / MathF.PI));

        return new Vector2(pitch, yaw);
    }

    internal static bool W2S(float[] matrix, Vector3 worldPos, out Vector2 screenPos)
    {
        screenPos = new Vector2(0, 0);

        float clipX =
            worldPos.X * matrix[0] + worldPos.Y * matrix[4] + worldPos.Z * matrix[8] + matrix[12];
        float clipY =
            worldPos.X * matrix[1] + worldPos.Y * matrix[5] + worldPos.Z * matrix[9] + matrix[13];
        float clipZ =
            worldPos.X * matrix[2] + worldPos.Y * matrix[6] + worldPos.Z * matrix[10] + matrix[14];
        float clipW =
            worldPos.X * matrix[3] + worldPos.Y * matrix[7] + worldPos.Z * matrix[11] + matrix[15];

        if (clipW < 0.1f)
            return false;

        float ndcX = clipX / clipW;
        float ndcY = clipY / clipW;

        float screenWidth = WindowFollowing.WindowWidth;
        float screenHeight = WindowFollowing.WindowHeight;

        float screenX = (screenWidth / 2f * ndcX) + (ndcX + screenWidth / 2f);
        float screenY = -(screenHeight / 2f * ndcY) + (ndcY + screenHeight / 2f);

        screenPos = new Vector2(screenX, screenY);
        return true;
    }
}
