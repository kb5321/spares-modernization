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

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var schedule = _db.StaffSchedule.FirstOrDefault(s => s.StaffScheduleId == id);

            if (schedule == null)
            {
                return NotFound();
            }

            _db.StaffSchedule.Remove(schedule);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpDelete("staff/{staffId}")]
        public IActionResult DeleteStaffSchedule(int staffId)
        {
            var staffSchedules = _db.StaffSchedule
                .Where(s => s.StaffId == staffId)
                .ToList();

            if (staffSchedules.Count == 0)
            {
                return NotFound();
            }

            foreach (var schedule in staffSchedules)
            {
                _db.StaffSchedule.Remove(schedule);
            }
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