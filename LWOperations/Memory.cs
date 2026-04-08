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
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern bool ReadProcessMemory(
        nint hProcess,
        nint lpBaseAddress,
        byte[] lpBuffer,
        int dwSize,
        out int lpNumberOfBytesRead
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern bool WriteProcessMemory(
        nint hProcess,
        nint lpBaseAddress,
        byte[] lpBuffer,
        int dwSize,
        out int lpNumberOfBytesWritten
    );

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
    public static Vector3? ReadVec3(nint address)
    {
        var buffer = new byte[12];
        try
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
                return null;
            if (bytesRead < buffer.Length)
                return null;
        }
        catch
        {
            return null;
        }
        return new Vector3(
            BitConverter.ToSingle(buffer, 0),
            BitConverter.ToSingle(buffer, 4),
            BitConverter.ToSingle(buffer, 8)
        );
    }

    public static float[]? ReadMatrix(nint address, int length)
    {
        if (length <= 0)
            return null;

        var array = new float[length];
        var buffer = new byte[length * 4];
        if (
            !ReadProcessMemory(
                ProcessManager.Handle,
                address,
                buffer,
                buffer.Length,
                out int bytesRead
            )
        )
            return null;
        if (bytesRead < buffer.Length)
            return null;

        Buffer.BlockCopy(buffer, 0, array, 0, bytesRead);
        return array;
    }

    public static int? ReadInt32(nint address)
    {
        try
        {
            var buffer = new byte[4];
            if (
                !ReadProcessMemory(
                    ProcessManager.Handle,
                    address,
                    buffer,
                    buffer.Length,
                    out int bytesRead
                )
            )
                return null;
            if (bytesRead < buffer.Length)
                return null;
            return BitConverter.ToInt32(buffer, 0);
        }
        catch
        {
            return null;
        }
    }

    public static float? ReadFloat(nint address)
    {
        var buffer = new byte[4];

        try
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
                return null;
            if (bytesRead < buffer.Length)
                return null;
        }
        catch
        {
            return null;
        }

        return BitConverter.ToSingle(buffer, 0);
    }

    public static nint? ReadPointer(nint address)
    {
        var buffer = new byte[IntPtr.Size];
        try
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
                return null;
            if (bytesRead < buffer.Length)
                return null;
        }
        catch
        {
            return null;
        }
        return IntPtr.Size == 4
            ? new nint(BitConverter.ToInt32(buffer, 0))
            : new nint(BitConverter.ToInt64(buffer, 0));
    }

    public static bool WriteBytes(nint address, int value)
    {
        byte[] buffer = BitConverter.GetBytes(value);

        if (
            !WriteProcessMemory(
                ProcessManager.Handle,
                address,
                buffer,
                buffer.Length,
                out int BytesWritten
            )
        )
            return false;

        return buffer.Length == BytesWritten;
    }

    public static bool WriteVec2(nint address, Vector2 value)
    {
        byte[] bufferX = BitConverter.GetBytes(value.X);
        byte[] bufferY = BitConverter.GetBytes(value.Y);

        var buffer = new byte[8];

        Buffer.BlockCopy(bufferX, 0, buffer, 0, 4);
        Buffer.BlockCopy(bufferY, 0, buffer, 4, 4);

        if (
            !WriteProcessMemory(
                ProcessManager.Handle,
                address,
                buffer,
                buffer.Length,
                out int BytesWritten
            )
        )
            return false;

        return buffer.Length == BytesWritten;
    }
}
