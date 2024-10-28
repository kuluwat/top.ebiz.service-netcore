﻿using System;
using System.Collections.Generic;
using System.Linq; 
using System.Data;
using top.ebiz.service.Models.Traveler_Profile;


namespace top.ebiz.service.Service.Traveler_Profile 
{
    public class EBizSAPService
    {
        string ret;
        string sqlstr;
        string sqlstr_all;
        SetDocService sw;
        cls_connection_ebiz conn;
        DataTable dt;

        public TravelExpenseOutModel SendTravelExpenseToSAP(TravelExpenseOutModel value)
        {
            var msg = "";
            var page_name = "travelexpense";
            var imglist = new List<ImgList>();
            var token_login = value.token_login;
            var doc_id = value.doc_id;

            var data = new TravelExpenseOutModel();
            data = value;
            data.token_login = token_login;
            data.doc_id = doc_id;
            data.id = "1";
            data.user_admin = true;
             
            //ทดสอบ update status = Send to SAP ทั้งหมดใน list ที่ส่งไป SAP ก่อน 
            for (int i = 0; i < data.travelexpense_detail.Count; i++)
            {
                ret = "false";
                conn = new cls_connection_ebiz();
                string emp_user_active = "";
                string id = data.travelexpense_detail[i].id;
                string emp_id = data.travelexpense_detail[i].emp_id;
                string status_sap = "";

                List<EmpListOutModel> dremplist = data.emp_list.Where(a => ((a.emp_id == emp_id) && (a.send_to_sap == "true"))).ToList(); 
                if (dremplist.Count > 0) { status_sap = "6"; } else { continue; }
                if (data.travelexpense_detail[i].status_active == "true") { status_sap = "6"; } else { continue; }

                data.travelexpense_detail[i].status = status_sap;
                 
                sqlstr = @" update BZ_DOC_TRAVELEXPENSE_DETAIL set";

                sqlstr += @" STATUS = " + conn.ChkSqlStr(status_sap, 4000);

                sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                sqlstr += @" ,UPDATE_DATE = sysdate";
                sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                sqlstr += @" where ";
                sqlstr += @" ID = " + conn.ChkSqlStr(id, 300);
                sqlstr += @" and DOC_ID = " + conn.ChkSqlStr(doc_id, 300);
                sqlstr += @" and EMP_ID = " + conn.ChkSqlStr(emp_id, 300);

                ret = SetDocService.conn_ExecuteNonQuery(sqlstr, false);
                sqlstr_all += sqlstr + "||";

                if (ret.ToLower() != "true") { goto Next_line_1; }
            }
        Next_line_1:;

            if (ret.ToLower() == "true")
            {
                ret = SetDocService.conn_ExecuteNonQuery(sqlstr_all, true);
            }

            var msg_error = "";
            if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {

                SearchDocService swd = new SearchDocService();
                TravelExpenseModel value_load = new TravelExpenseModel();
                value_load.token_login = data.token_login;
                value_load.doc_id = data.doc_id;
                data = new TravelExpenseOutModel();
                data = swd.SearchTravelExpense(value_load);
            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Send data to SAP succesed." : "Send data to SAP failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;
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

    }
}