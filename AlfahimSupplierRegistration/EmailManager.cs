using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Configuration;
using System.Net;

namespace AlfahimSupplierRegistration.Models
{
    public class EmailManager
    {
        private String m_Subject; 
        private String m_Body; 
        private MailAddress MailSender; 
        private MailAddress MailReceiver;
        public String RefNo;
        public String Remakrs;


        public String SenderName;
        public String SenderEmail;
        public String ReceiverName;
        public String ReceiverEmail;

        public string Subject
        {
            get { return m_Subject; }
            set { m_Subject = value; }
        }

        public string Body
        {
            get { return m_Body; }
            set { m_Body = value; }
        }

        public void SendEmail(string ReceiverAddress, string DisplayName)
        {
            string SMTPEmailAddress = ConfigurationManager.AppSettings["SMTPEmailAddress"].ToString();
            System.Net.Mail.MailMessage newemail = new System.Net.Mail.MailMessage();
            MailReceiver = new System.Net.Mail.MailAddress(ReceiverAddress, DisplayName);
            MailSender = new System.Net.Mail.MailAddress(SMTPEmailAddress, "Administrator");
            newemail.From = MailSender;
            newemail.To.Add(MailReceiver);
            newemail.IsBodyHtml = true;
            newemail.Subject = m_Subject;
            newemail.Body = m_Body;
            newemail.Body += "<BR>" + "<BR>" + "Thanks" + "<BR>" + "";
            SendMail(newemail);
        }

        private void SendMail(System.Net.Mail.MailMessage MailMsg)
        {
            System.Net.Mail.SmtpClient newe = new System.Net.Mail.SmtpClient();
            newe.Host = ConfigurationManager.AppSettings["SMTP"].ToString(); 
            newe.Port = Convert.ToInt16(ConfigurationManager.AppSettings["Port"]);
            newe.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
            //newe.DeliveryMethod = SmtpDeliveryMethod.Network;
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseDefaultCredentials"]) == true)
            {
                newe.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SMTPEmailAddress"].ToString(), ConfigurationManager.AppSettings["SMTPassword"].ToString());
            }
            else
             {
                newe.UseDefaultCredentials = false;
             }

            
            newe.Timeout = 20000;
            try
            {
               newe.Send(MailMsg);
            }
            catch (Exception ex)
            {
              throw ex;
            }
        }
    }
}


