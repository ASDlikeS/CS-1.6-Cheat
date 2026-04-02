using System.Diagnostics;
using System.Globalization;
using CS16Cheat.utils;

namespace CS16Cheat.LWOperations;

public enum Modules
{
    hw,
    client,
}

internal static class ModuleManager
{
    private static readonly Dictionary<Modules, nint> _baseAddresses = [];

    internal static nint GetBaseAddress(Modules module)
    {
        if (!_baseAddresses.TryGetValue(module, out nint value))
            throw new KeyNotFoundException($"Module {module} not initialized");
        return value;
    }

    internal static void Initialize()
    {
        Console.WriteLine("[*] Modules initialization...");
        bool isSuccess = true;
        foreach (Modules module in Enum.GetValues<Modules>())
        {
            var process =
                ProcessManager.GameProcess
                ?? throw new NotImplementedException("ERROR: PROCESS_NOT_INITIALIZED");
            nint baseAddress = GetModuleBaseAddress(process, $"{module}.dll");
            if (baseAddress == IntPtr.Zero)
                isSuccess = false;
            _baseAddresses[module] = baseAddress;
        }

        if (!isSuccess)
        {
            Utils.WriteWarningMessage(
                "[!] Initialization end with errors...\nAre you sure you want to continue? Some features may doesn't work, this will affect the gaming experience [y/N]: "
            );
            string answer = Console.ReadLine()?.ToLower(CultureInfo.CurrentCulture) ?? "";
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

        Utils.WriteErrorMessage($"[X] Module {moduleName} not found, some features may not work");
        return IntPtr.Zero;
    }
}
