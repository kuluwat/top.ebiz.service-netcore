using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
//using System.Web.Http;
//using System.Web.Script.Serialization;
//using ebiz.webservice.Models;

namespace top.ebiz.service.Controllers.Traveler_Profile
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrackingController : ControllerBase
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
public IActionResult Post([FromBody]TrackingModel value)
        {
            if (value == null) return null;

            
            HttpResponseMessage response = null;

            string json = null;

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
