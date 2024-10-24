
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Models.Create_Trip;
using top.ebiz.service.Service.Create_trip;

namespace top.ebiz.service.Controllers.Create_Trip
{
    [ApiController]
    [Route("api/[controller]")]
    public class loginWebController : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public loginWebController(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }
        // GET: api/loginWeb
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/loginWeb/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/loginWeb
        [HttpPost]
        public IActionResult Post([FromBody] loginClientModel value)
        {
            if (value == null) return null;


            logModel mLog = new logModel();
            mLog.module = "loginWeb";
            mLog.tevent = "";
            mLog.ref_id = 0;
            mLog.data_log = JsonSerializer.Serialize(value);
            logService.insertLog(mLog);

            HttpResponseMessage response = null;
            userAuthenService service = new userAuthenService();
            object result = service.loginWeb(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }

        // PUT: api/loginWeb/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/loginWeb/5
        public void Delete(int id)
        {
        }
    }
}
