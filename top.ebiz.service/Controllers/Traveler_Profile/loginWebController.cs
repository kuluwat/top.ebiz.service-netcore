﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using top.ebiz.service.Models.Traveler_Profile;
using top.ebiz.service.Service.Traveler_Profile;
//using System.Web.Http;
//using System.Web.Script.Serialization;


namespace top.ebiz.service.Controllers.Traveler_Profile
{
    [ApiController]
    [Route("api/[controller]")]
    public class loginWebController : ControllerBase
    {
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

            logService.logModel mLog = new logService.logModel();
            mLog.module = "loginWeb";
            mLog.tevent = "";
            mLog.ref_id = 0;
            mLog.data_log = JsonSerializer.Serialize(value);
            logService.insertLog(mLog);

            HttpResponseMessage response = null;
            userAuthenService service = new userAuthenService();
            Object result = service.loginWeb(value);

            string json = JsonSerializer.Serialize(result);

            return Ok(json);
        }

        // PUT: api/loginWeb/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/loginWeb/5
        public void Delete(int id)
        {
        }
    }
}