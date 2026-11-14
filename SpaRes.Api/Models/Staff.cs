namespace SpaRes.Api.Models
{
    public class Staff
    {
        public int StaffId { get; set; }
        public int TenantId { get; set; }
        public int HomeLocationId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateOnly? BirthDate { get; set; }

        public DateOnly? HireDate { get; set; }

        public string Note { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}