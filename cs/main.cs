using System.Text.Json;
using System.Diagnostics;
using System.Globalization;

Console.Title = "RPGProject";
Console.BackgroundColor = ConsoleColor.DarkRed;
Console.ForegroundColor = ConsoleColor.Black;
var baseBg = Console.BackgroundColor;
var baseFg = Console.ForegroundColor;

var asciiLines = (AsciiArt.Ascii ?? "").Replace("\r", "").Split('\n');

var comparadorNomes = CultureInfo
    .GetCultureInfo("pt-BR")
    .CompareInfo
    .GetStringComparer(CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);
    
string cabecalho = "----------- RPGProject -----------\n";
var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "agentes.json"));
var agentes = JsonSerializer.Deserialize<Dictionary<string, Agente>>(json, new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
}) ?? new Dictionary<string, Agente>();

agentes = new Dictionary<string, Agente>(agentes, comparadorNomes);

while (true) {
    Console.Clear();
    Console.WriteLine("----------RPGProject Menu----------\n");
    Console.WriteLine($"Escolha um agente: {string.Join(", ", agentes.Keys)}\n");
    Draw.DrawAscii(asciiLines, baseBg, baseFg, reservedBottomLines: 1);
    Draw.WriteFooter("Aperte P para sair, ENTER para abrir todas as fichas...", baseBg, baseFg);
    string input = (Console.ReadLine() ?? "").Trim();

    

    if (input == "p") {break;}
    else if (input == "")
    {
        Abrelink(agentes);
    }
    else if (agentes.TryGetValue(input, out var agente))
    {   
        string key = input;                         
        foreach (var item in agentes)                       //pega o valor da chave do agente x
        {
            if (ReferenceEquals(item.Value, agente))
            {
                key = item.Key;
                break;
            }
        }
        while (true)
        {
            Console.Clear();
            Console.WriteLine(cabecalho);
            Console.WriteLine($"{key}:\n");
            Console.WriteLine($"Pontos de vida: {agente.PV}");    
            Console.WriteLine($"Sanidade: {agente.Sanidade}");
            Console.WriteLine($"Pontos de esforço: {agente.PE}");
            Console.WriteLine($"Defesa: {agente.Defesa}");
            Console.WriteLine($"Esquiva: {agente.Esquiva}");
            Console.WriteLine("\n\nEditar - E   Abrir ficha - L   Voltar - P");

            Draw.WriteFooter("Aperte 0 para um segredo shhh...", baseBg, baseFg);
            Draw.DrawAscii(asciiLines, baseBg, baseFg, reservedBottomLines: 1);
            string? exit = Console.ReadLine();
            if (exit == "l")
            {
                Abrelink(agentes, input);
            }
            else if (exit == "e")
            {
                bool salvou = MiniEditor.EditarAgenteEmTelaUnica(agente, cabecalho, input);

                if (salvou)
                {
                    agentes[input] = agente;

                    MiniEditor.SalvarAgentes(Path.Combine(AppContext.BaseDirectory, "agentes.json"), agentes);
                }
            }  
            else if (exit == "p") {break;}
            else if (exit == "0") {Process.Start(new ProcessStartInfo{
                                FileName = "https://youtu.be/dQw4w9WgXcQ?si=xBoML9ejwzdXOqWh",
                                UseShellExecute = true
                                });
            }
            else {continue;}
        }
    }
    else
    {
        Console.Clear();
        Console.WriteLine(cabecalho);
        Console.WriteLine("Nome incorreto/não existente");
        Console.WriteLine("\nPressione Enter para voltar ao menu...");
        string? exit = Console.ReadLine();
        if (exit == "0") {break;}
    }
}
Console.ResetColor();
Console.Clear();
void Abrelink(Dictionary<string, Agente> agentes, string? nome = null)
{
    if (!string.IsNullOrWhiteSpace(nome))
    {
        if (!agentes.TryGetValue(nome, out var agente)) {return;}

        if (string.IsNullOrWhiteSpace(agente.Link))
        {
            Console.Clear();
            Console.WriteLine(cabecalho);
            Console.WriteLine("Não há link registrado para esse agente");
            Console.WriteLine("\nPressione Enter para voltar ao menu...");
            string? exit = Console.ReadLine();
            return;
        }
        Process.Start(new ProcessStartInfo{
        FileName = agente.Link,
        UseShellExecute = true
        });
        return;
    }
    else
    {
        var nomesFalha = new List<string>();
        foreach (var (key, agente) in agentes)
        {
            if (string.IsNullOrWhiteSpace(agente.Link))
            {
                nomesFalha.Add(key);
                continue;
            }
            Process.Start(new ProcessStartInfo{
            FileName = agente.Link,
            UseShellExecute = true
            });
        }
        if (nomesFalha.Count > 0)
        {
            Console.Clear();
            Console.WriteLine(cabecalho);
            Console.WriteLine($"Os agentes: {string.Join(", ", nomesFalha)} estão sem link registrado");
            Console.WriteLine("\nPressione Enter para voltar ao menu...");
            Console.ReadLine();
        }
    }
}
class Agente
{
    public int PV { get; set; }
    public int PE { get; set; }
    public int Sanidade { get; set; }
    public int Defesa { get; set; }
    public int Esquiva { get; set; }
    public string? Link { get; set; }
}