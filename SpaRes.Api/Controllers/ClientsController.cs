using Microsoft.AspNetCore.Mvc;

namespace SpaRes.Api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ClientsController : ControllerBase
    {
        private static readonly List<Client> clients = new List<Client>();

        [HttpGet("{id}")]
        public Client? GetClient(int id)
        {
            return clients.FirstOrDefault(client => client.Id == id);
        }

        [HttpGet]
        public IEnumerable<Client> GetClients()
        {
            return clients;
        }

        [HttpPost]
        public Client CreateClient(Client client)
        {
            clients.Add(client);
            return client;
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteClient(int id)
        {
            Client? client = clients.FirstOrDefault(client => client.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            clients.Remove(client);

            return Ok();
        }
        [HttpPut("{id}")]

        public IActionResult UpdateClient(int id, Client updatedClient)
        {
            Client? client = clients.FirstOrDefault(client => client.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            client.Name = updatedClient.Name;

            return Ok(client);
        }

    }


}
