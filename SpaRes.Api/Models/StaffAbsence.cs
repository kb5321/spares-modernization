namespace SpaRes.Api.Models
{
    public class StaffAbsence
    {
        public int StaffAbsenceId { get; set; }

        public int StaffId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string Note { get; set; } = string.Empty;
    }
}