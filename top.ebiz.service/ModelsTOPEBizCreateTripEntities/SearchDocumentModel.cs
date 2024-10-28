using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace top.ebiz.service.Models.Create_Trip
{
    public class SearchDocumentModel
    {
        public string token_login { get; set; }
        public string type { get; set; }
        public string country_id { get; set; }
        public DateFromToModel business { get; set; }
        public string status { get; set; }
        public string keyword { get; set; }
    }
    public class DateFromToModel
    {
        public string start { get; set; }
        public string stop { get; set; }
    }

    public class SearchDocumentResultModel
    {
        public string id { get; set; }
        public string doc_date { get; set; }
        public string button_status { get; set; }
        public string button_copy { get; set; }
        public string title { get; set; }
        public string business_trip { get; set; }
        public string type { get; set; }
        public string place { get; set; }
        public string business { get; set; }
        public string doc_id { get; set; }
        public string by { get; set; }
        public string person { get; set; }
        public string tab_no { get; set; }

        //status_trip_cancelled
        // --> 'true'   ที่ถูกยกเลิก
        //--> 'false'   ที่ไม่ถูกยกเลิก
        public string status_trip_cancelled { get; set; }
    }
    public class SearchUserModel
    {
        public string user_name { get; set; }
        public string user_id { get; set; }
        public string user_role { get; set; }
        public string action_status { get; set; }
        public string email { get; set; }
        public string user_type { get; set; }
        public string user_display { get; set; }
    }
    public class SearchCAP_TraverlerModel
    {
        public string user_name { get; set; }
        public string user_id { get; set; }
        public string user_role { get; set; }
        public string action_status { get; set; }
        public string email { get; set; }
        public string user_type { get; set; }
        public string user_traverler_id { get; set; }
        public string type_reject { get; set; }
        public string type_approve { get; set; }
    }
    public class costcenter_io
    {
        public string cc { get; set; }
        public string io { get; set; }
    }

    #region DevFix 20200909 1606 กรณที่กรอกข้อมูล GL Account ใหม่ที่ไม่ได้อยู่ใน master ให้เพิ่มเข้าระบบ 
    public class gl_account
    {
        public string gl_no { get; set; }
    }
    #endregion DevFix 20200909 1606 กรณที่กรอกข้อมูล GL Account ใหม่ที่ไม่ได้อยู่ใน master ให้เพิ่มเข้าระบบ 

    public class SearchCAPModel
    {
        //DTA_TRAVEL_EMPID, DTA_APPR_LEVEL, DTA_APPR_EMPID, DTA_ACTION_STATUS
        public string DTA_TRAVEL_EMPID { get; set; }
        public string DTA_APPR_LEVEL { get; set; }
        public string DTA_APPR_EMPID { get; set; }
        public string DTA_ACTION_STATUS { get; set; }
    }
    public class TelephoneModel
    {
        public string tel_services_team { get; set; }
        public string tel_call_center { get; set; }
    }
}