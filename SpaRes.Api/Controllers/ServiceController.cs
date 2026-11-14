using Microsoft.AspNetCore.Mvc;
using SpaRes.Api.Data;
using SpaRes.Api.Models;

namespace SpaRes.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly SpaResDbContext _db;

        public ServiceController(SpaResDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetServices()
        {
            var services = _db.Services.ToList();

            return Ok(services);
        }
        [HttpGet("{id}")]
        public IActionResult GetService(int id)
        {
            var service = _db.Services
                .FirstOrDefault(s => s.ServiceId == id);

            if (service == null)
                return NotFound();

            return Ok(service);
        }

        [HttpPost]
        public IActionResult CreateService(Service service)
        {
            _db.Services.Add(service);
            _db.SaveChanges();

            return CreatedAtAction(
                nameof(GetService),
                new { id = service.ServiceId },
                service);
        }
        [HttpPatch("{id}")]
        public IActionResult UpdateService(int id, UpdateServiceRequest request)
        {
            var service = _db.Services
                .FirstOrDefault(s => s.ServiceId == id);

            if (service == null)
                return NotFound();

            

            if (request.Description != null)
                service.Description = request.Description;

            if (request.Amount.HasValue)
                service.Amount = request.Amount.Value;

            if (request.Availability.HasValue)
                service.Availability = request.Availability.Value;

            if (request.CatId.HasValue)
                service.CatId = request.CatId.Value;

            if (request.ShortName != null)
                service.ShortName = request.ShortName;

            if (request.ServiceDuration.HasValue)
                service.ServiceDuration = request.ServiceDuration.Value;

            _db.SaveChanges();

            return NoContent();
        }
        [HttpPut("{id}")]
        public IActionResult UpdateService(int id, Service updatedService)
        {
            var service = _db.Services
                .FirstOrDefault(s => s.ServiceId == id);

            if (service == null)
                return NotFound();

            service.TenantId = updatedService.TenantId;
            service.Description = updatedService.Description;
            service.Amount = updatedService.Amount;
            service.Availability = updatedService.Availability;
            service.CatId = updatedService.CatId;
            service.ShortName = updatedService.ShortName;
            service.ServiceDuration = updatedService.ServiceDuration;

            _db.SaveChanges();

            return NoContent();
        }
    }
}