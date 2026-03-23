using System.Runtime.InteropServices;

namespace CS16Cheat.core;

public static class Memory
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool VirtualProtectEx(
        nint hProcess,
        nint lpAddress,
        uint dwSize,
        uint flNewProtect,
        out int lpflOldProtect
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool ReadProcessMemory(
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

    private const uint PAGE_READWRITE = 0x04;

    public enum ReadType
    {
        Int64,
        UInt64,
        Int32,
        UInt32,
        Float,
        Double,
        Short,
        Bool,
        Byte,
    }

    internal static T? Read<T> (nint address) where T: struct
    {
        int size = Marshal.SizeOf<T>();
        var buffer = new byte[size];

        if (!ReadProcessMemory(ProcessManager.Handle, address, buffer, size, out int bytesRead))
        {
            int errorCode = Marshal.GetLastWin32Error();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error of reading memory in type {}. | ERROR_CODE: {errorCode}");
            Console.ResetColor();
            return null;
        }

        if (bytesRead < size)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Read {bytesRead} bytes, expected {size}");
            Console.ResetColor();
            return null;
        }

        return type switch
        {
            ReadType.Int64 => BitConverter.ToInt64(buffer, 0),
            ReadType.Double => BitConverter.ToDouble(buffer, 0),
            ReadType.UInt64 => BitConverter.ToUInt64(buffer, 0),
            ReadType.Int32 => BitConverter.ToInt32(buffer, 0),
            ReadType.UInt32 => BitConverter.ToUInt32(buffer, 0),
            ReadType.Float => BitConverter.ToSingle(buffer, 0),
            ReadType.Short => BitConverter.ToInt16(buffer, 0),
            ReadType.Bool => BitConverter.ToBoolean(buffer, 0),
            ReadType.Byte => buffer[0],
            _ => throw new NotImplementedException(),
        };
    }
}
