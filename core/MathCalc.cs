using System.Numerics;

namespace CS16Cheat.core;

static class MathCalc
{
    internal static Vector2 CalculateAngles(Vector3 from, Vector3 to)
    {
        float yaw;
        float pitch;

        float deltaX = to.X - from.X;
        float deltaY = to.Y - from.Y;
        float deltaZ = to.Z - from.Z;
        double distance = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));

        yaw = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI);
        pitch = -(float)(Math.Atan2(deltaZ, distance) * 180 / Math.PI);

        return new Vector2(yaw, pitch);
    }
}
