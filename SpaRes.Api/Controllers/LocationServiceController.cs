using Microsoft.AspNetCore.Mvc;
using SpaRes.Api.Data;
using SpaRes.Api.Models;

namespace SpaRes.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationServiceController : ControllerBase
    {
        private readonly SpaResDbContext _db;

        public LocationServiceController(SpaResDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetLocationServices()
        {
            return Ok(_db.LocationServices.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetLocationService(int id)
        {
            var locationService = _db.LocationServices
                .FirstOrDefault(ls => ls.LocationServiceId == id);

            if (locationService == null)
                return NotFound();

            return Ok(locationService);
        }

        [HttpPost]
        public IActionResult CreateLocationService(LocationService locationService)
        {
            _db.LocationServices.Add(locationService);
            _db.SaveChanges();

            return CreatedAtAction(
                nameof(GetLocationService),
                new { id = locationService.LocationServiceId },
                locationService);
        }
    }
}