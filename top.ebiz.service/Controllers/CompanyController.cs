
using System.Net;
using top.ebiz.service.Models;
using top.ebiz.service.Service;

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace top.ebiz.service.Controllers
{
    //public class CompanyController : ApiController
    //{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public CompanyController(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }

        // GET: api/Company
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Company/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Company
        [HttpPost]
        public IActionResult Post([FromBody] CompanyModel value)
        {
            if (value == null) return null;
             
            //logModel mLog = new logModel();
            //mLog.module = "COMPANY";
            //mLog.tevent = "GET";
            //mLog.ref_id = 0;
            //mLog.data_log = JsonSerializer.Serialize(value);
            //logService.insertLog(mLog);

            HttpResponseMessage response = null;
            masterService service = new masterService();
            Object result = service.getCompany(value);
             
            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }

        // PUT: api/Company/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Company/5
        public void Delete(int id)
        {
        }
    }
}
