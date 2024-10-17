
using top.ebiz.service.Models;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Service;


namespace top.ebiz.service.Controllers
{
    //public class genDocNoController : ApiController

    [ApiController]
    [Route("api/[controller]")]
    public class genDocNoController : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public genDocNoController(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }

        // GET: api/genDocNo
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/genDocNo/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/genDocNo
        [HttpPost]
        public IActionResult Post([FromBody] genDocNoModel value)
        {
            if (value == null) return null;


            logModel mLog = new logModel();
            mLog.module = "DOCUMENT";
            mLog.tevent = "GEN_DOCNO";
            mLog.ref_id = 0;
            mLog.data_log = JsonSerializer.Serialize(value);
            logService.insertLog(mLog);

            HttpResponseMessage response = null;
            documentService service = new documentService();
            Object result = service.genDocNo(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }

        // PUT: api/genDocNo/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/genDocNo/5
        public void Delete(int id)
        {
        }
    }
}
