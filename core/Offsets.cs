namespace CS16Cheat.core;

public static class Offsets
{
    // Module: hw.dll
    public static int viewAngles = 0x108AEC4;
    public static Dictionary<int, int> entityListByVersion = new()
    {
        { 8684, 0x7F6304 },
        { 10039, 0x7F5F84 },
    };

    // Entity
    public static int position = 0x8;
    public static float health = 0x160;
    public static int team = 0x1C8;
    public static int localData = 0x4;

    // List
    public static int initialEntity = 0x7C;
    public static int step = 0x324;
}
