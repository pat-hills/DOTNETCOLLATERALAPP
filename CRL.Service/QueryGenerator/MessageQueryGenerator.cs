using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model;
using CRL.Model.ModelViews;
using CRL.Repository.EF.All;
using CRL.Repository.EF.All.Repository;
using CRL.Service.Messaging;
using CRL.Service.Views;
using CRL.Model.Memberships;

namespace CRL.Service.QueryGenerator
{
    public static class MessageQueryGenerator
    {
        public static IQueryable<MessageView> SelectMessage(
         ViewMessagesRequest request, IMessageRepository _messageRepository, Roles[] UserRoles, bool DoCount = false)
        {
            CBLContext ctx = ((MessageRepository)_messageRepository).ctx;

            bool UserInAdminRole = request.SecurityUser.IsAdministrator();


            IQueryable<Message> query = _messageRepository.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false && ((
                    (s.LimitToAdmin == null || s.LimitToAdmin == false || (s.LimitToAdmin == true && UserInAdminRole))
                   && (s.LimitToClientOrOwners == null || (s.LimitToClientOrOwners == 2 && request.SecurityUser.IsOwnerUser == true) || (s.LimitToClientOrOwners == 1 && request.SecurityUser.IsOwnerUser == false))
                   && (s.LimitToInstitutionOrIndividual == null || (s.LimitToInstitutionOrIndividual == 1 && request.SecurityUser.InstitutionId != null)
                   || (s.LimitToInstitutionOrIndividual == 2 && request.SecurityUser.InstitutionId == null))
                   && (s.Roles.Count() < 1 || (s.Roles.Any(d => UserRoles.Contains(d.Id)))))));
            if (request.OnlyUnRead)
            {
                query = query.Where(s => s.ReadBy .Any(d => d.Id == request.SecurityUser.Id)==false);
            }
            var query2 = query.Select(s => new MessageView
       {
           Id = s.Id,
           Body = s.Body,
           Title = s.Title,
           MessageTypeId = s.MessageTypeId,
           MessageTypeName = s.MessageType.Name,
           CreatedOn = s.CreatedOn,
           CreatedBy = s.CreatedBy ,
           Read = s.ReadBy.Any(r => r.Id == request.SecurityUser.Id)
       });


            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "CreatedOn";
                    request.SortOrder = "desc";
                }
            }
            if (String.IsNullOrWhiteSpace(request.SortColumn) == false)
            {

                if (request.SortColumn == "CreatedOn")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.CreatedOn);
                        //query3 = query3.OrderByDescending(s => s.AuditDate);
                        //query4 = query4.OrderByDescending(s => s.AuditDate);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.CreatedOn);
                        //query3 = query3.OrderBy(s => s.AuditDate);
                        //query4 = query4.OrderBy(s => s.AuditDate);
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
    }


}
