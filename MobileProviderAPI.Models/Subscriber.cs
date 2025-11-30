namespace MobileProviderAPI.Models;

public class Subscriber
{
    public int Id { get; set; }
    public string SubscriberNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<ApiCallLog> ApiCallLogs { get; set; } = new List<ApiCallLog>();
}

