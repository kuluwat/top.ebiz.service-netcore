﻿
using System.Data;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;
using top.ebiz.service.Models;

using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace top.ebiz.service.Service
{
    public class searchDocService
    {
        public List<SearchDocumentResultModel> searchDocument(SearchDocumentModel value)
        {
            string sql = "";
            var data = new List<SearchDocumentResultModel>();
            string p_status = "-1";
            string p_country_id = "-1";
            if (!string.IsNullOrEmpty((value.status ?? "").Trim()))
                p_status = value.status ?? "";

            if (!string.IsNullOrEmpty((value.country_id ?? "").Trim()))
                p_country_id = value.country_id ?? "";

            string start_date = "";
            string stop_date = "";
            if (value.business != null)
            {
                if (!string.IsNullOrEmpty(value.business.start) && value.business.start.Length >= 10)
                    start_date = value.business.start.Substring(0, 10);

                if (!string.IsNullOrEmpty(value.business.stop) && value.business.stop.Length >= 10)
                    stop_date = value.business.stop.Substring(0, 10);
            }

            using (TOPEBizEntities context = new TOPEBizEntities())
            {
                string user_id = "";
                string user_admin = "";
                var login_empid = new List<SearchUserModel>();
                var pmdv_admin_list = new List<SearchUserModel>();
                string pmdv_admin = "false";

                sql = "SELECT  a.USER_NAME, a.user_id, to_char(u.ROLE_ID) user_role ";
                sql += "FROM bz_login_token a left join bz_users u on a.user_id=u.employeeid ";
                sql += " WHERE a.TOKEN_CODE ='" + value.token_login + "' ";
                login_empid = context.Database.SqlQuery<SearchUserModel>(sql).ToList();
                if (login_empid != null && login_empid.Count() > 0)
                {
                    user_id = login_empid[0].user_id ?? "";
                    if ((login_empid[0].user_role ?? "") == "1")
                    {
                        user_admin = "admin";
                    }
                    else
                    {
                        sql = "select emp_id from bz_data_manage where pmdv_admin = 'true' and emp_id = '" + user_id + "'";
                        pmdv_admin_list = context.Database.SqlQuery<SearchUserModel>(sql).ToList();
                        if (pmdv_admin_list != null && pmdv_admin_list.Count() > 0)
                        {
                            user_admin = "admin";
                            pmdv_admin = "true";
                        }
                        else { user_admin = user_id; }
                    }
                }

                using (var connection = context.Database.GetDbConnection())
                {
                    var keyword = value.keyword;
                    try { keyword = keyword.Trim(); } catch { }
                    connection.Open();
                    DbCommand cmd = connection.CreateCommand();
                    //cmd.CommandText = "bz_sp_get_document";
                    cmd.CommandText = "bz_sp_get_document2";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("p_token", value.token_login ?? ""));
                    cmd.Parameters.Add(new OracleParameter("p_userid", user_id ?? ""));
                    cmd.Parameters.Add(new OracleParameter("p_userid_action", user_admin ?? ""));
                    cmd.Parameters.Add(new OracleParameter("p_type", value.type ?? ""));
                    cmd.Parameters.Add(new OracleParameter("p_status", p_status));
                    cmd.Parameters.Add(new OracleParameter("p_country_id", p_country_id));
                    cmd.Parameters.Add(new OracleParameter("p_keyword", keyword ?? ""));
                    cmd.Parameters.Add(new OracleParameter("p_date_start", start_date));
                    cmd.Parameters.Add(new OracleParameter("p_date_stop", stop_date));
                    cmd.Parameters.Add(new OracleParameter("p_pmdv_admin", pmdv_admin));

                    OracleParameter oraP = new OracleParameter();
                    oraP.ParameterName = "mycursor";
                    oraP.OracleDbType = OracleDbType.RefCursor;
                    oraP.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(oraP);

                    using (var reader = cmd.ExecuteReader())
                    {
                        try
                        {
                            var schema = reader.GetSchemaTable();
                            data = reader.MapToList<SearchDocumentResultModel>() ?? new List<SearchDocumentResultModel>();
                        }
                        catch (Exception ex) { }
                    }
                    if (data != null)
                    {
                        //DevFix 20211014 0000 ข้อมูล person ต้องไม่รวมจำนวน user ที่ถูก reject ???
                        sql = @"select  to_char(count(dte_emp_id)) as dh_doc_status, to_char(dh_code) as dh_code from(
                                     select distinct dte_emp_id, dh_code from bz_doc_traveler_expense
                                     where (nvl(dte_appr_status,1) not in (30) or nvl(dte_cap_appr_status,1) not in (40))
                                     and nvl(dte_cap_appr_status,1) not in (40)
                                     and(nvl(dte_appr_opt, 'true') = 'true' and nvl(dte_cap_appr_opt, 'true') = 'true')
                                     )t group by dh_code";
                        var dh_person = new List<StatusDocModel>();
                        dh_person = context.Database.SqlQuery<StatusDocModel>(sql).ToList();
                        if (dh_person != null) { }

                        foreach (var item in data)
                        {

                            //pmdv หน้า tracking

                            var dh_status = new List<StatusDocModel>();
                            // แก้ไข status หน้า tracking เนื่องจากมีการณี 1 ใบงานมีหลายสถานะที่คาบเกี่ยวกัน ยึดจาก status ใบงานไม่ได้
                            sql = @"  select distinct 'true' as dh_doc_status
                                      from  BZ_DOC_HEAD h
                                      inner join BZ_DOC_TRAVELER_EXPENSE a on  h.dh_code = a.dh_code  
                                      where substr(h.dh_doc_status,0,1) in ( 3,4,5)
                                      and a.dte_appr_status  in( 23, 32, 42)
                                      and h.dh_code in ('" + item.doc_id + "')";
                            sql = @"  select distinct 'true' as dh_doc_status
                                     from  BZ_DOC_HEAD h
                                     inner join BZ_DOC_TRAVELER_EXPENSE a on  h.dh_code = a.dh_code  
                                     inner join BZ_DOC_TRAVELER_APPROVER b on h.dh_code = b.dh_code  
                                     where b.dta_action_status = 4
                                     and h.dh_code in ('" + item.doc_id + "')";
                            dh_status = context.Database.SqlQuery<StatusDocModel>(sql).ToList();
                            if (dh_status != null)
                            {
                                if (dh_status.Count > 0)
                                {
                                    if (dh_status[0].dh_doc_status.ToString() == "true")
                                    {
                                        item.button_status = "4";
                                    }
                                }
                            }

                            #region DevFix 20210921 0000 แก้ไขกรณีที่มีการยกเลิอก trip จาก phase2    
                            item.status_trip_cancelled = "false";
                            sql = " select distinct nvl(STATUS_TRIP_CANCELLED,'false') as dh_doc_status from BZ_DOC_TRAVELEXPENSE where doc_id = '" + item.doc_id + "' ";
                            dh_status = context.Database.SqlQuery<StatusDocModel>(sql).ToList();
                            if (dh_status != null)
                            {
                                if (dh_status.Count > 0)
                                {
                                    item.status_trip_cancelled = dh_status[0].dh_doc_status.ToString();
                                }
                            }
                            #endregion DevFix 20210921 0000 แก้ไขกรณีที่มีการยกเลิอก trip จาก phase2    

                            if (dh_person != null)
                            {
                                try
                                {
                                    var check_data = dh_person.Where(t => t.dh_code == item.doc_id).ToList();
                                    item.person = check_data[0].dh_doc_status.ToString() + " Traveler";
                                }
                                catch { }
                            }

                        }
                    }

                }
            }

            var data2 = new List<SearchDocumentResultModel>();
            if (data != null)
            {
                string temp_doc_no = "";
                foreach (var item in data)
                {
                    if (temp_doc_no != item.doc_id)
                    {
                        temp_doc_no = item.doc_id;
                        data2.Add(item);
                    }
                    else
                    {
                        data2[data2.Count() - 1].place += ", " + item.place;
                    }
                }

                if (data2.Count > 50)
                {
                    var data3 = new List<SearchDocumentResultModel>();
                    int irow_count = 50;// data2.Count - 50;
                    int irows = 0;
                    foreach (var item in data2)
                    {

                        //if (irow_count > irows) { irows++; continue; }
                        temp_doc_no = item.doc_id;
                        data3.Add(item);
                        irows++;
                        if (irows > irow_count) { break; }
                    }
                    data2 = new List<SearchDocumentResultModel>();
                    data2 = data3;
                }
            }

            return data2;
        }

        public DocDetailModel searchDetail(DocDetailSearchModel value)
        {
            var data = new DocDetailModel();
            var docHead = new List<DocHeadModel>();
            var docCheckTab = new List<DocList2Model>();
            var travelType = new List<BZ_DOC_TRAVEL_TYPE>();
            var continent = new List<ContinentDocModel>();
            var country = new List<CountryDocModel>();
            var province = new List<ProvinceDocModel>();
            var traveler = new List<travelerDocModel>();

            string user_id = "";
            string user_role = "";
            //DevFix 20210622 0000 เพิ่มข้อมูล ประเภทพนักงาน 1:Employee, 2:Contract
            string user_type = "2";
            try
            {
                using (TOPEBizEntities context = new TOPEBizEntities())
                {
                    string sql = "";

                    var docHeadStatus = new List<DocHeadModel>();
                    sql = " select to_char(dh_doc_status) as document_status from bz_doc_head h where h.dh_code = '" + value.id_doc + "' ";
                    docHeadStatus = context.Database.SqlQuery<DocHeadModel>(sql).ToList();



                    var login_empid = new List<SearchUserModel>();
                    sql = "SELECT  a.USER_NAME, a.user_id, to_char(u.ROLE_ID) user_role ";
                    //DevFix 20210622 0000 เพิ่มข้อมูล ประเภทพนักงาน
                    sql += " ,u.usertype as user_type";
                    sql += " FROM bz_login_token a left join bz_users u on a.user_id=u.employeeid ";
                    sql += " WHERE a.TOKEN_CODE ='" + value.token_login + "' ";
                    login_empid = context.Database.SqlQuery<SearchUserModel>(sql).ToList();
                    if (login_empid != null && login_empid.Count() > 0)
                    {
                        user_id = login_empid[0].user_id ?? "";
                        user_role = login_empid[0].user_role ?? "";
                        user_type = login_empid[0].user_type ?? "";
                    }

                    //หาว่า  type นี้เป็น oversea หรือ local
                    sql = " select DH_CODE , DH_TYPE type, DH_EXPENSE_OPT1 checkbox_1, DH_EXPENSE_OPT2 checkbox_2, DH_EXPENSE_REMARK remark " + Environment.NewLine;
                    sql += ", to_char(DH_DOC_STATUS) doc_status ";
                    sql += " , b.TS_NAME document_status, a.DH_TYPE_FLOW ";
                    sql += " from BZ_DOC_HEAD a left join BZ_MASTER_STATUS b on a.DH_DOC_STATUS=b.TS_ID ";
                    sql += " WHERE DH_CODE = '" + value.id_doc + "' " + Environment.NewLine;
                    docCheckTab = context.Database.SqlQuery<DocList2Model>(sql).ToList();
                    if (docCheckTab != null) { }

                    sql = " select a.DTE_EMP_ID emp_id, nvl(b.ENTITLE, '')|| ' ' || b.ENFIRSTNAME || ' ' || b.ENLASTNAME emp_name ";
                    sql += "  ,  b.ORGNAME emp_organization ";
                    sql += " , to_char(a.CTN_ID)continent_id, c.CTN_NAME continent, a.CITY_TEXT city ";
                    sql += " , to_char(a.CT_ID) country_id, d.CT_NAME country_name ";
                    sql += " , to_char(e.PV_ID) province_id, e.PV_NAME province_name ";
                    sql += "  , case when a.DTE_BUS_FROMDATE is null then '' else to_char(a.DTE_BUS_FROMDATE, 'YYYY-MM-DD') end business_date_start ";
                    sql += "  , case when a.DTE_BUS_TODATE is null then '' else to_char(a.DTE_BUS_TODATE, 'YYYY-MM-DD') end business_date_stop ";
                    sql += "  , case when a.DTE_TRAVEL_FROMDATE is null then '' else to_char(a.DTE_TRAVEL_FROMDATE, 'YYYY-MM-DD') end travel_date_start ";
                    sql += "  , case when a.DTE_TRAVEL_TODATE is null then '' else to_char(a.DTE_TRAVEL_TODATE, 'YYYY-MM-DD') end travel_date_stop ";
                    sql += " , a.DTE_GL_ACCOUNT gl_account, a.DTE_COST_CENTER cost, a.DTE_ORDER_WBS order_wbs, a.DTE_TRAVELER_REMARK remark ";
                    //DevFix 20210714 0000 เพิ่มสถานะที่ Line/CAP --> 1:Draft , 2:Pendding , 3:Approve , 4:Revise , 5:Reject , 6:Not Active
                    sql += " , '' as approve_status";
                    sql += " , '' as approve_remark";
                    //DevFix 20210719 0000 เพิ่ม field OPT
                    sql += " , '' as approve_opt";
                    //sql += " , a.dte_appr_remark as remark_opt";
                    if (docCheckTab[0].doc_status.ToString().Substring(0, 1) == "4" ||
                       docCheckTab[0].doc_status.ToString().Substring(0, 1) == "5")
                    {
                        if (docCheckTab[0].doc_status.ToString() == "41")
                        {
                            sql += " , a.dte_appr_remark as remark_opt";
                        }
                        else
                        {
                            sql += " , case when a.dte_appr_opt = 'false' then a.dte_appr_remark else a.dte_cap_appr_remark end remark_opt";
                        }
                    }
                    else
                    {
                        sql += " , a.dte_appr_remark as remark_opt";
                    }
                    sql += " , a.dte_cap_appr_remark as remark_cap";

                    //DevFix 20210817 เพิ่ม key traveler_ref_id เพื่อใช้ในการแยกข้อมูลออกแต่ละรายการ เนื่องจากเงื่อนไขเดิมข้อมูลซ้ำ --> เก็บค่าเป็น token id
                    //เนื่องจากไม่สามารถ up dataset model ได้ให้ใช้ DTE_TOKEN แทน
                    sql += " , to_char(DTE_TOKEN) as traveler_ref_id";

                    sql += "  from BZ_DOC_TRAVELER_EXPENSE a left join bz_users b on a.DTE_EMP_ID = b.employeeid  ";
                    sql += " left join BZ_MASTER_CONTINENT c on a.CTN_ID = c.CTN_ID  ";
                    sql += " left join BZ_MASTER_COUNTRY d on a.CT_ID = d.CT_ID ";
                    sql += " left join BZ_MASTER_PROVINCE e on a.PV_ID = e.PV_ID ";
                    sql += " where a.DH_CODE = '" + value.id_doc + "' and a.dte_status = 1 ";
                    sql += "  order by DTE_ID ";
                    traveler = context.Database.SqlQuery<travelerDocModel>(sql).ToList();
                    if (traveler != null)
                    {
                        #region DevFix 20210714 0000 ดึงข้อมูลรายละเอียด approver เดิม

                        var pf_doc_id = docHeadStatus[0].document_status.Substring(0, 1);
                        var bCheckPF_CAP = true;//กรณีที่ Line submit to CAP แต่ CAP ยังไม่ได้ active
                        sql = @" select to_char(count(1)) as approve_status
                                from BZ_DOC_TRAVELER_APPROVER a
                                where dta_action_status >  2 and a.dta_type = 2 and dh_code =  '" + value.id_doc + "'  ";
                        var dataCheck_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                        if (dataCheck_Def != null)
                        {
                            if (dataCheck_Def.Count > 0) { if (dataCheck_Def[0].approve_status.ToString() == "0") { bCheckPF_CAP = false; } }
                        }
                        sql = @" select dta_appr_level,dta_travel_empid as emp_id, (a.dta_action_status) as approve_status,  a.dta_appr_remark as approve_remark
                                ,to_char(nvl(a.dta_appr_status,'true')) as  approve_opt
                                from BZ_DOC_TRAVELER_APPROVER a 
                                where  dh_code =  '" + value.id_doc + "'  ";
                        if (pf_doc_id == "3")
                        {
                            sql += @" and a.dta_type = 1";
                        }
                        else if (pf_doc_id == "4")
                        {
                            if (bCheckPF_CAP == true) { sql += @" and a.dta_type = 2 and dta_action_status not in ('6') "; } else { sql += @" and a.dta_type = 1"; }
                        }
                        sql += @" order by dta_appr_level ";
                        var dataApporver_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                        #region DevFix 20211013 0000 กรณีที่เป็น step cap ให้ตรวจสอบ line ว่ามีการ reject หรือไม่
                        var dataApporverLine_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                        var dataApporverLine2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                        var dataApporverLine3_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                        var dataApporverCAP_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                        var dataApporverCAP2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                        var dataApporverCAP3_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                        var dataApporverRevise_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                        var check_status_approver_line = false;
                        if (pf_doc_id == "3" || pf_doc_id == "4" || pf_doc_id == "5")
                        {
                            //line approve
                            sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '3' as approve_status, dte_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where dte_appr_opt = 'true' and dte_status = 1 and dte_appr_status <> 23
                                         and dh_code = '" + value.id_doc + "' ";
                            dataApporverLine_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                            ////line reject
                            //sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                            //             , '5' as approve_status, dte_appr_remark as approve_remark
                            //             from BZ_DOC_TRAVELER_EXPENSE a 
                            //             where ((dte_appr_opt = 'false' and dte_status = 1) or dte_appr_status = 30 )
                            //             and dh_code = '" + value.id_doc + "' ";
                            //dataApporverLine2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                            ////line pendding
                            //sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                            //             , '2' as approve_status, dte_appr_remark as approve_remark
                            //             from BZ_DOC_TRAVELER_EXPENSE a 
                            //             where dte_status = 1 and dte_appr_status = 31 and dh_code = '" + value.id_doc + "' ";
                            //dataApporverLine3_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                            ////cap approve
                            //sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                            //             , '3' as approve_status, dte_cap_appr_remark as approve_remark
                            //             from BZ_DOC_TRAVELER_EXPENSE a 
                            //             where nvl(dte_cap_appr_status,41) = '42' and (dte_cap_appr_opt = 'true' and dte_appr_opt = 'true') and dte_status = 1
                            //             and dh_code = '" + value.id_doc + "' ";
                            //dataApporverCAP_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                            ////cap reject
                            //sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                            //             , '5' as approve_status, dte_cap_appr_remark as approve_remark
                            //             from BZ_DOC_TRAVELER_EXPENSE a 
                            //             where nvl(dte_cap_appr_status,41) = '42' and ( (dte_cap_appr_opt = 'false' and dte_status = 1) or (dte_appr_opt = 'false' and dte_appr_status = 32) or dte_cap_appr_status = 40 )
                            //             and dh_code = '" + value.id_doc + "' ";
                            //dataApporverCAP2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();


                            //line reject
                            sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '5' as approve_status, dte_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where ((dte_appr_opt = 'false' and dte_status = 1) or dte_appr_status = 30 )
                                         and dh_code = '" + value.id_doc + "' ";
                            dataApporverLine2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                            //line pendding
                            sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '2' as approve_status, dte_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where dte_status = 1 and dte_appr_status = 31 and dh_code = '" + value.id_doc + "' ";
                            dataApporverLine3_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                            //cap approve
                            sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '3' as approve_status, dte_cap_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where nvl(dte_cap_appr_status,41) = '42' and (dte_cap_appr_opt = 'true' and dte_appr_opt = 'true') and dte_status = 1
                                         and dh_code = '" + value.id_doc + "' ";

                            //cap reject
                            sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '5' as approve_status, dte_cap_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where nvl(dte_cap_appr_status,41) = '42' and ( (dte_cap_appr_opt = 'false' and dte_status = 1) or (dte_appr_opt = 'false' and dte_appr_status = 32) or dte_cap_appr_status = 40 )
                                         and dh_code = '" + value.id_doc + "' ";
                            sql += "         union ";
                            sql += @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '5' as approve_status, dte_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where dte_status = 1  
                                         and ( dte_cap_appr_status = 23 and a.dte_cap_appr_opt = 'false')  
                                         and dh_code = '" + value.id_doc + "'";

                            dataApporverCAP2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                            //cap pendding
                            sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '2' as approve_status, dte_cap_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where dte_status = 1 and (dte_cap_appr_status = 41 or (dte_cap_appr_status is null and  dte_appr_status = 32 and dte_appr_opt = 'true' ) )
                                         and dh_code = '" + value.id_doc + "' ";
                            dataApporverCAP3_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                            //line/CAP revise
                            sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '4' as approve_status, dte_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where dte_status = 1 and (dte_appr_status = 23 or dte_cap_appr_status = 23)
                                         and dh_code = '" + value.id_doc + "' ";
                            dataApporverRevise_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                        }
                        #endregion DevFix 20211013 0000 กรณีที่เป็น step cap ให้ตรวจสอบ line ว่ามีการ reject หรือไม่

                        #endregion DevFix 20210714 0000 ดึงข้อมูลรายละเอียด approver เดิม

                        foreach (var item in traveler)
                        {
                            var b_date = new DateModel();
                            b_date.start = item.business_date_start;
                            b_date.stop = item.business_date_stop;
                            var t_date = new DateModel();
                            t_date.start = item.travel_date_start;
                            t_date.stop = item.travel_date_stop;

                            var approve_status = "1";
                            var approve_remark = "";
                            var approve_opt = "";
                            var check_data = dataApporver_Def.Where(t => t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id);

                            #region DevFix 20211013 0000 กรณีที่เป็น step cap ให้ตรวจสอบ line ว่ามีการ reject หรือไม่ 
                            if (pf_doc_id == "4" || pf_doc_id == "5")
                            {
                                try
                                {
                                    if (bCheckPF_CAP == true)
                                    {
                                        check_data = dataApporverCAP_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                        if (check_data.Count() == 0)
                                        {
                                            check_data = dataApporverCAP2_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                            if (check_data.Count() == 0)
                                            {
                                                check_data = dataApporverCAP3_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                                if (check_data.Count() == 0)
                                                {
                                                    check_data = dataApporverRevise_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                                    if (check_data.Count() == 0)
                                                    {
                                                        //กรณีที่มีข้อมูล line ให้เอา remark line มาแสดง
                                                        check_data = dataApporverLine2_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                                        if (check_data.Count() == 0)
                                                        {
                                                            check_data = dataApporverLine_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                                        }
                                                        check_status_approver_line = false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        check_data = dataApporverLine2_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                        if (check_data.Count() == 0)
                                        {
                                            check_data = dataApporverLine3_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));

                                            check_data = dataApporverRevise_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                            if (check_data.Count() == 0)
                                            {
                                                //เฉพาะ tab 1 and 2
                                                if (check_data.Count() == 0)
                                                {
                                                    check_data = dataApporverCAP3_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                                }
                                            }
                                        }
                                    }
                                }
                                catch { }
                            }

                            if (pf_doc_id == "3" || check_status_approver_line == true)
                            {
                                check_data = dataApporverLine_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                if (check_data.Count() == 0)
                                {
                                    check_data = dataApporverLine2_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                    if (check_data.Count() == 0)
                                    {
                                        check_data = dataApporverRevise_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                        if (check_data.Count() == 0)
                                        {
                                            check_data = dataApporverLine3_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                        }
                                    }
                                }
                            }
                            #endregion DevFix 20211013 0000 กรณีที่เป็น step cap ให้ตรวจสอบ line ว่ามีการ reject หรือไม่
                            var bcheck_change_status = false;
                            foreach (var item2 in check_data)
                            {
                                approve_status = item2.approve_status;
                                approve_opt = item2.approve_opt;

                                if (approve_remark != "") { approve_remark += ","; }
                                if (item2.approve_remark != "") { approve_remark += item2.approve_remark; }
                                bcheck_change_status = true;
                            }
                            if (approve_status == "" || bcheck_change_status == false) { approve_status = item.approve_status; }
                            if (approve_remark == "") { approve_remark = item.approve_remark; }

                            data.summary_table.Add(new Traveler
                            {
                                emp_id = item.emp_id ?? "",
                                emp_name = item.emp_name ?? "",
                                emp_organization = item.emp_organization ?? "",
                                continent_id = item.continent_id ?? "",
                                continent = item.continent ?? "",
                                country_id = item.country_id ?? "",
                                country_name = item.country_name ?? "",
                                province_id = item.province_id ?? "",
                                province_name = (item.province_name ?? ""), //+ (string.IsNullOrEmpty(item.city) ? "" : "/" + item.city),
                                city = item.city ?? "",
                                business_date = b_date,
                                travel_date = t_date,
                                gl_account = item.gl_account ?? "",
                                cost = item.cost ?? "",
                                order = item.order_wbs ?? "",
                                remark = item.remark ?? "",

                                //DevFix 20210714 0000 เพิ่มสถานะที่ Line/CAP --> 1:Draft , 2:Pendding , 3:Approve , 4:Revise , 5:Reject 
                                //0 กับ 3 แก้ไขได้
                                approve_status = approve_status ?? "",
                                approve_remark = approve_remark ?? "",
                                approve_opt = approve_opt ?? "",
                                remark_opt = item.remark_opt ?? "",
                                remark_cap = item.remark_cap ?? "",

                                //DevFix 20210817 เพิ่ม key เพื่อใช้ในการแยกข้อมูลออกแต่ละรายการ เนื่องจากเงื่อนไขเดิมข้อมูลซ้ำ --> เก็บค่าเป็น token id
                                traveler_ref_id = item.traveler_ref_id

                            });


                        }


                    }

                    sql = " select h.DH_TYPE, h.DH_BEHALF_EMP_ID, h.DH_COM_CODE, h.DH_TOPIC, h.DH_CITY ";
                    sql += ", h.DH_TRAVEL_OBJECT, h.DH_REMARK, h.DH_TRAVEL ";
                    sql += ", case when h.DH_BUS_FROMDATE is null then '' else to_char(h.DH_BUS_FROMDATE, 'YYYY-MM-DD') end bus_start ";
                    sql += ", case when h.DH_BUS_TODATE is null then '' else to_char(h.DH_BUS_TODATE, 'YYYY-MM-DD') end bus_stop ";
                    sql += ", case when h.DH_TRAVEL_FROMDATE is null then '' else to_char(h.DH_TRAVEL_FROMDATE, 'YYYY-MM-DD') end travel_start ";
                    sql += ", case when h.DH_TRAVEL_TODATE is null then '' else to_char(h.DH_TRAVEL_TODATE, 'YYYY-MM-DD') end travel_stop ";
                    sql += ", (case when nvl(u.ENTITLE,'') = '' then '' else nvl(u.ENTITLE,'') || ' ' end) ||u.ENFIRSTNAME||' '||u.ENLASTNAME ENNAME, u.ORGNAME COMPANYCODE, c.COM_NAME COMPANYNAME,h.DH_INITIATOR_EMPID ";
                    sql += ", (case when nvl(u2.ENTITLE,'') = '' then '' else nvl(u2.ENTITLE,'') || ' ' end)||u2.ENFIRSTNAME||' '||u2.ENLASTNAME INITIATOR_NAME, u2.ORGNAME INITIATOR_COM ,DH_INITIATOR_REMARK";
                    sql += ", s.TS_NAME document_status, h.DH_DOC_STATUS ";
                    sql += ", DH_AFTER_TRIP_OPT1,DH_AFTER_TRIP_OPT2,DH_AFTER_TRIP_OPT3 ,DH_AFTER_TRIP_OPT2_REMARK,DH_AFTER_TRIP_OPT3_REMARK";

                    //DevFix 20210527 0000 เพิ่มข้อมูล ประเภทใบงานเป็น 1:flow, 2:history, 3:training เก็บไว้ที่  BZ_DOC_HEAD.DH_TYPE_FLOW
                    sql += " , nvl(h.DH_TYPE_FLOW,1) as DH_TYPE_FLOW";

                    //DevFix 20210622 0000 เพิ่มข้อมูล ประเภทพนักงาน 1:Employee, 2:Contract
                    sql += ", u3.usertype as REQUEST_USER_TYPE ";
                    sql += ", trim( (case when nvl(u3.ENTITLE,'') = '' then '' else nvl(u3.ENTITLE,'') || ' ' end) || u3.ENFIRSTNAME ||' '||u3.ENLASTNAME) as REQUEST_USER_NAME";

                    sql += " from       bz_doc_head h left join bz_users u on h.DH_BEHALF_EMP_ID=u.employeeid ";
                    sql += "            left join BZ_MASTER_COMPANY c on h.DH_COM_CODE=c.COM_CODE ";
                    sql += "            left join bz_users u2 on h.DH_INITIATOR_EMPID=u2.employeeid ";
                    sql += "            left join bz_users u3 on h.DH_CREATE_BY = u3.employeeid  ";
                    sql += "            left join BZ_MASTER_STATUS s on h.DH_DOC_STATUS=s.TS_ID ";
                    sql += " where      h.dh_code = '" + value.id_doc + "' ";

                    docHead = context.Database.SqlQuery<DocHeadModel>(sql).ToList();
                    if (docHead != null && docHead.Count() > 0)
                    {
                        //DevFix 20210527 0000 เพิ่มข้อมูล ประเภทใบงานเป็น 1:flow, 2:not flow, 3:training เก็บไว้ที่  BZ_DOC_HEAD.DH_TYPE_FLOW
                        data.type_flow = docHead[0].DH_TYPE_FLOW ?? "1";
                        //DevFix 20210622 0000 เพิ่มข้อมูล ประเภทพนักงาน 1:Employee, 2:Contract
                        data.user_type = user_type ?? "";
                        data.request_user_type = docHead[0].REQUEST_USER_TYPE ?? "";

                        //DevFix 20210806 0000 เพิ่มข้อมูล requester name
                        data.requester_emp_name = docHead[0].REQUEST_USER_NAME ?? "";

                        data.document_status = docHead[0].document_status ?? "";
                        //DevFix 20210806 0000 เพิ่มข้อมูล doc status
                        data.doc_status = docHead[0].DH_DOC_STATUS.ToString() ?? "";

                        data.type = docHead[0].DH_TYPE ?? "";
                        data.behalf.emp_id = docHead[0].DH_BEHALF_EMP_ID ?? "";
                        if (!string.IsNullOrEmpty(data.behalf.emp_id))
                        {
                            data.behalf.status = "true";
                            data.behalf.emp_name = docHead[0].ENNAME ?? "";
                            data.behalf.emp_organization = docHead[0].COMPANYCODE ?? "";

                        }
                        else data.behalf.status = "false";

                        data.id_company = docHead[0].DH_COM_CODE ?? "";
                        data.company_name = docHead[0].COMPANYNAME ?? "";
                        data.topic_of_travel = docHead[0].DH_TOPIC ?? "";
                        data.city = docHead[0].DH_CITY ?? "";
                        data.travel_objective_expected = docHead[0].DH_TRAVEL_OBJECT ?? "";
                        data.remark = docHead[0].DH_REMARK ?? "";
                        data.business_date.start = docHead[0].bus_start ?? "";
                        data.business_date.stop = docHead[0].bus_stop ?? "";
                        data.travel_date.start = docHead[0].travel_start ?? "";
                        data.travel_date.stop = docHead[0].travel_stop ?? "";
                        data.initiator.emp_id = docHead[0].DH_INITIATOR_EMPID ?? "";
                        if (!string.IsNullOrEmpty(data.initiator.emp_id))
                        {
                            data.initiator.status = "true";
                            data.initiator.emp_name = docHead[0].INITIATOR_NAME ?? "";
                            data.initiator.emp_organization = docHead[0].INITIATOR_COM ?? "";
                            data.initiator.remark = docHead[0].DH_INITIATOR_REMARK ?? "";
                        }
                        else
                            data.initiator.status = "false";

                        data.travel = docHead[0].DH_TRAVEL ?? "";

                        data.after_trip.opt1 = (docHead[0].DH_AFTER_TRIP_OPT1 ?? "") == "Y" ? "true" : "false";
                        data.after_trip.opt2 = new subAfterTripModel();
                        data.after_trip.opt2.status = (docHead[0].DH_AFTER_TRIP_OPT2 ?? "") == "Y" ? "true" : "false";
                        data.after_trip.opt2.remark = docHead[0].DH_AFTER_TRIP_OPT2_REMARK ?? "";
                        data.after_trip.opt3 = new subAfterTripModel();
                        data.after_trip.opt3.status = (docHead[0].DH_AFTER_TRIP_OPT3 ?? "") == "Y" ? "true" : "false";
                        data.after_trip.opt3.remark = docHead[0].DH_AFTER_TRIP_OPT3_REMARK ?? "";

                        data.action.type = "1";

                        sql = "select * from BZ_DOC_TRAVEL_TYPE where DH_CODE = '" + value.id_doc + "' ";
                        travelType = context.Database.SqlQuery<BZ_DOC_TRAVEL_TYPE>(sql).ToList();
                        data.type_of_travel.meeting = "false";
                        data.type_of_travel.siteVisite = "false";
                        data.type_of_travel.workshop = "false";
                        data.type_of_travel.roadshow = "false";
                        data.type_of_travel.conference = "false";
                        data.type_of_travel.other = "false";
                        //DevFix 20220805 --> after go-live เพิ่ม Tick box = Training 
                        data.type_of_travel.training = "false";

                        if (travelType != null)
                        {
                            foreach (var iTravelType in travelType)
                            {
                                if (iTravelType.DTT_ID == 1)
                                    data.type_of_travel.meeting = "true";
                                else if (iTravelType.DTT_ID == 2)
                                    data.type_of_travel.siteVisite = "true";
                                else if (iTravelType.DTT_ID == 3)
                                    data.type_of_travel.workshop = "true";
                                else if (iTravelType.DTT_ID == 4)
                                    data.type_of_travel.roadshow = "true";
                                else if (iTravelType.DTT_ID == 5)
                                    data.type_of_travel.conference = "true";
                                else if (iTravelType.DTT_ID == 6)
                                {
                                    data.type_of_travel.other = "true";
                                    data.type_of_travel.other_detail = iTravelType.DTT_NOTE ?? "";
                                }
                                //DevFix 20220805 --> after go-live เพิ่ม Tick box = Training 
                                else if (iTravelType.DTT_ID == 7)
                                { data.type_of_travel.training = "true"; }

                            }
                        }

                        sql = "select a.CTN_ID, b.CTN_NAME ";
                        sql += " from BZ_DOC_CONTIENT a left join BZ_MASTER_CONTINENT b on a.CTN_ID=b.CTN_ID ";
                        sql += " where DH_CODE = '" + value.id_doc + "' ";
                        continent = context.Database.SqlQuery<ContinentDocModel>(sql).ToList();
                        if (continent != null)
                        {
                            foreach (var item in continent)
                            {
                                data.continent.Add(new dataListModel
                                {
                                    id = item.CTN_ID.ToString(),
                                    name = item.CTN_NAME ?? ""
                                });
                            }
                        }

                        sql = "select to_char(a.CT_ID) contry_id, b.CT_NAME country_name, to_char(b.CTN_ID) continent_id, c.CTN_NAME continent_name ";
                        sql += " from BZ_DOC_COUNTRY a left join BZ_MASTER_COUNTRY b on a.CT_ID=b.CT_ID ";
                        sql += " left join BZ_MASTER_CONTINENT c on b.CTN_ID=c.CTN_ID ";
                        sql += " where DH_CODE = '" + value.id_doc + "' ";
                        country = context.Database.SqlQuery<CountryDocModel>(sql).ToList();
                        if (country != null)
                        {
                            foreach (var item in country)
                            {
                                data.country.Add(new DocCountry
                                {
                                    contry_id = item.contry_id ?? "",
                                    country_id = item.contry_id ?? "",
                                    country_name = item.country_name ?? "",
                                    continent_id = item.continent_id ?? "",
                                    continent_name = item.continent_name ?? "",

                                });
                            }
                        }

                        sql = "select to_char(b.PV_ID) province_id, b.PV_NAME province_name ";
                        sql += " from BZ_DOC_PROVINCE a left join BZ_MASTER_PROVINCE b on a.PV_ID=b.PV_ID ";
                        sql += " where DH_CODE = '" + value.id_doc + "' ";
                        province = context.Database.SqlQuery<ProvinceDocModel>(sql).ToList();
                        if (province != null)
                        {
                            foreach (var item in province)
                            {
                                data.province.Add(new DocProvince
                                {
                                    province_id = item.province_id ?? "",
                                    province_name = item.province_name ?? ""
                                });
                            }
                        }
                    }

                    data.button.part_i = "false";
                    data.button.part_ii = "false";
                    data.button.part_iii = "false";
                    data.button.part_iiii = "false";
                    data.button.part_cap = "false";

                    string pf_doc_status = docHead[0].DH_DOC_STATUS.ToString().Substring(0, 1);

                    if (docHead[0].DH_DOC_STATUS.ToString() == "22")
                    {
                        data.button.part_i = "true";
                    }
                    else
                    {
                        if (pf_doc_status == "1")
                        {
                            data.button.part_i = "true";
                        }
                        else if (pf_doc_status == "2")
                        {
                            data.button.part_i = "true";
                            data.button.part_ii = "true";
                        }
                        else if (pf_doc_status == "3")
                        {
                            data.button.part_i = "true";
                            data.button.part_ii = "true";
                            data.button.part_iii = "true";
                        }
                        else if (pf_doc_status == "4")
                        {
                            data.button.part_i = "true";
                            data.button.part_ii = "true";
                            data.button.part_iii = "true";
                            data.button.part_iiii = "true";
                            data.button.part_cap = "true";
                        }
                        else if (pf_doc_status == "5")
                        {
                            data.button.part_i = "true";
                            data.button.part_ii = "true";
                            data.button.part_iii = "true";
                            data.button.part_iiii = "true";
                            data.button.part_cap = "true";
                        }
                    }

                    string user_action = user_id;
                    if (user_role == "1")
                        user_action = "admin";

                    var action = context.BZ_DOC_ACTION.Where(p => p.EMP_ID.Equals(user_action) && p.ACTION_STATUS == 1 && p.TAB_NO == 1 && p.DH_CODE.Equals(value.id_doc)).ToList();
                    if (action != null && action.Count() > 0)
                    {
                        data.button.save = "true";
                        data.button.cancel = "true";
                        data.button.reject = "true";
                        data.button.revise = "true";
                        data.button.approve = "true";
                        data.button.submit = "true";
                    }
                    else
                    {
                        data.button.save = "false";
                        data.button.cancel = "false";
                        data.button.reject = "false";
                        data.button.revise = "false";
                        data.button.approve = "false";
                        data.button.submit = "false";
                    }

                    #region DevFix 20211012 0000 กรณีที่เป็น pmdv admin ทำ training ให้ button.save กับ button.approve = true 
                    string doc_status_tab = docHead[0].DH_DOC_STATUS.ToString().Substring(0, 1);
                    string doc_status_chk = docHead[0].DH_DOC_STATUS.ToString();
                    if (doc_status_tab == "5" || doc_status_chk == "10" || doc_status_chk == "20" || doc_status_chk == "30" || doc_status_chk == "40" || doc_status_chk == "50") { }
                    else
                    {
                        if (value.id_doc.IndexOf("T") > -1 && doc_status_tab == "1")
                        {
                            data.button.approve = "false";
                            data.button.cancel = "false";
                            data.button.reject = "false";
                            data.button.revise = "false";
                            data.button.save = "false";
                            data.button.submit = "false";

                            sql = @"select distinct to_char(pmdv_admin) as type 
                                    from bz_data_manage where pmdv_admin = 'true' and emp_id = '" + user_id + "' ";
                            var pmdv_admin_list = context.Database.SqlQuery<approverModel>(sql).ToList();
                            if (pmdv_admin_list != null)
                            {
                                if (pmdv_admin_list.Count > 0)
                                {
                                    if (pmdv_admin_list[0].type.ToString() == "true")
                                    {
                                        data.button.approve = "true";
                                        data.button.cancel = "true";
                                        data.button.reject = "true";
                                        data.button.revise = "true";
                                        data.button.save = "true";
                                        data.button.submit = "true";

                                    }
                                }
                            }
                        }
                    }
                    #endregion DevFix 20211012 0000 กรณีที่เป็น pmdv admin ทำ training ให้ button.save กับ button.approve = true 

                }

            }
            catch (Exception ex)
            {

            }

            return data;
        }

        public DocDetail2Model searchDetail2(DocDetailSearchModel value)
        {
            string user_id = "";
            string user_role = "";
            string pf_doc_id = "";

            var data = new DocDetail2Model();
            var docHead = new List<DocList2Model>();

            var traveler = new List<travelerDoc2Model>();
            var employee = new List<employeeDoc2>();
            var approver = new List<approverModel>();
            var travelerapprover = new List<approvertraveler>();

            data.button.approve = "false";
            data.button.cancel = "false";
            data.button.reject = "false";
            data.button.revise = "false";
            data.button.save = "false";
            data.button.submit = "false";

            data.button.part_i = "false";
            data.button.part_ii = "false";
            data.button.part_iii = "false";
            data.button.part_iiii = "false";
            data.button.part_cap = "false";


            string sql = "";
            var TypeModel = new List<TypeModel>();
            try
            {
                using (TOPEBizEntities context = new TOPEBizEntities())
                {
                    //var docHeadStatus = new List<DocHeadModel>();
                    //sql = " select to_char(dh_doc_status) as document_status from bz_doc_head h where h.dh_code = '" + value.id_doc + "' ";
                    //docHeadStatus = context.Database.SqlQuery<DocHeadModel>(sql).ToList();

                    var id_doc = value.id_doc;
                    var docHeadStatus = context.DocHeadModels
                        .FromSqlRaw("select to_char(dh_doc_status) as document_status from bz_doc_head h where h.dh_code = :id_doc"
                       , context.ConvertTypeParameter("id_doc", id_doc, "char")).ToList();


                    //sql = "delete from BZ_DOC_TRAVELER_EXPENSE  ";
                    //sql += " where DH_CODE='" + value.id_doc + "' and DTE_STATUS =0  ";
                    //context.Database.ExecuteSqlCommand(sql);
                    context.Database.ExecuteSqlRaw("DELETE FROM BZ_DOC_TRAVELER_EXPENSE WHERE DTE_STATUS = 0 and DH_CODE = :id_doc"
                        , context.ConvertTypeParameter("id_doc", id_doc));



                    sql = "delete from BZ_DOC_TRAVELER_APPROVER  ";
                    sql += " where DH_CODE='" + value.id_doc + "' and DTA_STATUS =0  ";
                    //DevFix 20210718 0000 ปิด code นี้ เนื่องจาก กรณีที่ line revise --> admin revise --> request submit  ??? 

                    context.Database.ExecuteSqlCommand(sql);

                    var login_empid = new List<SearchUserModel>();
                    sql = "SELECT  a.USER_NAME, a.user_id, to_char(u.ROLE_ID) user_role ";
                    sql += "FROM bz_login_token a left join bz_users u on a.user_id=u.employeeid ";
                    sql += " WHERE a.TOKEN_CODE ='" + value.token_login + "' ";
                    login_empid = context.Database.SqlQuery<SearchUserModel>(sql).ToList();
                    if (login_empid != null && login_empid.Count() > 0)
                    {
                        user_id = login_empid[0].user_id ?? "";
                        user_role = login_empid[0].user_role ?? "";
                    }

                    sql = " SELECT  emp_id user_id, to_char(action_status) action_status  ";
                    sql += " FROM bz_doc_action b ";
                    sql += " WHERE b.dh_code = '" + value.id_doc + "' ";

                    if (user_role == "1")
                        sql += " and b.emp_id = 'admin' ";
                    else
                        sql += " and b.emp_id = '" + user_id + "' ";

                    sql += " and action_status = 1 ";
                    sql += " and b.tab_no = 2 ";
                    var action = context.Database.SqlQuery<SearchUserModel>(sql).ToList();


                    //หาว่า  type นี้เป็น oversea หรือ local
                    sql = " select DH_CODE , DH_TYPE type, DH_EXPENSE_OPT1 checkbox_1, DH_EXPENSE_OPT2 checkbox_2, DH_EXPENSE_REMARK remark " + Environment.NewLine;
                    sql += ", to_char(DH_DOC_STATUS) doc_status ";
                    sql += " , b.TS_NAME document_status, a.DH_TYPE_FLOW ";
                    sql += " from BZ_DOC_HEAD a left join BZ_MASTER_STATUS b on a.DH_DOC_STATUS=b.TS_ID ";
                    sql += " WHERE DH_CODE = '" + value.id_doc + "' " + Environment.NewLine;
                    docHead = context.Database.SqlQuery<DocList2Model>(sql).ToList();
                    if (docHead != null)
                    {
                        //DevFix 20210527 0000 เพิ่มข้อมูล ประเภทใบงานเป็น 1:flow, 2:not flow, 3:training เก็บไว้ที่  BZ_DOC_HEAD.DH_TYPE_FLOW
                        data.type_flow = docHead[0].DH_TYPE_FLOW ?? "1";

                        data.type = docHead[0].type.ToString();
                        data.document_status = docHead[0].document_status ?? "";

                        if (data.type == "oversea" || data.type == "overseatraining")
                        {
                            data.oversea = new TypeModel();
                            data.oversea.checkbox_1 = (docHead[0].checkbox_1 ?? "") == "Y" ? "true" : "false";
                            data.oversea.checkbox_2 = (docHead[0].checkbox_2 ?? "") == "Y" ? "true" : "false";
                            data.oversea.remark = docHead[0].remark ?? "";
                        }
                        else
                        {
                            data.local = new TypeModel();
                            data.local.checkbox_1 = (docHead[0].checkbox_1 ?? "") == "Y" ? "true" : "false";
                            data.local.checkbox_2 = (docHead[0].checkbox_2 ?? "") == "Y" ? "true" : "false";
                            data.local.remark = docHead[0].remark;
                        }

                        pf_doc_id = docHead[0].doc_status.Substring(0, 1);

                        #region"#### Button Control ####"

                        if (action != null && action.Count() > 0)
                        {
                            data.button.approve = "true";
                            data.button.cancel = "true";
                            data.button.reject = "true";
                            data.button.revise = "true";
                            data.button.save = "true";
                            data.button.submit = "true";
                        }

                        #endregion

                        #region"#### Tab Control ####"

                        if (pf_doc_id == "2")
                        {
                            data.button.part_i = "true";
                            data.button.part_ii = "true";
                        }
                        else if (pf_doc_id == "3")
                        {
                            data.button.part_i = "true";
                            data.button.part_ii = "true";
                            data.button.part_iii = "true";
                        }
                        else if (pf_doc_id == "4")
                        {
                            data.button.part_i = "true";
                            data.button.part_ii = "true";
                            data.button.part_iii = "true";
                            data.button.part_iiii = "true";
                            data.button.part_cap = "true";
                        }
                        else if (pf_doc_id == "5")
                        {
                            data.button.part_i = "true";
                            data.button.part_ii = "true";
                            data.button.part_iii = "true";
                            data.button.part_iiii = "true";
                            data.button.part_cap = "true";
                        }

                        #endregion

                    }

                    //employee
                    sql = " SELECT DTE_TOKEN ref_id, DTE_EMP_ID id, U.Employeeid ";
                    sql += " , nvl(U.ENTITLE, '') || ' ' || U.ENFIRSTNAME || ' ' || U.ENLASTNAME || case when h.DH_TRAVEL ='1' then '' else ' | ' || case when h.DH_TYPE like 'local%' then p.pv_name else c.ct_name end end name ";
                    sql += " , nvl(U.ENTITLE, '') || ' ' || U.ENFIRSTNAME || ' ' || U.ENLASTNAME  name2 ";
                    sql += " , U.ORGNAME org, DTE_TRAVEL_DAYS ";
                    sql += " , case when tv.DTE_BUS_FROMDATE is null then '' else to_char(tv.DTE_BUS_FROMDATE, 'dd Mon rrrr') || ' - ' || to_char(tv.DTE_BUS_TODATE, 'dd Mon rrrr') end as business_date ";
                    sql += " , case when DTE_TRAVEL_FROMDATE is null then '' else to_char(DTE_TRAVEL_FROMDATE, 'dd Mon rrrr') || ' - ' || to_char(DTE_TRAVEL_TODATE, 'dd Mon rrrr') end as travel_date ";
                    sql += " , to_char('') visa_fee, '' passport_expense, '' clothing_expense ";
                    sql += " , to_char(c.ct_id) country_id, c.ct_name country ";
                    sql += " , p.pv_name || (case when nvl(tv.city_text,'') = '' then '' else '/'||tv.city_text end)  province ";
                    sql += " , tv.dte_traveler_remark remark ";
                    sql += " FROM bz_doc_traveler_expense tv inner join BZ_DOC_HEAD h on h.dh_code=tv.dh_code ";
                    sql += " inner join BZ_USERS U on tv.DTE_Emp_Id = u.employeeid ";
                    sql += " left join bz_master_country c on tv.ct_id = c.ct_id ";
                    sql += " left join BZ_MASTER_PROVINCE p on tv.PV_ID = p.PV_ID ";
                    sql += " WHERE tv.dh_code = '" + value.id_doc + "' and tv.dte_status = 1 ";
                    sql += " order by DTE_ID ";

                    if (data.type == "oversea" || data.type == "overseatraining")
                        data.oversea.employee = context.Database.SqlQuery<employeeDoc2>(sql).ToList();
                    else
                        data.local.employee = context.Database.SqlQuery<employeeDoc2>(sql).ToList();


                    //traveler
                    sql = " SELECT DTE_EMP_ID emp_id, DTE_AIR_TECKET air_ticket, DTE_ACCOMMODATIC accommodation , to_char(DTE_ALLOWANCE_DAY) allowance_day ";
                    sql += " , to_char(DTE_ALLOWANCE_NIGHT) allowance_night, to_char(DTE_CL_VALID, 'dd MON rrrr') clothing_valid ,DTE_CL_EXPENSE clothing_expense ";
                    sql += "  , to_char(DTE_PASSPORT_VALID,'dd MON rrrr') passport_valid ,  DTE_PASSPORT_EXPENSE passport_expense, DTE_VISA_FREE visa_fee ";
                    sql += "  , DTE_TRAVEL_INS travel_insurance, DTE_TRANSPORT transportation , DTE_REGIS_FREE registration_fee, DTE_MISCELLANEOUS miscellaneous ";
                    sql += "  , to_char(DTE_TOTAL_EXPENSE) total_expenses ";
                    sql += "  , nvl(U.ENTITLE, '') || ' ' || U.ENFIRSTNAME || ' ' || U.ENLASTNAME || case when h.DH_TRAVEL ='1' then '' else ' | ' || case when h.DH_TYPE like 'local%' then p.pv_name else c.ct_name end end emp_name ";
                    sql += "  , u.ORGNAME org, to_char(c.ct_id)country_id, c.ct_name country ";
                    sql += "  , case when t.DTE_BUS_FROMDATE is null then '' else to_char(t.DTE_BUS_FROMDATE, 'dd Mon rrrr') || ' - ' || to_char(t.DTE_BUS_TODATE, 'dd Mon rrrr') end as business_date ";
                    sql += "  , case when t.DTE_TRAVEL_FROMDATE is null then '' else to_char(t.DTE_TRAVEL_FROMDATE, 'dd Mon rrrr') || ' - ' || to_char(t.DTE_TRAVEL_TODATE, 'dd Mon rrrr') end as travel_date ";
                    sql += "  , t.DTE_ALLOWANCE Allowance ";
                    sql += " , p.pv_name || (case when nvl(t.city_text,'') = '' then '' else '/'||t.city_text end) province ";
                    sql += " , DTE_TOKEN ref_id, 'true' edit, 'true' \"delete\" ";

                    sql += " , t.dte_traveler_remark remark ";

                    //DevFix 20210714 0000 เพิ่มสถานะที่ Line/CAP --> 1:Draft , 2:Pendding , 3:Approve , 4:Revise , 5:Reject 
                    sql += " , '' as approve_status";
                    sql += " , '' as approve_remark";

                    //DevFix 20210719 0000 เพิ่ม field OPT
                    sql += " , t.dte_appr_remark as approve_opt";
                    //DevFix 20210719 0000 เพิ่ม field OPT
                    //sql += " , t.dte_appr_remark as remark_opt"; 
                    if (docHead[0].doc_status.ToString().Substring(0, 1) == "4" ||
                        docHead[0].doc_status.ToString().Substring(0, 1) == "5")
                    {
                        if (docHead[0].doc_status.ToString() == "41")
                        {
                            sql += " , t.dte_appr_remark as remark_opt";
                        }
                        else
                        {
                            sql += " , case when t.dte_appr_opt = 'false' then t.dte_appr_remark else t.dte_cap_appr_remark end remark_opt";
                        }
                    }
                    else
                    {
                        sql += " , t.dte_appr_remark as remark_opt";
                    }

                    sql += " , t.dte_cap_appr_remark as remark_cap";

                    //DevFix 20210817 เพิ่ม key traveler_ref_id เพื่อใช้ในการแยกข้อมูลออกแต่ละรายการ เนื่องจากเงื่อนไขเดิมข้อมูลซ้ำ --> เก็บค่าเป็น token id
                    //เนื่องจากไม่สามารถ up dataset model ได้ให้ใช้ DTE_TOKEN แทน
                    sql += " , to_char(DTE_TOKEN) as traveler_ref_id";

                    sql += " from BZ_DOC_TRAVELER_EXPENSE t left join bz_users u on t.dte_emp_id = u.employeeid ";
                    sql += " inner join bz_doc_head h on t.dh_code=h.dh_code ";
                    sql += " left join bz_master_country c on t.CT_ID = c.ct_id ";
                    sql += " left join BZ_MASTER_PROVINCE p on t.PV_ID = p.PV_ID ";
                    sql += " WHERE t.DH_CODE = '" + value.id_doc + "' and t.dte_status = 1  and  t.DTE_EXPENSE_CONFIRM = 1  ";
                    sql += " order by DTE_ID ";
                    var travelTemp = context.Database.SqlQuery<travelerDoc2Model>(sql).ToList();

                    #region DevFix 20210714 0000 ดึงข้อมูลรายละเอียด approver เดิม
                    var bCheckPF_CAP = true;//กรณีที่ Line submit to CAP แต่ CAP ยังไม่ได้ active
                    sql = @" select to_char(count(1)) as approve_status
                                from BZ_DOC_TRAVELER_APPROVER a
                                where dta_action_status >  2 and a.dta_type = 2 and dh_code =  '" + value.id_doc + "'  ";
                    var dataCheck_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                    if (dataCheck_Def != null)
                    {
                        if (dataCheck_Def.Count > 0) { if (dataCheck_Def[0].approve_status.ToString() == "0") { bCheckPF_CAP = false; } }
                    }
                    //DevFix 20210714 0000 เพิ่มสถานะที่ Line/CAP --> 1:Draft , 2:Pendding , 3:Approve , 4:Revise , 5:Reject    
                    sql = @" select dta_appr_level,dta_travel_empid as emp_id, (a.dta_action_status) as approve_status,  a.dta_appr_remark as approve_remark
                                ,to_char(nvl(a.dta_appr_status,'true')) as  approve_opt
                                from BZ_DOC_TRAVELER_APPROVER a
                                where dh_code =  '" + value.id_doc + "'  ";
                    if (pf_doc_id == "3")
                    {
                        sql += @" and a.dta_type = 1";
                    }
                    else if (pf_doc_id == "4")
                    {
                        if (bCheckPF_CAP == true) { sql += @" and a.dta_type = 2 and dta_action_status not in ('6') "; } else { sql += @" and a.dta_type = 1"; }
                    }
                    sql += @" order by dta_appr_level ";
                    var dataApporver_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                    #region DevFix 20211013 0000 กรณีที่เป็น step cap ให้ตรวจสอบ line ว่ามีการ reject หรือไม่
                    var dataApporverLine_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                    var dataApporverLine2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                    var dataApporverLine3_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                    var dataApporverCAP_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                    var dataApporverCAP2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                    var dataApporverCAP3_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                    var dataApporverRevise_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                    var check_status_approver_line = false;
                    if (pf_doc_id == "3" || pf_doc_id == "4" || pf_doc_id == "5")
                    {
                        //line approve
                        sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '3' as approve_status, dte_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where dte_appr_opt = 'true' and dte_status = 1 and dte_appr_status <> 23
                                         and dh_code = '" + value.id_doc + "' ";
                        dataApporverLine_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                        //line reject
                        sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '5' as approve_status, dte_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where ((dte_appr_opt = 'false' and dte_status = 1) or dte_appr_status = 30 )
                                         and dh_code = '" + value.id_doc + "' ";
                        sql += "         union ";
                        sql += @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '5' as approve_status, dte_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where dte_status = 1  
                                         and (dte_appr_status = 23 and a.dte_appr_opt = 'false')  
                                         and dh_code = '" + value.id_doc + "'";
                        dataApporverLine2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                        //line pendding
                        sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '2' as approve_status, dte_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where dte_status = 1 and dte_appr_status = 31 and dh_code = '" + value.id_doc + "' ";
                        dataApporverLine3_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                        //cap approve
                        sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '3' as approve_status, dte_cap_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where nvl(dte_cap_appr_status,41) = '42' and (dte_cap_appr_opt = 'true' and dte_appr_opt = 'true') and dte_status = 1
                                         and dh_code = '" + value.id_doc + "' ";
                        dataApporverCAP_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                        //cap reject
                        sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '5' as approve_status, dte_cap_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where nvl(dte_cap_appr_status,41) = '42' and ( (dte_cap_appr_opt = 'false' and dte_status = 1) or (dte_appr_opt = 'false' and dte_appr_status = 32) or dte_cap_appr_status = 40 )
                                         and dh_code = '" + value.id_doc + "' ";
                        sql += "         union ";
                        sql += @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '5' as approve_status, dte_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where dte_status = 1  
                                         and ( dte_cap_appr_status = 23 and a.dte_cap_appr_opt = 'false')  
                                         and dh_code = '" + value.id_doc + "'";

                        dataApporverCAP2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();


                        //cap pendding
                        sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '2' as approve_status, dte_cap_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where dte_status = 1 and (dte_cap_appr_status = 41 or (dte_cap_appr_status is null and  dte_appr_status = 32 and dte_appr_opt = 'true' ) )
                                         and dh_code = '" + value.id_doc + "' ";
                        dataApporverCAP3_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();


                        //line/CAP revise
                        sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '4' as approve_status, dte_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where dte_status = 1 and (dte_appr_status = 23 or dte_cap_appr_status = 23)
                                         and dh_code = '" + value.id_doc + "' ";
                        dataApporverRevise_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                    }
                    #endregion DevFix 20211013 0000 กรณีที่เป็น step cap ให้ตรวจสอบ line ว่ามีการ reject หรือไม่

                    foreach (var item in travelTemp)
                    {
                        if (item.emp_id == "00001393" && item.country == "Denmark")
                        {
                            var xdebug = "";
                        }
                        var approve_status = "1";
                        var approve_remark = "";
                        var approve_opt = "";
                        var check_data = dataApporver_Def.Where(t => t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id);

                        #region DevFix 20211013 0000 กรณีที่เป็น step cap ให้ตรวจสอบ line ว่ามีการ reject หรือไม่ 
                        if (pf_doc_id == "4" || pf_doc_id == "5")
                        {
                            try
                            {
                                if (bCheckPF_CAP == true)
                                {
                                    check_data = dataApporverCAP_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                    if (check_data.Count() == 0)
                                    {
                                        check_data = dataApporverCAP2_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                        if (check_data.Count() == 0)
                                        {
                                            check_data = dataApporverCAP3_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                            if (check_data.Count() == 0)
                                            {
                                                check_data = dataApporverRevise_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                                if (check_data.Count() == 0)
                                                {
                                                    //กรณีที่มีข้อมูล line ให้เอา remark line มาแสดง
                                                    check_data = dataApporverLine2_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                                    if (check_data.Count() == 0)
                                                    {
                                                        check_data = dataApporverLine_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                                    }
                                                    check_status_approver_line = false;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    check_data = dataApporverLine2_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                    if (check_data.Count() == 0)
                                    {
                                        check_data = dataApporverLine3_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                        if (check_data.Count() == 0)
                                        {
                                            check_data = dataApporverRevise_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                            //เฉพาะ tab 1 and 2
                                            if (check_data.Count() == 0)
                                            {
                                                check_data = dataApporverCAP3_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                            }

                                        }
                                    }
                                }
                            }
                            catch { }
                        }

                        if (pf_doc_id == "3" || check_status_approver_line == true)
                        {
                            check_data = dataApporverLine_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                            if (check_data.Count() == 0)
                            {
                                check_data = dataApporverLine2_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                if (check_data.Count() == 0)
                                {
                                    check_data = dataApporverRevise_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                    if (check_data.Count() == 0)
                                    {
                                        check_data = dataApporverLine3_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                    }
                                }
                            }
                        }
                        #endregion DevFix 20211013 0000 กรณีที่เป็น step cap ให้ตรวจสอบ line ว่ามีการ reject หรือไม่
                        var bcheck_change_status = false;
                        foreach (var item2 in check_data)
                        {
                            approve_status = item2.approve_status;
                            approve_opt = item2.approve_opt;

                            if (approve_remark != "") { approve_remark += ","; }
                            if (item2.approve_remark != "") { approve_remark += item2.approve_remark; }
                            bcheck_change_status = true;
                        }
                        if (approve_status == "" || bcheck_change_status == false) { approve_status = item.approve_status; }
                        if (approve_remark == "") { approve_remark = item.approve_remark; }

                        item.approve_status = approve_status;
                        item.approve_remark = approve_remark; // remark btn action
                        item.approve_opt = approve_opt;
                    }
                    #endregion DevFix 20210714 0000 ดึงข้อมูลรายละเอียด approver เดิม


                    if (data.type == "oversea" || data.type == "overseatraining")
                        data.oversea.traveler = travelTemp;
                    else
                        data.local.traveler = travelTemp;

                    sql = "  select to_char(a.dta_id)line_id ";
                    sql += "  , to_char(a.dta_type) as type ";
                    sql += "  , a.dta_travel_empid emp_id ";
                    sql += "  , nvl(u.ENTITLE, '') ||' '|| u.ENFIRSTNAME || ' ' || u.ENLASTNAME as emp_name ";
                    sql += "  , u.ORGNAME emp_org ";
                    sql += "  , a.dta_appr_empid appr_id ";
                    sql += "  , nvl(u2.ENTITLE, '') ||' '|| u2.ENFIRSTNAME || ' ' || u2.ENLASTNAME as appr_name ";
                    sql += "  , u2.ORGNAME appr_org ";
                    sql += "  , case when a.dta_type = 1 then 'Endorsed' else 'CAP' end remark ";
                    //DevFix 20210714 0000 เพิ่มสถานะที่ Line/CAP --> 1:Draft , 2:Pendding , 3:Approve , 4:Revise , 5:Reject 
                    sql += "  , a.dta_action_status as approve_status , a.dta_appr_remark as approve_remark";

                    //DevFix 20210810 0000 approve level ตามลำดับได้เลย เรียงตาม traverler id 
                    sql += "  , to_char(a.dta_appr_level) as approve_level ";

                    sql += "   from BZ_DOC_TRAVELER_APPROVER a inner join bz_users u on a.dta_travel_empid = u.employeeid ";
                    sql += "   inner join bz_users u2 on a.dta_appr_empid = u2.employeeid ";
                    sql += "   where a.dh_code = '" + value.id_doc + "' and a.DTA_STATUS=1 ";
                    sql += "   order by  a.dta_id ";

                    var travelTempApprover = context.Database.SqlQuery<doc2ApproverModel>(sql).ToList();
                    if (data.type == "oversea" || data.type == "overseatraining")
                        data.oversea.approver = travelTempApprover;
                    else
                        data.local.approver = travelTempApprover;

                    sql = "test log 1";//test log

                    #region DevFix 20200827 2349 Exchange Rates as of -->ExchangeRates  
                    if (data.type == "oversea" || data.type == "overseatraining")
                    {
                        try
                        {
                            ExchangeRatesModel ex_rate = ExchangeRates(ref sql);
                            sql = "test log 2";//test log

                            decimal ex_value = toDecimal(ex_rate.ex_value1);
                            data.ExchangeRates = new ExchangeRatesModel();
                            data.ExchangeRates.ex_value1 = ex_value.ToString("#,##0.#0") + " THB";//T_FXB_VALUE1 คือ exchange rate 
                            data.ExchangeRates.ex_date = ex_rate.ex_date;//T_FXB_VALDATE คือวันที่ของข้อมูล ex.rate นี้ --> 20161202
                            data.ExchangeRates.ex_cur = ex_rate.ex_cur;//T_FXB_CUR คือ สกุลเงิน 
                            sql = "test log 3";//test log
                        }
                        catch { }

                    }
                    #endregion DevFix 20200827 2349 Exchange Rates as of -->ExchangeRates 


                    #region DevFix 20211012 0000 กรณีที่เป็น pmdv admin ทำ training ให้ button.save กับ button.approve = true 
                    string doc_status_tab = docHead[0].doc_status.ToString().Substring(0, 1);
                    string doc_status_chk = docHead[0].doc_status.ToString();
                    if (doc_status_tab == "5" || doc_status_chk == "10" || doc_status_chk == "20" || doc_status_chk == "30" || doc_status_chk == "40" || doc_status_chk == "50") { }
                    else
                    {
                        if (value.id_doc.IndexOf("T") > -1 && doc_status_tab == "2")
                        {
                            data.button.approve = "false";
                            data.button.cancel = "false";
                            data.button.reject = "false";
                            data.button.revise = "false";
                            data.button.save = "false";
                            data.button.submit = "false";


                            sql = "test log 4";//test log
                            sql = @"select distinct to_char(pmdv_admin) as type 
                                    from bz_data_manage where pmdv_admin = 'true' and emp_id = '" + user_id + "' ";
                            var pmdv_admin_list = context.Database.SqlQuery<approverModel>(sql).ToList();

                            sql = "test log 5";//test log

                            if (pmdv_admin_list != null)
                            {
                                if (pmdv_admin_list.Count > 0)
                                {
                                    if (pmdv_admin_list[0].type.ToString() == "true")
                                    {
                                        data.button.approve = "true";
                                        data.button.cancel = "true";
                                        data.button.reject = "true";
                                        data.button.revise = "true";
                                        data.button.save = "true";
                                        data.button.submit = "true";

                                    }
                                }
                            }
                        }
                    }
                    #endregion DevFix 20211012 0000 กรณีที่เป็น pmdv admin ทำ training ให้ button.save กับ button.approve = true 

                }
            }
            catch (Exception ex)
            {
                var line = "";

                data.msg_remark = ex.Message.ToString() + " sql :" + sql;
                //throw;
            }
            data.msg_remark += " sql :" + sql;
            return data;
        }

        public DocDetail3OutModel searchDetail3(DocDetail3Model value)
        {
            var data = new DocDetail3OutModel();
            var docHead = new List<DocList3Model>();
            string doc_type = "";
            string user_id = "";
            string user_role = "";
            Boolean have_action = false;
            var pf_doc_id = "";

            data.button.approve = "false";
            data.button.cancel = "false";
            data.button.reject = "false";
            data.button.revise = "false";
            data.button.save = "false";
            data.button.submit = "false";

            data.button.part_i = "true";
            data.button.part_ii = "true";
            data.button.part_iii = "true";
            data.button.part_iiii = "false";
            data.button.part_cap = "false";

            var TypeModel = new List<TypeModel>();
            string sql = "";
            try
            {
                using (TOPEBizEntities context = new TOPEBizEntities())
                {
                    decimal grand_total = 0;
                    var person_user = 0;
                    var document_status = "";
                    //string sql = "";

                    #region ตรวจสอบสถานะใบงาน
                    var docHeadStatus = new List<DocHeadModel>();
                    sql = " select to_char(dh_doc_status) as document_status from bz_doc_head h where h.dh_code = '" + value.id_doc + "' ";
                    docHeadStatus = context.Database.SqlQuery<DocHeadModel>(sql).ToList();
                    if (docHeadStatus != null && docHeadStatus.Count > 0)
                    {
                        document_status = docHeadStatus[0].document_status;
                    }
                    #endregion ตรวจสอบสถานะใบงาน


                    sql = "delete from BZ_DOC_TRAVELER_EXPENSE  ";
                    sql += " where DH_CODE='" + value.id_doc + "' and dte_status =0  ";
                    context.Database.ExecuteSqlCommand(sql);

                    var login_empid = new List<SearchUserModel>();
                    sql = "SELECT  a.USER_NAME, a.user_id, to_char(u.ROLE_ID) user_role ";
                    sql += "FROM bz_login_token a left join bz_users u on a.user_id=u.employeeid ";
                    sql += " WHERE a.TOKEN_CODE ='" + value.token + "' ";
                    login_empid = context.Database.SqlQuery<SearchUserModel>(sql).ToList();
                    if (login_empid != null && login_empid.Count() > 0)
                    {
                        user_id = login_empid[0].user_id ?? "";
                        user_role = login_empid[0].user_role ?? "";
                    }

                    //กรณีที่เป็น pmdv admin, pmsv_admin
                    if (value.id_doc.IndexOf("T") > -1)
                    {
                        sql = @" select emp_id as user_id from bz_data_manage where (pmsv_admin = 'true' or pmdv_admin = 'true') and emp_id = '" + user_id + "' ";
                        var adminlist = context.Database.SqlQuery<SearchUserModel>(sql).ToList();
                        if (adminlist != null)
                        {
                            if (adminlist.Count > 0) { user_role = "1"; }
                        }
                    }

                    sql = " SELECT  emp_id user_id, to_char(action_status) action_status  ";
                    sql += " FROM bz_doc_action b ";
                    sql += " WHERE b.dh_code = '" + value.id_doc + "' ";

                    //DevFix 20200901 2340 กรณีที่ admin ไม่ต้องเช็ค status 
                    if (user_role == "1")
                        sql += " and b.emp_id <> 'admin' ";
                    else
                        sql += " and b.emp_id = '" + user_id + "' ";

                    sql += " and action_status = 1 ";
                    sql += " and b.tab_no = 3 ";
                    var action = context.Database.SqlQuery<SearchUserModel>(sql).ToList();

                    #region DevFix 20210806 0000 กรณีที่ไม่ใช่ admin ให้ตรวจสอบ emp ว่าเป็น traveler 
                    var login_emp_traveler_view = false;
                    var login_emp_requester_view = false;
                    if (user_role != "1")
                    {
                        var emp_type = new List<SearchUserModel>();
                        sql = @"select to_char(t.user_type ) as user_type
                                from (
                                select dh_code as doc_id, 1 as user_type, a.dta_travel_empid as emp_id
                                from  bz_doc_traveler_approver a where a.dta_type = 1
                                union
                                select dh_code as doc_id, 2 as user_type, a.dta_appr_empid as emp_id
                                from  bz_doc_traveler_approver a where a.dta_type = 1 
                                union
                                select dh_code as doc_id, 3 as user_type, a.dta_appr_empid as emp_id
                                from  bz_doc_traveler_approver a  where a.dta_type = 2  
                                union
                                 select dh_code as doc_id, 4 as user_type, a.dh_behalf_emp_id as emp_id
                                 from  bz_doc_head a
                                 union 
                                 select dh_code as doc_id, 4 as user_type, a.dh_initiator_empid as emp_id
                                 from  bz_doc_head a
                                 union 
                                 select dh_code as doc_id, 4 as user_type, a.dh_create_by as emp_id
                                 from  bz_doc_head a
                                )t  where t.user_type in (1,2,4) and t.doc_id ='" + value.id_doc + "' and t.emp_id = '" + user_id + "' ";
                        sql += " order by user_type desc ";
                        emp_type = context.Database.SqlQuery<SearchUserModel>(sql).ToList();
                        if (emp_type != null && emp_type.Count() > 0)
                        {
                            if (emp_type[0].user_type.ToString() == "1") { login_emp_traveler_view = true; }
                            if (emp_type[0].user_type.ToString() == "4") { login_emp_requester_view = true; }
                        }
                    }
                    else { login_emp_traveler_view = false; login_emp_requester_view = false; }
                    #endregion DevFix 20210806 0000 กรณีที่ไม่ใช่ admin ให้ตรวจสอบ emp ว่าเป็น traveler 

                    #region DevFix 20200910 1200 ตรวจสอบข้อมูล emp ก่อนว่าเป็น role : approver line ,cap 
                    string check_dta_type = "1"; // line approver
                    //แก้ไขเพิ่มเติมเนื่องจากรณีที่เป็น CAP  ไม่สามารถดูข้อมูลใน TAB2 ได้  dta_type
                    //ถ้าแก้แล้วจะแสดงข้อมูลแต่ต้องเช็คอีกทีว่ามีผลอะไรหรือป่าว???
                    sql = "  select dta_type as type from bz_doc_traveler_approver a where a.dh_code =  '" + value.id_doc + "' ";
                    if (user_role != "1" && login_emp_requester_view == false)
                    {
                        sql += " and a.dta_appr_empid = '" + user_id + "' ";
                    }
                    var actionapprover_type = context.Database.SqlQuery<approverModel>(sql).ToList();
                    if (actionapprover_type != null)
                    {
                        if (actionapprover_type.Count > 0) { check_dta_type = actionapprover_type[0].type.ToString(); }
                    }
                    #endregion DevFix 20200910 1200 ตรวจสอบข้อมูล emp ก่อนว่าเป็น role : approver line ,cap 

                    //หาว่า  type นี้เป็น oversea หรือ local
                    sql = " select a.DH_CODE , DH_TYPE type, DH_EXPENSE_OPT1 checkbox_1, DH_EXPENSE_OPT2 checkbox_2, DH_EXPENSE_REMARK remark " + Environment.NewLine;
                    sql += ", to_char(DH_DOC_STATUS) doc_status ";
                    sql += " , b.TS_NAME document_status ";
                    sql += ", DH_AFTER_TRIP_OPT1, DH_AFTER_TRIP_OPT2, DH_AFTER_TRIP_OPT3, DH_AFTER_TRIP_OPT2_REMARK, DH_AFTER_TRIP_OPT3_REMARK";
                    sql += " , to_char(nvl(DH_TOTAL_PERSON, 0)) || ' Person(s)' person ";
                    sql += " , a.DH_TOPIC topic ";

                    sql += "  , case when DH_BUS_FROMDATE is null then '' else to_char(DH_BUS_FROMDATE, 'dd Mon rrrr') || ' - ' || to_char(DH_BUS_TODATE, 'dd Mon rrrr') end as bus_date ";
                    sql += "  , case when DH_TRAVEL_FROMDATE is null then '' else to_char(DH_TRAVEL_FROMDATE, 'dd Mon rrrr') || ' - ' || to_char(DH_TRAVEL_TODATE, 'dd Mon rrrr') end as travel_date ";
                    sql += " , a.DH_CITY city_text ";
                    sql += " , d.ct_name country ";
                    sql += " , e.ctn_name continent , a.DH_TYPE_FLOW";
                    sql += " from BZ_DOC_HEAD a left join BZ_MASTER_STATUS b on a.DH_DOC_STATUS=b.TS_ID ";
                    sql += " left join BZ_DOC_COUNTRY c on a.dh_code=c.dh_code ";
                    sql += " left join BZ_MASTER_COUNTRY d on c.ct_id=d.ct_id ";
                    sql += " left join BZ_MASTER_CONTINENT e on d.ctn_id=e.ctn_id ";
                    sql += " WHERE a.DH_CODE = '" + value.id_doc + "' " + Environment.NewLine;
                    sql += " order by e.ctn_name ";
                    docHead = context.Database.SqlQuery<DocList3Model>(sql).ToList();
                    if (docHead != null)
                    {
                        try
                        {
                            pf_doc_id = docHead[0].doc_status.Substring(0, 1);
                        }
                        catch { }

                        //DevFix 20210527 0000 เพิ่มข้อมูล ประเภทใบงานเป็น 1:flow, 2:not flow, 3:training เก็บไว้ที่  BZ_DOC_HEAD.DH_TYPE_FLOW
                        data.type_flow = docHead[0].DH_TYPE_FLOW ?? "1";

                        doc_type = docHead[0].type ?? "";

                        var sql_select = "";
                        var sql_from = "";
                        var sql_from_traveler = "";
                        sql_select = @" SELECT distinct ct.ctn_name continent, pv.pv_name province, cr.ct_name country, ex.city_text, to_char(c.DTA_DOC_STATUS) action_status 

                            --DevFix 20210710 0000 เพิ่มเงือนไขว่าต้องเป็น line --> dta_type = 1  
                            , case when c.DTA_ACTION_STATUS in (2) and c.dta_type = 1 then 'true' else 'false' end take_action
                            , case when c.DTA_APPR_STATUS is null or nvl(c.DTA_DOC_STATUS, 31) = 31 then nvl(ex.dte_appr_opt,'true') else nvl(c.DTA_APPR_STATUS, 'true') end appr_status

                             --DevFix 20210710 0000 เปลี่ยน field 
                             , ex.DTE_APPR_REMARK appr_remark

                             , case when ex.DTE_BUS_FROMDATE is null then '' else to_char(ex.DTE_BUS_FROMDATE, 'dd Mon rrrr') || ' - ' || to_char(ex.DTE_BUS_TODATE, 'dd Mon rrrr') end as bus_date
                             , case when ex.DTE_TRAVEL_FROMDATE is null then '' else to_char(ex.DTE_TRAVEL_FROMDATE, 'dd Mon rrrr') || ' - ' || to_char(ex.DTE_TRAVEL_TODATE, 'dd Mon rrrr') end as travel_date
                             , u.employeeid emp_id, nvl(u.ENTITLE, '') || ' ' || u.ENFIRSTNAME || ' ' || u.ENLASTNAME as emp_name, u.ORGNAME emp_org
                             , u2.employeeid appr_emp_id, nvl(u2.ENTITLE, '') || ' ' || u2.ENFIRSTNAME || ' ' || u2.ENLASTNAME as appr_emp_name, u2.ORGNAME appr_emp_org
                             -- , to_char(c.dta_id) ref_id 
                             , to_char(ex.dte_token) ref_id
                             , to_char(ex.dte_total_expense) total
                            , ex.dte_id

                            --DevFix 20210714 0000 เพิ่มสถานะที่ Line/CAP --> 1:Draft , 2:Pendding , 3:Approve , 4:Revise , 5:Reject  
                            , case when ex.dte_appr_opt = 'true' then c.dta_action_status else (case when ex.dte_appr_opt = 'false' and nvl(ex.dte_appr_status,31) <> '31' then '5' else '2' end) end approve_status
                            , case when ex.dte_appr_opt = 'true' then c.dta_appr_remark else ex.dte_cap_appr_remark end approve_remark


                            --DevFix 20210719 0000 เพิ่ม field OPT
                            --, to_char(nvl(c.dta_appr_status,'true'))  as approve_opt
                            , nvl(case when ex.dte_appr_opt = 'true' then to_char(nvl(c.dta_appr_status,'true')) else ex.dte_appr_opt end,'true') approve_opt
                            , ex.dte_appr_remark as remark_opt

                            , ex.dte_cap_appr_remark as remark_cap

                            --DevFix 20210817 เพิ่ม key traveler_ref_id เพื่อใช้ในการแยกข้อมูลออกแต่ละรายการ เนื่องจากเงื่อนไขเดิมข้อมูลซ้ำ --> เก็บค่าเป็น token id
                            --เนื่องจากไม่สามารถ up dataset model ได้ให้ใช้ DTE_TOKEN แทน
                            , to_char(DTE_TOKEN) as traveler_ref_id";

                        sql_from += $@"  FROM  bz_doc_traveler_approver c  
                          inner join BZ_DOC_TRAVELER_EXPENSE ex on c.dh_code = ex.dh_code and c.dta_travel_empid = ex.dte_emp_id
                          left join bz_users u on u.employeeid = ex.dte_emp_id
                          left join bz_users u2 on c.dta_appr_empid = u2.employeeid
                          left join bz_master_continent ct on ex.ctn_id = ct.ctn_id
                          left join bz_master_country cr on ex.ct_id = cr.ct_id
                          left join bz_master_province pv on ex.pv_id = pv.pv_id
                         WHERE 1 = 1
                         and c.dh_code = '{value.id_doc}'

                        --DevFix 20200910 1200 ตรวจสอบข้อมูล emp ก่อนว่าเป็น role : approver line ,cap  
                         and c.dta_type = 1  -- รอ line approver    
                         and c.dta_status = 1  -- สถานะใช้งาน 
                        ";

                        if (login_emp_requester_view == true && document_status == "31") { } else { sql_from += " and ex.DTE_EXPENSE_CONFIRM = 1 "; }

                        // query ชุดนี้ให้เอาไว้ล่างสุด 
                        if (user_role != "1" && login_emp_requester_view == false)
                        {
                            if (login_emp_traveler_view == true)
                            {
                                sql_from_traveler = sql_from + " and c.DTA_TRAVEL_EMPID = '" + user_id + "' ";
                            }
                            sql_from += " and c.DTA_APPR_EMPID = '" + user_id + "' ";
                        }

                        //*****************************************
                        sql = sql_select + sql_from + " order by ex.dte_id ";

                        var docDetail3Head = context.Database.SqlQuery<DocDetail3HeadModel>(sql).ToList();
                        var bcheck_data_head = false;
                        if (user_role != "1" && login_emp_requester_view == false)
                        {
                            if (docDetail3Head != null) { if (docDetail3Head.Count() > 0) { bcheck_data_head = true; } }
                            if (bcheck_data_head == false && login_emp_traveler_view == true)
                            {
                                //กรณีที่เป็น traverler ข้อมูล head จะไม่มีให้ดึงใหม่
                                sql = sql_select + sql_from_traveler + " order by ex.dte_id ";
                            }
                        }

                        #region รายละเอียด head //select * from ()t2 order by  dte_id
                        var sql_p = " select * from ( select distinct min(dte_id) as dte_id, country as PROVINCE from (" + sql + ")t group by country)t2 order by  dte_id";
                        var sql_c = " select * from ( select distinct min(dte_id) as dte_id, country as PROVINCE, CITY_TEXT from (" + sql + ")t group by  country, city_text )t2 order by  dte_id ";
                        if (doc_type.ToLower().IndexOf("local") > -1)
                        {
                            sql_p = " select * from ( select distinct min(dte_id) as dte_id, PROVINCE from (" + sql + ")t group by PROVINCE)t2 order by  dte_id ";
                            sql_c = " select * from ( select distinct min(dte_id) as dte_id, PROVINCE, CITY_TEXT from (" + sql + ")t group by PROVINCE, CITY_TEXT)t2 order by  dte_id";
                        }
                        var sql_date = @"select to_char(min(ex.DTE_BUS_FROMDATE), 'dd Mon rrrr') || ' - ' || to_char(max(ex.DTE_BUS_TODATE), 'dd Mon rrrr') as bus_date 
                                         , to_char(min(ex.DTE_TRAVEL_FROMDATE), 'dd Mon rrrr') || ' - ' || to_char(max(ex.DTE_TRAVEL_TODATE), 'dd Mon rrrr') as travel_date  ";
                        if (user_role != "1" && login_emp_requester_view == false)
                        {
                            if (bcheck_data_head == false && login_emp_traveler_view == true)
                            {
                                sql_date += "  " + sql_from_traveler + "";
                            }
                            else { sql_date += sql_from + ""; }
                        }
                        else { sql_date += sql_from + ""; }

                        //data.msg_remark = "error:" + sql_p;
                        //data.msg_remark += "error:" + sql_c;
                        //data.msg_remark += "error:" + sql_date;
                        //return data;

                        var docDetailProvince = context.Database.SqlQuery<DocDetail3HeadModel>(sql_p).ToList();
                        var docDetailCity = context.Database.SqlQuery<DocDetail3HeadModel>(sql_c).ToList();
                        var docDetailDate = context.Database.SqlQuery<DocDetail3HeadModel>(sql_date).ToList();
                        #endregion รายละเอียด head

                        #region DevFix 20210527 0000 เพิ่มข้อมูล ประเภทใบงานเป็น 1:flow, 2:not flow, 3:training เก็บไว้ที่  BZ_DOC_HEAD.DH_TYPE_FLOW
                        //ของเดิมดึงตาม traverler id ทำให้กรณีที่มีมากกว่า 1 รายการ แสดงข้อมูลผิด
                        //ต้องดึงข้อมูล ตามรายการ จาก BZ_DOC_TRAVELER_EXPENSE-->DTE_APPR_OPT, DTE_APPR_REMARK
                        //แก้ใน query แล้ว
                        #endregion DevFix 20210527 0000 เพิ่มข้อมูล ประเภทใบงานเป็น 1:flow, 2:not flow, 3:training เก็บไว้ที่  BZ_DOC_HEAD.DH_TYPE_FLOW

                        data.document_status = docHead[0].document_status ?? "";
                        data.topic = docHead[0].topic ?? "";
                        data.total_travel = "0 Person(s)";
                        data.grand_total = "0";
                        data.checkbox_1 = (docHead[0].checkbox_1 ?? "") == "Y" ? "true" : "false";
                        data.checkbox_2 = (docHead[0].checkbox_2 ?? "") == "Y" ? "true" : "false";
                        data.remark = docHead[0].remark ?? "";
                        data.travel_date = docHead[0].travel_date ?? "";
                        data.business_date = docHead[0].bus_date ?? "";

                        string continent = "";
                        string country = "";
                        foreach (var h in docHead)
                        {
                            if (continent != h.continent)
                            {
                                if (!string.IsNullOrEmpty(continent)) continent += ", ";
                                continent += h.continent;
                            }

                            if (!string.IsNullOrEmpty(country)) country += ", ";
                            country += h.country;
                        }

                        data.continent = continent;

                        #region  DevFix 20210817 0000 กรณีที่ traverler 1 มี 2 รายการ 
                        // Country or Province / City ให้ group ตามประเทศ/จังหวัด และ city
                        var country_text = "";
                        var city_text = "";
                        foreach (var p in docDetailProvince)
                        {
                            city_text = "";
                            if (country_text != "") { country_text += ", "; }
                            var findDataCity = docDetailCity.Where(a => a.province == p.province).ToList();
                            if (findDataCity != null && findDataCity.Count > 0)
                            {
                                foreach (var c in findDataCity)
                                {
                                    if (city_text != "") { city_text += ","; }
                                    city_text += c.city_text + "";
                                }
                                if (city_text != "") { country_text += p.province + "/" + city_text; }
                            }
                        }
                        data.country = country_text;
                        if (doc_type.ToLower().IndexOf("local") > -1)
                        {
                            data.province = "";
                        }
                        data.travel_date = docDetailDate[0].travel_date ?? "";
                        data.business_date = docDetailDate[0].bus_date ?? "";
                        #endregion  DevFix 20210817 0000 กรณีที่ traverler 1 มี 2 รายการ 


                        //DevFix 20210721 0000 กรณีที่เป็น CAP ของ Line นั้นๆ ให้แสดงจำนวน Traveler และ Total
                        var bCheckApproverLineInDoc = false;
                        var bCheckTravelerListInDoc = false;
                        var no = 0;
                        var no2 = 0;

                        if (docDetail3Head != null)
                        {
                            if (docDetail3Head.Count() > 0)
                            {
                                //data.total_travel = docDetail3Head.Count().ToString() + " Person(s)"; 
                                //data.province = docDetail3Head[0].province ?? "";
                                if (doc_type.ToLower().IndexOf("local") > -1)
                                { }
                                else { data.province = docDetail3Head[0].province ?? ""; }

                                foreach (var t in docDetail3Head)
                                {
                                    decimal total = 0;
                                    decimal total_expenses = 0;

                                    //DevFix 20210721 0000 ด้านบนเห็นคนเดียวพร้อมเงินเฉพาะคนที่ถูก approve ถูกแล้ว แต่ด้านล่างควรเห็น ทั้งหมดที่อยู่ภายใต้ CAP 
                                    if ((t.take_action != null && t.take_action == "true") ||
                                        (t.take_action != null && t.take_action == "false" && t.action_status != "31") ||
                                        ((user_role == "1" || login_emp_requester_view == true) && t.approve_status != "5"))
                                    {
                                        if (t.approve_opt == "true")
                                        {
                                            //DevFix 20210817 0000 กรณีที่ traverler 1 มี 2 รายการ ให้แสดงรายการเดียวพอ 
                                            var add_traveler = true;
                                            var row_check = data.traveler_list.Where(p => p.emp_id.Equals(t.emp_id)).FirstOrDefault();
                                            if (row_check != null)
                                            {
                                                if (row_check.emp_id != "")
                                                {
                                                    //กรณีที่มีข้อมูลไม่ต้อง add เพิ่ม
                                                    add_traveler = false;
                                                }
                                            }
                                            if (add_traveler == true)
                                            {
                                                no2++;
                                                data.traveler_list.Add(new travelerList
                                                {
                                                    //DevFix 20200910 1658 เอาวงเล็บออก เหลือจุด . พอ 
                                                    text = no2.ToString() + ". " + t.emp_id + "  " + t.emp_name + " : " + t.emp_org,

                                                    //DevFix 20210817 0000 กรณีที่ traverler 1 มี 2 รายการ ให้แสดงรายการเดียวพอ --> เพิ่ม emp_id เพื่อเป็นเงื่อนไข
                                                    emp_id = t.emp_id,
                                                });

                                                person_user += 1;
                                            }
                                            //DevFix 20210721 0000 กรณีที่เป็น CAP ของ Line นั้นๆ ให้แสดงจำนวน Traveler และ Total
                                            bCheckTravelerListInDoc = true;

                                            //decimal total = toDecimal(t.total);
                                            total = toDecimal(t.total);
                                            grand_total += total;
                                        }
                                    }

                                    try
                                    {
                                        total_expenses = toDecimal(t.total);
                                    }
                                    catch { }

                                    //DevFix 20210818 0000 แก้ไขตาม Front End
                                    //ถ้า oversea / overseatraining พี่ส่ง country / province มาใน field country เลย เพราะมันแค่เอามาแสดง 
                                    //แต่ถ้าเป็น local / localtraining ให้ส่ง province / city มาใน field province 
                                    if (doc_type.ToLower().IndexOf("local") > -1)
                                    {
                                        t.province += "/" + t.city_text;
                                    }
                                    else
                                    {
                                        t.country += "/" + t.city_text;
                                    }

                                    no++;
                                    data.traveler_summary.Add(new travelerSummaryList
                                    {
                                        no = no.ToString(),
                                        emp_id = t.emp_id,
                                        emp_name = t.emp_name,
                                        emp_unit = t.emp_org,

                                        country = t.country,
                                        province = t.province,
                                        business_date = t.bus_date,
                                        traveler_date = t.travel_date,
                                        total_expenses = total_expenses.ToString(),

                                        appr_id = t.appr_emp_id,
                                        appr_name = t.appr_emp_name,
                                        take_action = t.take_action,
                                        ref_id = t.ref_id,

                                        appr_remark = t.appr_remark,
                                        appr_status = t.appr_status,

                                        //DevFix 20210714 0000 เพิ่มสถานะที่ Line/CAP --> 1:Draft , 2:Pendding , 3:Approve , 4:Revise , 5:Reject
                                        approve_status = t.approve_status,
                                        approve_remark = t.approve_remark,
                                        //DevFix 20210719 0000 เพิ่ม field OPT
                                        approve_opt = t.approve_opt,
                                        remark_opt = t.remark_opt,
                                        remark_cap = t.remark_cap,


                                        //DevFix 20211013 0000 เพิ่ม key เพื่อใช้ในการแยกข้อมูลออกแต่ละรายการ เนื่องจากเงื่อนไขเดิมข้อมูลซ้ำ --> เก็บค่าเป็น token id
                                        traveler_ref_id = t.traveler_ref_id,
                                    });

                                    if (t.take_action == "true")
                                        have_action = true;

                                }

                                data.grand_total = grand_total.ToString("#,##0.#0") + " THB";
                                data.total_travel = person_user.ToString() + " Person(s)";

                                //DevFix 20210721 0000 กรณีที่เป็น CAP ของ Line นั้นๆ ให้แสดงจำนวน Traveler และ Total
                                bCheckApproverLineInDoc = true;
                                bCheckTravelerListInDoc = true;
                            }
                        }
                        if (user_role != "1" && login_emp_requester_view == false) // ถ้าไม่ใช่ admin : ดึงของรายการคนอนุมัติคนอื่นมาแสดงด้วย
                        {
                            sql = "SELECT ct.ctn_name continent, cr.ct_name country, pv.pv_name province, ex.city_text, to_char(c.DTA_DOC_STATUS) action_status ";
                            sql += " , case when ex.DTE_BUS_FROMDATE is null then '' else to_char(ex.DTE_BUS_FROMDATE, 'dd MON rrrr') || ' - ' || to_char(ex.DTE_BUS_TODATE, 'dd MON rrrr') end as bus_date ";
                            sql += " , case when ex.DTE_TRAVEL_FROMDATE is null then '' else to_char(ex.DTE_TRAVEL_FROMDATE, 'dd MON rrrr') || ' - ' || to_char(ex.DTE_TRAVEL_TODATE, 'dd MON rrrr') end as travel_date ";
                            sql += " , u.employeeid emp_id, nvl(u.ENTITLE, '') || ' ' || u.ENFIRSTNAME || ' ' || u.ENLASTNAME as emp_name, u.ORGNAME emp_org ";
                            sql += " , u2.employeeid appr_emp_id, nvl(u2.ENTITLE, '') || ' ' || u2.ENFIRSTNAME || ' ' || u2.ENLASTNAME as appr_emp_name, u2.ORGNAME appr_emp_org ";
                            sql += " , to_char(ex.dte_token) ref_id ";
                            sql += " , to_char(ex.dte_total_expense) total ";
                            sql += " , ex.dte_id ";

                            //DevFix 20210714 0000 เพิ่มสถานะที่ Line/CAP --> 1:Draft , 2:Pendding , 3:Approve , 4:Revise , 5:Reject      
                            sql += " , case when ex.dte_appr_opt = 'true' then c.dta_action_status else (case when ex.dte_appr_opt = 'false' and nvl(ex.dte_appr_status,31) <> '31'  then '5' else '2' end) end approve_status";
                            sql += " , case when ex.dte_appr_opt = 'true' then c.dta_appr_remark else ex.dte_cap_appr_remark end approve_remark";

                            ////DevFix 20210719 0000 เพิ่ม field OPT 
                            sql += " , nvl(case when ex.dte_appr_opt = 'true' then to_char(nvl(c.dta_appr_status,'true')) else ex.dte_appr_opt end,'true') approve_opt";
                            sql += " , ex.dte_appr_remark as remark_opt";
                            sql += " , ex.dte_cap_appr_remark as remark_cap";

                            //DevFix 20210817 เพิ่ม key traveler_ref_id เพื่อใช้ในการแยกข้อมูลออกแต่ละรายการ เนื่องจากเงื่อนไขเดิมข้อมูลซ้ำ --> เก็บค่าเป็น token id
                            //เนื่องจากไม่สามารถ up dataset model ได้ให้ใช้ DTE_TOKEN แทน
                            sql += " , to_char(DTE_TOKEN) as traveler_ref_id";

                            //DevFix 20200821 1415 bug จุดนี้ ที่ยกเลิก query ใช้ -- ในการปิด query ทำให้ query หลังจากนั้นลงไปหาย
                            sql += " FROM bz_doc_traveler_approver c";
                            sql += " inner join BZ_DOC_TRAVELER_EXPENSE ex on c.dh_code = ex.dh_code and c.dta_travel_empid = ex.dte_emp_id ";
                            sql += " left join bz_users u on u.employeeid = ex.dte_emp_id ";
                            sql += " left join bz_users u2 on c.dta_appr_empid = u2.employeeid ";
                            sql += " left join bz_master_continent ct on ex.ctn_id = ct.ctn_id ";
                            sql += " left join bz_master_country cr on ex.ct_id = cr.ct_id ";
                            sql += " left join bz_master_province pv on ex.pv_id = pv.pv_id ";
                            sql += " WHERE c.DTA_APPR_EMPID != '" + user_id + "' ";
                            sql += " and c.dh_code = '" + value.id_doc + "' ";
                            sql += " and c.dta_type = 1 ";
                            sql += " and ex.DTE_EXPENSE_CONFIRM = 1 ";
                            sql += " and ex.dte_status = 1 ";
                            sql += " order by ex.dte_id ";

                            var docDetail3Head_2 = context.Database.SqlQuery<DocDetail3HeadModel>(sql).ToList();
                            if (docDetail3Head_2 != null)
                            {
                                if (docDetail3Head_2.Count > 0)
                                {
                                    if ((data.travel_date.Replace("-", "")).Trim() == "")
                                    {
                                        data.travel_date = docDetail3Head_2[0].travel_date ?? "";
                                        data.business_date = docDetail3Head_2[0].bus_date ?? "";

                                        continent = "";
                                        country = "";
                                        foreach (var h in docDetail3Head_2)
                                        {
                                            if (continent != h.continent)
                                            {
                                                if (!string.IsNullOrEmpty(continent)) continent += ", ";
                                                continent += h.continent;
                                            }

                                            if (!string.IsNullOrEmpty(country)) country += ", ";
                                            country += h.country;
                                        }
                                        data.continent = continent;
                                    }
                                    data.country = country ?? "";
                                    if (data.province == null)
                                    {
                                        data.province = docDetail3Head_2[0].province ?? "";
                                    }
                                }

                                //DevFix 20210721 0000 กรณีที่เป็น CAP ของ Line นั้นๆ ให้แสดงจำนวน Traveler และ Total 
                                decimal grand_total_user_other = 0;
                                var person_user_other = 0;
                                no2 = 0;

                                sql = @" select a.dta_travel_empid as emp_id
                                                from bz_doc_traveler_approver  a 
                                                where a.dta_type = 2 and a.dh_code  = '" + value.id_doc + "' and a.dta_appr_empid = '" + user_id + "'";
                                var apprlist = context.Database.SqlQuery<DocDetail3HeadModel>(sql).ToList();

                                foreach (var t in docDetail3Head_2)
                                {
                                    decimal total = toDecimal(t.total);

                                    //DevFix 20210324 1320 กรณีที่ไม่ใช่ admin ที่ดึงข้อมูล user อื่นมาไม่ต้องรวม Grand Total
                                    //grand_total += total;


                                    //DevFix 20210818 0000 แก้ไขตาม Front End
                                    //ถ้า oversea / overseatraining พี่ส่ง country / province มาใน field country เลย เพราะมันแค่เอามาแสดง 
                                    //แต่ถ้าเป็น local / localtraining ให้ส่ง province / city มาใน field province 
                                    if (doc_type.ToLower().IndexOf("local") > -1)
                                    {
                                        t.province += "/" + t.city_text;
                                    }
                                    else
                                    {
                                        t.country += "/" + t.city_text;
                                    }


                                    no++;
                                    data.traveler_summary.Add(new travelerSummaryList
                                    {
                                        no = no.ToString(),
                                        emp_id = t.emp_id,
                                        emp_name = t.emp_name,
                                        emp_unit = t.emp_org,
                                        country = t.country,
                                        province = t.province,

                                        business_date = t.bus_date,
                                        traveler_date = t.travel_date,
                                        total_expenses = total.ToString(),

                                        appr_id = t.appr_emp_id,
                                        appr_name = t.appr_emp_name,
                                        take_action = "false",
                                        ref_id = t.ref_id,
                                        appr_remark = "",
                                        appr_status = "",

                                        //DevFix 20210714 0000 เพิ่มสถานะที่ Line/CAP --> 1:Draft , 2:Pendding , 3:Approve , 4:Revise , 5:Reject
                                        approve_status = t.approve_status,
                                        approve_remark = t.approve_remark,
                                        //DevFix 20210719 0000 เพิ่ม field OPT
                                        approve_opt = t.approve_opt,
                                        remark_opt = t.remark_opt,
                                        remark_cap = t.remark_cap,


                                        //DevFix 20211013 0000 เพิ่ม key เพื่อใช้ในการแยกข้อมูลออกแต่ละรายการ เนื่องจากเงื่อนไขเดิมข้อมูลซ้ำ --> เก็บค่าเป็น token id
                                        traveler_ref_id = t.traveler_ref_id,
                                    });

                                    //DevFix 20210721 0000 กรณีที่เป็น CAP ของ Line นั้นๆ ให้แสดงจำนวน Traveler และ Total
                                    if (apprlist != null)
                                    {
                                        if (bCheckApproverLineInDoc == false)
                                        {
                                            if (login_emp_traveler_view == true)
                                            {
                                                if (t.emp_id.ToString() == user_id.ToString())
                                                {
                                                    if (t.approve_status != "5")
                                                    {

                                                        //DevFix 20210817 0000 กรณีที่ traverler 1 มี 2 รายการ ให้แสดงรายการเดียวพอ 
                                                        var add_traveler = true;
                                                        var row_check = data.traveler_list.Where(p => p.emp_id.Equals(t.emp_id)).FirstOrDefault();
                                                        if (row_check != null)
                                                        {
                                                            if (row_check.emp_id != "")
                                                            {
                                                                //กรณีที่มีข้อมูลไม่ต้อง add เพิ่ม
                                                                add_traveler = false;
                                                            }
                                                        }
                                                        if (add_traveler == true)
                                                        {
                                                            //DevFix 20210721 0000 ด้านบนเห็นคนเดียวพร้อมเงินเฉพาะคนที่ถูก approve ถูกแล้ว แต่ด้านล่างควรเห็น ทั้งหมดที่อยู่ภายใต้ CAP  
                                                            no2++;
                                                            data.traveler_list.Add(new travelerList
                                                            {
                                                                //DevFix 20200910 1658 เอาวงเล็บออก เหลือจุด . พอ 
                                                                text = no2.ToString() + ". " + t.emp_id + "  " + t.emp_name + " : " + t.emp_org,
                                                                //DevFix 20210817 0000 กรณีที่ traverler 1 มี 2 รายการ ให้แสดงรายการเดียวพอ --> เพิ่ม emp_id เพื่อเป็นเงื่อนไข
                                                                emp_id = t.emp_id,
                                                            });
                                                            person_user_other += 1;
                                                        }

                                                        //กรณีที่เป็น step line
                                                        grand_total_user_other += total;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var checkapprlist = apprlist.Where(p => p.emp_id.Equals(t.emp_id)).FirstOrDefault();
                                                if (t.approve_opt == "true")
                                                {
                                                    if (checkapprlist != null && checkapprlist.emp_id != null && checkapprlist.emp_id.ToString() != "")
                                                    {
                                                        //DevFix 20210817 0000 กรณีที่ traverler 1 มี 2 รายการ ให้แสดงรายการเดียวพอ 
                                                        var add_traveler = true;
                                                        var row_check = data.traveler_list.Where(p => p.emp_id.Equals(t.emp_id)).FirstOrDefault();
                                                        if (row_check != null)
                                                        {
                                                            if (row_check.emp_id != "")
                                                            {
                                                                //กรณีที่มีข้อมูลไม่ต้อง add เพิ่ม
                                                                add_traveler = false;
                                                            }
                                                        }
                                                        if (add_traveler == true)
                                                        {
                                                            //DevFix 20210721 0000 ด้านบนเห็นคนเดียวพร้อมเงินเฉพาะคนที่ถูก approve ถูกแล้ว แต่ด้านล่างควรเห็น ทั้งหมดที่อยู่ภายใต้ CAP  
                                                            no2++;
                                                            data.traveler_list.Add(new travelerList
                                                            {
                                                                //DevFix 20200910 1658 เอาวงเล็บออก เหลือจุด . พอ 
                                                                text = no2.ToString() + ". " + t.emp_id + "  " + t.emp_name + " : " + t.emp_org,
                                                                //DevFix 20210817 0000 กรณีที่ traverler 1 มี 2 รายการ ให้แสดงรายการเดียวพอ --> เพิ่ม emp_id เพื่อเป็นเงื่อนไข
                                                                emp_id = t.emp_id,
                                                            });
                                                            person_user_other += 1;
                                                        }


                                                        //กรณีที่เป็น step line
                                                        grand_total_user_other += total;
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }

                                //DevFix 20210721 0000 กรณีที่เป็น CAP ของ Line นั้นๆ ให้แสดงจำนวน Traveler และ Total
                                if (bCheckApproverLineInDoc == false)
                                {
                                    data.total_travel = person_user_other + " Person(s)";
                                    data.grand_total = (grand_total_user_other).ToString("#,##0.#0") + " THB";
                                }

                            }

                        }

                        //DevFix 20211013 0000 กรณีที่ Line submit to CAP แต่ CAP ยังไม่ได้ active --> ยังไม่ได้ใช้งานนะ เขียนไว้ก่อน ???
                        if (pf_doc_id == "3" || pf_doc_id == "4" || pf_doc_id == "5")
                        {
                            var bCheckPF_CAP = true;
                            sql = @" select to_char(count(1)) as approve_status
                                from BZ_DOC_TRAVELER_APPROVER a
                                where dta_action_status >  2 and a.dta_type = 2 and dh_code =  '" + value.id_doc + "'  ";
                            var dataCheck_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                            if (dataCheck_Def != null)
                            {
                                if (dataCheck_Def.Count > 0) { if (dataCheck_Def[0].approve_status.ToString() == "0") { bCheckPF_CAP = false; } }
                            }
                            sql = @" select dta_appr_level,dta_travel_empid as emp_id, (a.dta_action_status) as approve_status,  a.dta_appr_remark as approve_remark
                                ,to_char(nvl(a.dta_appr_status,'true')) as  approve_opt
                                from BZ_DOC_TRAVELER_APPROVER a 
                                where  dh_code =  '" + value.id_doc + "'  ";
                            if (pf_doc_id == "3")
                            {
                                sql += @" and a.dta_type = 1";
                            }
                            else if (pf_doc_id == "4")
                            {
                                if (bCheckPF_CAP == true) { sql += @" and a.dta_type = 2 and dta_action_status not in ('6') "; } else { sql += @" and a.dta_type = 1"; }
                            }
                            sql += @" order by dta_appr_level ";
                            var dataApporver_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                            #region DevFix 20211013 0000 กรณีที่เป็น step cap ให้ตรวจสอบ line ว่ามีการ reject หรือไม่
                            var dataApporverLine_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                            var dataApporverLine2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                            var dataApporverLine3_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                            var dataApporverCAP_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                            var dataApporverCAP2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                            var dataApporverCAP3_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                            if (pf_doc_id == "3" || pf_doc_id == "4" || pf_doc_id == "5")
                            {
                                //line approve
                                sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '3' as approve_status, dte_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where nvl(dte_appr_status,31) = '32' and dte_appr_opt = 'true' and dte_status = 1 and dte_appr_status <> 23
                                         and dh_code = '" + value.id_doc + "' ";
                                dataApporverLine_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                                //line reject
                                sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '5' as approve_status, dte_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where nvl(dte_appr_status,31) = '32' and ((dte_appr_opt = 'false' and dte_status = 1) or dte_appr_status = 30 )
                                         and dh_code = '" + value.id_doc + "' ";
                                dataApporverLine2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                                //line pendding
                                sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '2' as approve_status, dte_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where dte_status = 1 and dte_appr_status = 32 and dh_code = '" + value.id_doc + "' ";
                                dataApporverLine3_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                                //cap approve
                                sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '3' as approve_status, dte_cap_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where nvl(dte_cap_appr_status,41) = '42' and (dte_cap_appr_opt = 'true' and dte_appr_opt = 'true') and dte_status = 1
                                         and dh_code = '" + value.id_doc + "' ";
                                dataApporverCAP_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                                //cap reject
                                sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '5' as approve_status, dte_cap_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where nvl(dte_cap_appr_status,41) = '42' and ( (dte_cap_appr_opt = 'false' and dte_status = 1) or (dte_appr_opt = 'false' and dte_appr_status = 32) or dte_cap_appr_status = 40 )
                                         and dh_code = '" + value.id_doc + "' ";
                                dataApporverCAP2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                            }
                            #endregion DevFix 20211013 0000 กรณีที่เป็น step cap ให้ตรวจสอบ line ว่ามีการ reject หรือไม่
                            #region DevFix 20211013 0000 update status  
                            foreach (var item in data.traveler_summary)
                            {
                                if (item.emp_id == "00001393" && item.country == "Denmark")
                                {
                                    var xdebug = "";
                                }
                                var approve_status = "1";
                                var approve_remark = "";
                                var approve_opt = "";
                                var appr_remark = "";
                                var check_data = dataApporver_Def.Where(t => t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id);

                                #region DevFix 20211013 0000 กรณีที่เป็น step cap ให้ตรวจสอบ line ว่ามีการ reject หรือไม่
                                if (pf_doc_id == "4" || pf_doc_id == "5")
                                {
                                    if (bCheckPF_CAP == true)
                                    {
                                        check_data = dataApporverCAP_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                        if (check_data.Count() == 0)
                                        {
                                            check_data = dataApporverCAP2_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                            if (check_data.Count() == 0)
                                            {
                                                check_data = dataApporverCAP3_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                                if (check_data.Count() == 0)
                                                {
                                                    //กรณีที่มีข้อมูล line ให้เอา remark line มาแสดง
                                                    check_data = dataApporverLine2_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                                    if (check_data.Count() == 0)
                                                    {
                                                        check_data = dataApporverLine_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                                    }
                                                }
                                            }
                                        }

                                        //กรณีที่มีข้อมูล cap ให้เอา remark cap มาแสดง
                                        appr_remark = item.remark_cap;
                                    }
                                    else
                                    {
                                        check_data = dataApporverLine2_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                        if (check_data.Count() == 0)
                                        {
                                            check_data = dataApporverLine3_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                        }
                                    }
                                }
                                else if (pf_doc_id == "3")
                                {
                                    //กรณีที่มีข้อมูล line ให้เอา remark line มาแสดง
                                    appr_remark = item.remark_opt;

                                    check_data = dataApporverLine_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                    if (check_data.Count() == 0)
                                    {
                                        check_data = dataApporverLine2_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                        if (check_data.Count() == 0)
                                        {
                                            check_data = dataApporverLine3_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                        }
                                    }
                                }
                                #endregion DevFix 20211013 0000 กรณีที่เป็น step cap ให้ตรวจสอบ line ว่ามีการ reject หรือไม่

                                var bcheck_change_status = false;
                                foreach (var item2 in check_data)
                                {
                                    approve_status = item2.approve_status;
                                    approve_opt = item2.approve_opt;

                                    if (approve_remark != "") { approve_remark += ","; }
                                    if (item2.approve_remark != "") { approve_remark += item2.approve_remark; }

                                    bcheck_change_status = true;

                                }
                                if (approve_status == "" || bcheck_change_status == false) { approve_status = item.approve_status; }
                                if (approve_remark == "") { approve_remark = item.approve_remark; }

                                item.approve_status = approve_status;
                                item.approve_remark = approve_remark; // remark btn action
                                item.approve_opt = approve_opt;

                                item.appr_remark = appr_remark;
                            }
                            #endregion DevFix 20211013 0000 update status  
                        }

                        data.after_trip.opt1 = (docHead[0].DH_AFTER_TRIP_OPT1 ?? "") == "Y" ? "true" : "false";
                        data.after_trip.opt2.status = (docHead[0].DH_AFTER_TRIP_OPT2 ?? "") == "Y" ? "true" : "false";
                        data.after_trip.opt2.remark = docHead[0].DH_AFTER_TRIP_OPT2_REMARK ?? "";
                        data.after_trip.opt3.status = (docHead[0].DH_AFTER_TRIP_OPT3 ?? "") == "Y" ? "true" : "false";
                        data.after_trip.opt3.remark = docHead[0].DH_AFTER_TRIP_OPT3_REMARK ?? "";


                        string pf_doc_status = docHead[0].doc_status.ToString().Substring(0, 1);
                        if (pf_doc_status == "1")
                        {
                            data.button.part_i = "true";
                        }
                        else if (pf_doc_status == "2")
                        {
                            data.button.part_i = "true";
                            data.button.part_ii = "true";
                        }
                        else if (pf_doc_status == "3")
                        {
                            data.button.part_i = "true";
                            data.button.part_ii = "true";
                            data.button.part_iii = "true";
                        }
                        else if (pf_doc_status == "4")
                        {
                            data.button.part_i = "true";
                            data.button.part_ii = "true";
                            data.button.part_iii = "true";
                            data.button.part_iiii = "true";
                            data.button.part_cap = "true";
                        }
                        else if (pf_doc_status == "5")
                        {
                            data.button.part_i = "true";
                            data.button.part_ii = "true";
                            data.button.part_iii = "true";
                            data.button.part_iiii = "true";
                            data.button.part_cap = "true";
                        }

                        if ((action != null && action.Count() > 0) && (have_action == true))
                        {
                            data.button.save = "true";
                            data.button.cancel = "true";
                            data.button.reject = "true";
                            data.button.revise = "true";
                            data.button.approve = "true";
                        }

                        #region DevFix 20211012 0000 กรณีที่เป็น pmdv admin ทำ training ให้ button.save กับ button.approve = true 
                        string doc_status_tab = docHead[0].doc_status.ToString().Substring(0, 1);
                        string doc_status_chk = docHead[0].doc_status.ToString();
                        if (doc_status_tab == "5" || doc_status_chk == "10" || doc_status_chk == "20" || doc_status_chk == "30" || doc_status_chk == "40" || doc_status_chk == "50") { }
                        else
                        {
                            if (value.id_doc.IndexOf("T") > -1 && doc_status_tab == "3")
                            {
                                data.button.approve = "false";
                                data.button.cancel = "false";
                                data.button.reject = "false";
                                data.button.revise = "false";
                                data.button.save = "false";
                                data.button.submit = "false";

                                sql = @"select distinct to_char(pmdv_admin) as type 
                                    from bz_data_manage where pmdv_admin = 'true' and emp_id = '" + user_id + "' ";
                                var pmdv_admin_list = context.Database.SqlQuery<approverModel>(sql).ToList();
                                if (pmdv_admin_list != null)
                                {
                                    if (pmdv_admin_list.Count > 0)
                                    {
                                        if (pmdv_admin_list[0].type.ToString() == "true")
                                        {
                                            data.button.approve = "true";
                                            data.button.cancel = "true";
                                            data.button.reject = "true";
                                            data.button.revise = "true";
                                            data.button.save = "true";
                                            data.button.submit = "true";

                                        }
                                    }
                                }
                            }
                        }
                        #endregion DevFix 20211012 0000 กรณีที่เป็น pmdv admin ทำ training ให้ button.save กับ button.approve = true 

                    }


                }
            }
            catch (Exception ex)
            {
                data.msg_remark = "error:" + sql;
                throw;
            }


            return data;
        }

        public DocDetail3OutModel searchDetail4(DocDetail3Model value)
        {
            var data = new DocDetail3OutModel();
            var docHead = new List<DocList3Model>();
            string doc_type = "";
            string user_id = "";
            string user_role = "";
            Boolean have_action = false;

            data.button.approve = "false";
            data.button.cancel = "false";
            data.button.reject = "false";
            data.button.revise = "false";
            data.button.save = "false";
            data.button.submit = "false";

            data.button.part_i = "true";
            data.button.part_ii = "true";
            data.button.part_iii = "true";
            data.button.part_iiii = "true";
            data.button.part_cap = "false";

            decimal grand_total = 0;
            var pf_doc_id = "";
            string sql = "";

            var TypeModel = new List<TypeModel>();
            try
            {
                using (TOPEBizEntities context = new TOPEBizEntities())
                {
                    sql = "";
                    string document_status = "";

                    sql = "delete from BZ_DOC_TRAVELER_EXPENSE  ";
                    sql += " where DH_CODE='" + value.id_doc + "' and dte_status =0  ";
                    context.Database.ExecuteSqlCommand(sql);

                    var login_empid = new List<SearchUserModel>();
                    sql = "SELECT  a.USER_NAME, a.user_id, to_char(u.ROLE_ID) user_role ";
                    sql += "FROM bz_login_token a left join bz_users u on a.user_id=u.employeeid ";
                    sql += " WHERE a.TOKEN_CODE ='" + value.token + "' ";
                    login_empid = context.Database.SqlQuery<SearchUserModel>(sql).ToList();
                    if (login_empid != null && login_empid.Count() > 0)
                    {
                        user_id = login_empid[0].user_id ?? "";
                        user_role = login_empid[0].user_role ?? "";
                    }


                    #region ตรวจสอบสถานะใบงาน
                    var docHeadStatus = new List<DocHeadModel>();
                    sql = " select to_char(dh_doc_status) as document_status from bz_doc_head h where h.dh_code = '" + value.id_doc + "' ";
                    docHeadStatus = context.Database.SqlQuery<DocHeadModel>(sql).ToList();
                    if (docHeadStatus != null && docHeadStatus.Count > 0)
                    {
                        document_status = docHeadStatus[0].document_status;
                    }
                    #endregion ตรวจสอบสถานะใบงาน

                    //กรณีที่เป็น pmdv admin, pmsv_admin
                    if (value.id_doc.IndexOf("T") > -1)
                    {
                        sql = @" select emp_id as user_id from bz_data_manage where (pmsv_admin = 'true' or pmdv_admin = 'true') and emp_id = '" + user_id + "' ";
                        var adminlist = context.Database.SqlQuery<SearchUserModel>(sql).ToList();
                        if (adminlist != null)
                        {
                            if (adminlist.Count > 0) { user_role = "1"; }
                        }
                    }

                    sql = " SELECT  emp_id user_id, to_char(action_status) action_status  ";
                    sql += " FROM bz_doc_action b ";
                    sql += " WHERE b.dh_code = '" + value.id_doc + "' ";

                    if (user_role == "1")
                        //DevFix 20200901 2340 กรณีที่ admin ไม่ต้องเช็ค status 
                        sql += " and b.emp_id <> 'admin' ";
                    else
                        sql += " and b.emp_id = '" + user_id + "' ";

                    sql += " and action_status = 1 ";
                    sql += " and b.tab_no = 4 ";
                    var action = context.Database.SqlQuery<SearchUserModel>(sql).ToList();

                    #region DevFix 20210806 0000 กรณีที่ไม่ใช่ admin ให้ตรวจสอบ emp ว่าเป็น traveler 
                    var login_emp_traveler_view = false;
                    var login_emp_requester_view = false;
                    if (user_role != "1")
                    {
                        var emp_type = new List<SearchUserModel>();
                        sql = @"select to_char(t.user_type ) as user_type
                            from (
                            select dh_code as doc_id, 1 as user_type, a.dta_travel_empid as emp_id
                            from  bz_doc_traveler_approver a where a.dta_type = 1
                            union
                            select dh_code as doc_id, 2 as user_type, a.dta_appr_empid as emp_id
                            from  bz_doc_traveler_approver a where a.dta_type = 1 
                            union
                            select dh_code as doc_id, 3 as user_type, a.dta_appr_empid as emp_id
                            from  bz_doc_traveler_approver a  where a.dta_type = 2  
                            union
                             select dh_code as doc_id, 4 as user_type, a.dh_behalf_emp_id as emp_id
                             from  bz_doc_head a
                             union 
                             select dh_code as doc_id, 4 as user_type, a.dh_initiator_empid as emp_id
                             from  bz_doc_head a
                             union 
                             select dh_code as doc_id, 4 as user_type, a.dh_create_by as emp_id
                             from  bz_doc_head a
                            )t  where t.user_type in (1,3,4) and t.doc_id ='" + value.id_doc + "' and t.emp_id = '" + user_id + "' ";
                        sql += " order by user_type desc ";
                        emp_type = context.Database.SqlQuery<SearchUserModel>(sql).ToList();
                        if (emp_type != null && emp_type.Count() > 0)
                        {
                            if (emp_type[0].user_type.ToString() == "1") { login_emp_traveler_view = true; }
                            if (emp_type[0].user_type.ToString() == "4") { login_emp_requester_view = true; }
                        }
                    }
                    else { login_emp_traveler_view = false; login_emp_requester_view = false; }
                    #endregion DevFix 20210806 0000 กรณีที่ไม่ใช่ admin ให้ตรวจสอบ emp ว่าเป็น traveler 

                    //หาว่า  type นี้เป็น oversea หรือ local
                    sql = " select a.DH_CODE , DH_TYPE type, DH_EXPENSE_OPT1 checkbox_1, DH_EXPENSE_OPT2 checkbox_2, DH_EXPENSE_REMARK remark " + Environment.NewLine;
                    sql += ", to_char(DH_DOC_STATUS) doc_status ";
                    sql += " , b.TS_NAME document_status ";
                    sql += ", DH_AFTER_TRIP_OPT1, DH_AFTER_TRIP_OPT2, DH_AFTER_TRIP_OPT3, DH_AFTER_TRIP_OPT2_REMARK, DH_AFTER_TRIP_OPT3_REMARK";
                    sql += " , to_char(nvl(DH_TOTAL_PERSON, 0)) || ' Person(s)' person ";
                    sql += " , a.DH_TOPIC topic ";

                    sql += "  , case when DH_BUS_FROMDATE is null then '' else to_char(DH_BUS_FROMDATE, 'dd Mon rrrr') || ' - ' || to_char(DH_BUS_TODATE, 'dd Mon rrrr') end as bus_date ";
                    sql += "  , case when DH_TRAVEL_FROMDATE is null then '' else to_char(DH_TRAVEL_FROMDATE, 'dd Mon rrrr') || ' - ' || to_char(DH_TRAVEL_TODATE, 'dd Mon rrrr') end as travel_date ";
                    sql += " , a.DH_CITY city_text ";
                    sql += " , d.ct_name country ";
                    sql += " , e.ctn_name continent, a.DH_TYPE_FLOW ";

                    sql += " from BZ_DOC_HEAD a left join BZ_MASTER_STATUS b on a.DH_DOC_STATUS=b.TS_ID ";
                    sql += " left join BZ_DOC_COUNTRY c on a.dh_code=c.dh_code ";
                    sql += " left join BZ_MASTER_COUNTRY d on c.ct_id=d.ct_id ";
                    sql += " left join BZ_MASTER_CONTINENT e on d.ctn_id=e.ctn_id ";
                    sql += " WHERE a.DH_CODE = '" + value.id_doc + "' " + Environment.NewLine;
                    sql += " order by e.ctn_name ";

                    docHead = context.Database.SqlQuery<DocList3Model>(sql).ToList();
                    if (docHead != null)
                    {
                        try
                        {
                            pf_doc_id = docHead[0].doc_status.Substring(0, 1);
                        }
                        catch { }

                        //DevFix 20210527 0000 เพิ่มข้อมูล ประเภทใบงานเป็น 1:flow, 2:not flow, 3:training เก็บไว้ที่  BZ_DOC_HEAD.DH_TYPE_FLOW
                        data.type_flow = docHead[0].DH_TYPE_FLOW ?? "1";
                        doc_type = docHead[0].type ?? "";

                        var sql_select = "";
                        var sql_from = "";
                        var sql_from_traveler = "";
                        sql_select = @" SELECT distinct ct.ctn_name continent, cr.ct_name country, pv.pv_name province, ex.city_text, to_char(c.DTA_DOC_STATUS) action_status 
                             , case when c.DTA_DOC_STATUS = 41 then 'true' else 'false' end take_action
                             , case when c.DTA_APPR_STATUS is null or nvl(c.DTA_DOC_STATUS, 31) = 31  then nvl(ex.dte_cap_appr_opt,'true') else nvl(c.DTA_APPR_STATUS, 'true') end appr_status

                            --DevFix 20210710 0000 เปลี่ยน field
                            --  , c.DTA_APPR_REMARK appr_remark
                             , ex.DTE_CAP_APPR_REMARK appr_remark

                             , case when ex.DTE_BUS_FROMDATE is null then '' else to_char(ex.DTE_BUS_FROMDATE, 'dd Mon rrrr') || ' - ' || to_char(ex.DTE_BUS_TODATE, 'dd Mon rrrr') end as bus_date
                             , case when ex.DTE_TRAVEL_FROMDATE is null then '' else to_char(ex.DTE_TRAVEL_FROMDATE, 'dd Mon rrrr') || ' - ' || to_char(ex.DTE_TRAVEL_TODATE, 'dd Mon rrrr') end as travel_date
                             , u.employeeid emp_id, nvl(u.ENTITLE, '') || ' ' || u.ENFIRSTNAME || ' ' || u.ENLASTNAME as emp_name, u.ORGNAME emp_org
                             , u2.employeeid appr_emp_id, nvl(u2.ENTITLE, '') || ' ' || u2.ENFIRSTNAME || ' ' || u2.ENLASTNAME as appr_emp_name, u2.ORGNAME appr_emp_org
                             , to_char(ex.dte_token) ref_id
                             , to_char(ex.dte_total_expense) total

                            ----DevFix 20210714 0000 เพิ่มสถานะที่ Line/ CAP-- > 1:Draft , 2:Pendding , 3:Approve , 4:Revise , 5:Reject
                            -- , c.dta_action_status as approve_status
                            -- , c.dta_appr_remark as approve_remark
                             , case when ex.dte_appr_opt = 'true' then c.dta_action_status else '5' end approve_status
                             , case when ex.dte_cap_appr_status is null  then c.dta_appr_remark else ex.dte_cap_appr_remark end approve_remark

                            ----DevFix 20210719 0000 เพิ่ม field OPT
                             , case when ex.dte_appr_opt = 'true' then to_char(nvl(c.dta_appr_status,'true')) else ex.dte_appr_opt end approve_opt

                             , case when ex.dte_appr_opt = 'true' then ex.dte_cap_appr_remark else ex.dte_appr_remark end remark_opt
                             , ex.dte_cap_appr_remark as remark_cap

                            , ex.dte_id

                            --DevFix 20210817 เพิ่ม key traveler_ref_id เพื่อใช้ในการแยกข้อมูลออกแต่ละรายการ เนื่องจากเงื่อนไขเดิมข้อมูลซ้ำ-- > เก็บค่าเป็น token id
                            --เนื่องจากไม่สามารถ up dataset model ได้ให้ใช้ DTE_TOKEN แทน
                             , to_char(DTE_TOKEN) as traveler_ref_id ";

                        //DevFix 20210721 0000 ด้านบนเห็นคนเดียวพร้อมเงินเฉพาะคนที่ถูก approve ถูกแล้ว แต่ด้านล่างควรเห็น ทั้งหมดที่อยู่ภายใต้ CAP  
                        //sql_from += "  and ex.dte_appr_status in ( 32,41) and ex.dte_appr_opt = 'true' ";
                        sql_from += $@" FROM  bz_doc_traveler_approver c 
                                        inner join BZ_DOC_TRAVELER_EXPENSE ex on c.dh_code = ex.dh_code and c.dta_travel_empid = ex.dte_emp_id     
                                        left join bz_users u on u.employeeid = ex.dte_emp_id     
                                        left join bz_users u2 on c.dta_appr_empid = u2.employeeid     
                                        left join bz_master_continent ct on ex.ctn_id = ct.ctn_id    
                                        left join bz_master_country cr on ex.ct_id = cr.ct_id     
                                        left join bz_master_province pv on ex.pv_id = pv.pv_id     
                                        where 1=1     
                                        and c.dh_code = '{value.id_doc}' 
                                        and c.dta_status = 1 
                                        and c.dta_type = 2 

                                        --DevFix 20210721 0000 ด้านบนเห็นคนเดียวพร้อมเงินเฉพาะคนที่ถูก approve ถูกแล้ว แต่ด้านล่างควรเห็น ทั้งหมดที่อยู่ภายใต้ CAP  
                                        --  and ex.dte_appr_status in ( 32,41) and ex.dte_appr_opt = 'true' 
                                        and ex.dte_appr_status in (32,31,30,41)  
                                        and ex.dh_code in (select dh_code from bz_doc_head where dh_doc_status in ('41','40','50')) ";

                        // query ชุดนี้ให้เอาไว้ล่างสุด 
                        if (user_role != "1" && login_emp_requester_view == false)
                        {
                            if (login_emp_traveler_view == true)
                            {
                                sql_from_traveler = sql_from + " and c.DTA_TRAVEL_EMPID = '" + user_id + "' ";
                            }
                            sql_from += " and c.DTA_APPR_EMPID = '" + user_id + "' ";
                        }


                        //*****************************************
                        sql = sql_select + sql_from + " order by ex.dte_id ";

                        //DevFix 20210329 1200 กรณีที่เป็น Admin เนื่องจาก emp 1 คนอาจจะมี ได้มากกว่า  1 aprrover ทำให้ตอน calulate person & total ซ้ำได้ต้องกรองตาม emp  
                        //ต้องแยก table
                        var sqlmain = sql;
                        if (user_role == "1" && login_emp_requester_view == false)
                        {
                            sqlmain = @" select distinct continent,country,province,city_text,null as action_status,null as take_action,null as appr_status
                                     ,appr_remark,bus_date,travel_date
                                     ,emp_id,emp_name,emp_org
                                     ,null as appr_emp_id,null as appr_emp_name,null as appr_emp_org
                                     ,ref_id,total,dte_id,approve_status
                                     from (" + sql + ")t order by dte_id";
                        }
                        var docDetail3Head = context.Database.SqlQuery<DocDetail3HeadModel>(sqlmain).ToList();
                        var docDetail3HeadSummary = context.Database.SqlQuery<DocDetail3HeadModel>(sql).ToList();

                        var bcheck_data_head = false;
                        if (user_role != "1" && login_emp_requester_view == false)
                        {
                            if (docDetail3Head != null) { if (docDetail3Head.Count() > 0) { bcheck_data_head = true; } }
                            if (bcheck_data_head == false && login_emp_traveler_view == true)
                            {
                                //กรณีที่เป็น traverler ข้อมูล head จะไม่มีให้ดึงใหม่
                                sql = sql_select + sql_from_traveler + " order by ex.dte_id ";
                            }
                        }

                        #region รายละเอียด head 
                        var sql_p = " select * from ( select distinct min(dte_id) as dte_id, country as PROVINCE from (" + sql + ")t group by country)t2 order by  dte_id";
                        var sql_c = " select * from ( select distinct min(dte_id) as dte_id, country as PROVINCE, CITY_TEXT from (" + sql + ")t group by  country, city_text )t2 order by  dte_id ";
                        if (doc_type.ToLower().IndexOf("local") > -1)
                        {
                            sql_p = " select * from ( select distinct min(dte_id) as dte_id, PROVINCE from (" + sql + ")t group by PROVINCE)t2 order by  dte_id ";
                            sql_c = " select * from ( select distinct min(dte_id) as dte_id, PROVINCE, CITY_TEXT from (" + sql + ")t group by PROVINCE, CITY_TEXT)t2 order by  dte_id";
                        }
                        var sql_date = @"select to_char(min(ex.DTE_BUS_FROMDATE), 'dd Mon rrrr') || ' - ' || to_char(max(ex.DTE_BUS_TODATE), 'dd Mon rrrr') as bus_date 
                                         , to_char(min(ex.DTE_TRAVEL_FROMDATE), 'dd Mon rrrr') || ' - ' || to_char(max(ex.DTE_TRAVEL_TODATE), 'dd Mon rrrr') as travel_date  ";
                        if (user_role != "1" && login_emp_requester_view == false)
                        {
                            if (bcheck_data_head == false && login_emp_traveler_view == true)
                            {
                                sql_date += "  " + sql_from_traveler + "";
                            }
                            else { sql_date += sql_from + ""; }
                        }
                        else { sql_date += sql_from + ""; }

                        var docDetailProvince = context.Database.SqlQuery<DocDetail3HeadModel>(sql_p).ToList();
                        var docDetailCity = context.Database.SqlQuery<DocDetail3HeadModel>(sql_c).ToList();
                        var docDetailDate = context.Database.SqlQuery<DocDetail3HeadModel>(sql_date).ToList();
                        #endregion รายละเอียด head

                        var role_in_doc = "";//1 : traverler, 2 : line, 3 : cap
                        if (user_role != "1" && login_emp_requester_view == false)
                        {
                            sql = @" select to_char(count(1)) as action_status
                                 from bz_doc_traveler_approver c
                                 where c.dta_type = 2
                                 and c.dh_code = '" + value.id_doc + "' and c.dta_appr_empid = '" + user_id + "' ";
                            var role_cap = context.Database.SqlQuery<DocDetail3HeadModel>(sql).ToList();
                            if (role_cap != null && role_cap.Count > 0 && role_cap[0].action_status != "0")
                            { role_in_doc = "3"; }
                            if (role_in_doc != "") { goto next_role_in_doc; }

                            sql = @" select to_char(count(1)) as action_status
                                 from bz_doc_traveler_approver c
                                 where c.dta_type = 1
                                 and c.dh_code = '" + value.id_doc + "' and c.dta_appr_empid = '" + user_id + "' ";
                            var role_line = context.Database.SqlQuery<DocDetail3HeadModel>(sql).ToList();
                            if (role_line != null && role_line.Count > 0 && role_line[0].action_status != "0")
                            { role_in_doc = "2"; }
                            if (role_in_doc != "") { goto next_role_in_doc; }

                            sql = @" select to_char(count(1)) as action_status
                                 from bz_doc_traveler_approver c
                                 where c.dta_type = 1
                                 and c.dh_code = '" + value.id_doc + "'  and c.dta_travel_empid = '" + user_id + "' ";
                            var role_traverler = context.Database.SqlQuery<DocDetail3HeadModel>(sql).ToList();
                            if (role_traverler != null && role_traverler.Count > 0 && role_traverler[0].action_status != "0")
                            { role_in_doc = "1"; }
                            if (role_in_doc != "") { goto next_role_in_doc; }
                        next_role_in_doc:;
                        }

                        //DevFix 20210527 0000 file
                        #region DevFix 20210527 0000 เพิ่มข้อมูลไฟล์แนบ
                        sql = @" select DH_CODE, to_CHAR(DF_ID) as DF_ID, DF_NAME, DF_PATH, DF_REMARK 
                                from BZ_DOC_FILE where DH_CODE = '" + value.id_doc + "' order by  DF_ID ";
                        var docFile = context.Database.SqlQuery<DocFileListModel>(sql).ToList();
                        if (docFile.Count == 0)
                        {
                        }
                        data.docfile = docFile;


                        #endregion DevFix 20210527 0000 เพิ่มข้อมูลไฟล์แนบ

                        data.document_status = docHead[0].document_status ?? "";
                        data.topic = docHead[0].topic ?? "";
                        data.total_travel = "0 Person(s)";
                        data.grand_total = "0";
                        data.checkbox_1 = (docHead[0].checkbox_1 ?? "") == "Y" ? "true" : "false";
                        data.checkbox_2 = (docHead[0].checkbox_2 ?? "") == "Y" ? "true" : "false";
                        data.remark = docHead[0].remark ?? "";

                        data.travel_date = docHead[0].travel_date ?? "";
                        data.business_date = docHead[0].bus_date ?? "";

                        string continent = "";
                        string country = "";
                        foreach (var h in docHead)
                        {
                            if (continent != h.continent)
                            {
                                if (!string.IsNullOrEmpty(continent)) continent += ", ";
                                continent += h.continent;
                            }

                            if (!string.IsNullOrEmpty(country)) country += ", ";
                            country += h.country;
                        }
                        data.continent = continent;

                        #region  DevFix 20210817 0000 กรณีที่ traverler 1 มี 2 รายการ 
                        // Country or Province / City ให้ group ตามประเทศ/จังหวัด และ city
                        var country_text = "";
                        var city_text = "";
                        foreach (var p in docDetailProvince)
                        {
                            city_text = "";
                            if (country_text != "") { country_text += ", "; }
                            var findDataCity = docDetailCity.Where(a => a.province == p.province).ToList();
                            if (findDataCity != null && findDataCity.Count > 0)
                            {
                                foreach (var c in findDataCity)
                                {
                                    if (city_text != "") { city_text += ","; }
                                    city_text += c.city_text + "";
                                }
                                if (city_text != "") { country_text += p.province + "/" + city_text; }
                            }
                        }
                        data.country = country_text;
                        if (doc_type.ToLower().IndexOf("local") > -1) { data.province = ""; }

                        data.travel_date = docDetailDate[0].travel_date ?? "";
                        data.business_date = docDetailDate[0].bus_date ?? "";

                        #endregion  DevFix 20210817 0000 กรณีที่ traverler 1 มี 2 รายการ 



                        //DevFix 20210721 0000 ด้านบนเห็นคนเดียวพร้อมเงินเฉพาะคนที่ถูก approve ถูกแล้ว แต่ด้านล่างควรเห็น ทั้งหมดที่อยู่ภายใต้ CAP 
                        var bCheckApproverLineInDoc = false;
                        var bCheckApproverCAPInDoc = false;
                        var bCheckTraveler_list = false;
                        var total_travel = 0;
                        var no = 0;
                        var no2 = 0;



                        //DevFix 20221121 0000 กรณีที่ traverler 1 มีมากกว่า 1 cap ให้ใช้ ค่าใช้จ่าย รายการเดียวพอ --> ใช้ dte_id รหัสข้อมูลรายการ เป็น key 
                        List<travelerList> total_list = new List<Models.travelerList>();

                        if (docDetail3Head != null && docDetail3Head.Count() > 0)
                        {
                            //data.total_travel = docDetail3Head.Count().ToString() + " Person(s)";
                            //data.province = docDetail3Head[0].province ?? "";
                            if (doc_type.ToLower().IndexOf("local") > -1)
                            { }
                            else { data.province = docDetail3Head[0].province ?? ""; }

                            foreach (var t in docDetail3Head)
                            {
                                //DevFix 20210721 0000 ด้านบนเห็นคนเดียวพร้อมเงินเฉพาะคนที่ถูก approve ถูกแล้ว แต่ด้านล่างควรเห็น ทั้งหมดที่อยู่ภายใต้ CAP 
                                if ((t.take_action != null && t.take_action == "true") ||
                                    (t.take_action != null && t.take_action == "false" && t.action_status != "41") ||
                                    ((user_role == "1" || login_emp_requester_view == true) && t.approve_status != "5"))
                                {
                                    var bDataShows = false;
                                    if (t.approve_opt == "true" && t.action_status != "40")
                                    {
                                        bDataShows = true;
                                    }
                                    if ((user_role == "1" || login_emp_requester_view == true) && t.approve_status != "5")
                                    {
                                        bDataShows = true;
                                    }

                                    if (bDataShows == true)
                                    {
                                        //DevFix 20210817 0000 กรณีที่ traverler 1 มี 2 รายการ ให้แสดงรายการเดียวพอ 
                                        var add_traveler = true;
                                        //20231108 NoppadonK แก้ไข Grand Total ไม่ถูกต้อง
                                        var row_check = data.traveler_list
                                                        .Where(p => p.emp_id.Equals(t.emp_id) &&
                                                                    p.businessDate == t.bus_date &&
                                                                    p.country == t.country).FirstOrDefault();
                                        if (row_check != null)
                                        {
                                            if (row_check.emp_id != "")
                                            {
                                                //กรณีที่มีข้อมูลไม่ต้อง add เพิ่ม
                                                add_traveler = false;
                                            }
                                        }
                                        if (add_traveler == true)
                                        {
                                            no++;
                                            data.traveler_list.Add(new travelerList
                                            {
                                                //DevFix 20200910 1658 เอาวงเล็บออก เหลือจุด . พอ 
                                                text = no.ToString() + ".  " + t.emp_id + "  " + t.emp_name + " : " + t.emp_org,

                                                //DevFix 20210817 0000 กรณีที่ traverler 1 มี 2 รายการ ให้แสดงรายการเดียวพอ --> เพิ่ม emp_id เพื่อเป็นเงื่อนไข
                                                emp_id = t.emp_id,
                                                //20231108 NoppadonK แก้ไข Grand Total ไม่ถูกต้อง
                                                businessDate = t.bus_date,
                                                country = t.country
                                            });
                                            total_travel += 1;
                                        }


                                        //DevFix 20221121 0000 กรณีที่ traverler 1 มีมากกว่า 1 cap ให้ใช้ ค่าใช้จ่าย รายการเดียวพอ --> ใช้ dte_id รหัสข้อมูลรายการ เป็น key 
                                        //decimal total = toDecimal(t.total); 
                                        if (true)
                                        {
                                            var add_total = true;
                                            var row_check_total = total_list.Where(p => p.emp_id.Equals(t.emp_id)).FirstOrDefault();
                                            if (row_check != null)
                                            {
                                                if (row_check.emp_id != "")
                                                {
                                                    add_total = false;
                                                }
                                            }

                                            if (add_total == true)
                                            {
                                                total_list.Add(new travelerList
                                                {
                                                    emp_id = t.dte_id.ToString(),
                                                });
                                                grand_total += toDecimal(t.total);
                                            }
                                        }

                                        bCheckApproverCAPInDoc = true;
                                        bCheckTraveler_list = true;
                                    }
                                }
                            }

                            //DevFix 20210329 1200 กรณีที่เป็น Admin เนื่องจาก emp 1 คนอาจจะมี ได้มากกว่า  1 aprrover ทำให้ตอน calulate person & total ซ้ำได้ต้องกรองตาม emp  

                            no = 0;
                            foreach (var t in docDetail3HeadSummary)
                            {
                                //DevFix 20210818 0000 แก้ไขตาม Front End
                                //ถ้า oversea / overseatraining พี่ส่ง country / province มาใน field country เลย เพราะมันแค่เอามาแสดง 
                                //แต่ถ้าเป็น local / localtraining ให้ส่ง province / city มาใน field province 
                                if (doc_type.ToLower().IndexOf("local") > -1)
                                {
                                    t.province += "/" + t.city_text;
                                }
                                else
                                {
                                    t.country += "/" + t.city_text;
                                }


                                no++;
                                decimal total = toDecimal(t.total);
                                data.traveler_summary.Add(new travelerSummaryList
                                {
                                    no = no.ToString(),
                                    emp_id = t.emp_id,
                                    emp_name = t.emp_name,
                                    emp_unit = t.emp_org,
                                    country = t.country,
                                    province = t.province,
                                    business_date = t.bus_date,
                                    traveler_date = t.travel_date,
                                    total_expenses = total.ToString(),

                                    //DevFix 20211116 0000 เพิ่ม approver id ใช้ในการตรวจสอบ
                                    appr_id = t.appr_emp_id,
                                    appr_name = t.appr_emp_name,

                                    take_action = t.take_action,
                                    ref_id = t.ref_id,
                                    appr_remark = t.appr_remark,
                                    appr_status = t.appr_status,

                                    //DevFix 20210714 0000 เพิ่มสถานะที่ Line/CAP --> 1:Draft , 2:Pendding , 3:Approve , 4:Revise , 5:Reject
                                    approve_status = t.approve_status,
                                    approve_remark = t.approve_remark,
                                    //DevFix 20210719 0000 เพิ่ม field OPT
                                    approve_opt = t.approve_opt,
                                    remark_opt = t.remark_opt,
                                    remark_cap = t.remark_cap,


                                    //DevFix 20211013 0000 เพิ่ม key เพื่อใช้ในการแยกข้อมูลออกแต่ละรายการ เนื่องจากเงื่อนไขเดิมข้อมูลซ้ำ --> เก็บค่าเป็น token id
                                    traveler_ref_id = t.traveler_ref_id,
                                });

                                if (t.take_action == "true")
                                {
                                    have_action = true;

                                    //DevFix 20210721 0000 กรณีที่เป็น CAP ของ Line นั้นๆ ให้แสดงจำนวน Traveler และ Total
                                    bCheckApproverLineInDoc = true;
                                }
                            }
                            data.grand_total = grand_total.ToString("#,##0.#0") + " THB";
                            data.total_travel = total_travel.ToString() + " Person(s)";
                        }

                        if (user_role != "1" && login_emp_requester_view == false) // ถ้าไม่ใช่ admin
                        {
                            //DevFix 20210723 0000 กรณีที่เป็น Line เข้ามาดู tab 4 
                            // - ต้องหาว่า CAP ของ Line ที่เข้ามาดู
                            var emp_id_under_cap = "";
                            sql = "select to_char(dta_type) as approve_role_type, dta_travel_empid as emp_id from bz_doc_traveler_approver c where  c.dta_type = 1 and c.dta_appr_empid = '" + user_id + "'  and c.dh_code = '" + value.id_doc + "' ";
                            var approver_role = context.Database.SqlQuery<DocDetail3HeadModel>(sql).ToList();
                            if (approver_role != null && approver_role.Count > 0)
                            {
                                foreach (var t in approver_role)
                                {
                                    if (emp_id_under_cap != "") { emp_id_under_cap += ","; }
                                    emp_id_under_cap += "'" + approver_role[0].emp_id + "'";
                                }
                            }

                            sql = " SELECT ct.ctn_name continent, cr.ct_name country, pv.pv_name province, ex.city_text, to_char(c.DTA_DOC_STATUS) action_status ";
                            sql += "  , case when ex.DTE_BUS_FROMDATE is null then '' else to_char(ex.DTE_BUS_FROMDATE, 'dd Mon rrrr') || ' - ' || to_char(ex.DTE_BUS_TODATE, 'dd Mon rrrr') end as bus_date ";
                            sql += "  , case when ex.DTE_TRAVEL_FROMDATE is null then '' else to_char(ex.DTE_TRAVEL_FROMDATE, 'dd Mon rrrr') || ' - ' || to_char(ex.DTE_TRAVEL_TODATE, 'dd Mon rrrr') end as travel_date ";
                            sql += "  , u.employeeid emp_id, nvl(u.ENTITLE, '') || ' ' || u.ENFIRSTNAME || ' ' || u.ENLASTNAME as emp_name, u.ORGNAME emp_org ";
                            sql += "    , u2.employeeid appr_emp_id, nvl(u2.ENTITLE, '') || ' ' || u2.ENFIRSTNAME || ' ' || u2.ENLASTNAME as appr_emp_name, u2.ORGNAME appr_emp_org, to_char(c.dta_id)ref_id ";
                            sql += " , to_char(ex.dte_total_expense) total ";

                            sql += " , case when c.DTA_APPR_STATUS is null then nvl(ex.dte_cap_appr_opt,'true') else nvl(c.DTA_APPR_STATUS, 'true') end appr_status ";

                            ////DevFix 20210714 0000 เพิ่มสถานะที่ Line/CAP --> 1:Draft , 2:Pendding , 3:Approve , 4:Revise , 5:Reject 
                            //sql += " , c.dta_action_status as approve_status";
                            //sql += " , c.dta_appr_remark as approve_remark";
                            sql += " , case when ex.dte_appr_opt = 'true' then c.dta_action_status else '5' end approve_status";
                            sql += " , case when ex.dte_appr_opt = 'true' then c.dta_appr_remark else ex.dte_cap_appr_remark end approve_remark";

                            ////DevFix 20210719 0000 เพิ่ม field OPT
                            //sql += " , to_char(nvl(c.dta_appr_status,'true'))  as approve_opt";
                            sql += " , case when ex.dte_appr_opt = 'true' then to_char(nvl(c.dta_appr_status,'true')) else ex.dte_appr_opt end approve_opt";

                            //sql += " , ex.dte_appr_remark as remark_opt";
                            sql += " , case when ex.dte_appr_opt = 'true' then ex.dte_cap_appr_remark else ex.dte_appr_remark end remark_opt";
                            sql += " , ex.dte_cap_appr_remark as remark_cap";

                            //DevFix 20210817 เพิ่ม key traveler_ref_id เพื่อใช้ในการแยกข้อมูลออกแต่ละรายการ เนื่องจากเงื่อนไขเดิมข้อมูลซ้ำ --> เก็บค่าเป็น token id
                            //เนื่องจากไม่สามารถ up dataset model ได้ให้ใช้ DTE_TOKEN แทน
                            sql += " , to_char(DTE_TOKEN) as traveler_ref_id";

                            sql += "  FROM bz_doc_traveler_approver c ";
                            sql += "  inner join BZ_DOC_TRAVELER_EXPENSE ex on c.dh_code = ex.dh_code and c.dta_travel_empid = ex.dte_emp_id ";
                            sql += "  left join bz_users u on u.employeeid = ex.dte_emp_id ";
                            sql += "  left join bz_users u2 on c.dta_appr_empid = u2.employeeid ";
                            sql += "  left join bz_master_continent ct on ex.ctn_id = ct.ctn_id ";
                            sql += "  left join bz_master_country cr on ex.ct_id = cr.ct_id ";
                            sql += "  left join bz_master_province pv on ex.pv_id = pv.pv_id ";
                            sql += "  WHERE 1=1";

                            sql += "  and c.DTA_APPR_EMPID != '" + user_id + "' ";

                            sql += "  and c.dh_code = '" + value.id_doc + "' ";
                            sql += "  and c.dta_type = 2 ";

                            //DevFix 20210721 0000 ด้านบนเห็นคนเดียวพร้อมเงินเฉพาะคนที่ถูก approve ถูกแล้ว แต่ด้านล่างควรเห็น ทั้งหมดที่อยู่ภายใต้ CAP 
                            //sql += "  and ex.dte_appr_status in (32,41) and ex.dte_appr_opt = 'true' ";
                            //sql += "  and ex.dte_appr_status in (32,31,30,41) ";
                            sql += "  and ex.dh_code in (select dh_code from bz_doc_head where dh_doc_status in ('41','40','50')) ";

                            sql += "  order by ex.dte_id ";

                            var docDetail3Head_2 = context.Database.SqlQuery<DocDetail3HeadModel>(sql).ToList();
                            if (docDetail3Head_2 != null)
                            {
                                if ((data.travel_date.Replace("-", "")).Trim() == "")
                                {
                                    data.travel_date = docDetail3Head_2[0].travel_date ?? "";
                                    data.business_date = docDetail3Head_2[0].bus_date ?? "";

                                    continent = "";
                                    country = "";
                                    foreach (var h in docDetail3Head_2)
                                    {
                                        if (continent != h.continent)
                                        {
                                            if (!string.IsNullOrEmpty(continent)) continent += ", ";
                                            continent += h.continent;
                                        }

                                        if (country.IndexOf(h.country) > -1) { }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(country)) country += ", ";
                                            country += h.country;
                                        }
                                    }
                                    data.continent = continent;
                                }

                                if (data.province == null)
                                {
                                    data.province = docDetail3Head_2[0].province ?? "";
                                }

                                //DevFix 20210721 0000 ด้านบนเห็นคนเดียวพร้อมเงินเฉพาะคนที่ถูก approve ถูกแล้ว แต่ด้านล่างควรเห็น ทั้งหมดที่อยู่ภายใต้ CAP  
                                decimal grand_total_user_other = 0;
                                var person_user_other = 0;

                                sql = @" select a.dta_travel_empid as emp_id
                                                from bz_doc_traveler_approver  a 
                                                where  a.dh_code  = '" + value.id_doc + "' and a.dta_appr_empid = '" + user_id + "'";
                                var apprlist = context.Database.SqlQuery<DocDetail3HeadModel>(sql).ToList();


                                //DevFix 20210809 0000 ตรวจสอบกรณีที่ Trvaverler 1 คน มีมากกว่า 1 Line/CAP
                                List<travelerList> traveler_list_check = new List<travelerList>();

                                foreach (var t in docDetail3Head_2)
                                {
                                    decimal total = toDecimal(t.total);

                                    //DevFix 20210324 1320 กรณีที่ไม่ใช่ admin ที่ดึงข้อมูล user อื่นมาไม่ต้องรวม Grand Total
                                    //grand_total += total;

                                    //DevFix 20210818 0000 แก้ไขตาม Front End
                                    //ถ้า oversea / overseatraining พี่ส่ง country / province มาใน field country เลย เพราะมันแค่เอามาแสดง 
                                    //แต่ถ้าเป็น local / localtraining ให้ส่ง province / city มาใน field province 
                                    if (doc_type.ToLower().IndexOf("local") > -1)
                                    {
                                        t.province += "/" + t.city_text;
                                    }
                                    else
                                    {
                                        t.country += "/" + t.city_text;
                                    }

                                    no++;
                                    data.traveler_summary.Add(new travelerSummaryList
                                    {
                                        no = no.ToString(),
                                        emp_id = t.emp_id,
                                        emp_name = t.emp_name,
                                        emp_unit = t.emp_org,
                                        country = t.country,
                                        province = t.province,
                                        business_date = t.bus_date,
                                        traveler_date = t.travel_date,
                                        total_expenses = total.ToString(),

                                        //DevFix 20211116 0000 เพิ่ม approver id ใช้ในการตรวจสอบ
                                        appr_id = t.appr_emp_id,
                                        appr_name = t.appr_emp_name,
                                        take_action = "false",
                                        ref_id = t.ref_id,
                                        appr_remark = "",
                                        appr_status = "",

                                        //DevFix 20210714 0000 เพิ่มสถานะที่ Line/CAP --> 1:Draft , 2:Pendding , 3:Approve , 4:Revise , 5:Reject
                                        approve_status = t.approve_status,
                                        approve_remark = t.approve_remark,
                                        //DevFix 20210719 0000 เพิ่ม field OPT
                                        approve_opt = t.approve_opt,
                                        remark_opt = t.remark_opt,
                                        remark_cap = t.remark_cap,

                                        //DevFix 20211013 0000 เพิ่ม key เพื่อใช้ในการแยกข้อมูลออกแต่ละรายการ เนื่องจากเงื่อนไขเดิมข้อมูลซ้ำ --> เก็บค่าเป็น token id
                                        traveler_ref_id = t.traveler_ref_id,
                                    });

                                    //DevFix 20210721 0000 กรณีที่เป็น CAP ของ Line นั้นๆ ให้แสดงจำนวน Traveler และ Total
                                    if (apprlist != null)
                                    {
                                        if (bCheckApproverLineInDoc == false)
                                        {
                                            if (login_emp_traveler_view == true)
                                            {
                                                if (t.emp_id.ToString() == user_id.ToString())
                                                {
                                                    if (t.approve_status != "5")
                                                    {

                                                        //DevFix 20210817 0000 กรณีที่ traverler 1 มี 2 รายการ ให้แสดงรายการเดียวพอ 
                                                        var add_traveler = true;
                                                        var row_check = data.traveler_list.Where(p => p.emp_id.Equals(t.emp_id)).FirstOrDefault();
                                                        if (row_check != null)
                                                        {
                                                            if (row_check.emp_id != "")
                                                            {
                                                                //กรณีที่มีข้อมูลไม่ต้อง add เพิ่ม
                                                                add_traveler = false;
                                                            }
                                                        }
                                                        var bcehck_add_traveler_list = true;
                                                        if (role_in_doc == "1")
                                                        {
                                                            //DevFix 20210809 0000 ตรวจสอบกรณีที่ Trvaverler 1 คน มีมากกว่า 1 Line/CAP
                                                            var check_data = traveler_list_check.SingleOrDefault(a => a.emp_id == t.emp_id);
                                                            if (check_data == null)
                                                            {
                                                                traveler_list_check.Add(new travelerList
                                                                {
                                                                    emp_id = t.emp_id
                                                                });
                                                            }
                                                            else { bcehck_add_traveler_list = false; }
                                                        }

                                                        if (bcehck_add_traveler_list == true)
                                                        {
                                                            if (add_traveler == true)
                                                            {
                                                                //DevFix 20210721 0000 ด้านบนเห็นคนเดียวพร้อมเงินเฉพาะคนที่ถูก approve ถูกแล้ว แต่ด้านล่างควรเห็น ทั้งหมดที่อยู่ภายใต้ CAP  
                                                                no2++;
                                                                data.traveler_list.Add(new travelerList
                                                                {
                                                                    //DevFix 20200910 1658 เอาวงเล็บออก เหลือจุด . พอ 
                                                                    text = no2.ToString() + ". " + t.emp_id + "  " + t.emp_name + " : " + t.emp_org,
                                                                    //DevFix 20210817 0000 กรณีที่ traverler 1 มี 2 รายการ ให้แสดงรายการเดียวพอ --> เพิ่ม emp_id เพื่อเป็นเงื่อนไข
                                                                    emp_id = t.emp_id,
                                                                });
                                                                person_user_other += 1;
                                                            }
                                                            //กรณีที่เป็น step line
                                                            grand_total_user_other += total;
                                                        }

                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var checkapprlist = apprlist.Where(p => p.emp_id.Equals(t.emp_id)).FirstOrDefault();
                                                if (checkapprlist != null && checkapprlist.emp_id != null && checkapprlist.emp_id.ToString() != "")
                                                {
                                                    var bDataShows = false;
                                                    if (t.approve_opt == "true" && t.action_status != "40")
                                                    {
                                                        bDataShows = true;
                                                    }
                                                    if (t.approve_status != "5")
                                                    {
                                                        bDataShows = true;
                                                    }
                                                    if (bCheckTraveler_list == true) { bDataShows = false; }
                                                    if (bDataShows == true)
                                                    {
                                                        //DevFix 20210817 0000 กรณีที่ traverler 1 มี 2 รายการ ให้แสดงรายการเดียวพอ 
                                                        var add_traveler = true;
                                                        var row_check = data.traveler_list.Where(p => p.emp_id.Equals(t.emp_id)).FirstOrDefault();
                                                        if (row_check != null)
                                                        {
                                                            if (row_check.emp_id != "")
                                                            {
                                                                //กรณีที่มีข้อมูลไม่ต้อง add เพิ่ม
                                                                add_traveler = false;
                                                            }
                                                        }
                                                        if (add_traveler == true)
                                                        {
                                                            //DevFix 20210721 0000 ด้านบนเห็นคนเดียวพร้อมเงินเฉพาะคนที่ถูก approve ถูกแล้ว แต่ด้านล่างควรเห็น ทั้งหมดที่อยู่ภายใต้ CAP  
                                                            no2++;
                                                            data.traveler_list.Add(new travelerList
                                                            {
                                                                //DevFix 20200910 1658 เอาวงเล็บออก เหลือจุด . พอ 
                                                                text = no2.ToString() + ". " + t.emp_id + "  " + t.emp_name + " : " + t.emp_org,
                                                                //DevFix 20210817 0000 กรณีที่ traverler 1 มี 2 รายการ ให้แสดงรายการเดียวพอ --> เพิ่ม emp_id เพื่อเป็นเงื่อนไข
                                                                emp_id = t.emp_id,
                                                            });

                                                            person_user_other += 1;
                                                        }
                                                        //กรณีที่เป็น step line
                                                        grand_total_user_other += total;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                                //DevFix 20210721 0000 ด้านบนเห็นคนเดียวพร้อมเงินเฉพาะคนที่ถูก approve ถูกแล้ว แต่ด้านล่างควรเห็น ทั้งหมดที่อยู่ภายใต้ CAP 
                                if (bCheckApproverLineInDoc == false && bCheckApproverCAPInDoc == false)
                                {
                                    if (bCheckTraveler_list == false)
                                    {
                                        data.total_travel = person_user_other + " Person(s)";
                                        data.grand_total = grand_total_user_other.ToString("#,##0.#0") + " THB";
                                    }
                                }

                            }

                        }
                        //DevFix 20211013 0000 กรณีที่ Line submit to CAP แต่ CAP ยังไม่ได้ active --> ยังไม่ได้ใช้งานนะ เขียนไว้ก่อน ???
                        if (pf_doc_id == "4" || pf_doc_id == "5")
                        {
                            var bCheckPF_CAP = true;
                            sql = @" select to_char(count(1)) as approve_status
                                from BZ_DOC_TRAVELER_APPROVER a
                                where dta_action_status >  2 and a.dta_type = 2 and dh_code =  '" + value.id_doc + "'  ";
                            var dataCheck_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                            if (dataCheck_Def != null)
                            {
                                if (dataCheck_Def.Count > 0) { if (dataCheck_Def[0].approve_status.ToString() == "0") { bCheckPF_CAP = false; } }
                            }
                            sql = @" select dta_appr_level,dta_travel_empid as emp_id, (a.dta_action_status) as approve_status,  a.dta_appr_remark as approve_remark
                                ,to_char(nvl(a.dta_appr_status,'true')) as  approve_opt
                                from BZ_DOC_TRAVELER_APPROVER a 
                                where  dh_code =  '" + value.id_doc + "'  ";
                            if (pf_doc_id == "4")
                            {
                                if (bCheckPF_CAP == true) { sql += @" and a.dta_type = 2 and dta_action_status not in ('6') "; } else { sql += @" and a.dta_type = 1"; }
                            }
                            sql += @" order by dta_appr_level ";
                            var dataApporver_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                            #region DevFix 20211013 0000 กรณีที่เป็น step cap ให้ตรวจสอบ line ว่ามีการ reject หรือไม่
                            var dataApporverLine_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                            var dataApporverLine2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                            var dataApporverLine3_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                            var dataApporverCAP_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                            var dataApporverCAP2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                            var dataApporverCAP3_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                            var dataApporverCAP3Level_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                            if (pf_doc_id == "4" || pf_doc_id == "5")
                            {
                                //line approve
                                sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '3' as approve_status, dte_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where dte_appr_opt = 'true' and dte_status = 1 and dte_appr_status <> 23
                                         and dh_code = '" + value.id_doc + "' ";
                                dataApporverLine_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                                //line reject
                                sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '5' as approve_status, dte_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where ((dte_appr_opt = 'false' and dte_status = 1) or dte_appr_status = 30 )
                                         and dh_code = '" + value.id_doc + "' ";
                                dataApporverLine2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                                //line pendding 
                                sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '2' as approve_status, dte_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where dte_status = 1 and dte_appr_status = 31 and dh_code = '" + value.id_doc + "' ";
                                dataApporverLine3_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();


                                //cap approve
                                sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '3' as approve_status, dte_cap_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where nvl(dte_cap_appr_status,41) = '42' and (dte_cap_appr_opt = 'true' and dte_appr_opt = 'true') and dte_status = 1
                                         and dh_code = '" + value.id_doc + "' ";
                                dataApporverCAP_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                                //cap reject
                                sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '5' as approve_status, dte_cap_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where nvl(dte_cap_appr_status,41) = '42' and ( (dte_cap_appr_opt = 'false' and dte_status = 1) or (dte_appr_opt = 'false' and dte_appr_status = 32) or dte_cap_appr_status = 40 )
                                         and dh_code = '" + value.id_doc + "' ";
                                dataApporverCAP2_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                                //cap pendding 
                                sql = @" select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                         , '2' as approve_status, dte_cap_appr_remark as approve_remark
                                         from BZ_DOC_TRAVELER_EXPENSE a 
                                         where dte_status = 1 and (dte_cap_appr_status = 41 or (dte_cap_appr_status is null and  dte_appr_status = 32 and dte_appr_opt = 'true' ) )
                                         and dh_code = '" + value.id_doc + "' ";

                                dataApporverCAP3_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                            }
                            #endregion DevFix 20211013 0000 กรณีที่เป็น step cap ให้ตรวจสอบ line ว่ามีการ reject หรือไม่
                            #region DevFix 20211013 0000 update status  
                            foreach (var item in data.traveler_summary)
                            {
                                if (item.emp_id == "00001393" && item.country == "Denmark")
                                {
                                    var xdebug = "";
                                }
                                var approve_status = "1";
                                var approve_remark = "";
                                var approve_opt = "";
                                var appr_remark = "";
                                var check_data = dataApporver_Def.Where(t => t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id);

                                #region DevFix 20211013 0000 กรณีที่เป็น step cap ให้ตรวจสอบ line ว่ามีการ reject หรือไม่
                                if (pf_doc_id == "4" || pf_doc_id == "5")
                                {
                                    if (bCheckPF_CAP == true)
                                    {

                                        check_data = dataApporverCAP_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                        if (check_data.Count() > 0)
                                        {
                                            sql = @" select distinct to_char(dta_appr_level) as approve_level from BZ_DOC_TRAVELER_APPROVER 
                                                 where dta_type = 2 and dta_appr_level > 1 and dh_code = '" + value.id_doc + "' and dta_travel_empid = '" + item.emp_id + "'  ";
                                            sql += @" and dta_appr_empid = '" + item.appr_id + "' ";

                                            var dtListApprLevel = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();
                                            if (dtListApprLevel.Count > 0)
                                            {
                                                if (dtListApprLevel[0].approve_level.ToString() != "1")
                                                {
                                                    sql = "";
                                                    foreach (var apprlist in dtListApprLevel)
                                                    {
                                                        if (sql != "") { sql += " union "; }
                                                        sql += @"
                                                     select to_char(dte_token) as traveler_ref_id, dte_emp_id as emp_id
                                                     , '2' as approve_status, dte_cap_appr_remark as approve_remark
                                                     from BZ_DOC_TRAVELER_EXPENSE a  
                                                     where dte_status = 1 
                                                     and dte_cap_appr_status = 42
                                                     and (dte_emp_id, dh_code) in (
                                                     select dta_travel_empid, dh_code from BZ_DOC_TRAVELER_APPROVER
                                                     where dta_type = 2 and dta_doc_status = 41 and dta_appr_level = " + apprlist.approve_level + " ) and dh_code = '" + value.id_doc + "' and dte_emp_id = '" + item.emp_id + "' ";
                                                    }
                                                    dataApporverCAP3Level_Def = context.Database.SqlQuery<TravelerApproverConditionModel>(sql).ToList();

                                                    check_data = dataApporverCAP3Level_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                                }
                                            }
                                        }
                                        if (check_data.Count() == 0)
                                        {
                                            check_data = dataApporverCAP2_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                            if (check_data.Count() == 0)
                                            {
                                                check_data = dataApporverCAP3_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                                if (check_data.Count() == 0)
                                                {
                                                    //กรณีที่มีข้อมูล line ให้เอา remark line มาแสดง
                                                    check_data = dataApporverLine2_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                                }
                                            }
                                        }
                                        //กรณีที่มีข้อมูล cap ให้เอา remark cap มาแสดง
                                        appr_remark = item.remark_cap;
                                    }
                                    else
                                    {
                                        check_data = dataApporverLine2_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                        if (check_data.Count() == 0)
                                        {
                                            check_data = dataApporverLine3_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                        }
                                    }
                                }
                                else if (pf_doc_id == "3")
                                {
                                    //กรณีที่มีข้อมูล line ให้เอา remark line มาแสดง
                                    appr_remark = item.remark_opt;

                                    check_data = dataApporverLine_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                    if (check_data.Count() == 0)
                                    {
                                        check_data = dataApporverLine2_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                        if (check_data.Count() == 0)
                                        {
                                            check_data = dataApporverLine3_Def.Where(t => (t.emp_id == item.emp_id && t.traveler_ref_id == item.traveler_ref_id));
                                        }
                                    }
                                }
                                #endregion DevFix 20211013 0000 กรณีที่เป็น step cap ให้ตรวจสอบ line ว่ามีการ reject หรือไม่

                                var bcheck_change_status = false;
                                foreach (var item2 in check_data)
                                {
                                    approve_status = item2.approve_status;
                                    approve_opt = item2.approve_opt;

                                    if (approve_remark != "") { approve_remark += ","; }
                                    if (item2.approve_remark != "") { approve_remark += item2.approve_remark; }

                                    bcheck_change_status = true;

                                }
                                if (approve_status == "" || bcheck_change_status == false) { approve_status = item.approve_status; }
                                if (approve_remark == "") { approve_remark = item.approve_remark; }

                                item.approve_status = approve_status;
                                item.approve_remark = approve_remark; // remark btn action
                                item.approve_opt = approve_opt;

                                item.appr_remark = appr_remark;
                            }
                            #endregion DevFix 20211013 0000 update status  
                        }


                        data.after_trip.opt1 = (docHead[0].DH_AFTER_TRIP_OPT1 ?? "") == "Y" ? "true" : "false";
                        data.after_trip.opt2.status = (docHead[0].DH_AFTER_TRIP_OPT2 ?? "") == "Y" ? "true" : "false";
                        data.after_trip.opt2.remark = docHead[0].DH_AFTER_TRIP_OPT2_REMARK ?? "";
                        data.after_trip.opt3.status = (docHead[0].DH_AFTER_TRIP_OPT3 ?? "") == "Y" ? "true" : "false";
                        data.after_trip.opt3.remark = docHead[0].DH_AFTER_TRIP_OPT3_REMARK ?? "";


                        string pf_doc_status = docHead[0].doc_status.ToString().Substring(0, 1);
                        if (pf_doc_status == "1")
                        {
                            data.button.part_i = "true";
                        }
                        else if (pf_doc_status == "2")
                        {
                            data.button.part_i = "true";
                            data.button.part_ii = "true";
                        }
                        else if (pf_doc_status == "3")
                        {
                            data.button.part_i = "true";
                            data.button.part_ii = "true";
                            data.button.part_iii = "true";
                        }
                        else if (pf_doc_status == "4")
                        {
                            data.button.part_i = "true";
                            data.button.part_ii = "true";
                            data.button.part_iii = "true";
                            data.button.part_iiii = "true";
                            data.button.part_cap = "true";
                        }
                        else if (pf_doc_status == "5")
                        {
                            data.button.part_i = "true";
                            data.button.part_ii = "true";
                            data.button.part_iii = "true";
                            data.button.part_iiii = "true";
                            data.button.part_cap = "true";
                        }

                        if ((action != null && action.Count() > 0) && (have_action == true))
                        {
                            data.button.save = "true";
                            data.button.cancel = "true";
                            data.button.reject = "true";
                            data.button.revise = "true";
                            data.button.approve = "true";
                        }

                        #region DevFix 202009141200 ตรวจสอบว่า CAP Approver Level ก่อนนี้ approve ครบทุกคนหรือไม่
                        if (pf_doc_status == "4" && user_role != "1")
                        {
                            //DevFix 20210811 0000 เงื่อนไขเดิมใช้ไม่ได้ เนื่องจากมีกรณีที่ CAP มีมากกว่า 1 Traverler 
                            sql = @"  SELECT DISTINCT DTA_TRAVEL_EMPID 
                                     FROM BZ_DOC_TRAVELER_APPROVER A
                                     WHERE A.DTA_TYPE = 2 AND A.DH_CODE = '" + value.id_doc + "' AND DTA_APPR_EMPID = '" + user_id + "' ";
                            sql += " ORDER BY DTA_TRAVEL_EMPID ";
                            var traverler_in_cap = context.Database.SqlQuery<SearchCAPModel>(sql).ToList();
                            if (traverler_in_cap != null && traverler_in_cap.Count > 0)
                            {
                                var baction_cap = false;
                                var baction_active = true;
                                foreach (var t in traverler_in_cap)
                                {
                                    baction_cap = false;
                                    var traverler_id_def = t.DTA_TRAVEL_EMPID.ToString();
                                    sql = @"  SELECT DTA_TRAVEL_EMPID, to_char(DTA_APPR_LEVEL) as DTA_APPR_LEVEL, DTA_APPR_EMPID, to_char(DTA_ACTION_STATUS) as DTA_ACTION_STATUS
                                     FROM BZ_DOC_TRAVELER_APPROVER A
                                     WHERE A.DTA_TYPE = 2 AND A.DH_CODE = '" + value.id_doc + "' AND A.DTA_TRAVEL_EMPID = '" + traverler_id_def + "' ";
                                    sql += " ORDER BY DTA_TRAVEL_EMPID, DTA_APPR_LEVEL DESC";
                                    var action_cap = context.Database.SqlQuery<SearchCAPModel>(sql).ToList();
                                    if (action_cap != null)
                                    {
                                        if (action_cap.Count > 0)
                                        {
                                            foreach (var c in action_cap)
                                            {
                                                if (c.DTA_APPR_EMPID.ToString() == user_id.ToString())
                                                {
                                                    baction_cap = true;
                                                }
                                                else
                                                {
                                                    if (baction_cap == true)
                                                    {
                                                        //ลำดับถัดจาก CAP ที่เข้ามาดู
                                                        if (c.DTA_ACTION_STATUS.ToString() == "2")
                                                        {
                                                            baction_active = false;
                                                            break;
                                                        }
                                                    }
                                                }

                                            }

                                        }
                                    }

                                    if (baction_active == false) { break; }
                                }

                                if (baction_active == false)
                                {
                                    data.button.save = "false";
                                    data.button.cancel = "false";
                                    data.button.reject = "false";
                                    data.button.revise = "false";
                                    data.button.approve = "false";
                                }
                            }

                        }
                        #endregion DevFix 202009141200 ตรวจสอบว่า CAP Approver Level ก่อนนี้ approve ครบทุกคนหรือไม่  

                        #region DevFix 20211012 0000 กรณีที่เป็น pmdv admin ทำ training ให้ button.save กับ button.approve = true
                        string doc_status_tab = docHead[0].doc_status.ToString().Substring(0, 1);
                        string doc_status_chk = docHead[0].doc_status.ToString();
                        if (doc_status_tab == "5" || doc_status_chk == "10" || doc_status_chk == "20" || doc_status_chk == "30" || doc_status_chk == "40" || doc_status_chk == "50") { }
                        else
                        {
                            if (value.id_doc.IndexOf("T") > -1 && doc_status_tab == "4")
                            {
                                data.button.approve = "false";
                                data.button.cancel = "false";
                                data.button.reject = "false";
                                data.button.revise = "false";
                                data.button.save = "false";
                                data.button.submit = "false";

                                sql = @"select distinct to_char(pmdv_admin) as type 
                                    from bz_data_manage where pmdv_admin = 'true' and emp_id = '" + user_id + "' ";
                                var pmdv_admin_list = context.Database.SqlQuery<approverModel>(sql).ToList();
                                if (pmdv_admin_list != null)
                                {
                                    if (pmdv_admin_list.Count > 0)
                                    {
                                        if (pmdv_admin_list[0].type.ToString() == "true")
                                        {
                                            data.button.approve = "true";
                                            data.button.cancel = "true";
                                            data.button.reject = "true";
                                            data.button.revise = "true";
                                            data.button.save = "true";
                                            data.button.submit = "true";

                                        }
                                    }
                                }
                            }
                        }
                        #endregion DevFix 20211012 0000 กรณีที่เป็น pmdv admin ทำ training ให้ button.save กับ button.approve = true 

                    }


                }
            }
            catch (Exception ex)
            {
                data.msg_remark = ex.Message.ToString() + " sql :" + sql;
                //throw;
            }
            return data;
        }

        private DateTime? chkDate(string value)
        {
            DateTime? date = null;
            try
            {
                if (value == null)
                    return date;

                if (value.Length < 10)
                    return date;

                date = DateTime.ParseExact(value.Substring(0, 10), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            }
            catch (Exception ex)
            {

            }
            return date;
        }

        private string retCheckValue(string value)
        {
            string ret = "N";
            try
            {
                if (value == "true")
                    ret = "Y";
            }
            catch (Exception ex)
            {

            }
            return ret;
        }

        private decimal? retDecimal(string value)
        {
            decimal? ret = null;
            try
            {
                ret = string.IsNullOrEmpty(value) ? ret : Convert.ToDecimal(value);
            }
            catch (Exception ex)
            {

            }
            return ret;
        }

        private decimal toDecimal(string value)
        {
            decimal ret = 0;
            try
            {
                ret = string.IsNullOrEmpty(value) ? ret : Convert.ToDecimal(value);
            }
            catch (Exception ex)
            {

            }
            return ret;
        }

        //???ต้องย้ายไปที่ batch
        public ExchangeRatesModel ExchangeRates(ref string msg)
        {
            ExchangeRatesModel ex_rate = new ExchangeRatesModel();
            DataTable dt = new DataTable();
            string sqlstr = "";
            try
            {
                //cls_connection_cpai conn = new cls_connection_cpai();
                //conn.OpenConnection();
                //dt = conn.ExecuteAdapter(@" select t_fxb_cur as ex_cur, t_fxb_value1 as ex_value1
                //                            ,to_char(to_date(t_fxb_valdate,'yyyyMMdd') ,'dd Mon rrrr')   as ex_date
                //                            from CMDB.VW_FX_TYPE_M 
                //                            where  t_fxb_cur = 'USD' 
                //                            and t_fxb_valdate in (select  max(t_fxb_valdate) from CMDB.VW_FX_TYPE_M where  t_fxb_cur = 'USD' )").Tables[0];
                //conn.CloseConnection(); 

                //ws_conn.wsConnection conn = new ws_conn.wsConnection();
                //dt = conn.adapter_data(@" select t_fxb_cur as ex_cur, t_fxb_value1 as ex_value1  ,to_char(to_date(t_fxb_valdate,'yyyyMMdd') ,'dd Mon rrrr')   as ex_date  from BZ_DATA_FX_TYPE_M  where  t_fxb_cur = 'USD'   and t_fxb_valdate in (select  max(t_fxb_valdate) from BZ_DATA_FX_TYPE_M where  t_fxb_cur = 'USD' )");

                sqlstr = @" select t_fxb_cur as ex_cur, t_fxb_value1 as ex_value1
                                        ,to_char(to_date(t_fxb_valdate,'yyyyMMdd') ,'dd Mon rrrr')   as ex_date
                                        from BZ_DATA_FX_TYPE_M
                                        where  t_fxb_cur = 'USD' 
                                        and t_fxb_valdate in (select  max(t_fxb_valdate) from BZ_DATA_FX_TYPE_M where  t_fxb_cur = 'USD' )";

                cls_connection_ebiz conn = new cls_connection_ebiz();
                conn.OpenConnection();
                dt = conn.ExecuteAdapter(sqlstr).Tables[0];
                conn.CloseConnection();

                if (dt.Rows.Count > 0)
                {
                    ex_rate = new ExchangeRatesModel();
                    ex_rate.ex_value1 = dt.Rows[0]["ex_value1"].ToString();
                    ex_rate.ex_cur = dt.Rows[0]["ex_cur"].ToString();
                    ex_rate.ex_date = dt.Rows[0]["ex_date"].ToString();
                }
                msg = "";
            }
            catch (Exception ex) { msg = ex.Message.ToString() + " sql:" + sqlstr; }

            return ex_rate;
        }


    }
}