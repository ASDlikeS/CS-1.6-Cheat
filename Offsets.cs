using CS16Cheat.core;

namespace CS16Cheat;

public static class Offsets
{
    public static readonly int[] Health = [0x6E92AC, 0x7C, 0x4, 0x160];
    public static readonly int[] CurrentSlotAmmo = [0x6E92AC, 0x7C, 0x5D4, 0xCC];
    public static readonly int[] Money = [0x1213C4];

    internal static Modules GetModuleFor(string valueName)
    {
        return valueName switch
        {
            "health" => Modules.hw,
            "currentSlotAmmo" => Modules.hw,
            "money" => Modules.client,
            _ => throw new NotImplementedException(),
        };
    }
}
