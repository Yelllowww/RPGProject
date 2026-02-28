internal static class Draw
{
    public static void WriteFooter(string message, ConsoleColor baseBg, ConsoleColor baseFg)
    {
        int leftAntes = Console.CursorLeft;
        int topAntes = Console.CursorTop;

        var prevBg = Console.BackgroundColor;
        var prevFg = Console.ForegroundColor;

        int width = Math.Max(1, Console.WindowWidth);
        int y = Math.Clamp(Console.WindowHeight - 1, 0, Console.WindowHeight - 1);

        string line = message ?? "";
        if (line.Length >= width)
            line = line.Substring(0, width - 1);

        Console.SetCursorPosition(0, y);
        Console.BackgroundColor = baseBg;
        Console.ForegroundColor = baseFg;
        Console.Write(new string(' ', Math.Max(0, width - 1)));

        Console.SetCursorPosition(0, y);
        Console.BackgroundColor = prevBg;
        Console.ForegroundColor = prevFg;
        Console.Write(line);

        Console.BackgroundColor = prevBg;
        Console.ForegroundColor = prevFg;
        Console.SetCursorPosition(leftAntes, topAntes);
    }

    public static void DrawAscii(string[] lines, ConsoleColor baseBg, ConsoleColor baseFg, int reservedBottomLines)
    {
        if (lines is null || lines.Length == 0)
            return;

        int width = Math.Max(1, Console.WindowWidth);
        int height = Math.Max(1, Console.WindowHeight);

        int availableHeight = Math.Max(0, height - reservedBottomLines);
        if (availableHeight == 0)
            return;

        int maxArtWidth = Math.Max(10, width / 2);
        int x0 = width - maxArtWidth;
        if (x0 < 0)
            return;

        int leftAntes = Console.CursorLeft;
        int topAntes = Console.CursorTop;
        var prevBg = Console.BackgroundColor;
        var prevFg = Console.ForegroundColor;

        int drawLines = Math.Min(lines.Length, availableHeight);
        int startY = Math.Max(0, (availableHeight - drawLines) / 2);

        for (int i = 0; i < drawLines; i++)
        {
            int y = startY + i;
            string line = lines[i] ?? "";
            if (line.Length > maxArtWidth)
                line = line.Substring(0, maxArtWidth);

            Console.SetCursorPosition(x0, y);
            Console.BackgroundColor = baseBg;
            Console.ForegroundColor = baseFg;
            Console.Write(new string(' ', Math.Max(0, width - x0 - 1)));

            Console.SetCursorPosition(x0, y);
            Console.BackgroundColor = prevBg;
            Console.ForegroundColor = prevFg;
            Console.Write(line);
        }

        Console.BackgroundColor = prevBg;
        Console.ForegroundColor = prevFg;
        Console.SetCursorPosition(leftAntes, topAntes);
    }
}