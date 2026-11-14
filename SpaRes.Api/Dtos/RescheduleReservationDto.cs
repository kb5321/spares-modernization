namespace SpaRes.Api.Dtos
{
    public class RescheduleReservationDto
    {
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
    }
}