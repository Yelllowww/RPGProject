using System.Text.Json;
using System.Text.Json.Serialization;

Console.Title = "RPGProject";
//Console.BackgroundColor = ConsoleColor.DarkRed;
//Console.ForegroundColor = ConsoleColor.Black;
Console.Clear();
var json = File.ReadAllText("agentes.json");
var agentes = JsonSerializer.Deserialize<Dictionary<string, Agente>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
) ?? new Dictionary<string, Agente>(StringComparer.OrdinalIgnoreCase);

Console.WriteLine("----------RPGProject menu----------\n");
Console.WriteLine($"Escolha um agente: {string.Join(", ", agentes.Keys)}");
string input = Console.ReadLine();
if (agentes.TryGetValue(input, out var agente))
{
    Console.WriteLine($"{input}:\n");
    Console.WriteLine($"Pontos de vida: {agente.PV}");    
    Console.WriteLine($"Sanidade: {agente.Sanidade}");
    Console.WriteLine($"Pontos de esforço: {agente.PE}");
    Console.WriteLine($"Defesa: {agente.Defesa}");
    Console.WriteLine($"Esquiva: {agente.Esquiva}");
}
else
{
    Console.WriteLine("Nome incorreto/não existente");
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