using CRL.Model.Notification;
using CRL.Model.Notification .IRepository;
using CRL.Repository.EF.All;
using CRL.Repository.EF.All.Repository.Email;
using CRL.Service.Messaging.EmailAdministration.Request;
using CRL.Service.Messaging.EmailAdministration.Response;
using CRL.Service.Messaging.Memberships.Request;
using CRL.Service.Views.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Service.Views.Memberships;

namespace CRL.Service.QueryGenerator
{
    public  class EmailAdministrationQueryGenerator
    {
        public static IQueryable<EmailView> GetAllEmails(
           ViewEmailRequest request, IEmailRepository _emailRepository, bool DoCount = false)
        {
            CBLContext ctx = ((EmailRepository)_emailRepository).ctx ;
            IQueryable<Email> query = _emailRepository.GetDbSet();

            if (request.SentDate != null) 
            {
                query = query.Where(s => s.CreatedOn >= request.SentDate.StartDate && s.CreatedOn < request.SentDate.EndDate);
            }
            if (request.AllOrUnsent == 2) 
            {
                query = query.Where(s => s.IsSent == false);
            }
            var query2 = query.Select(s => new EmailView
            {
                Id = s.Id,
                EmailSubject =s.EmailSubject,                            
                EmailBcc= s.EmailBcc,
                EmailBody =s.EmailBody,
                EmailCc = s.EmailCc,
                EmailFrom = s.EmailFrom,
                EmailTo = s.EmailTo,
                IsSent = s.IsSent,
                IsActive = s.IsActive,
                NumRetries = s.NumRetries,
               LastRetryDate = s.LastRetryDate,
               HasAttachment = ( ctx.FileUploads.Where(m=>m.EmailId==s.Id).Count() > 0? true:false) ,
               CreatedOn = s.CreatedOn 
               

            });

            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "CreatedOn";
                    request.SortOrder = "desc";
                }

                if (request.SortColumn == "CreatedOn")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.CreatedOn);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.CreatedOn);
                    }
                }



            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }





            /*
            select new ClientView
                        {
                            Name =
                                (
                                m.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == m.Id && a.InstitutionId == null).Select(r => r.FirstName + r.MiddleName + r.Surname).SingleOrDefault() :
                                m.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == m.Id).Select(r => r.Name).SingleOrDefault() : "Unknown"

                                ),
                            AccountNumber = m.AccountNumber,
                            ClientType =
                             (
                                m.isIndividualOrLegalEntity == 1 ? "Individual" :
                                m.isIndividualOrLegalEntity == 2 ? "Institution" : "Unknown"

                                ),

                            RepresentativeClientId = m.RepresentativeId,
                            RepresentativeClient = ctx.Institutions.Where(d => d.MembershipId == m.RepresentativeId).SingleOrDefault().Name
                        };



            */

            return query2;


        }
        public static ClientEmailView GetEmailDetails(ViewClientEmailRequest request, IEmailRepository _emailRepository)
        {
            CBLContext ctx = ((EmailRepository)_emailRepository).ctx;
            IQueryable<Email> query = _emailRepository.GetDbSet();


            var query1 = query.Where(s => s.Id == request.AssignmentId).Select(c => new ClientEmailView
            {
                Id = c.Id,
                EmailSubject = c.EmailSubject,
                EmailBcc = c.EmailBcc,
                EmailBody = c.EmailBody,
                EmailCc = c.EmailCc,
                EmailFrom = c.EmailFrom,
                EmailTo = c.EmailTo,
                CreatedOn = c.CreatedOn,
                IsSent = c.IsSent,
                NumRetries = c.NumRetries,
                HasAttachment = (ctx.FileUploads.Where(m => m.EmailId == c.Id).Count() > 0 ? true : false)
            }
        );
            var query2 = query1.SingleOrDefault();

            if (query2.HasAttachment == true)
            {
                query2.EmailAttachments = ctx.FileUploads.Where(m => m.EmailId == query2.Id).ToList();
            }
            return query2;
        }
    }
}
