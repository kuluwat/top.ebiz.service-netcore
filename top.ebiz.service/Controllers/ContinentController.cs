
using System.Net;
using top.ebiz.service.Models;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Service;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace top.ebiz.service.Controllers
{
    //public class ContinentController : ApiController
    [ApiController]
    [Route("api/[controller]")]
    public class ContinentController : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public ContinentController(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }

        // GET: api/Continent
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Continent/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Continent
        [HttpPost]
        public IActionResult Post([FromBody] ContinentModel value)
        {
            if (value == null) return null;
             
            HttpResponseMessage response = null;
            masterService service = new masterService();
            Object result = service.getContinuent(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);

        }

        // PUT: api/Continent/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Continent/5
        public void Delete(int id)
        {
        }
    }
}
