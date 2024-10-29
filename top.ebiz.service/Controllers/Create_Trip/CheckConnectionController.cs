
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using top.ebiz.service.Service.Create_Trip;
using top.ebiz.service.Models.Create_Trip;
using Oracle.ManagedDataAccess.Client;
using Microsoft.EntityFrameworkCore;

namespace top.ebiz.service.Controllers.Create_Trip
{

    public class CheckConnectionController : ControllerBase
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
        //[IgnoreAntiforgeryToken]
        [HttpPost( Name = "Getxxx")]
        public IActionResult CheckConnection()
        { 
            return Ok("xxxx");
        }

        // POST: api/login 
        //[IgnoreAntiforgeryToken]
        [HttpPost(Name = "CheckConnection")]
        public IActionResult CheckConnection([FromBody] loginModel value)
        {
            if (value == null) return null;
            userAuthenService service = new userAuthenService();

            object result = "";
            try
            {
                using (TOPEBizCreateTripEntities context = new TOPEBizCreateTripEntities())
                {

                    result = "-->ebiz: ";
                    var mLog = new logModel
                    { module = "login", tevent = "", ref_id = 0, data_log = JsonSerializer.Serialize(value) };

                    result += logService.insertLogTest(mLog).ToString();


                    var sql = @" select id, travel_category as name, allowance_rate as text from  BZ_CONFIG_DAILY_ALLOWANCE  ";
                    var parameters = new List<OracleParameter>();
                    var data = context.NormalModelList.FromSqlRaw(sql, parameters.ToArray()).ToList();


                    result += "-->ebiz: true ," + data?.Count;

                    if (data != null && data?.Count > 0)
                    {
                        mLog = new logModel();
                        for (int i = 0; i < data?.Count; i++)
                        {
                            mLog.module = "-->rows:" + (i + 1);
                            mLog.tevent = " " + data[0].id;
                            mLog.data_log = " " + data[0].text;
                            if (i == 6) { break; }
                        }
                    }
                }
            }
            catch (Exception ex) { result += ex.Message.ToString(); }


            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
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
