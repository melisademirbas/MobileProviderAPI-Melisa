namespace MobileProviderAPI.Models.DTOs;

public class TransactionResponse
{
    public string Status { get; set; } = "Success"; // "Success" or "Error"
    public string? ErrorMessage { get; set; }
    public int? RecordsProcessed { get; set; }
}

