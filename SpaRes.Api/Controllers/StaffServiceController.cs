using Microsoft.AspNetCore.Mvc;
using SpaRes.Api.Data;
using SpaRes.Api.Models;

namespace SpaRes.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffServiceController : ControllerBase
    {
        private readonly SpaResDbContext _db;

        public StaffServiceController(SpaResDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetStaffServices()
        {
            return Ok(_db.StaffServices.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetStaffService(int id)
        {
            var staffService = _db.StaffServices
                .FirstOrDefault(ss => ss.StaffServiceId == id);

            if (staffService == null)
                return NotFound();

            return Ok(staffService);
        }

        [HttpPost]
        public IActionResult CreateStaffService(StaffService staffService)
        {
            _db.StaffServices.Add(staffService);
            _db.SaveChanges();

            return CreatedAtAction(
                nameof(GetStaffService),
                new { id = staffService.StaffServiceId },
                staffService);
        }
    }
}