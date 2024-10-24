using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using top.ebiz.service.Models.Create_Trip;
using top.ebiz.service.Service.Create_trip;

namespace top.ebiz.service.Controllers.Create_Trip
{
    //public class LoadDocListController : ApiController


    [ApiController]
    [Route("api/[controller]")]
    public class LoadDocListController : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public LoadDocListController(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }


        // GET: api/LoadDocList
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/LoadDocList/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/LoadDocList
        [HttpPost]
        public IActionResult Post([FromBody] SearchDocumentModel value)
        {
            if (value == null) return null;


            logModel mLog = new logModel();
            mLog.module = "DOCUMENT(LIST)";
            mLog.tevent = "SEARCH";
            mLog.ref_id = 0;
            mLog.data_log = JsonSerializer.Serialize(value);
            logService.insertLog(mLog);

            searchDocService service = new searchDocService();
            HttpResponseMessage response = null;
            object result = service.searchDocument(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }

        // PUT: api/LoadDocList/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/LoadDocList/5
        public void Delete(int id)
        {
        }
    }
}
