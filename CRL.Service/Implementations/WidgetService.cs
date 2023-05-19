using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model;

using CRL.Model.Notification;
using CRL.Model.Notification.IRepository;
using CRL.Model.WorkflowEngine.IRepository;

using CRL.Service.Interfaces;
using CRL.Service.Messaging;
using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Service.Messaging.FinancialStatements.Response;
using CRL.Service.Messaging.Workflow.Request;
using CRL.Service.Messaging.Workflow.Response;
using CRL.Service.QueryGenerator;
using CRL.Model.Messaging;
using CRL.Infrastructure.Messaging;
using CRL.Model.FS.IRepository;
using CRL.Service.Views.Workflow;
using CRL.Model.Memberships.IRepository;

namespace CRL.Service.Implementations
{
  

    public class WidgetService : IWidgetService
    {
        private readonly IUnitOfWork _uow;
        private readonly IWFCaseRepository _caseRepository;
     private readonly IFinancialStatementRepository _financialStatementRepository;
     private readonly IAuditRepository _auditRepository;
     private readonly IMessageRepository _messageRepository;
     private readonly IEmailRepository _emailRepository;
        private readonly IRoleRepository _roleRepository;
          public WidgetService(IUnitOfWork uow,            
           IWFCaseRepository caseRepository,IFinancialStatementRepository financialStatementRepository,
              IAuditRepository auditRepository,
               IMessageRepository messageRepository,
              IEmailRepository emailRepository,
              IRoleRepository roleRepository
          )
          {
              _uow = uow;
               _financialStatementRepository = financialStatementRepository;
               _auditRepository = auditRepository;
              _messageRepository = messageRepository;
              _roleRepository = roleRepository;
              _emailRepository = emailRepository;
              _caseRepository = caseRepository;
          }

        public ViewMyTasksResponse ViewMy10LatestTasks(RequestBase request)
        {
            ViewMyTasksResponse response = new ViewMyTasksResponse();
            //Let's get the query from the response
            ViewMyTasksRequest requestQuery =  new ViewMyTasksRequest ();
            requestQuery.SecurityUser = request.SecurityUser ;
           

            var myquery = TaskQueryGenerator.CreateQueryForTask(requestQuery, _caseRepository);
              ////We are doing clientside filtering            
            myquery = myquery.Take(10);
            response.TaskGridView = myquery;
            return response;
        }

      
         public ViewFSResponse ViewMy10AboutToExpireFinancingStatement(RequestBase request)
        {

            ViewFSResponse response ;
             ViewFSRequest requestQuery= new ViewFSRequest();
             requestQuery.SecurityUser = request.SecurityUser;
             requestQuery.PageIndex =1;
             requestQuery .PageSize =10;
             requestQuery.ShowActive = FilterFinancingStatement.ActiveButTenDays;
             requestQuery.SortColumn = "ExpiryDate";
             requestQuery.SortOrder = "asc";
            //Let's get the query from the response
             response = _financialStatementRepository.SelectFSGridViewCQ(requestQuery);
             response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
             return response;       
            //    var myquery = FSQueryGenerator.CreateQueryForFindFS (requestQuery, _financialStatementRepository , false);
               
            //response.FSGridView = myquery.ToList();
            //return response;
        }

         public ViewAuditsResponse ViewMy10Audits(RequestBase request)
         {
             ViewAuditsResponse response = new ViewAuditsResponse();
             ViewAuditsRequest requestQuery = new ViewAuditsRequest();
             //Let's get the query from the response
               requestQuery.SecurityUser = request.SecurityUser;
             requestQuery.PageIndex =1;
             requestQuery .PageSize =10;
             var myquery = AuditQueryGenerator.SelectAudit(requestQuery, _auditRepository, false);

                 response.AuditViews = myquery.ToList();
             return response;
         }

         public ViewMyMessagesResponse ViewMy10Messages(RequestBase request)
         {
             var response = new ViewMyMessagesResponse {MessagesViews = new List<MessagesView>()};
             var roleNames = request.SecurityUser.Roles.Split(new[] {'|'});
             ICollection<Message> globalMessages = 
                                roleNames.Select(roleName => _messageRepository.GetMessagesByRoleName(roleName).Take(5).ToList())
                                .Where(assignedMessages => ((ICollection<Message>) assignedMessages)
                                .Count > 0).SelectMany(assignedMessages => (ICollection<Message>) assignedMessages)
                                .GroupBy(s=>s.Id).Select(grp=>grp.First()).OrderByDescending(s=>s.CreatedOn).Take(5).ToList();

             var userEmails = _emailRepository.GetEmailsByUserId(request.SecurityUser.Id);


             IEnumerable<MessagesView> messageViews = new List<MessagesView>();
             if (globalMessages.Count > 0)
             {
                 messageViews = globalMessages.Select(s => new MessagesView()
                                                             {
                                                                 Id = s.Id,
                                                                 Title = s.Title,
                                                                 ClientMessageTypeId = 1,
                                                                 CreatedOn = s.CreatedOn
                                                             });
             }

             IEnumerable<MessagesView> emailViews = new List<MessagesView>();
             if(userEmails.Count > 0)
             {
                 emailViews = userEmails.Select(s => new MessagesView()
                                                         {
                                                             Id = s.EmailUserAssignments.Where(m=>m.UserId == request .SecurityUser .Id).Select(c => c.Id).First(),
                                                             Title = s.EmailSubject,
                                                             ClientMessageTypeId = 2,
                                                             CreatedOn = s.CreatedOn
                                                         });

             }

             var allMessages = emailViews.Concat(messageViews);

             response.MessagesViews = allMessages.OrderByDescending(s=>s.CreatedOn);
             response.NumRecords = allMessages.Count();
             return response;
         }


         public ViewStatResponse ViewNoOfFinancingStatement(RequestBase request)
         {
             ViewStatResponse response = new ViewStatResponse();
             var myquery2 = StatisticalQueryGenerator.TotalNoOfFinancingStatement(_financialStatementRepository, request);
             response.Total = myquery2.Count();
             return response;
         }

    }
}
