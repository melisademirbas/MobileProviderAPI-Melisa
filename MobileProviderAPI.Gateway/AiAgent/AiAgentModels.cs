using System.Text.Json.Serialization;

namespace MobileProviderAPI.Gateway.AiAgent;

public sealed class AiAgentConversationState
{
    public string? PendingIntent { get; set; }
    public string? PendingSubscriberNo { get; set; }
    public string? PendingMonth { get; set; }
    public decimal? PendingAmount { get; set; }
    public int PendingPageNumber { get; set; } = 1;
    public int PendingPageSize { get; set; } = 10;

    // Token cache for this connection
    public string? JwtToken { get; set; }
}

public sealed class AiAgentWsClientMessage
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

public sealed class AiAgentWsServerMessage
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "assistant";

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public object? Data { get; set; }
}

public sealed class AiAgentParsedIntent
{
    [JsonPropertyName("intent")]
    public string? Intent { get; set; }

    [JsonPropertyName("subscriberNo")]
    public string? SubscriberNo { get; set; }

    [JsonPropertyName("month")]
    public string? Month { get; set; } // YYYY-MM

    [JsonPropertyName("amount")]
    public decimal? Amount { get; set; }

    [JsonPropertyName("pageNumber")]
    public int? PageNumber { get; set; }

    [JsonPropertyName("pageSize")]
    public int? PageSize { get; set; }

    [JsonPropertyName("clarificationQuestion")]
    public string? ClarificationQuestion { get; set; }
}

