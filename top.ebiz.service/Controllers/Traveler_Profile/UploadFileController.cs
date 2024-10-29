//using Microsoft.AspNetCore.Mvc;
//using System.Text.Json;
//using top.ebiz.service.Models.Traveler_Profile;
//using top.ebiz.service.Service.Traveler_Profile;

//namespace top.ebiz.service.Controllers.Traveler_Profile
//{
//   // [ApiController]
//    //[Route("api/[controller]")]
//    public class UploadFileController : ControllerBase
//    {
//        private readonly logService _logService;
//        private readonly SetDocService _setdocService;

//        public UploadFileController(logService logService, SetDocService setdocService)
//        {
//            _logService = logService;
//            _setdocService = setdocService;
//        }

//        // GET: api/UploadFile
       
//        public IEnumerable<string> Get()
//        {
//            return new string[] { "value1", "value2" };
//        }

//        // GET: api/UploadFile/5
        
//        public string Get(int id)
//        {
//            return "value";
//        }

//        // POST: api/UploadFile
//       [IgnoreAntiforgeryToken]
//        [HttpPost("UploadFile", Name = "UploadFile")]
//        public IActionResult Post()
//        {
//            // Log the upload file event
//            logService.logModel mLog = new logService.logModel();
//            mLog.module = "UploadFile";
//            mLog.tevent = "";
//            mLog.ref_id = 0;
//            logService.insertLog(mLog);

//            HttpResponseMessage response = null;
//            SetDocService service = new SetDocService();
//            object result = service.uploadfile_data_form();

//            // Serialize the result to JSON
//            var json = JsonSerializer.Serialize(result);
//            return Ok(json);
//        }

//        // PUT: api/UploadFile/5
        
//        public void Put(int id, [FromBody] string value)
//        {
//        }

//        // DELETE: api/UploadFile/5
        
//        public void Delete(int id)
//        {
//        }

        

//    }
//}
