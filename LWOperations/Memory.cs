using System.ComponentModel;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CS16Cheat.LWOperations;

public static class Memory
{
    // [DllImport("kernel32.dll", SetLastError = true)]
    // private static extern bool VirtualProtectEx(
    //     nint hProcess,
    //     nint lpAddress,
    //     uint dwSize,
    //     uint flNewProtect,
    //     out int lpflOldProtect
    // );

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(
        nint hProcess,
        nint lpBaseAddress,
        byte[] lpBuffer,
        int dwSize,
        out int lpNumberOfBytesRead
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteProcessMemory(
        nint hProcess,
        nint lpBaseAddress,
        byte[] lpBuffer,
        int dwSize,
        out int lpNumberOfBytesWritten
    );

    private static void ReadMemoryHandle(nint address, byte[] buffer)
    {
        if (
            !ReadProcessMemory(
                ProcessManager.Handle,
                address,
                buffer,
                buffer.Length,
                out int bytesRead
            )
        )
        {
            throw new Win32Exception(
                $"ERROR READING MEMORY | ERROR_CODE: {Marshal.GetLastWin32Error()}"
            );
        }
        if (bytesRead < buffer.Length)
        {
            throw new FormatException($"SMALL COUNT READING BYTES");
        }
    }

    // private const uint PAGE_READWRITE = 0x04;

    /// <summary>
    /// Read vector from given address memory in format (X, Y, Z).
    /// </summary>
    /// <param name="address">Intial address of first vector value(probably X).</param>
    /// <returns>Vector <see cref="Vector3"/>, read from memory.</returns>
    /// <remarks>
    /// Method reads 12 bytes (3 float by 4 bytes) starts since initial address.
    /// It is assumed that the data is arranged sequentially: X, Y, Z.
    /// </remarks>
    public static Vector3 ReadVec3(nint address)
    {
        var buffer = new byte[12];
        ReadMemoryHandle(address, buffer);
        return new Vector3(
            BitConverter.ToSingle(buffer, 0),
            BitConverter.ToSingle(buffer, 4),
            BitConverter.ToSingle(buffer, 8)
        );
    }

    public static int ReadInt32(nint address)
    {
        var buffer = new byte[4];
        ReadMemoryHandle(address, buffer);
        return BitConverter.ToInt32(buffer, 0);
    }

    public static nint ReadPointer(nint address)
    {
        var buffer = new byte[IntPtr.Size];
        ReadMemoryHandle(address, buffer);
        return IntPtr.Size == 4
            ? new nint(BitConverter.ToInt32(buffer, 0))
            : new nint(BitConverter.ToInt64(buffer, 0));
    }
}
