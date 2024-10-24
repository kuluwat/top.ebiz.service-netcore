
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Models.Create_Trip;
using top.ebiz.service.Service.Create_Trip;

namespace top.ebiz.service.Controllers.Create_Trip
{ 
    public class loginController : ControllerBase
    { 
        // GET: api/login
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/loginProfile/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/login
        [HttpPost("login", Name = "login")]
        public IActionResult Post([FromBody] loginModel value)
        {
            if (value == null) return null;
             
            // Use System.Text.Json to serialize the object
            var mLog = new logModel { module = "login", tevent = "", ref_id = 0, data_log = JsonSerializer.Serialize(value) };

            // Insert log
            logService.insertLog(mLog);

            // Call service method
            userAuthenService service = new userAuthenService();
            var result = service.login(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);

            return Ok(result);
        }

        // PUT: api/login/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/login/5
        public void Delete(int id)
        {
        }
    }
}
