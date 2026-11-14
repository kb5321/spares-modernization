using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaRes.Api.Data;
using SpaRes.Api.Models;

namespace SpaRes.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly SpaResDbContext _db;

        public RoomController(SpaResDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<List<Room>> GetAll()
        {
            var rooms = _db.Room.ToList();
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public ActionResult<Room> GetById(int id)
        {
            var room = _db.Room
                .FirstOrDefault(r => r.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        [HttpPost]
        public ActionResult<Room> Create(Room room)
        {
            _db.Room.Add(room);
            _db.SaveChanges();

            return CreatedAtAction(
                nameof(GetById),
                new { id = room.RoomId },
                room
            );
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Room updatedRoom)
        {
            var room = _db.Room
                .FirstOrDefault(r => r.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            room.LocationId = updatedRoom.LocationId;
            room.RoomName = updatedRoom.RoomName;
            room.Capacity = updatedRoom.Capacity;
            room.OutOfService = updatedRoom.OutOfService;

            _db.SaveChanges();

            return NoContent();
        }
        
    }
}