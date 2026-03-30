using CS16Cheat.LWOperations;

namespace CS16Cheat.utils;

public enum ClientVersion
{
    V8684,
}

public static class Info
{
    private const string RepoUrl = "https://github.com/ASDlikeS/CS-1.6-Cheat/issues/new";

    public static ClientVersion CurrentVersion { get; private set; }

    public static void ShowStartHandle()
    {
        Console.WriteLine(
            "Choose CS 1.6 version build (You can check it, using command `version` in game terminal): "
        );

        while (true)
        {
            Console.Write("1. Aug 3 2020 (8684) build\n2. I have another game version.\nChoose: ");

            if (int.TryParse(Console.ReadLine(), out int answer))
            {
                switch (answer)
                {
                    case 1:
                        CurrentVersion = ClientVersion.V8684;
                        return;
                    case 2:
                        HandleUnsupportedVersion();
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter 1 or 2.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }
        }
    }

    private static void HandleUnsupportedVersion()
    {
        ProcessManager.CloseHandle(ProcessManager.Handle);
        Console.WriteLine(
            $"We can implement your build if you create new issue on my github repo: {RepoUrl}"
        );
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
        Environment.Exit(0);
    }
}
