using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Service.Traveler_Profile;
using System.Text.Json;

namespace top.ebiz.service.Controllers.Traveler_Profile
{
   // [ApiController]
    //[Route("api/[controller]")]
    public class ConnectionTestController : ControllerBase
    {
        // GET: api/ConnectionTest
       
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ConnectionTest/5
        
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ConnectionTest
       [IgnoreAntiforgeryToken]
        [HttpPost("ConnectionTest", Name = "ConnectionTest")]
        public IActionResult Post([FromBody] string value)
        {
            if (string.IsNullOrEmpty(value))
                return BadRequest("Input value cannot be null or empty.");

            string ret = string.Empty;
            string ConnStrOleDb = value.ToString();

            try
            {
                // Assuming cls_connection_ebiz is already set up to handle DB operations
                cls_connection_ebiz conn = new cls_connection_ebiz();
                conn.OpenConnection();
                ret += "*********Ok cls_connection_ebiz.";

                DataTable dt = new DataTable();
                string sqlstr = value;

                if (conn.ExecuteData(ref dt, sqlstr) == "")
                {
                    // Perform necessary logic if query executes successfully
                }

                conn.CloseConnection();

                if (dt.Rows.Count > 0)
                {
                    ret += "*********Ok data." + value;
                    ret += "*********rows data." + dt.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                ret += "*********error cls_connection_ebiz: " + ex.Message;
            }

            // Log the results using SearchDocService's logModel
            SearchDocService.logModel log = new SearchDocService.logModel
            {
                module = ret
            };

            // Serialize the log object to JSON
            string json = JsonSerializer.Serialize(log);

            return Ok(json);
        }

        // PUT: api/ConnectionTest/5
        
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ConnectionTest/5
        
        public void Delete(int id)
        {
        }

    }
}
