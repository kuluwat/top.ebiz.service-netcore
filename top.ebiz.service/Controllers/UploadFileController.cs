using System;
using System.Collections.Generic;
using System.Linq;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Models;
using top.ebiz.service.Service;

namespace top.ebiz.service.Controllers
{
    //public class UploadFileController : ApiController

    [ApiController]
    [Route("api/[controller]")]
    public class UploadFileController : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public UploadFileController(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        } 

        // GET: api/login
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/loginProfile/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/login
        [HttpPost]
        public IActionResult Post()
        {

            logModel mLog = new logModel();
            mLog.module = "UploadFile";
            mLog.tevent = "";
            mLog.ref_id = 0;
            logService.insertLog(mLog);

            HttpResponseMessage response = null;
            documentService service = new documentService();
            Object result = service.uploadfile();

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }

        // PUT: api/login/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/login/5
        public void Delete(int id)
        {
        }
    }
}
