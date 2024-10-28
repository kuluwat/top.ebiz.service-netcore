using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace top.ebiz.service.Models.Create_Trip
{
    public class TravelerSummaryModel
    {
        public string token_login { get; set; }
        public string doc_no { get; set; }
        public List<TravelerSummary> traveler_list { get; set; }
    }

    public class TravelerSummary
    {
        public string emp_id { get; set; }
        public string total_expen { get; set; }

        //DevFix 20210527 0000 แก้ไขกรณีที่หา Line/CAP หาได้จาก  2 วิธี คือ หาตาม user หรือหาตาม costcenter 
        public string cost_center { get; set; }

        //DevFix 20210719 0000 เพิ่มส่งสถานะของ user ที่ต้องคำนวณ 1 = ให้คำนวณหา approver , 0 = ไม่ต้องคำนวณหา approver ใหม่
        public string emp_status { get; set; }
    }

    public class TravelerSummaryResultModel
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


        public string approve_action { get; set; }
    }

    public class TravelerDocHead
    {
        public string DH_CODE { get; set; }
        public string DH_TYPE { get; set; }
    }

    public class TravelerExpense
    {
        public string DH_CODE { get; set; }
        public string DTE_EMP_ID { get; set; }
        public string DTE_COST_CENTER { get; set; }
    }

    public class ApproverConditionModel
    {
        public string DOC_TYPE { get; set; }
        public string APPR_TYPE { get; set; }
        public decimal BUDGET_LIMIT { get; set; }
        public string EMP_POSITION { get; set; }
        public string APPROVER_L2 { get; set; }
    }
    public class RestApproverListModel
    {
        public string RestLineByCostcenter { get; set; }
        public string RestCAPByCostcenter { get; set; }
    }

    public class TravelerApproverConditionModel
    {
        public string doc_type { get; set; }
        public string emp_id { get; set; }
        public string total_expen { get; set; }
        public decimal budget_limit { get; set; }
        public string appr_position { get; set; }
        public string appr_type { get; set; }
        public string cost_center { get; set; }
        public string appr_id { get; set; }

        //DevFix 20210714 0000 เพิ่มสถานะที่ Line/CAP --> 1:Draft , 2:Pendding , 3:Approve , 4:Revise , 5:Reject
        //1 กับ 4 แก้ไขได้
        public string approve_status { get; set; }
        public string approve_remark { get; set; }
        //DevFix 20210719 0000 เพิ่ม field OPT
        public string approve_opt { get; set; }
        public string remark_opt { get; set; }


        //DevFix 20210810 0000 approve level ตามลำดับได้เลย เรียงตาม traverler id
        public string approve_level { get; set; }

        //DevFix 20211013 0000 เพิ่ม key เพื่อใช้ในการแยกข้อมูลออกแต่ละรายการ เนื่องจากเงื่อนไขเดิมข้อมูลซ้ำ --> เก็บค่าเป็น token id
        public string traveler_ref_id { get; set; }
    }
    public class BZ_DOC_TRAVELER_APPROVER_V2
    {
        public string DH_CODE { get; set; }
        public decimal? DTA_ID { get; set; }
        public string DTA_TYPE { get; set; }
        public string DTA_APPR_EMPID { get; set; }
        public string DTA_TRAVEL_EMPID { get; set; }
        public string DTA_REMARK { get; set; }
        public decimal? DTA_DOC_STATUS { get; set; }
        public string DTA_APPR_STATUS { get; set; }
        public string DTA_APPR_REMARK { get; set; }
        public decimal? DTA_STATUS { get; set; }
        public string DTA_UPDATE_TOKEN { get; set; }
        public decimal? DTA_APPR_LEVEL { get; set; }
        public string DTA_APPR_POS { get; set; }

        //DevFix 20210714 0000 เพิ่มสถานะที่ Line/CAP --> 1:Draft , 2:Pendding , 3:Approve , 4:Revise , 5:Reject 
        public string DTA_ACTION_STATUS { get; set; }
    }

    public class MasterCostCenter
    {
        public string COST_CENTER { get; set; }
        public string ORG_ID { get; set; }
        public string OTYPE { get; set; }
        public string COM_CODE { get; set; }
        public string SH { get; set; }
        public string VP { get; set; }
        public string AEP { get; set; }
        public string EVP { get; set; }
        public string SEVP { get; set; }
        public string CEO { get; set; }

    }

    public class TravelerUsers
    {
        public string EMPLOYEEID { get; set; }
        public string ENTITLE { get; set; }
        public string ENFIRSTNAME { get; set; }
        public string ENLASTNAME { get; set; }
        public string ORGID { get; set; }
        public string ORGNAME { get; set; }
        public string MANAGER_EMPID { get; set; }
        public string SH { get; set; }
        public string VP { get; set; }
        public string AEP { get; set; }
        public string EVP { get; set; }
        public string SEVP { get; set; }
        public string CEO { get; set; }
        //DevFix 20210527 0000 แก้ไขกรณีที่หา Line/CAP หาได้จาก  2 วิธี คือ หาตาม user หรือหาตาม costcenter 
        public string COST_CENTER { get; set; }
    }
}