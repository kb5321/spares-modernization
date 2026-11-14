using Microsoft.AspNetCore.Mvc;
using SpaRes.Api.Data;
using SpaRes.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace SpaRes.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ClientsController : ControllerBase
    {
        
        private readonly SpaResDbContext _db;

        public ClientsController(SpaResDbContext db) {

            _db= db;
        
        }

        [HttpGet("{id}")]
        public Client? GetClient(int id)
        {
            return _db.Clients.FirstOrDefault(client => client.ClientId == id);
        }

        [HttpGet]
        public IEnumerable<Client> GetClients()
        {
            var clients = _db.Clients.ToList();
            return clients;


            
        }

        [HttpPost]
        public Client CreateClient(Client client)
        {
            client.RegistrationDate = DateTime.Now;
            _db.Clients.Add(client);
            _db.SaveChanges();
            return client;

        
        }

     
        [HttpPut("{id}")]
        public IActionResult UpdateClient(int id, UpdateClientRequest request)
        {
            var client = _db.Clients
                .FirstOrDefault(c => c.ClientId == id);

            if (client == null)
            {
                return NotFound();
            }

            client.FirstName = request.FirstName;
            client.LastName = request.LastName;
            client.Gender = request.Gender;
            client.Birthdate = request.Birthdate;
            client.Phone = request.Phone;
            client.Email = request.Email;
            client.Address = request.Address;
            client.City = request.City;
            client.State = request.State;
            client.ZipCode = request.ZipCode;

            _db.SaveChanges();

            return NoContent();
        }
        [HttpPatch("{id}")]
        public IActionResult PatchClient(int id, PatchClientRequest request)
        {
            var client = _db.Clients
                .FirstOrDefault(c => c.ClientId == id);

            if (client == null)
            {
                return NotFound();
            }

            if (request.FirstName != null)
                client.FirstName = request.FirstName;

            if (request.LastName != null)
                client.LastName = request.LastName;

            if (request.Gender != null)
                client.Gender = request.Gender;

            if (request.Birthdate != null)
                client.Birthdate = request.Birthdate;

            if (request.Phone != null)
                client.Phone = request.Phone;

            if (request.Email != null)
                client.Email = request.Email;

            if (request.Address != null)
                client.Address = request.Address;

            if (request.City != null)
                client.City = request.City;

            if (request.State != null)
                client.State = request.State;

            if (request.ZipCode != null)
                client.ZipCode = request.ZipCode;

            _db.SaveChanges();

            return NoContent();
        }

    }


}
