﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
//using System.Web.Http;
//using System.Web.Script.Serialization;
using System.Text.Json;
using top.ebiz.service.Models.Traveler_Profile;
using top.ebiz.service.Service.Traveler_Profile;

namespace top.ebiz.service.Controllers.Traveler_Profile
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        // GET: api/Controller name
        [HttpGet]
public IEnumerable<string> Get()
{
    return new string[] { "value1", "value2" };
}

// GET: api/Controller name/5
[HttpGet("{id}")]
public string Get(int id)
{
    return "value";
}


               // POST: api/Controller name
        [HttpPost]
public IActionResult Post([FromBody]ExportFileModel value)
        {
            if (value == null) return null;

            var pagename = value.pagename;

            
            ExportReportService service = new ExportReportService();
            HttpResponseMessage response = null;
             
            logService.logModel mLog = new logService.logModel();
            mLog.module = "Report";
            mLog.tevent = "review " + pagename;
            mLog.ref_id = 0;
            mLog.data_log = JsonSerializer.Serialize(value);
            logService.insertLog(mLog);
             
            Object result = null;  
            result = service.repoprt_data_allowance(value);

            string json = JsonSerializer.Serialize(result);

                return Ok(json);
}

// PUT: api/Controller name/5
[HttpPut("{id}")]
public void Put(int id, [FromBody] string value)
{
}

// DELETE: api/Controller name/5
[HttpDelete("{id}")]
public void Delete(int id)
{
}
}
}