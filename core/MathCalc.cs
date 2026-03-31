using System.Numerics;

namespace CS16Cheat.core;

static class MathCalc
{
    internal static Vector2 CalculateAngles(Vector3 from, Vector3 to)
    {
        Vector3 delta = to - from;

        float horizontalDistance = MathF.Sqrt(delta.X * delta.X + delta.Y * delta.Y);

        float yaw = MathF.Atan2(delta.Y, delta.X) * (180f / MathF.PI);
        float pitch = -(MathF.Atan2(delta.Z, horizontalDistance) * (180f / MathF.PI));

        // CS 1.6 диапазон: yaw -180..180, pitch -89..89
        yaw = NormalizeAngle(yaw);
        pitch = Math.Clamp(pitch, -89f, 89f);

        return new Vector2(pitch, yaw);
    }

    private static float NormalizeAngle(float angle)
    {
        while (angle > 180f)
            angle -= 360f;
        while (angle < -180f)
            angle += 360f;
        return angle;
    }
}
