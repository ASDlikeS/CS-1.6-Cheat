using System.Runtime.InteropServices;
using static CS16Cheat.core.Initialization;
using static CS16Cheat.core.Memory;
using static CS16Cheat.Utils;

namespace CS16Cheat;

class Program
{
    internal const uint PROCESS_VM_READ = 0x0010;
    internal const uint PROCESS_QUERY_INFORMATION = 0x0400;

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool CloseHandle(IntPtr hObject);

    internal static int Main(string[] args)
    {
        nint handle = InitProcAndGetHandle();
        CancelCB(handle);

        //---------------------------------------------------------
        CloseHandle(handle);
        Console.WriteLine("Handle closed successfully");

        Console.WriteLine("\nPress ENTER to escape...");
        Console.ReadKey();
        return 0;
    }
}
