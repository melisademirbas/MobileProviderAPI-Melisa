namespace MobileProviderAPI.Models.DTOs;

public class AddBillRequest
{
    public string SubscriberNo { get; set; } = string.Empty;
    public string Month { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<BillDetailRequest>? BillDetails { get; set; }
}

public class BillDetailRequest
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
}

