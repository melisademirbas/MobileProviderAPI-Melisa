namespace MobileProviderAPI.Models.DTOs;

public class QueryBillResponse
{
    public decimal BillTotal { get; set; }
    public bool PaidStatus { get; set; }
    public string Month { get; set; } = string.Empty;
}

