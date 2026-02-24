using System.Text.Json;
using System.Text.Json.Serialization;

Console.Title = "RPGProject";
//Console.BackgroundColor = ConsoleColor.DarkRed;
//Console.ForegroundColor = ConsoleColor.Black;
var json = File.ReadAllText("agentes.json");
var agentes = JsonSerializer.Deserialize<Dictionary<string, Agente>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
) ?? new Dictionary<string, Agente>(StringComparer.OrdinalIgnoreCase);
while (true) {
    Console.Clear();
    Console.WriteLine("----------RPGProject menu----------\n");
    Console.WriteLine($"Escolha um agente: {string.Join(", ", agentes.Keys)}\n");
    string input = Console.ReadLine();
    if (input == "0") {break;}
    if (agentes.TryGetValue(input, out var agente))
    {
        Console.Clear();
        Console.WriteLine($"{input}:\n");
        Console.WriteLine($"Pontos de vida: {agente.PV}");    
        Console.WriteLine($"Sanidade: {agente.Sanidade}");
        Console.WriteLine($"Pontos de esforço: {agente.PE}");
        Console.WriteLine($"Defesa: {agente.Defesa}");
        Console.WriteLine($"Esquiva: {agente.Esquiva}");
        Console.WriteLine("\nPressione Enter para voltar ao menu...");
        string exit = Console.ReadLine();
        if (exit == "0") {break;}

    }
    else
    {
        Console.Clear();
        Console.WriteLine("Nome incorreto/não existente");
        Console.WriteLine("\nPressione Enter para voltar ao menu...");
        string exit = Console.ReadLine();
        if (exit == "0") {break;}
    }
} 
class Agente
{
    public string Nome { get; set; } = "";
    public int PV { get; set; }
    public int PE { get; set; }
    public int Sanidade { get; set; }
    public int Defesa { get; set; }
    public int Esquiva { get; set; }
}