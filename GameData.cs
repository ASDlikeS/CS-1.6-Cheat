using CS16Cheat.core;

namespace CS16Cheat;

public record GameField
{
    public string Name { get; init; } = null!;
    public Modules Module { get; init; }
    public int[] OffsetChain { get; init; } = null!;
    public Memory.ReadType DataType { get; init; }
}

public static class GameData
{
    public static readonly GameField Health = new()
    {
        Name = "health",
        Module = Modules.hw,
        OffsetChain = [0x6E92AC, 0x7C, 0x4, 0x160],
        DataType = Memory.ReadType.Float,
    };
    public static readonly GameField CurrentSlotAmmo = new()
    {
        Name = "currentSlotAmmo",
        Module = Modules.hw,
        OffsetChain = [0x6E92AC, 0x7C, 0x5D4, 0xCC],
        DataType = Memory.ReadType.Int32,
    };
    public static readonly GameField Money = new()
    {
        Name = "money",
        Module = Modules.client,
        OffsetChain = [0x1213C4],
        DataType = Memory.ReadType.Int32,
    };

    public static readonly GameField[] AllFields = [Health, CurrentSlotAmmo, Money];
}
