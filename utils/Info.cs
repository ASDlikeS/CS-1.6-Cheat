using CS16Cheat.core;
using CS16Cheat.LWOperations;

namespace CS16Cheat.utils;

public static class Info
{
    public const int CLIENT_VERSION = 8684;

    public static void ShowStartHandle()
    {
        int isCorrectVerison = Memory.ReadInt32(
            ModuleManager.GetBaseAddress(Modules.hw) + Offsets.clientVersion
        );
        if (CLIENT_VERSION != isCorrectVerison)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(
                "[!] This cheat works and tested on version:[Aug 3 2020 (8684) build].\nYou can check your CS 1.6 version, using command `version` in client terminal."
            );
            Console.ResetColor();
            Console.Write("You sure of using 8684 build? [Y/n] ");
            Utils.GetChooseMessage();
        }
    }
}
