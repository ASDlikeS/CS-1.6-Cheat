using System.ComponentModel;
using CS16Cheat.core;
using CS16Cheat.LWOperations;

namespace CS16Cheat.utils;

public static class Info
{
    public static int[] CLIENT_VERSIONS { get; } = [8684, 10039];
    public static int CURRENT_VERSION { get; private set; } = 8684;

    public static void ShowStartHandle()
    {
        Console.Write(
            "Enter CS 1.6 version (You can check it, using command `version` in client terminal): "
        );
        if (!int.TryParse(Console.ReadLine(), out int answer))
        {
            throw new InvalidDataException(
                "[X] You have to enter only numbers of your CS 1.6 version"
            );
        }

        CURRENT_VERSION = answer;

        Console.WriteLine("[*] Checking client version...");

        if (!CLIENT_VERSIONS.Contains(CURRENT_VERSION))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(
                "[!] Your client version doesn't exist in tested ones. Tested on :[Aug 3 2020 (8684) build | Apr 19 2024 (10039)].\nYou can check it, using command `version` in client terminal."
            );
            Console.ResetColor();
            Console.Write("You sure of using one of these builds? [Y/n] ");
            Utils.GetChooseMessage();
        }
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[+] Version {CURRENT_VERSION} detected. Compatibiliy x86-Arch");
        Console.ResetColor();
        Console.WriteLine($"[*] Configure cheat by current version...");
    }
}
