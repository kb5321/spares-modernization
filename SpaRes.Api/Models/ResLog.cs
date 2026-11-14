namespace SpaRes.Api.Models
{
    using System.ComponentModel.DataAnnotations;
    public class ResLog
    {
        [Key]
        public int LogId { get; set; }
        public int TenantId { get; set; }

        public int ReservationId { get; set; }

        public int UpdatedByStaffId { get; set; }

        public DateTime LogTime { get; set; }

        public string LogInfo { get; set; } = string.Empty;

        public int LogFieldNumber { get; set; }

        public string LogValueBefore { get; set; } = string.Empty;

        public string LogValueAfter { get; set; } = string.Empty;
    }
}