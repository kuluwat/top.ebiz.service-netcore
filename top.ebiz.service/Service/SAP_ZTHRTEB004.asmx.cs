using System;
using System.Collections.Generic;
using System.Web.Http;
using top.ebiz.service.Service;
using System.Web.Services;

namespace top.ebiz.service.Service
{
    /// <summary>
    /// Summary description for SAP_ZTHRTEB004
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class SAP_ZTHRTEB004 : System.Web.Services.WebService
    {
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World ";
        }
        [WebMethod]
        public string ZTHROMB004(string action_test)
        {
            string data_daily = "";
            if (action_test == "1") { data_daily = "true"; } else { data_daily = "false"; }
             
            SAPService service = new SAPService();
            string ret = service.ZTHROMB004(data_daily);
            return ret;
        }
    }
}
