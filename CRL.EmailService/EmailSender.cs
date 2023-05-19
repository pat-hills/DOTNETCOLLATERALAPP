using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.FS;

namespace CRL.EmailService
{
    public class EmailSender
    {
       
        public bool onTestSite =false;
        public string onTestSiteReceipients { get; set; }
        public string onTestSiteSubjectPrefix { get; set; }
        public string alwaysCopy { get; set; }

        public string DefaultSubjectIfEmpty { get; set; }
        public string fromEmail { get; set; }
        public string fromName { get; set; }
        public string messageBody { get; set; }           
        public string smtpServer { get; set; }
        public int port { get; set; }
        public string host { get; set; }
        private string userName { get; set; }
        private string password { get; set; }
        private int maxAddresses { get; set; }
        private bool SentMailWithSMTPCredential = false;
        private bool EnableSSL = false;
        private EventLog eventLogEmail = new EventLog();
        public NetworkCredential smtpCredentials { get; set; }
        private List<FileUpload> mEmailAttachmentAttributeList;
        private ADOHealthMonitoringDataContext adoDataContext;
        public ADOHealthMonitoringDataContext ADODataContext
        {
            get
            {
                if (adoDataContext == null)
                    adoDataContext = new ADOHealthMonitoringDataContext();
                return adoDataContext;
            }
            set
            {
                adoDataContext = value;
            }
        }


        public EmailSender(EventLog _eventLogEmail)
        {
            fromEmail = ConfigurationManager.AppSettings["from"];
            host = ConfigurationManager.AppSettings["host"];
            port = Convert.ToInt16( ConfigurationManager.AppSettings["port"]);
            userName = ConfigurationManager.AppSettings["userName"];
            password = ConfigurationManager.AppSettings["password"];
            alwaysCopy = ConfigurationManager.AppSettings["alwaysCopy"];
            maxAddresses = ConfigurationManager.AppSettings["maxAddresses"] != null ? int.Parse(ConfigurationManager.AppSettings["maxAddresses"]) : 0;
            onTestSite = ConfigurationManager.AppSettings["onTestSite"] != null ? Boolean.Parse (ConfigurationManager.AppSettings["onTestSite"]): false;
            onTestSiteReceipients =  ConfigurationManager.AppSettings["onTestSiteReceipients"]!=null ? ConfigurationManager.AppSettings["onTestSiteReceipients"] :"";
            onTestSiteSubjectPrefix = ConfigurationManager.AppSettings["onTestSiteSubjectPrefix"] != null ? ConfigurationManager.AppSettings["onTestSiteSubjectPrefix"] : "";
            SentMailWithSMTPCredential = ConfigurationManager.AppSettings["SentMailWithSMTPCredential"]!=null ? Boolean.Parse(ConfigurationManager.AppSettings["SentMailWithSMTPCredential"]):false;
            EnableSSL = ConfigurationManager.AppSettings["EnableSSL"]!=null? Boolean.Parse(ConfigurationManager.AppSettings["EnableSSL"]): false;
            
            if (SentMailWithSMTPCredential)
            {
                smtpCredentials = new System.Net.NetworkCredential(userName, password);
            }
            eventLogEmail = _eventLogEmail;                  
            fromName = "Collateral Registry Systems";
            this.smtpServer = host;
           

            
        }

