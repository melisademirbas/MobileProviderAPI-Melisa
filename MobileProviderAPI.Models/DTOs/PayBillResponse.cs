namespace MobileProviderAPI.Models.DTOs;

public class PayBillResponse
{
    public string PaymentStatus { get; set; } = "Successful"; // "Successful" or "Error"
    public string? ErrorMessage { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
}

