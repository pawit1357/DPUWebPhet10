using System.Net.Mail;
using System;
using System.Net.Mime;
using System.Collections;
using System.Net;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Configuration;
using DPUWebPhet10.Models;
using System.Globalization;
using System.Text.RegularExpressions;
namespace DPUWebPhet10.Utility
{
    public class Email
    {

        private const String Host = "smtp.gmail.com";
        private const String EmailUser = "petchyod.mon@gmail.com";
        private const String EmailPassword = "petch2013";
        private const int Port = 587;
        public static void SendEmail(TB_APPLICATION_SCHOOL school,String registerdPassword,String culture)
        {

            try
            {


                String templateName = (culture.ToUpper().Equals("TH")) ? "AccountDetail_th.html" : "AccountDetail_en.html";
                String emailSubject = (culture.ToUpper().Equals("TH")) ? "การลงทะเบียนเข้าร่วมการแข่งขัน" : "การลงทะเบียนเข้าร่วมการแข่งขัน";
                String displanName = (culture.ToUpper().Equals("TH")) ? "การแข่งขันภาษาจีนเพชรยอดมงกุฎ ครั้งที่ 12 (นานาชาติ)" : "การแข่งขันภาษาจีนเพชรยอดมงกุฎ ครั้งที่ 12  (นานาชาติ)";

                MailDefinition mailDefinition = new MailDefinition();
                mailDefinition.BodyFileName = "~/Utils/Email-Templates/" + templateName;
                mailDefinition.From = EmailUser;




                //Create a key-value collection of all the tokens you want to replace in your template...
                ListDictionary ldReplacements = new ListDictionary();
                ldReplacements.Add("<%School Name%>", school.SCHOOL_NAME);
                ldReplacements.Add("<%User%>", school.SCHOOL_EMAIL.Trim());
                ldReplacements.Add("<%Password%>", registerdPassword.Trim());

                string mailTo = string.Format("{0} <{1}>", school.SCHOOL_NAME, school.SCHOOL_EMAIL);
                MailMessage mailMessage = mailDefinition.CreateMailMessage(mailTo, ldReplacements, new System.Web.UI.Control());
                mailMessage.From = new MailAddress(EmailUser, displanName);
                mailMessage.IsBodyHtml = true;
                mailMessage.Subject = emailSubject;
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;

                // smtp settings
                System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient();
                {
                    smtpClient.Host = Host;
                    smtpClient.Port = Port;
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtpClient.Credentials = new NetworkCredential(EmailUser, EmailPassword);
                    smtpClient.Timeout = 20000;
                }
                //SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"].ToString(), 25);
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                //throw ex;
                Console.WriteLine(ex.Message);
            }
        }

        public static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
    }
}