        //public bool SendEmail(string toEmail,string ccEmail, string bccEmail)
        public bool SendEmail(string EmailBody,string EmailTo, string EmailCC, string EmailBCC, string EmailSubject, ICollection<FileUpload> mEmailAttachmentAttributeList)
        {
            try
            {
                //eventLogEmail.WriteEntry("fromEmail = ConfigurationManager.AppSettings[from]; : " + this.fromEmail, EventLogEntryType.Information);
                //eventLogEmail.WriteEntry("string host = ConfigurationManager.AppSettings[host]; : " + this.smtpServer, EventLogEntryType.Information);
                //eventLogEmail.WriteEntry("string port = ConfigurationManager.AppSettings[port]; : " + this.port, EventLogEntryType.Information);
                //eventLogEmail.WriteEntry("string userName = ConfigurationManager.AppSettings[userName]; : " + this.userName, EventLogEntryType.Information);

                //eventLogEmail.WriteEntry("To address : " + toEmail, EventLogEntryType.Information);
                //eventLogEmail.WriteEntry("CC address : " + ccEmail, EventLogEntryType.Information);
                SmtpClient sc = new SmtpClient();
                sc.Host = this.smtpServer;
                sc.Port = this.port;
                if (SentMailWithSMTPCredential)
                {
                    sc.Credentials = smtpCredentials;

                }
                else
                {
                    sc.Credentials = CredentialCache.DefaultNetworkCredentials;
                }
                if (EnableSSL)
                    sc.EnableSsl = true;
                else
                    sc.EnableSsl = false;

                
                //Message.IsBodyHtml = true;
               

            
                    string[] to;                    
                    string[] ccCopy=null;
                    string[] cc = null;
                    string[] bcc = null;
                    List<string> uniquesmails = null;
                    int totalEmailReceipients = 0;

                    //Make them unique here

                    if (onTestSite == true)
                    {
                        EmailSubject = "(" + this.onTestSiteSubjectPrefix + ") - "+ EmailSubject;
                        to = onTestSiteReceipients.Split(';');                      
                        totalEmailReceipients = totalEmailReceipients + to.Length;
                        //Find the body part of the email
                        int xStart = EmailBody.IndexOf("<body>", 0);
                        if (xStart <0 ) 
                            xStart =0;
                        else
                            xStart = xStart + 6;
                        
                        string Header ="<div style=\"margin-bottom:10px;padding-top:10px; padding-bottom:10px; color: #FFFF00;background-color: #FF0000;text-align: center;\">This email was generated by the Collateral Registry Test Server (" + this.onTestSiteSubjectPrefix +  ") </div>";
                       EmailBody =  EmailBody.Insert(xStart, Header);
                     

                        //eventLogEmail.WriteEntry("Step One Complete");
                    }
                    else
                    {
                        to = EmailTo.Split(';');
                        //Remove duplicates from to
                        totalEmailReceipients = totalEmailReceipients + to.Length;

                        if (EmailCC != null && EmailCC.Length > 0)
                        {
                            
                            cc = EmailCC.Split(';');
                            totalEmailReceipients = totalEmailReceipients + cc.Length;
                        }

                        if (alwaysCopy != null && alwaysCopy.Length > 0)
                        {
                            ccCopy = alwaysCopy.Split(';');
                            totalEmailReceipients = totalEmailReceipients + ccCopy.Length;
                        }

                        if (EmailBCC != null && EmailBCC.Length > 0)
                        {
                            bcc = EmailBCC.Split(';');
                            totalEmailReceipients = totalEmailReceipients + bcc.Length;
                        }

                    }

                   
                   
                   

                    //Get the total cc + bcc.  If they are more than the max entries then we will simply make all of them to and state that the ff were just copied due to limitation in filesize
                    //Now we are going to check that the cc + bcc <= max entries
                    //If they are then we need to 



                    if (totalEmailReceipients <= maxAddresses || maxAddresses <1)
                    {
                        System.Net.Mail.MailMessage Message = new MailMessage();
                        Message.Subject = EmailSubject;
                        Message.Body = EmailBody ;
                        
                        //NORMAL WAY
                        if (to.Length > 0)
                        {
                            for (int iNDX = 0; iNDX < to.Length; iNDX++)
                            {
                                if (to[iNDX].Length > 0)
                                {
                                    Message.To.Add(new MailAddress(to[iNDX]));
                                }
                            }
                        }

                       
                        if (cc!= null && cc.Length > 0)
                        {
                            for (int iNDX = 0; iNDX < cc.Length; iNDX++)
                            {
                                if (cc[iNDX].Length > 0)
                                    Message.CC.Add(new MailAddress(cc[iNDX]));

                            }
                        }

                       
                        if (ccCopy != null && ccCopy.Length > 0)
                        {
                            for (int iNDX = 0; iNDX < ccCopy.Length; iNDX++)
                            {
                                if (ccCopy[iNDX].Length > 0)
                                    Message.CC.Add(new MailAddress(ccCopy[iNDX]));

                            }
                        }

                       
                        if (bcc!= null && bcc.Length > 0)
                        {
                            for (int iNDX = 0; iNDX < bcc.Length; iNDX++)
                            {
                                if (bcc[iNDX].Length > 0)
                                    Message.Bcc.Add(new MailAddress(bcc[iNDX]));

                            }
                        }

                        
                        if (Message.To.Count == 0)
                        {
                            Message.To.Add(new MailAddress(this.fromEmail, this.fromName));
                        }

                        this.SubmitMessage(Message, sc,mEmailAttachmentAttributeList);
                    }
                    else
                    {
                        eventLogEmail.WriteEntry("Multiple sending of single email" );
                        
                        //LOOPING WAY
                        List<string> allmails=new List<string> ();
                        allmails .AddRange (to);
                        if (cc!=null)
                            allmails .AddRange (cc);
                        if (ccCopy != null)
                            allmails .AddRange (ccCopy );
                        if (bcc!=null)
                            allmails .AddRange (bcc );

                        List<string> activeMails;
                        int indx = 0;
                        while (indx <=(allmails .Count -1))
                        {

                            System.Net.Mail.MailMessage Message = new MailMessage();
                            Message.Subject = EmailSubject;
                            Message.Body = EmailBody ;
                            int countrange = 0;
                            if ((allmails.Count - 1) >= indx + (maxAddresses-1))
                                countrange = maxAddresses;
                            else
                                countrange = ((allmails.Count - 1) - indx )+1;
                            activeMails = allmails.GetRange(indx, countrange);
                            indx = indx + maxAddresses;

                            foreach (string mailer in activeMails)
                            {
                                Message.To.Add(new MailAddress(mailer));
                            }

                            this.SubmitMessage(Message, sc,mEmailAttachmentAttributeList );
                        }
                    }



              
             
            }
            catch (Exception ex)
            {
                ADODataContext.InsertError(ex.Message, ex.StackTrace, true, "Exception whiles sending mail to the email(s) ("+ EmailTo+") ", eventLogEmail);
                //eventLogEmail.WriteEntry("Exception within SendEmail function: \nTo Email : " + EmailTo+ " \nMessage :" + ex.Message + "\n Stack : " + ex.StackTrace, EventLogEntryType.Error);
                
                return false;
            }
            return true;
        }

