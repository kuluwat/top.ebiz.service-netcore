using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Models.Traveler_Profile;
using top.ebiz.service.Service.Traveler_Profile;

namespace top.ebiz.service.Controllers.Traveler_Profile
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly logService _logService;
        private readonly SetDocService _setDocService;

        // Constructor Injection for services
        public CompanyController(logService logService, SetDocService setDocService)
        {
            _logService = logService;
            _setDocService = setDocService;
        }

        // GET: api/Company
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Company/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Company
        [HttpPost]
        public IActionResult Post([FromBody] CompanyModel value)
        {
            if (value == null)
            {
                return BadRequest("Input cannot be null.");
            }
            var json = JsonSerializer.Serialize(value);
            return Ok(json);
        }

        // PUT: api/Company/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Company/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
