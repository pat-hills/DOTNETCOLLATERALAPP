using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.FS;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;
using CRL.Model.Memberships;
using CRL.Model.Notification.IRepository;


namespace CRL.Model.Notification
{
    public class EmailUserAssignment : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        
        public int EmailId { get; set; }
        public Email Email { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool IsRead { get; set; }
        public bool UserIsCopied { get; set; }


        public void AssignEmailToUser (Email email, int UserId)
        {
               this.UserId =UserId   ;
            this.IsActive = true;
            this.IsRead = false;
            this.Email = email;

        }


        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
    public class Email : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public IEmailUserAssignmentRepository _emailUserAssignmentRepository { get; set; }
       
        public Email()
        {
            EmailAttachments = new HashSet<FileUpload>();
            IsSent = false;
        }

        public string EmailBody{get;set;}
        [MaxLength(255)]
        public string EmailSubject{get;set;}

        public string EmailTo{get;set;}

        public string EmailCc{get;set;}

        public string EmailBcc{get;set;}
        [MaxLength(100)]
        public string EmailFrom{get;set;}
        public bool? IsSent{get;set;}
        public int NumRetries{get;set;}
        public DateTime? LastRetryDate { get; set; }
        public virtual ICollection<FileUpload> EmailAttachments { get; set; }
        public virtual ICollection<EmailUserAssignment> EmailUserAssignments { get; set; }

        public void AddEmailAddresses(List<UserEmailView> UserEmailView, AuditingTracker tracker = null)
        {
             foreach (UserEmailView userEmail in UserEmailView)
            {
               this.AddEmailAddresses (userEmail,tracker  );

            }

          
        }

        public void CleanEmailAddresses()
        {
            if (this.EmailTo != null)
                this.EmailTo=this.EmailTo.TrimEnd(';');
            if (this.EmailCc  != null)
                this.EmailCc  = this.EmailTo.TrimEnd(';');
            if (this.EmailBcc  != null)
                this.EmailBcc  =this.EmailTo.TrimEnd(';');

        }

        public void AddEmailAddresses(UserEmailView userEmail, AuditingTracker tracker = null)
        {
            EmailUserAssignment em = new EmailUserAssignment();
            em.UserId = userEmail.Id;
            em.IsActive = true;
            em.IsRead = false;
            em.Email = this;
            _emailUserAssignmentRepository.Add(em);
            tracker.Created.Add(em);
            if (userEmail.RecepientType == MailReceipientType.To)
            {
                this.EmailTo += userEmail.Email + ";";
                em.UserIsCopied = false;
                
            }
            else if (userEmail.RecepientType == MailReceipientType.CC)
            {
                this.EmailCc += userEmail.Email + ";";
                em.UserIsCopied = false;
            }
            else if (userEmail.RecepientType == MailReceipientType.BCC)
            {
                this.EmailBcc += userEmail.Email + ";";
                em.UserIsCopied = false;
            }
        }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
