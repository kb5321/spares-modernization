using Microsoft.AspNetCore.Mvc;
using SpaRes.Api.Data;
using SpaRes.Api.Models;


namespace SpaRes.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffAbsenceController : ControllerBase
    {
        private readonly SpaResDbContext _db;

        public StaffAbsenceController(SpaResDbContext db)
        {
            _db = db;
        }

      

        [HttpGet]
        public ActionResult<List<StaffAbsence>> GetAll()
        {
            var staffAbsences = _db.StaffAbsence.ToList();
            return Ok(staffAbsences);

          
        }

        [HttpGet("{id}")]
        public ActionResult<StaffAbsence> GetById(int id)
        {
            var staffAbsence = _db.StaffAbsence
                .FirstOrDefault(a => a.StaffAbsenceId == id);

            if (staffAbsence == null)
            {
                return NotFound();
            }

            return Ok(staffAbsence);
        }

        [HttpPost]
        public ActionResult<StaffAbsence> Create(StaffAbsence staffAbsence)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (staffAbsence.StartDate < today)
            {
                return BadRequest("StartDate cannot be in the past.");
            }

            if (staffAbsence.EndDate < today)
            {
                return BadRequest("EndDate cannot be in the past.");
            }

            if (staffAbsence.EndDate <= staffAbsence.StartDate)
            {
                return BadRequest("EndDate must be later than StartDate.");
            }



            _db.StaffAbsence.Add(staffAbsence);
            _db.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = staffAbsence.StaffAbsenceId },
                staffAbsence
            );
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, StaffAbsence updatedAbsence)
        {
            var staffAbsence = _db.StaffAbsence
                .FirstOrDefault(a => a.StaffAbsenceId == id);

            if (staffAbsence == null)
            {
                return NotFound();
            }
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (updatedAbsence.StartDate < today)
            {
                return BadRequest("StartDate cannot be in the past.");
            }

            if (updatedAbsence.EndDate < today)
            {
                return BadRequest("EndDate cannot be in the past.");
            }

            if (updatedAbsence.EndDate <= updatedAbsence.StartDate)
            {
                return BadRequest("EndDate must be later than StartDate.");
            }

            staffAbsence.StaffId = updatedAbsence.StaffId;
            staffAbsence.StartDate = updatedAbsence.StartDate;
            staffAbsence.EndDate = updatedAbsence.EndDate;
            staffAbsence.Note = updatedAbsence.Note;
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, UpdateStaffAbsenceRequest request)
        {
            var staffAbsence = _db.StaffAbsence 
                .FirstOrDefault(a => a.StaffAbsenceId == id);

            if (staffAbsence == null)
            {
                return NotFound();
            }
            var today = DateOnly.FromDateTime(DateTime.Today);

            var newStartDate = request.StartDate ?? staffAbsence.StartDate;
            var newEndDate = request.EndDate ?? staffAbsence.EndDate;

            if (newStartDate < today)
            {
                return BadRequest("StartDate cannot be in the past.");
            }

            if (newEndDate < today)
            {
                return BadRequest("EndDate cannot be in the past.");
            }

            if (newEndDate <= newStartDate)
            {
                return BadRequest("EndDate must be later than StartDate.");
            }

            if (request.StaffId.HasValue)
            {
                staffAbsence.StaffId = request.StaffId.Value;
            }

            if (request.StartDate.HasValue)
            {
                staffAbsence.StartDate = request.StartDate.Value;
            }

            if (request.EndDate.HasValue)
            {
                staffAbsence.EndDate = request.EndDate.Value;
            }

            if (request.Note != null)
            {
                staffAbsence.Note = request.Note;
            }
            _db.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var staffAbsence = _db.StaffAbsence
                .FirstOrDefault(a => a.StaffAbsenceId == id);

            if (staffAbsence == null)
            {
                return NotFound();
            }

            _db.StaffAbsence.Remove(staffAbsence);
            _db.SaveChanges();

            return NoContent();
        }
    }
}