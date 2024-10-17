 
using top.ebiz.service.Models;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Service; 

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace top.ebiz.service.Controllers
{
    //public class CostCenterController : ApiController

    [ApiController]
    [Route("api/[controller]")]
    public class CostCenterController : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public CostCenterController(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }

        // GET: api/CostCenter
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/CostCenter/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/CostCenter
           [HttpPost] public IActionResult Post([FromBody]CCInputModel value)
        {
            if (value == null) return null;

            
            //logModel mLog = new logModel();
            //mLog.module = "EMPLOYEE";
            //mLog.tevent = "SEARCH";
            //mLog.ref_id = 0;
            //mLog.data_log = JsonSerializer.Serialize(value);
            //logService.insertLog(mLog);

            HttpResponseMessage response = null;
            masterService service = new masterService();
            Object result = service.getCostCenter(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }

        // PUT: api/CostCenter/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CostCenter/5
        public void Delete(int id)
        {
        }
    }
}
