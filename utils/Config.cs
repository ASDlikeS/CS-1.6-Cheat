using System.Text.Json;

namespace CS16Cheat.utils;

record Vec3
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}

record ConfigData
{
    public float LocalFOV { get; set; }
    public string GamePath { get; set; } = null!;
    public Vec3 ColorESP { get; set; } = new Vec3();
}

static class Config
{
    public static ConfigData? ConfigData { get; set; }
    private static string ConfigPath =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

    public static void Initialize()
    {
        Console.WriteLine("[*] Intialization config.json...");
        if (!File.Exists(ConfigPath))
        {
            Create();
            return;
        }
        else
            LoadData();
    }

    private static void Create()
    {
        Console.Write(
            "Welcome to CS 1.6 cheat made by ASD. In first starting programm.\nWould you like to add the path to the game's executable file for automatic launch? [Y/n] "
        );
        string? answer = Console.ReadLine()?.ToLower();
        if (answer == "n")
        {
            Utils.WriteWarningMessage(
                "[!] Your game should be have already opened when you start cheat."
            );
            return;
        }

        bool correct = false;
        while (!correct)
        {
            Console.Write("Enter url to CS 1.6 .exe file: ");
            string? url = Console.ReadLine();
            if (url == null)
            {
                Utils.WriteErrorMessage("[X] GAME PATH must be not NULL!");
                continue;
            }
            Console.Write($"Does this url correct?: {url} [Y/n]");

            string? answer2 = Console.ReadLine();
            if (answer2 == "n")
            {
                continue;
            }

            ConfigData = new() { GamePath = url };
            correct = true;
        }

        SaveData();
    }

    private static void LoadData()
    {
        string configText = File.ReadAllText(ConfigPath!);
        var JSON =
            Utils.Deserialize<ConfigData>(configText)
            ?? throw new JsonException("Couldn't deserialize config file data");

        ConfigData = new()
        {
            GamePath = JSON.GamePath,
            LocalFOV = JSON.LocalFOV,
            ColorESP = JSON.ColorESP,
        };
    }

    private static void SaveData()
    {
        string JSON = Utils.Serialize(ConfigData);
        File.WriteAllText(ConfigPath!, JSON);
    }

    public static void ChangeGamePath()
    {
        Console.WriteLine(
            @"Type another path below to your game exe. It has to be in root game directory.Possible game path should looks like: C:\Games\CS 1.6\hl.exe:"
        );
        string? answer = Console.ReadLine();
        if (answer == null)
        {
            ChangeGamePath();
            return;
        }
        ConfigData!.GamePath = answer;
        SaveData();
        Utils.WriteSuccessMessage("[+] Config changed successfully... Restart the cheat.");
        Environment.Exit(1);
    }
}
