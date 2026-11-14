public class Service
{
    public int ServiceId { get; set; }
    public int TenantId { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public bool Availability { get; set; }

    public int CatId { get; set; }

    public string ShortName { get; set; } = string.Empty;

    public int ServiceDuration { get; set; }
}