        private void SubmitMessage(System.Net.Mail.MailMessage Message, SmtpClient sc, ICollection<FileUpload> EmailAttachmentAttributeList)
        {
           
            Message.From = (new MailAddress(this.fromEmail, this.fromName));
           

            // eventLogEmail.WriteEntry("Message.From = (new MailAddress(this.fromEmail, this.fromName)); : " + Message.From.Address, EventLogEntryType.Information);

         
            Message.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

            System.Net.Mail.AlternateView plainview = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(
                 Message.Body, @"<(.|\n)*?>", string.Empty), null, "text/plain");


            System.Net.Mail.AlternateView htmlview = System.Net.Mail.AlternateView.CreateAlternateViewFromString
                (Message.Body, null, "text/html");

            Message.AlternateViews.Add(plainview);
            Message.AlternateViews.Add(htmlview);

            if (EmailAttachmentAttributeList.Count > 0)
            {
                foreach (var ea in EmailAttachmentAttributeList)
                {
                    byte[] byteData = ea.AttachedFile ;
                    MemoryStream mOutputStream = new MemoryStream(byteData);
                    Attachment a = new Attachment(mOutputStream, ea.AttachedFileName , MediaTypeNames.Application.Octet);
                    Message.Attachments.Add(a);

                }


            }
            //Let's get the number 
            try
            {
                sc.Send(Message);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                string failedEmails = ex.FailedRecipient;

            }
            eventLogEmail.WriteEntry("Mail message sent to " + Message.To);

            
        }

        public delegate bool SendEmailDelegate(string EmailBody, string EmailTo, string EmailCC, string EmailBCC, string EmailSubject, ICollection<FileUpload> mEmailAttachmentAttributeList);

        public void GetResultsOnCallback(IAsyncResult ar)
        {
            SendEmailDelegate del = (SendEmailDelegate)((System.Runtime.Remoting.Messaging.AsyncResult)ar).AsyncDelegate;
            try
            {
                bool result = del.EndInvoke(ar);
            }
            catch (Exception ex)
            {
                bool result = false;
            }
        }

        public bool SendEmailAsync(string EmailBody,string toEmail, string ccEmail, string bccEmail, string EmailSubject,ICollection<FileUpload> mEmailAttachmentAttributeList)
        {
            try
            {
                SendEmailDelegate dc = new SendEmailDelegate(this.SendEmail);
                AsyncCallback cb = new AsyncCallback(this.GetResultsOnCallback);
                IAsyncResult ar = dc.BeginInvoke(EmailBody, toEmail, ccEmail,  bccEmail, EmailSubject,mEmailAttachmentAttributeList,cb, null);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
