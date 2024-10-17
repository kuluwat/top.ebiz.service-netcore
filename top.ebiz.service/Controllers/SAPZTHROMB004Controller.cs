using System;
using System.Collections.Generic; 
using System.Web.Http;
using top.ebiz.service.Service;

namespace top.ebiz.service.Controllers
{
    public class SAPZTHROMB004Controller : ApiController
    {
        // GET: api/SAPZTHROMB004
        public IEnumerable<string> Get()
        {
            SAPService service = new SAPService();
            string ret = service.ZTHROMB004("false");

            return new string[] { "value1", "value2", ret };
        }

        // GET: api/SAPZTHROMB004/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/SAPZTHROMB004
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/SAPZTHROMB004/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/SAPZTHROMB004/5
        public void Delete(int id)
        {
        }
    }
}
