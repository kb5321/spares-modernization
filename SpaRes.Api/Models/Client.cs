namespace SpaRes.Api.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public int TenantId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public DateOnly? Birthdate { get; set; }

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;

        public DateTime RegistrationDate { get; set; }

        public int CreatedByStaffId { get; set; }
    }
}