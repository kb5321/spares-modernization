namespace SpaRes.Api.Models
{
    public class StaffBreak
    {
        public int StaffBreakId { get; set; }

        public int StaffId { get; set; }

        public int LocationId { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string Note { get; set; } = string.Empty;
    }
}