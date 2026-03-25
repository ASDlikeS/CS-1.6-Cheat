using System.Runtime.InteropServices;

namespace CS16Cheat.core;

public enum MemoryType
{
    Int16,
    Int32,
    Int64,
    Single,
    Double,
}

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

    // private const uint PAGE_READWRITE = 0x04;

    internal static T? Read<T>(nint address)
        where T : struct
    {
        int size = Marshal.SizeOf<T>();
        var buffer = new byte[size];

        if (!ReadProcessMemory(ProcessManager.Handle, address, buffer, size, out int bytesRead))
        {
            int errorCode = Marshal.GetLastWin32Error();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(
                $"Error of reading memory in type {typeof(T).Name}. | ERROR_CODE: {errorCode}"
            );
            Console.ResetColor();
            return null;
        }

        if (bytesRead < size)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[!] Read {bytesRead} bytes, but expected = {size}");
            Console.ResetColor();
            return null;
        }

        unsafe
        {
            fixed (byte* ptr = buffer)
            {
                return Marshal.PtrToStructure<T>((nint)ptr);
            }
        }
    }

    internal static bool Write<T>(nint address, T value)
        where T : struct
    {
        int size = Marshal.SizeOf<T>();
        var buffer = new byte[size];

        unsafe
        {
            fixed (byte* ptr = buffer)
            {
                Marshal.StructureToPtr(value, (nint)ptr, false);
            }
        }

        if (!WriteProcessMemory(ProcessManager.Handle, address, buffer, size, out int bytesWritten))
        {
            int errorCode = Marshal.GetLastWin32Error();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(
                $"Error of writing memory in type {typeof(T).Name}. | ERROR_CODE: {errorCode}"
            );
            Console.ResetColor();
            return false;
        }

        if (bytesWritten < size)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[!] Read {bytesWritten} bytes, but expected = {size}");
            Console.ResetColor();
            return false;
        }

        return true;
    }
}
