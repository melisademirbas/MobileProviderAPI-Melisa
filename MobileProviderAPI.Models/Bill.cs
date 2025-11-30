namespace MobileProviderAPI.Models;

public class Bill
{
    public int Id { get; set; }
    public int SubscriberId { get; set; }
    public string SubscriberNo { get; set; } = string.Empty;
    public string Month { get; set; } = string.Empty; // Format: "YYYY-MM"
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public bool IsPaid => PaidAmount >= TotalAmount;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Subscriber Subscriber { get; set; } = null!;
    public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

