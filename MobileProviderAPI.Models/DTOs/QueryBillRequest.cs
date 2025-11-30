namespace MobileProviderAPI.Models.DTOs;

public class QueryBillRequest
{
    public string SubscriberNo { get; set; } = string.Empty;
    public string? Month { get; set; } // Optional for banking app
}

