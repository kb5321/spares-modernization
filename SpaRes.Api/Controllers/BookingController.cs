using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaRes.Api.Data;
using SpaRes.Api.Dtos;
using SpaRes.Api.Models;
using SpaRes.Api.Services;


namespace SpaRes.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly SpaResDbContext _db;
        private readonly ReservationAuditService _reservationAuditService;

        public BookingController(
         SpaResDbContext db,
         ReservationAuditService reservationAuditService)
        {
            _db = db;
            _reservationAuditService = reservationAuditService;
        }
        
        [HttpGet("available-times")]
        public IActionResult GetAvailableTimes(
            int staffId,
            int locationId,
            int serviceId,
            DateOnly date)
        {
            // Past dates cannot have bookable times.
            if (date < DateOnly.FromDateTime(DateTime.Today))
            {
                return Ok(new List<TimeOnly>());
            }

            // Get the selected service and its duration.
            var service = _db.Services
                .FirstOrDefault(s =>
                    s.ServiceId == serviceId &&
                    s.Availability);

            if (service == null)
            {
                return BadRequest(
                    "The selected service does not exist or is unavailable."
                );
            }

            // The provider must have a schedule at this location on this date.
            var schedule = _db.StaffSchedule
                .FirstOrDefault(s =>
                    s.StaffId == staffId &&
                    s.LocationId == locationId &&
                    s.Date == date);

            if (schedule == null)
            {
                return Ok(new List<TimeOnly>());
            }

            // An absence removes the provider for the entire date.
            var isAbsent = _db.StaffAbsence
                .Any(a =>
                    a.StaffId == staffId &&
                    date >= a.StartDate &&
                    date <= a.EndDate);

            if (isAbsent)
            {
                return Ok(new List<TimeOnly>());
            }

            // Provider breaks for this location and date.
            var breaks = _db.StaffBreak
                .Where(b =>
                    b.StaffId == staffId &&
                    b.LocationId == locationId &&
                    b.Date == date)
                .ToList();

            // Reservations that still occupy scheduling resources.
            // Cancelled and Void reservations do not block availability.
            var reservations = _db.Reservations
                .Where(r =>
                    r.LocationId == locationId &&
                    r.Date == date &&
                    r.ReservationStatus != ReservationStatus.Cancelled &&
                    r.ReservationStatus != ReservationStatus.Void)
                .ToList();

            // Usable rooms at the selected location.
            var rooms = _db.Room
                .Where(r =>
                    r.LocationId == locationId &&
                    !r.OutOfService)
                .ToList();

            // No usable room means there are no bookable times.
            if (rooms.Count == 0)
            {
                return Ok(new List<TimeOnly>());
            }

            var availableTimes = new List<TimeOnly>();

            var currentTime = schedule.StartTime;
            var serviceDuration = service.ServiceDuration;

            while (currentTime.AddMinutes(serviceDuration) <= schedule.EndTime)
            {
                var appointmentEnd =
                    currentTime.AddMinutes(serviceDuration);

                // Do not offer times that have already passed today.
                if (
                    date == DateOnly.FromDateTime(DateTime.Today) &&
                    currentTime < TimeOnly.FromDateTime(DateTime.Now)
                )
                {
                    currentTime = currentTime.AddMinutes(15);
                    continue;
                }

                // Is the provider on break during this candidate appointment?
                var overlapsBreak = breaks.Any(b =>
                    currentTime < b.EndTime &&
                    appointmentEnd > b.StartTime);

                // Is the provider already performing another appointment?
                var overlapsProviderReservation = reservations.Any(r =>
                {
                    if (r.ServiceProviderId != staffId)
                    {
                        return false;
                    }

                    var reservationEnd =
                        r.Time.AddMinutes(r.Duration);

                    return currentTime < reservationEnd &&
                           appointmentEnd > r.Time;
                });

                // Is there at least one usable room with remaining capacity?
                var roomAvailable = rooms.Any(room =>
                {
                    var roomOccupancy = reservations.Count(r =>
                    {
                        if (r.RoomId != room.RoomId)
                        {
                            return false;
                        }

                        var reservationEnd =
                            r.Time.AddMinutes(r.Duration);

                        return currentTime < reservationEnd &&
                               appointmentEnd > r.Time;
                    });

                    return roomOccupancy < room.Capacity;
                });

                // A time is truly bookable only when BOTH
                // the provider and a room are available.
                if (!overlapsBreak &&
                    !overlapsProviderReservation &&
                    roomAvailable)
                {
                    availableTimes.Add(currentTime);
                }

                // SpaRes booking grid uses 15-minute increments.
                currentTime = currentTime.AddMinutes(15);
            }

            return Ok(availableTimes);
        }
        [Authorize]
        [HttpPatch("{id}/status")]
        public IActionResult UpdateReservationStatus(int id, ReservationStatus status)
        {
            var reservation = _db.Reservations
                .FirstOrDefault(r => r.ReservationId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            var oldStatus = reservation.ReservationStatus;

            reservation.ReservationStatus = status;

            _reservationAuditService.AddLog(
                reservation,
                GetCurrentStaffId(),
                "Reservation status changed",
                oldStatus.ToString(),
                status.ToString()
            );

            _db.SaveChanges();

            return Ok(reservation);
        }

        private int GetCurrentStaffId()
        {
            var staffIdClaim = User.FindFirst("StaffId")?.Value;

            if (!int.TryParse(staffIdClaim, out var staffId))
            {
                throw new UnauthorizedAccessException(
                    "Authenticated StaffId is missing."
                );
            }

            return staffId;
        }




    }
}
