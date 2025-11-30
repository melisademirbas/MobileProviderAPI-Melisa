namespace MobileProviderAPI.Models.DTOs;

public class QueryBillDetailedResponse
{
    public decimal BillTotal { get; set; }
    public string Month { get; set; } = string.Empty;
    public List<BillDetailDto> BillDetails { get; set; } = new();
}

public class BillDetailDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
}

