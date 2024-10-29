using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Common;
//using Oracle.ManagedDataAccess.Client;
//using System.Data.Entity;

//using System.Data.OracleClient;
//using Newtonsoft.Json;
using System.Text.Json;
using top.ebiz.service.Models.Traveler_Profile;
using top.ebiz.service.Service.Traveler_Profile;
//using System.IO;
using top.ebiz.service.Models.Traveler_Profile;

namespace top.ebiz.service.Service.Traveler_Profile 
{

    public class SetMasterDataService
    {
        SetDocService sw;
        //cls_connection conn;
        string sqlstr = "";
        string sqlstr_all = "";
        string ret = "";
        DataTable dt;
        DataTable dtdata;
        public ReimbursementOutModel SetReimbursementExchageRate(ReimbursementOutModel value)
        {
            //ยังไม่ได้เขียนเพิ่ม ??? ตอนนี้ให้ส่งข้อมูลเข้ามาเพื่อ insert/update อย่างเดียวก่อน

            var doc_type = value.data_type;
            var data = value;
            var data_def = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data  
            sw = new SetDocService();
            int imaxid = sw.GetMaxID("BZ_DATA_FX_TYPE_M");

            conn = new cls_connection();

            if (data.data_type.ToString() != "")
            {
                sqlstr_all = "";
                if (data.m_exchangerate.Count > 0)
                {
                    List<ExchangeRateList> dtlist = data.m_exchangerate;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }

                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_DATA_FX_TYPE_M
                            (ID,T_FXB_CUR,T_FXB_VALUE1,T_FXB_VALDATE,STATUS_ACTIVE,REMARK,DATA_SOURCE_CPAI
                            ,CREATE_BY,CREATE_DATE,TOKEN_UPDAT) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].currency_id, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].exchange_rate, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].date_from, 300);
                            sqlstr += @" ," + conn.ChkSqlStr("1", 300);
                            sqlstr += @" ," + conn.ChkSqlStr("", 300);
                            sqlstr += @" ," + conn.ChkSqlStr("0", 300);
                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            dtlist[i].id = imaxid.ToString();
                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_DATA_FX_TYPE_M set";

                            sqlstr += @" T_FXB_CUR = " + conn.ChkSqlStr(dtlist[i].currency_id, 300);
                            sqlstr += @" ,T_FXB_VALUE1 = " + conn.ChkSqlStr(dtlist[i].exchange_rate, 300);
                            sqlstr += @" ,T_FXB_VALDATE = " + conn.ChkSqlStr(dtlist[i].date_from, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_DATA_FX_TYPE_M ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                        }

                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                    }
                }

            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = SetDocService.conn_ExecuteNonQuery(sqlstr_all, false);
                }
            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() == "")
            {
                msg_error = ret;
            }
            else if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;

                //กรณีที่มี error ให้คืนค่า id ของ exchange rate 
                data = data_def;
            }
            else
            {
                List<ExchangeRateList> dtlist = data.m_exchangerate;
                for (int i = 0; i < dtlist.Count; i++)
                {
                    dtlist[i].action_change = "false";
                    dtlist[i].action_type = "update";
                }
                data.m_exchangerate = dtlist;
            }

           data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
           data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
           data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
           data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt3.status = "Error msg";
           data.after_trip.opt3.remark = msg_error;

            return data;
        }
        public MMaintainDataModel SetAirticketType(MMaintainDataModel value)
        {
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data  
            sw = new SetDocService();
            int imaxid = sw.GetMaxID("BZ_MASTER_ALREADY_BOOKED_TYPE");

            conn = new cls_connection();
            if (data.data_type.ToString() != "")
            {
                sqlstr_all = "";
                if (data.airticket_type.Count > 0)
                {
                    List<MasterNormalModel> dtlist = data.airticket_type;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }


                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_MASTER_ALREADY_BOOKED_TYPE
                                    (ID,NAME,STATUS,SORT_BY,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].name, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_MASTER_ALREADY_BOOKED_TYPE set";

                            sqlstr += @" NAME = " + conn.ChkSqlStr(dtlist[i].name, 300);
                            sqlstr += @" ,STATUS = " + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ,SORT_BY = " + conn.ChkSqlStr(dtlist[i].sort_by, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_MASTER_ALREADY_BOOKED_TYPE ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                        }

                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                    }
                }

            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = SetDocService.conn_ExecuteNonQuery(sqlstr_all, false);
                }
            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() == "")
            {
                msg_error = ret;
            }
            else if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {

                SearchMasterDataService swd = new SearchMasterDataService();
                MMaintainDataModel value_load = new MMaintainDataModel();
                value_load.token_login = data.token_login;
                value_load.page_name = data.page_name;
                value_load.module_name = data.module_name;

                data = new MMaintainDataModel();
                data = swd.SearchAirticketType(value_load);
            }

           data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
           data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
           data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
           data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt3.status = "Error msg";
           data.after_trip.opt3.remark = msg_error;


            return data;
        }
        public MMaintainDataModel SetAlreadyBooked(MMaintainDataModel value)
        {

            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data  
            sw = new SetDocService();
            int imaxid = sw.GetMaxID("BZ_MASTER_ALREADY_BOOKED_TYPE");

            conn = new cls_connection();

            if (data.data_type.ToString() != "")
            {
                sqlstr_all = "";
                if (data.already_booked.Count > 0)
                {
                    List<MasterNormalModel> dtlist = data.already_booked;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }

                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_MASTER_ALREADY_BOOKED_TYPE
                                    (ID,NAME,STATUS,SORT_BY,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].name, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_MASTER_ALREADY_BOOKED_TYPE set";

                            sqlstr += @" NAME = " + conn.ChkSqlStr(dtlist[i].name, 300);
                            sqlstr += @" ,STATUS = " + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ,SORT_BY = " + conn.ChkSqlStr(dtlist[i].sort_by, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_MASTER_ALREADY_BOOKED_TYPE ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                        }

                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                    }
                }

            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = SetDocService.conn_ExecuteNonQuery(sqlstr_all, false);
                }
            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() == "")
            {
                msg_error = ret;
            }
            else if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {

                SearchMasterDataService swd = new SearchMasterDataService();
                MMaintainDataModel value_load = new MMaintainDataModel();
                value_load.token_login = data.token_login;
                value_load.page_name = data.page_name;
                value_load.module_name = data.module_name;

                data = new MMaintainDataModel();
                data = swd.SearchAlreadyBooked(value_load);
            }

           data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
           data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
           data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
           data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt3.status = "Error msg";
           data.after_trip.opt3.remark = msg_error;


            return data;
        }
        public MMaintainDataModel SetListStatus(MMaintainDataModel value)
        {
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data  
            sw = new SetDocService();
            int imaxid = sw.GetMaxID("BZ_MASTER_LIST_STATUS");

            conn = new cls_connection();
            if (data.data_type.ToString() != "")
            {
                sqlstr_all = "";
                if (data.list_status.Count > 0)
                {
                    List<MasterNormalModel> dtlist = data.list_status;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }

                        dtlist[i].page_name = "all";

                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_MASTER_LIST_STATUS
                                    (ID,NAME,STATUS,SORT_BY,PAGE_NAME,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].name, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].page_name, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_MASTER_LIST_STATUS set";

                            sqlstr += @" NAME = " + conn.ChkSqlStr(dtlist[i].name, 300);
                            sqlstr += @" ,STATUS = " + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ,SORT_BY = " + conn.ChkSqlStr(dtlist[i].sort_by, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and PAGE_NAME = " + conn.ChkSqlStr(dtlist[i].page_name, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_MASTER_LIST_STATUS ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and PAGE_NAME = " + conn.ChkSqlStr(dtlist[i].page_name, 300);
                        }

                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                    }
                }

            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = SetDocService.conn_ExecuteNonQuery(sqlstr_all, false);
                }
            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() == "")
            {
                msg_error = ret;
            }
            else if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {
                SearchMasterDataService swd = new SearchMasterDataService();
                MMaintainDataModel value_load = new MMaintainDataModel();
                value_load.token_login = data.token_login;
                value_load.page_name = data.page_name;
                value_load.module_name = data.module_name;
                data = new MMaintainDataModel();
                data = swd.SearchListStatus(value_load);
            }

           data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
           data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
           data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
           data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt3.status = "Error msg";
           data.after_trip.opt3.remark = msg_error;


            return data;
        }
        public MMaintainDataModel SetAllowanceType(MMaintainDataModel value)
        {
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data  
            sw = new SetDocService();
            int imaxid = sw.GetMaxID("BZ_MASTER_ALLOWANCE_TYPE");

            conn = new cls_connection();
            if (data.data_type.ToString() != "")
            {
                sqlstr_all = "";
                if (data.allowance_type.Count > 0)
                {
                    List<MasterNormalModel> dtlist = data.allowance_type;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }


                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_MASTER_ALLOWANCE_TYPE
                                    (ID,NAME,STATUS,SORT_BY,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].name, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_MASTER_ALLOWANCE_TYPE set";

                            sqlstr += @" NAME = " + conn.ChkSqlStr(dtlist[i].name, 300);
                            sqlstr += @" ,STATUS = " + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ,SORT_BY = " + conn.ChkSqlStr(dtlist[i].sort_by, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_MASTER_ALLOWANCE_TYPE ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                        }

                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                    }
                }

            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = SetDocService.conn_ExecuteNonQuery(sqlstr_all, false);
                }
            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() == "")
            {
                msg_error = ret;
            }
            else if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {
                SearchMasterDataService swd = new SearchMasterDataService();
                MMaintainDataModel value_load = new MMaintainDataModel();
                value_load.token_login = data.token_login;
                value_load.page_name = data.page_name;
                value_load.module_name = data.module_name;

                data = new MMaintainDataModel();
                data = swd.SearchAllowanceType(value_load);
            }

           data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
           data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
           data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
           data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt3.status = "Error msg";
           data.after_trip.opt3.remark = msg_error;


            return data;
        }


        public MMaintainDataModel SetFeedbackType(MMaintainDataModel value)
        {
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data  
            sw = new SetDocService();
            int imaxid = sw.GetMaxID("BZ_MASTER_FEEDBACK_TYPE");

            conn = new cls_connection();
            if (data.data_type.ToString() != "")
            {
                sqlstr_all = "";
                if (data.feedback_type.Count > 0)
                {
                    List<MasterNormalModel> dtlist = data.feedback_type;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }


                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_MASTER_FEEDBACK_TYPE
                                    (ID,NAME,STATUS,SORT_BY,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].name, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_MASTER_FEEDBACK_TYPE set";

                            sqlstr += @" NAME = " + conn.ChkSqlStr(dtlist[i].name, 300);
                            sqlstr += @" ,STATUS = " + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ,SORT_BY = " + conn.ChkSqlStr(dtlist[i].sort_by, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_MASTER_FEEDBACK_TYPE ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);

                        }

                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                        if (action_type == "delete" && (dtlist[i].sub_data.ToString() == "true"))
                        {
                            //delete BZ_MASTER_FEEDBACK_LIST  
                            sqlstr = @" delete from BZ_MASTER_FEEDBACK_LIST ";
                            sqlstr += @" where ";
                            sqlstr += @" FEEDBACK_TYPE_ID = " + conn.ChkSqlStr(dtlist[i].id, 300);


                            ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                            sqlstr_all += sqlstr + "||";

                            if (ret.ToLower() != "true") { goto Next_line_1; }
                        }

                    }
                }

            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = SetDocService.conn_ExecuteNonQuery(sqlstr_all, false);
                }
            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() == "")
            {
                msg_error = ret;
            }
            else if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {

                SearchMasterDataService swd = new SearchMasterDataService();
                MMaintainDataModel value_load = new MMaintainDataModel();
                value_load.token_login = data.token_login;
                value_load.page_name = data.page_name;
                value_load.module_name = data.module_name;

                data = new MMaintainDataModel();
                data = swd.SearchFeedbackType(value_load);
            }

           data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
           data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
           data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
           data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt3.status = "Error msg";
           data.after_trip.opt3.remark = msg_error;


            return data;
        }

        public MMaintainDataModel SetFeedbackList(MMaintainDataModel value)
        {
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data  
            sw = new SetDocService();
            int imaxid = sw.GetMaxID("BZ_MASTER_FEEDBACK_LIST");

            conn = new cls_connection();
            if (data.data_type.ToString() != "")
            {
                sqlstr_all = "";
                if (data.feedback_list.Count > 0)
                {
                    List<MasterNormalModel> dtlist = data.feedback_list;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }


                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_MASTER_FEEDBACK_LIST
                                    (ID,FEEDBACK_TYPE_ID,QUESTION_OTHER,NAME,STATUS,SORT_BY,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].id_main, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].question_other, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].name, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_MASTER_FEEDBACK_LIST set";

                            sqlstr += @" NAME = " + conn.ChkSqlStr(dtlist[i].name, 300);
                            sqlstr += @" ,STATUS = " + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ,SORT_BY = " + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                            sqlstr += @" ,QUESTION_OTHER = " + conn.ChkSqlStr(dtlist[i].question_other, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and FEEDBACK_TYPE_ID = " + conn.ChkSqlStr(dtlist[i].id_main, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_MASTER_FEEDBACK_LIST ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and FEEDBACK_TYPE_ID = " + conn.ChkSqlStr(dtlist[i].id_main, 300);

                        }

                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";
                        if (ret.ToLower() != "true") { goto Next_line_1; }

                        if (action_type == "delete" && (dtlist[i].sub_data.ToString() == "true"))
                        {
                            //delete BZ_MASTER_FEEDBACK_LIST  
                            sqlstr = @" delete from BZ_MASTER_FEEDBACK_QUESTION ";
                            sqlstr += @" where ";
                            sqlstr += @" FEEDBACK_LIST_ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and FEEDBACK_TYPE_ID = " + conn.ChkSqlStr(dtlist[i].id_main, 300);

                            ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                            sqlstr_all += sqlstr + "||";
                            if (ret.ToLower() != "true") { goto Next_line_1; }
                        }


                    }
                }

            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = SetDocService.conn_ExecuteNonQuery(sqlstr_all, false);
                }
            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() == "")
            {
                msg_error = ret;
            }
            else if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {

                SearchMasterDataService swd = new SearchMasterDataService();
                MMaintainDataModel value_load = new MMaintainDataModel();
                value_load.token_login = data.token_login;
                value_load.page_name = data.page_name;
                value_load.module_name = data.module_name;

                data = new MMaintainDataModel();
                data = swd.SearchFeedbackList(value_load);
            }

           data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
           data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
           data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
           data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt3.status = "Error msg";
           data.after_trip.opt3.remark = msg_error;


            return data;
        }
        public MMaintainDataModel SetFeedbackQuestion(MMaintainDataModel value)
        {
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data  
            sw = new SetDocService();
            int imaxid = sw.GetMaxID("BZ_MASTER_FEEDBACK_QUESTION");

            conn = new cls_connection();
            if (data.data_type.ToString() != "")
            {
                sqlstr_all = "";
                if (data.feedback_question.Count > 0)
                {
                    List<MasterNormalModel> dtlist = data.feedback_question;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }


                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_MASTER_FEEDBACK_QUESTION
                                    (ID,FEEDBACK_TYPE_ID,FEEDBACK_LIST_ID,QUESTION,DESCRIPTION,STATUS,SORT_BY,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].id_main, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].id_sub, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].name, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].description, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_MASTER_FEEDBACK_QUESTION set";

                            sqlstr += @" QUESTION = " + conn.ChkSqlStr(dtlist[i].name, 300);
                            sqlstr += @" ,DESCRIPTION = " + conn.ChkSqlStr(dtlist[i].description, 300);
                            sqlstr += @" ,STATUS = " + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ,SORT_BY = " + conn.ChkSqlStr(dtlist[i].sort_by, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and FEEDBACK_TYPE_ID = " + conn.ChkSqlStr(dtlist[i].id_main, 300);
                            sqlstr += @" and FEEDBACK_LIST_ID = " + conn.ChkSqlStr(dtlist[i].id_sub, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_MASTER_FEEDBACK_QUESTION ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                            sqlstr += @" and FEEDBACK_TYPE_ID = " + conn.ChkSqlStr(dtlist[i].id_main, 300);
                            sqlstr += @" and FEEDBACK_LIST_ID = " + conn.ChkSqlStr(dtlist[i].id_sub, 300);
                        }

                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                    }
                }

            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = SetDocService.conn_ExecuteNonQuery(sqlstr_all, false);
                }
            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() == "")
            {
                msg_error = ret;
            }
            else if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {
                SearchMasterDataService swd = new SearchMasterDataService();
                MMaintainDataModel value_load = new MMaintainDataModel();
                value_load.token_login = data.token_login;
                value_load.page_name = data.page_name;
                value_load.module_name = data.module_name;

                data = new MMaintainDataModel();
                data = swd.SearchFeedbackQuestion(value_load);
            }

           data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
           data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
           data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
           data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt3.status = "Error msg";
           data.after_trip.opt3.remark = msg_error;


            return data;
        }

        public MMaintainDataModel SetConfigDailyAllowance(MMaintainDataModel value)
        {
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data  
            sw = new SetDocService();
            int imaxid = sw.GetMaxID("BZ_CONFIG_DAILY_ALLOWANCE");

            conn = new cls_connection();
            if (data.data_type.ToString() != "")
            {
                sqlstr_all = "";
                if (data.allowance_list.Count > 0)
                {
                    List<MasterAllowance_ListModel> dtlist = data.allowance_list;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }


                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_CONFIG_DAILY_ALLOWANCE
                                    (ID,TRAVEL_CATEGORY,OVERNIGHT_TYPE,KH_CODE,WORKPLACE,WORKPLACE_TYPE_COUNTRY
                                    ,ALLOWANCE_RATE,CURRENCY,STATUS,SORT_BY,REMARK
                                    ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].travel_category, 300);
                            sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].overnight_type, "D");
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].kh_code, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].workplace, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].workplace_type_country, 300);
                            sqlstr += @" ," + conn.ChkSqlNum(dtlist[i].allowance_rate, "D");
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].currency, 300);

                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].remark, 4000);

                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";



                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_CONFIG_DAILY_ALLOWANCE set";

                            sqlstr += @" TRAVEL_CATEGORY = " + conn.ChkSqlStr(dtlist[i].travel_category, 300);
                            sqlstr += @" ,OVERNIGHT_TYPE = " + conn.ChkSqlNum(dtlist[i].overnight_type, "D");
                            sqlstr += @" ,KH_CODE = " + conn.ChkSqlStr(dtlist[i].kh_code, 300);

                            sqlstr += @" ,WORKPLACE = " + conn.ChkSqlStr(dtlist[i].workplace, 300);
                            sqlstr += @" ,WORKPLACE_TYPE_COUNTRY = " + conn.ChkSqlStr(dtlist[i].workplace_type_country, 300);
                            sqlstr += @" ,ALLOWANCE_RATE = " + conn.ChkSqlNum(dtlist[i].allowance_rate, "D");
                            sqlstr += @" ,CURRENCY = " + conn.ChkSqlStr(dtlist[i].currency, 300);

                            sqlstr += @" ,STATUS = " + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ,SORT_BY = " + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                            sqlstr += @" ,REMARK = " + conn.ChkSqlStr(dtlist[i].remark, 4000);


                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_CONFIG_DAILY_ALLOWANCE ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                        }

                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                    }
                }

            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = SetDocService.conn_ExecuteNonQuery(sqlstr_all, false);
                }
            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() == "")
            {
                msg_error = ret;
            }
            else if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {
                SearchMasterDataService swd = new SearchMasterDataService();
                MMaintainDataModel value_load = new MMaintainDataModel();
                value_load.token_login = data.token_login;
                value_load.page_name = data.page_name;
                value_load.module_name = data.module_name;

                data = new MMaintainDataModel();
                data = swd.SearchConfigDailyAllowance(value_load);
            }

           data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
           data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
           data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
           data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt3.status = "Error msg";
           data.after_trip.opt3.remark = msg_error;


            return data;
        }

        public MMaintainDataModel SetInsurancePlan(MMaintainDataModel value)
        {
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data  
            sw = new SetDocService();
            int imaxid = sw.GetMaxID("BZ_MASTER_FEEDBACK_TYPE");

            conn = new cls_connection();
            if (data.data_type.ToString() != "")
            {
                sqlstr_all = "";
                if (data.feedback_type.Count > 0)
                {
                    List<MasterNormalModel> dtlist = data.feedback_type;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }


                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_MASTER_FEEDBACK_TYPE
                                    (ID,NAME,STATUS,SORT_BY,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].name, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_MASTER_FEEDBACK_TYPE set";

                            sqlstr += @" NAME = " + conn.ChkSqlStr(dtlist[i].name, 300);
                            sqlstr += @" ,STATUS = " + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ,SORT_BY = " + conn.ChkSqlStr(dtlist[i].sort_by, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_MASTER_FEEDBACK_TYPE ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);

                        }

                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                        if (action_type == "delete" && (dtlist[i].sub_data.ToString() == "true"))
                        {
                            //delete BZ_MASTER_FEEDBACK_LIST  
                            sqlstr = @" delete from BZ_MASTER_FEEDBACK_LIST ";
                            sqlstr += @" where ";
                            sqlstr += @" FEEDBACK_TYPE_ID = " + conn.ChkSqlStr(dtlist[i].id, 300);


                            ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                            sqlstr_all += sqlstr + "||";

                            if (ret.ToLower() != "true") { goto Next_line_1; }
                        }

                    }
                }

            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = SetDocService.conn_ExecuteNonQuery(sqlstr_all, false);
                }
            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() == "")
            {
                msg_error = ret;
            }
            else if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {

                SearchMasterDataService swd = new SearchMasterDataService();
                MMaintainDataModel value_load = new MMaintainDataModel();
                value_load.token_login = data.token_login;
                value_load.page_name = data.page_name;
                value_load.module_name = data.module_name;

                data = new MMaintainDataModel();
                data = swd.SearchFeedbackType(value_load);
            }

           data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
           data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
           data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
           data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt3.status = "Error msg";
           data.after_trip.opt3.remark = msg_error;


            return data;
        }
        public MMaintainDataModel SetVISADocument(MMaintainDataModel value)
        {
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data  
            sw = new SetDocService();
            int imaxid = sw.GetMaxID("BZ_MASTER_VISA_DOCUMENT");

            conn = new cls_connection();
            if (data.data_type.ToString() != "")
            {
                sqlstr_all = "";
                if (data.visa_document.Count > 0)
                {
                    List<MasterVISADocument_ListModel> dtlist = data.visa_document;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }


                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_MASTER_VISA_DOCUMENT
                                    (ID,NAME,DESCRIPTION,PREPARING_BY,STATUS,SORT_BY,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].name, 4000);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].description, 4000);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].preparing_by, 4000);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_MASTER_VISA_DOCUMENT set";

                            sqlstr += @" NAME = " + conn.ChkSqlStr(dtlist[i].name, 4000);
                            sqlstr += @" ,DESCRIPTION = " + conn.ChkSqlStr(dtlist[i].description, 4000);
                            sqlstr += @" ,PREPARING_BY = " + conn.ChkSqlStr(dtlist[i].preparing_by, 4000);
                            sqlstr += @" ,STATUS = " + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ,SORT_BY = " + conn.ChkSqlStr(dtlist[i].sort_by, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_MASTER_VISA_DOCUMENT ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);

                        }

                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                        if (action_type == "delete" && (dtlist[i].sub_data.ToString() == "true"))
                        {
                            sqlstr = @" delete from BZ_MASTER_VISA_DOCOUNTRIES ";
                            sqlstr += @" where ";
                            sqlstr += @" VISA_DOC_ID = " + conn.ChkSqlStr(dtlist[i].id, 300);

                            ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                            sqlstr_all += sqlstr + "||";

                            if (ret.ToLower() != "true") { goto Next_line_1; }
                        }

                    }
                }

            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = SetDocService.conn_ExecuteNonQuery(sqlstr_all, false);
                }
            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() == "")
            {
                msg_error = ret;
            }
            else if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {

                SearchMasterDataService swd = new SearchMasterDataService();
                MMaintainDataModel value_load = new MMaintainDataModel();
                value_load.token_login = data.token_login;
                value_load.page_name = data.page_name;
                value_load.module_name = data.module_name;

                data = new MMaintainDataModel();
                data = swd.SearchVISADocument(value_load);
            }

           data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
           data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
           data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
           data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt3.status = "Error msg";
           data.after_trip.opt3.remark = msg_error;


            return data;
        }
        public MMaintainDataModel SetVISADocountries(MMaintainDataModel value)
        {
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data  
            sw = new SetDocService();
            int imaxid = sw.GetMaxID("BZ_MASTER_VISA_DOCOUNTRIES");
            int imaxidImg = sw.GetMaxID("BZ_DOC_IMG");

            conn = new cls_connection();
            if (data.data_type.ToString() != "")
            {
                sqlstr_all = "";
                if (data.visa_docountries.Count > 0)
                {
                    List<MasterVISADocountries_ListModel> dtlist = data.visa_docountries;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true";
                        sqlstr = "";
                        var id_def = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }
                         
                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_MASTER_VISA_DOCOUNTRIES
                                    (ID,CONTINENT_ID,COUNTRY_ID,VISA_DOC_ID,NAME,DESCRIPTION,PREPARING_BY,STATUS,SORT_BY,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].continent_id, 4000);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].country_id, 4000);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].visa_doc_id, 4000);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].name, 4000);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].description, 4000);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].preparing_by, 4000);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";



                            //กรณีที่เป็นข้อมูลใหม่ ให้ map id ใหม่ให้กับ Img ด้วย  
                            if (data.img_list.Count > 0)
                            {
                                List<ImgList> drimg = data.img_list.Where(a => (a.id_level_1 == dtlist[i].id & a.id == dtlist[i].id)).ToList();
                                if (drimg.Count > 0)
                                {
                                    drimg[0].id_level_1 = imaxid.ToString();
                                }
                            }
                             
                            id_def = imaxid.ToString(); 
                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_MASTER_VISA_DOCOUNTRIES set";

                            sqlstr += @" VISA_DOC_ID = " + conn.ChkSqlStr(dtlist[i].visa_doc_id, 300);
                            sqlstr += @" ,CONTINENT_ID = " + conn.ChkSqlStr(dtlist[i].continent_id, 300);
                            sqlstr += @" ,COUNTRY_ID = " + conn.ChkSqlStr(dtlist[i].country_id, 300);
                            sqlstr += @" ,NAME = " + conn.ChkSqlStr(dtlist[i].name, 4000);
                            sqlstr += @" ,DESCRIPTION = " + conn.ChkSqlStr(dtlist[i].description, 4000);
                            sqlstr += @" ,PREPARING_BY = " + conn.ChkSqlStr(dtlist[i].preparing_by, 4000);
                            sqlstr += @" ,STATUS = " + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ,SORT_BY = " + conn.ChkSqlStr(dtlist[i].sort_by, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_MASTER_VISA_DOCOUNTRIES ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);

                        }

                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                    }
                }

                if (data.img_list.Count > 0)
                {
                    ret = "true"; sqlstr = "";
                    ret = sw.SetImgList(data.img_list, imaxidImg, emp_user_active, token_login, ref conn, ref sqlstr_all);
                    if (ret.ToLower() != "true") { goto Next_line_1; }
                }

            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = SetDocService.conn_ExecuteNonQuery(sqlstr_all, false);
                }
            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() == "")
            {
                msg_error = ret;
            }
            else if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {

                SearchMasterDataService swd = new SearchMasterDataService();
                MMaintainDataModel value_load = new MMaintainDataModel();
                value_load.token_login = data.token_login;
                value_load.page_name = data.page_name;
                value_load.module_name = data.module_name;

                data = new MMaintainDataModel();
                data = swd.SearchVISADocountries(value_load);
            }

           data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
           data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
           data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
           data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt3.status = "Error msg";
           data.after_trip.opt3.remark = msg_error;


            return data;
        }
        public MMaintainDataModel SetInsurancebroker(MMaintainDataModel value)
        {
            var doc_type = value.data_type;
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = data.token_login;

            #region set data  
            sw = new SetDocService();
            int imaxid = sw.GetMaxID("BZ_MASTER_INSURANCE_COMPANY");

            conn = new cls_connection();
            if (data.data_type.ToString() != "")
            {
                sqlstr_all = "";
                if (data.master_insurancebroker.Count > 0)
                {
                    List<MMasterInsurancebrokerModel> dtlist = data.master_insurancebroker;
                    for (int i = 0; i < dtlist.Count; i++)
                    {
                        ret = "true"; sqlstr = "";
                        var action_type = dtlist[i].action_type.ToString();
                        if (action_type == "") { continue; }
                        else if (action_type != "delete")
                        {
                            var action_change = dtlist[i].action_change + "";
                            if (action_change.ToLower() != "true") { continue; }
                        }


                        if (action_type == "insert")
                        {
                            sqlstr = @" insert into  BZ_MASTER_INSURANCE_COMPANY
                                    (ID,NAME,EMAIL,TRAVELCOMPANY_TYPE,STATUS,STATUS_ISOS,STATUS_INSURANCE,SORT_BY,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

                            sqlstr += @" " + imaxid;
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].name, 4000);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].email, 4000);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].travelcompany_type, 4000); 
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status_isos, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].status_insurance, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(dtlist[i].sort_by, 300);
                            sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,sysdate";
                            sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            imaxid++;
                        }
                        else if (action_type == "update")
                        {
                            sqlstr = @" update BZ_MASTER_INSURANCE_COMPANY set";

                            sqlstr += @" NAME = " + conn.ChkSqlStr(dtlist[i].name, 300); 
                            sqlstr += @" ,EMAIL = " + conn.ChkSqlStr(dtlist[i].email, 4000);
                            sqlstr += @" ,TRAVELCOMPANY_TYPE = " + conn.ChkSqlStr(dtlist[i].travelcompany_type, 4000);
                            sqlstr += @" ,STATUS = " + conn.ChkSqlStr(dtlist[i].status, 300);
                            sqlstr += @" ,STATUS_ISOS = " + conn.ChkSqlStr(dtlist[i].status_isos, 300);
                            sqlstr += @" ,STATUS_INSURANCE = " + conn.ChkSqlStr(dtlist[i].status_insurance, 300);
                            sqlstr += @" ,SORT_BY = " + conn.ChkSqlStr(dtlist[i].sort_by, 300);

                            sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);
                        }
                        else if (action_type == "delete")
                        {
                            sqlstr = @" delete from BZ_MASTER_INSURANCE_COMPANY ";
                            sqlstr += @" where ";
                            sqlstr += @" ID = " + conn.ChkSqlStr(dtlist[i].id, 300);

                        }

                        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                        sqlstr_all += sqlstr + "||";

                        if (ret.ToLower() != "true") { goto Next_line_1; }

                    }
                }

            Next_line_1:;
                if (ret.ToLower() == "true")
                {
                    ret = SetDocService.conn_ExecuteNonQuery(sqlstr_all, false);
                }
            }
            #endregion set data

            var msg_error = "";
            if (ret.ToLower() == "")
            {
                msg_error = ret;
            }
            else if (ret.ToLower() != "true")
            {
                msg_error = ret + " --> query error :" + sqlstr;
            }
            else
            {

                SearchMasterDataService swd = new SearchMasterDataService();
                MMaintainDataModel value_load = new MMaintainDataModel();
                value_load.token_login = data.token_login;
                value_load.page_name = data.page_name;
                value_load.module_name = data.module_name;

                data = new MMaintainDataModel();
                data = swd.SearchInsurancebroker(value_load);
            }

           data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
           data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Save data succesed." : "Save data failed.";
           data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
           data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
           data.after_trip.opt3.status = "Error msg";
           data.after_trip.opt3.remark = msg_error;


            return data;
        }
    }
}