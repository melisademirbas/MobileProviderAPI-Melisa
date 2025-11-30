namespace MobileProviderAPI.Models;

public class Payment
{
    public int Id { get; set; }
    public int BillId { get; set; }
    public int SubscriberId { get; set; }
    public string SubscriberNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Successful"; // "Successful" or "Error"
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Bill Bill { get; set; } = null!;
    public virtual Subscriber Subscriber { get; set; } = null!;
}

