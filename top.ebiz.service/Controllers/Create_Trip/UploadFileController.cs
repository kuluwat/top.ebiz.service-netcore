using System;
using System.Collections.Generic;
using System.Linq;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Models.Create_Trip;
using top.ebiz.service.Service.Create_Trip;

namespace top.ebiz.service.Controllers.Create_Trip
{ 
    public class UploadFileController : ControllerBase
    {  
        // POST: api/UploadFile
        [HttpPost("UploadFile", Name = "UploadFile")]
        public IActionResult Post()
        { 
            logModel mLog = new logModel();
            mLog.module = "UploadFile";
            mLog.tevent = "";
            mLog.ref_id = 0;
            logService.insertLog(mLog);

            HttpResponseMessage response = null;
            documentService service = new documentService();
            object result = service.uploadfile();

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }
         
    }
}
