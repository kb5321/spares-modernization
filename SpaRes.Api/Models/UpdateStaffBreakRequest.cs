namespace SpaRes.Api.Models
{
    public class UpdateStaffBreakRequest
    {
        public int? StaffId { get; set; }
        public int? LocationId { get; set; }
        public DateOnly? Date { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public string? Note { get; set; }
    }
}