 
using top.ebiz.service.Models;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Service;
using System.Text.Json;

namespace top.ebiz.service.Controllers
{
    //public class RequestTypeController : ApiController


    [ApiController]
    [Route("api/[controller]")]
    public class RequestTypeController : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public RequestTypeController(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }

        // GET: api/RequestType
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/RequestType/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/RequestType
           [HttpPost] public IActionResult Post([FromBody]RequestTypeModel value)
        {
            if (value == null) return null;

            
            //logModel mLog = new logModel();
            //mLog.module = "REQUEST_TYPE";
            //mLog.tevent = "GET";
            //mLog.ref_id = 0;
            //mLog.data_log = JsonSerializer.Serialize(value);
            //logService.insertLog(mLog);

            HttpResponseMessage response = null;
            masterService service = new masterService();
            Object result = service.getRequestType(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }

        // PUT: api/RequestType/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/RequestType/5
        public void Delete(int id)
        {
        }
    }
}
