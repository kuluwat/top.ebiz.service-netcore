

using top.ebiz.service.Models;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Service;
using System.Text.Json;

namespace top.ebiz.service.Controllers
{
    //public class docFlow3Controller : ApiController
    [ApiController]
    [Route("api/[controller]")]
    public class docFlow3Controller : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public docFlow3Controller(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }    
        
        // GET: api/docFlow3
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/docFlow3/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/docFlow3
        [HttpPost]
        public IActionResult Post([FromBody] DocFlow3Model value)
        {
            if (value == null) return null;


            logModel mLog = new logModel();
            mLog.module = "DOCUMENT";
            mLog.tevent = "FLOW3";
            mLog.ref_id = 0;
            mLog.data_log = JsonSerializer.Serialize(value);
            logService.insertLog(mLog);

            HttpResponseMessage response = null;
            documentService service = new documentService();
            Object result = service.submitFlow3(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }

        // PUT: api/docFlow3/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/docFlow3/5
        public void Delete(int id)
        {
        }
    }
}
