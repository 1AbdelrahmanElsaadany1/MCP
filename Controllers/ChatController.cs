using MCPSharp;
using MCPv1.Models.Services;
using MCPv1.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MCPv1.Controllers
{
    public class ChatController : Controller
    {
        private readonly OllamaService _ollama;
        private readonly IStudentService _studentService;
        private readonly IDepartmentService _departmentService;
        private readonly MCPClient _mcpClient;

        public ChatController(OllamaService ollama, IStudentService studentService, IDepartmentService departmentService, MCPClient mcpClient)
        {
            _ollama = ollama;
            _studentService = studentService;
            _departmentService = departmentService;
            _mcpClient = mcpClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] ChatRequest request)
        {
            var tools = await _mcpClient.GetToolsAsync();
            var toolList = string.Join("\n", tools.Select(t => $"- {t.Name}: {t.Description}"));

            var systemPrompt = $@"
You are a student management assistant.

CRITICAL RULES:
1. Output EXACTLY ONE JSON object per response
2. Never add text before or after the JSON

TOOLS AVAILABLE:
{toolList}

OUTPUT FORMAT:
{{ ""tool"": ""tool_name"", ""params"": {{ ""key"": ""value"" }} }}

EXAMPLES:
User: show all students
You: {{ ""tool"": ""get_students"", ""params"": {{}} }}

User: show student names
You: {{ ""tool"": ""get_students"", ""params"": {{ ""query"": ""$select=Name"" }} }}

User: show 2 students
You: {{ ""tool"": ""get_students"", ""params"": {{ ""query"": ""$top=2"" }} }}

User: students in physics department
You: {{ ""tool"": ""get_students"", ""params"": {{ ""query"": ""$filter=DepartmentName eq 'physics'"" }} }}";

            var aiResponse = await _ollama.ChatAsync(request.History, request.Message, systemPrompt);

            if (_ollama.TryParseToolCall(aiResponse, out var toolName, out var parameters))
            {
                var toolResult = await ExecuteTool(toolName, parameters);
                return Ok(new { reply = toolResult, history = request.History });
            }

            return Ok(new { reply = aiResponse, history = request.History });
        }

        private async Task<string> ExecuteTool(string toolName, Dictionary<string, JsonElement> parameters)
        {
            var args = parameters.ToDictionary(p => p.Key, p => p.Value.ValueKind == JsonValueKind.Number ? p.Value.GetInt32() : (object)p.Value.ToString());
            var result = await _mcpClient.CallToolAsync(toolName, args);
            return result.Content.FirstOrDefault()?.Text ?? "No content";
        }
        public class ChatRequest
        {
            public string Message { get; set; }
            public List<object> History { get; set; } = new();
        }
    }
}