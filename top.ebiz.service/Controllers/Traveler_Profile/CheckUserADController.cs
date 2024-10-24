using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using System.Web.Script.Serialization;
using System.Text.Json;
using top.ebiz.service.Models.Traveler_Profile;
using top.ebiz.service.Service.Traveler_Profile;
using Microsoft.AspNetCore.Mvc;

namespace top.ebiz.service.Controllers.Traveler_Profile
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckUserADController : ControllerBase
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


        // PUT: api/Controller name/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Controller name/5
        public void Delete(int id)
        {
        }
        // POST: api/Controller name
        [HttpPost]
public IActionResult Post([FromBody] ADUserModel value)
        {
            if (value == null) return null;

            
            userAuthenService service = new userAuthenService();
            logService.logModel mLog = new logService.logModel();

            //mLog.module = "Visa";
            //mLog.tevent = "SetVisa";
            //mLog.ref_id = 0;
            //mLog.data_log = JsonSerializer.Serialize(value);
            //logService.insertLog(mLog);

            string UserName = value.UserName;
            string Password = value.Password;
            string msg = "";
            HttpResponseMessage response = null;
            Object result = service.GetADUsersFilter(UserName, Password, ref msg);
             
            string json = JsonSerializer.Serialize(result);

            return Ok(json);
        }
        public class ADUserModel
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }


    }
}
