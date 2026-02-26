using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;

Console.Title = "RPGProject";
//Console.BackgroundColor = ConsoleColor.DarkRed;
//Console.ForegroundColor = ConsoleColor.Black;
string cabecalho = "----------- RPGProject -----------\n";
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
        while (true)
        {
            Console.Clear();
            Console.WriteLine(cabecalho);
            Console.WriteLine($"{input}:\n");
            Console.WriteLine($"Pontos de vida: {agente.PV}");    
            Console.WriteLine($"Sanidade: {agente.Sanidade}");
            Console.WriteLine($"Pontos de esforço: {agente.PE}");
            Console.WriteLine($"Defesa: {agente.Defesa}");
            Console.WriteLine($"Esquiva: {agente.Esquiva}");
            Console.WriteLine("\n\nAperte L para abrir a ficha");
            Console.WriteLine("\nPressione enter para voltar ao menu...");
            string exit = Console.ReadLine();
            if (exit == "l")
            {
                if (string.IsNullOrWhiteSpace(agente.Link))
                {
                    Console.Clear();
                    Console.WriteLine(cabecalho);
                    Console.WriteLine("O Agente não possui link cadastrado");
                    Console.ReadLine();
                    continue;
                }
                else
                {
                    Process.Start(new ProcessStartInfo{
                    FileName = agente.Link,
                    UseShellExecute = true
                    });
                }
            }
            else if (exit == "e")
            {
                bool salvou = MiniEditor.EditarAgenteEmTelaUnica(agente, cabecalho, input);

                if (salvou)
                {
                    agentes[input] = agente; // garante que o dicionário atualiza

                    MiniEditor.SalvarAgentes("agentes.json", agentes); // grava no arquivo
                }
            }
            else if (exit == "") {break;}
            else {return;}
        }
    }
    else
    {
        Console.Clear();
        Console.WriteLine(cabecalho);
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
    public string Link { get; set;}
}