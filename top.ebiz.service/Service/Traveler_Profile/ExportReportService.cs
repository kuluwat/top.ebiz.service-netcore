
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using OfficeOpenXml;
using top.ebiz.service.Models.Create_Trip;
using top.ebiz.service.Models.Traveler_Profile;


namespace top.ebiz.service.Service.Traveler_Profile
{
    public class ExportFileInModel
    {
        public string token_login { get; set; }
        public string doc_id { get; set; }
        public string emp_id { get; set; }

        public string path { get; set; }
        public string filename { get; set; }
        public string pagename { get; set; }
        public string actionname { get; set; }
        public string filetype { get; set; }//excel,pdf

        [NotMapped] public afterTripModel after_trip { get; set; } = new afterTripModel();
    }

    public class ExportFileOutModel
    {
        public string token_login { get; set; }
        public string doc_id { get; set; }
        public string emp_id { get; set; }

        public string path { get; set; }
        public string filename { get; set; }
        public string pagename { get; set; }
        public string actionname { get; set; }
        public string filetype { get; set; }//excel,pdf

        public afterTripModel after_trip { get; set; } = new afterTripModel();
    }
    public class ExportISOSModel
    {
        public string token_login { get; set; }
        public string doc_id { get; set; }
        public string emp_id { get; set; }

        public string path { get; set; }
        public string filename { get; set; }
        public string pagename { get; set; }
        public string actionname { get; set; }
        public string filetype { get; set; }//excel,pdf

        [NotMapped] public afterTripModel after_trip { get; set; } = new afterTripModel();
    }
    public class ExportRecordModel
    {
        public string token_login { get; set; }
        public string year { get; set; }

        [NotMapped] public afterTripModel after_trip { get; set; } = new afterTripModel();
    }

    public class Report_AllowanceModel
    {
        public string token_login { get; set; }
        public string doc_id { get; set; }
        public string emp_id { get; set; }
        public string emp_name { get; set; }
        public string id { get; set; }

        public string title { get; set; }
        public string country { get; set; }
        public string functional { get; set; }
        public string business_date { get; set; }
        public string departure_date { get; set; }
        public string arrival_date { get; set; }
        public string io_number { get; set; }
        public string cost_center { get; set; }
        public string gl_account { get; set; }

        public string employee_id { get; set; }
        public string employee_name { get; set; }
        public string total { get; set; }
        public string unit { get; set; }
        public string total_thb { get; set; }
        public string last_update { get; set; }

        public string icon_travel_agent { get; set; }
        public string icon_other { get; set; }
        public string passport { get; set; }
        public string passport_date { get; set; }
        public string luggage_clothing { get; set; }
        public string luggage_clothing_date { get; set; }
        public string remark { get; set; }
        public string important_note { get; set; }


        public List<ExchangeRateList> m_exchangerate { get; set; } = new List<ExchangeRateList>();
        public List<ExchangeRateList> m_exchangerate_max { get; set; } = new List<ExchangeRateList>();

        public List<dailyallowanceModel> dailyallowance { get; set; } = new List<dailyallowanceModel>();
        public List<flightscheduleModel> flightschedule { get; set; } = new List<flightscheduleModel>();

        public afterTripModel after_trip { get; set; } = new afterTripModel();

    }
    public class dailyallowanceModel
    {
        public string doc_id { get; set; }
        public string emp_id { get; set; }
        public string emp_name { get; set; }
        public string id { get; set; }

        public string allowance_days { get; set; }
        public string allowance_date { get; set; }
        public string allowance_low { get; set; }
        public string allowance_mid { get; set; }
        public string allowance_hight { get; set; }
        public string allowance_total { get; set; }
        public string allowance_unit { get; set; }

    }
    public class flightscheduleModel
    {
        public string doc_id { get; set; }
        public string emp_id { get; set; }
        public string emp_name { get; set; }
        public string id { get; set; }

        public string airticket_date { get; set; }
        public string airticket_route_from { get; set; }
        public string airticket_route_to { get; set; }
        public string airticket_flight { get; set; }
        public string airticket_departure_time { get; set; }
        public string airticket_arrival_time { get; set; }
    }



    public class ReportISOSRecordOutModel
    {
        public string token_login { get; set; }
        public string year { get; set; }

        public List<reportisosList> details_list { get; set; } = new List<reportisosList>();

        [NotMapped]
        public afterTripModel after_trip { get; set; } = new afterTripModel();
    }
    public class ReportInsuranceRecordOutModel
    {
        public string token_login { get; set; }
        public string year { get; set; }

        public List<insuranceModel> details_list { get; set; } = new List<insuranceModel>();


        [NotMapped]
        public afterTripModel after_trip { get; set; } = new afterTripModel();
    }
    public class reportisosList
    {
        public string no { get; set; }
        public string type_of_travel { get; set; }
        public string emp_id { get; set; }
        public string emp_title { get; set; }
        public string emp_name { get; set; }
        public string emp_surname { get; set; }
        public string emp_section { get; set; }
        public string emp_department { get; set; }
        public string emp_function { get; set; }
        public string emp_display { get; set; }

    }
    public class insuranceModel
    {
        public string id { get; set; }
        public string doc_id { get; set; }

        public string emp_id { get; set; }
        public string emp_display { get; set; }

        public string emp_passport { get; set; }

        public string emp_section { get; set; }
        public string emp_department { get; set; }
        public string emp_function { get; set; }

        public string name_beneficiary { get; set; }
        public string relationship { get; set; }

        public string certificates_no { get; set; }
        public string period_ins_from { get; set; }
        public string period_ins_to { get; set; }
        public string duration { get; set; }

        public string country { get; set; }
        public string billing_charge { get; set; }
        public string certificates_total { get; set; }
    }

    public class ExportReportService
    {
        cls_connection conn;
        string sqlstr = "";
        string ret = "";
        DataTable dt;

        public ExportFileOutModel exportfile_data(ExportFileInModel value)
        {
            //var data = value;
            DataTable dtdef = new DataTable();
            //HttpResponse response = HttpContext.Current.Response;


            var datetime_run = DateTime.Now.ToString("yyyyMMddHHmm");
            string _Folder = "/ExportFile/" + value.doc_id + "/" + value.pagename + "/" + value.emp_id + "/";

            //string _PathSave = System.Web.HttpContext.Current.Server.MapPath("~" + _Folder);
            string _PathFileSave = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "ExportFile", value.doc_id, value.pagename, value.emp_id);
            string _FileName = "";
            string ret = "";

