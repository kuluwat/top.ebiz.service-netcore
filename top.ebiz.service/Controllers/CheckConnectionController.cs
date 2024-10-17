
using System.Data;
using System.Net;
using top.ebiz.service.Models;
using top.ebiz.service.Service;

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace top.ebiz.service.Controllers
{
    //public class CheckConnectionController : ApiController
    //{

    [ApiController]
    [Route("api/[controller]")]
    public class CheckConnectionController : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public CheckConnectionController(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }


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
        //   [HttpPost] public IActionResult Post([FromBody] loginModel value)
        [HttpPost]
        public IActionResult Post([FromBody] loginModel value)
        {
            if (value == null) return null;
            HttpResponseMessage response = null;
            userAuthenService service = new userAuthenService();

            Object result = "";
            try
            {
                result = "-->ebiz: ";
                //logModel mLog = new logModel();
                //mLog.module = "login";
                //mLog.tevent = "";
                //mLog.ref_id = 0;
                //mLog.data_log = JsonSerializer.Serialize(value);
                var mLog = new logModel
                {
                    module = "login",
                    tevent = "",
                    ref_id = 0,
                    data_log = JsonSerializer.Serialize(value)
                };

                result += logService.insertLogTest(mLog).ToString();

                cls_connection_ebiz connebiz = new cls_connection_ebiz();
                connebiz.OpenConnection();
                DataTable dt = new DataTable();
                dt = connebiz.ExecuteAdapter(@" select id, travel_category, allowance_rate from  BZ_CONFIG_DAILY_ALLOWANCE  ").Tables[0];
                connebiz.CloseConnection();

                result += "-->ebiz: true ," + dt.Rows.Count;

                mLog = new logModel();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    mLog.module = "-->rows:" + (i + 1);
                    mLog.tevent = " " + dt.Rows[i]["id"].ToString();
                    mLog.data_log = " " + dt.Rows[i]["allowance_rate"].ToString();
                    if (i == 6) { break; }
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
