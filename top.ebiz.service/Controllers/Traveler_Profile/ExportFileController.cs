using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;
using top.ebiz.service.Service.Traveler_Profile;

namespace top.ebiz.service.Controllers.Traveler_Profile
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportFileController : ControllerBase
    {
        private readonly ExportReportService _exportReportService;

        public ExportFileController(ExportReportService exportReportService)
        {
            _exportReportService = exportReportService;
        }

        // GET: api/ExportFile
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ExportFile/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ExportFile
        [HttpPost]
        public IActionResult Post([FromBody] ExportFileModel value)
        {
            if (value == null) return null;

            try
            {
                HttpResponseMessage response = null;
                ExportReportService service = new ExportReportService();
                object result = service.exportfile_data(value);

                // Serialize and return the result
                var json = JsonSerializer.Serialize(result);

                return Ok(json);
            }
            catch (Exception ex)
            {
                
                var errorResponse = new
                {
                    ErrorMessage = ex.Message,
                };

                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }
        }

        // PUT: api/ExportFile/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ExportFile/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