            //http://TBKC-DAPPS-05.thaioil.localnet/ebiz_ws/Image/D001/travelerhistory/TO102155//Image/D001/travelerhistory/TO102155/
            //"http://TBKC-DAPPS-05.thaioil.localnet/ebiz_ws"
            string _Server_path = configApp.GetStringFromAppSettings("ServerPath_Service") ?? ""; // System.Configuration.ConfigurationManager.AppSettings["ServerPath_Service"].ToString();

            #region Determine whether the directory exists. 
            string msg_error = "";
            try
            {
                #region Export Excel 
                if (value.pagename.ToString() == "allowance")
                {
                    _FileName = "Allowance Payment Form " + value.doc_id + "_" + datetime_run + ".xlsx";
                    //_PathFileSave += _FileName;
                    //export_excel_allowance(value, _PathFileSave, ref msg_error);
                }
                #endregion Export Excel 
            }
            catch (Exception ex) { msg_error = "create folder " + ex.Message.ToString(); }

            #endregion Determine whether the directory exists.

            //next_line_1:;

            var data = new ExportFileOutModel();
            data.path = _Server_path + _Folder;
            data.filename = _FileName;

            data.after_trip.opt1 = (ret ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
            data.after_trip.opt2.status = (ret ?? "") == "true" ? "Upload file succesed." : "Export file failed.";
            data.after_trip.opt2.remark = (ret ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
            data.after_trip.opt3.status = "";
            data.after_trip.opt3.remark = _PathFileSave;

            return data;
        }
        public string export_excel_allowance(ExportFileInModel value, string name, ref string msg_error)
        {
            string ret = "";
            string token_login = value.token_login;
            string doc_id = value.doc_id;
            string emp_id = value.emp_id;
            Boolean user_admin = false;

            #region get data
            SearchDocService wssearch = new SearchDocService();
            DataTable dtref = new DataTable();
            DataRow[] drref;
            Double dLuggage_Clothing = 0.00;
            string luggage_clothing_date = "";
            Double dPassport = 0.00;
            string passport_date = "";
            string remark = "";

            //ยังไม่มีข้อมูลที่จะนำมาแสดง ต้องถาม user ???
            string icon_travel_agent = "";
            string icon_other = "";
            string important_note = "";


            //ใช้วิธีดึงข้อมูลตรงจาก db ใส่ใน function เนื่องจากอาจจะมีการเปลี่ยนเปลงบ่อย
            DataSet ds = new DataSet();
            ds = refdata_excel_allowance(value);

            #endregion get data  

            try
            {

                ExcelPackage ExcelPkg = new ExcelPackage();
                ExcelWorksheet wsSheet1 = ExcelPkg.Workbook.Worksheets.Add("Sheet1");
                int i = 0;
                int irows = 1;
                int isheet = 1;

                #region sheet 1
                if (true)
                {
                    DataTable dtMain = ds.Tables["allowance header"].Copy();
                    for (isheet = 0; isheet < dtMain.Rows.Count; isheet++)
                    {
                        ExcelPkg = new ExcelPackage();
                        wsSheet1 = ExcelPkg.Workbook.Worksheets.Add("Sheet" + isheet);
                        wsSheet1.Name = "Allowance Form";
                        int icell_start = 2;
                        int icell_end = 8;

                        i = 0; irows = 1;

                        #region ข้อมูล allowance header 
                        string total_thb = dtMain.Rows[i]["total_thb"].ToString();
                        Double dTotalAllowance = 0.00;
                        try
                        {
                            dTotalAllowance = Convert.ToDouble(total_thb);
                        }
                        catch { }
                        irows += 1;
                        //wsSheet1.Cells[irows, 0].Merge = true;
                        wsSheet1.Cells[irows, icell_start].Value = "BUSINESS TRIP ALLOWANCE FORM";
                        wsSheet1.Cells[irows, icell_start].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        irows += 1;
                        wsSheet1.Cells[irows, icell_end].Value = "Update as of " + dtMain.Rows[i]["last_update"].ToString(); ;//Update as of 6 March 2019
                        wsSheet1.Cells[irows, icell_end].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        irows += 1;
                        wsSheet1.Cells[irows, icell_start].Value = "Title";
                        wsSheet1.Cells[irows, icell_start + 1].Value = dtMain.Rows[i]["title"].ToString();

                        wsSheet1.Cells[irows, 6].Value = "TOTAL";
                        wsSheet1.Cells[irows, 7].Value = dtMain.Rows[i]["total"].ToString();
                        wsSheet1.Cells[irows, 8].Value = dtMain.Rows[i]["unit"].ToString();

                        irows += 1;
                        wsSheet1.Cells[irows, icell_start].Value = "Name";
                        wsSheet1.Cells[irows, icell_start + 1].Value = dtMain.Rows[i]["employee_id"].ToString();
                        //wsSheet1.Cells[irows, 0].Merge = true;
                        wsSheet1.Cells[irows, icell_start + 2].Value = dtMain.Rows[i]["employee_name"].ToString();

                        wsSheet1.Cells[irows, 6].Value = "TOTAL";
                        wsSheet1.Cells[irows, 7].Value = total_thb;
                        wsSheet1.Cells[irows, 8].Value = "THB";

                        irows += 1;
                        wsSheet1.Cells[irows, icell_start].Value = "Country";
                        wsSheet1.Cells[irows, icell_start + 1].Value = dtMain.Rows[i]["country"].ToString();
                        wsSheet1.Cells[irows, icell_start + 2].Value = "Sect./Dept.";
                        wsSheet1.Cells[irows, icell_start + 3].Value = dtMain.Rows[i]["functional"].ToString();

                        irows += 1;
                        wsSheet1.Cells[irows, icell_start].Value = "Business Date";
                        wsSheet1.Cells[irows, icell_start + 1].Value = dtMain.Rows[i]["business_date"].ToString();
                        wsSheet1.Cells[irows, icell_start + 2].Value = "Departure Date";
                        wsSheet1.Cells[irows, icell_start + 3].Value = dtMain.Rows[i]["departure_date"].ToString();
                        wsSheet1.Cells[irows, icell_start + 4].Value = "Arrival Date";
                        wsSheet1.Cells[irows, icell_start + 5].Value = dtMain.Rows[i]["arrival_date"].ToString();

                        irows += 1;
                        wsSheet1.Cells[irows, icell_start].Value = "I/O Number";
                        wsSheet1.Cells[irows, icell_start + 1].Value = dtMain.Rows[i]["io_number"].ToString();
                        wsSheet1.Cells[irows, icell_start + 2].Value = "Cost Xenter";
                        wsSheet1.Cells[irows, icell_start + 3].Value = dtMain.Rows[i]["cost_center"].ToString();
                        wsSheet1.Cells[irows, icell_start + 4].Value = "GL Account ";
                        wsSheet1.Cells[irows, icell_start + 5].Value = dtMain.Rows[i]["gl_account"].ToString();

                        #endregion ข้อมูล allowance header 


                        #region ข้อมูล table daily allowance 
                        irows = 14;
                        dt = new DataTable();
                        dt = ds.Tables["daily allowance"].Copy();
                        //ต้องเพิ่ม กรอง emp_id ???


                        irows += 0;
                        wsSheet1.Cells[irows, icell_start].Value = "Dairy allowance";
                        wsSheet1.Cells[irows, icell_start].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        irows += 1;
                        wsSheet1.Cells[irows, icell_start, irows, icell_end].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        wsSheet1.Cells[irows, icell_start, irows, icell_end].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wsSheet1.Cells[irows, icell_start].Value = "Days";
                        wsSheet1.Cells[irows, icell_start + 1].Value = "Date";
                        wsSheet1.Cells[irows, icell_start + 2].Value = "< 6 hr";
                        wsSheet1.Cells[irows, icell_start + 3].Value = "6-12 hr";
                        wsSheet1.Cells[irows, icell_start + 4].Value = "> 12 hr";
                        wsSheet1.Cells[irows, icell_start + 5, irows, icell_start + 6].Merge = true;
                        wsSheet1.Cells[irows, icell_start + 5].Value = "TOTAL";

                        for (i = 0; i < dt.Rows.Count; i++)
                        {
                            irows += 1;
                            wsSheet1.Cells[irows, icell_start, irows, icell_start + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            wsSheet1.Cells[irows, icell_start].Value = dt.Rows[i]["allowance_days"].ToString();
                            wsSheet1.Cells[irows, icell_start + 1].Value = dt.Rows[i]["allowance_date"].ToString();

                            wsSheet1.Cells[irows, icell_start, irows + 2, icell_start + 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            wsSheet1.Cells[irows, icell_start + 2].Value = dt.Rows[i]["allowance_low"].ToString();
                            wsSheet1.Cells[irows, icell_start + 3].Value = dt.Rows[i]["allowance_mid"].ToString();
                            wsSheet1.Cells[irows, icell_start + 4].Value = dt.Rows[i]["allowance_hight"].ToString();
                            wsSheet1.Cells[irows, icell_start + 5].Value = dt.Rows[i]["allowance_total"].ToString();

                            wsSheet1.Cells[irows, icell_start + 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            wsSheet1.Cells[irows, icell_start + 6].Value = dt.Rows[i]["allowance_unit"].ToString();
                        }

                        irows += 1;
                        wsSheet1.Cells[irows, icell_start, irows, icell_end - 2].Merge = true;
                        wsSheet1.Cells[irows, icell_start].Value = "TOTAL ALLOWANCE";
                        wsSheet1.Cells[irows, icell_start].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        wsSheet1.Cells[irows, icell_end - 1, irows, icell_end].Merge = true;
                        wsSheet1.Cells[irows, icell_end - 1].Value = "$" + dTotalAllowance.ToString("#,000.00");
                        wsSheet1.Cells[irows, icell_end - 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        #endregion ข้อมูล table daily allowance 


                        #region ข้อมูล table flight schedule
                        irows += 1;
                        dt = new DataTable();
                        dt = ds.Tables["flight schedule"].Copy();

                        irows += 0;
                        wsSheet1.Cells[irows, icell_start].Value = "FLIGHT SCHEDULE";

                        irows += 1;
                        wsSheet1.Cells[irows, icell_start, irows, icell_end].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        wsSheet1.Cells[irows, icell_start].Value = "Date";
                        wsSheet1.Cells[irows, icell_start + 1].Value = "From";
                        wsSheet1.Cells[irows, icell_start + 2].Value = "To";
                        wsSheet1.Cells[irows, icell_start + 3].Value = "Flight";
                        wsSheet1.Cells[irows, icell_start + 4].Value = "Depature Time";
                        wsSheet1.Cells[irows, icell_start + 5].Value = "Arrival Time";

                        irows += 1;
                        for (i = 0; i < dt.Rows.Count; i++)
                        {
                            string check_over_day = "";
                            try
                            {
                                if (Convert.ToDouble(dt.Rows[i]["airticket_arrival_time"].ToString().Replace(":", ".")) <
                                    Convert.ToDouble(dt.Rows[i]["airticket_departure_time"].ToString().Replace(":", "."))
                                    )
                                {
                                    check_over_day = dt.Rows[i]["airticket_date_next"].ToString();
                                    if (check_over_day != "")
                                    {
                                        check_over_day += "(" + check_over_day + ")";
                                    }
                                }
                            }
                            catch { }

                            irows += 1;
                            wsSheet1.Cells[irows, icell_start, irows, icell_end].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            wsSheet1.Cells[irows, icell_start].Value = dt.Rows[i]["airticket_date"].ToString();
                            wsSheet1.Cells[irows, icell_start + 1].Value = dt.Rows[i]["airticket_route_from"].ToString();
                            wsSheet1.Cells[irows, icell_start + 2].Value = dt.Rows[i]["airticket_route_to"].ToString();
                            wsSheet1.Cells[irows, icell_start + 3].Value = dt.Rows[i]["airticket_flight"].ToString();
                            wsSheet1.Cells[irows, icell_start + 4].Value = dt.Rows[i]["airticket_departure_time"].ToString();//เอาแค่ time
                            wsSheet1.Cells[irows, icell_start + 5].Value = dt.Rows[i]["airticket_arrival_time"].ToString() + check_over_day; //เอาแค่ time 
                        }

                        #endregion ข้อมูล table flight schedule

                        #region ข้อมูล record of outfit allowances
                        irows += 1;
                        wsSheet1.Cells[irows, icell_start, irows, icell_end].Merge = true;
                        wsSheet1.Cells[irows, icell_start].Value = "Accommodation";
                        wsSheet1.Cells[irows, icell_start].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        irows += 1;
                        wsSheet1.Cells[irows, icell_start, irows, icell_start + 3].Merge = true;
                        wsSheet1.Cells[irows, icell_start].Value = icon_travel_agent + " Booking by Travel Agent";

                        wsSheet1.Cells[irows, icell_start + 4, irows, icell_end].Merge = true;
                        wsSheet1.Cells[irows, icell_start + 4].Value = icon_other + " Booking by others and reclaim after travelling";

                        irows += 1;
                        wsSheet1.Cells[irows, icell_start].Value = "RECORD OF OUTFIT ALLOWANCES";

                        irows += 1;
                        wsSheet1.Cells[irows, icell_start].Value = "LUGGAGE & CLOTHING (THB)";
                        wsSheet1.Cells[irows, icell_start + 2, irows, icell_start + 3].Merge = true;
                        wsSheet1.Cells[irows, icell_start + 2].Value = dLuggage_Clothing.ToString("#,##0.00");
                        wsSheet1.Cells[irows, icell_start + 4].Value = "Valid Date";
                        wsSheet1.Cells[irows, icell_start + 5, irows, icell_start + 6].Merge = true;
                        wsSheet1.Cells[irows, icell_start + 5].Value = luggage_clothing_date;

                        irows += 1;
                        wsSheet1.Cells[irows, icell_start].Value = "PASSPORT (THB)";
                        wsSheet1.Cells[irows, icell_start + 2, irows, icell_start + 3].Merge = true;
                        wsSheet1.Cells[irows, icell_start + 2].Value = dPassport.ToString("#,##0.00");
                        wsSheet1.Cells[irows, icell_start + 4].Value = "Valid Date";
                        wsSheet1.Cells[irows, icell_start + 5, irows, icell_start + 6].Merge = true;
                        wsSheet1.Cells[irows, icell_start + 5].Value = passport_date;

                        irows += 1;
                        wsSheet1.Cells[irows, icell_start].Value = "Remark : " + remark;

                        irows += 2;
                        wsSheet1.Cells[irows, icell_start].Value = "Important Note";
                        wsSheet1.Cells[irows, icell_start + 1].Value = important_note;

                        #endregion ข้อมูล record of outfit allowances
                    }

                    try
                    {
                        wsSheet1.Protection.IsProtected = false;
                        wsSheet1.Protection.AllowSelectLockedCells = false;
                        var path = Path.Combine(name);
                        ExcelPkg.SaveAs(new FileInfo(path));
                    }
                    catch (Exception ex_savefile) { ret = "save excel " + ex_savefile.Message.ToString(); }
                }

                #endregion sheet 1

            }
            catch (Exception ex) { ret = "open excel " + ex.Message.ToString(); }

            msg_error = ret;

            return ret;

        }

        public string import_excel_kh_code(string fullpath, string token_login, string emp_user_active)
        {
            DataTable dtcol = new DataTable();
            dtcol.Columns.Add("emp_id");
            dtcol.Columns.Add("oversea_code");
            dtcol.Columns.Add("local_code");
            dtcol.AcceptChanges();

            var imsg_rows = 1;
            string ret = "";
            try
            {
                sw_WriteLine("name11", " path to your excel file ");

                //// path to your excel file 
                fullpath = @"D:\Ebiz2\EBiz_Webservice\DocumentFile\khcode\KH_QR_CODE.xlsx";
                imsg_rows = 2;
                ExcelPackage ExcelPkg = new ExcelPackage();
                imsg_rows = 3;
                ExcelWorksheet wsSheet1 = ExcelPkg.Workbook.Worksheets.Add("Sheet1");
                imsg_rows = 4;

                FileInfo fileInfo = new FileInfo(fullpath);
                imsg_rows = 5;
                ExcelPackage package = new ExcelPackage(fileInfo, true);
                imsg_rows = 6;
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                imsg_rows = 7;
                // get number of rows and columns in the sheet
                int rows = worksheet.Dimension.Rows; // 20
                int columns = worksheet.Dimension.Columns; // 7
                int irows = 0;
                imsg_rows = 8;

                sw_WriteLine("name12", "loop through the worksheet rows " + rows);


                //// loop through the worksheet rows and columns
                //for (int i = 1; i <= rows; i++)
                //{
                //    sw_WriteLine("excel rows s"+ i.ToString(), "rows:" + i);
                //    //column 1: emp --> 913 ต้องเพิ่ม 000000000 ให้ครบสิบหลัก
                //    //column 9: Overseas --> OA2
                //    //column 11: Local Allo --> LA2 
                //    string emp_id = worksheet.Cells[i, 1].Value.ToString();
                //    string oversea_code = worksheet.Cells[i, 9].Value.ToString();
                //    string local_code = worksheet.Cells[i, 11].Value.ToString();

                //    dtcol.Rows.Add(dtcol.NewRow());
                //    dtcol.Rows[irows]["emp_id"] = emp_id;
                //    dtcol.Rows[irows]["oversea_code"] = oversea_code;
                //    dtcol.Rows[irows]["local_code"] = local_code;
                //    dtcol.AcceptChanges();
                //    sw_WriteLine("excel rows e" + i.ToString(), emp_id);
                //}



            }
            catch (Exception ex)
            {
                ret = "rows error: " + imsg_rows + " fullpath: " + fullpath + " --> open excel " + ex.Message.ToString();
            }

            sw_WriteLine("name10", ret.ToString());
            //try
            //{
            //    SetDocService wss = new SetDocService();
            //    Boolean bNewData = false;
            //    int imaxid = wss.GetMaxID("BZ_DATA_KH_CODE");
            //    var sqlstr_all = "";

            //    for (int i = 0; i < dtcol.Rows.Count; i++)
            //    {
            //        string user_id = "";
            //        string emp_id = dtcol.Rows[i]["emp_id"].ToString();
            //        string oversea_code = dtcol.Rows[i]["oversea_code"].ToString();
            //        string local_code = dtcol.Rows[i]["local_code"].ToString();
            //        if (emp_id.ToString() == "") { continue; }

            //        if (bNewData == false)
            //        {
            //            bNewData = true;
            //            sqlstr = "delete from BZ_DATA_KH_CODE ";
            //            ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
            //            sqlstr_all += sqlstr + "||";

            //            if (ret.ToLower() != "true") { goto Next_line_1; }
            //        }

            //        sqlstr = @" insert into BZ_DATA_KH_CODE
            //                        (ID,EMP_ID,USER_ID,OVERSEA_CODE,LOCAL_CODE,STATUS
            //                        ,DATA_FOR_SAP
            //                        ,CREATE_BY,CREATE_DATE,TOKEN_UPDATE) values ( ";

            //        sqlstr += @" " + imaxid;
            //        sqlstr += @" ," + conn.ChkSqlStr(emp_id, 300);

            //        sqlstr += @" ," + conn.ChkSqlStr(user_id, 300);
            //        sqlstr += @" ," + conn.ChkSqlStr(oversea_code, 300);
            //        sqlstr += @" ," + conn.ChkSqlStr(local_code, 300);
            //        sqlstr += @" ," + conn.ChkSqlStr("1", 300);

            //        sqlstr += @" ," + conn.ChkSqlStr("0", 300); //sap = 1, ebiz = 0

            //        sqlstr += @" ," + conn.ChkSqlStr(emp_user_active, 300);//user name login
            //        sqlstr += @" ,sysdate";
            //        sqlstr += @" ," + conn.ChkSqlStr(token_login, 300);
            //        sqlstr += @" )";

            //        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
            //        sqlstr_all += sqlstr + "||";

            //        if (ret.ToLower() != "true") { goto Next_line_1; }
            //    }

            //Next_line_1:;

            //    if (ret.ToLower() == "true")
            //    {
            //        sqlstr = "update BZ_DATA_KH_CODE a set emp_id =  substr('00000000' || a.emp_id,length('00000000' || a.emp_id)-7) ";
            //        ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
            //        sqlstr_all += sqlstr + "||";

            //        ret = SetDocService.conn_ExecuteNonQuery(sqlstr_all, false);
            //    }
            //}
            //catch (Exception ex) { ret = "import excel to table " + ex.Message.ToString() + " จำนวนข้อมูล : " + dtcol.Rows.Count; }

            //sw_WriteLine("e1", ret);

            return ret;

        }
        private void sw_WriteLine(string name_x, string msg_ref)
        {
            string pathw = @"D:\Ebiz2\EBiz_Webservice\DocumentFile\khcode\MyTest" + name_x + ".txt";
            if (!File.Exists(pathw))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(pathw))
                {
                    sw.WriteLine(msg_ref);

                }
            }
        }

        public Report_AllowanceModel repoprt_data_allowance(ExportFileInModel value)
        {
            string msg_error = "";
            Report_AllowanceModel data = new Report_AllowanceModel();
            try
            {
                SearchDocService wssearch = new SearchDocService();

                DataSet ds = refdata_excel_allowance(value);
                //add data set to object

                int ifrom = 0;
                int sto = 0;
                DataTable dtMain = ds.Tables["allowance header"].Copy();
                for (int i = 0; i < dtMain.Rows.Count; i++)
                {
                    var doc_id = dtMain.Rows[i]["doc_id"].ToString();
                    var emp_id = dtMain.Rows[i]["employee_id"].ToString();
                    var emp_name = dtMain.Rows[i]["employee_name"].ToString();

                    //20211027 เพิ่มดึงข้อมูล passport ใหม่ 
                    string passport = dtMain.Rows[i]["passport"].ToString();
                    string passport_date = dtMain.Rows[i]["passport_date"].ToString();
                    try
                    {
                        SearchDocService _swd = new SearchDocService();
                        var est = _swd.EstimateExpense(doc_id, emp_id);
                        if (est.PassportExpense.ToString() != "")
                        {
                            passport = est.PassportExpense.ToString();
                        }
                        if (est.PassportDate.ToString() != "")
                        {
                            passport_date = _swd.convert_date_display(est.PassportDate.ToString());
                        }
                    }
                    catch { }

                    #region ข้อมูล allowance header  
                    data.doc_id = doc_id;
                    data.emp_id = emp_id;
                    data.emp_name = emp_name;

                    data.title = dtMain.Rows[i]["title"].ToString();
                    data.country = dtMain.Rows[i]["country"].ToString();
                    data.functional = dtMain.Rows[i]["functional"].ToString();
                    data.business_date = dtMain.Rows[i]["business_date"].ToString();
                    data.departure_date = dtMain.Rows[i]["departure_date"].ToString();
                    data.arrival_date = dtMain.Rows[i]["arrival_date"].ToString();
                    data.io_number = dtMain.Rows[i]["io_number"].ToString();
                    data.cost_center = dtMain.Rows[i]["cost_center"].ToString();
                    data.gl_account = dtMain.Rows[i]["gl_account"].ToString();

                    data.employee_id = dtMain.Rows[i]["employee_id"].ToString();
                    data.employee_name = dtMain.Rows[i]["employee_name"].ToString();
                    data.total = dtMain.Rows[i]["total"].ToString();
                    data.unit = dtMain.Rows[i]["unit"].ToString();
                    data.total_thb = dtMain.Rows[i]["total_thb"].ToString();
                    data.last_update = dtMain.Rows[i]["last_update"].ToString();

                    #region ข้อมูล record of outfit allowances
                    data.icon_travel_agent = dtMain.Rows[i]["icon_travel_agent"].ToString();
                    data.icon_other = dtMain.Rows[i]["icon_other"].ToString();

                    data.passport = passport;
                    data.passport_date = passport_date;
                    data.luggage_clothing = dtMain.Rows[i]["luggage_clothing"].ToString();
                    data.luggage_clothing_date = dtMain.Rows[i]["luggage_clothing_date"].ToString();

                    data.remark = dtMain.Rows[i]["remark"].ToString();
                    data.important_note = dtMain.Rows[i]["important_note"].ToString();
                    #endregion ข้อมูล record of outfit allowances

                    #endregion ข้อมูล allowance header 

                    DataRow[] dr;
                    #region ข้อมูล table daily allowance 
                    dt = new DataTable();
                    dt = ds.Tables["daily allowance"].Copy();
                    dr = dt.Select("emp_id ='" + emp_id + "' ");
                    for (int j = 0; j < dr.Length; j++)
                    {
                        data.dailyallowance.Add(new dailyallowanceModel
                        {
                            doc_id = doc_id,
                            emp_id = emp_id,
                            emp_name = emp_name,

                            id = (j + 1).ToString(),
                            allowance_days = dr[j]["allowance_days"].ToString(),
                            allowance_date = dr[j]["allowance_date"].ToString(),
                            allowance_low = dr[j]["allowance_low"].ToString(),
                            allowance_mid = dr[j]["allowance_mid"].ToString(),
                            allowance_hight = dr[j]["allowance_hight"].ToString(),
                            allowance_total = dr[j]["allowance_total"].ToString(),
                            allowance_unit = dr[j]["allowance_unit"].ToString(),
                        });
                    }
                    #endregion ข้อมูล table daily allowance 


                    #region ข้อมูล table flight schedule 
                    dt = new DataTable();
                    dt = ds.Tables["flight schedule"].Copy();
                    dr = dt.Select("emp_id ='" + emp_id + "' ");
                    for (int j = 0; j < dr.Length; j++)
                    {
                        string check_over_day = "";
                        try
                        {
                            if (Convert.ToDouble(dr[j]["airticket_arrival_time"].ToString().Replace(":", ".")) <
                                Convert.ToDouble(dr[j]["airticket_departure_time"].ToString().Replace(":", "."))
                                )
                            {
                                check_over_day = " (" + dr[j]["airticket_date_next"].ToString() + ")";
                            }
                        }
                        catch { }

                        data.flightschedule.Add(new flightscheduleModel
                        {
                            doc_id = doc_id,
                            emp_id = emp_id,
                            emp_name = emp_name,

                            id = (j + 1).ToString(),
                            airticket_date = dr[j]["airticket_date"].ToString(),
                            airticket_route_from = dr[j]["airticket_route_from"].ToString(),
                            airticket_route_to = dr[j]["airticket_route_to"].ToString(),
                            airticket_flight = dr[j]["airticket_flight"].ToString(),
                            airticket_departure_time = dr[j]["airticket_departure_time"].ToString(),
                            airticket_arrival_time = dr[j]["airticket_arrival_time"].ToString() + check_over_day,
                        });
                    }
                    #endregion ข้อมูล table flight schedule

                }

                DataTable dtm_exchangerate = wssearch.ref_exchangerate();
                if (dtm_exchangerate.Rows.Count > 0)
                {
                    dt = new DataTable();
                    dt = dtm_exchangerate.Copy(); dt.AcceptChanges();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        data.m_exchangerate_max.Add(new ExchangeRateList
                        {
                            id = dt.Rows[i]["id"].ToString(),
                            currency_id = dt.Rows[i]["currency_id"].ToString(),
                            exchange_rate = dt.Rows[i]["exchange_rate"].ToString(),
                            date_from = dt.Rows[i]["date_from"].ToString(),
                            date_to = dt.Rows[i]["date_to"].ToString(),
                        });
                    }
                }
                dtm_exchangerate = new DataTable();
                dtm_exchangerate = wssearch.ref_exchangerate_by_doc(value.doc_id);
                if (dtm_exchangerate.Rows.Count > 0)
                {
                    dt = new DataTable();
                    dt = dtm_exchangerate.Copy(); dt.AcceptChanges();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        data.m_exchangerate.Add(new ExchangeRateList
                        {
                            id = dt.Rows[i]["id"].ToString(),
                            currency_id = dt.Rows[i]["currency_id"].ToString(),
                            exchange_rate = dt.Rows[i]["exchange_rate"].ToString(),
                            date_from = dt.Rows[i]["date_from"].ToString(),
                            date_to = dt.Rows[i]["date_to"].ToString(),
                        });
                    }
                }
                ret = "true";
            }
            catch (Exception ex) { msg_error = ex.Message.ToString(); data.token_login = msg_error; }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Export report succesed." : "Export report failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;

            return data;
        }
        public DataSet refdata_excel_allowance(ExportFileInModel value)
        {
            string ret = "";
            string token_login = value.token_login;
            string doc_id = value.doc_id;
            string emp_id = value.emp_id;
            Boolean user_admin = false;

            #region get data
            SearchDocService wssearch = new SearchDocService();
            DataTable dtref = new DataTable();
            DataRow[] drref;

            Double dLuggage_Clothing = 0.00;
            string luggage_clothing_date = "";
            Double dPassport = 0.00;
            string passport_date = "";
            string remark = "";

            //ยังไม่มีข้อมูลที่จะนำมาแสดง ต้องถาม user ???
            string icon_travel_agent = "";
            string icon_other = "";
            string important_note = "";

            string sbz_doc_traveler_expense = @" 
                             select tb1.dh_code as doc_id, tb1.dte_emp_id as emp_id
                             , sum(to_char(tb1.dte_cl_expense)) as luggage_clothing
                             , sum(to_char(tb1.dte_cl_expense)) as dte_cl_expense
                             , to_char(tb2.dte_cl_valid-1,'dd Mon rrrr') as luggage_clothing_date
                             , tb2.dte_passport_expense as passport
                             , to_char(tb2.dte_passport_valid-1,'dd Mon rrrr') as passport_date
                             from bz_doc_traveler_expense tb1
                             left join (select ex.dh_code, ex.dte_emp_id, ex.dte_cl_valid, ex.dte_passport_expense, ex.dte_passport_valid 
                             from bz_doc_traveler_expense ex
                             where ex.dh_code ='" + doc_id + "' and ex.dte_emp_id ='" + emp_id + "' " +
                          @" and rownum =1) tb2 on tb1.dh_code = tb2.dh_code and tb1.dte_emp_id = tb2.dte_emp_id
                             where tb1.dh_code ='" + doc_id + "' and tb1.dte_emp_id ='" + emp_id + "'  " +
                          @" group by tb1.dh_code, tb1.dte_emp_id, tb2.dh_code, tb2.dte_emp_id, tb2.dte_cl_valid, tb2.dte_passport_expense, tb2.dte_passport_valid ";

            //ใช้วิธีดึงข้อมูลตรงจาก db ใส่ใน function เนื่องจากอาจจะมีการเปลี่ยนเปลงบ่อย
            DataSet ds = new DataSet();

            //ข้อมูล allowance header  
            sqlstr = @"  select distinct null as title,null as country,null as business_date,null as departure_date,null as arrival_date,null as io_number,null as cost_center,null as gl_account
                    ,a.doc_id,a.emp_id as employee_id,null as employee_name,null as functional 
                    ,b.total,b.total_thb,'USD' as unit
                    ,nvl(a.update_date,a.create_date) as last_update
                    ,a.remark 
                    , case when a.luggage_clothing  is null then  to_char(ex.dte_cl_expense) else to_char(a.luggage_clothing)  end luggage_clothing 
                    ,ex.luggage_clothing_date as luggage_clothing_date
                    ,p.passport_no
                    ,nvl(ex.passport,0) as passport
                    ,nvl(ex.passport,0) as passport_thb
                    ,ex.passport_date as passport_date

                    ,a.emp_id
                    ,null as icon_travel_agent,null as icon_other,null as important_note 

                    from  bz_doc_allowance a
                    inner join (
                    select doc_id,emp_id,nvl(sum(allowance_total),0) as total,nvl(sum(allowance_total * nvl(allowance_exchange_rate,1)),0)  as total_thb
                    from bz_doc_allowance_detail group by doc_id,emp_id 
                    ) b on a.doc_id = b.doc_id and a.emp_id = b.emp_id 
                    left join bz_data_passport p on a.emp_id = p.emp_id and p.default_type ='true'
                    left join ( " + sbz_doc_traveler_expense + " ) ex on b.emp_id = ex.emp_id and a.doc_id = ex.doc_id  where a.doc_id ='" + doc_id + "' and b.emp_id ='" + emp_id + "' ";

            //ข้อมูลที่ออกมาตั้งมีแค่ rows เดียวก่อน ยังไม่มี กรณีที่ admin export all
            conn = new cls_connection();
            if (SetDocService.conn_ExecuteData(ref dt, sqlstr) == "")
            {
                wssearch = new SearchDocService();
                dtref = new DataTable();
                dtref = wssearch.refdata_emp_detail(token_login, doc_id, emp_id, ref user_admin);

                wssearch = new SearchDocService();
                DataTable dtref2 = new DataTable();
                dtref2 = wssearch.refdata_accom_book(token_login, doc_id, emp_id, user_admin);

                if (dt.Rows.Count > 0 && dtref.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var emp_id_select = dt.Rows[i]["emp_id"].ToString();
                        drref = dtref.Select("emp_id ='" + emp_id_select + "'");
                        if (drref.Length > 0)
                        {
                            for (int k = 0; k < drref.Length; k++)
                            {
                                try
                                {
                                    dPassport += Convert.ToDouble(dt.Rows[i]["passport_thb"].ToString());
                                }
                                catch { }
                                try
                                {
                                    dLuggage_Clothing += Convert.ToDouble(dt.Rows[i]["luggage_clothing"].ToString());
                                }
                                catch { }
                                passport_date = dt.Rows[i]["passport_date"].ToString();
                                luggage_clothing_date = dt.Rows[i]["luggage_clothing_date"].ToString();
                            }

                            dt.Rows[i]["title"] = drref[0]["travel_topic"].ToString();
                            dt.Rows[i]["country"] = drref[0]["country_name"].ToString();
                            dt.Rows[i]["business_date"] = drref[0]["business_date"].ToString();
                            dt.Rows[i]["departure_date"] = drref[0]["datefrom"].ToString();
                            dt.Rows[i]["arrival_date"] = drref[0]["dateto"].ToString();
                            dt.Rows[i]["io_number"] = drref[0]["io_wbs"].ToString();
                            dt.Rows[i]["cost_center"] = drref[0]["cost_center"].ToString();
                            dt.Rows[i]["gl_account"] = drref[0]["gl_account"].ToString();

                            dt.Rows[i]["employee_name"] = drref[0]["emp_name"].ToString();
                            dt.Rows[i]["functional"] = drref[0]["emp_organization"].ToString();

                            dt.Rows[i]["passport"] = dPassport.ToString();
                            dt.Rows[i]["luggage_clothing"] = dLuggage_Clothing.ToString();

                            //dt.Rows[i]["passport_date"] =   passport_date.ToString();
                            // dt.Rows[i]["luggage_clothing_date"] = luggage_clothing_date.ToString();

                            remark = dt.Rows[i]["remark"].ToString();
                        }

                        drref = dtref2.Select("emp_id ='" + emp_id_select + "'");
                        if (drref.Length > 0)
                        {
                            //ยังระบุ field ไม่ได้
                            if (drref[0]["booking"].ToString().ToLower() == "true") { icon_travel_agent = "X"; }
                            if (drref[0]["booking"].ToString().ToLower() == "true") { icon_other = "X"; }
                            dt.Rows[i]["icon_travel_agent"] = icon_travel_agent;
                            dt.Rows[i]["icon_other"] = icon_other;
                        }
                        dt.AcceptChanges();

                    }

                }

                dt.TableName = "allowance header";
                ds.Tables.Add(dt); ds.AcceptChanges();
            }


            //ข้อมูล table daily allowance 
            dt = new DataTable();
            dt = wssearch.refdata_allowance_detail(token_login, doc_id, "", user_admin);
            dt.TableName = "daily allowance";
            ds.Tables.Add(dt); ds.AcceptChanges();


            //ข้อมูล table flight schedule 
            dt = new DataTable();
            dt = wssearch.refdata_air_book_detail(token_login, doc_id, "", user_admin);
            dt.TableName = "flight schedule";
            ds.Tables.Add(dt); ds.AcceptChanges();

            #endregion get data  

            return ds;
        }

        public ReportISOSRecordOutModel report_isos_member_list_record(ExportRecordModel value)
        {
            string msg_error = "";
            ReportISOSRecordOutModel data = new ReportISOSRecordOutModel();
            try
            {
                string year = value.year;
                data.token_login = value.token_login;
                data.year = year;

                sqlstr = @" select a.*
                            from bz_doc_isos_record a
                            where a.year = '" + year + "' ";
                sqlstr += @" order by to_number(a.id) ";
                dt = new DataTable();
                conn = new cls_connection();
                if (SetDocService.conn_ExecuteData(ref dt, sqlstr) == "")
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        data.details_list.Add(new reportisosList
                        {
                            no = dt.Rows[i]["id"].ToString(),
                            type_of_travel = dt.Rows[i]["isos_type_of_travel"].ToString(),
                            emp_id = dt.Rows[i]["isos_emp_id"].ToString(),
                            emp_title = dt.Rows[i]["isos_emp_title"].ToString(),
                            emp_name = dt.Rows[i]["isos_emp_name"].ToString(),
                            emp_surname = dt.Rows[i]["isos_emp_surname"].ToString(),
                            emp_section = dt.Rows[i]["isos_emp_section"].ToString(),
                            emp_department = dt.Rows[i]["isos_emp_department"].ToString(),
                            emp_function = dt.Rows[i]["isos_emp_function"].ToString(),
                            emp_display = dt.Rows[i]["isos_emp_title"].ToString() + " " + dt.Rows[i]["isos_emp_name"].ToString() + " " + dt.Rows[i]["isos_emp_surname"].ToString(),
                        });
                    }
                }


                ret = "true";
            }
            catch (Exception ex) { msg_error = ex.Message.ToString(); data.token_login = msg_error; }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Export report succesed." : "Export report failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;

