using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace top.ebiz.service.Models.Create_Trip
{
    public class sendEmailModel2
    {
        public string user_log { get; set; }
    }


    public class docEmailModel
    {
        public string action { get; set; }
        public string user_token { get; set; }
        public string doc_no { get; set; }
        public string doc_link_url { get; set; }
        public string mail_from { get; set; }
        public string mail_to { get; set; }
        public string mail_cc { get; set; }
        public string mail_subject { get; set; }
        public string mail_dear { get; set; }
        public string mail_detail { get; set; }
        public string mail_revise_reason { get; set; }

        public string mail_business_title { get; set; }
        public string mail_business_date { get; set; }
        public string mail_business_location { get; set; }
        public bool test_send_email { get; set; }
        public List<TravelerMail> mail_business_traveller { get; set; }

    }

    public class sendEmailModel
    {
        public string mail_from { get; set; }
        public string mail_to { get; set; }
        public string mail_cc { get; set; }
        public string mail_subject { get; set; }
        public string mail_body { get; set; }
        public string mail_attachments { get; set; }

    }

    public class TravelerMail
    {
        public string name { get; set; }
    }

    public class tempModel
    {
        public string id { get; set; }
        public string name1 { get; set; }
        public string name2 { get; set; }
        public string name3 { get; set; }
        public string name4 { get; set; }
    }

}
