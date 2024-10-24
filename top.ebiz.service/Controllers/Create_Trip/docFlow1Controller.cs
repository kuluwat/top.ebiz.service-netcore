using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using top.ebiz.service.Models.Create_Trip;
using top.ebiz.service.Service.Create_trip;

namespace top.ebiz.service.Controllers.Create_Trip
{
    //public class docFlow1Controller : ApiController
    [ApiController]
    [Route("api/[controller]")]
    public class docFlow1Controller : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public docFlow1Controller(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }
        // GET: api/docFlow1
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/docFlow1/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/docFlow1
        [HttpPost]
        public IActionResult Post([FromBody] DocModel value)
        {
            if (value == null) return null;


            logModel mLog = new logModel();
            mLog.module = "DOCUMENT";
            mLog.tevent = "FLOW1";
            mLog.ref_id = 0;
            mLog.data_log = JsonSerializer.Serialize(value);
            logService.insertLog(mLog);

            HttpResponseMessage response = null;
            documentService service = new documentService();
            object result = service.submitFlow1(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }

        // PUT: api/docFlow1/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/docFlow1/5
        public void Delete(int id)
        {
        }
    }
}
