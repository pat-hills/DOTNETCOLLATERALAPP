using System;
using System.Collections.Generic;
using System.Linq;
using CRL.Model.Notification;
using CRL.Model.Notification.IRepository;
using CRL.Repository.EF.All;
using CRL.Repository.EF.All.Repository.Email;
using CRL.Service.Messaging.Memberships.Request;
using CRL.Service.Views.Memberships;
using System.Data.Entity;

namespace CRL.Service.QueryGenerator
{
    public class ClientEmailQueryGenerator
    {
        public static IQueryable<ClientEmailView> GetAllEmails(
           ViewClientEmailRequest request, IEmailUserAssignmentRepository _emailUserAssignmentRepository, bool DoCount = false)
        {
            CBLContext ctx = ((EmailUserAssignmentRepository)_emailUserAssignmentRepository).ctx;
            IQueryable<EmailUserAssignment> query = _emailUserAssignmentRepository.GetDbSet();


            if(!(String.IsNullOrEmpty(request.EmailSubject)))
            {
                query = query.Where(s => s.Email.EmailSubject.ToLower().Contains( request.EmailSubject.ToLower()));
            }

            if (request.CreatedOn != null)
            {
                query = query.Where(s => s.Email.CreatedOn >= request.CreatedOn.StartDate && s.Email.CreatedOn < request.CreatedOn.EndDate);
            }


            if (request.NewCreatedOn != null)
            {

                query = query.Where(s => DbFunctions.DiffDays(s.Email.CreatedOn, request.NewCreatedOn) == 0);



            }

            var query2 = query.Where(s => s.UserId == request.SecurityUser.Id).Select(s => new ClientEmailView
            {
                Id = s.Id,
                EmailSubject = s.Email.EmailSubject,
                CreatedOn = s.Email.CreatedOn,
                IsSent = s.Email.IsSent,
                IsRead = s.IsRead,
                HasAttachment = (ctx.FileUploads.Where(m => m.EmailId == s.Email.Id).Count() > 0 ? true : false)

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

                if (request.SortColumn == "EmailSubject")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.EmailSubject);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.EmailSubject);
                    }
                }

            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }
            return query2;
        }

        public static ClientEmailView GetEmailDetails(ViewClientEmailRequest request, IEmailUserAssignmentRepository _emailUserAssignmentRepository)
        {
            CBLContext ctx = ((EmailUserAssignmentRepository)_emailUserAssignmentRepository).ctx;
            IQueryable<EmailUserAssignment> query = _emailUserAssignmentRepository.GetDbSet();


            var query1 = query.Where(s => s.Id == request.AssignmentId && s.UserId == request.SecurityUser.Id).Select(c => new ClientEmailView
            {
                Id = c.Email.Id,
                EmailSubject = c.Email.EmailSubject,
                EmailBcc = c.Email.EmailBcc,
                EmailBody = c.Email.EmailBody,
                EmailCc = c.Email.EmailCc,
                EmailFrom = c.Email.EmailFrom,
                EmailTo = c.Email.EmailTo,
                CreatedOn = c.Email.CreatedOn,
                IsSent = c.Email.IsSent,
                NumRetries = c.Email.NumRetries,
                HasAttachment = (ctx.FileUploads.Where(m => m.EmailId == c.Email.Id).Count() > 0 ? true : false) 
            }
        );
            var query2 = query1.SingleOrDefault();

            if(query2.HasAttachment == true)
            {
                query2.EmailAttachments = ctx.FileUploads.Where(m => m.EmailId == query2.Id).ToList();
            }
            return query2;
        }
        
    }
}
