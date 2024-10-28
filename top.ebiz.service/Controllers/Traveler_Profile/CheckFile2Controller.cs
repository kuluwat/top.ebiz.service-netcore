﻿using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Models.Traveler_Profile;
using top.ebiz.service.Service.Traveler_Profile;



namespace top.ebiz.service.Controllers.Traveler_Profile
{
   // [ApiController]
    //[Route("api/[controller]")]
    public class CheckFile2Controller : ControllerBase
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
        [HttpPost("CheckFile2", Name = "CheckFile2")]
        public IActionResult Post([FromBody] ImgList value)
        {
            SetDocService service = new SetDocService();
            //logService.logModel mLog = new logService.logModel();

            //mLog.module = "CheckFile";
            //mLog.tevent = "";
            //mLog.ref_id = 0;
            //mLog.data_log = JsonSerializer.Serialize(value);
            //logService.insertLog(mLog);

            HttpResponseMessage response = null;
            Object result = service.SetTravelerHistoryImg(value);

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
