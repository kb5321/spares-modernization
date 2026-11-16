using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaRes.Api.Data;
using SpaRes.Api.Models;

namespace SpaRes.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffScheduleController : ControllerBase
    {
        private readonly SpaResDbContext _db;

        public StaffScheduleController(SpaResDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<List<StaffSchedule>> GetAll()
        {
            var schedules = _db.StaffSchedule.ToList();
            return Ok(schedules);
        }

        [HttpGet("{id}")]
        public ActionResult<StaffSchedule> GetById(int id)
        {
            var schedule = _db.StaffSchedule.FirstOrDefault(s => s.StaffScheduleId == id);

            if (schedule == null)
            {
                return NotFound();
            }

            return Ok(schedule);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var schedule = _db.StaffSchedule.FirstOrDefault(s => s.StaffScheduleId == id);

            if (schedule == null)
            {
                return NotFound();
            }

            var hasActiveReservations = _db.Reservations.Any(r =>
                r.ServiceProviderId == schedule.StaffId &&
                r.LocationId == schedule.LocationId &&
                r.Date == schedule.Date &&
                (r.ReservationStatus == ReservationStatus.Pending ||
                 r.ReservationStatus == ReservationStatus.Confirmed));

            if (hasActiveReservations)
            {
                return Conflict(
                    "Cannot delete the schedule because Pending or Confirmed reservations exist for this date."
                );
            }

            _db.StaffSchedule.Remove(schedule);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPost]
        public ActionResult<StaffSchedule> Create(StaffSchedule schedule)
        {
            _db.StaffSchedule.Add(schedule);
            _db.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = schedule.StaffScheduleId },
                schedule
            );
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, StaffSchedule updatedSchedule)
        {
            var schedule = _db.StaffSchedule
                .FirstOrDefault(s => s.StaffScheduleId == id);

            if (schedule == null)
            {
                return NotFound();
            }

            schedule.StaffId = updatedSchedule.StaffId;
            schedule.LocationId = updatedSchedule.LocationId;
            schedule.Date = updatedSchedule.Date;
            schedule.StartTime = updatedSchedule.StartTime;
            schedule.EndTime = updatedSchedule.EndTime;

            _db.SaveChanges();

            return NoContent();
        }

        




    }



}