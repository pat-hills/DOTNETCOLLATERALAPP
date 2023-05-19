using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model;
using CRL.Model.FS.Enums;
using CRL.Model.ModelViews;


using CRL.Service.Messaging;
using CRL.Service.Messaging.User.Request;
using CRL.Service.Messaging.User.Response;
using CRL.Service.QueryGenerator;
using CRL.Service.Views;
using CRL.Service.Mappings.Membership;
using CRL.Infrastructure.Messaging;
using CRL.Model.Messaging;
using CRL.Model.Memberships;
using CRL.Model.Memberships.IRepository;

namespace CRL.Service.Implementations
{
    public class MessageService
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository  _messageRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAuditRepository _auditRepository;
        
        
        public AuditingTracker auditTracker { get; set; }
        public DateTime AuditedDate { get; set; }
        private AuditAction AuditAction;
        private String AuditMessage;

        public MessageService(IUserRepository userRepository, IMessageRepository messageRepository, IRoleRepository roleRepository,  IAuditRepository auditRepository,IUnitOfWork uow)
         {
             AuditedDate = DateTime.Now;
             auditTracker = new AuditingTracker();
             _userRepository = userRepository;
             _messageRepository = messageRepository;
             _roleRepository = roleRepository;
             _auditRepository = auditRepository;
             _uow = uow;
         }

        public ViewMessagesResponse ViewMessages(ViewMessagesRequest request)
        {

            ViewMessagesResponse response = new ViewMessagesResponse();
            User user = _userRepository.FindBy(request.SecurityUser.Id);
            Roles[] UserRoles = (user.Roles).Select(s => s.Id).ToArray();
            //Let's get the query from the response
            if (request.PageIndex > 0)
            {
                var myquery = MessageQueryGenerator.SelectMessage(request, _messageRepository, UserRoles, true);
                int TotalRecords = myquery.Count();
                response.NumRecords = TotalRecords;
            }


            var myquery2 = MessageQueryGenerator.SelectMessage(request, _messageRepository, UserRoles,false);

            response.MessagesView  = myquery2.ToList();
            return response;
        }

         public ViewMessageResponse ViewMessage(RequestBase request)
        {
            ViewMessageResponse response = new ViewMessageResponse();
            response.MessageView  = _messageRepository.GetDbSet().Where(s => s.Id == request.Id)
                .Select(d => new MessageView
        {
            Id = d.Id,
            Body = d.Body,
            Title = d.Title,
            MessageTypeId = d.MessageTypeId,
            MessageTypeName = d.MessageType.Name,
            CreatedOn = d.CreatedOn,
            CreatedBy = d.CreatedBy ,
            Read = d.ReadBy .Any (r=>r.Id == request .SecurityUser .Id)
        }).FirstOrDefault();

             
           
          Message message = _messageRepository.FindBy (request .Id );
            User user = _userRepository.FindBy(request.SecurityUser.Id);
            message.ReadBy.Add(user);
            auditTracker.Updated.Add(message);


            _uow.Commit();
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            response.MessageInfo.Message = "Message read!";
            return response;
             
         }

         public ViewUserRolesResponse GetReadyForCreateMessage(RequestBase request)
         {
             //Check request for edit so that we load only roles
             ViewUserRolesResponse response = new ViewUserRolesResponse();
             //Let's get the query from the response

             ViewUserRolesRequest req = new ViewUserRolesRequest();
             req.SecurityUser = request.SecurityUser;    

             response.RoleGridView = _roleRepository .GetRolesGrid (req);
             

             return response;
         }
        
         public ResponseBase CreateMessage(CreateMessagesRequest request)
         {
             ResponseBase response = new ResponseBase();
             Message message = new Message
             {
                 IsActive = true,
                 IsDeleted = false,
                 LimitToAdmin = request.MessageView.LimitToAdmin,
                 LimitToClientOrOwners = request.MessageView.LimitToClientOrOwners,
                 LimitToInstitutionOrIndividual = request.MessageView.LimitToClientOrOwners,
                 Body = request.MessageView.Body,
                 Title = request.MessageView.Title,
                 Roles = _roleRepository.GetDbSet().Where(s => request.MessageView.Roles.Contains((int)s.Id)).ToList()
             };

             _messageRepository.Add(message);

             AuditAction = Model.FS.Enums.AuditAction.CreateMessage ;
             AuditMessage = "Message Title:" + message.Title ;
             Audit audit = new Audit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction);
             _auditRepository.Add(audit);
             auditTracker.Created.Add(audit);
             _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);

             try
             {
                 _uow.Commit();
             }
             catch (Exception ex)
             {
                 //WFor exceptions that we can handle we will have a set of handled exceptions enumeration and 
                 //pass them back to the response call
                 throw ex;
             }


             response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
             response.MessageInfo.Message =  "Message created successfully!";
             return response;


             

         }

        
        
        //Delete Message
        
      
        //LogMessageViewed
         public ResponseBase LogMessageViewed(RequestBase request)
         {
             ResponseBase response = new ResponseBase();
             Message message = _messageRepository.FindBy(request.Id);
             User user = _userRepository.FindBy(request.SecurityUser.Id);
             message.ReadBy.Add(user);
             auditTracker.Updated.Add(message);


             _uow.Commit();
             response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
             response.MessageInfo.Message = "Message read!";
             return response;


         }

         
      
    }
}
