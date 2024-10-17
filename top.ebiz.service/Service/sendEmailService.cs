
using System.Data;
using top.ebiz.service.Models;

using System.Configuration;

using Microsoft.Exchange.WebServices.Data;

namespace top.ebiz.service.Service
{
    public class sendEmailService
    {
        private readonly IConfiguration _configuration;

        // Constructor Injection เพื่อดึง IConfiguration เข้ามาในคลาสนี้
        public sendEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string submitMail(docEmailModel value)
        {
            string msg = "";
            try
            {
                if (value == null)
                    return "false";

                string s_subject = "";
                string s_body = "";
                string s_mail_to = value.mail_to ?? "";
                string s_mail_cc = "";
                string s_mail_from = "";

                if (value.action == "p1_submit_admin")
                {
                    s_subject = value.doc_no + " : Please submit business travel document request.";
                }
                else if (value.action == "p1_submit_initiator")
                {
                    s_subject = value.doc_no + " : Please initiate business travel document workflow request.";
                }

                sendEmailModel data = new sendEmailModel();
                data.mail_subject = s_subject;
                data.mail_body = s_body;
                data.mail_to = s_mail_to;
                data.mail_cc = s_mail_cc;
                data.mail_from = s_mail_from;
                sendMail(data);
            }
            catch (Exception ex)
            {

            }
            return msg;

        }
        public string sendMail(sendEmailModel value)
        {

            String s_mail_to = value.mail_to + "";
            String s_mail_cc = value.mail_cc + "";
            String s_subject = value.mail_subject + "";
            String s_mail_body = value.mail_body + "";
            String s_mail_attachments = value.mail_attachments + "";

            s_mail_to = string.Join(";", s_mail_to.Split(';').Where(x => x != string.Empty).Distinct().ToArray());
            s_mail_cc = string.Join(";", s_mail_cc.Split(';').Where(x => x != string.Empty).Distinct().ToArray());

            write_log_mail("71-sendMail start", "s_subject:" + s_subject + "  =>email to:" + s_mail_to.ToString() + "  =>email cc:" + s_mail_cc.ToString());

            String msg_mail = "";
            String msg_mail_file = "";

            string mail_server = _configuration["MailSettings:MailSMTPServer"] ?? "";
            string mail_from = _configuration["MailSettings:MailFrom"]?? "";
            string mail_test = _configuration["MailSettings:MailTest"]?? "";
            string mail_font = _configuration["MailSettings:MailFont"] ?? "";
            string mail_fontsize = _configuration["MailSettings:MailFontSize"] ?? "";

            string mail_user = _configuration["MailSettings:MailUser"] ?? "";
            string mail_password = _configuration["MailSettings:MailPassword"] ?? "";
            bool SendAndSaveCopy = Convert.ToBoolean(_configuration["MailSettings:MailSendAndSaveCopy"]);


            if (mail_test != "")
            {
                s_mail_body += "</br>ข้อมูล mail to: " + s_mail_to + "</br></br>ข้อมูล mail cc: " + s_mail_cc;

                s_mail_to = mail_test;
                s_mail_cc = mail_test;
            }

            ExchangeService service = new ExchangeService();
            service.Credentials = new WebCredentials(mail_user, mail_password);
            service.TraceEnabled = true;


            EmailMessage email = new EmailMessage(service);
            service.AutodiscoverUrl(mail_from, RedirectionUrlValidationCallback);
            email.From = new EmailAddress("Mail Display ใส่ไม่มีผล", mail_from);

            var email_to = s_mail_to.Split(';');

            for (int i = 0; i < email_to.Length; i++)
            {
                string _mail = (email_to[i].ToString()).Trim();
                if (_mail != "")
                {
                    // Mail To จะต้องใช้วิธี Loop และห้ามใส่ ; ต่อท้าย
                    email.ToRecipients.Add(_mail);
                }
            }
            var email_cc = s_mail_cc.Split(';');
            for (int i = 0; i < email_cc.Length; i++)
            {
                string _mail = (email_cc[i].ToString()).Trim();
                if (_mail != "")
                {
                    //Mail CC จะต้องใช้วิธี Loop และห้ามใส่ ; ต่อท้าย
                    email.CcRecipients.Add(_mail);
                }
            }

            //Subject
            if (mail_test != "") { s_subject = "(DEV)" + s_subject; }
            email.Subject = s_subject;

            //Body
            //เพิ่ม กำหนด font  
            if (mail_font == "") { mail_font = "Cordia New"; }
            if (mail_fontsize == "") { mail_fontsize = "18"; }
            s_mail_body = "<html><div style='font-size:" + mail_fontsize + "px; font-family:" + mail_font + ";'>" + s_mail_body + "</div></html>";
            email.Body = new MessageBody(BodyType.HTML, s_mail_body);

            try
            {
                msg_mail_file = "";
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
            }
            catch (Exception ex)
            {
                msg_mail_file = ex.ToString();
            }

            try
            {
                write_log_mail("72-sendMail send", (msg_mail_file != "" ? "=>msg_mail_file:" + msg_mail_file : ""));
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
                write_log_mail("79-sendMail end", "");
            }
            catch (Exception ex)
            {
                msg_mail = ex.ToString();
                write_log_mail("78-sendMail end ", "error:" + msg_mail + " => msg_mail_file:" + msg_mail_file);
            }

            return msg_mail;
        }

        private bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            throw new NotImplementedException();
        }


        #region auwat 20221026 1435 เพิ่มเก็บ log การส่ง mail => เนื่องจากมีกรณที่กดปุ่มแล้ว mail ไม่ไป
        public void write_log_mail(string step, string data_log)
        {
            try
            {
                logModel mLog = new logModel();
                mLog.module = "E-MAIL";
                mLog.tevent = step;//step
                mLog.ref_id = 0;
                mLog.data_log = data_log;
                logService.insertLog(mLog);
            }
            catch (Exception ex_write)
            {
                logModel mLog = new logModel();
                mLog.module = "E-MAIL";
                mLog.tevent = "write log Send Mail Service error";//step
                mLog.ref_id = 0;
                mLog.data_log = ex_write.Message.ToString();
                logService.insertLog(mLog);
            }
        }
        #endregion auwat 20221026 1435 เพิ่มเก็บ log การส่ง mail => เนื่องจากมีกรณที่กดปุ่มแล้ว mail ไม่ไป

    }
}