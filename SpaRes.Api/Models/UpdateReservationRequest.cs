namespace SpaRes.Api.Models
{
    public class UpdateReservationRequest
    {
        public int? LocationId { get; set; }
        public DateOnly? Date { get; set; }
        public TimeOnly? Time { get; set; }
        public int? Duration { get; set; }
        public int? ServiceProviderId { get; set; }
        public ReservationStatus? ReservationStatus { get; set; }
        public string? Note { get; set; }
        public int? ServiceId { get; set; }
        public int? ClientId { get; set; }
        public int? BookedById { get; set; }
        public int? RoomId { get; set; }
        public bool? Request { get; set; }
        public bool? PartOfPackage { get; set; }
        public bool? IsRepeatClient { get; set; }
    }
}