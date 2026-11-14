using Microsoft.AspNetCore.Mvc;
using SpaRes.Api.Data;
using SpaRes.Api.Models;
namespace SpaRes.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffBreakController : ControllerBase
    {
        private readonly SpaResDbContext _db;

        public StaffBreakController(SpaResDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<List<StaffBreak>> GetAll()
        {
            var staffBreaks = _db.StaffBreak.ToList();
            return Ok(staffBreaks);
            
        }

        [HttpGet("{id}")]
        public ActionResult<StaffBreak> GetById(int id)
        {
            var staffBreak = _db.StaffBreak
                .FirstOrDefault(b => b.StaffBreakId == id);

            if (staffBreak == null)
            {
                return NotFound();
            }

            return Ok(staffBreak);
        }

        [HttpPost]
        public ActionResult<StaffBreak> Create(StaffBreak staffBreak)
        {

            _db.StaffBreak.Add(staffBreak);
            _db.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = staffBreak.StaffBreakId },
                staffBreak
            );
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, StaffBreak updatedBreak)
        {
            var staffBreak = _db.StaffBreak
                .FirstOrDefault(b => b.StaffBreakId == id);

            if (staffBreak == null)
            {
                return NotFound();
            }

            staffBreak.StaffId = updatedBreak.StaffId;
            staffBreak.LocationId = updatedBreak.LocationId;
            staffBreak.Date = updatedBreak.Date;
            staffBreak.StartTime = updatedBreak.StartTime;
            staffBreak.EndTime = updatedBreak.EndTime;
            staffBreak.Note = updatedBreak.Note;
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, UpdateStaffBreakRequest request)
        {
            var staffBreak = _db.StaffBreak
                .FirstOrDefault(b => b.StaffBreakId == id);

            if (staffBreak == null)
            {
                return NotFound();
            }

            if (request.StaffId.HasValue)
            {
                staffBreak.StaffId = request.StaffId.Value;
            }

            if (request.LocationId.HasValue)
            {
                staffBreak.LocationId = request.LocationId.Value;
            }

            if (request.Date.HasValue)
            {
                staffBreak.Date = request.Date.Value;
            }

            if (request.StartTime.HasValue)
            {
                staffBreak.StartTime = request.StartTime.Value;
            }

            if (request.EndTime.HasValue)
            {
                staffBreak.EndTime = request.EndTime.Value;
            }

            if (request.Note != null)
            {
                staffBreak.Note = request.Note;
            }
            _db.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var staffBreak = _db.StaffBreak
                .FirstOrDefault(b => b.StaffBreakId == id);

            if (staffBreak == null)
            {
                return NotFound();
            }

           
            _db.StaffBreak.Remove(staffBreak);
            _db.SaveChanges();

            return NoContent();
        }

    }
}