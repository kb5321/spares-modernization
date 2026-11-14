namespace SpaRes.Api.Models
{
    public class UpdateServiceRequest
    {
        public string? Description { get; set; }
        public decimal? Amount { get; set; }
        public bool? Availability { get; set; }
        public int? CatId { get; set; }
        public string? ShortName { get; set; }
        public int? ServiceDuration { get; set; }
    }
}