using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Models.Traveler_Profile;
using top.ebiz.service.Service.Traveler_Profile;

namespace ebiz.webservice.service.Controllers
{
   // [ApiController]
    //[Route("api/[controller]")]
    public class EmailConfigController : ControllerBase
    {
        private readonly logService _logService;
        private readonly SendEmailService _sendemailService;

        public EmailConfigController(logService logService, SendEmailService sendemailService)
        {
            _logService = logService;
            _sendemailService = sendemailService;
        }
        // GET: api/EmailConfig
       
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/EmailConfig/5
        
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/EmailConfig
       [IgnoreAntiforgeryToken]
        [HttpPost("EmailConfig", Name = "EmailConfig")]
        public IActionResult Post([FromBody] EmailModel value)
        {
            if (value == null) return null;


            logService.logModel mLog = new logService.logModel();
            mLog.module = "email";
            mLog.tevent = "";
            mLog.ref_id = 0;
            mLog.data_log = JsonSerializer.Serialize(value);
            logService.insertLog(mLog);

            //flow : E-Biz-008 Trabelling Isurance 
            //step : 2 to 3
            //page : travelinsurance ,action : NotiTravelInsuranceForm

            //step : 3 to 4
            //page : travelinsurance ,action : NotiTravelInsuranceListPassportInfo

            //step : 4 to 5
            //page : travelinsurance ,action : NotiTravelInsuranceCertificates

            //flow : E-Biz-009 ISOS 
            //step : 3 to 4
            //page : isos ,action : NotiISOSNewListRuningNoName

            //step : 4 to 5
            //page : isos ,action : NotiISOSNewList


            // Send email service
            HttpResponseMessage response = null;
            SendEmailService service = new SendEmailService();
            object result = service.EmailConfig(value);

            // Serialize and return the result
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }

        // PUT: api/EmailConfig/5
        
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/EmailConfig/5
        
        public void Delete(int id)
        {
           
        }
    }
}
