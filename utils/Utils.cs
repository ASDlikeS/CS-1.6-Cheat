using System.Text.Json;
using CS16Cheat.LWOperations;

namespace CS16Cheat.utils;

internal static class Utils
{
    internal static int HOTKEY_AIM = 0x06;

    internal static void SetupCancelHandler()
    {
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            Utils.WriteWarningMessage("\n[!] Ctrl+C pressed. Exiting...");

            if (ProcessManager.Handle != IntPtr.Zero)
            {
                ProcessManager.CloseHandle(ProcessManager.Handle);
                WriteSuccessMessage("[+] Process handle closed successfully!");
            }
            Environment.Exit(0);
        };
    }

    internal static void GetChooseMessage()
    {
        var answer = Console.ReadLine()?.ToLower();
        if (answer != "y")
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }

    public static void WriteSuccessMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void WriteErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void WriteWarningMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static readonly JsonSerializerOptions s_writeOptions = new() { WriteIndented = true };
    public static readonly JsonSerializerOptions s_readOptions = new()
    {
        AllowTrailingCommas = true,
    };

    public static string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, s_writeOptions);
    }

    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, s_readOptions);
    }
}
