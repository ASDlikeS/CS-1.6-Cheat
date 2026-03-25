using CS16Cheat.core;

namespace CS16Cheat;

public record GameField
{
    public string Name { get; init; } = null!;
    public MemoryType DataType { get; init; }
    public nint DynamicAddress { get; internal set; }
}

public static class GameData
{
    internal static readonly int[] healthOff = [0x6E92AC, 0x7C, 0x4, 0x160];
    internal static readonly int[] ammoOff = [0x6E92AC, 0x7C, 0x5D4, 0xCC];
    internal static readonly int[] moneyOff = [0x1213C4];

    internal static readonly int[] CUSTOM_healthOff = [0x658480, 0x1E0];
    internal static readonly int[] CUSTOM_ammoOff = [0x658480, 0x7C, 0x5D4, 0xCC];
    internal static readonly int[] CUSTOM_moneyOff = [0x0120427C, 0xE94];
    public static readonly GameField Health = new()
    {
        Name = "health",
        DataType = MemoryType.Single,
        DynamicAddress = PointerResolver.FollowPointerChain(
            ModuleManager.GetBaseAddress(
                Program.openMode == "classic" ? Modules.hw : Modules.engine
            ),
            Program.openMode == "classic" ? healthOff : CUSTOM_healthOff
        ),
    };
    public static readonly GameField CurrentSlotAmmo = new()
    {
        Name = "currentSlotAmmo",
        DataType = MemoryType.Int32,
        DynamicAddress = PointerResolver.FollowPointerChain(
            ModuleManager.GetBaseAddress(
                Program.openMode == "classic" ? Modules.hw : Modules.engine
            ),
            Program.openMode == "classic" ? ammoOff : CUSTOM_ammoOff
        ),
    };
    public static readonly GameField Money = new()
    {
        Name = "money",
        DataType = MemoryType.Int32,
        DynamicAddress = PointerResolver.FollowPointerChain(
            ModuleManager.GetBaseAddress(
                Program.openMode == "classic" ? Modules.client : Modules.engine
            ),
            Program.openMode == "classic" ? moneyOff : CUSTOM_moneyOff
        ),
    };

    public static readonly GameField[] AllFields = [Health, CurrentSlotAmmo, Money];

    internal static void RefreshDynamicAddress(GameField value, Modules module, int[] offsets)
    {
        value.DynamicAddress = PointerResolver.FollowPointerChain(
            ModuleManager.GetBaseAddress(module),
            offsets
        );
    }
}
