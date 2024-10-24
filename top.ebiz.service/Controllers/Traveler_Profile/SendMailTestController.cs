using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
//using System.Web.Http;
//using System.Web.Script.Serialization;
using System.Text.Json;
using top.ebiz.service.Models.Traveler_Profile;
using top.ebiz.service.Service.Traveler_Profile;
//using ebiz.webservice.Service;

namespace top.ebiz.service.Controllers.Traveler_Profile
{
    [ApiController]
    [Route("api/[controller]")]
    public class SendMailTestController : ControllerBase
    {
        // GET: api/Controller name
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Controller name/5
        public string Get(int id)
        {
            return id.ToString();
        }


               // POST: api/Controller name
        [HttpPost]
public IActionResult Post([FromBody]VisaOutModel value)
        {

            if (value == null) return null; 
            

            SendEmailService swemail = new SendEmailService();

            String s_mail_to = "znoppadonk@thaioilgroup.com;";
            String s_mail_cc = "";
            String s_subject = "ทดสอบส่ง email ";
            String s_mail_body = "email พร้อมแนบไฟล์ ของ e-Biz";
            string s_mail_attachments = @"";

            string ret = "";
            string msg_error = "";
            try
            {
                ret = swemail.send_mail(s_mail_to, s_mail_cc, s_subject, s_mail_body, s_mail_attachments);
            }
            catch(Exception ex)
            {
                ret = "false";
                msg_error = ex.Message.ToString();
            }
             
            value.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            value.after_trip.opt2 = new subAfterTripModel();
            value.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Send mail succesed." : "Send mail failed.";
            value.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            value.after_trip.opt3 = new subAfterTripModel();
            value.after_trip.opt3.status = "Error msg";
            value.after_trip.opt3.remark = msg_error;


            HttpResponseMessage response = null;
            Object result = value;
            string json = JsonSerializer.Serialize(result);

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
