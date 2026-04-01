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
    public string GamePath { get; set; } = @"C:\Games\CS 1.6\hl.exe";
    public Vec3 ColorESP { get; set; } = new Vec3();
}

static class Config
{
    public static ConfigData ConfigData { get; set; } = new ConfigData();
    private static string ConfigPath =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

    public static void Initialize()
    {
        Console.WriteLine("[*] Intialization config.json...");
        if (!File.Exists(ConfigPath))
        {
            InitURL();
            return;
        }
        LoadData();
    }

    private static void InitURL()
    {
        Console.Write(
            "Welcome to CS 1.6 cheat made by ASD. In first starting programm.\nWould you like to add the path to the game's executable file for automatic launch? [Y/n] "
        );
        string? answer = Console.ReadLine()?.ToLower();
        if (answer == "n")
            return;

        bool correct = false;
        while (!correct)
        {
            Console.Write("Enter url to CS 1.6 .exe file: ");
            string? url = Console.ReadLine();
            if (url == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: url must be not null");
                Console.ResetColor();
                continue;
            }
            Console.Write($"Does this url correct?: {url} [Y/n]");

            string? answer2 = Console.ReadLine();
            if (answer2 == "n")
            {
                continue;
            }

            ConfigData.GamePath = url;
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

        ConfigData.GamePath = JSON.GamePath;
        ConfigData.LocalFOV = JSON.LocalFOV;
        ConfigData.ColorESP = JSON.ColorESP;
    }

    private static void SaveData()
    {
        string JSON = Utils.Serialize(ConfigData);
        File.WriteAllText(ConfigPath!, JSON);
    }
}
