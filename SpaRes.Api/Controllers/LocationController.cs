using Microsoft.AspNetCore.Mvc;
using SpaRes.Api.Data;
using SpaRes.Api.Models;

namespace SpaRes.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly SpaResDbContext _db;

        public LocationController(SpaResDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetLocations()
        {
            var locations = _db.Locations
                .Where(location => location.IsActive)
                .ToList();

            return Ok(locations);
        }

        [HttpPost]
        public IActionResult CreateLocation(Location location)
        {
            _db.Locations.Add(location);
            _db.SaveChanges();

            return CreatedAtAction(
                nameof(GetLocations),
                new { id = location.LocationId },
                location
            );
        }
    }
}