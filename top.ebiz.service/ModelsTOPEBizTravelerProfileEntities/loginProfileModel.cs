﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace top.ebiz.service.Models.Traveler_Profile
{
    public class loginProfileModel
    {
        public string token_login { get; set; }
    }
    public class loginProfileResultModel
    {
        public string token_login { get; set; }
        public Boolean user_admin { get; set; }
        public string empId { get; set; }
        public string empName { get; set; }
        public string deptName { get; set; }
        public string imgUrl { get; set; }
        public string remark { get; set; }

        //DevFix 20210622 0000 เพิ่มข้อมูล ประเภทพนักงาน 1:Employee, 2:Contract
        public string user_type { get; set; }
    }
}