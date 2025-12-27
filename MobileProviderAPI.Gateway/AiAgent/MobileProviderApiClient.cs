using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MobileProviderAPI.Models.DTOs;

namespace MobileProviderAPI.Gateway.AiAgent;

public sealed class MobileProviderApiClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public MobileProviderApiClient(HttpClient http, IConfiguration config, string gatewayBaseUrl)
    {
        _http = http;
        _config = config;
        _http.BaseAddress = new Uri(gatewayBaseUrl.TrimEnd('/') + "/");
    }

    public async Task<string> LoginAndGetJwtAsync(CancellationToken ct)
    {
        var username = _config["AiAgent:AuthUsername"] ?? "mobileapp";
        var password = _config["AiAgent:AuthPassword"] ?? "mobile123";

        var payload = JsonSerializer.Serialize(new LoginRequest { Username = username, Password = password }, JsonOptions);
        using var resp = await _http.PostAsync("api/authentication/login", new StringContent(payload, Encoding.UTF8, "application/json"), ct);
        var body = await resp.Content.ReadAsStringAsync(ct);
        resp.EnsureSuccessStatusCode();

        var parsed = JsonSerializer.Deserialize<LoginResponse>(body, JsonOptions);
        if (parsed?.Token is null)
            throw new InvalidOperationException("Login response did not contain a token.");

        return parsed.Token;
    }

    public void SetBearer(string jwt)
    {
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
    }

    public async Task<QueryBillResponse?> QueryBillAsync(string subscriberNo, string month, CancellationToken ct)
    {
        using var resp = await _http.GetAsync($"api/mobileapp/querybill?subscriberNo={Uri.EscapeDataString(subscriberNo)}&month={Uri.EscapeDataString(month)}", ct);
        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        var body = await resp.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<QueryBillResponse>(body, JsonOptions);
    }

    public async Task<QueryBillDetailedResponse?> QueryBillDetailedAsync(string subscriberNo, string month, int pageNumber, int pageSize, CancellationToken ct)
    {
        using var resp = await _http.GetAsync(
            $"api/mobileapp/querybilldetailed?subscriberNo={Uri.EscapeDataString(subscriberNo)}&month={Uri.EscapeDataString(month)}&pageNumber={pageNumber}&pageSize={pageSize}",
            ct);
        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        var body = await resp.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<QueryBillDetailedResponse>(body, JsonOptions);
    }

    public async Task<PayBillResponse> PayBillAsync(string subscriberNo, string month, decimal? amount, CancellationToken ct)
    {
        var payload = JsonSerializer.Serialize(new PayBillRequest { SubscriberNo = subscriberNo, Month = month, Amount = amount }, JsonOptions);
        using var resp = await _http.PostAsync("api/website/paybill", new StringContent(payload, Encoding.UTF8, "application/json"), ct);
        var body = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
        {
            var parsedErr = JsonSerializer.Deserialize<PayBillResponse>(body, JsonOptions);
            return parsedErr ?? new PayBillResponse { PaymentStatus = "Error", ErrorMessage = "PayBill failed" };
        }

        var parsed = JsonSerializer.Deserialize<PayBillResponse>(body, JsonOptions);
        return parsed ?? new PayBillResponse { PaymentStatus = "Error", ErrorMessage = "Invalid PayBill response" };
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}

