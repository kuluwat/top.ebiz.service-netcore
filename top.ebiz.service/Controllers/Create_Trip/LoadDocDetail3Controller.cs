using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using top.ebiz.service.Models.Create_Trip;
using top.ebiz.service.Service.Create_trip;
namespace top.ebiz.service.Controllers.Create_Trip
{
    //public class LoadDocDetail3Controller : ApiController

    [ApiController]
    [Route("api/[controller]")]
    public class LoadDocDetail3Controller : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public LoadDocDetail3Controller(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }

        // GET: api/LoadDocDetail3
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/LoadDocDetail3/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/LoadDocDetail3
        [HttpPost]
        public IActionResult Post([FromBody] DocDetail3Model value)
        {
            if (value == null) return null;


            logModel mLog = new logModel();
            mLog.module = "DOCUMENT";
            mLog.tevent = "SEARCH DETAIL 3";
            mLog.ref_id = 0;
            mLog.data_log = JsonSerializer.Serialize(value);
            logService.insertLog(mLog);

            searchDocService service = new searchDocService();
            HttpResponseMessage response = null;
            object result = service.searchDetail3(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);

        }

        // PUT: api/LoadDocDetail3/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/LoadDocDetail3/5
        public void Delete(int id)
        {
        }
    }
}
