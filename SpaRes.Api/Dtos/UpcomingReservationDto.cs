namespace SpaRes.Api.Dtos
{
    public class UpcomingReservationDto
    {
        public int ServiceId { get; set; }
        public int ServiceProviderId { get; set; }
        public int LocationId { get; set; }
        public int ReservationId { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }

        public int Duration { get; set; }

        public string ClientName { get; set; } = string.Empty;

        public int ClientId { get; set; }

        public string ServiceName { get; set; } = string.Empty;

        public string ServiceProviderName { get; set; } = string.Empty;

        public string LocationName { get; set; } = string.Empty;

        public string RoomName { get; set; } = string.Empty;
        public int RoomId { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? Note { get; set; }

        public bool Request { get; set; }
        public bool PartOfPackage { get; set; }
    }
}