using System;
using System.Web.Services;
using System.Data;
using top.ebiz.service.Models;

namespace top.ebiz.service.Service
{
    /// <summary>
    /// Summary description for SAP_ZTHRTEB020
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class SAP_ZTHRTEB020 : System.Web.Services.WebService
    {
        string SAP_AppServerHost = System.Configuration.ConfigurationManager.AppSettings["SAP_AppServerHost"].ToString();
        string SAP_Client = System.Configuration.ConfigurationManager.AppSettings["SAP_Client"].ToString();
        string SAP_Username = System.Configuration.ConfigurationManager.AppSettings["SAP_Username"].ToString();
        string SAP_Password = System.Configuration.ConfigurationManager.AppSettings["SAP_Password"].ToString();

        //2. BAPI se37/ZTHROMB005 RFC สำหรับ update ข้อมูลพนักงาน เพื่ออัพเดตข้อมูลการเคลมค่าพาสปอร์ตและกระเป๋าเดินทางเข้า SAP  
        //-->pa30
        //3. BAPI se37/ZTHRTEB020 Create Object E(Course) with Resource เพื่อสร้าง Course ลงใน SAP
        //-->lso_psv2 
        //4. BAPI se37/ZTHRTEB021 LSO Create Course Booking(E->P) เพื่อ Book คนลง course

        [WebMethod]
        public string ZTHROMB005_ref(string emp_id, string sdate, string edate)
        {
            string msg = "";

            ZTHROMB005.ZESS_PA0002_UTable PA0002 = new ZTHROMB005.ZESS_PA0002_UTable();
            ZTHROMB005.ZESS_PA0006_UTable PA0006 = new ZTHROMB005.ZESS_PA0006_UTable();
            ZTHROMB005.ZESS_PA0019_UTable PA0019 = new ZTHROMB005.ZESS_PA0019_UTable();
            ZTHROMB005.ZESS_PA0021_UTable PA0021 = new ZTHROMB005.ZESS_PA0021_UTable();
            ZTHROMB005.ZESS_PA0041_UTable PA0041 = new ZTHROMB005.ZESS_PA0041_UTable();
            ZTHROMB005.ZESS_PA0182_UTable PA0182 = new ZTHROMB005.ZESS_PA0182_UTable();
            ZTHROMB005.ZESS_PA0185_UTable PA0185 = new ZTHROMB005.ZESS_PA0185_UTable();
            ZTHROMB005.ZESS_PA0364_UTable PA0364 = new ZTHROMB005.ZESS_PA0364_UTable();

            //ข้อมูลที่นำเข้า SAP จะอยู่ในรูปแบบของ Table โดยแต่ละรายการมีข้อมูลและ Data Type ดังนี้ (ตาม requirement จะมีการใช้เฉพาะ Input เข้าข้อมูลที่ไฮไลท์สีเหลืองเท่านั้น)input  
            ZTHROMB005.ZESS_PA0019_U PA0019ref = new ZTHROMB005.ZESS_PA0019_U();
            PA0019ref.Id = 1;//Recoder ID
            PA0019ref.Pernr = emp_id;//Personel number
            PA0019ref.Subty = "AE";//Subtype
            PA0019ref.Mndat = edate;//Reminder Date 
            PA0019ref.Termn = sdate;//Date of Task
            PA0019.Add(PA0019ref);
            //??? ตอนนี้เหมือนจะเป็นการเพิ่มข้อมูลเข้า SAP อย่างเดียวไม่มีการ update ข้อมูล

            #region call
            ZTHROMB005.ZTHROMB005 bapi = new ZTHROMB005.ZTHROMB005();
            System.Net.NetworkCredential net = new System.Net.NetworkCredential();
            SAP.Connector.Destination Connector = new SAP.Connector.Destination();

            Connector.AppServerHost = SAP_AppServerHost;
            Connector.SystemNumber = 0;
            Connector.Client = Convert.ToInt16(SAP_Client);
            Connector.Username = SAP_Username;
            Connector.Password = SAP_Password;
            bapi.Connection = new SAP.Connector.SAPConnection(Connector);

            bapi.Zthromb005(ref PA0002, ref PA0006, ref PA0019, ref PA0021, ref PA0041, ref PA0182, ref PA0185, ref PA0364);

            //SAP จะมีการ return status ให้ในแต่ละรายการผ่าน RET_TYPE และ RET_MESSAGE หลังจากทำการ import แล้ว   
            if (PA0019 != null & PA0019.Count > 0)
            {
                if (PA0019[0].Ret_Type.ToString() == "S") { msg = "true"; } else { msg = PA0019[0].Ret_Message.ToString(); }
            }
            #endregion call

            return msg;
        }

        [WebMethod]
        public string ZTHRTEB020_DOC(string doc_id, string emp_id, string sdate, string edate
            , string location, string token_login)
        {
            string msg = "";
            string msg2 = "";
            try
            {
                var emp_user_active = "";//เอา token_login ไปหา
                var status_sap = "";
                string msg_sap = ""; string obj_id = "";
                string d_obj_id = "";//BU-NISOP

                ws_conn.wsConnection conn = new ws_conn.wsConnection();
                DataTable dt = new DataTable();
                DataTable dtemp = new DataTable();
                string sqlstr = "";


                sqlstr = @" select distinct emp_id from bz_doc_travelexpense_detail a where a.doc_id = '" + doc_id + "'  ";
                conn = new ws_conn.wsConnection();
                dtemp = new DataTable();
                dtemp = conn.adapter_data(sqlstr);
                if (dtemp.Rows.Count == 0) { return "no data."; }

                sqlstr = @" select a.doc_id
                                 , met.name as expense_type
                                 , met.field_sap    
                                 , '' as obj_main
                                 , h.dh_topic as obj_subject 
                                 , sum(to_number(nvl(a.grand_total,0)))/a2.xcount as grand_total 
                                 from  bz_doc_travelexpense_detail a
                                 inner join bz_doc_head h on a.doc_id = h.dh_code 
                                 inner join bz_master_expense_type met on a.expense_type = met.id
                                 left join bz_master_list_status ms on a.status = ms.id
                                 left join (select count(emp_id) as xcount from(select distinct emp_id from bz_doc_travelexpense_detail 
                                 where doc_id = '" + doc_id + "') )a2 on 1=1 where a.doc_id = '" + doc_id + "' " +
                            @" group by  a.doc_id , met.name , met.field_sap , h.dh_topic, to_number(met.sort_by) ,a2.xcount
                                order by a.doc_id, to_number(met.sort_by)";

                conn = new ws_conn.wsConnection();
                dt = new DataTable();
                dt = conn.adapter_data(sqlstr);
                if (dt.Rows.Count == 0) { return "no data."; }

                string Short = doc_id;
                string Stext = dt.Rows[0]["obj_subject"].ToString();
                decimal dtotal_grand = 0;

                //SAP_ZTHRTEB020_OBJID เปลี่ยนไปใช้ตาราง BZ_CONFIG_DATA 
                var doc_type = "OVERSEA";
                conn = new ws_conn.wsConnection();
                sqlstr = @" select key_value, key_filter
                            from BZ_CONFIG_DATA
                            where key_name = 'SAP_ZTHRTEB020_OBJID'  and status = 1    
                            and substr(key_filter,0,1) = substr('" + doc_id.ToUpper() + "',0,1)";
                DataTable dtObj = conn.adapter_data(sqlstr);
                if (dtObj.Rows.Count > 0)
                {
                    d_obj_id = dtObj.Rows[0]["key_value"].ToString();
                    doc_type = dtObj.Rows[0]["key_filter"].ToString().ToLower();
                }

                //location -->
                string location_id = "";
                if (location == "") { if (doc_type == "local") { location = "1"; } else { location = "19"; } }
                sqlstr = @" select name, objectid as location_id, ct_id, pv_id  from BZ_MASTER_COURSE_LOCATION a
                            where 1=1 ";
                if (doc_type == "local")
                {
                    sqlstr += @" and pv_id in (" + location + ")";
                }
                else { sqlstr += @" and ct_id in (" + location + ")"; }
                DataTable dtLocation = conn.adapter_data(sqlstr);
                if (dtLocation.Rows.Count > 0)
                {
                    location_id = dtLocation.Rows[0]["location_id"].ToString();
                }

                msg2 = "start sap";

                #region structure
                //ข้อมูลที่นำเข้า SAP จะอยู่ในรูปแบบของ Table โดยแต่ละรายการมีข้อมูลและ Data Type ดังนี้ (ตาม requirement จะมีการใช้เฉพาะ Input เข้าข้อมูลที่ไฮไลท์สีเหลืองเท่านั้น)input  
                ZTHRTEB020.HRI1029 Im_Hrp1029 = new ZTHRTEB020.HRI1029();
                ZTHRTEB020.ZESS_HRP1036 Im_Hrp1036 = new ZTHRTEB020.ZESS_HRP1036();
                ZTHRTEB020.ZESS_HRP1036 Im_Hrp1036_Copy = new ZTHRTEB020.ZESS_HRP1036();
                ZTHRTEB020.P1000 Is_P1000 = new ZTHRTEB020.P1000();
                string Iv_Parent = "";
                string Iv_Istat = "";
                string Ev_Objid = "";
                ZTHRTEB020.P1002_EXPTable It_P1002 = new ZTHRTEB020.P1002_EXPTable();
                ZTHRTEB020.P1021Table It_P1021 = new ZTHRTEB020.P1021Table();
                ZTHRTEB020.P1024Table It_P1024 = new ZTHRTEB020.P1024Table();
                ZTHRTEB020.P1026Table It_P1026 = new ZTHRTEB020.P1026Table();
                ZTHRTEB020.P1035_EXPTable It_P1035 = new ZTHRTEB020.P1035_EXPTable();
                ZTHRTEB020.BAPIRET2Table Return_ref = new ZTHRTEB020.BAPIRET2Table();
                ZTHRTEB020.ZLSO_HRP1001_A023Table Tab_A023 = new ZTHRTEB020.ZLSO_HRP1001_A023Table();
                ZTHRTEB020.ZLSO_HRP1001Table Tab_Hrp1001 = new ZTHRTEB020.ZLSO_HRP1001Table();
                ZTHRTEB020.ZLSO_HRP9403Table Tab_Hrp9403 = new ZTHRTEB020.ZLSO_HRP9403Table();
                #endregion structure

                #region input data 
                //3.1    ข้อมูลที่นำเข้า SAP ในส่วนของ IMPORT จะเป็นการนำเข้าทีละ course โดยใส่ข้อมูลดังนี้ 
                //3.1.1 IS_P1000        TYPE structure(P1000)   Required Field    
                //เพื่อใส่ข้อมูล master ของ course ได้แก่ ชื่อ(Short Name (12), Text(40)), Event Period
                Is_P1000.Objid = d_obj_id;
                Is_P1000.Begda = sdate;
                Is_P1000.Endda = edate;
                Is_P1000.Short = Short;// "EBiz 12- test post";
                Is_P1000.Stext = Stext;// "Test Post Business Trip 12";
                Is_P1000.Langu = "EN";

                //3.1.2 IV_PARENT       TYPE CHAR(8)            Optional Field     
                //เพื่อใส่ข้อมูล D object ID ที่จะให้ course ที่ต้องการสร้างอยู่ภายใต้ D นั้น(Recommend ให้ใส่) 
                Iv_Parent = d_obj_id;

                //3.1.3 IM_HRP1029      TYPE structure(HRI1029) Optional Field      
                //เพื่อใส่ข้อมูล Budget Type(ใช้ในกรณีทำค่าใช้จ่ายผ่าน SAP Training Expense ซึ่ง E - Business Trip ไม่ได้ใช้)

                //3.1.4 IM_HRP1036      TYPE structure(ZESS_HRP1036) Optional Field     
                //เพื่อใส่ข้อมูล Cost ได้เพียง 1 Subtype โดยสรุป E - Business Trip จะใส่ Actual Cost มา(Subtype 0001 Normal Case) 
                //IT_P1036 => มาจากตัวอย่าง file word ???    
                //Record2   
                Im_Hrp1036.Objid = d_obj_id;
                Im_Hrp1036.Begda = sdate;
                Im_Hrp1036.Endda = edate;
                Im_Hrp1036.Subty = "0001";

                //กรณีที่มีมากกว่า 1 Location 
                Im_Hrp1036_Copy.Objid = d_obj_id;
                Im_Hrp1036_Copy.Begda = sdate;
                Im_Hrp1036_Copy.Endda = edate;
                Im_Hrp1036_Copy.Subty = "0001";

                #region ค่าใช้จ่าย
                var iloop_def = 1;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ////ACCOM = ACCOMMODATION
                    //Im_Hrp1036.Kobes = "ACCOM";
                    //Im_Hrp1036.Prdir = "X";
                    //Im_Hrp1036.Price = 10000;
                    //Im_Hrp1036.Waers = "THB";
                    //Im_Hrp1036.Bunit = "0";
                    if (dt.Rows[i]["field_sap"].ToString() == "ACCOM")
                    {
                        Im_Hrp1036.Kobes = "ACCOM";
                        Im_Hrp1036.Prdir = "X";
                        Im_Hrp1036.Price = Convert.ToDecimal(dt.Rows[i]["grand_total"].ToString());
                        Im_Hrp1036.Waers = "THB";
                        Im_Hrp1036.Bunit = "0";
                        continue;
                    }
                    #region ต้อง loop ค่าตามข้อมูล
                    ////AIR_TK = AIR TICKET
                    //Im_Hrp1036.Kobes001 = "AIR_TK";
                    //Im_Hrp1036.Prdir001 = "X";
                    //Im_Hrp1036.Price001 = 20000;
                    //Im_Hrp1036.Waers001 = "THB";
                    //Im_Hrp1036.Bunit001 = "0";
                    var Kobes_def = dt.Rows[i]["field_sap"].ToString();// "AIR_TK";
                    var Price_def = Convert.ToDecimal(dt.Rows[i]["grand_total"].ToString());// 20000;
                    if (iloop_def == 1)
                    {
                        Im_Hrp1036.Kobes001 = Kobes_def;
                        Im_Hrp1036.Prdir001 = "X";
                        Im_Hrp1036.Price001 = Price_def;
                        Im_Hrp1036.Waers001 = "THB";
                        Im_Hrp1036.Bunit001 = "0";
                    }
                    else if (iloop_def == 2)
                    {
                        Im_Hrp1036.Kobes002 = Kobes_def;
                        Im_Hrp1036.Prdir002 = "X";
                        Im_Hrp1036.Price002 = Price_def;
                        Im_Hrp1036.Waers002 = "THB";
                        Im_Hrp1036.Bunit002 = "0";
                    }
                    else if (iloop_def == 3)
                    {
                        Im_Hrp1036.Kobes003 = Kobes_def;
                        Im_Hrp1036.Prdir003 = "X";
                        Im_Hrp1036.Price003 = Price_def;
                        Im_Hrp1036.Waers003 = "THB";
                        Im_Hrp1036.Bunit003 = "0";
                    }
                    else if (iloop_def == 4)
                    {
                        Im_Hrp1036.Kobes004 = Kobes_def;
                        Im_Hrp1036.Prdir004 = "X";
                        Im_Hrp1036.Price004 = Price_def;
                        Im_Hrp1036.Waers004 = "THB";
                        Im_Hrp1036.Bunit004 = "0";
                    }
                    else if (iloop_def == 5)
                    {
                        Im_Hrp1036.Kobes005 = Kobes_def;
                        Im_Hrp1036.Prdir005 = "X";
                        Im_Hrp1036.Price005 = Price_def;
                        Im_Hrp1036.Waers005 = "THB";
                        Im_Hrp1036.Bunit005 = "0";
                    }
                    else if (iloop_def == 6)
                    {
                        Im_Hrp1036.Kobes006 = Kobes_def;
                        Im_Hrp1036.Prdir006 = "X";
                        Im_Hrp1036.Price006 = Price_def;
                        Im_Hrp1036.Waers006 = "THB";
                        Im_Hrp1036.Bunit006 = "0";
                    }
                    else if (iloop_def == 7)
                    {
                        Im_Hrp1036.Kobes007 = Kobes_def;
                        Im_Hrp1036.Prdir007 = "X";
                        Im_Hrp1036.Price007 = Price_def;
                        Im_Hrp1036.Waers007 = "THB";
                        Im_Hrp1036.Bunit007 = "0";
                    }
                    else if (iloop_def == 8)
                    {
                        Im_Hrp1036.Kobes008 = Kobes_def;
                        Im_Hrp1036.Prdir008 = "X";
                        Im_Hrp1036.Price008 = Price_def;
                        Im_Hrp1036.Waers008 = "THB";
                        Im_Hrp1036.Bunit008 = "0";
                    }
                    iloop_def++;
                    #endregion ต้อง loop ค่าตามข้อมูล

                    dtotal_grand += Convert.ToDecimal(dt.Rows[i]["grand_total"].ToString());
                }
                #endregion ค่าใช้จ่าย

                //3.1.5 LV_ISTAT        TYPE CHAR(1)            Optional Field      
                //status default เป็น “1” ด้วย E-Business Trip ส่งมาคือรายการที่ Firmly แล้ว ฉะนั้นจะเป็น “1” เสมอ; 
                Iv_Istat = "1";

                //3.2 ข้อมูลที่นำเข้า SAP ในส่วนของ TABLE จะเป็นรายละเอียดข้อมูลของ course ที่นำเข้าโดยใส่ข้อมูลได้ดังนี้ 
                //3.2.1 IT_P1002 TYPE      structure(P1002_EXP)          Optional 
                //เพื่อใส่ข้อมูล Description ของ Course เช่น ชื่อเต็มที่มากกว่า 40 ตัวอักษร
                ZTHRTEB020.P1002_EXP P1002_def = new ZTHRTEB020.P1002_EXP();
                P1002_def.Objid = d_obj_id;
                P1002_def.Begda = sdate;
                P1002_def.Endda = edate;
                It_P1002.Add(P1002_def);

                //3.2.2 IT_P1021 TYPE      structure(P1021)          Optional 
                //เพื่อใส่ข้อมูล Price หน้าหลัก
                ZTHRTEB020.P1021 P1021_def = new ZTHRTEB020.P1021();
                P1021_def.Objid = d_obj_id;
                P1021_def.Begda = sdate;
                P1021_def.Endda = edate;
                P1021_def.Ekost = dtotal_grand;// 80000;
                It_P1021.Add(P1021_def);

                //3.2.3 IT_P1024 TYPE      structure(P1024)          Required 
                //เพื่อใส่ข้อมูล Capacity
                //IT_P1024 => มีตัวอย่าง file word ???
                ZTHRTEB020.P1024 P1024_def = new ZTHRTEB020.P1024();
                P1024_def.Objid = d_obj_id;
                P1024_def.Begda = sdate;
                P1024_def.Endda = edate;
                P1024_def.Kapz1 = 1;
                P1024_def.Kapz2 = 10;
                P1024_def.Kapz3 = 20;
                It_P1024.Add(P1024_def);

                //3.2.4 IT_P1026 TYPE      structure(P1026)          Optional 
                //เพื่อใส่ข้อมูล Business Event Info
                ZTHRTEB020.P1026 P1026_def = new ZTHRTEB020.P1026();
                P1026_def.Objid = d_obj_id;
                P1026_def.Begda = sdate;
                P1026_def.Endda = edate;
                It_P1026.Add(P1026_def);

                //3.2.5 IT_P1035 TYPE      structure(P1035)          Required
                //เพื่อใส่ข้อมูลตารางเวลาของ course
                //IT_P1035  => มีตัวอย่าง file word ???   
                var totoal_day = 0;
                var totoal_hours = 0;
                DateTime dstart = new DateTime(Convert.ToInt32(sdate.Substring(0, 4))
                   , Convert.ToInt32(sdate.Substring(4, 2)), Convert.ToInt32(sdate.Substring(6, 2)));
                DateTime dedate = new DateTime(Convert.ToInt32(edate.Substring(0, 4))
                    , Convert.ToInt32(edate.Substring(4, 2)), Convert.ToInt32(edate.Substring(6, 2)));
                totoal_day = ((dedate - dstart).Days);
                totoal_hours = (totoal_day * 24);
                ZTHRTEB020.P1035_EXP P1035_def = new ZTHRTEB020.P1035_EXP();
                P1035_def.Objid = d_obj_id;
                P1035_def.Begda = sdate;
                P1035_def.Endda = edate;
                P1035_def.Ndays = totoal_day;//จำนวนวัน
                P1035_def.Nhours = totoal_hours;//จำนวน ชม
                It_P1035.Add(P1035_def);

                //3.2.6 TAB_HRP1001 TYPE      structure(ZLSO_HRP1001)              Required 
                //เพื่อใส่ข้อมูล Relationship เช่น HRP1001 subtype A024 “Takes place in” with object Location F, Cost Center(K) (K ใช้ในกรณีทำค่าใช้จ่ายผ่าน SAP Training Expense ซึ่ง E-Business Trip ไม่ได้ใช้)
                ZTHRTEB020.ZLSO_HRP1001 HRP1001_def = new ZTHRTEB020.ZLSO_HRP1001();
                HRP1001_def.Rsign = "A";
                HRP1001_def.Relat = "024";
                HRP1001_def.Sclas = "F";
                HRP1001_def.Sobid = location_id;// "00900104";
                Tab_Hrp1001.Add(HRP1001_def);

                #endregion input data 

                #region กรณีที่เดินทางมากกว่า 1 Location ให้ลงค่าใช้จ่ายที่ Location แรก
                //กรณีที่มีมากกว่า 1 Location -->Im_Hrp1036_Copy
                if (dtLocation.Rows.Count > 0)
                {
                    for (int i = 0; i < dtLocation.Rows.Count; i++)
                    {
                        string ct_id = dtLocation.Rows[i]["ct_id"].ToString();
                        string pv_id = dtLocation.Rows[i]["pv_id"].ToString();
                        //แก้ไข location id 
                        location_id = dtLocation.Rows[i]["location_id"].ToString();
                        Tab_Hrp1001[0].Sobid = location_id;

                        #region call 
                        ZTHRTEB020.ZTHRTEB020 bapi = new ZTHRTEB020.ZTHRTEB020();
                        System.Net.NetworkCredential net = new System.Net.NetworkCredential();
                        SAP.Connector.Destination Connector = new SAP.Connector.Destination();

                        Connector.AppServerHost = SAP_AppServerHost;
                        Connector.SystemNumber = 0;
                        Connector.Client = Convert.ToInt16(SAP_Client);
                        Connector.Username = SAP_Username;
                        Connector.Password = SAP_Password;
                        bapi.Connection = new SAP.Connector.SAPConnection(Connector);

                        if (i == 0)
                        {
                            bapi.Zthrteb020(
                                Im_Hrp1029, Im_Hrp1036, Is_P1000, Iv_Parent, Iv_Istat
                                , out Ev_Objid
                                , ref It_P1002, ref It_P1021, ref It_P1024, ref It_P1026, ref It_P1035
                                , ref Return_ref, ref Tab_A023, ref Tab_Hrp1001, ref Tab_Hrp9403);
                        }
                        else
                        {
                            bapi.Zthrteb020(
                               Im_Hrp1029, Im_Hrp1036_Copy, Is_P1000, Iv_Parent, Iv_Istat
                               , out Ev_Objid
                               , ref It_P1002, ref It_P1021, ref It_P1024, ref It_P1026, ref It_P1035
                               , ref Return_ref, ref Tab_A023, ref Tab_Hrp1001, ref Tab_Hrp9403);
                        }

                        //3.3 SAP มีการ return ID ของ course ให้ในกรณีที่สร้างสำเร็จ ผ่าน Parameter EV_OBJID ที่อยู่ใน Export (CHAR(8))
                        obj_id = Ev_Objid;

                        if (obj_id != "" && obj_id != "00000000")
                        {
                            status_sap = "6";
                            for (int m = 0; m < dtemp.Rows.Count; m++)
                            {
                                emp_id = dtemp.Rows[m]["emp_id"].ToString();
                                msg_sap = ZTHRTEB021_ref(emp_id, sdate, edate, d_obj_id, ref obj_id);
                                if (msg_sap == "") { msg_sap = "S"; }
                                else
                                {
                                    msg_sap = "Error: Create Course Booking(E->P).";
                                    if (Return_ref.Count > 0) { msg_sap += " " + Return_ref[0].Message.ToString(); }
                                }
                            }
                        }
                        else
                        {
                            msg_sap = "Error: Create Object E(Course) with Resource.";
                            if (Return_ref.Count > 0) { msg_sap += " " + Return_ref[0].Message.ToString(); }
                        }

                        if (i == 0)
                        {
                            // update ข้อมูลหลังบ้าน 
                            sqlstr = @" update BZ_DOC_TRAVELEXPENSE_DETAIL set";
                            sqlstr += @" SAP_OBJ_ID = " + ChkSqlStr(obj_id, 20);
                            sqlstr += @" ,SAP_STATUS_BOOKING = " + ChkSqlStr(msg_sap, 4000);
                            if (status_sap != "")
                            {
                                sqlstr += @" ,STATUS = " + ChkSqlStr(status_sap, 4000);
                            }
                            sqlstr += @" ,UPDATE_BY = " + ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" ,UPDATE_DATE = sysdate";
                            sqlstr += @" ,TOKEN_UPDATE = " + ChkSqlStr(token_login, 300);
                            //sqlstr += @" where nvl(STATUS,0) not in ('6')  and nvl(STATUS_ACTIVE,'false') = 'true'  ";
                            //sqlstr += @" and DOC_ID = " + ChkSqlStr(doc_id, 300);
                            //sqlstr += @" and EMP_ID = " + ChkSqlStr(emp_id, 300);
                            sqlstr += @" where DOC_ID = " + ChkSqlStr(doc_id, 300);

                            msg = conn.execute_data(sqlstr, false);
                            if (msg.ToLower() == "true")
                            {
                                sqlstr = " delete from BZ_DOC_TRAVELEXPENSE_SAP ";
                                sqlstr += @" where DOC_ID = " + ChkSqlStr(doc_id, 300);
                                //sqlstr += @" and EMP_ID = " + ChkSqlStr(emp_id, 300);
                                msg = conn.execute_data(sqlstr, false);
                            }
                        }

                        if (msg.ToLower() == "true")
                        {
                            //บันทึกข้อมูลลง BZ_DOC_TRAVELEXPENSE_SAP
                            string id = (i + 1).ToString();
                            string type_data = "true";
                            if (i > 0) { type_data = "false"; }

                            sqlstr = @" insert into BZ_DOC_TRAVELEXPENSE_SAP (DOC_ID, EMP_ID, ID, TYPE_MAIN, CT_ID, PV_ID
                                    , SAP_OBJ_ID, SAP_STATUS_BOOKING, REMARK
                                    , CREATE_BY, CREATE_DATE, UPDATE_BY, UPDATE_DATE, TOKEN_UPDATE) ";
                            sqlstr += @" values (";
                            sqlstr += @" " + ChkSqlStr(doc_id, 20);
                            sqlstr += @" ," + ChkSqlStr(emp_id, 20);
                            sqlstr += @" ," + ChkSqlStr(id, 20);
                            sqlstr += @" ," + ChkSqlStr(type_data, 20);
                            sqlstr += @" ," + ChkSqlStr(ct_id, 20);
                            sqlstr += @" ," + ChkSqlStr(pv_id, 20);
                            sqlstr += @" ," + ChkSqlStr(obj_id, 20);
                            sqlstr += @" ," + ChkSqlStr(msg_sap, 4000);
                            sqlstr += @" ," + ChkSqlStr(status_sap, 4000);

                            sqlstr += @" ," + ChkSqlStr(emp_user_active, 300);//user name login
                            sqlstr += @" , sysdate";
                            sqlstr += @" , null, null";
                            sqlstr += @" ," + ChkSqlStr(token_login, 300);
                            sqlstr += @" )";

                            msg = conn.execute_data(sqlstr, false);
                        }

                        #endregion call 
                    }
                }

                #endregion กรณีที่เดินทางมากกว่า 1 Location ให้ลงค่าใช้จ่ายที่ Location แรก


            }
            catch (Exception ex_msg) { msg = msg2 + " Error:" + ex_msg.Message.ToString(); }
            return msg;
        }

        public string ZTHRTEB021_ref(string emp_id, string sdate, string edate, string d_obj_id, ref string obj_id)
        {
            string msg = "";
            #region structure
            ZTHRTEB021.ZLSO_BOOKINGTable Tab_Booking = new ZTHRTEB021.ZLSO_BOOKINGTable();
            ZTHRTEB021.ZLSO_BOOKING values = new ZTHRTEB021.ZLSO_BOOKING();
            #endregion structure


            #region input data 
            //4.1    ข้อมูลที่นำเข้า SAP จะเป็นรูปแบบ Table นำเข้าได้ทีละหลายรายการ โดย
            //OTYPE คือ “E” 
            //และ SCLAS คือ “P” หากเป็นพนักงาน หรือ “H” หากเป็น external person 
            string sclass = "P";
            values.Otype = sclass;//Object type
            values.Objid = emp_id;//Object ID 
            values.Begda = sdate;//Start Date
            values.Endda = edate;//End Date 

            values.Sclas = "E";//Type of Related Object
            values.Sobid = obj_id;//ID of Related Object   

            Tab_Booking.Add(values);
            #endregion input data 


            #region call
            ZTHRTEB021.ZTHRTEB021 bapi = new ZTHRTEB021.ZTHRTEB021();
            System.Net.NetworkCredential net = new System.Net.NetworkCredential();
            SAP.Connector.Destination Connector = new SAP.Connector.Destination();

            Connector.AppServerHost = SAP_AppServerHost;
            Connector.SystemNumber = 0;
            Connector.Client = Convert.ToInt16(SAP_Client);
            Connector.Username = SAP_Username;
            Connector.Password = SAP_Password;
            bapi.Connection = new SAP.Connector.SAPConnection(Connector);

            bapi.Zthrteb021(ref Tab_Booking);

            //4.2    SAP จะมีการ return message เพื่อแสดงผลในการ import ข้อมูลแต่ละรายการผ่าน RET_TYPE และ RET__MESSAGE
            if (Tab_Booking != null & Tab_Booking.Count > 0)
            {
                if (Tab_Booking[0].Ret_Type.ToString() != "S") { msg = Tab_Booking[0].Ret_Message.ToString(); }
            }
            #endregion call


            return msg;
        }

        public string ChkSqlStr(object Str, int Length)
        {
            //วิธีที่ 1 --> แทนที่ ' ด้วย ช่องว่าง 1 ช่อง --> " " ทำให้ ' ใน base หายไป
            //วิธีที่ 2 --> แทนที่ ' ด้วย ''         --> Chr(39) & Chr(39) ทำให้ ' ใน base ยังอยู่ 

            //Str = "เลี้ยงตอบแทน' บ.Cyberouis, XX'XX'xxx'xxx"

            string Str1;

            if (Str == null || Convert.IsDBNull(Str))
            {
                return "null";
            }

            if (Str.ToString().ToLower() == "null")
            {
                return "null";
            }

            if (Str.ToString().Trim() == "")
            {
                return "null";
            }

            Str1 = Str.ToString();

            //วิธีที่ 1
            //Str1 = Replace(Str1, Chr(39), " ")

            //วิธีที่ 2
            //Str1 = Replace(Str1, Chr(39), Chr(39) & Chr(39))
            Str1 = Str1.Replace("'", "''");

            if (Str1.ToString().Length >= Length)
            {
                return "'" + Str1.ToString().Substring(0, Length) + "'";
            }
            else
            {
                return "'" + Str1.ToString().Trim() + "'";
            }
        }

        public DataTable refsearch_emprole_list(string role_type)
        {
            string sqlstr = "";
            role_type = role_type + "";

            var dt = new DataTable();
            //id,emp_id,username,name_th,name_en,email,telephone,mobile,imgurl
            sqlstr = @" select 0 as id,a.employeeid as emp_id,a.userid as username
                         ,trim(a.thtitle) ||' '|| trim(a.thfirstname) ||' '|| trim(a.thlastname) as name_th
                         ,case when usertype = 2 then a.enfirstname else a.entitle || ' ' || a.enfirstname || ' ' || a.enlastname end name_en
                         ,upper(a.email) as email
                         ,b.telephone
                         ,b.mobile 
                         ,case when b.imgurl is null then a.imgurl else b.imgurl end imgurl 
                         ,a.orgname as idicator
                         from vw_bz_users a
                         left join (select  b.userid, b.telephone, b.mobile, b.imgpath || b.imgprofilename  as imgurl from bz_user_peofile b
                         where (b.userid,nvl(b.update_date ,b.create_date )) in (
                         select userid,max(nvl(update_date ,create_date ))as  last_date  from bz_user_peofile  group by  userid)
                         ) b on lower(a.userid) = lower(b.userid)
                         where 1=1  ";
            if (role_type.ToLower() != "")
            {
                sqlstr += @" and  lower(a.userid) in (select lower(user_id) as userid from bz_data_manage where " + role_type.ToLower() + " = 'true')";
            }
            sqlstr += @" order by (a.enfirstname || ' ' || a.enlastname) ";

            ws_conn.wsConnection conn = new ws_conn.wsConnection();
            dt = new DataTable();
            dt = conn.adapter_data(sqlstr);

            return dt;
        }
        [WebMethod]
        public string CONFIRMATIONLETTER()
        {
            //ย้ายไปใช้ใน project ebiz.webservice แทน
            //CONFIRMATION LETTER	028_OB/LB/OT/LT : Business travel confirmation/information is now available!
            //ตั้ง batch --> Local 3 day, Overse 5 day  เช่น  busses พฤ ให้ส่งวันจันทร์  
            string msg = "";
            string sqlstr = "";

            //DevFix 20200910 0727 เพิ่มแนบ link Ebiz ด้วย Link ไปหน้า login 
            //http://tbkc-dapps-05.thaioil.localnet/Ebiz2/authen.aspx?page=/main/request/edit/###/i
            String LinkLogin = System.Configuration.ConfigurationManager.AppSettings["LinkLogin"].ToString();

            //DevFix 20211004 0000 เพิ่มแนบ link Ebiz Phase2  
            //http://tbkc-dapps-05.thaioil.localnet/Ebiz2/master/###/travelerhistory
            String LinkLoginPhase2 = System.Configuration.ConfigurationManager.AppSettings["LinkLoginPhase2"].ToString();

            string sDear = "";
            string sDetail = "";
            string sTitle = "";
            string sBusinessDate = "";
            string sLocation = "";
            string sTravelerList = "";
            string sOtherList = "";

            string email_admin = "";


            ws_conn.wsConnection conn = new ws_conn.wsConnection();
            sendEmailService mail = new sendEmailService();
            sendEmailModel dataMail = new sendEmailModel();

            DataTable dtresult = new DataTable();
            dtresult = refsearch_emprole_list("pmsv_admin");
            for (int i = 0; i < dtresult.Rows.Count; i++)
            {
                email_admin += dtresult.Rows[i]["email"] + ";";
            }
            dtresult = new DataTable();
            dtresult = refsearch_emprole_list("pmdv_admin");
            for (int i = 0; i < dtresult.Rows.Count; i++)
            {
                email_admin += dtresult.Rows[i]["email"] + ";";
            }

            string Tel_Services_Team = "";
            string Tel_Call_Center = "";
            string i_petty_cash = "";
            sqlstr = @" SELECT key_value as tel_services_team from bz_config_data where lower(key_name) = lower('tel_services_team') and status = 1";
            conn = new ws_conn.wsConnection();
            dtresult = conn.adapter_data(sqlstr);
            if (dtresult.Rows.Count == 0) { return "no data."; }
            try { Tel_Services_Team = dtresult.Rows[0]["tel_services_team"].ToString(); } catch { }

            sqlstr = @" SELECT key_value as tel_call_center from bz_config_data where lower(key_name) = lower('tel_call_center') and status = 1";
            dtresult = conn.adapter_data(sqlstr);
            if (dtresult.Rows.Count == 0) { return "no data."; }
            try { Tel_Call_Center = dtresult.Rows[0]["tel_call_center"].ToString(); } catch { }

            sqlstr = @" SELECT key_value as i_petty_cash from bz_config_data where lower(key_name) = lower('URL I-PETTY CASH') and status = 1";
            dtresult = conn.adapter_data(sqlstr);
            if (dtresult.Rows.Count == 0) { return "no data."; }
            try { i_petty_cash = dtresult.Rows[0]["i_petty_cash"].ToString(); } catch { }

            //หาใบงานที่ต้องแจ้งเตือน 
            sqlstr = @"select * from BZ_DOC_TRIP_COMPLETED ";
            conn = new ws_conn.wsConnection();
            DataTable dtdoc = conn.adapter_data(sqlstr);
            if (dtdoc.Rows.Count == 0) { return "no data."; }

            for (int i = 0; i < dtdoc.Rows.Count; i++)
            {
                string doc_id = dtdoc.Rows[i]["dh_code"].ToString();
                string doc_type = dtdoc.Rows[i]["dh_type"].ToString();//oversea,local
                string email_traverler = "";

                //http://tbkc-dapps-05.thaioil.localnet/Ebiz2/master/###/travelerhistory
                //http://tbkc-dapps-05.thaioil.localnet/Ebiz2/authen.aspx?page=master/OB21100059/airticket


                string url_air_ticket = (LinkLoginPhase2).Replace("master/###/travelerhistory", "authen.aspx?page=master/" + doc_id + "/airticket");
                string url_accommodation = (LinkLoginPhase2).Replace("master/###/travelerhistory", "authen.aspx?page=master/" + doc_id + "/accommodation");
                string url_allowance = (LinkLoginPhase2).Replace("master/###/travelerhistory", "authen.aspx?page=master/" + doc_id + "/allowance");
                string url_travel_insurance = (LinkLoginPhase2).Replace("master/###/travelerhistory", "authen.aspx?page=master/" + doc_id + "/travelinsurance");
                string url_isos = (LinkLoginPhase2).Replace("master/###/travelerhistory", "authen.aspx?page=master/" + doc_id + "/isos");
                string url_transportation = (LinkLoginPhase2).Replace("master/###/travelerhistory", "authen.aspx?page=master/" + doc_id + "/transportation");
                string url_reimbursement = (LinkLoginPhase2).Replace("master/###/travelerhistory", "authen.aspx?page=master/" + doc_id + "/reimbursement");
                string url_feedback = (LinkLoginPhase2).Replace("master/###/travelerhistory", "authen.aspx?page=master/" + doc_id + "/feedback");
                string url_i_petty_cash = i_petty_cash;

                #region traverler in doc
                sqlstr = @" select distinct nvl(b.ENTITLE,'')||' '||b.ENFIRSTNAME||' '||b.ENLASTNAME as emp_name, b.email as email  
                             , b.employeeid as emp_id, b.orgname  
                             from bz_doc_traveler_approver a
                             inner join BZ_DOC_HEAD h on h.dh_code=a.dh_code
                             inner join bz_users b on a.dta_travel_empid = b.employeeid 
                             where h.dh_doc_status = 50 
                             and a.dh_code ='" + doc_id + "' and a.dta_type = 2 and a.dta_action_status in (3) and a.dta_doc_status = 42 ";
                conn = new ws_conn.wsConnection();
                DataTable dtTravel = conn.adapter_data(sqlstr);
                if (dtTravel.Rows.Count == 0) { continue; ; }
                for (int j = 0; j < dtTravel.Rows.Count; j++)
                {
                    if (email_traverler != "") { email_traverler += ";"; }
                    email_traverler += dtTravel.Rows[j]["email"].ToString();
                }
                #endregion traverler in doc

                sqlstr = @"  select distinct h.dh_topic, h.dh_type
                             , to_char(min(to_date(to_char(te.dte_bus_todate, 'DD MON YYYY'), 'DD MON YYYY')), 'DD MON YYYY') as date_min
                             , to_char(max(to_date(to_char(te.dte_bus_todate, 'DD MON YYYY'), 'DD MON YYYY')), 'DD MON YYYY') as date_max
                             , min(to_date(to_char(te.dte_bus_todate, 'DD MON YYYY'), 'DD MON YYYY')) as bus_date
                             FROM bz_doc_traveler_expense te 
                             inner join BZ_DOC_HEAD h on h.dh_code=te.dh_code  
                             WHERE h.dh_doc_status = 50 and te.dte_status = 1 
                             and (nvl(te.dte_appr_opt,'true') = 'true' and nvl(te.dte_cap_appr_opt,'true') = 'true') 
                             and h.dh_code in ('" + doc_id + "') group by h.dh_topic, h.dh_type";

                sqlstr = @"select t.* from (" + sqlstr + ")t where t.bus_date = (sysdate- (case when t.dh_type like 'local%' then 3 else 5 end)) ";

                conn = new ws_conn.wsConnection();
                DataTable dtbusdate = conn.adapter_data(sqlstr);
                if (dtbusdate != null && dtbusdate.Rows.Count > 0)
                {
                    if (dtbusdate.Rows.Count == 0) { continue; }

                    sTitle = "Title : " + dtbusdate.Rows[0]["DH_TOPIC"].ToString() ?? "";
                    sBusinessDate = "Business Date : " + dtbusdate.Rows[0]["date_min"].ToString() + "-" + dtbusdate.Rows[0]["date_max"].ToString();
                }

                sqlstr = @"  select distinct h.dh_topic
                             , case when substr(te.dh_code,0,1) = 'L' then p.pv_name else c.ct_name end name1
                             , te.city_text as name2
                             FROM bz_doc_traveler_expense te 
                             inner join BZ_DOC_HEAD h on h.dh_code=te.dh_code
                             inner join BZ_USERS U on te.DTE_Emp_Id = u.employeeid
                             left join bz_master_country c on te.ct_id = c.ct_id
                             left join BZ_MASTER_PROVINCE p on te.PV_ID = p.PV_ID  
                             WHERE h.dh_doc_status = 50 and te.dte_status = 1 
                             and (nvl(te.dte_appr_opt,'true') = 'true' and nvl(te.dte_cap_appr_opt,'true') = 'true') 
                             and h.dh_code in ('" + doc_id + "') " +
                             " order by case when substr(te.dh_code,0,1) = 'L' then p.pv_name else c.ct_name end , te.city_text";
                conn = new ws_conn.wsConnection();
                DataTable dtLocation = conn.adapter_data(sqlstr);
                if (dtLocation != null && dtLocation.Rows.Count > 0)
                {
                    if (dtLocation.Rows.Count == 1)
                    {
                        sLocation = "Location : " + dtLocation.Rows[0]["name1"] + "/" + dtLocation.Rows[0]["name2"];
                    }
                    else
                    {
                        sLocation = "";
                        for (int l = 0; l < dtLocation.Rows.Count; l++)
                        {
                            if (sLocation == "") { sLocation = "Location : "; } else { sLocation += ","; }
                            sLocation += dtLocation.Rows[l]["name1"] + "/" + dtLocation.Rows[l]["name2"];
                        }
                    }
                }

                //TO: Traveler
                //CC : Admin - PMSV; Admin - PMDV(if any) ;
                //Subj: OB / LBYYMMXXXX : Business / Training Travel Confirmation Letter

                dataMail.mail_to = email_traverler;
                dataMail.mail_cc = email_admin;
                dataMail.mail_subject = doc_id + " : Travel arrangements for a business trip has been ready!";

                sDear = @"Dear All,";

                sDetail = "To get ready to travel, Business travel confirmation/information is now available;";
                sDetail += "<br>";
                sDetail += "To view the approval details, click ";
                sDetail += "<a href='" + (LinkLogin.Replace("/i", "/cap")).Replace("###", doc_id) + "'>" + doc_id + "</a>";
                sDetail += "<br>";
                sDetail += "To view travel details, click ";
                sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + "'>travel details.</a>";

                var iNo = 1;
                sTravelerList = "<table>";
                for (int j = 0; j < dtTravel.Rows.Count; j++)
                {
                    sTravelerList += " <tr>";
                    sTravelerList += " <td><font face='Browallia New,sans-serif' size='4'><span style='margin:0 0 0 20pt;font-size:15pt;'>" + iNo + ") " + dtTravel.Rows[j]["emp_name"].ToString() + "</span></font></td>";//1) [Title_Name of traveler] 
                    sTravelerList += " <td><font face='Browallia New,sans-serif' size='4'><span style='margin:0 0 0 36pt;font-size:15pt;'>" + dtTravel.Rows[j]["emp_id"].ToString() + "</span></font></td>";//Emp. ID
                    sTravelerList += " <td><font face='Browallia New,sans-serif' size='4'><span style='margin:0 0 0 36pt;font-size:15pt;'>" + dtTravel.Rows[j]["orgname"].ToString() + "</span></font></td>";//SEC./DEP./FUNC. 
                    sTravelerList += " </tr>";
                    iNo++;
                }
                sTravelerList += "</table>";

                sOtherList = @"<table width='1042' border='1' cellspacing='0' cellpadding='0' style='border-collapse:collapse;width:625.5pt;margin-left:35.7pt;border-style:none;'>";
                sOtherList += @"<tbody>";
                sOtherList += @"<tr height='48' style='height:28.8pt;'>
                                        <td width='187' style='width:112.5pt;height:28.8pt;background-color:#1F4E78;padding:0 5.4pt;border-style:solid dotted none solid;border-top-width:1pt;border-right-width:1pt;border-left-width:1pt;border-top-color:windowtext;border-right-color:#1F4E78;border-left-color:windowtext;'>
                                        <span style='background-color:#1F4E78;'>
                                        <div align='center' style='text-align:center;margin:0;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4' color='white'><span style='font-size:15pt;'><b>Items</b></span></font></span></font></div>
                                        </span></td><td width='855' style='width:513pt;height:28.8pt;background-color:#1F4E79;padding:0 5.4pt;border-style:solid solid none none;border-top-width:1pt;border-right-width:1pt;border-top-color:windowtext;border-right-color:windowtext;'>
                                        <span style='background-color:#1F4E79;'>
                                        <div align='center' style='text-indent:14.5pt;text-align:center;margin:0;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='DB Heavent' size='4' color='white'><span style='font-size:14pt;'><b>Details</b></span></font></span></font></div>
                                        </span></td></tr>";

                if (doc_type == "oversea")
                {
                    //สีขาว
                    sOtherList += @"<tr height='24' style='height:14.4pt;'>
                                <td width='187' valign='top' style='width:112.5pt;height:14.4pt;padding:0 5.4pt;border-style:none dotted none solid;border-right-width:1pt;border-left-width:1pt;border-right-color:#1F4E78;border-left-color:windowtext;'>
                                <div style='margin:0 0 0 8.1pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'>
                                <a href='" + url_air_ticket + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'><span style='font-size:15pt;'>AirTicket</span></font></a>" +
                                  @"</span></font></div></td>
                                <td width='855' style='width:513pt;height:14.4pt;padding:0 5.4pt;border-style:none solid none none;border-right-width:1pt;border-right-color:windowtext;'>
                                <div style='margin:0;'><font face='Browallia New,sans-serif' size='4' color='#222A35'><span style='font-size:15pt;'>
                                The Itinerary have been confirm, to check information" +
                                  @" <a href='" + url_air_ticket + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'><span style='font-size:15pt;'><i>click here</i></span></font></a>" +
                                  @" here and any incorrect, please contact to Business Travel  Services Team urgently.</font></div></td></tr>";

                    //สีฟ้า
                    sOtherList += @"<tr height='24' style='height:14.4pt;'>
                                <td width='187' valign='top' style='width:112.5pt;height:14.4pt;background-color:#DDEBF7;padding:0 5.4pt;border-style:none dotted none solid;border-right-width:1pt;border-left-width:1pt;border-right-color:#1F4E78;border-left-color:windowtext;'>
                                <span style='background-color:#DDEBF7;'>
                                <div style='margin:0 0 0 8.1pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'>
                                <a href='" + url_accommodation + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='black'>" +
                                  @"<span style='font-size:15pt;'>Accommodation</span></font></a></span></font></div></span></td>
                                <td width='855' valign='top' style='width:513pt;height:14.4pt;background-color:#DDEBF7;padding:0 5.4pt;border-style:none solid none none;border-right-width:1pt;border-right-color:windowtext;'>
                                <div style='margin:0;'><font face='Browallia New,sans-serif' size='4' color='#222A35'><span style='font-size:15pt;'>
                                The Itinerary have been confirm, to check information" +
                                  @" <a href='" + url_accommodation + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'><span style='font-size:15pt;'><i>click here</i></span></font></a>" +
                                  @" and any incorrect, please contact to Business Travel  Services Team urgently.</font></div></td></tr>";

                    //สีขาว
                    sOtherList += @"<tr height='24' style='height:14.4pt;'>
                                <td width='187' valign='top' style='width:112.5pt;height:14.4pt;padding:0 5.4pt;border-style:none dotted none solid;border-right-width:1pt;border-left-width:1pt;border-right-color:#1F4E78;border-left-color:windowtext;'>
                                <div style='margin:0 0 0 8.1pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'>
                                <a href='" + url_allowance + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'>" +
                                  @"<span style='font-size:15pt;'>Allowance</span></font></a></span></font></div></td>
                                <td width='855' style='width:513pt;height:14.4pt;padding:0 5.4pt;border-style:none solid none none;border-right-width:1pt;border-right-color:windowtext;'>
                                <div style='margin:0;'><font face='Browallia New,sans-serif' size='4' color='#222A35'><span style='font-size:15pt;'>
                                Individual allowance request to i-Petty cash to check document status" +
                                  @" <a href='" + url_i_petty_cash + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'><span style='font-size:15pt;'><i>click here</i></span></font></a>" +
                                  @" </font></div></td></tr>";

                    //สีฟ้า
                    sOtherList += @"<tr height='24' style='height:14.4pt;'>
                                <td width='187' valign='top' style='width:112.5pt;height:14.4pt;background-color:#DDEBF7;padding:0 5.4pt;border-style:none dotted none solid;border-right-width:1pt;border-left-width:1pt;border-right-color:#1F4E78;border-left-color:windowtext;'>
                                <span style='background-color:#DDEBF7;'>
                                <div style='margin:0 0 0 8.1pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'>
                                <a href='" + url_travel_insurance + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='black'>" +
                                  @"<span style='font-size:15pt;'>Travel Insurance</span></font></a></span></font></div></span></td>
                                    <td width='855' valign='top' style='width:513pt;height:14.4pt;background-color:#DDEBF7;padding:0 5.4pt;border-style:none solid none none;border-right-width:1pt;border-right-color:windowtext;'>
                                    <div style='margin:0;'><font face='Browallia New,sans-serif' size='4' color='#222A35'><span style='font-size:15pt;'>
                                    Please read carefully in Travel Insurance coverage" +
                                  @" <a href='" + url_travel_insurance + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'><span style='font-size:15pt;'><i>click here</i></span></font></a>" +
                                  @" and ISOS to medical assistance,
                                <br>international healthcare and security assistance" +
                                  @" <a href='" + url_isos + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'><span style='font-size:15pt;'><i>click here</i></span></font></a>" +
                                  @" before your travel date.</font></div></td></tr>";

                    //สีขาว
                    sOtherList += @"<tr height='24' style='height:14.4pt;'>
                                <td width='187' valign='top' style='width:112.5pt;height:14.4pt;padding:0 5.4pt;border-style:none dotted none solid;border-right-width:1pt;border-left-width:1pt;border-right-color:#1F4E78;border-left-color:windowtext;'>
                                <div style='margin:0 0 0 8.1pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'>
                                <a href='" + url_transportation + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'>" +
                                  @"<span style='font-size:15pt;'>Transportation</span></font></a></span></font></div></td>
                                    <td width='855' style='width:513pt;height:14.4pt;padding:0 5.4pt;border-style:none solid none none;border-right-width:1pt;border-right-color:windowtext;'>
                                    <div style='margin:0;'><font face='Browallia New,sans-serif' size='4' color='#222A35'><span style='font-size:15pt;'>
                                    To check/request your transportation for travel in Thailand or request a Private Car Requisition" +
                                  @" <a href='" + url_transportation + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'><span style='font-size:15pt;'><i>click here.</i></span></font></a>" +
                                  @" </font></div></td></tr>";

                    //สีฟ้า
                    sOtherList += @"<tr height='24' style='height:14.4pt;'>
                                <td width='187' valign='top' style='width:112.5pt;height:14.4pt;background-color:#DDEBF7;padding:0 5.4pt;border-style:none dotted none solid;border-right-width:1pt;border-left-width:1pt;border-right-color:#1F4E78;border-left-color:windowtext;'>
                                <span style='background-color:#DDEBF7;'>
                                <div style='margin:0 0 0 8.1pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'>
                                <a href='" + url_reimbursement + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='black'>" +
                                  @"<span style='font-size:15pt;'>Reimbursement</span></font></a></span></font></div></span></td>
                                    <td width='855' valign='top' style='width:513pt;height:14.4pt;background-color:#DDEBF7;padding:0 5.4pt;border-style:none solid none none;border-right-width:1pt;border-right-color:windowtext;'>
                                    <div style='margin:0;'><font face='Browallia New,sans-serif' size='4' color='#222A35'><span style='font-size:15pt;'>
                                    After approval/traveling you able to request reimbursement (actual payment claims) with receipt i.e. passport,
                                    <br> taxi by" +
                                  @" <a href='" + url_reimbursement + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'><span style='font-size:15pt;'><i>click here.</i></span></font></a>" +
                                  @" to get a formula form attached via E-Business system to i-Petty
                                    <br> cash or requested to i-Petty cash directly with attaches ; approval form receipts, currency refer (if any).
                                    </font></div></td></tr>";

                    //สีขาว
                    sOtherList += @"<tr height='24' style='height:14.4pt;'>
                                <td width='187' valign='top' style='width:112.5pt;height:14.4pt;padding:0 5.4pt;border-style:none dotted solid solid;border-right-width:1pt;border-bottom-width:1pt;border-left-width:1pt;border-right-color:#1F4E78;border-bottom-color:windowtext;border-left-color:windowtext;'>
                                <div style='margin:0 0 0 8.1pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'>
                                <a href='" + url_feedback + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'>" +
                                  @"<span style='font-size:15pt;'>Feedback</span></font></a></span></font></div></td>
                                    <td width='855' style='width:513pt;height:14.4pt;padding:0 5.4pt;border-style:none solid solid none;border-right-width:1pt;border-bottom-width:1pt;border-right-color:windowtext;border-bottom-color:windowtext;'>
                                    <div style='margin:0;'><font face='Browallia New,sans-serif' size='4' color='#222A35'><span style='font-size:15pt;'>
                                    Please complete feedback survey after travel as sending by E-Mail or" +
                                  @" <a href='" + url_feedback + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'><span style='font-size:15pt;'><i>click here</i></span></font></a>" +
                                  @" for feedback our service.
                                    </font></div></td></tr>";
                }
                else
                {
                    //สีขาว
                    sOtherList += @"<tr height='24' style='height:14.4pt;'>
                                    <td width='187' valign='top' style='width:112.5pt;height:14.4pt;padding:0 5.4pt;border-style:none dotted none solid;border-right-width:1pt;border-left-width:1pt;border-right-color:#1F4E78;border-left-color:windowtext;'>
                                    <div style='margin:0 0 0 8.1pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'>
                                    <a href='" + url_air_ticket + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'><span style='font-size:15pt;'>AirTicket</span></font></a>" +
                                  @"</span></font></div></td>
                                    <td width='855' style='width:513pt;height:14.4pt;padding:0 5.4pt;border-style:none solid none none;border-right-width:1pt;border-right-color:windowtext;'>
                                    <div style='margin:0;'><font face='Browallia New,sans-serif' size='4' color='#222A35'><span style='font-size:15pt;'>
                                    The Itinerary have been confirm, to check information" +
                                  @" <a href='" + url_air_ticket + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'><span style='font-size:15pt;'><i>click here</i></span></font></a>" +
                                  @" here and any incorrect, please contact to Business Travel  Services Team urgently.</font></div></td></tr>";

                    //สีฟ้า
                    sOtherList += @"<tr height='24' style='height:14.4pt;'>
                                    <td width='187' valign='top' style='width:112.5pt;height:14.4pt;background-color:#DDEBF7;padding:0 5.4pt;border-style:none dotted none solid;border-right-width:1pt;border-left-width:1pt;border-right-color:#1F4E78;border-left-color:windowtext;'>
                                    <span style='background-color:#DDEBF7;'>
                                    <div style='margin:0 0 0 8.1pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'>
                                    <a href='" + url_accommodation + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='black'>" +
                                  @"<span style='font-size:15pt;'>Accommodation</span></font></a></span></font></div></span></td>
                                    <td width='855' valign='top' style='width:513pt;height:14.4pt;background-color:#DDEBF7;padding:0 5.4pt;border-style:none solid none none;border-right-width:1pt;border-right-color:windowtext;'>
                                    <div style='margin:0;'><font face='Browallia New,sans-serif' size='4' color='#222A35'><span style='font-size:15pt;'>
                                    The Itinerary have been confirm, to check information" +
                                  @" <a href='" + url_accommodation + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'><span style='font-size:15pt;'><i>click here</i></span></font></a>" +
                                  @" and any incorrect, please contact to Business Travel  Services Team urgently.</font></div></td></tr>";

                    //สีขาว
                    sOtherList += @"<tr height='24' style='height:14.4pt;'>
                                    <td width='187' valign='top' style='width:112.5pt;height:14.4pt;padding:0 5.4pt;border-style:none dotted none solid;border-right-width:1pt;border-left-width:1pt;border-right-color:#1F4E78;border-left-color:windowtext;'>
                                    <div style='margin:0 0 0 8.1pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'>
                                    <a href='" + url_allowance + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'>" +
                                  @"<span style='font-size:15pt;'>Allowance</span></font></a></span></font></div></td>
                                    <td width='855' style='width:513pt;height:14.4pt;padding:0 5.4pt;border-style:none solid none none;border-right-width:1pt;border-right-color:windowtext;'>
                                    <div style='margin:0;'><font face='Browallia New,sans-serif' size='4' color='#222A35'><span style='font-size:15pt;'>
                                    Individual allowance request to i-Petty cash to check document status" +
                                  @" <a href='" + url_i_petty_cash + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'><span style='font-size:15pt;'><i>click here</i></span></font></a>" +
                                  @" </font></div></td></tr>";

                    //สีฟ้า
                    sOtherList += @"<tr height='24' style='height:14.4pt;'>
                                    <td width='187' valign='top' style='width:112.5pt;height:14.4pt;background-color:#DDEBF7;padding:0 5.4pt;border-style:none dotted none solid;border-right-width:1pt;border-left-width:1pt;border-right-color:#1F4E78;border-left-color:windowtext;'>
                                    <span style='background-color:#DDEBF7;'>
                                    <div style='margin:0 0 0 8.1pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'>
                                    <a href='" + url_transportation + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='black'>" +
                                  @"<span style='font-size:15pt;'>Transportation</span></font></a></span></font></div></span></td>
                                    <td width='855' valign='top' style='width:513pt;height:14.4pt;background-color:#DDEBF7;padding:0 5.4pt;border-style:none solid none none;border-right-width:1pt;border-right-color:windowtext;'>
                                    <div style='margin:0;'><font face='Browallia New,sans-serif' size='4' color='#222A35'><span style='font-size:15pt;'>
                                    To check/request your transportation for travel in Thailand or request a Private Car Requisition" +
                                  @" <a href='" + url_transportation + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'><span style='font-size:15pt;'><i>click here.</i></span></font></a>" +
                                  @" </font></div></td></tr>";

                    //สีขาว
                    sOtherList += @"<tr height='24' style='height:14.4pt;'>
                                    <td width='187' valign='top' style='width:112.5pt;height:14.4pt;padding:0 5.4pt;border-style:none dotted none solid;border-right-width:1pt;border-left-width:1pt;border-right-color:#1F4E78;border-left-color:windowtext;'>
                                    <div style='margin:0 0 0 8.1pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'>
                                    <a href='" + url_reimbursement + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'>" +
                                  @"<span style='font-size:15pt;'>Reimbursement</span></font></a></span></font></div></td>
                                    <td width='855' style='width:513pt;height:14.4pt;padding:0 5.4pt;border-style:none solid none none;border-right-width:1pt;border-right-color:windowtext;'>
                                    <div style='margin:0;'><font face='Browallia New,sans-serif' size='4' color='#222A35'><span style='font-size:15pt;'>
                                    After approval/traveling you able to request reimbursement (actual payment claims) with receipt i.e." +
                                  @" <br> taxi by" +
                                  @" <a href='" + url_reimbursement + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'><span style='font-size:15pt;'><i>click here.</i></span></font></a>" +
                                  @" to get a formula form attached via Business Travel Services to i-Petty cash or requested to i-Petty" +
                                  @" <br>cash directly with attaches ; approval form receipts, currency refer (if any).</font></div></td></tr>";

                    //สีฟ้า
                    sOtherList += @"<tr height='24' style='height:14.4pt;'>
                                    <td width='187' valign='top' style='width:112.5pt;height:14.4pt;background-color:#DEEAF6;padding:0 5.4pt;border-style:none dotted solid solid;border-right-width:1pt;border-bottom-width:1pt;border-left-width:1pt;border-right-color:#1F4E78;border-bottom-color:windowtext;border-left-color:windowtext;'>
                                    <span style='background-color:#DDEBF7;'>
                                    <div style='margin:0 0 0 8.1pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'>
                                    <a href='" + url_feedback + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='black'>" +
                                  @"<span style='font-size:15pt;'>Reimbursement</span></font></a></span></font></div></span></td>
                                    <td width='855' valign='top' style='width:513pt;height:14.4pt;background-color:#DEEAF6;padding:0 5.4pt;border-style:none solid solid none;border-right-width:1pt;border-bottom-width:1pt;border-right-color:windowtext;border-bottom-color:windowtext;'>
                                    <div style='margin:0;'><font face='Browallia New,sans-serif' size='4' color='#222A35'><span style='font-size:15pt;'>
                                    Please complete feedback survey after travel as sending by E-Mail or" +
                                  @" <a href='" + url_feedback + "' target='_blank'><font face='Browallia New,sans-serif' size='4' color='windowtext'><span style='font-size:15pt;'><i>click here.</i></span></font></a>" +
                                  @" for feedback our service.</font></div></td></tr>";

                }

                sOtherList += @" </tbody></table>";


                #region set mail
                dataMail.mail_body = @"<span lang='en-US'>";
                dataMail.mail_body += "<div>";
                dataMail.mail_body += "     <div style='margin:0;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>";
                dataMail.mail_body += "     " + sDear + "</span></font></div>";
                dataMail.mail_body += "     <br>";
                dataMail.mail_body += "     <div style='margin:0 0 0 36pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>";
                dataMail.mail_body += "     " + sDetail + "</span></font></div>";
                dataMail.mail_body += "     <br>";
                dataMail.mail_body += "     <div style='margin:0 0 0 36pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>";
                dataMail.mail_body += "     " + sTitle + "</span></font></div>";
                dataMail.mail_body += "     <div style='margin:0 0 0 36pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>";
                dataMail.mail_body += "     " + sBusinessDate + "</span></font></div>";
                dataMail.mail_body += "     <div style='margin:0 0 0 36pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>";
                dataMail.mail_body += "     " + sLocation + "</span></font></div>";
                dataMail.mail_body += "     <br>";
                dataMail.mail_body += "     <div style='margin:0 0 0 36pt;'><font face='Browallia New,sans-serif' size='4'>";
                dataMail.mail_body += "     Traveler List : " + sTravelerList + "</font></div>";
                dataMail.mail_body += "     <br>";

                dataMail.mail_body += "     <span style='font-size:15pt;'>" + sOtherList + "</span></font></div>";
                dataMail.mail_body += "     <br>";

                dataMail.mail_body += "     <div style='margin:0 0 0 36pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>";
                dataMail.mail_body += "     If you have any question please contact Business Travel Services Team (Tel. " + Tel_Services_Team + ").";
                dataMail.mail_body += "     <br>";
                dataMail.mail_body += "     For the application assistance, please contact PTT Digital Call Center (Tel. " + Tel_Call_Center + ").";
                dataMail.mail_body += "     </span></font></div>";

                dataMail.mail_body += "     <div style='margin:0;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>";
                dataMail.mail_body += "     <br>";
                dataMail.mail_body += "     Best Regards,";
                dataMail.mail_body += "     <br>";
                dataMail.mail_body += "     Business Travel Services Team (PMSV)";
                dataMail.mail_body += "     </span></font></div>";

                dataMail.mail_body += "</div>";
                dataMail.mail_body += "</span>";

                #endregion set mail
                mail.sendMail(dataMail);

            }


            return msg;
        }
    }
}

