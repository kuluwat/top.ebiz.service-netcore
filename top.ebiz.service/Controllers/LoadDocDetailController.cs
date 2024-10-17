 
using top.ebiz.service.Models;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Service;
using System.Text.Json;

namespace top.ebiz.service.Controllers
{
    //public class LoadDocDetailController : ApiController


    [ApiController]
    [Route("api/[controller]")]
    public class LoadDocDetailController : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public LoadDocDetailController(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }


        // GET: api/LoadDocDetail
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/LoadDocDetail/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/LoadDocDetail
           [HttpPost] public IActionResult Post([FromBody]DocDetailSearchModel value)
        {
            if (value == null) return null;
            

            logModel mLog = new logModel();
            mLog.module = "DOCUMENT";
            mLog.tevent = "SEARCH(DETAIL2)";
            mLog.ref_id = 0;
            mLog.data_log = JsonSerializer.Serialize(value);
            logService.insertLog(mLog);

            searchDocService service = new searchDocService();
            HttpResponseMessage response = null;
            Object result = service.searchDetail(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }

        // PUT: api/LoadDocDetail/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/LoadDocDetail/5
        public void Delete(int id)
        {
        }
    }
}
