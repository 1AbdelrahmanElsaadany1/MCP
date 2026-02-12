using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MCPv1.Services
{
    public class OllamaService
    {
        private readonly HttpClient _http;
        
        public OllamaService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> ChatAsync(List<object> history, string userMessage, string systemPrompt)
        {
            history.Add(new { role = "user", content = userMessage });

            var body = new
            {
                model = "llama3",
                messages = new[]
                {
            new { role = "system", content = systemPrompt }
        }.Concat(history.Cast<object>()).ToArray(),
                stream = false
            };

            var response = await _http.PostAsync(
                "http://localhost:11434/api/chat",
                new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            );

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("message").GetProperty("content").GetString();
        }

        public bool TryParseToolCall(string response, out string toolName, out Dictionary<string, JsonElement> parameters)
        {
            toolName = null;
            parameters = null;

            try
            {
                var trimmed = response.Trim();
                if (!trimmed.StartsWith("{")) return false;

                var jsonObjects = trimmed.Split('\n')
                    .Where(line => line.Trim().StartsWith("{"))
                    .ToList();

                if (jsonObjects.Any())
                {
                    trimmed = jsonObjects.Last().Trim();
                }

                var doc = JsonDocument.Parse(trimmed);
                var root = doc.RootElement;

                toolName = root.GetProperty("tool").GetString();

                parameters = new Dictionary<string, JsonElement>();

                if (root.TryGetProperty("params", out var paramsElement))
                {
                    foreach (var prop in paramsElement.EnumerateObject())
                    {
                        parameters[prop.Name] = prop.Value.Clone();
                    }
                }

                Console.WriteLine($"✅ Parsed tool: {toolName}");
                if (parameters.Any())
                {
                    Console.WriteLine($"   Parameters: {string.Join(", ", parameters.Select(p => $"{p.Key}={p.Value}"))}");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Parse error: {ex.Message}");
                return false;
            }
        }
    }
}