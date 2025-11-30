namespace MobileProviderAPI.Models;

public class ApiCallLog
{
    public int Id { get; set; }
    public int? SubscriberId { get; set; }
    public string? SubscriberNo { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public DateTime CallDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Subscriber? Subscriber { get; set; }
}