            return data;
        }
        public ReportInsuranceRecordOutModel report_insurance_list_record(ExportRecordModel value)
        {
            string msg_error = "";
            ReportInsuranceRecordOutModel data = new ReportInsuranceRecordOutModel();
            try
            {
                string year = value.year;
                data.token_login = value.token_login;
                data.year = year;

                sqlstr = @" select distinct '' as id
                            , h.dh_code as doc_id
                            , ex.dte_emp_id as emp_id   
                            , case when  a.ins_emp_id is not null then a.ins_emp_name else 
                                case when p.emp_id is null then b.userdisplay else ( p.passport_title || ' ' || p.passport_name || ' ' || p.passport_surname ) end 
                              end emp_display 
                            , case when a.ins_emp_id is not null then a.ins_emp_passport else p.passport_no end emp_passport 
                    
                            , b.sections as emp_section
                            , b.department as emp_department
                            , b.function as emp_function
                    
                            , a.name_beneficiary
                            , a.relationship
                    
                            , a.certificates_no
                            , a.period_ins_from  
                            , a.period_ins_to 
                            , case when a.duration is null then to_number(nvl((case when a.period_ins_to is not null and to_date(a.period_ins_from,'dd MON rrrr') is not null then  to_date(a.period_ins_to,'dd MON rrrr') - to_date(a.period_ins_from,'dd MON rrrr') end),0))
                              else  to_number(nvl(a.duration,0)) end duration 
                            , mc.ct_name as country
       
                            , case when a.ins_emp_id is not null then a.insurance_company else cc.key_value end  billing_charge 
                            , a.certificates_total  
                     
                            from bz_doc_head h  
                            inner join bz_doc_traveler_expense ex on h.dh_code = ex.dh_code 
                            inner join vw_bz_users b on ex.dte_emp_id = b.employeeid
                            inner join bz_data_passport p on ex.dte_emp_id = p.emp_id and p.default_type ='true' 
                            inner join bz_doc_insurance a on h.dh_code =  a.doc_id and ex.dh_code =  a.doc_id and ex.dte_emp_id = a.emp_id 
                            and p.emp_id = p.emp_id
                            left join bz_config_data cc on cc.status = 1 and cc.key_name ='Company Name'  and b.companyname = cc.key_filter
                            left join bz_config_data ca on ca.status = 1 and ca.key_name ='Company Address'  and b.companyname = ca.key_filter 
                            left join bz_master_country mc on ex.ct_id = mc.ct_id   
                            where substr(h.dh_code,3,2) = substr('" + year + "',3,2) ";
                sqlstr += @" order by h.dh_code,ex.dte_emp_id ";

                dt = new DataTable();
                conn = new cls_connection();
                if (SetDocService.conn_ExecuteData(ref dt, sqlstr) == "")
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        data.details_list.Add(new insuranceModel
                        {
                            id = dt.Rows[i]["id"].ToString(),
                            doc_id = dt.Rows[i]["doc_id"].ToString(),
                            emp_id = dt.Rows[i]["emp_id"].ToString(),
                            emp_passport = dt.Rows[i]["emp_passport"].ToString(),
                            emp_display = dt.Rows[i]["emp_display"].ToString(),
                            emp_section = dt.Rows[i]["emp_section"].ToString(),
                            emp_department = dt.Rows[i]["emp_department"].ToString(),
                            emp_function = dt.Rows[i]["emp_function"].ToString(),

                            name_beneficiary = dt.Rows[i]["name_beneficiary"].ToString(),
                            relationship = dt.Rows[i]["relationship"].ToString(),

                            certificates_no = dt.Rows[i]["certificates_no"].ToString(),
                            period_ins_from = dt.Rows[i]["period_ins_from"].ToString(),
                            period_ins_to = dt.Rows[i]["period_ins_to"].ToString(),
                            duration = dt.Rows[i]["duration"].ToString(),
                            country = dt.Rows[i]["country"].ToString(),
                            billing_charge = dt.Rows[i]["billing_charge"].ToString(),
                            certificates_total = dt.Rows[i]["certificates_total"].ToString(),

                        });
                    }
                }

                ret = "true";
            }
            catch (Exception ex) { msg_error = ex.Message.ToString(); data.token_login = msg_error; }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
            data.after_trip.opt2.status = (ret.ToLower() ?? "") == "true" ? "Export report succesed." : "Export report failed.";
            data.after_trip.opt2.remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error;
            data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
            data.after_trip.opt3.status = "Error msg";
            data.after_trip.opt3.remark = msg_error;

            return data;
        }

    }






}