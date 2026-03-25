using System.Runtime.InteropServices;
using CS16Cheat.core;
using static CS16Cheat.utils.Info;

namespace CS16Cheat;

static class Program
{
    internal static string openMode = "classic";
    private static bool infiniteAmmo = false;
    private static bool infiniteHP = false;
    private static bool setMoneyMax = false;

    internal static int Main(string[] args)
    {
        try
        {
            ProcessManager.Initialize();
            ModuleManager.Initialize();

            ShowStartHandle();
            ShowKeybinds();

            var startLine = Console.CursorTop;

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Escape)
                        break;
                    if (key == ConsoleKey.F1)
                    {
                        infiniteAmmo = !infiniteAmmo;
                        Console.WriteLine($"Infinite ammo: {(infiniteAmmo ? "ON" : "OFF")}");
                        Thread.Sleep(1000);
                    }
                    if (key == ConsoleKey.F2)
                    {
                        infiniteHP = !infiniteHP;
                        Console.WriteLine($"Infinite HP: {(infiniteHP ? "ON" : "OFF")}");
                        Thread.Sleep(1000);
                    }
                    if (key == ConsoleKey.F3)
                    {
                        setMoneyMax = !setMoneyMax;
                        Console.WriteLine("[+] Money set on 16000$");
                        Thread.Sleep(1000);
                    }
                }

                GameData.RefreshDynamicAddress(
                    GameData.CurrentSlotAmmo,
                    openMode == "classic" ? Modules.hw : Modules.engine,
                    openMode == "classic" ? GameData.ammoOff : GameData.CUSTOM_ammoOff
                );

                Dictionary<string, object?> values = [];
                byte countErrors = 0;

                foreach (var data in GameData.AllFields)
                {
                    values[data.Name] = data.DataType switch
                    {
                        MemoryType.Single => Memory.Read<float>(data.DynamicAddress),
                        MemoryType.Int16 => Memory.Read<short>(data.DynamicAddress),
                        MemoryType.Int32 => Memory.Read<int>(data.DynamicAddress),
                        MemoryType.Int64 => Memory.Read<long>(data.DynamicAddress),
                        MemoryType.Double => Memory.Read<double>(data.DynamicAddress),
                        _ => throw new NotImplementedException(),
                    };
                    if (values[data.Name] == null)
                        countErrors++;
                }

                if (countErrors == GameData.AllFields.Length)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[X] There's no values in addresses found!".PadRight(20));
                    Console.ResetColor();
                }
                else if (countErrors > 1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(
                        $"[!] There's {values.Count} values found from {GameData.AllFields.Length} exist values..."
                    );
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[+] Utility works correct!".PadRight(20));
                    Console.ResetColor();
                }

                foreach (var n in values)
                {
                    Console.WriteLine($"{n.Key}: {n.Value ?? null}");
                }

                if (infiniteAmmo)
                {
                    if (!Memory.Write<int>(GameData.CurrentSlotAmmo.DynamicAddress, 30))
                    {
                        infiniteAmmo = false;
                    }
                }
                if (infiniteHP)
                {
                    if (!Memory.Write<float>(GameData.Health.DynamicAddress, 100))
                    {
                        infiniteHP = false;
                    }
                }
                if (setMoneyMax)
                {
                    Memory.Write<int>(GameData.Money.DynamicAddress, 16000);
                    setMoneyMax = false;
                }

                int endLine = Console.CursorTop;
                Thread.Sleep(500);

                for (; endLine > startLine - 1; endLine--)
                {
                    Console.SetCursorPosition(0, endLine);
                    Console.Write(new string(' ', Console.WindowWidth));
                }
            }

            if (!ProcessManager.CloseHandleP())
            {
                Console.WriteLine(
                    $"[X] Error closing process handle | ERROR_CODE: {Marshal.GetLastWin32Error()}"
                );
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
            ProcessManager.CloseHandleP();
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return 1;
        }
    }
}
