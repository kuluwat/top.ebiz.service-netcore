
using top.ebiz.service.Models;
using Microsoft.AspNetCore.Mvc;
using top.ebiz.service.Service;
using System.Text.Json;

namespace top.ebiz.service.Controllers
{
    //public class EstimateExpenseController : ApiController

    [ApiController]
    [Route("api/[controller]")]
    public class EstimateExpenseController : ControllerBase
    {
        private readonly logService _logService;
        private readonly documentService _documentService;

        // Use constructor dependency injection for services
        public EstimateExpenseController(logService logService, documentService documentService)
        {
            _logService = logService;
            _documentService = documentService;
        }


        // GET: api/EstimateExpense
        public IEnumerable<string> Get()
        {
            EstimateExpenseService service = new EstimateExpenseService();
            DateTime? d = new DateTime(2020, 3, 30);
            DateTime dchk = (DateTime)d;
            DateTime result = dchk.AddMonths(-2);
            //decimal result = service.getExpireDateBefore(d);
            return new string[] { "value1", "value2" };
        }

        // GET: api/EstimateExpense/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/EstimateExpense
        // POST: api/Company
        [HttpPost]
        public IActionResult Post([FromBody] EstExpInputModel value)
        {
            if (value == null) return null;


            //logModel mLog = new logModel();
            //mLog.module = "COMPANY";
            //mLog.tevent = "GET";
            //mLog.ref_id = 0;
            //mLog.data_log = JsonSerializer.Serialize(value);
            //logService.insertLog(mLog);

            HttpResponseMessage response = null;
            EstimateExpenseService service = new EstimateExpenseService();
            Object result = service.EstimateExpense(value);

            // Serialize the result to JSON
            var json = JsonSerializer.Serialize(result);
            return Ok(json);
        }

        // PUT: api/EstimateExpense/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/EstimateExpense/5
        public void Delete(int id)
        {
        }
    }
}
