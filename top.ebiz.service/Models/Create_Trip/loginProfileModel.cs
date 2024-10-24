using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace top.ebiz.service.Models.Create_Trip
{
    public class loginProfileModel
    {
        public string token_login { get; set; }
    }
    public class loginProfileResultModel
    {
        public string empId { get; set; }
        public string empName { get; set; }
        public string deptName { get; set; }
        public string imgUrl { get; set; }
    }
}