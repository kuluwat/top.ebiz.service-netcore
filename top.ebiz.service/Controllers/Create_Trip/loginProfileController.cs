﻿
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Models.Create_Trip;
using top.ebiz.service.Service.Create_Trip;

namespace top.ebiz.service.Controllers.Create_Trip
{ 
    public class loginProfileController : ControllerBase
    { 
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
        [HttpPost("loginProfile", Name = "loginProfile")]
        public IActionResult Post([FromBody] loginProfileModel value)
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
            object result = service.getProfile(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }

        // PUT: api/loginProfile/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/loginProfile/5
        public void Delete(int id)
        {
        }


    }
}