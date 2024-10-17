
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;  // For JSON Serialization
using top.ebiz.service.Models;
using top.ebiz.service.Service;

namespace top.ebiz.service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocFlow2Controller : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public DocFlow2Controller(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }


        // GET: api/docFlow2
        public IEnumerable<string> Get()
        {
            string value = "2019-10-03";
            DateTime? date = DateTime.ParseExact(value, "yyyy-M-d", System.Globalization.CultureInfo.InvariantCulture);
            return new string[] { "value1", "value2" };
        }

        // GET: api/docFlow2/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public IActionResult Post([FromBody] DocFlow2Model value)
        {
            if (value == null)
            {
                return BadRequest("Invalid input");
            }

            // Use System.Text.Json to serialize the object
            var mLog = new logModel { module = "DOCUMENT", tevent = "FLOW2", ref_id = 0, data_log = JsonSerializer.Serialize(value) };

            // Insert log
            logService.insertLog(mLog);

            // Call service method
            var result = _documentService.submitFlow2_v3(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(result);
        }


        // PUT: api/docFlow2/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/docFlow2/5
        public void Delete(int id)
        {
        }
    }
}
