namespace MobileProviderAPI.Models.DTOs;

public class PayBillRequest
{
    public string SubscriberNo { get; set; } = string.Empty;
    public string Month { get; set; } = string.Empty;
    public decimal? Amount { get; set; } // Optional, if not provided, pays full amount
}

