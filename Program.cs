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
                    values[data.Name] = "value not found, probably base address doesn't exist.";
                    continue;
                }

                values[data.Name] = Memory.ReadWTypeMemory(dataAddr, data.DataType);
            }

            if (values.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[X] There's no values in addresses found!".PadRight(20));
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
                Console.WriteLine($"[+] All values were found successfully!".PadRight(20));
                Console.ResetColor();
            }

            foreach (var n in values)
            {
                Console.WriteLine($"{n.Key}: {n.Value ?? null}");
            }

            int endLine = Console.CursorTop;
            Thread.Sleep(100);

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
}
