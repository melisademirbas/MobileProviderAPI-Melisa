namespace MobileProviderAPI.Models;

public class BillDetail
{
    public int Id { get; set; }
    public int BillId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty; // e.g., "Voice", "Data", "SMS", "Roaming"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Bill Bill { get; set; } = null!;
}

