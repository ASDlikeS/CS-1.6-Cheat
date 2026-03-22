using CS16Cheat.core;
using static CS16Cheat.core.Initialization;
using static CS16Cheat.Utils;

namespace CS16Cheat;

class Program
{
    internal static int Main(string[] args)
    {
        InitProcess();
        CancelCB();
        bool success = InitModuleBaseAddress();
        if (!success)
        {
            CloseHandle(_handle);
            Console.WriteLine("\nPress any key to escape...");
            Console.ReadKey();
            return 1;
        }

        int[] healthOffsets = [0x6E92AC, 0x7C, 0x4, 0x160];
        int[] currentSlotAmmoOffsets = [0x6E92AC, 0x7C, 0x5D4, 0xCC];
        int[] moneyOffsets = [0x1213C4];

        var healthMd = Game.valuesAndModules["health"];
        var currentSlotAmmoMd = Game.valuesAndModules["currentSlotAmmo"];
        var moneyMd = Game.valuesAndModules["money"];

        nint healthAddr = Game.FollowPointerChain(Game.baseAddresses[healthMd], healthOffsets);
        nint currentSlotAmmoAddr = Game.FollowPointerChain(
            Game.baseAddresses[currentSlotAmmoMd],
            currentSlotAmmoOffsets
        );
        nint moneyAddr = Game.FollowPointerChain(Game.baseAddresses[moneyMd], moneyOffsets);

        var health = Memory.ReadWTypeMemory(healthAddr, Memory.ReadType.Float);
        var currentSlotAmmo = Memory.ReadWTypeMemory(currentSlotAmmoAddr, Memory.ReadType.Int32);
        var money = Memory.ReadWTypeMemory(moneyAddr, Memory.ReadType.Int32);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("[+] Game values found successfully!");
        Console.WriteLine(
            $"Current values:\nHealth: {health}\nCurrent weapon's ammo: {currentSlotAmmo}\nMoney: {money}"
        );
        Console.ResetColor();

        //---------------------------------------------------------
        CloseHandle(_handle);

        Console.WriteLine("\nPress eny key to escape...");
        Console.ReadKey();
        return 0;
    }
}
