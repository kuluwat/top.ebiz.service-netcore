using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Text.Json;
using top.ebiz.service.Models.Create_Trip;
using top.ebiz.service.Service.Create_Trip;

namespace top.ebiz.service.Controllers
{
    public class CreateFlowController : Controller
    {
        //private readonly logService _logService;
        //private readonly documentService _documentService;

        //// Use constructor dependency injection for services
        //public CreateFlowController(logService logService, documentService documentService)
        //{
        //    _logService = logService;
        //    //_documentService = documentService;
        //}

        //[ValidateAntiForgeryToken]  
        [HttpPost("IndexTest", Name = "IndexTest")]
        public IActionResult IndexTest()
        {
            var result = new { Message = "This is a test", Success = true };
            return Ok(result);  // ส่งข้อมูล JSON กลับไป
        }

        [HttpPost("IndexTOPEBizEntities", Name = "IndexTOPEBizEntities")]
        public IActionResult IndexTOPEBizEntities([FromBody] loginModel value)
        {
            if (value == null) return null;
            userAuthenService service = new userAuthenService();

            object result = "";
            try
            {
                using (TOPEBizEntities context = new TOPEBizEntities())
                {

                    var sql = @" select id, travel_category as name, allowance_rate as text from  BZ_CONFIG_DAILY_ALLOWANCE where travel_category= :travel_category ";
                    var parameters = new List<OracleParameter>();
                    parameters.Add(context.ConvertTypeParameter("travel_category", "oversea", "char"));
                    var data = context.NormalModelList.FromSqlRaw(sql, parameters.ToArray()).ToList();


                    result += "-->ebiz: true ," + data?.Count;

                    if (data != null && data?.Count > 0)
                    {
                        var mLog = new logModel();
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
    }
}
