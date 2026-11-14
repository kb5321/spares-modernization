namespace SpaRes.Api.Models
{
    public class AppUser
    {
        public int AppUserId { get; set; }

        public int TenantId { get; set; }

        public int StaffId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}