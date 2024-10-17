 
using System.Text.Json; 
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Models;
using top.ebiz.service.Service;

namespace top.ebiz.service.Controllers
{
    //public class loginProfileController : ApiController

    [ApiController]
    [Route("api/[controller]")]
    public class loginProfileController : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public loginProfileController(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }
        // GET: api/loginProfile
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/loginProfile/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/loginProfile
           [HttpPost] public IActionResult Post([FromBody]loginProfileModel value)
        {
            if (value == null) return null;

            
            logModel mLog = new logModel();
            mLog.module = "loginProfile";
            mLog.tevent = "";
            mLog.ref_id = 0;
            mLog.data_log = JsonSerializer.Serialize(value);
            logService.insertLog(mLog);

            HttpResponseMessage response = null;
            userAuthenService service = new userAuthenService();
            Object result = service.getProfile(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }

        // PUT: api/loginProfile/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/loginProfile/5
        public void Delete(int id)
        {
        }


    }
}
