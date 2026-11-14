namespace SpaRes.Api.Models
{
    public class LocationLicense
    {
        public int LocationLicenseId { get; set; }

        public int LocationId { get; set; }

        public string LicenseType { get; set; } = string.Empty;

        public string LicenseNumber { get; set; } = string.Empty;

        public string IssuingAuthority { get; set; } = string.Empty;

        public DateOnly? IssueDate { get; set; }

        public DateOnly? ExpirationDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}