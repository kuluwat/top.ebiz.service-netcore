﻿ 
using top.ebiz.service.Models;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Service;
using System.Text.Json;
namespace top.ebiz.service.Controllers
{
    //public class WBSController : ApiController

    [ApiController]
    [Route("api/[controller]")]
    public class WBSController : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public WBSController(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }


        // GET: api/WBS
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/WBS/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/WBS
           [HttpPost] public IActionResult Post([FromBody]WBSInputModel value)
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
            Object result = service.getWBS(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }

        // PUT: api/WBS/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/WBS/5
        public void Delete(int id)
        {
        }
    }
}
