using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;
using top.ebiz.service.Models.Create_Trip;

namespace top.ebiz.service.Service.Create_trip
{
    public class TravelerSummaryService
    {
        public List<TravelerSummaryResultModel> getResult(TravelerSummaryModel value)
        {
            return getResultV2(value);
        }
        public List<TravelerSummaryResultModel> getResultV2(TravelerSummaryModel value)
        {
            var bCheckByEmpStatus = true;

            var data = new List<TravelerSummaryResultModel>();
            try
            {
                var emp_id_active = "";
                var emp_id_inactive = "";
                List<TravelerSummary> temp = new List<TravelerSummary>();
                List<TravelerSummary> temp_active = new List<TravelerSummary>();
                foreach (var item in value.traveler_list)
                {
                    //เนื่องจากหน้าบ้านยังไม่ได้ up ขึ้นไปใหม่ทำให้ส่งข้อมูล item.emp_status  = null
                    if (item.emp_status == null) { item.emp_status = "1"; }

                    //กรณีที่ไม่ใช่ emp ที่ active ไม่ต้องนำไปคำนวณใหม่ ให้ดึงของเดิมมาจาก table 
                    if (item.emp_status == "1")
                    {
                        if (emp_id_active != "") { emp_id_active += ","; }
                        emp_id_active += "'" + item.emp_id + "'";

                        temp_active.Add(new TravelerSummary
                        {
                            emp_id = item.emp_id,
                        });
                    }
                    else
                    {
                        if (emp_id_inactive != "") { emp_id_inactive += ","; }
                        emp_id_inactive += "'" + item.emp_id + "'";
                    }


                    string total_expense = string.IsNullOrEmpty(item.total_expen) ? "0" : item.total_expen;

                    var t = temp.Where(p => p.emp_id.Equals(item.emp_id)).FirstOrDefault();
                    if (t == null)
                    {
                        temp.Add(new TravelerSummary
                        {
                            emp_id = item.emp_id,
                            total_expen = total_expense,
                        });
                    }
                    else
                    {
                        t.total_expen = Convert.ToString(Convert.ToDecimal(t.total_expen) + Convert.ToDecimal(total_expense));
                    }
                }

                value.traveler_list = temp;

                using (TOPEBizEntities context = new TOPEBizEntities())
                {
                    var query = "";

                    #region query
                    //DevFix 20210714 0000 ดึงข้อมูลรายละเอียด approver เดิม --> กรณีที่ไม่เอา approve CAP ที่ยังไม่ถึงขั้นตอนการ อนุมัติต้องเอา status = 1 ออก
                    query = @"  select distinct dta_travel_empid as emp_id, dta_appr_empid as appr_id, dta_type as appr_type
                                , a.dta_action_status as approve_status
                                , a.dta_appr_remark as approve_remark
                                , to_char(a.dta_appr_level) as approve_level
                                from BZ_DOC_TRAVELER_APPROVER a
                                where a.dta_action_status in (2,3,4,5) and dta_doc_status is not null
                                and a.dh_code = '" + value.doc_no + "'  ";
                    var dataApporver_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(query).ToList();

                    query = @"  select distinct dta_travel_empid as emp_id, dta_appr_empid as appr_id, dta_type as appr_type
                                , a.dta_action_status as approve_status
                                , a.dta_appr_remark as approve_remark
                                , to_char(a.dta_appr_level) as approve_level
                                from BZ_DOC_TRAVELER_APPROVER a
                                where a.dta_action_status in (2,5) and dta_doc_status is not null
                                and a.dh_code = '" + value.doc_no + "'  ";
                    var dataApporver_Def2 = context.Database.SqlQuery<TravelerApproverConditionModel>(query).ToList();

                    query = @"  select distinct dta_travel_empid as emp_id, dta_appr_empid as appr_id, dta_type as appr_type
                                , a.dta_action_status as approve_status
                                , a.dta_appr_remark as approve_remark 
                                , to_char(a.dta_appr_level) as approve_level
                                from BZ_DOC_TRAVELER_APPROVER a 
                                where a.dh_code = '" + value.doc_no + "' and dta_doc_status is not null ";
                    if (bCheckByEmpStatus == true)
                    {
                        if (emp_id_inactive != "")
                        {
                            query += @"  and a.dta_travel_empid in ( " + emp_id_inactive + ") ";
                        }
                        else
                        {
                            query += @"  and a.dta_travel_empid not in ( " + emp_id_active + ") ";
                        }
                    }
                    else
                    {
                    }
                    var dataApporver_not_active = context.Database.SqlQuery<TravelerApproverConditionModel>(query).ToList();

                    query = @"  select distinct dta_travel_empid as emp_id, dta_appr_empid as appr_id, dta_type as appr_type
                                , a.dta_action_status as approve_status
                                , a.dta_appr_remark as approve_remark 
                                , to_char(a.dta_appr_level) as approve_level
                                from BZ_DOC_TRAVELER_APPROVER a 
                                where  dta_doc_status is not null and a.dta_type ='2' and a.dta_action_status <> '1' 
                                and a.dh_code = '" + value.doc_no + "'  and a.dta_travel_empid in ( " + emp_id_active + ") ";

                    query += @"  order by a.dta_travel_empid, a.dta_appr_level";
                    var dataApporver_in_active = context.Database.SqlQuery<TravelerApproverConditionModel>(query).ToList();

                    query = @" select distinct a.dta_travel_empid as emp_id, to_char((max(nvl(a.dta_appr_level,0))+1)) as approve_level
                                 from BZ_DOC_TRAVELER_APPROVER a 
                                 where  dta_doc_status is not null and a.dta_type ='2' 
                                 and a.dh_code = '" + value.doc_no + "'  and a.dta_travel_empid in ( " + emp_id_active + ") ";
                    query += @"   group by a.dta_travel_empid";
                    var dataMaxApprover_Level = context.Database.SqlQuery<TravelerApproverConditionModel>(query).ToList();

                    query = "SELECT *";
                    query += " FROM BZ_DOC_HEAD";
                    query += " WHERE DH_CODE = '" + value.doc_no + "'";
                    var dataDoc = context.Database.SqlQuery<TravelerDocHead>(query).ToList().FirstOrDefault();

                    query = "SELECT EMPLOYEEID, ENTITLE, ENFIRSTNAME, ENLASTNAME, ORGNAME, MANAGER_EMPID, SH, VP, AEP, EVP, SEVP, CEO";
                    query += " FROM BZ_USERS";
                    var usersList = context.Database.SqlQuery<TravelerUsers>(query).ToList();

                    //DevFix 20210527 0000 แก้ไขกรณีที่หา CAP หาได้จาก  2 วิธี คือ หาตาม user หรือหาตาม costcenter 
                    query = @"SELECT distinct to_char(a.DTE_EMP_ID) as EMPLOYEEID, null as ENTITLE, null as ENFIRSTNAME, null as ENLASTNAME, null as ORGNAME, null as MANAGER_EMPID, b.SH, b.VP, b.AEP, b.EVP, b.SEVP, b.CEO
                             ,b.COST_CENTER
                             FROM BZ_DOC_TRAVELER_EXPENSE a
                             INNER JOIN BZ_MASTER_COSTCENTER_ORG b on a.dte_cost_center = b.cost_center
                             where a.DH_CODE = '" + value.doc_no + "' ";
                    var costcenterList = context.Database.SqlQuery<TravelerUsers>(query).ToList();
                    foreach (var item in temp)
                    {
                        //var t = costcenterList.SingleOrDefault(p => p.EMPLOYEEID.Equals(item.emp_id));
                        //if (t != null)
                        //{
                        //    item.cost_center = t.COST_CENTER;
                        //}

                        //DevFix 20210813 0000 เนื่องจากคำสั่งมีบัคแก้ไม่เป็น เลยเปลี่ยนวิธี
                        var costcenterListList = costcenterList.Where(a => a.EMPLOYEEID == item.emp_id).ToList();
                        if (costcenterListList != null & costcenterListList.Count > 0)
                        {
                            item.cost_center = costcenterListList[0].COST_CENTER;
                        }
                    }
                    value.traveler_list = temp;

                    //DevFix 20210527 0000 แก้ไขกรณีที่หา Line/CAP หาได้จาก  2 วิธี คือ หาตาม user หรือหาตาม costcenter 
                    bool RestLineByCostcenter = false;
                    bool RestCAPByCostcenter = false;
                    query = @" select 
                              (select  KEY_VALUE as RestLineByCostcenter from BZ_CONFIG_DATA   where KEY_NAME in('RestLineByCostcenter') ) as RestLineByCostcenter
                              ,(select KEY_VALUE as RestCAPByCostcenter from BZ_CONFIG_DATA   where KEY_NAME in('RestCAPByCostcenter')) as RestCAPByCostcenter        
                              from dual  ";
                    var restApproverList = context.Database.SqlQuery<RestApproverListModel>(query).ToList();
                    if (restApproverList.Count > 0)
                    {
                        if (restApproverList[0].RestLineByCostcenter == "true") { RestLineByCostcenter = true; }
                        if (restApproverList[0].RestCAPByCostcenter == "true") { RestCAPByCostcenter = true; }
                    }

                    //DevFix 20210607 0000 แก้เงื่อนไขเพิ่มเติม กรณีที่เป็น type training 
                    query = "SELECT * FROM BZ_APPROVER_CONDITION ";
                    query += " WHERE upper(doc_type) like '" + dataDoc.DH_TYPE.ToUpper() + "%' order by budget_limit ";
                    var conditionList = context.Database.SqlQuery<ApproverConditionModel>(query).ToList();

                    #region DevFix ตรวจสอบ Position ของ user  
                    query = @"  select a.employeeid as EMPLOYEEID, a.function as ORGNAME  
                                from  bz_users a where department is null and sections is null 
                                and (select b.function from bz_users b where b.department is null and b.sections is null and b.employeeid = a.reporttoid  ) = 'SEVP'
                                union
                                select a.employeeid as EMPLOYEEID, a.department as ORGNAME from  bz_users a where function in ( 'SEVP') and department is not null and sections is null 
                                union 
                                select a.employeeid as EMPLOYEEID, a.function as ORGNAME from  bz_users a  where  function = 'SEVP' and department is null and sections is null 
                                union 
                                select a.employeeid as EMPLOYEEID, 'CEO' as ORGNAME from  bz_users a  where  function = 'MGT' and department is null and sections is null ";

                    var positionList = context.Database.SqlQuery<TravelerUsers>(query).ToList();
                    #endregion DevFix ตรวจสอบ Position ของ user 


                    #endregion query


                    if (dataDoc != null)
                    {

                        List<TravelerApproverConditionModel> apprListCAP = new List<TravelerApproverConditionModel>();
                        if (dataDoc.DH_TYPE.ToUpper() == "OVERSEA")
                        {
                            foreach (var item in value.traveler_list)
                            {
                                //ADD LINE
                                var usersDetail = usersList.SingleOrDefault(a => a.EMPLOYEEID == item.emp_id);
                                if (usersDetail != null)
                                {
                                    var usersApprDetail = usersList.SingleOrDefault(a => a.EMPLOYEEID == usersDetail.MANAGER_EMPID);
                                    TravelerSummaryResultModel resultModel = new TravelerSummaryResultModel();
                                    resultModel.line_id = "1";
                                    resultModel.type = "1";
                                    resultModel.emp_id = usersDetail.EMPLOYEEID;
                                    resultModel.emp_name = usersDetail.ENTITLE + " " + usersDetail.ENFIRSTNAME + " " + usersDetail.ENLASTNAME;
                                    resultModel.emp_org = usersDetail.ORGNAME;
                                    resultModel.appr_id = usersApprDetail != null ? usersApprDetail.EMPLOYEEID : "";
                                    resultModel.appr_name = usersApprDetail.ENTITLE + " " + usersApprDetail.ENFIRSTNAME + " " + usersApprDetail.ENLASTNAME;
                                    resultModel.appr_org = usersApprDetail != null ? usersApprDetail.ORGNAME : "";
                                    resultModel.remark = "Endorsed";
                                    resultModel.approve_level = "0";
                                    data.Add(resultModel);
                                }

                                //ADD CAP

                                #region  DevFix แก้ไขตามเงื่อนไขใหม่ 
                                //กรณีที่น้อยกว่า 150,000 ให้ CAP EVP approve 
                                //กรณีที่น้อยกว่า 150,000 แล้วไม่มี CAP EVP ให้ add manual
                                //กรณีที่น้อยกว่า 300,000 ให้ CAP EVP approve 
                                //กรณีที่น้อยกว่า 300,000 แล้วไม่มี CAP EVP ให้ add manual
                                //กรณีที่น้อยกว่า 300,000 ให้ CAP SEVP approve 
                                //กรณีที่น้อยกว่า 300,000 แล้วไม่มี CAP SEVP ให้ CEO approve

                                //ทุกกรณีต้องผ่าน EVP
                                //กรณีที่เป็น  EVPM,EVPE,QMVP ไม่ต้องให้ CAP EVP approve 
                                //กรณีที่เป็น SEVP,CEO ไม่ต้อง CAP SEVP approve  

                                #endregion  DevFix แก้ไขตามเงื่อนไขใหม่ 
                                bool bBreakLine = false;
                                bool sRoleUpEVP = false; //กรณีที่เป็น  EVPM,EVPE,QMVP ไม่ต้องให้ CAP EVP approve 
                                string EMP_POSITION_DEF = "";//ตำแหน่งของพนักงาน 
                                var check_position = positionList.SingleOrDefault(a => a.EMPLOYEEID == item.emp_id);
                                if (check_position != null)
                                {
                                    EMP_POSITION_DEF = check_position.ORGNAME;
                                    sRoleUpEVP = true;
                                }

                                var apprConditionList = conditionList.Where(a => a.DOC_TYPE == "oversea").ToList();
                                var oldCap = new ApproverConditionModel();
                                foreach (var cap in apprConditionList)
                                {
                                    bBreakLine = false;
                                    //if (Convert.ToDecimal(item.total_expen) <= cap.BUDGET_LIMIT )
                                    {
                                        if (cap.BUDGET_LIMIT > Convert.ToDecimal(item.total_expen)) { bBreakLine = true; }

                                        if (cap.EMP_POSITION == "EVP"
                                        && (EMP_POSITION_DEF == "SEVP" || EMP_POSITION_DEF == "CEO" || sRoleUpEVP == true))
                                        {
                                            //กรณีที่เป็น  EVPM,EVPE,QMVP ไม่ต้องให้ CAP EVP approve 
                                            //กรณีที่เป็น SEVP,CEO ไม่ต้อง CAP SEVP approve 
                                            continue;
                                        }
                                        else
                                        {
                                            TravelerApproverConditionModel traveler = new TravelerApproverConditionModel();
                                            traveler = new TravelerApproverConditionModel();
                                            traveler.emp_id = item.emp_id;
                                            traveler.total_expen = item.total_expen;
                                            traveler.budget_limit = cap.BUDGET_LIMIT;
                                            traveler.appr_position = cap.EMP_POSITION;
                                            traveler.appr_type = cap.APPR_TYPE;
                                            traveler.doc_type = cap.DOC_TYPE;
                                            apprListCAP.Add(traveler);
                                        }
                                        if (bBreakLine == true) { break; }
                                    }
                                }
                            }

                            query = "SELECT *";
                            query += " FROM BZ_DOC_TRAVELER_EXPENSE";
                            query += " WHERE DH_CODE = '" + value.doc_no + "' and dte_status = 1 ";
                            var dataTravelExpenseList = context.Database.SqlQuery<TravelerExpense>(query).ToList();


                            var traverleremp_id_bef = "";
                            var traverler_id_aff = "";
                            var iapprove_level = 1;
                            foreach (var item in apprListCAP)
                            {
                                traverler_id_aff = item.emp_id;
                                if (traverler_id_aff != traverleremp_id_bef)
                                {
                                    traverleremp_id_bef = traverler_id_aff;
                                    iapprove_level = 1;
                                }


                                TravelerSummaryResultModel resultModel = new TravelerSummaryResultModel();
                                var travelExpen = dataTravelExpenseList.Where(a => a.DTE_EMP_ID == item.emp_id).ToList().FirstOrDefault();
                                if (travelExpen != null)
                                {
                                    item.cost_center = travelExpen.DTE_COST_CENTER;
                                    query = "SELECT *";
                                    query += " FROM BZ_MASTER_COSTCENTER_ORG";
                                    query += " WHERE COST_CENTER = '" + item.cost_center + "'"; //TODO COST CENTER ID
                                    var masterCostCenterList = context.Database.SqlQuery<MasterCostCenter>(query).ToList();
                                    if (masterCostCenterList.Count == 0 && item.appr_position == "SEVP")
                                    {
                                        //กรณีที่ไม่มี Cost center ให้ไปหา CEO
                                        query = "select null as SEVP, employeeid as CEO from  bz_users a  where  function = 'MGT' and department is null and sections is null ";
                                        masterCostCenterList = context.Database.SqlQuery<MasterCostCenter>(query).ToList();
                                    }
                                    if (masterCostCenterList.Count > 0)
                                    {
                                        foreach (var costDetail in masterCostCenterList)
                                        {
                                            var appr_id = "";
                                            //กรณีที่ไม่มี CAP Approver ให้ add manual 
                                            if (item.appr_position == "SH")
                                            {
                                                appr_id = !string.IsNullOrEmpty(costDetail.SH) ? costDetail.SH : "";
                                            }
                                            else if (item.appr_position == "VP")
                                            {
                                                appr_id = !string.IsNullOrEmpty(costDetail.VP) ? costDetail.VP : "";
                                            }
                                            else if (item.appr_position == "AEP")
                                            {
                                                appr_id = !string.IsNullOrEmpty(costDetail.AEP) ? costDetail.AEP : "";
                                            }
                                            else if (item.appr_position == "EVP")
                                            {
                                                appr_id = !string.IsNullOrEmpty(costDetail.EVP) ? costDetail.EVP : "";
                                            }
                                            else if (item.appr_position == "SEVP")
                                            {
                                                appr_id = !string.IsNullOrEmpty(costDetail.SEVP) ? costDetail.SEVP : !string.IsNullOrEmpty(costDetail.CEO) ? costDetail.CEO : "";
                                            }
                                            else if (item.appr_position == "CEO")
                                            {
                                                appr_id = costDetail.CEO;
                                            }
                                            item.appr_id = appr_id;
                                        }
                                    }

                                    var usersDetail = usersList.SingleOrDefault(a => a.EMPLOYEEID == item.emp_id);
                                    var usersApprDetail = usersList.SingleOrDefault(a => a.EMPLOYEEID == item.appr_id);
                                    //DevFix 20200828 2140 กรณีที่เป็น Approve และ Approve type เดียวกัน ไม่ต้อง add ซ้ำ
                                    var check_data = data.SingleOrDefault(a => a.appr_id == item.appr_id && a.type == "2"
                                     && a.emp_id == item.emp_id);

                                    if (usersApprDetail != null && check_data == null)
                                    {
                                        resultModel.line_id = "1";
                                        resultModel.type = "2";
                                        resultModel.emp_id = usersDetail.EMPLOYEEID;
                                        resultModel.emp_name = usersDetail != null ? usersDetail.ENTITLE + " " + usersDetail.ENFIRSTNAME + " " + usersDetail.ENLASTNAME : "";
                                        resultModel.emp_org = usersDetail != null ? usersDetail.ORGNAME : "";
                                        resultModel.appr_id = usersApprDetail.EMPLOYEEID;
                                        resultModel.appr_name = usersApprDetail.ENTITLE + " " + usersApprDetail.ENFIRSTNAME + " " + usersApprDetail.ENLASTNAME;
                                        resultModel.appr_org = usersApprDetail.ORGNAME;
                                        resultModel.remark = "CAP";
                                        resultModel.approve_level = iapprove_level.ToString();
                                        data.Add(resultModel);
                                        iapprove_level += 1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //DevFix 2020828 2341 เพิ่มเงื่อนไข a.DOC_TYPE == dataDoc.DH_TYPE 
                            //var dataListLINE = conditionList.Where(a => a.APPR_TYPE == "LINE" && a.DOC_TYPE == dataDoc.DH_TYPE).ToList().OrderBy(a => a.BUDGET_LIMIT);
                            //var dataListCAP = conditionList.Where(a => a.APPR_TYPE == "CAP" && a.DOC_TYPE == dataDoc.DH_TYPE).ToList().OrderBy(a => a.BUDGET_LIMIT); 
                            var dataListCAP = conditionList.Where(a => a.APPR_TYPE == "CAP" && a.DOC_TYPE == dataDoc.DH_TYPE).ToList();

                            apprListCAP = new List<TravelerApproverConditionModel>();
                            decimal totleExpens = 0;
                            foreach (var item in value.traveler_list)
                            {
                                totleExpens += Convert.ToDecimal(item.total_expen);
                            }
                            foreach (var item in value.traveler_list)
                            {
                                //ADD LINE
                                var usersDetail = usersList.SingleOrDefault(a => a.EMPLOYEEID == item.emp_id);
                                if (usersDetail != null)
                                {
                                    //DevFix 20210527 0000 เนื่องจาก local MANAGER_EMPID ที่ได้ไม่ตรงกับข้อมูลจริงของ approver
                                    //ปรับให้ค้นหาตามตำแหน่ง SH,VP,AEP,SEVP,CEO 
                                    //var usersApprDetail = usersList.SingleOrDefault(a => a.EMPLOYEEID == usersDetail.MANAGER_EMPID);
                                    #region check line approver 
                                    var line_id = ""; var line_id_cost = "";
                                    line_id = !string.IsNullOrEmpty(usersDetail.SH) ? usersDetail.SH : !string.IsNullOrEmpty(usersDetail.VP) ? usersDetail.VP : !string.IsNullOrEmpty(usersDetail.AEP) ? usersDetail.AEP : !string.IsNullOrEmpty(usersDetail.EVP) ? usersDetail.EVP : !string.IsNullOrEmpty(usersDetail.SEVP) ? usersDetail.SEVP : !string.IsNullOrEmpty(usersDetail.CEO) ? usersDetail.CEO : "";

                                    var usersApprDetail = usersList.SingleOrDefault(a => a.EMPLOYEEID == line_id);
                                    if (RestLineByCostcenter == true)
                                    {
                                        //DevFix 20210527 0000 แก้ไขกรณีที่หา CAP หาได้จาก  2 วิธี คือ หาตาม user หรือหาตาม costcenter 
                                        if (item.cost_center != "")
                                        {
                                            //DevFix 20210720 0000 เพิ่ม try กรณีที่ไม่มีข้อมูล 
                                            try
                                            {
                                                usersApprDetail = costcenterList.Where(a => a.COST_CENTER == item.cost_center).FirstOrDefault();
                                                line_id_cost = !string.IsNullOrEmpty(usersApprDetail.SH) ? usersApprDetail.SH : !string.IsNullOrEmpty(usersApprDetail.VP) ? usersApprDetail.VP : !string.IsNullOrEmpty(usersApprDetail.AEP) ? usersApprDetail.AEP : !string.IsNullOrEmpty(usersApprDetail.EVP) ? usersApprDetail.EVP : !string.IsNullOrEmpty(usersApprDetail.SEVP) ? usersApprDetail.SEVP : !string.IsNullOrEmpty(usersApprDetail.CEO) ? usersApprDetail.CEO : "";
                                            }
                                            catch { continue; }
                                        }
                                    }
                                    if (RestLineByCostcenter == true & line_id_cost != "")
                                    {
                                        line_id = line_id_cost;
                                    }
                                    usersApprDetail = new TravelerUsers();
                                    usersApprDetail = usersList.SingleOrDefault(a => a.EMPLOYEEID == line_id);

                                    #endregion check line approver

                                    TravelerSummaryResultModel resultModel = new TravelerSummaryResultModel();
                                    resultModel.line_id = "1";
                                    resultModel.type = "1";
                                    resultModel.emp_id = usersDetail.EMPLOYEEID;
                                    resultModel.emp_name = usersDetail.ENTITLE + " " + usersDetail.ENFIRSTNAME + " " + usersDetail.ENLASTNAME;
                                    resultModel.emp_org = usersDetail.ORGNAME;
                                    resultModel.appr_id = usersApprDetail != null ? usersApprDetail.EMPLOYEEID : "";
                                    resultModel.appr_name = usersApprDetail.ENTITLE + " " + usersApprDetail.ENFIRSTNAME + " " + usersApprDetail.ENLASTNAME;
                                    resultModel.appr_org = usersApprDetail != null ? usersApprDetail.ORGNAME : "";
                                    resultModel.remark = "Endorsed";
                                    resultModel.approve_level = "0";


                                    data.Add(resultModel);
                                }

                                //FIND CAP
                                var oldCap = new ApproverConditionModel();
                                foreach (var cap in dataListCAP)
                                {
                                    bool bBreakLine = false;
                                    if (cap.BUDGET_LIMIT >= Convert.ToDecimal(totleExpens)) { bBreakLine = true; }
                                    if (bBreakLine == true)
                                    {
                                        TravelerApproverConditionModel traveler = new TravelerApproverConditionModel();

                                        traveler = new TravelerApproverConditionModel();
                                        traveler.emp_id = item.emp_id;
                                        traveler.total_expen = item.total_expen;
                                        traveler.budget_limit = cap.BUDGET_LIMIT;
                                        traveler.appr_position = cap.EMP_POSITION;
                                        traveler.appr_type = cap.APPR_TYPE;
                                        traveler.doc_type = cap.DOC_TYPE;

                                        //DevFix 20210527 0000 แก้ไขกรณีที่หา Line/CAP หาได้จาก  2 วิธี คือ หาตาม user หรือหาตาม costcenter 
                                        traveler.cost_center = item.cost_center;

                                        apprListCAP.Add(traveler);

                                        //DevFix 20200827 1103 add CEO???
                                        if (cap.APPROVER_L2 != null)
                                        {
                                            traveler = new TravelerApproverConditionModel();
                                            traveler.emp_id = item.emp_id;
                                            traveler.total_expen = item.total_expen;
                                            traveler.budget_limit = cap.BUDGET_LIMIT;
                                            traveler.appr_position = cap.APPROVER_L2;
                                            traveler.appr_type = cap.APPR_TYPE;
                                            traveler.doc_type = cap.DOC_TYPE;

                                            //DevFix 20210527 0000 แก้ไขกรณีที่หา Line/CAP หาได้จาก  2 วิธี คือ หาตาม user หรือหาตาม costcenter 
                                            traveler.cost_center = item.cost_center;

                                            apprListCAP.Add(traveler);
                                        }
                                        if (bBreakLine == true) { break; }

                                    }
                                    else
                                    {
                                        //DevFix 20210723 เพิ่มผู็อนุมัติลำดับก่อนหน้า เช่น ถ้ายอดเงินที่ต้องให้ EVP อนุมัติ ต้องให้ SH อนุมัติก่อน 
                                        // SH คือ line ไม่ต้องเพิ่มใน CAP
                                        if (cap.EMP_POSITION != "SH")
                                        {
                                            TravelerApproverConditionModel traveler = new TravelerApproverConditionModel();
                                            traveler = new TravelerApproverConditionModel();
                                            traveler.emp_id = item.emp_id;
                                            traveler.total_expen = item.total_expen;
                                            traveler.budget_limit = cap.BUDGET_LIMIT;
                                            traveler.appr_position = cap.EMP_POSITION;
                                            traveler.appr_type = cap.APPR_TYPE;
                                            traveler.doc_type = cap.DOC_TYPE;
                                            traveler.cost_center = item.cost_center;
                                            apprListCAP.Add(traveler);
                                        }
                                    }
                                }
                            }


                            var traverleremp_id_bef = "";
                            var traverler_id_aff = "";
                            var iapprove_level = 1;
                            foreach (var item in apprListCAP)
                            {
                                traverler_id_aff = item.emp_id;
                                if (traverler_id_aff != traverleremp_id_bef)
                                {
                                    traverleremp_id_bef = traverler_id_aff;
                                    iapprove_level = 1;
                                }

                                TravelerSummaryResultModel resultModel = new TravelerSummaryResultModel();
                                var appr_id = "";
                                var apprData = usersList.SingleOrDefault(a => a.EMPLOYEEID == item.emp_id);

                                try
                                {
                                    if (item.appr_position == "SH")
                                    {
                                        appr_id = apprData.SH;
                                    }
                                    else if (item.appr_position == "VP")
                                    {
                                        appr_id = apprData.VP;
                                    }
                                    else if (item.appr_position == "AEP")
                                    {
                                        appr_id = apprData.AEP;
                                    }
                                    else if (item.appr_position == "EVP")
                                    {
                                        appr_id = apprData.EVP;
                                    }
                                    else if (item.appr_position == "SEVP")
                                    {
                                        appr_id = !string.IsNullOrEmpty(apprData.SEVP) ? apprData.SEVP : !string.IsNullOrEmpty(apprData.CEO) ? apprData.CEO : "";
                                    }
                                    else if (item.appr_position == "CEO")
                                    {
                                        appr_id = apprData.CEO;
                                    }
                                }
                                catch { continue; }
                                item.appr_id = appr_id;

                                var usersDetail = usersList.SingleOrDefault(a => a.EMPLOYEEID == item.emp_id);
                                var usersApprDetail = usersList.SingleOrDefault(a => a.EMPLOYEEID == item.appr_id);
                                //DevFix 20200828 2140 กรณีที่เป็น Approve และ Approve type เดียวกัน ไม่ต้อง add ซ้ำ
                                var check_data = data.SingleOrDefault(a => a.appr_id == item.appr_id && a.type == (item.appr_type == "LINE" ? "1" : "2")
                                     && a.emp_id == item.emp_id);
                                if (usersApprDetail != null && check_data == null)
                                {
                                    resultModel.line_id = "1";
                                    resultModel.type = item.appr_type == "LINE" ? "1" : "2";
                                    resultModel.emp_id = usersDetail.EMPLOYEEID;
                                    resultModel.emp_name = usersDetail != null ? usersDetail.ENTITLE + " " + usersDetail.ENFIRSTNAME + " " + usersDetail.ENLASTNAME : "";
                                    resultModel.emp_org = usersDetail != null ? usersDetail.ORGNAME : "";
                                    resultModel.appr_id = usersApprDetail.EMPLOYEEID;
                                    resultModel.appr_name = usersApprDetail.ENTITLE + " " + usersApprDetail.ENFIRSTNAME + " " + usersApprDetail.ENLASTNAME;
                                    resultModel.appr_org = usersApprDetail.ORGNAME;
                                    resultModel.remark = item.appr_type == "LINE" ? "Endorsed" : "CAP";

                                    //DevFix 20210810 0000 approve level ตามลำดับได้เลย เรียงตาม traverler id
                                    resultModel.approve_level = iapprove_level.ToString();
                                    iapprove_level += 1;

                                    data.Add(resultModel);
                                }

                            }

                        }

                    }

                    var data_def = new List<TravelerSummaryResultModel>();
                    if (true)
                    {
                        foreach (var item in dataApporver_not_active)
                        {
                            //add Approver ที่เคย Action ไปแล้ว แต่ไม่มีใน list ที่ต้องคำนวณใหม่ ให้ดึงข้อมูลใน db มาได้เลย
                            var usersDetail = usersList.SingleOrDefault(a => a.EMPLOYEEID == item.emp_id);
                            if (usersDetail != null)
                            {
                                var usersApprDetail = usersList.SingleOrDefault(a => a.EMPLOYEEID == item.appr_id);
                                TravelerSummaryResultModel resultModel = new TravelerSummaryResultModel();
                                resultModel.line_id = item.appr_type == "1" ? "1" : "2";
                                resultModel.type = item.appr_type;
                                resultModel.emp_id = usersDetail.EMPLOYEEID;
                                resultModel.emp_name = usersDetail.ENTITLE + " " + usersDetail.ENFIRSTNAME + " " + usersDetail.ENLASTNAME;
                                resultModel.emp_org = usersDetail.ORGNAME;
                                resultModel.appr_id = usersApprDetail != null ? usersApprDetail.EMPLOYEEID : "";
                                resultModel.appr_name = usersApprDetail.ENTITLE + " " + usersApprDetail.ENFIRSTNAME + " " + usersApprDetail.ENLASTNAME;
                                resultModel.appr_org = usersApprDetail != null ? usersApprDetail.ORGNAME : "";
                                resultModel.remark = item.appr_type == "1" ? "Endorsed" : "CAP";

                                resultModel.approve_status = item.approve_status;
                                resultModel.approve_remark = item.approve_remark;
                                resultModel.approve_action = "true";
                                //DevFix 20210810 0000 approve level ตามลำดับได้เลย เรียงตาม traverler id
                                resultModel.approve_level = item.approve_level;
                                data_def.Add(resultModel);
                            }
                        }

                        //กรณีที่ไม่ใช่ emp ที่ active ไม่ต้องนำไปคำนวณใหม่ ให้ดึงของเดิมมาจาก table 
                        foreach (var item in data)
                        {
                            var check_active = temp_active.SingleOrDefault(a => a.emp_id == item.emp_id);
                            if (check_active == null)
                            {
                                continue;
                            }
                            if (check_active.emp_id.ToString() != "")
                            {
                                //กรณีที่เป็นการคำนวณใหม่และมีข้อมูลเดิมอยู่แล้วให้ยึด approver level จากของเดิมมาตั้งต้น
                                var approve_level = item.approve_level;
                                if (item.type == "2")
                                {
                                    if (dataApporver_in_active != null)
                                    {
                                        try
                                        {
                                            var check_active_new = data_def.SingleOrDefault(a => a.emp_id == item.emp_id && a.appr_id == item.appr_id.ToString()
                                            && a.type == "2");
                                            if (check_active_new != null)
                                            {
                                                if (check_active_new.emp_id.ToString() != "")
                                                {
                                                    continue;//กรณีที่เป็นคนเดียวกันไม่ต้อง add เพิ่ม
                                                }
                                            }
                                        }
                                        catch { }
                                    }
                                    if (approve_level == "")
                                    {
                                        var check_active_new = dataMaxApprover_Level.Where(a => a.emp_id == item.emp_id).ToList();
                                        if (check_active_new != null && check_active_new.Count > 0)
                                        {
                                            if (check_active_new[0].emp_id.ToString() != "")
                                            {
                                                approve_level = check_active_new[0].approve_level;
                                                check_active_new[0].approve_level = (Convert.ToInt32(approve_level) + 1).ToString();
                                            }
                                        }
                                        else
                                        {
                                            //กรณีที่ยังไม่เคยมีข้อมูลใน db
                                            approve_level = "1";
                                            dataMaxApprover_Level.Add(new TravelerApproverConditionModel()
                                            {
                                                emp_id = item.emp_id,
                                                approve_level = approve_level,
                                            });
                                        }
                                    }
                                }

                                TravelerSummaryResultModel resultModel = new TravelerSummaryResultModel();
                                resultModel.line_id = item.line_id;
                                resultModel.type = item.type;
                                resultModel.emp_id = item.emp_id;
                                resultModel.emp_name = item.emp_name;
                                resultModel.emp_org = item.emp_org;
                                resultModel.appr_id = item.appr_id;
                                resultModel.appr_name = item.appr_name;
                                resultModel.appr_org = item.appr_org;
                                resultModel.remark = item.remark;

                                resultModel.approve_status = item.approve_status;
                                resultModel.approve_remark = item.approve_remark;
                                resultModel.approve_action = item.approve_action;

                                //DevFix 20210810 0000 approve level ตามลำดับได้เลย เรียงตาม traverler id
                                resultModel.approve_level = approve_level;
                                data_def.Add(resultModel);
                            }
                        }

                        if (dataApporver_in_active != null)
                        {
                            var iapprove_level = 1;
                            foreach (var item in dataApporver_in_active)
                            {
                                //ไม่เอา approver cap ที่ได้ก่อนหน้านี้
                                var check_active = data_def.SingleOrDefault(a => a.emp_id == item.emp_id && a.appr_id == item.appr_id.ToString() && a.type == "2");
                                if (check_active == null)
                                {
                                    var check_approve_level = data_def.Where(a => a.emp_id == item.emp_id && a.type == "2").ToList();
                                    if (check_approve_level != null && check_approve_level.Count > 0)
                                    {
                                        var i1 = 0;
                                        foreach (var j in check_approve_level)
                                        {
                                            if (Convert.ToInt32(j.approve_level) > i1)
                                            {
                                                i1 = Convert.ToInt32(j.approve_level);
                                            }
                                        }
                                        iapprove_level = i1 + 1;
                                    }

                                    TravelerSummaryResultModel resultModel = new TravelerSummaryResultModel();
                                    resultModel.line_id = "2";
                                    resultModel.type = item.appr_type;
                                    resultModel.emp_id = item.emp_id;
                                    var usersDetail = usersList.SingleOrDefault(a => a.EMPLOYEEID == item.emp_id);
                                    resultModel.emp_name = usersDetail != null ? usersDetail.ENTITLE + " " + usersDetail.ENFIRSTNAME + " " + usersDetail.ENLASTNAME : "";
                                    resultModel.emp_org = usersDetail != null ? usersDetail.ORGNAME : "";

                                    resultModel.appr_id = item.appr_id;
                                    var usersApprDetail = usersList.SingleOrDefault(a => a.EMPLOYEEID == item.appr_id);
                                    resultModel.appr_name = usersApprDetail.ENTITLE + " " + usersApprDetail.ENFIRSTNAME + " " + usersApprDetail.ENLASTNAME;
                                    resultModel.appr_org = usersApprDetail.ORGNAME;
                                    resultModel.remark = "CAP";

                                    resultModel.approve_status = item.approve_status;
                                    resultModel.approve_remark = item.approve_remark;
                                    resultModel.approve_action = "true";// item.approve_status  == "1" ? "true" : "false";

                                    //DevFix 20210810 0000 approve level ตามลำดับได้เลย เรียงตาม traverler id
                                    resultModel.approve_level = iapprove_level.ToString();
                                    data_def.Add(resultModel);

                                }
                            }
                        }

                        return data_def;
                    }

                }


            }
            catch (Exception ex)
            {
                var x = ex.Message.ToString();
            }

            return data;
        }


    }
}