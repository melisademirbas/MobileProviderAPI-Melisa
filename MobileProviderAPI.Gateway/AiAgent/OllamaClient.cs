using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MobileProviderAPI.Gateway.AiAgent;

public sealed class OllamaClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<OllamaClient> _logger;

    public OllamaClient(HttpClient http, IConfiguration config, ILogger<OllamaClient> logger)
    {
        _http = http;
        _config = config;
        _logger = logger;
    }

    public async Task<string> ChatAsync(string systemPrompt, string userPrompt, CancellationToken ct)
    {
        var model = _config["Ollama:Model"] ?? "llama3.1";

        var payload = new OllamaChatRequest
        {
            Model = model,
            Stream = false,
            Messages =
            [
                new OllamaChatMessage { Role = "system", Content = systemPrompt },
                new OllamaChatMessage { Role = "user", Content = userPrompt }
            ]
        };

        var json = JsonSerializer.Serialize(payload, JsonOptions);
        using var resp = await _http.PostAsync("api/chat", new StringContent(json, Encoding.UTF8, "application/json"), ct);

        var body = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
        {
            _logger.LogWarning("Ollama error {StatusCode}: {Body}", (int)resp.StatusCode, body);
            throw new InvalidOperationException($"Ollama request failed: {(int)resp.StatusCode}");
        }

        var parsed = JsonSerializer.Deserialize<OllamaChatResponse>(body, JsonOptions);
        return parsed?.Message?.Content ?? string.Empty;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private sealed class OllamaChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = "llama3.1";

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }

        [JsonPropertyName("messages")]
        public List<OllamaChatMessage> Messages { get; set; } = [];
    }

    private sealed class OllamaChatMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = "user";

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    private sealed class OllamaChatResponse
    {
        [JsonPropertyName("message")]
        public OllamaChatMessage? Message { get; set; }
    }
}

