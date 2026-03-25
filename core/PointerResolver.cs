using static CS16Cheat.core.Memory;

namespace CS16Cheat.core;

internal static class PointerResolver
{
    internal static nint FollowPointerChain(nint startAddress, int[] offsets)
    {
        if (startAddress == IntPtr.Zero)
        {
            return IntPtr.Zero;
        }

        nint currentAddress = startAddress + offsets[0];

        byte[] buffer = new byte[4];

        for (int i = 1; i < offsets.Length; i++)
        {
            // Console.WriteLine($"[engine.dll + 0x{offsets[0]:X}] = 0x{currentAddress:X}");
            if (
                !ReadProcessMemory(
                    ProcessManager.Handle,
                    currentAddress,
                    buffer,
                    buffer.Length,
                    out _
                )
            )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    $"[X] Failed to read at 0x{currentAddress - offsets[i]:X} + {offsets[i]}"
                );
                Console.ResetColor();
                return IntPtr.Zero;
            }
            currentAddress = BitConverter.ToInt32(buffer, 0);
            // Console.ForegroundColor = ConsoleColor.Green;
            // Console.WriteLine($"READ SUCCESS! 0x{currentAddress:X}");
            // Console.ResetColor();
            currentAddress += offsets[i];
            // Console.WriteLine($"0x{currentAddress:X} + 0x{offsets[i]:X}");
        }

        // Console.WriteLine(new string('=', Console.WindowWidth));

        return currentAddress;
    }
}
