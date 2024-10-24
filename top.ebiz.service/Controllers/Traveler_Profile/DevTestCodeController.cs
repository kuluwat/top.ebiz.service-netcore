using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Service.Traveler_Profile;

namespace top.ebiz.service.Controllers.Traveler_Profile
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevTestCodeController : ControllerBase
    {
        // GET: api/DevTestCode
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/DevTestCode/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/DevTestCode
        [HttpPost]
        public ActionResult<string> Post([FromBody] string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return BadRequest("Input value cannot be null or empty.");
            }

            SetDocService service = new SetDocService();

            return Ok("test");
        }

        // PUT: api/DevTestCode/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/DevTestCode/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

    }
}
