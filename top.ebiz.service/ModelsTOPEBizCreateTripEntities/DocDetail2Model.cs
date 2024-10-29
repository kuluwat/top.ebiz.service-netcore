using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace top.ebiz.service.Models.Create_Trip
{


    public class DocDetail2Model
    {
        public buttonModel button { get; set; } = new buttonModel();
        public string type { get; set; }
        public string document_status { get; set; }
        public TypeModel oversea { get; set; } = new TypeModel();
        public TypeModel local { get; set; } = new TypeModel();

        //DevFix 20200827 2349 Exchange Rates as of -->ExchangeRatesModel
        public ExchangeRatesModel ExchangeRates { get; set; } = new ExchangeRatesModel();

        //DevFix 20210527 0000 เพิ่มข้อมูล ประเภทใบงานเป็น 1:flow, 2:not flow, 3:training เก็บไว้ที่  BZ_DOC_HEAD.DH_TYPE_FLOW
        public string type_flow { get; set; }

        public string msg_remark { get; set; }


    }
    public class DocList2Model
    {
        public string type { get; set; }
        public string checkbox_1 { get; set; }
        public string checkbox_2 { get; set; }
        public string doc_status { get; set; }
        public string document_status { get; set; }
        public string remark { get; set; }


        //DevFix 20210527 0000 เพิ่มข้อมูล ประเภทใบงานเป็น 1:flow, 2:not flow, 3:training เก็บไว้ที่  BZ_DOC_HEAD.DH_TYPE_FLOW
        public string DH_TYPE_FLOW { get; set; }
    }
    public class TypeModel
    {
       
         
        public List<employeeDoc2Model> employee { get; set; } = new List<employeeDoc2Model>();
        public List<travelerDoc2Model> traveler { get; set; } = new List<travelerDoc2Model>();
        public List<doc2ApproverModel> approver { get; set; } = new List<doc2ApproverModel>();
        public string checkbox_1 { get; set; }
        public string checkbox_2 { get; set; }
        public string remark { get; set; }
    }
    public class employeeDoc2Model
    {
        public string ref_id { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public string name { get; set; }
        public string name2 { get; set; }
        public string org { get; set; }
        public string country_id { get; set; }
        public string country { get; set; }
        public string province { get; set; }
        public string business_date { get; set; }
        public string travel_date { get; set; }
        public string clothing_expense { get; set; }
        public string passport_expense { get; set; }
        public string visa_fee { get; set; }
        public string remark { get; set; }

        //DevFix 20210813 0000 เพิ่ม field city
        public string city { get; set; }

    }
    public class travelerDoc2Model
    {

        public string emp_id { get; set; }
        public string emp_name { get; set; }
        public string emp_name2 { get; set; }
        public string org { get; set; }
        public string country_id { get; set; }
        public string country { get; set; }
        public string province { get; set; }
        public string business_date { get; set; }
        public string travel_date { get; set; }
        public string air_ticket { get; set; }
        public string accommodation { get; set; }
        public string Allowance { get; set; }
        public string allowance_day { get; set; }
        public string allowance_night { get; set; }
        public string clothing_valid { get; set; }
        public string clothing_expense { get; set; }
        public string passport_valid { get; set; }
        public string passport_expense { get; set; }
        public string visa_fee { get; set; }
        public string travel_insurance { get; set; }
        public string transportation { get; set; }
        public string registration_fee { get; set; }
        public string miscellaneous { get; set; }
        public string total_expenses { get; set; }
        public string ref_id { get; set; }
        public string edit { get; set; }
        public string delete { get; set; }
        public string remark { get; set; }

        //DevFix 20210714 0000 เพิ่มสถานะที่ Line/CAP --> 1:Draft , 2:Pendding , 3:Approve , 4:Revise , 5:Reject 
        //0 กับ 3 แก้ไขได้
        public string approve_status { get; set; }
        public string approve_remark { get; set; }

        //DevFix 20210719 0000 เพิ่ม field OPT
        public string approve_opt { get; set; }
        public string remark_opt { get; set; }
        public string remark_cap { get; set; }


        //DevFix 20210817 0000 เพิ่ม field status_approve_line, status_approve_cap, remark_approve_line, remark_approve_cap 
        public string status_approve_line { get; set; }
        public string status_approve_cap { get; set; }
        public string remark_approve_line { get; set; }
        public string remark_approve_cap { get; set; }

        //DevFix 20211013 0000 เพิ่ม key เพื่อใช้ในการแยกข้อมูลออกแต่ละรายการ เนื่องจากเงื่อนไขเดิมข้อมูลซ้ำ --> เก็บค่าเป็น token id
        public string traveler_ref_id { get; set; }
    }
    public class doc2ApproverModel
    {
        public string line_id { get; set; }
        public string type { get; set; }
        public string emp_id { get; set; }
        public string emp_name { get; set; }
        public string emp_org { get; set; }
        public string appr_id { get; set; }
        public string appr_name { get; set; }
        public string appr_org { get; set; }
        public string remark { get; set; }

        //DevFix 20210714 0000 เพิ่มสถานะที่ Line/CAP --> 1:Draft , 2:Pendding , 3:Approve , 4:Revise , 5:Reject 
        //0 กับ 3 แก้ไขได้
        public string approve_status { get; set; }
        public string approve_remark { get; set; }

        //DevFix 20210810 0000 approve level ตามลำดับได้เลย เรียงตาม traverler id
        public string approve_level { get; set; }
    }
    public class approverModel
    {
        public string line_id { get; set; }
        public string type { get; set; }
        public string emp_id { get; set; }
        public string emp_name { get; set; }
        public string emp_org { get; set; }

        public List<approvertraveler> approver_traveler { get; set; } = new List<approvertraveler>();




    }
    public class approvertraveler
    {

        public string emp_id { get; set; }
        public string emp_name { get; set; }
        public string emp_org { get; set; }


    }

    public class approveRemark
    {
        public string appr_emp_id { get; set; }
        public string travel_emp_id { get; set; }
        public string remark { get; set; }
    }

    //DevFix 20200827 2349 Exchange Rates as of -->ExchangeRatesModel
    public class ExchangeRatesModel
    {
        public string ex_value1  { get; set; }
        public string ex_date { get; set; }
        public string ex_cur { get; set; }
    }

}