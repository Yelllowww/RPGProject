using System.Text;
using System.Text.Json;

static class MiniEditor
{
    public static bool EditarAgenteEmTelaUnica(Agente agente, string cabecalho, string agenteKey)
    {
        var labels = new[] { "PV", "PE", "Sanidade", "Defesa", "Esquiva", "Link" };
        var isInt  = new[] {  true,  true,      true,     true,      true,  false };

        var buffers = new[]
        {
            new StringBuilder(agente.PV.ToString()),
            new StringBuilder(agente.PE.ToString()),
            new StringBuilder(agente.Sanidade.ToString()),
            new StringBuilder(agente.Defesa.ToString()),
            new StringBuilder(agente.Esquiva.ToString()),
            new StringBuilder(agente.Link ?? "")
        };

        var cursors = new int[buffers.Length];
        for (int i = 0; i < cursors.Length; i++)
            cursors[i] = buffers[i].Length;

        int selected = 0;
#pragma warning disable CA1416
        bool oldCursor = Console.CursorVisible;
        Console.CursorVisible = false;
#pragma warning restore CA1416

        try
        {
            while (true)
            {
                Render(cabecalho, agenteKey, labels, buffers, selected, cursors[selected]);

                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Escape)
                    return false;

                if (key.Key == ConsoleKey.Enter)
                {
                    if (!TryApply(buffers, agente, out var error))
                    {
                        Console.WriteLine();
                        Console.WriteLine(error);
                        Console.WriteLine("Pressione qualquer tecla para continuar...");
                        Console.ReadKey(intercept: true);
                        continue;
                    }
                    return true;
                }

                if (key.Key == ConsoleKey.Tab || key.Key == ConsoleKey.DownArrow)
                {
                    selected = (selected + 1) % labels.Length;
                    continue;
                }

                if (key.Key == ConsoleKey.UpArrow)
                {
                    selected = (selected - 1 + labels.Length) % labels.Length;
                    continue;
                }

                if (key.Key == ConsoleKey.LeftArrow)
                {
                    if (cursors[selected] > 0) cursors[selected]--;
                    continue;
                }

                if (key.Key == ConsoleKey.RightArrow)
                {
                    if (cursors[selected] < buffers[selected].Length) cursors[selected]++;
                    continue;
                }

                if (key.Key == ConsoleKey.Home)
                {
                    cursors[selected] = 0;
                    continue;
                }

                if (key.Key == ConsoleKey.End)
                {
                    cursors[selected] = buffers[selected].Length;
                    continue;
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (cursors[selected] > 0)
                    {
                        buffers[selected].Remove(cursors[selected] - 1, 1);
                        cursors[selected]--;
                    }
                    continue;
                }

                if (key.Key == ConsoleKey.Delete)
                {
                    if (cursors[selected] < buffers[selected].Length)
                        buffers[selected].Remove(cursors[selected], 1);
                    continue;
                }

                if (char.IsControl(key.KeyChar))
                    continue;

                if (isInt[selected] && !char.IsDigit(key.KeyChar))
                    continue;

                buffers[selected].Insert(cursors[selected], key.KeyChar);
                cursors[selected]++;
            }
        }
        finally
        {
#pragma warning disable CA1416
            Console.CursorVisible = oldCursor;
#pragma warning restore CA1416
        }
    }

    public static void SalvarAgentes(string path, Dictionary<string, Agente> agentes)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(path, JsonSerializer.Serialize(agentes, options));
    }

    static void Render(
        string cabecalho,
        string agenteKey,
        string[] labels,
        StringBuilder[] buffers,
        int selected,
        int cursorPosInSelectedField)
    {
        Console.Clear();
        Console.WriteLine(cabecalho);
        Console.WriteLine($"Editando: {agenteKey}");
        Console.WriteLine();
        Console.WriteLine("Enter: salvar | Esc: cancelar");
        Console.WriteLine("Tab/↑/↓: navegar | ←/→: mover cursor | Digitar: inserir | Backspace/Delete: apagar");
        Console.WriteLine();

        int cursorLine = -1;
        int cursorCol = -1;

        for (int i = 0; i < labels.Length; i++)
        {
            bool isSel = i == selected;
            string prefix = isSel ? "> " : "  ";
            string left = $"{prefix}{labels[i]}: ";

            int lineStartTop = Console.CursorTop;
            int lineStartLeft = 0;

            if (isSel)
            {
                var bg = Console.BackgroundColor;
                var fg = Console.ForegroundColor;

                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.Black;

                Console.Write(left);
                Console.Write(buffers[i].ToString());
                Console.WriteLine();

                Console.BackgroundColor = bg;
                Console.ForegroundColor = fg;

                cursorLine = lineStartTop;
                cursorCol = left.Length + cursorPosInSelectedField;
            }
            else
            {
                Console.Write(left);
                Console.Write(buffers[i].ToString());
                Console.WriteLine();
            }

            _ = lineStartLeft;
        }

        if (cursorLine >= 0 && cursorCol >= 0)
        {
            cursorCol = Math.Clamp(cursorCol, 0, Math.Max(0, Console.BufferWidth - 1));
            cursorLine = Math.Clamp(cursorLine, 0, Math.Max(0, Console.BufferHeight - 1));
            Console.SetCursorPosition(cursorCol, cursorLine);
        }
    }

    static bool TryApply(StringBuilder[] buffers, Agente agente, out string error)
    {
        error = "";

        if (!TryParseInt(buffers[0], "PV", out var pv, out error)) return false;
        if (!TryParseInt(buffers[1], "PE", out var pe, out error)) return false;
        if (!TryParseInt(buffers[2], "Sanidade", out var san, out error)) return false;
        if (!TryParseInt(buffers[3], "Defesa", out var def, out error)) return false;
        if (!TryParseInt(buffers[4], "Esquiva", out var esq, out error)) return false;

        agente.PV = pv;
        agente.PE = pe;
        agente.Sanidade = san;
        agente.Defesa = def;
        agente.Esquiva = esq;
        agente.Link = buffers[5].ToString();

        return true;
    }

    static bool TryParseInt(StringBuilder sb, string label, out int value, out string error)
    {
        error = "";
        var text = sb.ToString();

        if (text.Length == 0)
        {
            value = 0;
            return true;
        }

        if (!int.TryParse(text, out value))
        {
            error = $"Valor inválido para {label}. Use apenas números.";
            return false;
        }

        return true;
    }
}
