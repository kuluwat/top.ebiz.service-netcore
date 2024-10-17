
using top.ebiz.service.Models;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Service;
using System.Text.Json;

namespace top.ebiz.service.Controllers
{
    //public class LoadDocDetail2Controller : ApiController

    [ApiController]
    [Route("api/[controller]")]
    public class LoadDocDetail2Controller : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public LoadDocDetail2Controller(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }


        // GET: api/LoadDocDetail2
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/LoadDocDetail2/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/LoadDocDetail2
        [HttpPost]
        public IActionResult Post([FromBody] DocDetailSearchModel value)
        {
            if (value == null) return null;


            logModel mLog = new logModel();
            mLog.module = "DOCUMENT2(DETAIL)";
            mLog.tevent = "SEARCH";
            mLog.ref_id = 0;
            mLog.data_log = JsonSerializer.Serialize(value);
            logService.insertLog(mLog);

            searchDocService service = new searchDocService();
            HttpResponseMessage response = null;
            Object result = service.searchDetail2(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }

        // PUT: api/LoadDocDetail2/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/LoadDocDetail2/5
        public void Delete(int id)
        {
        }
    }
}
