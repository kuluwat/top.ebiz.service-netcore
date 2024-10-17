using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace top.ebiz.service.Models
{
    public class WBSModel
    {

    }

    public class WBSInputModel
    {
        public string token_login { get; set; }
        public string text { get; set; }
    }

    public class WBSOutModel
    {
        public string wbs { get; set; } = ""; 
        public string cost_center { get; set; }
    }

     

}