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
   // [ApiController]
    //[Route("api/[controller]")]
    public class ReimbursementExchageRateController : ControllerBase
    {
        // GET: api/Controller name
       
public IEnumerable<string> Get()
{
    return new string[] { "value1", "value2" };
}

// GET: api/Controller name/5

public string Get(int id)
{
    return "value";
}


               // POST: api/Controller name
       [IgnoreAntiforgeryToken]
        [HttpPost("ReimbursementExchageRate", Name = "ReimbursementExchageRate")]
public IActionResult Post([FromBody]ReimbursementOutModel value )
        {
            if (value == null) return null;

            var page_name = "reimbursement";
            var module_name = "insert data exchange rate";

            
            SetMasterDataService service = new SetMasterDataService();
            logService.logModel mLog = new logService.logModel();

            mLog.module = "Maintain Data";
            mLog.tevent = "Insert Data Exchange Rate";
            mLog.ref_id = 0;
            mLog.data_log = JsonSerializer.Serialize(value);
            logService.insertLog(mLog);

            HttpResponseMessage response = null;
            Object result = null;

            service.SetReimbursementExchageRate(value );

            string json = JsonSerializer.Serialize(result);

                return Ok(json);
}

// PUT: api/Controller name/5

public void Put(int id, [FromBody] string value)
{
}

// DELETE: api/Controller name/5

public void Delete(int id)
{
}
}
}
