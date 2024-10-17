using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace top.ebiz.service.Models
{
    public class loginModel
    {
        public string user_id { get; set; }
    }
    public class loginResultModel
    {
        public string msg_sts { get; set; }
        public string msg_txt { get; set; }
        public string token_login { get; set; }
    }
    public class loginWebResultModel
    {
        public string token { get; set; }
        public string name { get; set; } 
    }
    public class loginClientModel
    {
        public string user { get; set; }
        public string pass { get; set; }
    }
    public class logoutModel
    {
        public string token { get; set; }
    }
    public class loginUserResultModel
    {
        public string EMPLOYEEID { get; set; }
    }
    public class Users
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public bool isMapped { get; set; }

        //public string MemberOf { get; set; }
    }

}