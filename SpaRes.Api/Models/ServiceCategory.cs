namespace SpaRes.Api.Models
{
    public class ServiceCategory
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
