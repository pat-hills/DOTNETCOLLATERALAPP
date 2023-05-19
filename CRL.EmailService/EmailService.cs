using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CRL.Repository.EF.All;
using CRL.Model.Notification;
using System.Data.SqlClient;

namespace CRL.EmailService
{
     
   
    public partial class EmailService : ServiceBase
    {
        private Timer scheduleTimer;
        private DateTime lastRun;
        private bool flag;
        private bool IsFinishMailSend = false;
        EmailSender emailSender= null;
        private ADOHealthMonitoringDataContext adoDataContext;
        private EmailDbContext context;
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
        
        public EmailService()
        {
            InitializeComponent();
            if (!System.Diagnostics.EventLog.SourceExists("CRLEmailSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "CRLEmailSource", "CRLEmailLog");
            }
            eventLogEmail.Source = "Diagnostics";
            eventLogEmail.Log = "CRLEmailLog";

              scheduleTimer = new Timer();
           // scheduleTimer.Interval = 1 * 5 * 60 * 1000;
            scheduleTimer.Interval = 1 *  60 * 1000;
            scheduleTimer.Elapsed += new ElapsedEventHandler(scheduleTimer_Elapsed);
        }

        protected override void OnStart(string[] args)
        {        
           
            eventLogEmail.WriteEntry("In OnStart");
            //As soon as the service is started just push emails
            ServiceEmailMethod();
            IsFinishMailSend = true;
            scheduleTimer.Start();
           
        }

        protected override void OnStop()
        {
            scheduleTimer.Stop();
            eventLogEmail.WriteEntry("In onStop.");
        }

        protected override void OnPause()
        {
            scheduleTimer.Stop();
            eventLogEmail.WriteEntry("Paused");
        }
        protected override void OnContinue()
        {
            scheduleTimer.Start();
            eventLogEmail.WriteEntry("In OnContinue.");
        }

        protected override void OnShutdown()
        {
            scheduleTimer.Stop();
            eventLogEmail.WriteEntry("ShutDowned");
        }

        protected void scheduleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsFinishMailSend)
            {
                IsFinishMailSend = false;
                ServiceEmailMethod();
            }

        }
        
        private void ServiceEmailMethod()
        {
            //eventLogEmail.WriteEntry("In Sending Email Method, Date : " + DateTime.Now, EventLogEntryType.Information);
            ADODataContext.InsertError("","", false, "Processing mails" + DateTime.Now, eventLogEmail);
            List<Email> Emails = null;
            try
            {
               
                //Here is where we actually load the mails and sends them
                using (context = new EmailDbContext())
                {

                    eventLogEmail.WriteEntry("Get Emails from connection : " + context.Database.Connection.ConnectionString, EventLogEntryType.Information);
                    Emails = context.Emails.Include("EmailAttachments").Where(s => s.IsSent == false && !(s.EmailTo == null || s.EmailTo == "")).ToList();  //**We must limit this to a fix number at a time and use only one thread


                    if (Emails != null)
                    {
                        if (Emails.Count > 0)
                        {
                            eventLogEmail.WriteEntry("This thread pull db Emails : " + Emails.Count, EventLogEntryType.Information);                           
                        }
                    }
                }
              
            }
            catch (Exception ex)
            {
                ADODataContext.InsertError(ex.Message, ex.StackTrace, true, "General Error:" + DateTime.Now,eventLogEmail );
                //eventLogEmail.WriteEntry("Error : " + ex.Message + "\n Stack : " + ex.StackTrace + "\nDate :" + DateTime.Now, EventLogEntryType.Error);

            }
            if (Emails != null)
            {
                SendEmail(Emails);
                IsFinishMailSend = true;
            }
        }

        private void SendEmail(List<Email> Emails)
        {

            if (Emails.Count > 0)
            {

                if (emailSender == null)
                {
                    emailSender = new EmailSender(eventLogEmail);
                }

                List<int> EmailSentIds = new List<int>();
                List<int> failedSentIds = new List<int>();
                foreach (var email in Emails)
                {
                    if (!String.IsNullOrWhiteSpace(email.EmailTo))
                    {


                        // bool result = email.SendEmailAsync(em.To, em.Cc, em.Bcc);
                        bool result = emailSender.SendEmail(email.EmailBody, email.EmailTo, email.EmailCc, email.EmailBcc, email.EmailSubject, email.EmailAttachments);
                        if (result)
                        {
                            EmailSentIds.Add(email.Id);
                            eventLogEmail.WriteEntry("em.EmailId : " + email.Id, EventLogEntryType.Information);
                            //eventLogEmail.WriteEntry("Mail send successfully, Date : " + DateTime.Now, EventLogEntryType.SuccessAudit);
                            try
                            {
                                using (context = new EmailDbContext())
                                {
                                    Email m = context.Emails.Find(email.Id);
                                    m.IsSent = true;
                                    context.SaveChanges();
                                }
                            }
                            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                            {
                                var sqlException = ex.InnerException as SqlException;

                                if (sqlException != null)
                                {
                                    ADODataContext.InsertError(sqlException.Message, sqlException.StackTrace, true, "Error whiles trying to log mail is sent", eventLogEmail);
                                    eventLogEmail.WriteEntry("Sql Exception : " + sqlException.Message + ". Stacktrace: "+sqlException.StackTrace, EventLogEntryType.Error);
                                }
                            }
                            catch (Exception ex)
                            {
                                ADODataContext.InsertError(ex.Message, ex.StackTrace, true, "Error whiles trying to log mail is sent", eventLogEmail);
                            }
                        }
                        else
                        {
                            
                            try
                            {
                                using (context = new EmailDbContext())
                                {
                                    Email m = context.Emails.Find(email.Id);
                                    m.NumRetries = m.NumRetries + 1;
                                    context.SaveChanges();
                                }
                            }
                            catch (Exception ex)
                            {
                                ADODataContext.InsertError(ex.Message, ex.StackTrace, true, "Error whiles trying to log mail failure" + DateTime.Now, eventLogEmail);

                            }
                            


                        }
                    }

                }
               


            }

        }

    }
}
