using System.Diagnostics;

namespace CS16Cheat.LWOperations;

public enum Modules
{
    hw,
    client,
    engine,
}

internal static class ModuleManager
{
    private static readonly Dictionary<Modules, nint> _baseAddresses = [];

    internal static nint GetBaseAddress(Modules module)
    {
        if (!_baseAddresses.ContainsKey(module))
            throw new KeyNotFoundException($"Module {module} not initialized");
        return _baseAddresses[module];
    }

    internal static void Initialize()
    {
        Console.WriteLine("[*] Modules initialization...");
        bool isSuccess = true;
        foreach (Modules module in Enum.GetValues<Modules>())
        {
            nint baseAddress = GetModuleBaseAddress(ProcessManager.GameProcess, $"{module}.dll");
            if (baseAddress == IntPtr.Zero)
                isSuccess = false;
            _baseAddresses[module] = baseAddress;
        }

        if (!isSuccess)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[!] Initialization end with errors...");
            Console.Write(
                "You're sure you want to continue? Some features may NOT work, this will affect the gaming experience [y/N]: "
            );
            Console.ResetColor();
            string answer = Console.ReadLine()?.ToLower() ?? "";
            if (answer != "y")
            {
                ProcessManager.CloseHandle(ProcessManager.Handle);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
            }
            Console.WriteLine(
                "Perhaps if you notice any bugs and malfunctions, or want to fix this problem, contact the issue on my github: https://github.com/ASDlikeS/CS-1.6-Cheat/issues/new"
            );
        }
    }

    private static nint GetModuleBaseAddress(Process process, string moduleName)
    {
        foreach (ProcessModule module in process.Modules)
        {
            if (module.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"[+] {moduleName} at 0x{module.BaseAddress.ToInt64():X}");
                return module.BaseAddress;
            }
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[-] Module {moduleName} not found, some features may not work");
        Console.ResetColor();
        return IntPtr.Zero;
    }
}
