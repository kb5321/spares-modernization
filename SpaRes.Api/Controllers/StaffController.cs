using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaRes.Api.Data;
using SpaRes.Api.Models;

namespace SpaRes.Api.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        
        private readonly SpaResDbContext _db;
        public StaffController(SpaResDbContext db) {
            _db = db;

         }

        [HttpGet]
        public ActionResult<List<Staff>> GetAll()
        {
           
            var staff = _db.Staff
                .Where(s => s.IsActive)
                .ToList();

            return Ok(staff);
        }

        [HttpGet("{id}")]
        public ActionResult<Staff> GetById(int id)
        {
            var staff = _db.Staff.FirstOrDefault(s => s.StaffId == id);

            if (staff == null)
            {
                return NotFound();
            }

            return Ok(staff);
        }

        [HttpPost]
        public ActionResult<Staff> Create(Staff staff)
        {
           
            _db.Staff.Add(staff);
            _db.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = staff.StaffId },
                staff
            );
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Staff updatedStaff)
        {
            var staff = _db.Staff.FirstOrDefault(s => s.StaffId == id);

            if (staff == null)
            {
                return NotFound();
            }

            staff.TenantId = updatedStaff.TenantId;
            staff.HomeLocationId = updatedStaff.HomeLocationId;
            staff.FirstName = updatedStaff.FirstName;
            staff.LastName = updatedStaff.LastName;
            staff.Phone = updatedStaff.Phone;
            staff.Email = updatedStaff.Email;
            staff.BirthDate = updatedStaff.BirthDate;
            staff.HireDate = updatedStaff.HireDate;
            staff.Note = updatedStaff.Note;
            staff.IsActive = updatedStaff.IsActive;
            _db.SaveChanges();

            return NoContent();
        }

        [HttpGet("providers")]
        public IActionResult GetProviders(int serviceId, int locationId)
        {
            var serviceOfferedAtLocation = _db.LocationServices
                .Any(ls =>
                    ls.LocationId == locationId &&
                    ls.ServiceId == serviceId &&
                    ls.Available);

            if (!serviceOfferedAtLocation)
            {
                return Ok(new List<Staff>());
            }

            var qualifiedStaffIds = _db.StaffServices
                .Where(ss => ss.ServiceId == serviceId)
                .Select(ss => ss.StaffId)
                .ToList();

            var providers = _db.Staff
                .Where(s =>
                    s.IsActive &&
                    qualifiedStaffIds.Contains(s.StaffId))
                .ToList();

            return Ok(providers);
        }
    }
}