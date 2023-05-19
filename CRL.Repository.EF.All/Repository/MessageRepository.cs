using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Administration;
using CRL.Respositoy.EF.All.Common.Repository;
using System.Data.Entity;

namespace CRL.Repository.EF.All.Repository
{
 

    public class MessageRepository : Repository<Message, int>, IMessageRepository
    {
        public MessageRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "Message";
        }

        public ViewGlobalMessagesModelResponse GlobalMessageGridView(ViewGlobalMessagesModelRequest request)
        {
            ViewGlobalMessagesModelResponse response = new ViewGlobalMessagesModelResponse();
            IQueryable<Message> query = ctx.Message.AsNoTracking();
            query = query.Where(s => s.IsDeleted == false && s.IsActive == true);
            if (request.CreatedRange != null)
            {
                query = query.Where(s => s.CreatedOn >= request.CreatedRange.StartDate && s.CreatedOn < request.CreatedRange.EndDate);
            }
            if (!(String.IsNullOrEmpty(request.Title)))
            {
                query = query.Where(s => s.Title.ToLower().StartsWith(request.Title.ToLower()));
            }
            if (!(String.IsNullOrEmpty(request.Body)))
            {
                query = query.Where(s => s.Body.ToLower().StartsWith(request.Body.ToLower()));
            }
            if (request.NewCreatedOn != null)
            {

                query = query.Where(s => DbFunctions.DiffDays(s.CreatedOn, request.NewCreatedOn) == 0);


            }

            if (request.PageIndex > 0)
            {
                response.NumRecords = query.Count();
            }
            var query2 = query.Select(s => new GlobalMessageView
                                               {
                                                   Id = s.Id,
                                                   Title = s.Title,
                                                   Body = s.Body,
                                                   CreatedOn = s.CreatedOn
                                               });
            if ((String.IsNullOrWhiteSpace(request.SortColumn)))
            {
                request.SortColumn = "CreatedOn";
                request.SortOrder = "desc";
            }

            if (request.SortColumn == "Title")
            {
                if (request.SortOrder == "desc")
                {
                    query2 = query2.OrderByDescending(s => s.Title);
                }
                else
                {
                    query2 = query2.OrderBy(s => s.Title);
                }
            }

            if (request.SortColumn == "Body")
            {
                if (request.SortOrder == "desc")
                {
                    query2 = query2.OrderByDescending(s => s.Body);
                }
                else
                {
                    query2 = query2.OrderBy(s => s.Body);
                }
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


            if (request.PageIndex > 0)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }

            response.GlobalMessageView = query2.ToList();
            return response;
        }

        public Message GetGlobalMessageDetailsById(int id)
        {
            var query =
                ctx.Message.Include("Roles").Where(s => s.Id == id && s.IsDeleted == false && s.IsActive == true).SingleOrDefault();
            return query;
        }


        public IEnumerable<Message> GetMessagesByRoleName(string roleName)
        {
            var query = ctx.Message.Include("Roles").Where(s => s.Roles.Any(c => c.Name == roleName) && s.IsDeleted == false && s.IsActive == true).OrderByDescending(s=>s.Id);
            return query;
        } 
        
    }
}
