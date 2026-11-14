using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SpaRes.Api.Models;
using SpaRes.Api.Data;
using SpaRes.Api.Dtos;
using SpaRes.Api.Services;


namespace SpaRes.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly SpaResDbContext _db;
        private readonly ReservationAuditService _reservationAuditService;

        public ReservationController(
        SpaResDbContext db,
        ReservationAuditService reservationAuditService)
        {
            _db = db;
            _reservationAuditService = reservationAuditService;
        }


        [HttpGet]
        public ActionResult<List<Reservation>> GetAll()
        {
           // return Ok(reservations);
            return Ok(_db.Reservations.ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<Reservation> GetById(int id)
        {
            var reservation = _db.Reservations
                .FirstOrDefault(r => r.ReservationId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }
        [Authorize]
        [HttpPost]
        public IActionResult CreateReservation(Reservation reservation)
        {
            reservation.CreationDate = DateTime.Now;
            reservation.ReservationStatus = ReservationStatus.Pending;

            if (reservation.Date < DateOnly.FromDateTime(DateTime.Today))
            {
                return BadRequest(
                    "A reservation cannot be created for a past date."
                );
            }

            if (reservation.Date == DateOnly.FromDateTime(DateTime.Today) &&
            reservation.Time < TimeOnly.FromDateTime(DateTime.Now))
            {
                return BadRequest(
                    "A reservation cannot be created for a past time."
                );
            }

            var service = _db.Services
             .FirstOrDefault(s =>
            s.ServiceId == reservation.ServiceId &&
            s.TenantId == reservation.TenantId);

            if (service == null)
            {
                return BadRequest(
                    "The selected service does not exist."
                );
            }

            reservation.Duration = service.ServiceDuration;

            var availableRoom = FindAvailableRoom(
                reservation.LocationId,
                reservation.Date,
                reservation.Time,
                reservation.Duration);


            if (availableRoom == null)
            {
                return BadRequest(
                    "No room is available for the selected reservation time."
                );
            }


            reservation.RoomId = availableRoom.RoomId;


            var validationResult = ValidateReservation(
                0,
                reservation.TenantId,
                reservation.LocationId,
                reservation.Date,
                reservation.Time,
                reservation.Duration,
                reservation.ServiceProviderId,
                reservation.ServiceId,
                reservation.ClientId,
                reservation.RoomId);

            if (validationResult != null)
            {
                return validationResult;
            }
            reservation.BookedById = GetCurrentStaffId();
            _db.Reservations.Add(reservation);
            _db.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = reservation.ReservationId },
                reservation
            );
        }
        [Authorize]
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, UpdateReservationRequest request)
        {
            var reservation = _db.Reservations
                .FirstOrDefault(r => r.ReservationId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            if (reservation.Date < today)
            {
                return BadRequest("Past reservations cannot be modified.");
            }

            if (request.Date.HasValue && request.Date.Value < today)
            {
                return BadRequest("A reservation cannot be moved to a past date.");
            }

            var requestedLocationId =
                request.LocationId ?? reservation.LocationId;

            var requestedDate =
                request.Date ?? reservation.Date;

            var requestedTime =
                request.Time ?? reservation.Time;

            var requestedDuration =
                request.Duration ?? reservation.Duration;

            var requestedServiceProviderId =
                request.ServiceProviderId ?? reservation.ServiceProviderId;

            var requestedRoomId = request.RoomId ?? reservation.RoomId;

            var requestedServiceId = request.ServiceId ?? reservation.ServiceId;

            var requestedClientId = request.ClientId ?? reservation.ClientId;


            if (reservation.Request &&
            request.ServiceProviderId.HasValue &&
            request.ServiceProviderId.Value != reservation.ServiceProviderId)
            {
                return BadRequest(
                    "The service provider cannot be changed for a requested reservation."
                );
            }

            var validationResult = ValidateReservation(
            reservation.ReservationId,
            reservation.TenantId,
            requestedLocationId,
            requestedDate,
            requestedTime,
            requestedDuration,
            requestedServiceProviderId,
            requestedServiceId,
            requestedClientId,
            requestedRoomId);

            if (validationResult != null)
            {
                return validationResult;
            }

            


            if (request.LocationId.HasValue)
            {
                var oldLocationId = reservation.LocationId;
                var newLocationId = request.LocationId.Value;

                if (oldLocationId != newLocationId)
                {
                    reservation.LocationId = newLocationId;

                    AddReservationLog(
                        reservation,
                        "Location changed",
                        oldLocationId.ToString(),
                        newLocationId.ToString()
                    );
                }
            }

            if (request.Date.HasValue)
            {
                var oldDate = reservation.Date;
                var newDate = request.Date.Value;

                if (oldDate != newDate)
                {
                    reservation.Date = newDate;

                    AddReservationLog(
                        reservation,
                        "Reservation date changed",
                        oldDate.ToString(),
                        newDate.ToString()
                    );
                }
            }

            if (request.Time.HasValue)
            {
                var oldTime = reservation.Time;
                var newTime = request.Time.Value;

                if (oldTime != newTime)
                {
                    reservation.Time = newTime;

                    AddReservationLog(
                        reservation,
                        "Reservation time changed",
                        oldTime.ToString(),
                        newTime.ToString()
                    );
                }
            }

            if (request.Duration.HasValue)
            {
                var oldDuration = reservation.Duration;
                var newDuration = request.Duration.Value;

                if (oldDuration != newDuration)
                {
                    reservation.Duration = newDuration;

                    AddReservationLog(
                        reservation,
                        "Duration changed",
                        oldDuration.ToString(),
                        newDuration.ToString()
                    );
                }
            }


            if (request.ServiceId.HasValue)
            {
                var oldServiceId = reservation.ServiceId;
                var newServiceId = request.ServiceId.Value;

                if (oldServiceId != newServiceId)
                {
                    reservation.ServiceId = newServiceId;

                    AddReservationLog(
                        reservation,
                        "Service changed",
                        oldServiceId.ToString(),
                        newServiceId.ToString()
                    );
                }
            }

            if (request.ReservationStatus.HasValue)
            {
                var oldReservationStatus = reservation.ReservationStatus;
                var newReservationStatus = request.ReservationStatus.Value;

                if (oldReservationStatus != newReservationStatus)
                {
                    reservation.ReservationStatus = newReservationStatus;

                    AddReservationLog(
                        reservation,
                        "Reservation status changed",
                        oldReservationStatus.ToString(),
                        newReservationStatus.ToString()
                    );
                }
            }

            if (request.Note != null)
            {
                var oldNote = reservation.Note;
                var newNote = request.Note;

                if (oldNote != newNote)
                {
                    reservation.Note = newNote;

                    AddReservationLog(
                        reservation,
                        "Reservation note changed",
                        oldNote,
                        newNote
                    );
                }
            }


            if (request.ServiceProviderId.HasValue)
            {
                var oldServiceProviderId = reservation.ServiceProviderId;
                var newServiceProviderId = request.ServiceProviderId.Value;

                if (oldServiceProviderId != newServiceProviderId)
                {
                    reservation.ServiceProviderId = newServiceProviderId;

                    AddReservationLog(
                        reservation,
                        "Service provider changed",
                        oldServiceProviderId.ToString(),
                        newServiceProviderId.ToString()
                    );
                }
            }

            if (request.ClientId.HasValue)
            {
                var oldClientId = reservation.ClientId;
                var newClientId = request.ClientId.Value;

                if (oldClientId != newClientId)
                {
                    reservation.ClientId = newClientId;

                    AddReservationLog(
                        reservation,
                        "Client changed",
                        oldClientId.ToString(),
                        newClientId.ToString()
                    );
                }
            }

            if (request.BookedById.HasValue)
            {
                var oldBookedById = reservation.BookedById;
                var newBookedById = request.BookedById.Value;

                if (oldBookedById != newBookedById)
                {
                    reservation.BookedById = newBookedById;

                    AddReservationLog(
                        reservation,
                        "Booked by changed",
                        oldBookedById.ToString(),
                        newBookedById.ToString()
                    );
                }
            }

            if (request.RoomId.HasValue)
            {
                var oldRoomId = reservation.RoomId;
                var newRoomId = request.RoomId.Value;

                if (oldRoomId != newRoomId)
                {
                    reservation.RoomId = newRoomId;

                    AddReservationLog(
                        reservation,
                        "Room changed",
                        oldRoomId.ToString(),
                        newRoomId.ToString()
                    );
                }
            }

            if (request.Request.HasValue)
            {
                var oldRequest = reservation.Request;
                var newRequest = request.Request.Value;

                if (oldRequest != newRequest)
                {
                    reservation.Request = newRequest;

                    AddReservationLog(
                        reservation,
                        "Request status changed",
                        oldRequest.ToString(),
                        newRequest.ToString()
                    );
                }
            }

            if (request.PartOfPackage.HasValue)
            {
                var oldPartOfPackage = reservation.PartOfPackage;
                var newPartOfPackage = request.PartOfPackage.Value;

                if (oldPartOfPackage != newPartOfPackage)
                {
                    reservation.PartOfPackage = newPartOfPackage;

                    AddReservationLog(
                        reservation,
                        "Package status changed",
                        oldPartOfPackage.ToString(),
                        newPartOfPackage.ToString()
                    );
                }
            }

            if (request.IsRepeatClient.HasValue)
            {
                var oldIsRepeatClient = reservation.IsRepeatClient;
                var newIsRepeatClient = request.IsRepeatClient.Value;

                if (oldIsRepeatClient != newIsRepeatClient)
                {
                    reservation.IsRepeatClient = newIsRepeatClient;

                    AddReservationLog(
                        reservation,
                        "Repeat client status changed",
                        oldIsRepeatClient.ToString(),
                        newIsRepeatClient.ToString()
                    );
                }
            }
            _db.SaveChanges();
            return NoContent();
        }

        [HttpGet("{id}/logs")]
        public ActionResult<List<ResLog>> GetLogs(int id)
        {
            
            var logs = _db.ResLogs
            .Where(l => l.ReservationId == id)
            .OrderByDescending(l => l.LogTime)
            .ToList();


            return Ok(logs);
        }

        private IActionResult? ValidateReservation(
            int reservationId,
            int tenantId,
            int locationId,
            DateOnly date,
            TimeOnly time,
            int duration,
            int serviceProviderId,
            int serviceId,
            int clientId,
            int roomId)
        {

            var requestedClient = _db.Clients
            .FirstOrDefault(c =>
                c.ClientId == clientId &&
                c.TenantId == tenantId);

                    if (requestedClient == null)
                    {
                        return BadRequest(
                            "The selected client does not exist."
                        );
                    }

            var requestedService = _db.Services
            .FirstOrDefault(s =>
                s.ServiceId == serviceId &&
                s.TenantId == tenantId);




            if (requestedService == null)
            {
                return BadRequest(
                    "The selected service does not exist."
                );
            }

            if (!requestedService.Availability)
            {
                return BadRequest(
                    "The selected service is not active."
                );
            }

            var locationService = _db.LocationServices
            .FirstOrDefault(ls =>
                ls.LocationId == locationId &&
                ls.ServiceId == serviceId);

            if (locationService == null || !locationService.Available)
            {
                return BadRequest(
                    "The selected service is not offered at the selected location."
                );
            }

            var staffCanPerformService = _db.StaffServices
            .Any(ss =>
                ss.StaffId == serviceProviderId &&
                ss.ServiceId == serviceId);

            if (!staffCanPerformService)
            {
                return BadRequest(
                    "The selected service provider is not qualified to perform the selected service."
                );
            }

            var endTime =
                 time.AddMinutes(duration);

            var matchingSchedule = _db.StaffSchedule
                .FirstOrDefault(s =>
                    s.StaffId == serviceProviderId &&
                    s.LocationId == locationId &&
                    s.Date == date &&
                    time >= s.StartTime &&
                    endTime <= s.EndTime);

            if (matchingSchedule == null)
            {
                return BadRequest(
                    "The selected service provider is not scheduled for that reservation time."
                );
            }

            var matchingBreak = _db.StaffBreak
            .FirstOrDefault(b =>
            b.StaffId == serviceProviderId &&
            b.LocationId == locationId &&
            b.Date == date &&
            time < b.EndTime &&
            endTime > b.StartTime);

            if (matchingBreak != null)
            {
                return BadRequest(
                    "The selected service provider has a break during that reservation time."
                );
            }
            var matchingAbsence = _db.StaffAbsence
            .FirstOrDefault(a =>
            a.StaffId == serviceProviderId &&
            date >= a.StartDate &&
            date <= a.EndDate);

            if (matchingAbsence != null)
            {
                return BadRequest(
                    "The selected service provider is absent on that reservation date."
                );
            }
            var requestedRoom = _db.Room
            .FirstOrDefault(r =>
            r.RoomId == roomId &&
            r.LocationId == locationId);

            if (requestedRoom == null)
            {
                return BadRequest(
                    "The selected room does not exist."
                );
            }

            if (requestedRoom.OutOfService)
            {
                return BadRequest(
                    "The selected room is out of service."
                );
            }

            var overlappingRoomReservations = _db.Reservations
            .Where(r =>
                r.ReservationId != reservationId &&
                r.RoomId == roomId &&
                r.Date == date &&
                r.ReservationStatus != ReservationStatus.Cancelled &&
                r.ReservationStatus != ReservationStatus.Void)
            .ToList()
            .Count(r =>
                time < r.Time.AddMinutes(r.Duration) &&
                endTime > r.Time);

                    if (overlappingRoomReservations >= requestedRoom.Capacity)
                    {
                        return BadRequest(
                            "The selected room has reached its capacity during that time."
                        );
                    }

            var conflictingReservation = _db.Reservations
                .Where(r =>
                    r.ReservationId != reservationId &&
                    r.ServiceProviderId == serviceProviderId &&
                    r.Date == date &&
                    r.ReservationStatus != ReservationStatus.Cancelled &&
                    r.ReservationStatus != ReservationStatus.Void)
                .ToList()
                .FirstOrDefault(r =>
                    time < r.Time.AddMinutes(r.Duration) &&
                    endTime > r.Time);

            if (conflictingReservation != null)
            {
                return BadRequest(
                    "The selected service provider already has a reservation during that time."
                );
            }

            return null;
        }

        private Room? FindAvailableRoom(
        int locationId,
        DateOnly date,
        TimeOnly time,
        int duration,
        int reservationId = 0)
        {
            var endTime = time.AddMinutes(duration);

            var rooms = _db.Room
                .Where(r =>
                    r.LocationId == locationId &&
                    !r.OutOfService)
                .ToList();

            var reservations = _db.Reservations
                .Where(r =>
                    r.ReservationId != reservationId &&
                    r.LocationId == locationId &&
                    r.Date == date &&
                    r.ReservationStatus != ReservationStatus.Cancelled &&
                    r.ReservationStatus != ReservationStatus.Void)
                .ToList();

            return rooms.FirstOrDefault(room =>
            {
                var roomOccupancy = reservations.Count(r =>
                    r.RoomId == room.RoomId &&
                    time < r.Time.AddMinutes(r.Duration) &&
                    endTime > r.Time);

                return roomOccupancy < room.Capacity;
            });
        }

       
        private void AddReservationLog(
            Reservation reservation,
            string logInfo,
            string valueBefore,
            string valueAfter)
        {
            _reservationAuditService.AddLog(
                reservation,
                GetCurrentStaffId(),
                logInfo,
                valueBefore,
                valueAfter
            );
        }

        [HttpGet("upcoming")]
        public IActionResult GetUpcomingReservations()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var now = TimeOnly.FromDateTime(DateTime.Now);

            var reservations =
                from reservation in _db.Reservations

                join client in _db.Clients
                    on reservation.ClientId equals client.ClientId

                join service in _db.Services
                    on reservation.ServiceId equals service.ServiceId

                join staff in _db.Staff
                    on reservation.ServiceProviderId equals staff.StaffId

                join location in _db.Locations
                    on reservation.LocationId equals location.LocationId

                join room in _db.Room
                    on reservation.RoomId equals room.RoomId

                where
                    reservation.ReservationStatus != ReservationStatus.Cancelled &&
                    reservation.ReservationStatus != ReservationStatus.Void &&
                    (
                        reservation.Date > today ||
                        (reservation.Date == today &&
                         reservation.Time >= now)
                    )

                orderby reservation.Date, reservation.Time

                select new UpcomingReservationDto
                {

                    ServiceId = reservation.ServiceId,

                    ServiceProviderId = reservation.ServiceProviderId,

                    LocationId = reservation.LocationId,

                    ReservationId = reservation.ReservationId,

                    ClientId = reservation.ClientId,

                    Date = reservation.Date,

                    Time = reservation.Time,

                    Duration = reservation.Duration,

                    ClientName =
                        client.FirstName + " " + client.LastName,

                    ServiceName = service.Description,

                    ServiceProviderName =
                        staff.FirstName + " " + staff.LastName,

                    LocationName = location.Name,

                    RoomName = room.RoomName,
                    RoomId = reservation.RoomId,

                    Status = reservation.ReservationStatus.ToString(),

                    Note = reservation.Note,

                    Request = reservation.Request,

                    PartOfPackage = reservation.PartOfPackage
    };

            return Ok(reservations.ToList());
        }
        [Authorize]
        [HttpGet("inactive")]
        public IActionResult GetInactiveReservations()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var reservations =
                from reservation in _db.Reservations

                join client in _db.Clients
                    on reservation.ClientId equals client.ClientId

                join service in _db.Services
                    on reservation.ServiceId equals service.ServiceId

                join staff in _db.Staff
                    on reservation.ServiceProviderId equals staff.StaffId

                join location in _db.Locations
                    on reservation.LocationId equals location.LocationId

                join room in _db.Room
                    on reservation.RoomId equals room.RoomId

                where
                    (
                        reservation.ReservationStatus == ReservationStatus.Cancelled ||
                        reservation.ReservationStatus == ReservationStatus.Void
                    ) &&
                    reservation.Date >= today

                orderby reservation.Date, reservation.Time

                select new UpcomingReservationDto
                {
                    ServiceId = reservation.ServiceId,

                    ServiceProviderId = reservation.ServiceProviderId,

                    LocationId = reservation.LocationId,

                    ReservationId = reservation.ReservationId,

                    Date = reservation.Date,

                    Time = reservation.Time,

                    Duration = reservation.Duration,

                    ClientName =
                        client.FirstName + " " + client.LastName,

                    ServiceName = service.Description,

                    ServiceProviderName =
                        staff.FirstName + " " + staff.LastName,

                    LocationName = location.Name,

                    RoomName = room.RoomName,

                    Status = reservation.ReservationStatus.ToString()
                };

            return Ok(reservations.ToList());
        }


        [Authorize]
        [HttpPatch("{id}/reschedule")]
        public IActionResult RescheduleReservation(int id, RescheduleReservationDto request)
        {
            var reservation = _db.Reservations
                .FirstOrDefault(r => r.ReservationId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            if (request.Date < DateOnly.FromDateTime(DateTime.Today))
            {
                return BadRequest(
                    "A reservation cannot be rescheduled to a past date."
                );
            }

            if (
                request.Date == DateOnly.FromDateTime(DateTime.Today) &&
                request.Time < TimeOnly.FromDateTime(DateTime.Now)
            )
            {
                return BadRequest(
                    "A reservation cannot be rescheduled to a past time."
                );
            }

            var availableRoom = FindAvailableRoom(reservation.LocationId,request.Date,request.Time,reservation.Duration,reservation.ReservationId);

            if (availableRoom == null)
            {
                return BadRequest(
                    "No room is available for the selected reservation time."
                );
            }

            var validationResult = ValidateReservation(
                reservation.ReservationId,
                reservation.TenantId,
                reservation.LocationId,
                request.Date,
                request.Time,
                reservation.Duration,
                reservation.ServiceProviderId,
                reservation.ServiceId,
                reservation.ClientId,
                availableRoom.RoomId
            );

            if (validationResult != null)
            {
                return validationResult;
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            if (reservation.Date < today)
            {
                return BadRequest(
                    "Past reservations cannot be rescheduled."
                );
            }

            if (request.Date < today)
            {
                return BadRequest(
                    "A reservation cannot be rescheduled to a past date."
                );
            }

            if (request.Date == today && request.Time < TimeOnly.FromDateTime(DateTime.Now) )
            {
                return BadRequest(
                    "A reservation cannot be rescheduled to a past time."
                );
            }

            var oldDate = reservation.Date;
            var oldTime = reservation.Time;
            var oldRoomId = reservation.RoomId;

            reservation.Date = request.Date;
            reservation.Time = request.Time;
            reservation.RoomId = availableRoom.RoomId;

            if (oldDate != reservation.Date)
            {
                AddReservationLog(
                    reservation,
                    "Reservation date changed",
                    oldDate.ToString(),
                    reservation.Date.ToString()
                );
            }

            if (oldTime != reservation.Time)
            {
                AddReservationLog(
                    reservation,
                    "Reservation time changed",
                    oldTime.ToString(),
                    reservation.Time.ToString()
                );
            }

            if (oldRoomId != reservation.RoomId)
            {
                AddReservationLog(
                    reservation,
                    "Room changed",
                    oldRoomId.ToString(),
                    reservation.RoomId.ToString()
                );
            }

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