using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using top.ebiz.service.Models.Create_Trip;
using top.ebiz.service.Service.Create_trip;

namespace top.ebiz.service.Controllers.Create_Trip
{
    //public class TravelerSummaryController : ApiController

    [ApiController]
    [Route("api/[controller]")]
    public class TravelerSummaryController : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public TravelerSummaryController(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }

        // GET: api/TravelerSummary
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // POST: api/TravelerSummary
        [HttpPost]
        public IActionResult Post([FromBody] TravelerSummaryModel value)
        {
            HttpResponseMessage response = null;


            logModel mLog = new logModel();
            mLog.module = "TAB2";
            mLog.tevent = "APPROVER";
            mLog.ref_id = 0;
            mLog.data_log = JsonSerializer.Serialize(value);
            logService.insertLog(mLog);

            TravelerSummaryService service = new TravelerSummaryService();
            object result = service.getResult(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }

    }
}
