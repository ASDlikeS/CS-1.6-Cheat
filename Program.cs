using System.Runtime.InteropServices;
using CS16Cheat.core;

namespace CS16Cheat;

class Program
{
    const string csProcessName = "hl";

    internal static int Main(string[] args)
    {
        ProcessManager.Initialize(csProcessName);
        ModuleManager.InitializeAll();

        Console.Write("".PadRight(50));
        var startLine = Console.CursorTop;

        while (true)
        {
            Console.SetCursorPosition(0, startLine);
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                break;

            var values = new Dictionary<string, object?>();

            foreach (var data in GameData.AllFields)
            {
                nint dataAddr = PointerResolver.FollowPointerChain(
                    ModuleManager.GetBaseAddress(data.Module),
                    data.OffsetChain
                );

                if (dataAddr == IntPtr.Zero)
                {
                    values[data.Name] = null;
                    continue;
                }

                values[data.Name] = Memory.ReadWTypeMemory(dataAddr, data.DataType);
            }

            if (values.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[X] There's no values in addresses found!");
                Console.ResetColor();
            }
            else if (values.Count < GameData.AllFields.Length)
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
                Console.WriteLine($"[+] All values were found successfully!");
                Console.ResetColor();
            }

            foreach (var n in values)
            {
                Console.WriteLine($"{n.Key}: {n.Value ?? null}");
            }

            Thread.Sleep(100);
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
}
