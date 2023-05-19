using System.Data.Entity.Infrastructure;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.ModelViews;

using CRL.Model.ModelViews.Administration;
using CRL.Model.Notification;
using CRL.Model.Notification.IRepository;
using CRL.Model.Notification.IRepository;
using CRL.Repository.EF.All.Repository.Email;
using CRL.Service.BusinessServices;
using CRL.Service.Interfaces;
using CRL.Service.Mappings.Membership;
using CRL.Service.Messaging.Configuration.Request;
using CRL.Service.Messaging.Configuration.Response;
using CRL.Service.Messaging.EmailAdministration.Request;
using CRL.Service.Messaging.EmailAdministration.Response;
using CRL.Service.Messaging.Memberships.Request;
using CRL.Service.Messaging.Memberships.Response;
using CRL.Service.QueryGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewGlobalMessagesRequest = CRL.Service.Messaging.Configuration.Request.ViewGlobalMessagesRequest;
using ViewGlobalMessagesResponse = CRL.Service.Messaging.Configuration.Response.ViewGlobalMessagesResponse;
using CRL.Model.Memberships.IRepository;
using CRL.Model.Memberships;

namespace CRL.Service.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailRepository _emailRepository;
        private readonly IEmailUserAssignmentRepository _emailUserAssignmentRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly IUserRepository _userRepository;

        public AuditingTracker auditTracker { get; set; }
        public DateTime AuditedDate { get; set; }
        public AuditAction AuditAction { get; set; }
        public String AuditMessage;
        public EmailService(IEmailRepository emailRepository, 
            IEmailUserAssignmentRepository emailUserAssignmentRepository, 
            IMessageRepository messageRepository,
            IRoleRepository roleRepository,
            IAuditRepository auditRepository, IUserRepository userRepository,
            IUnitOfWork uow)
        {
            AuditedDate = DateTime.Now;
            auditTracker = new AuditingTracker();
            _emailRepository = emailRepository;
            _emailUserAssignmentRepository = emailUserAssignmentRepository;
            _messageRepository = messageRepository;
            _roleRepository = roleRepository;
            _auditRepository = auditRepository;
            _userRepository = userRepository;
            _uow = uow;
        }

        public ViewEmailResponse GetAllEmails(ViewEmailRequest request)
     {

         ViewEmailResponse response = new ViewEmailResponse();

         if (!request.SecurityUser.IsInAnyRoles("Administrator (Owner)", "Administrator (Client)"))
         {
             response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
             return response;
         }

         if (request.PageIndex > 0)
         {
             var myquery = EmailAdministrationQueryGenerator.GetAllEmails(request, _emailRepository, true);
            int TotalRecords = myquery.Count();
              response.NumRecords = TotalRecords;
         
         }
             var myquery2 = EmailAdministrationQueryGenerator.GetAllEmails(request, _emailRepository); //UPDAATE:  This conversion is wrong, we have to find a way of using 
            response.EmailView = myquery2.ToList();
            foreach (var email in response.EmailView)
            {
                if (email.NumRetries > 1 && email.IsSent == false)
                {
                    email.FailedMail = true;
                }
                else
                {
                    email.FailedMail = false;
                
                }
            }
            return response;
     }

        public ViewClientEmailResponse ViewClientEmails(ViewClientEmailRequest request)
        {
            ViewClientEmailResponse response = new ViewClientEmailResponse();
            if (request.PageIndex > 0)
            {
                var myquery = ClientEmailQueryGenerator.GetAllEmails(request,  _emailUserAssignmentRepository, true);
                int TotalRecords = myquery.Count();
                response.NumRecords = TotalRecords;
            }
            var myquery2 = ClientEmailQueryGenerator.GetAllEmails(request,  _emailUserAssignmentRepository);
            response.EmailViews = myquery2.ToList();
            return response;
        }
        public ViewClientEmailResponse ViewEmailDetails(ViewClientEmailRequest request)
        {
            ViewClientEmailResponse response = new ViewClientEmailResponse();
            if(!request.IsAdminMode)
            {
                EmailUserAssignment emailUserAssignment = _emailUserAssignmentRepository.FindBy(request.AssignmentId);
                emailUserAssignment.IsRead = true;
                auditTracker.Updated.Add(emailUserAssignment);


                var myquery = ClientEmailQueryGenerator.GetEmailDetails(request, _emailUserAssignmentRepository);
                response.ClientEmailView = myquery;

                _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
                try
                {
                    _uow.Commit();
                }
                catch (Exception ex)
                {

                    throw ex;
                } 
            }
            else
            {
                var myquery = EmailAdministrationQueryGenerator.GetEmailDetails(request, _emailRepository);
                response.ClientEmailView = myquery;
            }
            
            return response;
        }

        public GetDataForEmailAttachmentResponse GetDataForEmailAttachment(GetDataForEmailAttachmentRequest request)
        {
            FileUpload emailAttachment = null;
            emailAttachment = ((EmailRepository)_emailRepository).ctx.FileUploads.SingleOrDefault(s => s.Id == request.Id);
            GetDataForEmailAttachmentResponse response = new GetDataForEmailAttachmentResponse();
            response.AttachedFile = emailAttachment.AttachedFile;
            response.AttachedFileName = emailAttachment.AttachedFileName;
            response.AttachedFileSize = emailAttachment.AttachedFileSize;
            response.AttachedFileType = emailAttachment.AttachedFileType;
            return response;
        }

        public ViewGlobalMessagesResponse GetAllGlobalMessages(ViewGlobalMessagesModelRequest request)
        {
            /*var modelRequest =  new ViewGlobalMessagesModelRequest();
            modelRequest.CreatedRange = request.CreatedRange;
            modelRequest.Title = request.Title;
            //modelRequest.PageIndex = request.PageIndex;*/

            ViewGlobalMessagesResponse response = new ViewGlobalMessagesResponse();
            if (!request.SecurityUser.IsInAnyRoles("Administrator (Owner)"))
            {
                var roleNames = request.SecurityUser.Roles.Split(new[] { '|' });
                ICollection<Message> globalMessages =
                                   roleNames.Select(roleName => _messageRepository.GetMessagesByRoleName(roleName).ToList())
                                   .Where(assignedMessages => ((ICollection<Message>)assignedMessages)
                                   .Count > 0).SelectMany(assignedMessages => (ICollection<Message>)assignedMessages)
                                   .GroupBy(s => s.Id).Select(grp => grp.First()).OrderByDescending(s => s.CreatedOn).ToList();
                ICollection<GlobalMessageView> messageViews = new List<GlobalMessageView>();
                if (globalMessages.Count > 0)
                {
                    if (request.CreatedRange != null)
                    {
                        globalMessages = globalMessages.Where(s => s.CreatedOn >= request.CreatedRange.StartDate && s.CreatedOn < request.CreatedRange.EndDate).ToList();
                    }
                    if (!(String.IsNullOrEmpty(request.Title)))
                    {
                        globalMessages = globalMessages.Where(s => s.Title.ToLower().StartsWith(request.Title.ToLower())).ToList();
                    }
                    if (!(String.IsNullOrEmpty(request.Body)))
                    {
                        globalMessages = globalMessages.Where(s => s.Body.ToLower().StartsWith(request.Body.ToLower())).ToList();
                    }
                    if (request.NewCreatedOn != null)
                    {
                        globalMessages = globalMessages.Where(s=>s.CreatedOn.Subtract((DateTime)request.NewCreatedOn).Days == 0).ToList();
                    }

                    messageViews = globalMessages.Select(s => new GlobalMessageView()
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Body = s.Body,
                        CreatedOn = s.CreatedOn
                    }).OrderByDescending(s => s.CreatedOn).ToList();
                }
                response.GlobalMessageViews = messageViews;
                response.NumRecords = messageViews.Count();
                return response;
            }
            if (request.PageIndex > 0)
            {
                var myquery = _messageRepository.GlobalMessageGridView(request);
                response.GlobalMessageViews = myquery.GlobalMessageView;
                response.NumRecords = myquery.NumRecords;

            }
            return response;
        }


        public CreateGMResponse GetCreateDetails(CreateGMRequest request)
        {
            CreateGMResponse response = new CreateGMResponse();

            response.MessageRolesList = LookUpServiceHelper.Roles(_roleRepository);
            if (!request.SecurityUser.IsInAnyRoles("Administrator (Owner)") && (request.Mode == 3 || request.Mode == 2))
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }
            if(request.IsEditOrView)
            {
                var message = _messageRepository.GetGlobalMessageDetailsById(request.Id);
               if(message != null)
               {
                   response.GlobalMessageDetailsView = new GlobalMessageDetailsView()
                   {
                       Id = message.Id,
                       Title = message.Title,
                       Body = message.Body,
                       MessageTypeId = message.MessageTypeId,
                       IsLimitedToAdmin = message.LimitToAdmin,
                       IsLimitedToClientOrOwners = message.LimitToClientOrOwners,
                       IsLimitedToInstitutionOrIndividual = message.LimitToInstitutionOrIndividual,
                       CreatedOn = message.CreatedOn
                   };

                   ICollection<LookUpView> lookups = new List<LookUpView>();
                   var list = message.Roles.ToList();
                   
                   foreach (var c in list)
                   {
                       LookUpView lk = new LookUpView();
                       lk.LkValue = (int)c.Id;
                       lk.LkName = c.Name;
                       lookups.Add(lk);

                   }
                   response.SelectedMessageRolesList = lookups;
               }
            }
            
            return response;
        }

        public CreateSubmitGlobalMessageResponse CreateSubmitGm(CreateSubmitGlobalMessageRequest request)
        {
            CreateSubmitGlobalMessageResponse response = new CreateSubmitGlobalMessageResponse();
            if (!request.SecurityUser.IsInAnyRoles("Administrator (Owner)"))
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }

            Message message = new Message()
                                  {
                                      Title = request.GlobalMessageDetailsView.Title,
                                      Body = request.GlobalMessageDetailsView.Body,
                                      MessageTypeId = MessageCategory.Feed,
                                      LimitToAdmin =  request.GlobalMessageDetailsView.IsLimitedToAdmin,
                                      LimitToClientOrOwners = request.GlobalMessageDetailsView.IsLimitedToClientOrOwners,
                                      LimitToInstitutionOrIndividual = request.GlobalMessageDetailsView.IsLimitedToInstitutionOrIndividual
                                  };
            _messageRepository.Add(message);

            ICollection<User> _messageReciepients = new List<User>();

            var roles = _roleRepository.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).ToList();
            if(request.MessageRoles != null)
            {
                foreach (var roleId in request.MessageRoles)
                {
                    var selectedRole = roles.Single(s => s.Id == (Roles)roleId);
                    message.Roles.Add(selectedRole);
                    var users = _userRepository.GetDbSet().Where(s => s.Roles.FirstOrDefault(d=>d.Id==(Roles)roleId).Id == selectedRole.Id);
                    if (users != null)
                    {
                        foreach (var user in users)
                        {
                            if (!_messageReciepients.Contains(user))
                            {
                                _messageReciepients.Add(user);
                            }
                        }
                    }
                }

                foreach (var receipient in _messageReciepients)
                {
                    var body = "<body>" + request.GlobalMessageDetailsView.Body + "</body>";
                    var email = new Email() { EmailSubject = request.GlobalMessageDetailsView.Title, EmailBody = body, EmailTo = receipient.Address.Email };
                    _emailRepository.Add(email);
                    auditTracker.Created.Add(email);
                }

            }
            auditTracker.Created.Add(message);

            //Audit audit = new Audit(AuditMessage , request.RequestUrl, request.UserIP, AuditAction);
            //_auditRepository.Add(audit);
            //auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
         
            try
            {
                _uow.Commit();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Update original values from the database 
                var entry = ex.Entries.Single();
                entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.DatabaseConcurrencyConflict;
                return response;


            }
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            response.MessageInfo.Message = "Global Message added successfully!";
            return response;
        }

        public CreateSubmitGlobalMessageResponse UpdateSubmitGm(CreateSubmitGlobalMessageRequest request)
        {
            CreateSubmitGlobalMessageResponse response = new CreateSubmitGlobalMessageResponse();
            if (!request.SecurityUser.IsInAnyRoles("Administrator (Owner)"))
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }
            Message message = _messageRepository.GetGlobalMessageDetailsById(request.GlobalMessageDetailsView.Id);

            message.Title = request.GlobalMessageDetailsView.Title;
            message.Body = request.GlobalMessageDetailsView.Body;
            message.MessageTypeId = MessageCategory.Feed;
            message.LimitToAdmin = request.GlobalMessageDetailsView.IsLimitedToAdmin;
            message.LimitToClientOrOwners = request.GlobalMessageDetailsView.IsLimitedToClientOrOwners;
            message.LimitToInstitutionOrIndividual = request.GlobalMessageDetailsView.IsLimitedToInstitutionOrIndividual;

            ICollection<User> _messageReciepients = new List<User>();
            var roles = _roleRepository.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).ToList();


            if (message.Roles != null && request.MessageRoles != null) 
            {
                foreach (var role in message.Roles.ToList())
                {
                   
                    if (!request.MessageRoles.Contains((int)role.Id))
                        message.Roles.Remove(role);
                }

               
            }

            if (request.MessageRoles != null)
            {
                foreach (var roleId in request.MessageRoles)
                {
                    var selectedRole = roles.Single(s => s.Id == (Roles)roleId);
                    var users = _userRepository.GetDbSet().Where(s => s.Roles.FirstOrDefault(d => d.Id == (Roles)roleId).Id == selectedRole.Id);
                    if (users != null)
                    {
                        foreach (var user in users)
                        {
                            if (!_messageReciepients.Contains(user))
                            {
                                _messageReciepients.Add(user);
                            }
                        }
                    }
                    if (message.Roles.Any(c => (int)c.Id == roleId) == false)
                        message.Roles.Add(selectedRole);
                }

                foreach (var receipient in _messageReciepients)
                {
                    var body = "<body>" + request.GlobalMessageDetailsView.Body + "</body>";
                    var email = new Email() { EmailSubject = request.GlobalMessageDetailsView.Title, EmailBody = body, EmailTo = receipient.Address.Email };
                    _emailRepository.Add(email);
                    auditTracker.Created.Add(email);
                }
            }
            auditTracker.Updated.Add(message);

            //Audit audit = new Audit(AuditMessage , request.RequestUrl, request.UserIP, AuditAction);
            //_auditRepository.Add(audit);
            //auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
         
            try
            {
                _uow.Commit();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Update original values from the database 
                var entry = ex.Entries.Single();
                entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.DatabaseConcurrencyConflict;
                return response;


            }
            
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            response.MessageInfo.Message = "Global Message updated successfully!";
            return response;
        
        }

        public DeleteGMResponse DeletSubmitGm(DeleteGMRequest request)
        {
            DeleteGMResponse response = new DeleteGMResponse();

            if (!request.SecurityUser.IsInAnyRoles("Administrator (Owner)"))
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }
            Message message = _messageRepository.FindBy(request.Id);
            message.IsDeleted = true;
            auditTracker.Updated.Add(message);

            //Audit audit = new Audit(AuditMessage , request.RequestUrl, request.UserIP, AuditAction);
            //_auditRepository.Add(audit);
            //auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);

            try
            {
                _uow.Commit();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Update original values from the database 
                var entry = ex.Entries.Single();
                entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.DatabaseConcurrencyConflict;
                return response;


            }
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            response.MessageInfo.Message = "Global Message deleted successfully!";
            return response;
        }

    }
}
