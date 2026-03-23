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
                Console.WriteLine($"[X] Failed to read at 0x{currentAddress.ToInt64():X}");
                Console.ResetColor();
                return IntPtr.Zero;
            }

            currentAddress = (nint)BitConverter.ToInt32(buffer, 0);
            currentAddress += offsets[i];
        }

        return currentAddress;
    }
}
