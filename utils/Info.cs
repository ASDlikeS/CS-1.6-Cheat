namespace CS16Cheat.utils;

static class Info
{
    internal static void ShowStartHandle()
    {
        Console.WriteLine(
            "This program uses 2 type of cs 1.6, classic and custom versions, if classic mode won't work correctly, you can switch it on custom mode."
        );
        Console.WriteLine("1. Classic CS 1.6\n2. Custom bundle CS 1.6");
        Console.Write("Choose [1 default]: ");
        _ = int.TryParse(Console.ReadLine(), out int answer);
        if (answer == 2)
        {
            Program.openMode = "custom";
        }
    }

    internal static void ShowKeybinds()
    {
        Console.WriteLine(new string('=', Console.WindowWidth));
        Console.WriteLine("Key binds and their property:");
        Console.WriteLine("[f1] Set infinity ammo on current slot");
        Console.WriteLine("[f2] Set infinity HP");
        Console.WriteLine("[f3] Set 16_000 money");
    }
}
