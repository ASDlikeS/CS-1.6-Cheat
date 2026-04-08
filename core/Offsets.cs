using CS16Cheat.LWOperations;
using CS16Cheat.utils;

namespace CS16Cheat.core;

public static class Offsets
{
    // Module: hw
    public const int viewAngles = 0x108AEC4;
    public const int viewMatrix = 0xEC9780;
    public static readonly Dictionary<ClientVersion, EntityListEntry> EntityList = new()
    {
        { ClientVersion.V8684, new EntityListEntry(0x7F6304, Modules.hw) },
    };

    // Entity
    public const int position = 0x8;
    public const int health = 0x160;
    public const int team = 0x1C8;
    public const int objectData = 0x4;

    // List
    public const int localPlayer = 0x7C;
    public const int step = 0x324;
}
