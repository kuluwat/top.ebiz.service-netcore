using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

//using System.Web.Mail;
using System.Configuration;
//using ebiz.webservice.Models;
using System.DirectoryServices;
using System.IO;
using Microsoft.Exchange.WebServices.Data;
using top.ebiz.service.Models.Traveler_Profile;
using System.Net.Mail;
//using System.Web.Script.Serialization;

namespace top.ebiz.service.Service.Traveler_Profile 
{
    public class SendEmailService
    {
        SetDocService sws = new SetDocService();
        SearchDocService swd = new SearchDocService();

        cls_connection conn;
        string sqlstr = "";
        string sqlstr_all = "";
        string ret = "";
        DataTable dt;
        Boolean user_admin = false;
        string user_id = "";
        string user_role = "";

        string SystemUser = System.Configuration.ConfigurationManager.AppSettings["SystemUser"].ToString();
        string SystemPass = System.Configuration.ConfigurationManager.AppSettings["SystemPass"].ToString();



        #region  emailconfig  
        public string send_mail_bk(string s_mail_to, string s_mail_cc, string s_subject, string s_mail_body, string s_mail_attachments)
        {
            string mail_test = System.Configuration.ConfigurationManager.AppSettings["MailTest"].ToString();
            string mail_server = System.Configuration.ConfigurationManager.AppSettings["MailSMTPServer"].ToString();
            string mail_from = System.Configuration.ConfigurationManager.AppSettings["MailFrom"].ToString();
            if (mail_test != "")
            {
                s_mail_body += "</br>ข้อมูล mail to: " + s_mail_to + "</br></br>ข้อมูล mail cc: " + s_mail_cc;

                s_mail_to = mail_test;
                s_mail_cc = mail_test;
            }
            #region send
            MailPriority mailPriority = MailPriority.Normal; //High / Low/ Normal;
            //MailFormat mailFormat = MailFormat.Html;
            MailMessage objEmail;

            System.Net.Mail.MailAddress smailFrom = new System.Net.Mail.MailAddress(mail_from, "Thaioil workflow system [Do not reply]");
            //objEmail = new MailMessage
            //{
            //    From = smailFrom,
            //    To = s_mail_to,
            //    Cc = s_mail_cc.ToString(),
            //    Subject = s_subject,
            //    Body = s_mail_body
            //};

            try
            {
                if ((s_mail_attachments + "") != "")
                {
                    string[] xsplit_attachments = s_mail_attachments.Split(new char[] { '|', '|' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < xsplit_attachments.Length; i++)
                    {
                        //แนบไฟล์ไม่ได้
                        string templateFile = xsplit_attachments[i];
                        if (false)
                        {
                            objEmail.Attachments.Add(new System.Net.Mail.Attachment(templateFile));

                            ////Get some binary data
                            //byte[] templateContent = File.ReadAllBytes(templateFile);
                            //System.IO.MemoryStream stream = new System.IO.MemoryStream();
                            //stream.Write(templateContent, 0, templateContent.Length);
                            //WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true);
                            //byte[] contentOfWordFile = stream.ToArray();
                            //objEmail.Attachments.Add(contentOfWordFile);
                        }
                    }
                }
            }
            catch (Exception ex) { return ex.Message.ToString(); }

            //objEmail.Priority = mailPriority;
            //objEmail.BodyFormat = mailFormat;
            //SmtpMail.SmtpServer = mail_server;
            try
            {
                //SmtpMail.Send(objEmail);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            #endregion send

        }


        public Microsoft.Exchange.WebServices.Autodiscover.AutodiscoverRedirectionUrlValidationCallback RedirectionUrlValidationCallback { get; set; }
        public String send_mail(String s_mail_to, String s_mail_cc, String s_subject, String s_mail_body, string s_mail_attachments)
        {
            String msg_mail = "";
            Boolean SendAndSaveCopy = false;
            string mail_server = System.Configuration.ConfigurationManager.AppSettings["MailSMTPServer"].ToString();
            string mail_from = System.Configuration.ConfigurationManager.AppSettings["MailFrom"].ToString();
            string mail_test = System.Configuration.ConfigurationManager.AppSettings["MailTest"].ToString();
            string mail_user = System.Configuration.ConfigurationManager.AppSettings["MailUser"].ToString();
            string mail_password = System.Configuration.ConfigurationManager.AppSettings["MailPassword"].ToString();
            //bool SendAndSaveCopy = Convert.ToBoolean(ConfigurationManager.AppSettings["MailSendAndSaveCopy"].ToString());

            if (mail_test != "")
            {
                s_mail_body += "</br>ข้อมูล mail to: " + s_mail_to + "</br></br>ข้อมูล mail cc: " + s_mail_cc;
                s_mail_body += "</br>ข้อมูล attachments : " + s_mail_attachments;

                s_mail_to = mail_test;
                s_mail_cc = mail_test;
            }

            ExchangeService service = new ExchangeService
            {
                Credentials = new WebCredentials(mail_user, mail_password),
                TraceEnabled = true
            };


            EmailMessage email = new EmailMessage(service);
            service.AutodiscoverUrl(mail_from, RedirectionUrlValidationCallback);
            email.From = new EmailAddress("Mail Display ใส่ไม่มีผล", mail_from);
            //email.From.Name = "Car Service TSR";
            //email.From.Address = MailFrom;

            var email_to = s_mail_to.Split(';');
            for (int i = 0; i < email_to.Length; i++)
            {
                if (email_to[i].ToString() != "")
                {
                    // Mail To จะต้องใช้วิธี Loop และห้ามใส่ ; ต่อท้าย
                    email.ToRecipients.Add(email_to[i]);
                }
            }
            var email_cc = s_mail_cc.Split(';');
            for (int i = 0; i < email_cc.Length; i++)
            {
                if (email_cc[i].ToString() != "")
                {
                    //Mail CC จะต้องใช้วิธี Loop และห้ามใส่ ; ต่อท้าย
                    email.CcRecipients.Add(email_cc[i]);
                }
            }

            //Subject
            email.Subject = s_subject;

            //Body
            email.Body = new MessageBody(BodyType.HTML, s_mail_body);


            //Attachments
            //string filePath = Path.Combine(Server.MapPath("~/temp"), "EMPLOYEE LETTER_TES_Mr._Luck_Saraya_170521012548" + ".docx");
            string filePath = s_mail_attachments;
            if ((s_mail_attachments + "") != "")
            {
                string[] xsplit_attachments = s_mail_attachments.Split(new char[] { '|', '|' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < xsplit_attachments.Length; i++)
                {
                    string templateFile = xsplit_attachments[i];
                    email.Attachments.AddFileAttachment(templateFile);
                }
            }

            try
            {
                if (SendAndSaveCopy == true)
                {
                    //จะมีใน send box item
                    email.SendAndSaveCopy();
                }
                else
                {
                    //ไม่เก็บใน send box item
                    email.Send();
                }
                msg_mail = "";
            }
            catch (Exception ex)
            {
                msg_mail = ex.ToString();
            }

            return msg_mail;
        }

        //TOP GROUP PMSV-Business
        public List<Users> ListPMSVBusinessTest(string group_key_name)
        {
            DataTable dtadmin = new DataTable();
            List<Users> lstADUsers = new List<Users>();
            if (true)
            {
                string UserName = SystemUser.ToUpper();
                string Password = SystemPass;

                string memberOf = "CN=" + group_key_name + ",OU=Distribution Group,OU=E-Mail Resources,OU=Resources Management,DC=thaioil,DC=localnet";

                String domain = "ThaioilNT";

                String str = String.Format("LDAP://{0}", domain);
                String str2 = String.Format(("{0}\\" + UserName.Trim()), domain);
                String pass = Password;
                //DirectoryEntry Entry = new DirectoryEntry(str, str2, pass);
                //DirectorySearcher search = new DirectorySearcher(Entry)
                {
                    //Filter = "(&(objectClass=user)(objectCategory=person)(memberOf=" + memberOf + "))"
                };
                //search.PropertiesToLoad.Add("samaccountname");
                //search.PropertiesToLoad.Add("mail");
                //search.PropertiesToLoad.Add("usergroup");
                //search.PropertiesToLoad.Add("displayname");//first name
                //search.PropertiesToLoad.Add("memberOf");
                //SearchResult result;
                //SearchResultCollection resultCol = search.FindAll();

                //SearchResult xxx = search.FindOne();
                //if (resultCol != null)
                //{
                //    for (int counter = 0; counter < resultCol.Count; counter++)
                //    {
                //        string UserNameEmailString = string.Empty;
                //        result = resultCol[counter];
                //        if (result.Properties.Contains("samaccountname") &&
                //                 result.Properties.Contains("mail") &&
                //            result.Properties.Contains("displayname"))
                //        {

                //            Users objSurveyUsers = new Users
                //            {
                //                Email = (String)result.Properties["mail"][0],
                //                UserName = (String)result.Properties["samaccountname"][0],
                //                DisplayName = (String)result.Properties["displayname"][0],
                //                MemberOf = (String)result.Properties["memberOf"][0]
                //            };
                //            lstADUsers.Add(objSurveyUsers);
                //        }

                //    }
                //}


            }
            return lstADUsers;
        }

        public List<Users> ListSupperAdmin(ref string saduser_email, string group_key_name)
        {
            if (group_key_name == "") { group_key_name = "E-Mail Group E-Biz"; }
            DataTable dtadmin = new DataTable();
            List<Users> lstADUsers = new List<Users>();
            if (true)
            {
                DataTable dtEmailGroupList = new DataTable();
                sqlstr = @"select key_value as name  from  bz_config_data where trim(lower(key_name)) =trim(lower('" + group_key_name + "')) ";
                if (SetDocService.conn_ExecuteData(ref dtEmailGroupList, sqlstr) == "")
                {
                    if (dtEmailGroupList.Rows.Count > 0)
                    {
                        string UserName = SystemUser.ToUpper();
                        string Password = SystemPass;

                        for (int i = 0; i < dtEmailGroupList.Rows.Count; i++)
                        {
                            string memberOf = "CN=" + dtEmailGroupList.Rows[i]["name"].ToString() + ",OU=Distribution Group,OU=E-Mail Resources,OU=Resources Management,DC=thaioil,DC=localnet";

                            String domain = "ThaioilNT";

                            String str = String.Format("LDAP://{0}", domain);
                            String str2 = String.Format(("{0}\\" + UserName.Trim()), domain);
                            String pass = Password;
                            //DirectoryEntry Entry = new DirectoryEntry(str, str2, pass);
                            //DirectorySearcher search = new DirectorySearcher(Entry)
                            //{
                            //    Filter = "(&(objectClass=user)(objectCategory=person)(memberOf=" + memberOf + "))"
                            //};
                            //search.PropertiesToLoad.Add("samaccountname");
                            //search.PropertiesToLoad.Add("mail");
                            //search.PropertiesToLoad.Add("usergroup");
                            //search.PropertiesToLoad.Add("displayname");//first name
                            //search.PropertiesToLoad.Add("memberOf");
                            //SearchResult result;
                            //SearchResultCollection resultCol = search.FindAll();

                            ////SearchResult xxx = search.FindOne();
                            //if (resultCol != null)
                            //{
                            //    for (int counter = 0; counter < resultCol.Count; counter++)
                            //    {
                            //        string UserNameEmailString = string.Empty;
                            //        result = resultCol[counter];
                            //        if (result.Properties.Contains("samaccountname") &&
                            //                 result.Properties.Contains("mail") &&
                            //            result.Properties.Contains("displayname"))
                            //        {

                            //            Users objSurveyUsers = new Users
                            //            {
                            //                Email = (String)result.Properties["mail"][0],
                            //                UserName = (String)result.Properties["samaccountname"][0],
                            //                DisplayName = (String)result.Properties["displayname"][0],
                            //                MemberOf = (String)result.Properties["memberOf"][0]
                            //            };
                            //            lstADUsers.Add(objSurveyUsers);
                            //        }

                            //    }
                            //}


                        }
                    }
                }
                //data.aduser_list = lstADUsers;

                for (int i = 0; i < lstADUsers.Count; i++)
                {
                    //xxx1@thaioilgroup.com;xxx2@thaioilgroup.com;
                    saduser_email += lstADUsers[i].Email.ToString() + ";";
                }
            }
            return lstADUsers;
        }
        public List<Users> ListISOSMember(ref string saduser_email, string group_key_name)
        {
            if (group_key_name == "") { group_key_name = "E-Mail Group E-Biz"; }
            DataTable dtadmin = new DataTable();
            List<Users> lstADUsers = new List<Users>();
            if (true)
            {
                DataTable dtEmailGroupList = new DataTable();
                sqlstr = @"select key_value as name  from  bz_config_data where trim(lower(key_name)) =trim(lower('" + group_key_name + "')) ";
                if (SetDocService.conn_ExecuteData(ref dtEmailGroupList, sqlstr) == "")
                {
                    if (dtEmailGroupList.Rows.Count > 0)
                    {
                        string UserName = SystemUser.ToUpper();
                        string Password = SystemPass;

                        for (int i = 0; i < dtEmailGroupList.Rows.Count; i++)
                        {
                            string memberOf = "CN=" + dtEmailGroupList.Rows[i]["name"].ToString() + ",OU=Distribution Group,OU=E-Mail Resources,OU=Resources Management,DC=thaioil,DC=localnet";

                            String domain = "ThaioilNT";

                            String str = String.Format("LDAP://{0}", domain);
                            String str2 = String.Format(("{0}\\" + UserName.Trim()), domain);
                            String pass = Password;
                            //DirectoryEntry Entry = new DirectoryEntry(str, str2, pass);
                            //DirectorySearcher search = new DirectorySearcher(Entry)
                            //{
                            //    Filter = "(&(objectClass=user)(objectCategory=person)(memberOf=" + memberOf + "))"
                            //};
                            //search.PropertiesToLoad.Add("samaccountname");
                            //search.PropertiesToLoad.Add("mail");
                            //search.PropertiesToLoad.Add("usergroup");
                            //search.PropertiesToLoad.Add("displayname");//first name
                            //search.PropertiesToLoad.Add("memberOf");
                            //SearchResult result;
                            //SearchResultCollection resultCol = search.FindAll();

                            ////SearchResult xxx = search.FindOne();
                            //if (resultCol != null)
                            //{
                            //    for (int counter = 0; counter < resultCol.Count; counter++)
                            //    {
                            //        string UserNameEmailString = string.Empty;
                            //        result = resultCol[counter];
                            //        if (result.Properties.Contains("samaccountname") &&
                            //                 result.Properties.Contains("mail") &&
                            //            result.Properties.Contains("displayname"))
                            //        {

                            //            Users objSurveyUsers = new Users
                            //            {
                            //                Email = (String)result.Properties["mail"][0],
                            //                UserName = (String)result.Properties["samaccountname"][0],
                            //                DisplayName = (String)result.Properties["displayname"][0],
                            //                MemberOf = (String)result.Properties["memberOf"][0]
                            //            };
                            //            lstADUsers.Add(objSurveyUsers);
                            //        }

                            //    }
                            //}


                        }
                    }
                }
                //data.aduser_list = lstADUsers;

                for (int i = 0; i < lstADUsers.Count; i++)
                {
                    //xxx1@thaioilgroup.com;xxx2@thaioilgroup.com;
                    saduser_email += lstADUsers[i].Email.ToString() + ";";
                }
            }
            return lstADUsers;
        }

        public EmailModel EmailConfig(EmailModel value)
        {
            var msg_error = "";
            var data = value;
            var emp_user_active = "";//เอา token_login ไปหา
            var emp_id_active = "";// value.emp_id;
            var token_login = value.token_login;

            var page_name = value.page_name;
            var action_name = value.action_name;

            if (page_name.ToLower() == "travelinsurance")
            {
                if (action_name.ToLower() == ("NotiTravelInsuranceForm").ToLower())
                {
                    emailNotiTravelInsuranceForm(ref data, ref msg_error);
                }
                else if (action_name.ToLower() == ("NotiTravelInsuranceListPassportInfo").ToLower())
                {
                    emailNotiTravelInsuranceListPassportInfo(ref data, ref msg_error);
                }
                else if (action_name.ToLower() == ("NotiTravelInsuranceCertificates").ToLower())
                {
                    emailNotiTravelInsuranceCertificates(ref data, ref msg_error);
                }
            }
            else if (page_name.ToLower() == "isos")
            {
                if (action_name.ToLower() == ("NotiISOSNewListRuningNoName").ToLower())
                {
                    emailNotiISOSNewListRuningNoName(ref data, ref msg_error);
                }
                else if (action_name.ToLower() == ("NotiISOSNewList").ToLower())
                {
                    emailNotiISOSNewList(ref data, ref msg_error);
                }
            }
            else if (page_name.ToLower() == "visa")
            {
                if (action_name.ToLower() == ("NotiTravellerCheckVisa").ToLower())
                {
                    emailNotiISOSNewListRuningNoName(ref data, ref msg_error);
                }
                else if (action_name.ToLower() == ("NotiAccommodationToAdmin").ToLower())
                {
                    emailNotiISOSNewList(ref data, ref msg_error);
                }
                else if (action_name.ToLower() == ("NotiDetailVisaRequest").ToLower())
                {
                    emailNotiISOSNewList(ref data, ref msg_error);
                }
                else if (action_name.ToLower() == ("NotiRequestPrepareVisa").ToLower())
                {
                    emailNotiISOSNewList(ref data, ref msg_error);
                }
            }

            data.after_trip.opt1 = (ret.ToLower() ?? "") == "true" ? "true" : "false";
            data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel();
            data.after_trip.opt2 = new Models.Create_Trip.subAfterTripModel
            {
                status = (ret.ToLower() ?? "") == "true" ? "Send mail succesed." : "Send mail failed.",
                remark = (ret.ToLower() ?? "") == "true" ? "" : msg_error
            };
            data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel();
            data.after_trip.opt3 = new Models.Create_Trip.subAfterTripModel
            {
                status = "Error msg",
                remark = msg_error
            };

            return data;
        }

        #endregion  emailconfig 

        private void emailNotiTravelInsuranceForm(ref EmailModel data, ref string msg_error)
        {
            var token_login = data.token_login;
            var doc_id = data.doc_id;

            //แจงเตือนเพõ่อใหพนักงานเขายืนยันขอมูลในแบบฟอรม "คําขอเอาประกันภัยการเดินทาง
            //ตางประเทศ" ในระบบ
            //To: ผูเดินทาง
            //Cc: Super Admin โดยการเลือก กลุม PMSV หรéอ PMDV  
            //ระบบสรางกลุม E-Mail Group E-Biz ทั้ง Group PMSV และ PMDV --> Employee / Admin (SV&DV)

            //หาผู้เดินทางของ doc id ที่ CAP Approve แล้ว
            DataTable dtemp = new DataTable();
            swd = new SearchDocService();
            sqlstr = swd.sqlstr_data_emp_detail(token_login, doc_id, "ListEmployeeTravelInsurance");
            if (SetDocService.conn_ExecuteData(ref dtemp, sqlstr) == "")
            {
                if (dtemp.Rows.Count == 0)
                {
                    msg_error = "ไม่มีข้อมูลที่จะส่งไป แจ้งเตือนพนักงานเพื่อให้กรอกคำขอเอาประกันภัยการเดินทางต่างประเทศ";
                    return;
                }
                else
                {
                    List<emailList> email_list = new List<emailList>();
                    for (int i = 0; i < dtemp.Rows.Count; i++)
                    {
                        emailList deflist = new emailList
                        {
                            id = (i + 1).ToString(),
                            doc_id = doc_id,
                            emp_id = dtemp.Rows[i]["emp_id"].ToString(),
                            emp_name = dtemp.Rows[i]["emp_name"].ToString(),
                            email = dtemp.Rows[i]["email"].ToString(),
                            email_status = "",
                            email_msg = ""
                        };
                        email_list.Add(deflist);
                    }
                    data.email_list = email_list;
                }
            }

            //หากลุ่ม Super Admin 
            string saduser_email = "";
            data.aduser_list = ListSupperAdmin(ref saduser_email, "");

            //send mail 
            if (true)
            {
                string s_mail_to = "";
                string s_mail_cc = "";
                string s_mail_subject = "";
                string s_mail_body = "";
                string s_mail_attachments = "";

                for (int i = 0; i < data.email_list.Count; i++)
                {
                    s_mail_to += data.email_list[i].email.ToString() + ";";
                }
                s_mail_cc = saduser_email;

                s_mail_subject = @"Email แจ้งกรอกข้อมูล Traveling Insurance Form";
                s_mail_body = @"Admin (User Request) เขาดําเนินการ submit to send Travel Insurance หรéอ คําขอ
                                เอาประกันภัยการเดินทางตางประเทศ ใหผูเดินทางกรอกขอมูล
                                ระบบสราง Travel Insurance เพõ่อนําสงขอมูลทาง E-Mail ใหกับบรæษัทฯ
                                ประกันได ";

                msg_error = send_mail(s_mail_to, s_mail_cc, s_mail_subject, s_mail_body, s_mail_attachments);

            }
        }
        private void emailNotiTravelInsuranceListPassportInfo(ref EmailModel data, ref string msg_error)
        {
            var token_login = data.token_login;
            var doc_id = data.doc_id;
            var emp_id = data.emp_id; //user ที่ส่งข้อมูลมา กรณีที่เป็น admin ให้ส่งทั้งหมด  กรณีที่เป็นเจ้าของข้อมูลให้ส่งตัวเองเท่านั้น
            var page_name = "travelinsurance";

            sws = new SetDocService();
            sws.sqlEmpRole(token_login, ref user_id, ref user_role, ref user_admin, doc_id);


            //TO: Admin(User Request)
            //CC: ผู้เดินทาง 

            //หาผู้เดินทางของ doc id ที่ CAP Approve แล้ว
            DataTable dtemp = new DataTable();
            swd = new SearchDocService();
            sqlstr = swd.sqlstr_data_emp_detail(token_login, doc_id, "ListEmployeeTravelInsurance");
            if (SetDocService.conn_ExecuteData(ref dtemp, sqlstr) == "")
            {
                if (dtemp.Rows.Count == 0)
                {
                    msg_error = "ไม่มีข้อมูลที่จะส่งไป แจ้งเตือนพนักงานเพื่อให้กรอกคำขอเอาประกันภัยการเดินทางต่างประเทศ";
                    return;
                }
                else
                {
                    List<emailList> email_list = new List<emailList>();
                    for (int i = 0; i < dtemp.Rows.Count; i++)
                    {
                        if (emp_id == dtemp.Rows[i]["emp_id"].ToString() || user_admin == true)
                        {
                            emailList deflist = new emailList
                            {
                                id = (i + 1).ToString(),
                                doc_id = doc_id,
                                emp_id = dtemp.Rows[i]["emp_id"].ToString(),
                                emp_name = dtemp.Rows[i]["emp_name"].ToString(),
                                email = dtemp.Rows[i]["email"].ToString(),
                                email_status = "",
                                email_msg = ""
                            };
                            email_list.Add(deflist);
                        }
                    }
                    data.email_list = email_list;
                }
            }

            //หากลุ่ม Super Admin 
            string saduser_email = "";
            data.aduser_list = ListSupperAdmin(ref saduser_email, "");

            //ส่งข้อมูลให้ทาง admin เพื่อดำเนินการแก้ไขก่อน step ต่อไปจะส่งให้ บริษัทฯประกัน   
            List<ImgList> fileList = new List<ImgList>();

            List<ImgList> imgList = swd.refdata_img_list(doc_id, page_name, "", token_login);
            if (user_admin == false)
            {
                for (int i = 0; i < imgList.Count; i++)
                {
                    if (emp_id == imgList[i].emp_id.ToString() || user_admin == true)
                    {
                        data.file_list.Add(imgList[i]);
                    }
                }
            }

            //send mail 
            if (true)
            {
                string s_mail_to = "";
                string s_mail_cc = "";
                string s_mail_subject = "";
                string s_mail_body = "";
                string s_mail_attachments = "";

                for (int i = 0; i < data.email_list.Count; i++)
                {
                    s_mail_cc += data.email_list[i].email.ToString() + ";";
                }
                s_mail_to = saduser_email;

                for (int i = 0; i < data.file_list.Count; i++)
                {
                    if (s_mail_attachments != "") { s_mail_attachments += "||"; }
                    s_mail_attachments += data.file_list[i].path + data.file_list[i].filename;
                }

                s_mail_subject = @"Email แจ้ง Travel Insurance List + Passport Info";
                s_mail_body = @"1) พนักงานเขายืนยันขอมูลในแบบฟอรม คําขอเอาประกันภัยการเดินทางตางประเทศ หรéอบน
                                ระบบ ไดแก
                                 • ชื่อผูรับผลประโยชน
                                 • ความสัมพันธกับ ผูรับผลประโยชน
                                 • พนักงานสามารถเพòöมเติม Travel Insurance แบบสวนตัวได
                                2) ขอมูลอื่นๆ ใหทางระบบดึงขอมูลจากฐานขอมูล และระบบ E - B";

                msg_error = send_mail(s_mail_to, s_mail_cc, s_mail_subject, s_mail_body, s_mail_attachments);

            }

        }
        private void emailNotiTravelInsuranceCertificates(ref EmailModel data, ref string msg_error)
        {
            var token_login = data.token_login;
            var doc_id = data.doc_id;

            //•  Email แจ้ง Travel Insurance Certificates  
            //• Admin(User Request)ตรวจสอบขอมูลพรอมสง submit to send Travel Insurance
            //หรéอ คําขอเอาประกันภัยการเดินทางตางประเทศ ใหกับบรæษัทประกัน(Attached เปน PDF จาก
            //Insurance Form)
            //TO: บรæษัทฯ ประกัน(คาง E-Mail เดิม Defult คาไว)
            //CC: Super Admin โดยการเลือก กลุม PMSV หรéอ PMDV

            DataTable dtemp = new DataTable();
            string group_key_name = "E-Mail Group Company Insurance";
            sqlstr = @"select key_value, key_email  from  bz_config_data where trim(lower(key_name)) =trim(lower('" + group_key_name + "')) ";
            if (SetDocService.conn_ExecuteData(ref dtemp, sqlstr) == "")
            {
                if (dtemp.Rows.Count == 0)
                {
                    msg_error = "ไม่มีข้อมูลบริษัทฯประกัน";
                    return;
                }

                List<emailList> email_list = new List<emailList>();
                for (int i = 0; i < dtemp.Rows.Count; i++)
                {
                    emailList deflist = new emailList
                    {
                        id = (i + 1).ToString(),
                        doc_id = doc_id,
                        emp_id = "",
                        emp_name = dtemp.Rows[i]["key_value"].ToString(),
                        email = dtemp.Rows[i]["key_email"].ToString(),
                        email_status = "",
                        email_msg = ""
                    };
                    email_list.Add(deflist);
                }
                data.email_list = email_list;
            }
            //หากลุ่ม Super Admin 
            string saduser_email = "";
            data.aduser_list = ListSupperAdmin(ref saduser_email, "");

            //send mail 
            if (true)
            {
                string s_mail_to = "";
                string s_mail_cc = "";
                string s_mail_subject = "";
                string s_mail_body = "";
                string s_mail_attachments = "";

                for (int i = 0; i < data.email_list.Count; i++)
                {
                    s_mail_to += data.email_list[i].email.ToString() + ";";
                }
                s_mail_cc = saduser_email;

                s_mail_subject = @"Email แจ้ง Email แจ้ง Travel Insurance Certificates";
                s_mail_body = @"Admin(User Request)ตรวจสอบขอมูลพรอมสง submit to send Travel Insurance
                                หรéอ คําขอเอาประกันภัยการเดินทางตางประเทศ ใหกับบรæษัทประกัน(Attached เปน PDF จาก
                                Insurance Form)";

                msg_error = send_mail(s_mail_to, s_mail_cc, s_mail_subject, s_mail_body, s_mail_attachments);

            }
        }

        public void emailNotiISOSNewListRuningNoName(ref EmailModel data, ref string msg_error)
        {
            var token_login = data.token_login;
            var doc_id = data.doc_id;

            //เมื่อพบการ update กรมธรรมแลว เรçยบรอยแลว ใหระบบแจงเดือน Super Admin กลุมที่
            //requested Travel Insurance หรéอตรวจสอบหลักสูตรวาเปน business หรéอ training แลว
            //ดําเนินการดังนี้
            //1)หากไมพบ Record ใหระบบบันทึกรายชื่อ และ Create Runnin พรอมสงขอมูลใหกับบรæษัท
            //ISOS
            //TO: ISOS
            //CC: Super Admin โดยการเลือก กลุม PMSV หรéอ PMDV
            //ระบบสราง ISOS MEMBER LIST เพõ่อเก็บขอมูลการสมัคร ISOS

            //หาผู้เดินทางของ doc id ที่ CAP Approve แล้ว
            DataTable dtemp = new DataTable();
            swd = new SearchDocService();
            sqlstr = @" select a.doc_id,a.isos_emp_id as emp_id,a.isos_emp_name ||' '|| a.isos_emp_surname as emp_name
                        ,bu.email as email
                        from bz_doc_isos a
                        left join bz_users bu on a.isos_emp_id = bu.employeeid    
                        inner join (
                            select distinct ta.dh_code, ta.dta_appr_empid as approverid, ta.dta_travel_empid as employeeid
                            from  bz_doc_traveler_approver ta 
                            where lower(ta.dta_remark) = lower('CAP') and  ta.dta_doc_status = 42 and  lower(ta.dh_code) like lower('OB%') 
                         )ta   on a.isos_emp_id  = ta.employeeid and a.doc_id = ta.dh_code  
                        where a.send_mail = 0 and a.doc_id = '" + doc_id + "' order by a.isos_emp_id ";
            if (SetDocService.conn_ExecuteData(ref dtemp, sqlstr) == "")
            {
                if (dtemp.Rows.Count == 0)
                {
                    msg_error = "ไม่มีข้อมูล ISOS Member List";
                    return;
                }
                else
                {
                    List<emailList> email_list = new List<emailList>();
                    for (int i = 0; i < dtemp.Rows.Count; i++)
                    {
                        emailList deflist = new emailList
                        {
                            id = (i + 1).ToString(),
                            doc_id = doc_id,
                            emp_id = dtemp.Rows[i]["emp_id"].ToString(),
                            emp_name = dtemp.Rows[i]["emp_name"].ToString(),
                            email = dtemp.Rows[i]["email"].ToString(),
                            email_status = "",
                            email_msg = ""
                        };
                        email_list.Add(deflist);
                    }
                    data.email_list = email_list;
                }
            }

            //หากลุ่ม Super Admin 
            string saduser_email = "";
            data.aduser_list = ListSupperAdmin(ref saduser_email, "");

            //send mail 
            if (true)
            {
                string s_mail_to = "";
                string s_mail_cc = "";
                string s_mail_subject = "";
                string s_mail_body = "";
                string s_mail_attachments = "";

                for (int i = 0; i < data.email_list.Count; i++)
                {
                    s_mail_to += data.email_list[i].email.ToString() + ";";
                }
                s_mail_cc = saduser_email;

                s_mail_subject = @"Email แจ้ง ISOS New List (Running No + Name)";
                s_mail_body = @" เมื่อพบการ update กรมธรรมแลว เรçยบรอยแลว ใหระบบแจงเดือน Super Admin กลุมที่
                                 requested Travel Insurance หรéอตรวจสอบหลักสูตรวาเปน business หรéอ training แลว
                                 ดําเนินการดังนี้
                                 1)หากไมพบ Record ใหระบบบันทึกรายชื่อ และ Create Runnin พรอมสงขอมูลใหกับบรæษัท
                                 ISOS";

                msg_error = send_mail(s_mail_to, s_mail_cc, s_mail_subject, s_mail_body, s_mail_attachments);

            }


        }
        private void emailNotiISOSNewList(ref EmailModel data, ref string msg_error)
        {
            var token_login = data.token_login;
            var doc_id = data.doc_id;

            //หากมีการ Update กรมธรรมแลว ใหดําเนินการแจง E-Mail ใหกับผูเกี่ยวของเพõ่อเขาตรวจสอบ
            //ขอมูล พรอมกับดูตารางกรมธรรม รายละเอียดเพòöมเติมเกี่ยวกับการเบิกคืนคาสินไหมสินไหม และ
            // รายละเอียดการใชและบรæการ ISOS ซึ่งแสดงทั้งบน E-Mail และสามารถเขาไปดูในระบบได
            //TO: ผูเดินทาง
            //CC: Super Admin โดยการเลือก กลุม PMSV หรéอ PMDV

            //หาผู้เดินทางของ doc id ที่ CAP Approve แล้ว
            DataTable dtemp = new DataTable();
            swd = new SearchDocService();
            sqlstr = swd.sqlstr_data_emp_detail(token_login, doc_id, "ListEmployeeISOSMemberList");
            if (SetDocService.conn_ExecuteData(ref dtemp, sqlstr) == "")
            {
                if (dtemp.Rows.Count == 0)
                {
                    msg_error = "ไม่มีข้อมูลที่จะส่งไป แจ้งเตือนพนักงานเพื่อให้กรอกคำขอเอาประกันภัยการเดินทางต่างประเทศ";
                    return;
                }
                else
                {
                    List<emailList> email_list = new List<emailList>();
                    for (int i = 0; i < dtemp.Rows.Count; i++)
                    {
                        emailList deflist = new emailList
                        {
                            id = (i + 1).ToString(),
                            doc_id = doc_id,
                            emp_id = dtemp.Rows[i]["emp_id"].ToString(),
                            emp_name = dtemp.Rows[i]["emp_name"].ToString(),
                            email = dtemp.Rows[i]["email"].ToString(),
                            email_status = "",
                            email_msg = ""
                        };
                        email_list.Add(deflist);
                    }
                    data.email_list = email_list;
                }
            }

            //หากลุ่ม Super Admin 
            string saduser_email = "";
            data.aduser_list = ListSupperAdmin(ref saduser_email, "");

            //send mail 
            if (true)
            {
                string s_mail_to = "";
                string s_mail_cc = "";
                string s_mail_subject = "";
                string s_mail_body = "";
                string s_mail_attachments = "";

                for (int i = 0; i < data.email_list.Count; i++)
                {
                    s_mail_to += data.email_list[i].email.ToString() + ";";
                }
                s_mail_cc = saduser_email;

                s_mail_subject = @"Email แจ้งISOS New List";
                s_mail_body = @"หากมีการ Update กรมธรรมแลว ใหดําเนินการแจง E-Mail ใหกับผูเกี่ยวของเพõ่อเขาตรวจสอบ
                                ขอมูล พรอมกับดูตารางกรมธรรม รายละเอียดเพòöมเติมเกี่ยวกับการเบิกคืนคาสินไหมสินไหม และ
                                รายละเอียดการใชและบรæการ ISOS ซึ่งแสดงทั้งบน E-Mail และสามารถเขาไปดูในระบบได";

                msg_error = send_mail(s_mail_to, s_mail_cc, s_mail_subject, s_mail_body, s_mail_attachments);

            }

        }


        public void emailNotiISOSNewRecord(ref EmailModel data, ref string msg_error)
        {
            var token_login = data.token_login;
            var doc_id = data.doc_id;

            //เมื่อพบการ update กรมธรรมแลว เรçยบรอยแลว ใหระบบแจงเดือน Super Admin กลุมที่
            //requested Travel Insurance หรéอตรวจสอบหลักสูตรวาเปน business หรéอ training แลว
            //ดําเนินการดังนี้
            //1)หากไมพบ Record ใหระบบบันทึกรายชื่อ และ Create Runnin พรอมสงขอมูลใหกับบรæษัท
            //ISOS
            //TO: ISOS
            //CC: Super Admin โดยการเลือก กลุม PMSV หรéอ PMDV
            //ระบบสราง ISOS MEMBER LIST เพõ่อเก็บขอมูลการสมัคร ISOS

            //หาผู้เดินทางของ doc id ที่ CAP Approve แล้ว
            DataTable dtemp = new DataTable();
            swd = new SearchDocService();
            sqlstr = @" 
                      select a.id, a.doc_id, a.emp_id, bu.email as email,insurance_company_id
                      from bz_doc_isos_record a
                      left join bz_users bu on a.emp_id = bu.employeeid     
                      where nvl(a.send_mail_type,0) = 0 
                      and a.doc_id = '" + doc_id + "'  order by a.emp_id ";
            if (SetDocService.conn_ExecuteData(ref dtemp, sqlstr) == "")
            {
                if (dtemp.Rows.Count == 0)
                {
                    msg_error = "ไม่มีข้อมูล ISOS New Record";
                    return;
                }
                else
                {
                    List<emailList> email_list = new List<emailList>();
                    for (int i = 0; i < dtemp.Rows.Count; i++)
                    {
                        emailList deflist = new emailList
                        {
                            id = dtemp.Rows[i]["id"].ToString(),
                            doc_id = dtemp.Rows[i]["doc_id"].ToString(),
                            emp_id = dtemp.Rows[i]["emp_id"].ToString(),
                            email = dtemp.Rows[i]["email"].ToString(),
                            email_status = "",
                            email_msg = ""
                        };
                        email_list.Add(deflist);
                    }
                    data.email_list = email_list;
                }
            }

            //หากลุ่ม Super Admin 
            string saduser_email = "";
            data.aduser_list = ListSupperAdmin(ref saduser_email, "");

            //send mail 
            if (true)
            {
                string s_mail_to = "";
                string s_mail_cc = "";
                string s_mail_subject = "";
                string s_mail_body = "";
                string s_mail_attachments = "";

                for (int i = 0; i < data.email_list.Count; i++)
                {
                    s_mail_to += data.email_list[i].email.ToString() + ";";
                }
                s_mail_cc = saduser_email;

                s_mail_subject = @"Email แจ้ง ISOS New Record (Running No + Name)";
                s_mail_body = @" หากไมพบ Record ใหระบบบันทึกรายชื่อ และ Create Runnin พรอมสงขอมูลใหกับบรæษัท ISOS";

                msg_error = send_mail(s_mail_to, s_mail_cc, s_mail_subject, s_mail_body, s_mail_attachments);
                if (msg_error == "")
                {
                    string emp_user_active = "";
                    //update 
                    sqlstr = @" UPDATE BZ_DOC_ISOS_RECORD SET SEND_MAIL_TYPE = 1";
                    sqlstr += @" ,UPDATE_BY = " + conn.ChkSqlStr(emp_user_active, 300);
                    sqlstr += @" ,UPDATE_DATE = sysdate";
                    sqlstr += @" ,TOKEN_UPDATE = " + conn.ChkSqlStr(token_login, 300);
                    sqlstr += @" where ";
                    sqlstr += @" DOC_ID = " + conn.ChkSqlStr(data.doc_id, 300);

                    ret = SetDocService.conn_ExecuteNonQuery(sqlstr, true);
                }
            }


        }

        #region In Page
        public string SendMailInPage(ref List<mailselectList> mail_list
            , List<EmpListOutModel> emp_list
            , List<ImgList> img_list
            , string doc_id, string page_name, string module_name)
        {
            page_name = page_name.ToLower();
            module_name = module_name.ToLower();

            //CONFIRMATION LETTER	028_OB/LB/OT/LT : Business Travel Confirmation Letter
            //ตั้ง batch --> Local 3 day, Overse 5 day  เช่น  busses พฤ ให้ส่งวันจันทร์
            //??? ให้ตรวจสอบข้อมูลไฟล์แนบใหม่ให้เกินจำนวน limit

            SearchDocService swd = new SearchDocService();
            //mail_list จะมีแค่รายการเดียว
            try
            {
                #region DevFix 20200911 0000 
                //http://tbkc-dapps-05.thaioil.localnet/Ebiz2/master/###
                string LinkLoginPhase2 = System.Configuration.ConfigurationManager.AppSettings["LinkLoginPhase2"].ToString();
                string sDear = "";
                string sDetail = "";
                string sTitle = "";
                string sBusinessDate = "";
                string sLocation = "";
                string sTravelerList = "";
                string sOtherList = "";

                DataTable dtresult = new DataTable();
                string Tel_Services_Team = "";
                string Tel_Call_Center = "";
                sqlstr = @" SELECT key_value as tel_services_team from bz_config_data where lower(key_name) = lower('tel_services_team') and status = 1";
                SetDocService.conn_ExecuteData(ref dtresult, sqlstr);
                try { Tel_Services_Team = dtresult.Rows[0]["tel_services_team"].ToString(); } catch { }
                sqlstr = @" SELECT key_value as tel_call_center from bz_config_data where lower(key_name) = lower('tel_call_center') and status = 1";
                SetDocService.conn_ExecuteData(ref dtresult, sqlstr);
                try { Tel_Call_Center = dtresult.Rows[0]["tel_call_center"].ToString(); } catch { }
                #endregion DevFix 20200911 0000  

                string msg_log = "";
                int iNo = 1;
                ret = "";
                for (int i = 0; i < mail_list.Count; i++)
                {
                    string xhtml = "";
                    string continent_id = "";
                    string country_id = "";
                    string emp_id_select = "";
                    string emp_id = mail_list[i].emp_id.ToLower();
                    string[] xemp_id = emp_id.Split(';');

                    //ตรวจสอบว่ามีการ active เพื่อส่ง mail หรือไม่จาก mail_status = 'true'
                    List<EmpListOutModel> drempcheck = emp_list.Where(a => ((a.emp_id.ToLower() == emp_id) && a.mail_status == "true")).ToList();
                    if (drempcheck.Count == 0)
                    {
                        if (emp_id.IndexOf(";") > -1)
                        {
                            for (int j = 0; j < xemp_id.Length; j++)
                            {
                                if (page_name == "isos" && module_name == "sendmail_isos_to_broker")
                                {
                                    //เอาข้อมูลตามที่เลือกจากหน้าบ้านทั้งหมด ให้เป็น mail_status = true
                                    drempcheck = emp_list.Where(a => ((a.emp_id.ToLower() == xemp_id[j].ToString()))).ToList();
                                    if (drempcheck.Count > 0)
                                    {
                                        drempcheck[0].mail_status = "true";
                                        if (emp_id_select != "") { emp_id_select += ","; }
                                        emp_id_select += "'" + xemp_id[j].ToString() + "'";
                                    }
                                }
                                else
                                {
                                    drempcheck = emp_list.Where(a => ((a.emp_id.ToLower() == xemp_id[j].ToString()) && a.mail_status == "true")).ToList();
                                    if (drempcheck.Count > 0)
                                    {
                                        emp_id_select = "'" + xemp_id[j].ToString() + "'"; break;
                                    }
                                }
                            }
                        }
                        if (page_name == "isos" && module_name == "sendmail_isos_to_broker")
                        {
                            //กรองข้อมูลอีกครั้งเพื่อใช้ในลำดับถัดไป
                            drempcheck = emp_list.Where(a => (a.mail_status == "true")).ToList();
                        }
                    }
                    if (drempcheck.Count == 0)
                    {
                        //ตรวจสอบว่ามีการ active เพื่อส่ง mail หรือไม่จาก mail_status = 'true' กรณีที่เป็นการส่งหา admin ให้เอารายละเอียดจากคนแรก
                        drempcheck = emp_list.Where(a => (a.mail_status == "true")).ToList();
                        if (drempcheck.Count > 0)
                        {
                            emp_id_select = "'" + drempcheck[0].emp_id + "'";
                        }
                        else { continue; }
                    }
                    if ((page_name == "travelexpense" && module_name == "tripcancelled")
                        || (page_name == "travelexpense" && module_name == "sendmail_to_sap"))
                    {
                        drempcheck = emp_list.Where(a => ("true" == "true")).ToList();
                    }

                    string s_mail_to = (mail_list[i].mail_to + "").ToString();
                    string s_mail_cc = (mail_list[i].mail_cc + "").ToString();
                    string s_subject = "";
                    string s_mail_body = "";
                    string s_mail_to_display = "";
                    string s_mail_body_in_form = "";
                    string s_mail_attachments = "";
                    string s_mail_to_emp_name = "All,";

                    try
                    {
                        s_mail_to_display = (mail_list[i].mail_to_display + "").ToString();
                    }
                    catch { }
                    try
                    {
                        s_mail_body_in_form = (mail_list[i].mail_body_in_form + "").ToString();
                    }
                    catch { }
                    try
                    {
                        s_mail_attachments = (mail_list[i].mail_attachments + "").ToString();
                    }
                    catch { }
                    if (page_name == "isos" || page_name == "isosrecord")
                    {
                    }
                    else
                    {
                        continent_id = drempcheck[0].continent_id;
                        country_id = drempcheck[0].country_id;
                        try
                        {
                            string[] xemp = s_mail_to.Split(';');
                            if ((xemp.Length == 1) || (xemp[1].ToString() == ""))
                            {
                                string smail = xemp[0].ToString();
                                //กรณีที่มีเพียง 1 คน --> xxx@thaioilgroup.com;
                                List<EmpListOutModel> dremp = emp_list.Where(a => (a.userEmail.ToLower() == smail.ToLower())).ToList();
                                if (dremp.Count > 0)
                                {
                                    s_mail_to_emp_name = dremp[0].userDisplay;
                                }
                            }
                        }
                        catch { }
                    }

                    s_subject = "E-Biz : Test Send E-Mail Data (" + doc_id + ")";

                    #region ข้อมูลที่ต้องส่งใน mail ของแต่ละ module
                    if (page_name == "airticket")
                    {
                        //009_OB/LB/OT/LT  --> traveler กด และ status = confirmed กับถ้าเป็นการเลือก Ask Booking by Company ต้องมี Already Booked = true
                        //010_OB/LB/OT/LT  --> admin กด และ status <> confirmed 
                        //011_OB/LB/OT/LT  --> admin กด และ status = confirmed กับถ้าเป็นการเลือก Ask Booking by Company ต้องมี Already Booked = true
                        if (module_name == "traveler_request")
                        {
                            //กรณีที่ ทาง traveler submit ส่ง 009_OB/LB/OT/LT : Please book an Air Ticket - [Title_Name of traveler]  ต้องการให้ทาง admin จองตั๋ว
                            s_subject = doc_id + " : Please book an Air Ticket for " + s_mail_to_display;
                            sDear = @"Dear Admin,";
                            sDetail = "Traveler has been request company to book an Air Ticket. To view travel details, click ";
                            sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + @"/" + page_name + "'>" + doc_id + "</a>";
                        }
                        else if (module_name == "admin_not_confirmed")
                        {
                            //admin กดส่ง 010_OB/LB/OT/LT : Please review an Air Ticket - [Title_Name of traveler] ให้ traveler มากรอก
                            //009_OB/LB/OT/LT : Please book an Air Ticket - [Title_Name of traveler] 
                            s_subject = doc_id + " : Please review an Air Ticket for " + s_mail_to_display;
                            sDear = @"Dear Admin,";
                            sDetail = "Traveler has been booked an Air Ticket. To view travel details, click ";
                            sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + "'>" + doc_id + "</a>";
                        }
                        else if (module_name == "admin_confirmed")
                        {
                            //admin ตรวจสเสร็จ กดส่ง 011_OB/LB/OT/LT : Air Ticket has been confirmed - [Title_Name of traveler] ให้ traveler ทราบ
                            s_subject = doc_id + " : Air Ticket has been confirmed for " + s_mail_to_display;
                            sDear = @"Dear " + s_mail_to_emp_name;
                            sDetail = "Your Air Ticket has been confirmed, To view travel details, click ";
                            sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + "'>" + doc_id + "</a>";
                            sDetail += "<br>";
                            sDetail += "Please check and notify us as soon as possible if any information is incorrect or if there is an additional requirement.";
                        }
                        else if (module_name == "traveler_review")
                        {
                            //กรณีที่ ทาง traveler submit ส่ง 032_OB/LB/OT/LT : Please review an Air Ticket as booked by [Title_Name of traveler] 
                            s_subject = doc_id + " : Please review an Air Ticket as booked by " + s_mail_to_display;
                            sDear = @"Dear Admin,";
                            sDetail = "Traveler has been booked an Air Ticket. To check details, click ";
                            sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + @"/" + page_name + "'>" + doc_id + "</a>";
                        }
                    }
                    else if (page_name == "accommodation")
                    {
                        if (module_name == "traveler_request")
                        {
                            //012_OB/LB/OT/LT : Please book an Acommodation - [Title_Name of traveler]
                            s_subject = doc_id + " : Please book an Accommodation for " + s_mail_to_emp_name;
                            sDear = @"Dear Admin,";
                            sDetail = "Traveler has been request company to book an Accommodation. To view travel details, click ";
                            sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + "'>" + doc_id + "</a>";
                        }
                        else if (module_name == "admin_not_confirmed")
                        {
                            //013_OB/LB/OT/LT : Please review an Accommodation - [Title_Name of traveler]
                            s_subject = doc_id + " : Please review an Accommodation for " + s_mail_to_display;
                            sDear = @"Dear Admin,";
                            sDetail = "Traveler has been booked an Accommodation. To view travel details, click ";
                            sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + "'>" + doc_id + "</a>";
                        }
                        else if (module_name == "admin_confirmed")
                        {
                            //014_OB/LB/OT/LT : Acommodation has been confirmed - [Title_Name of traveler]
                            s_subject = doc_id + " : Accommodation has been confirmed for " + s_mail_to_display;
                            sDear = @"Dear " + s_mail_to_emp_name;
                            sDetail = "Your Accommodation has been confirmed, To view travel details, click ";
                            sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + "'>" + doc_id + "</a>";
                            sDetail += "<br>";
                            sDetail += "Please check and notify us as soon as possible if any information is incorrect or if there is an additional requirement.";
                        }
                        else if (module_name == "traveler_review")
                        {
                            //กรณีที่ ทาง traveler submit ส่ง 032_OB/LB/OT/LT : Please review an Accommodation as booked by [Title_Name of traveler] 
                            s_subject = doc_id + " : Please review an Accommodation as booked by " + s_mail_to_display;
                            sDear = @"Dear Admin,";
                            sDetail = "Traveler has been booked an Accommodation. To check details, click ";
                            sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + @"/" + page_name + "'>" + doc_id + "</a>";
                        }
                    }
                    else if (page_name == "visa")
                    {
                        if (module_name == "sendmail_visa_requisition")
                        {
                            s_subject = doc_id + " : VISA Requisition for " + s_mail_to_display;
                            //Attachment : VISA Application (If any)
                            sDear = @"Dear All,";
                            sDetail = "You are require to submit VISA documents as follow. To view the details, click ";
                            sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + @"/" + page_name + "'>" + doc_id + "</a>";

                            //เพิ่มกรณีที่เดินทางไปมากกว่า 1 ประเทศ
                            sqlstr = @" select distinct te.dte_emp_id as emp_id, te.ct_id as country_id, ct.ct_name as country_name, ct.ctn_id as continent_id   
                                 from bz_doc_traveler_expense te
                                 inner join  bz_master_country ct on te.ct_id = ct.ct_id
                                 where te.dh_code = '" + doc_id + "' and te.dte_emp_id = '" + drempcheck[0].emp_id + "'";
                            if ((mail_list[i].country_in_doc + "") != "")
                            {
                                sqlstr += @" and te.ct_id in (" + mail_list[i].country_in_doc + ")";
                            }

                            DataTable dtcountry = new DataTable();
                            SetDocService.conn_ExecuteData(ref dtcountry, sqlstr);
                            if (dtcountry.Rows.Count > 0)
                            {

                                Boolean bcheckrows_white = true;
                                for (int j = 0; j < dtcountry.Rows.Count; j++)
                                {
                                    iNo = 1;
                                    int iItem = 1;

                                    sOtherList += " <div>";
                                    if (j == 0)
                                    {
                                        sOtherList += "<table width='1080' border='0' cellspacing='0' cellpadding='0' style='border-collapse:collapse;width:648.45pt;margin-left:35.7pt;'>";
                                    }
                                    else
                                    {
                                        sOtherList += "<table width='1080' border='0' cellspacing='0' cellpadding='0' style='border-collapse:collapse;width:648.45pt;margin-left:35.7pt;'>";
                                    }
                                    sOtherList += @"
                                                <tbody><tr height='33' style='height:19.8pt;'>
                                                <td width='87' style='width:52.25pt;height:19.8pt;background-color:#1F4E78;padding:0 5.4pt;border-style:solid dotted none solid;border-top-width:1pt;border-right-width:1pt;border-left-width:1pt;border-top-color:windowtext;border-right-color:#1F4E78;border-left-color:windowtext;'>
                                                <span style='background-color:#1F4E78;'>
                                                <div style='text-indent:15.5pt;margin:0;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4' color='white'><span style='font-size:15pt;'><b>Item</b></span></font></span></font></div>
                                                </span></td>
                                                <td width='812' style='width:487.75pt;height:19.8pt;background-color:#1F4E79;padding:0 5.4pt;border-style:solid dotted none none;border-top-width:1pt;border-right-width:1pt;border-top-color:windowtext;border-right-color:#1F4E78;'>
                                                <span style='background-color:#1F4E79;'>
                                                <div style='text-indent:15.5pt;margin:0;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4' color='white'><span style='font-size:15pt;'><b>
                                                Document List(" + dtcountry.Rows[j]["country_name"].ToString() + ")</b></span></font></span></font></div>" +
                                             @" </span></td>
                                                <td width='180' style='width:108.45pt;height:19.8pt;background-color:#1F4E79;padding:0 5.4pt;border-style:solid solid none none;border-top-width:1pt;border-right-width:1pt;border-top-color:windowtext;border-right-color:windowtext;'>
                                                <span style='background-color:#1F4E79;'>
                                                <div align='center' style='text-align:center;margin:0;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4' color='white'><span style='font-size:15pt;'><b>by</b></span></font></span></font></div>
                                                </span></td>
                                                </tr>";

                                    string sdocument_text = "";
                                    string sdocument_by = "";
                                    string sborder_bt = "none";

                                    continent_id = dtcountry.Rows[j]["continent_id"].ToString();
                                    country_id = dtcountry.Rows[j]["country_id"].ToString();

                                    swd = new SearchDocService();
                                    DataTable dtdesc = new DataTable();
                                    DataTable dtdocountries = swd.refdata_visa_docountries(continent_id, country_id, ref dtdesc);
                                    if (dtdocountries.Rows.Count > 0)
                                    {
                                        for (int k = 0; k < dtdocountries.Rows.Count; k++)
                                        {
                                            if (s_mail_attachments != "") { s_mail_attachments += "|"; }

                                            string file_name = @"Image\master visa docountries\mtvisacountries\" + dtdocountries.Rows[k]["filename"].ToString();
                                            string _FolderMailAttachments = System.Configuration.ConfigurationManager.AppSettings["FilePathServerWebservice"].ToString();
                                            string mail_attachments = _FolderMailAttachments + file_name;
                                            s_mail_attachments += mail_attachments;
                                        }
                                    }
                                    if (dtdesc.Rows.Count > 0)
                                    {
                                        string border_bottom_color = "";
                                        for (int k = 0; k < dtdesc.Rows.Count; k++)
                                        {
                                            if (k == (dtdesc.Rows.Count - 1)) { border_bottom_color = "border-bottom-color:windowtext;"; sborder_bt = "solid"; }
                                            //s_mail_body_in_form += "<br>" + (k + 1) + " " + dtdesc.Rows[k]["docountries_name"].ToString();
                                            sdocument_text = dtdesc.Rows[k]["docountries_name"].ToString();
                                            sdocument_by = dtdesc.Rows[k]["preparing_by"].ToString();

                                            sOtherList += @"<tr height='24' style='height:14.4pt;'>";
                                            if (j == dtcountry.Rows.Count) { sborder_bt = "solid"; }
                                            if (bcheckrows_white == true)
                                            {
                                                bcheckrows_white = false;
                                                sOtherList += @" <td width='87' style='width:52.25pt;height:14.4pt;padding:0 5.4pt;" + border_bottom_color + " border-style:none dotted " + sborder_bt + " solid;border-right-width:1pt;border-left-width:1pt;border-right-color:#1F4E78;border-left-color:windowtext;'>" +
                                                              @" <div align='center' style='text-align:center;margin:0;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4' color='black'><span style='font-size:15pt;'>" +
                                                              @"" + iItem + "</span></font></span></font></div></td>" +
                                                              @" <td width='812' style='width:487.75pt;height:14.4pt;padding:0 5.4pt;" + border_bottom_color + "border-style:none dotted " + sborder_bt + " none;border-right-width:1pt;border-right-color:#1F4E78;'> " +
                                                              @" <div style='margin:0 0 0 0.85pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4' color='black'><span style='font-size:15pt;'>" +
                                                              @"" + sdocument_text + " </span></font></span></font></div></td>" +
                                                              @" <td width='180' valign='bottom' nowrap='' style='width:108.45pt;height:14.4pt;padding:0 5.4pt;" + border_bottom_color + "border-style:none solid " + sborder_bt + " none;border-right-width:1pt;border-right-color:windowtext;'>" +
                                                              @"<div style='margin:0;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'>" +
                                                              @"" + sdocument_by + " </span></font></div></td>";
                                            }
                                            else
                                            {
                                                bcheckrows_white = true;
                                                sOtherList += @" <td width='87' style='background-color:#DDEBF7;width:52.25pt;height:14.4pt;padding:0 5.4pt;" + border_bottom_color + " border-style:none dotted " + sborder_bt + " solid;border-right-width:1pt;border-left-width:1pt;border-right-color:#1F4E78;border-left-color:windowtext;'>" +
                                                         @"<div align='center' style='text-align:center;margin:0;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4' color='black'><span style='font-size:15pt;'>" +
                                                         @"" + iItem + "</span></font></span></font></div></td>" +
                                                         @" <td width='812' style='background-color:#DDEBF7;width:487.75pt;height:14.4pt;padding:0 5.4pt;" + border_bottom_color + " border-style:none dotted " + sborder_bt + " none;border-right-width:1pt;border-right-color:#1F4E78;'>" +
                                                         @" <div style='margin:0 0 0 0.85pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4' color='black'><span style='font-size:15pt;'>" +
                                                         @"" + sdocument_text + "</span></font></span></font></div></td>" +
                                                         @" <td width='180' valign='bottom' nowrap='' style='background-color:#DDEBF7;width:108.45pt;height:14.4pt;padding:0 5.4pt;" + border_bottom_color + " border-style:none solid " + sborder_bt + " none;border-right-width:1pt;border-right-color:windowtext;'>" +
                                                         @" <div style='margin:0;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'>" +
                                                         @"" + sdocument_by + "</span></font></div></td>";
                                            }
                                            sOtherList += @"</tr>";
                                            iItem++;

                                        }
                                    }

                                    sOtherList += "</table><br>";
                                    sOtherList += "</div>";

                                }

                            }

                        }
                        else if (module_name == "sendmail_visa_employee_letter")
                        {
                            //016_OB/LB/OT/LT : Please review the Employee Letter - [Title_Name of traveler] 
                            //เป็น step ของ traveler update ข้อมูล ในหน้า visa
                            s_subject = doc_id + " : Please review the Employee Letter for " + s_mail_to_display;
                            sDear = @"Dear Admin,";
                            sDetail = "Please review the Employee Letter that has been requested as attached. To view travel details, click ";
                            sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + @"/" + page_name + "'>" + doc_id + "</a>";

                        }
                    }
                    else if (page_name == "passport")
                    {
                        //017_OB/LB/OT/LT : Please update Passport information - [Title_Name of traveler]
                        //ส่งตอน CAP Approve แล้วและตรวจสอบได้ว่าไม่มี valid passport เเละให้ส่ง E-Mail  
                    }
                    else if (page_name == "allowance" || page_name == "reimbursement")
                    {
                        if (page_name == "allowance")
                        {
                            //018_OB/LB/OT/LT : Please submit an i-Petty Cash in Allowance - [Title_Name of traveler] 
                            s_subject = doc_id + " : Please create Allowance in i-Petty Cash for " + s_mail_to_display;
                            //Attachment : Allowance Payment Form
                            sDear = @"Dear All,";
                            sDetail = "Please create Allowance in i-Petty Cash for Traveler as attached. To view travel details, click ";
                            sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + @"/" + page_name + "'>" + doc_id + "</a>";
                        }
                        else if (page_name == "reimbursement")
                        {
                            //019_OB/LB/OT/LT : Please create Reimbursement in i-Petty Cash for [Title_Name of traveler]
                            s_subject = doc_id + " : Please create Reimbursement in i-Petty Cash for " + s_mail_to_display;
                            //Attachment : Allowance Payment Form
                            sDear = @"Dear All,";
                            sDetail = "Please create Reimbursement in i-Petty Cash for Traveler as attached. To view travel details, click ";
                            sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + @"/" + page_name + "'>" + doc_id + "</a>";
                        }

                        msg_log += "Send Mail Page : Configuration";
                    }
                    else if (page_name == "travelinsurance" || (page_name == "isos" && module_name == "sendmail_isos_to_traveler"))
                    {
                        if (module_name == "claim form requisition")
                        {
                            //023_OB/LB/OT/LT : Travel Insurance Claim Form Requested - [Title_Name of traveler]
                            s_subject = doc_id + " : Travel Insurance Claim Form Requested - " + s_mail_to_emp_name;

                            sDear = @"Dear " + s_mail_to_emp_name + ",";
                            sDetail = "Please submit the following documents to receive further reimbursement. To view travel details, click ";
                            sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + @"/" + page_name + "'>" + doc_id + "</a>";

                            sOtherList = @"Procedures and forms for Travel Insurance Claim :";
                            sOtherList += @"<br>";
                            sOtherList += @"1. Complete and sign the Travel Insurance Claim Form (attached).";
                            sOtherList += @"<br>";
                            sOtherList += @"2. Original medical certificate and receipt";
                            sOtherList += @"<br>";
                            sOtherList += @"3. A copy of your ID, a copy of your passport, and a copy of your departure/arrival Thailand boarding pass. All are required to sign each paper.";
                            sOtherList += @"<br>";
                            sOtherList += @"4. Make a copy of the first page of the bank account. All are required to sign each paper. (In the case of employees who have been paid in advance)";

                            //string file_name = @"DocumentFile\Travelinsurance\Claim form Requisition.pdf";
                            string file_name = @"DocumentFile\Travelinsurance\New Travel Claim Form (003).pdf";
                            string _FolderMailAttachments = System.Configuration.ConfigurationManager.AppSettings["FilePathServerWebservice"].ToString();
                            string mail_attachments = _FolderMailAttachments + file_name;
                            s_mail_attachments += mail_attachments;

                        }
                        else
                        {
                            //รายละเอียดไฟล์แนบมาจาก SetTravelInsurance โดยเเยกเป็น module = "Sendmail to Broker" กับ module = "Sendmail to Traveler";
                            if (module_name == "sendmail_to_traveler" || module_name == "sendmail_isos_to_traveler")
                            {
                                //020_OB/LB/OT/LT : Please complete Travel Insurance form - [Title_Name of traveler]
                                s_subject = doc_id + " : Please complete Travel Insurance form - " + s_mail_to_emp_name;

                                sDear = @"Dear " + s_mail_to_emp_name + ",";
                                sDetail = "Your require to complete a Travel Insurance form. To view travel details, click ";
                                sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + @"/" + page_name + "'>" + doc_id + "</a>";

                                sOtherList = @"Note : Your also require to update passport for Travel Insurance application. To view travel details, click ";
                                sOtherList += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + @"/passport'>" + doc_id + "</a>";
                            }
                            else if (module_name == "sendmail_to_insurance")
                            {
                                //021_OB/LB/OT/LT : Please submit Travel Insurance Certificate - [Title_Name of traveler]
                                s_subject = doc_id + " : Please submit Travel Insurance Certificate - " + s_mail_to_display;
                                //Attachment : Travel Insurance Form and Passport

                                sDear = @"Dear " + s_mail_to_display + ",";//Dear [Title_Name of Broker],
                                sDetail = "To requested Travel Insurance Certificate, please find a Travel Insurance form and a copy passport as attached.";

                                #region เพิ่มแนบไฟล์ Passport    
                                sqlstr = @"
                                select distinct null as doc_id  ,a.emp_id
                                , a.id 
                                , a.id_level_1, a.id_level_2
                                , a.path
                                , a.file_name as filename
                                , a.page_name as pagename
                                , a.action_name as actionname
                                , a.path || a.file_name as fullname
                                , (case when u.usertype = 2 then u.enfirstname else nvl(u.entitle, '')|| ' ' || u.enfirstname || ' ' || u.enlastname  end ) as modified_by
                                , to_char(case when a.update_date is null then a.create_date else a.update_date end,'dd MON rrrr') as modified_date
                                , (case when a.id is null then 'insert' else 'update' end) action_type, 'false' as action_change 
                                , nvl(active_type,'false') as active_type
                                from bz_doc_img a
                                left join vw_bz_users u on (case when a.update_by is null then a.create_by else a.update_by end) = u.employeeid 
                                where a.status = 1 and lower(a.page_name) = lower('passport')  
                                and (a.emp_id,a.id_level_1) in (select emp_id,id from bz_doc_passport ad  where  (ad.default_type is not null and ad.default_type = 'true')    )
                                and a.emp_id = '" + emp_id + "'";
                                sqlstr += " order by a.id ";
                                DataTable dtimg = new DataTable();
                                conn = new cls_connection();
                                if (SetDocService.conn_ExecuteData(ref dtimg, sqlstr) == "")
                                {
                                    //xxxx.jpg|vvvv.jpg 
                                    if (dtimg.Rows.Count > 0)
                                    {
                                        //s_mail_body_in_form += "<br>Passport file : ";
                                        for (int k = 0; k < dtimg.Rows.Count; k++)
                                        {
                                            string emp_id_def = dtimg.Rows[k]["emp_id"].ToString();
                                            if (s_mail_attachments != "") { s_mail_attachments += "|"; }

                                            string doc_folder = "personal";
                                            string file_name = @"Image\" + doc_folder + @"\passport\" + emp_id_def + @"\" + dtimg.Rows[k]["filename"].ToString();
                                            string _FolderMailAttachments = System.Configuration.ConfigurationManager.AppSettings["FilePathServerWebservice"].ToString();
                                            string mail_attachments = _FolderMailAttachments + file_name;
                                            s_mail_attachments += mail_attachments; 
                                            //s_mail_body_in_form += "<br>Emp ID: " + dtimg.Rows[k]["emp_id"].ToString() + " <br>File Name: " + dtimg.Rows[k]["filename"].ToString();
                                        }
                                    }
                                }
                                #endregion เพิ่มแนบไฟล์ Passport   
                            }
                            else if (module_name == "sendmail_to_been_completed")
                            {
                                //022_OB/LB/OT/LT : Travel Insurance Certificate has been completed - [Title_Name of traveler]
                                s_subject = doc_id + " : Travel Insurance Certificate has been completed - " + s_mail_to_emp_name;
                                //Attachment : Travel Insurance Certificate
                                sDear = @"Dear " + s_mail_to_emp_name + ",";
                                sDetail = "A Travel Insurance Certificate has been granted. To view Insurance Coverage or travel details, click ";
                                sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + @"/" + page_name + "'>" + doc_id + "</a>";

                                sOtherList = @"Thai Oil Public Company Limited partners with International SOS, the leading medical assistance, international healthcare and security assistance company.";
                                sOtherList += @"<br>";
                                sOtherList += @"So if you need a medical referral, lose your medication, seek pre-travel advice or experience a medical or security crisis or";
                                sOtherList += @"<br>";
                                sOtherList += @"prepare yourself by browsing through their various medical and security online tools and signing up for our alerts as follow;";
                                sOtherList += @"<br><br>";
                                sOtherList = @"<b>";
                                sOtherList += @"Thai Oil Public Company limited - MEMBERSHIP ID: 03AYCA096535";
                                sOtherList += @"</b>";
                                sOtherList += @"<br>";
                                sOtherList += @"Website : <a href='https://www.internationalsos.com/Members_Home/login/clientaccess.cfm?custno=03AYCA096535'>https://www.internationalsos.com/Members_Home/login/clientaccess.cfm?custno=03AYCA096535</a>";
                                sOtherList += @"<br>";
                                sOtherList += @"Application : Download the International SOS Assistance App via iOS App Store, Google Play";
                                sOtherList += @"<br>";
                                sOtherList += @"To view more information about International SOS, click ";
                                sOtherList += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + @"/" + "isos" + "'>" + doc_id + "</a>";

                            }
                        }
                    }
                    else if (page_name == "isos")
                    {
                        if (module_name == "sendmail_isos_to_broker")
                        {
                            //024_OB/LB/OT/LT : Please update traveler list of International SOS Record  
                            s_subject = doc_id + " : Please update traveler list of International SOS Record";

                            sDear = @"Dear " + s_mail_to_display + ",";
                            sDetail = "Please update traveler list of International SOS Record as follow ;";

                            #region ISOS New Record
                            sqlstr = @" select distinct a.id as no
                                     , a.emp_id
                                     , case when nvl(b.userid,'') = '' then (nvl(b.entitle, '')|| ' ' || b.enfirstname || ' ' || b.enlastname)  else (nvl(a.isos_emp_title, '')|| ' ' || a.isos_emp_name || ' ' || a.isos_emp_surname) end userdisplay 
                                     , case when nvl(b.userid,'') = '' then nvl(b.orgname, '') else (nvl(a.isos_emp_section, '')|| '/' || a.isos_emp_department || '/' || a.isos_emp_function) end division 
                                    from bz_doc_isos_record a 
                                    left join vw_bz_users b on a.emp_id = b.userid
                                    where a.emp_id in (" + emp_id_select + " ) and  substr(a.year,3,2) = substr('" + doc_id + "',3,2) order by a.id ";
                            DataTable dtisos_record = new DataTable();
                            SetDocService.conn_ExecuteData(ref dtisos_record, sqlstr);
                            if (dtisos_record.Rows.Count > 0)
                            {
                                sOtherList = @"<table border='1' cellspacing='0' cellpadding='0' style='border-collapse:collapse;margin-left:35.7pt;border-style:none;'>
                                            <tbody><tr height='37' style='height:22.7pt;'>
                                            <td width='135' style='width:81pt;height:22.7pt;background-color:#D5DCE4;padding:0 5.4pt;border:1pt solid #BFBFBF;'>
                                            <span style='background-color:#D5DCE4;'>
                                            <div align='center' style='text-align:center;margin:0;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'><b>Running </b></span></font><font face='Browallia New,sans-serif' size='4' color='black'><span style='font-size:15pt;'><b>No.</b></span></font></span></font></div>
                                            </span></td>
                                            <td width='405' style='width:243pt;height:22.7pt;background-color:#D5DCE4;padding:0 5.4pt;border-width:1pt;border-style:solid solid solid none;border-color:#BFBFBF;'>
                                            <span style='background-color:#D5DCE4;'>
                                            <div style='margin:0 0 0 36pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4' color='black'><span style='font-size:15pt;'><b>Name of Traveler</b></span></font></span></font></div>
                                            </span></td>
                                            <td width='255' valign='top' style='width:153pt;height:22.7pt;background-color:#D5DCE4;padding:0;border-width:1pt;border-style:solid solid solid none;border-color:#BFBFBF;'>
                                            <span style='background-color:#D5DCE4;'>
                                            <div align='center' style='text-align:center;margin:0 0 0 4.5pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4' color='black'><span style='font-size:15pt;'><b>Employee ID</b></span></font></span></font></div>
                                            </span></td>
                                            <td width='255' valign='top' style='width:153pt;height:22.7pt;background-color:#D5DCE4;padding:0;border-width:1pt;border-style:solid solid solid none;border-color:#BFBFBF;'>
                                            <span style='background-color:#D5DCE4;'>
                                            <div style='margin:0 0 0 36pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4' color='black'><span style='font-size:15pt;'><b>Organization Unit</b></span></font></span></font></div>
                                            </span></td>
                                            </tr>";
                                for (int m = 0; m < dtisos_record.Rows.Count; m++)
                                {
                                    sOtherList += @" <tr height='4' style='height:2.85pt;'>";
                                    sOtherList += @" <td width='135' style='width:81pt;height:2.85pt;padding:0 5.4pt;border-width:1pt;border-style:none solid solid solid;border-color:#BFBFBF;'>
                                                <div align='center' style='text-align:center;margin:0;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>
                                                " + dtisos_record.Rows[m]["no"] + "</span></font></span></font></div></td>";//1) //Running No.
                                    sOtherList += @" <td width='405' style='width:243pt;height:2.85pt;padding:0 5.4pt;border-style:none solid solid none;border-right-width:1pt;border-bottom-width:1pt;border-right-color:#BFBFBF;border-bottom-color:#BFBFBF;'>
                                                <div style='margin:0 0 0 36pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>
                                                " + dtisos_record.Rows[m]["userdisplay"] + "</span></font></span></font></div></td>";//Name of Traveler
                                    sOtherList += @" <td width='135' style='width:81pt;height:2.85pt;padding:0 5.4pt;border-width:1pt;border-style:none solid solid solid;border-color:#BFBFBF;'>
                                                <div align='center' style='text-align:center;margin:0;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>
                                                " + dtisos_record.Rows[m]["emp_id"] + "</span></font></span></font></div></td>";//Employee ID 
                                    sOtherList += @" <td width='135' style='width:81pt;height:2.85pt;padding:0 5.4pt;border-width:1pt;border-style:none solid solid solid;border-color:#BFBFBF;'>
                                                <div align='center' style='text-align:center;margin:0;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>
                                                " + dtisos_record.Rows[m]["division"] + "</span></font></span></font></div></td>";//Organization Unit
                                    sOtherList += @" </tr>";
                                }
                                sOtherList += "</body>";
                                sOtherList += "</table>";
                            }
                            #endregion ISOS New Record
                        }
                    }
                    else if (page_name == "transportation")
                    {
                        //#E-MAIL : 027_OB/LB/OT/LT : Private Car Requisition  - [Title_Name of traveler]
                        s_subject = doc_id + " : Private Car Requisition - " + s_mail_to_emp_name;

                        sDear = @"Dear " + s_mail_to_emp_name + ",";

                        sDetail = @"As you requested to use a Private Car to business travel, Please complete as follows step;";
                        sDetail += @"To view travel details, click";
                        sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + @"/" + page_name + "'>" + doc_id + "</a>";

                        sOtherList = @"<div style='margin:0 0 0 36pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4'>";
                        sOtherList += @"<span style='font-size:15pt;'>Traveler Name : " + drempcheck[0].userDisplay + "</span>";
                        sOtherList += @"<span style='font-size:15pt;margin:0 0 0 36pt;'>" + drempcheck[0].emp_id + "</span>";
                        sOtherList += @"<span style='font-size:15pt;margin:0 0 0 36pt;'>" + drempcheck[0].division + "</span>";
                        sOtherList += @"</font></span></font></div>";
                        sOtherList += @"<br>";
                        sOtherList += @"<div style='margin:0 0 0 36pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4'>
                                        <span style='font-size:15pt;'>The steps as follows in requesting a private car for business travel; </span></font></span></font></div>";
                        sOtherList += @"<div style='text-indent:-18pt;margin:0 0 0 58.5pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>1)</span></font><font face='Browallia New,sans-serif'><span style='font-size:;'>&nbsp;&nbsp;&nbsp;&nbsp;
                                        </span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'><u>Complete a Personal car application form</u></span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'> and seeking </span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'><u>approval
                                        from Vice President</u></span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'> with signature before travel date with </span></font></span></font></div>";
                        sOtherList += @"<div style='text-indent:-18pt;margin:0 0 0 58.5pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>2)</span></font><font face='Browallia New,sans-serif'><span style='font-size:;'>&nbsp;&nbsp;&nbsp;&nbsp;
                                        </span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>Attached (1) a copy of </span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'><u>(insurance) policy or the motor insurance schedule</u></span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>
                                        </span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;' lang='th'>สำเนากรมธรรม์ประกันภัยชั้น </span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>1 </span></font></span></font></div>";
                        sOtherList += @"<div style='margin:0 0 0 58.5pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>and (2) a </span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'><u>copy
                                        of Employee or Spouse of Car Registration</u></span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'> (</span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;' lang='th'>สำเนาทะเบียนรถ ชื่อผู้เข้าอบรมหรือคู่สมรส</span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>)
                                        (2)</span></font></span></font></div>";
                        sOtherList += @"<div style='text-indent:-18pt;margin:0 0 0 58.5pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>3)</span></font><font face='Browallia New,sans-serif'><span style='font-size:;'>&nbsp;&nbsp;&nbsp;&nbsp;
                                        </span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'><u>Process to reimbursement via I-Petty</u></span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'> cash with attached documents 1)
                                        &amp; 2)</span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;' lang='th'> </span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>with follow procedures; </span></font></span></font></div>
                                        <div style='text-indent:-18pt;margin:0 0 0 76.5pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Wingdings' size='4'><span style='font-size:15pt;'>§</span></font><font face='Wingdings'><span style='font-size:;'>&nbsp; </span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>7.8
                                        THB/Kilometers for total distance or</span></font></span></font></div>
                                        <div style='text-indent:-18pt;margin:0 0 0 76.5pt;'><font face='Calibri,sans-serif' size='2'><span style='font-size:11pt;'><font face='Wingdings' size='4'><span style='font-size:15pt;'>§</span></font><font face='Wingdings'><span style='font-size:;'>&nbsp; </span></font><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>1,950
                                        THB pay for Roundtrip to Bangkok Metropolis and Vicinity.</span></font></span></font></div>";
                        sOtherList += @"";
                    }
                    else if (page_name == "travelexpense")
                    {
                        if (module_name == "sendmail_to_sap")
                        {
                            //029_OB/LB : Business Travel Expenses has been updated and sent to SAP
                            s_subject = doc_id + " : Business Travel Expenses has been updated and sent to SAP.";

                            sDear = @"Dear All,";
                            sDetail = "Business Travel Expenses has been updated and sent to SAP. To view details, click ";
                            sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + @"/" + page_name + "'>" + doc_id + "</a>";
                        }
                        else if (module_name == "tripcancelled")
                        {
                            //030_OB/LB : The request for business travel has been cancelled
                            s_subject = doc_id + " : The request for business travel has been cancelled.";
                            //Attached : Approval / Output form

                            sDear = @"Dear All,";
                            sDetail = "The request for business travel has been cancelled. To view the approval details, click ";
                            sDetail += "<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + @"/" + page_name + "'>" + doc_id + "</a>";
                        }
                    }
                    else if (page_name == "feedback")
                    {
                        //031_OB/LB/OT/LT : Please complete Business Travel Feedback 
                        s_subject = doc_id + " : Please complete Business/Training Travel Feedback.";
                        //Attached : Approval / Output form

                        sDear = @"Dear " + s_mail_to_emp_name + ",";

                        sDetail = @"Your experience is important to us, please complete this feedback survey, click ";
                        sDetail += @"<a href='" + (LinkLoginPhase2).Replace("###", doc_id) + @"/" + page_name + "'>" + doc_id + "</a>";
                        sDetail += @"<br>";
                        sDetail += @"All surveys are confidential and are used for improving our service only.";
                    }
                    #endregion ข้อมูลที่ต้องส่งใน mail ของแต่ละ module 

                    #region set detail mail attachments
                    if ((page_name == "airticket" && module_name == "admin_confirmed")
                         || (page_name == "accommodation" && module_name == "admin_confirmed")
                         || (page_name == "travelinsurance" && module_name == "sendmail_to_been_completed")
                         || page_name == "allowance" || page_name == "reimbursement"
                         || page_name == "airticket" || page_name == "accommodation"
                         || page_name == "transportation")
                    {
                        //e-mail ต้องส่งไฟล์แนบกับรายละเอียด ของ  Allowance/Reimbursement และเอกสาร approved 
                        // แนบไฟล์ excel โดยไฟล์จะถูก genareate จาก ws Ebiz_App   
                        //d:\Ebiz2\EBiz_Webservice\
                        string _FilePathServerWebservice = System.Configuration.ConfigurationManager.AppSettings["FilePathServerWebservice"].ToString();
                        string _FilePathServerApp = System.Configuration.ConfigurationManager.AppSettings["FilePathServerApp"].ToString();

                        msg_log += "Send Mail Page : end Configuration";
                        var xerrorfile = "";
                        try
                        {
                            string xfile = mail_list[i].mail_attachments.ToString();
                            if (xfile != "")
                            {
                                s_mail_attachments = "";
                                string[] xsplit_file = xfile.Split(';');
                                for (int k = 0; k < xsplit_file.Length; k++)
                                {
                                    string xname = "";
                                    string xpath = xsplit_file[k].ToString();
                                    if (xpath == "") { continue; }
                                    string[] xsplit_path = xpath.Split('/');
                                    if (xsplit_path.Length > 4)
                                    {
                                        //"file_report": "http://TBKC-DAPPS-05.thaioil.localnet/ebiz_ws/ExportFile/OB20090026/allowance/00001109/Allowance_Payment_Test.xlsx",
                                        //"file_travel_report": "http://tbkc-dapps-05.thaioil.localnet/Ebiz2/temp/EBIZ_TRAVEL_REPORT_202106151443.xlsx",


                                        //<add key = "FilePathServerWebservice" value = "d:\Ebiz2\EBiz_Webservice\" />
                                        //<add key = "FilePathServerApp" value = "d:\Ebiz2\Ebiz_App\" />
                                        //มีแค่ 2 ไฟล์ fix ได้
                                        if (page_name == "allowance" || page_name == "reimbursement")
                                        {
                                            if (k == 0)
                                            {
                                                xname = _FilePathServerWebservice;
                                            }
                                            else if (k == 1)
                                            {
                                                xname = _FilePathServerApp;
                                            }
                                            else { xname = _FilePathServerWebservice; }
                                        }
                                        else { xname = _FilePathServerWebservice; }
                                        for (int m = 4; m < xsplit_path.Length; m++)
                                        {
                                            if (m > 4) { xname += @"\"; }
                                            xname += xsplit_path[m].ToString();
                                        }
                                        //ตรวจสอบ file บน server
                                        if (File.Exists(xname))
                                        {
                                            if (s_mail_attachments != "") { s_mail_attachments += "|"; }
                                            s_mail_attachments += xname;
                                        }
                                        else
                                        {
                                            //กรณีหลุดจากมีแค่ 2 ไฟล์ fix ได้
                                            xname = _FilePathServerWebservice;
                                            for (int m = 4; m < xsplit_path.Length; m++)
                                            {
                                                if (m > 4) { xname += @"\"; }
                                                xname += xsplit_path[m].ToString();
                                            }
                                            //ตรวจสอบ file บน server
                                            if (File.Exists(xname))
                                            {
                                                if (s_mail_attachments != "") { s_mail_attachments += "|"; }
                                                s_mail_attachments += xname; 
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        catch (Exception exfile) { xerrorfile = exfile.Message.ToString() + "-->irow :" + i + "-->file :" + s_mail_attachments; }

                        msg_log += "Send Mail Page : mail_attachments " + xerrorfile;
                    }

                    else if (page_name == "visa" && module_name == "sendmail_visa_employee_letter")
                    {
                        try
                        {
                            string xfile = mail_list[i].mail_attachments.ToString();
                            s_mail_attachments = "";
                            string[] xsplit_file = xfile.Split(';');
                            for (int k = 0; k < xsplit_file.Length; k++)
                            {
                                string xname = xsplit_file[k].ToString();
                                //ตรวจสอบ file บน server
                                if (File.Exists(xname))
                                {
                                    if (s_mail_attachments != "") { s_mail_attachments += "|"; }
                                    s_mail_attachments += xname;
                                }
                            }
                        }
                        catch { }
                    }
                    #endregion set detail mail attachments

                    #region set detail mail 2
                    sTitle = "Title : " + drempcheck[0].travel_topic + "";
                    sBusinessDate = "Business Date : " + drempcheck[0].business_date + "";
                    sLocation = "Location : " + drempcheck[0].country_city + "";


                    //Traveler List
                    if (page_name == "isos" || page_name == "transportation") { }
                    else
                    {
                        iNo = 1;
                        sTravelerList = "<table>";
                        foreach (var item in drempcheck)
                        {
                            sTravelerList += " <tr>";
                            sTravelerList += " <td><font face='Browallia New,sans-serif' size='4'><span style='margin:0 0 0 20pt;font-size:15pt;'>" + iNo + ") " + item.userDisplay + "</span></font></td>";//1) [Title_Name of traveler] 
                            sTravelerList += " <td><font face='Browallia New,sans-serif' size='4'><span style='margin:0 0 0 36pt;font-size:15pt;'>" + item.emp_id + "</span></font></td>";//Emp. ID
                            sTravelerList += " <td><font face='Browallia New,sans-serif' size='4'><span style='margin:0 0 0 36pt;font-size:15pt;'>" + item.division + "</span></font></td>";//SEC./DEP./FUNC. 
                            sTravelerList += " </tr>";
                            iNo++;
                        }
                        sTravelerList += "</table>";
                    }

                    #endregion set detail mail 2

                    #region set mail 
                    s_mail_body = @"<span lang='en-US'>";
                    s_mail_body += "<div>";
                    s_mail_body += "     <div style='margin:0;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>";
                    s_mail_body += "     " + sDear + "</span></font></div>";
                    s_mail_body += "     <br>";
                    s_mail_body += "     <div style='margin:0 0 0 36pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>";
                    s_mail_body += "     " + sDetail + "</span></font></div>";
                    s_mail_body += "     <br>";
                    s_mail_body += "     <div style='margin:0 0 0 36pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>";
                    s_mail_body += "     " + sTitle + "</span></font></div>";
                    s_mail_body += "     <div style='margin:0 0 0 36pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>";
                    s_mail_body += "     " + sBusinessDate + "</span></font></div>";
                    s_mail_body += "     <div style='margin:0 0 0 36pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>";
                    s_mail_body += "     " + sLocation + "</span></font></div>";
                    s_mail_body += "     <br>";
                    if (sTravelerList != "")
                    {
                        s_mail_body += "     <div style='margin:0 0 0 36pt;'><font face='Browallia New,sans-serif' size='4'>";
                        s_mail_body += "     <span style='font-size:15pt;'>Traveler Name : " + sTravelerList + "</span></font></div>";
                        s_mail_body += "     <br>";
                    }
                    if (sOtherList != "")
                    {
                        if (page_name == "isos" || (page_name == "visa" && module_name == "sendmail_visa_requisition"))
                        {
                            s_mail_body += "     <div style='margin:0 0 0 0;'><font face='Browallia New,sans-serif' size='4'>";
                        }
                        else
                        {
                            s_mail_body += "     <div style='margin:0 0 0 36pt;'><font face='Browallia New,sans-serif' size='4'>";
                        }
                        s_mail_body += "     <span style='font-size:15pt;'>" + sOtherList + "</span></font></div>";
                        s_mail_body += "     <br>";
                    }
                    if (module_name == "tripcancelled")
                    {
                        if (s_mail_body_in_form != "")
                        {
                            s_mail_body += "     <div style='margin:0 0 0 36pt;'><font face='Browallia New,sans-serif' size='4'>";
                            s_mail_body += "     <span style='font-size:15pt;'>" + s_mail_body_in_form + "</span></font></div>";
                            s_mail_body += "     <br>";
                        }
                    }
                    s_mail_body += "     <div style='margin:0 0 0 36pt;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>";
                    s_mail_body += "     If you have any question please contact Business Travel Services Team (Tel. " + Tel_Services_Team + ").";
                    s_mail_body += "     <br>";
                    s_mail_body += "     For the application assistance, please contact PTT Digital Call Center (Tel. " + Tel_Call_Center + ").";
                    s_mail_body += "     </span></font></div>";

                    s_mail_body += "     <div style='margin:0;'><font face='Browallia New,sans-serif' size='4'><span style='font-size:15pt;'>";
                    s_mail_body += "     <br>";
                    s_mail_body += "     Best Regards,";
                    s_mail_body += "     <br>";
                    s_mail_body += "     Business Travel Services Team (PMSV)";
                    s_mail_body += "     </span></font></div>";

                    s_mail_body += "</div>";
                    s_mail_body += "</span>";

                    //s_mail_body += "<br>";
                    //s_mail_body += "test module_name_select : " + module_name + "";
                    //s_mail_body += "<br>";
                    //s_mail_body += "test page_name : " + page_name + "";
                    #endregion set mail

                    msg_log += "Send Mail Page :esw.send_mail";
                    SendEmailService esw = new SendEmailService();
                    ret = esw.send_mail(s_mail_to, s_mail_cc, s_subject, s_mail_body, s_mail_attachments);

                    msg_log += "Send Mail Page : ret : " + ret;

                    if (ret == "")
                    {
                        mail_list[i].mail_status = "true";
                        mail_list[i].mail_remark = "Send mail Success.";
                        mail_list[i].mail_remark += msg_log;
                        ret = "true";
                    }
                    else
                    {
                        mail_list[i].mail_status = "false";
                        mail_list[i].mail_remark = "Send mail Error." + "to:" + s_mail_to + " cc:" + s_mail_cc;
                        mail_list[i].mail_remark += msg_log;
                        ret = "false";
                    }

                }
            }
            catch (Exception ex) { ret += "Send Mail Page error Message:" + ex.Message.ToString(); }
            return ret;
        }

        public string SendMailInContact(ref List<mailselectList> mail_list)
        {

            string s_mail_to = (mail_list[0].mail_to + "").ToString();
            string s_mail_cc = (mail_list[0].mail_cc + "").ToString();
            string s_subject = "";
            string s_mail_body = "";
            string s_mail_body_in_form = (mail_list[0].mail_body_in_form + "").ToString();
            string s_mail_attachments = "";
            string s_mail_to_emp_name = "All,";
            string module = mail_list[0].module;

            s_subject = "E-Biz : Test Send E-Mail Contact As";
            s_mail_body = @"Dear " + s_mail_to_emp_name;
            s_mail_body += @"<br><br>";
            s_mail_body += s_mail_body_in_form;
            s_mail_body += @"<br><br>";
            s_mail_body += @"<br><br>Regards, 
                                <br>
                                <br>System Administration Officer 
                                <br><br>Tel : 038-359000  Ext 20104";

            SendEmailService esw = new SendEmailService();
            ret = esw.send_mail(s_mail_to, s_mail_cc, s_subject, s_mail_body, s_mail_attachments);
            if (ret == "") { ret = "true"; }
            return ret;
        }


        #endregion In Page
    }
}