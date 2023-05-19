using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model;
using CRL.Model.Notification ;
using CRL.Model.Notification .IRepository;
using CRL.Model.FS.Enums;
using CRL.Model.ModelViews;


using CRL.Model.WorkflowEngine;
using CRL.Model.WorkflowEngine.IRepository;
using CRL.Repository.EF.All.Repository.Memberships;
using CRL.Service.BusinessServices;
using CRL.Service.Interfaces;
using CRL.Service.Messaging;
using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Service.Messaging.Institution.Response;
using CRL.Service.Messaging.Memberships.Request;
using CRL.Service.Messaging.Payments.Request;
using CRL.Service.Messaging.Payments.Response;
using CRL.Service.Messaging.Workflow.Request;
using CRL.Service.QueryGenerator;
using CRL.Service.Mappings.Membership;
using CRL.Service.BusinessService;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using CRL.Model.Messaging;
using CRL.Model.ModelService;
using CRL.Model.ModelViewMappers;
using CRL.Model.WorkflowEngine.Enums;
using CRL.Service.Common;
using CRL.Infrastructure.Configuration;
using CRL.Model.ModelViews.Enums;
using CRL.Model.ModelViews;
using CRL.Model.Memberships;
using CRL.Model.ModelViews.Memberships;
namespace CRL.Service.Implementations
{

    public class ClientService:ServiceBase,IClientService
    {
    //{
    //    private readonly IUnitOfWork _uow;
    //    private readonly IMembershipRepository _membershipRepository;
    //    private readonly IWFWorkflowRepository _workflowRepository;
    //    private readonly IWFCaseRepository _caseRepository;
    //    private readonly IEmailTemplateRepository _emailTemplateRepository;
    //    private readonly IEmailUserAssignmentRepository _emailUserAssignmentRepository;
    //    private readonly IEmailRepository _emailRepository;
    //    private readonly IUserRepository  _userRepository;
    //    private readonly IAuditRepository _auditRepository;
    //    private readonly IInstitutionRepository _institutionRepository;
    //    private readonly IAccountTransactionRepository _accountTransactionRepository;
        
    //    public AuditingTracker auditTracker { get; set; }
    //    public DateTime AuditedDate { get; set; }
        

    //    public ClientService(IMembershipRepository membershipRepository,
    //          IWFWorkflowRepository workflowRepository,
    //       IWFCaseRepository caseRepository,
    //        IEmailTemplateRepository emailTemplateRepository,
    //        IEmailUserAssignmentRepository  emailUserAssignmentRepository,
    //         IEmailRepository emailRepository, 
    //        IUserRepository  userRepository,
    //        IAuditRepository auditRepository,
    //        IInstitutionRepository institutionRepository,
    //        IAccountTransactionRepository accountTransactionRepository,

    //        IUnitOfWork uow)
    //    {
    //        AuditedDate = DateTime.Now;
    //        auditTracker = new AuditingTracker();
    //        _membershipRepository = membershipRepository;
    //        _workflowRepository = workflowRepository;
    //        _caseRepository = caseRepository;
    //        _emailTemplateRepository = emailTemplateRepository;
    //        _emailRepository = emailRepository;
    //        _userRepository = userRepository;
    //        _auditRepository = auditRepository;
    //        _institutionRepository = institutionRepository;
    //        _accountTransactionRepository = accountTransactionRepository;
    //        _emailUserAssignmentRepository = emailUserAssignmentRepository;
    //        _uow = uow;
    //    }

        public ResponseBase RevokePostpaidAccount(SetupPostpaidAccountRequest request)
        {
            ResponseBase response = new ResponseBase();
            
Membership  membership = _membershipRepository.FindBy(request.Id);

            bool MemberAndAdministrator =  request.SecurityUser.IsAdministrator() && (request.SecurityUser.MembershipId == membership.RepresentativeId || request.SecurityUser.IsOwnerAdministrator ());
            if (!MemberAndAdministrator)
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

            membership.RevokePostpaidAccount();         
            auditTracker.Updated.Add(membership);


            Email RevokePostpaidEmail = new Email();
            RevokePostpaidEmail.IsSent = false;
            RevokePostpaidEmail.NumRetries = 0;
            EmailTemplate RevokePostpaidEmailTemplate = null;       
            RevokePostpaidEmailTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "RevokePostPaidSetup").SingleOrDefault();
            
            //Get the institution email and the client administrator email and warn if no email appointed
            var user = _userRepository .GetDbSet ().Where (s=>s.MembershipId == membership.Id && (s.Roles .Any(r=>r.Name  == "Administrator (Client)") || s.InstitutionId ==null));
            List<UserEmailView> UserEmails = new List<UserEmailView>();
            foreach (var usr in user)
            {
                UserEmails.Add(new UserEmailView { Id = usr.Id, Email = usr.Address .Email, RecepientType = MailReceipientType.To });
                
            }            
            UserEmails.Add(new UserEmailView { Id = request.SecurityUser .Id , Email = request.SecurityUser .Email , RecepientType = MailReceipientType.CC  });
            RevokePostpaidEmail._emailUserAssignmentRepository = _emailUserAssignmentRepository;
            RevokePostpaidEmail.AddEmailAddresses(UserEmails, auditTracker );
            EmailTemplateGenerator.General(RevokePostpaidEmail ,RevokePostpaidEmailTemplate );
            _emailRepository.Add(RevokePostpaidEmail);
            auditTracker.Created.Add(RevokePostpaidEmail);


            string ClientName = "";
            if (membership.isIndividualOrLegalEntity == 1)
                ClientName = _userRepository.GetDbSet().Where(s => s.MembershipId == membership.Id).Select(r => r.FirstName + " " + r.MiddleName ?? "" + " " + r.Surname).Single();
            else
                ClientName = _institutionRepository.GetDbSet().Where(s => s.MembershipId == membership.Id).Single().Name;
            Audit audit = new Audit("Client Code:"+membership .ClientCode , request.RequestUrl, request.UserIP, AuditAction.RevokePostpaidRequest );
            audit.Message = MembershipAuditMsgGenerator.ClientDetails(membership, ClientName);
            _auditRepository.Add(audit);
            auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
            try
            {
                _uow.Commit();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return new ResponseBase("You have successfully removed the postpaid account of the client", Infrastructure.Messaging.MessageType.Success);

            
        }
        public GetMembershipViewResponse GetDataForMembershipView(GetMembershipViewRequest request)
        {
            //**TEST SECURITY HERE

            GetMembershipViewResponse response = new GetMembershipViewResponse();
            Membership clientMembership;
                 string RepresentativeMembershipName=null;
            
            if (request.SecurityUser.IsOwnerUser)
            {
                if (request.MembershipId != null)
                {
                    clientMembership = _membershipRepository.FindBy((int)request.MembershipId);
                    if (clientMembership.RepresentativeId != null)
                    {
                        RepresentativeMembershipName = _institutionRepository.GetDbSet ().Where(s => s.MembershipId == clientMembership.RepresentativeId).Single().Name;
                    }
                    response.MembershipView = clientMembership.ConvertToMembershipView(RepresentativeMembershipName);
                }
            }
            else
            {
                 clientMembership = _membershipRepository.FindBy(request.SecurityUser.MembershipId);
                 if (clientMembership.RepresentativeId != null)
                 {
                     RepresentativeMembershipName = _institutionRepository.GetDbSet().Where(s => s.MembershipId == clientMembership.RepresentativeId).Single().Name;
                 }
                 response.MembershipView = clientMembership.ConvertToMembershipView(RepresentativeMembershipName);
            }
          
            return response;
        }

        public GetSubmitPostpaidResponse GetSubmitPostpaid(RequestBase request)
        {
            GetSubmitPostpaidResponse response = new GetSubmitPostpaidResponse();
                        
            //Only Administrators are allowed here, in the future we might consider CRN doing so
            if (!request.SecurityUser.IsClientAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

           
            Membership membership = _membershipRepository.FindBy(request.SecurityUser.MembershipId); //**Load with representative
           

            //We cannot submit if we are alreayd postpaid
            if (membership.HasValidPostpaidAccount())
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo 
                { MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError, Message ="A valid postpaid account exists on this account" };
                return response;
            }

            if (membership.PendingPostpaidAccount == true)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError, Message = "This account has a pending postpaid account request!" };
                response.HasPendingPostpaidAccount = true;
                return response;
            }         

            //We need to check
            response.RegularClientInstitutions = LookUpServiceModel.RegularInstitutions(_institutionRepository);
            if (request.SecurityUser.InstitutionId != null)
            {
                response.CurrentClientIsBank = _institutionRepository.GetDbSet().Any(i => i.Id == request.SecurityUser.InstitutionId && Constants.SecuredPartyBankType.Contains((int)i.SecuringPartyTypeId));
            }
            else
            {
                response.CurrentClientIsBank = false;
            }

            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            response.MessageInfo.Message = "Successfully added new client";
            return response;
        }



        public ViewPostpaidClientsResponse ViewPostpaidClients(ViewPostpaidClientsRequest request)
        {
            ViewPostpaidClientsResponse response = new ViewPostpaidClientsResponse();

            if (!request.SecurityUser.IsOwnerAdminRegistrySupport() && !request .SecurityUser .IsClientAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

            response = _membershipRepository.CustomViewPostpaidClients(request);
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
           
          
        }

        public ResponseBase SubmitPostpaidAccount(SetupPostpaidAccountRequest request)
        {
            Role AdminRole = _roleRepository.FindBy(Roles.AdminClient); //**Use AdminRoleConst as int
            MembershipServiceModel msServiceModel = new MembershipServiceModel(_userRepository, _institutionRepository, _roleRepository, auditTracker, request.SecurityUser);
            WorkflowServiceModel wfServiceModel = new WorkflowServiceModel(_emailTemplateRepository, _workflowRepository, _emailRepository, _emailUserAssignmentRepository, _userRepository, _caseRepository, auditTracker, request.SecurityUser);
            ServiceRequest serRequest = null;
            ResponseBase response = new ResponseBase();

            #region Validation After Data Load (Unique Code, Payment)
            //Load the service request
            serRequest = _serviceRequestRepository.GetDbSet().Where(s => s.RequestNo == request.UniqueGuidForm).SingleOrDefault();

            //For only create, submit and resubmit, check that the service is unique
            if (this.ValidateUniqueServiceRequest(request.UniqueGuidForm, serRequest) == false)
            {
                response.SetResponseToResponseBase(this.ReturnAlreadyRequestedResponse()); return response;
            }

       
            //Only Administrators are allowed here, in the future we might consider CRN doing so
            if (!request.SecurityUser.IsClientAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

            if (request.MembershipView.MembershipAccountTypeId == MembershipAccountCategory.Regular)
            {
                request.MembershipView.RepresentativeMembershipId = Constants.RegistryMembershipId;
            }
            else if (request.MembershipView.MembershipAccountTypeId == MembershipAccountCategory.RegularRepresentative)
            {
                if (request.MembershipView.RepresentativeMembershipId == null)
                {
                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError, Message = "The representative bank was not included!" };
                }
            }
            else
            {
                throw new Exception("Membership type is not accepted here!");
            }

            #endregion

            Membership membership = _membershipRepository.FindBy(request.SecurityUser.MembershipId);
            Institution representativeInstitution = _institutionRepository.GetDbSet().Where(s => s.MembershipId == request.MembershipView.RepresentativeMembershipId
               && s.IsDeleted == false && s.IsActive == true).SingleOrDefault();
            if (representativeInstitution == null)
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }       

            string ClientName = _institutionRepository.GetDbSet ().Where (s=>s.MembershipId == membership.Id ).Single ().Name ;

            //We cannot submit if we are alreayd postpaid
            if (membership.HasValidPostpaidAccount())
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError, Message = "A valid postpaid account exists on this account" };
                return response;
            }

            if (membership.PendingPostpaidAccount == true)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError, Message = "This account has a pending postpaid account request!" };
                response.HasPendingPostpaidAccount = true;
                return response;
            }

         
           

           // membership.SetupPostpaidAccount(request.MembershipView);                 
            membership.PendingPostpaidAccount = true;
            //Load the workflow entities and create a new case
            WFCasePostpaidSetup _case = (WFCasePostpaidSetup)wfServiceModel.InitialiseCase(6, WorkflowRequestType.Membership, request.Comment, null);
            _case.CaseTitle = _case.CaseTitle + " - " + ClientName;
            _case.AccountNumber = request.MembershipView.AccountNumber;
            _case.MembershipAccountTypeId = request.MembershipView.MembershipAccountTypeId;
            _case.MembershipId = request.SecurityUser.MembershipId;

            _case.RepresentativeMembershipId = request.MembershipView.RepresentativeMembershipId;
            _case.LimitedToOtherMembershipId = _case.MembershipAccountTypeId == MembershipAccountCategory.RegularRepresentative ? _case.RepresentativeMembershipId : 1;
            
            _caseRepository.Add(_case);
            auditTracker.Created.Add(_case);

            wfServiceModel.ProcessCase(WFTaskType.PostpaidSetup, membership.ClientCode, _case.MembershipAccountTypeId == MembershipAccountCategory.RegularRepresentative ?_case.RepresentativeMembershipId:1);

            #region auditing and committing
            //Audititng Processing
            AuditMessage = MembershipAuditMsgGenerator.ClientDetails(membership, ClientName);
            AuditAction = Model.FS.Enums.AuditAction.SubmitPostpaidRequest ;
            var responseAfterCommit = this.AuditCommitWithConcurrencyCheck(AuditMessage, request.RequestUrl, request.UserIP, AuditAction, request.SecurityUser.Id);
            if (responseAfterCommit.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(responseAfterCommit); return response; }
            #endregion

            //Return a success response
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }
        public ResponseBase HandlePostpaidAccount(HandleWorkItemRequest prequest)
        {
            SetupPostpaidAccountRequest request = (SetupPostpaidAccountRequest)prequest;
            ResponseBase response = new ResponseBase();
            ServiceRequest serRequest = null;
            EmailTemplate ApproveDenyEmailTemplate = null;
            WorkflowServiceModel wfServiceModel = new WorkflowServiceModel(_emailTemplateRepository, _workflowRepository, _emailRepository, null, _userRepository, _caseRepository, auditTracker, request.SecurityUser);
            Membership membership = null;          
             
          
            #region Load Necessary Data
            serRequest = _serviceRequestRepository.GetDbSet().Where(s => s.RequestNo == request.UniqueGuidForm).SingleOrDefault();
            WFCasePostpaidSetup _case = (WFCasePostpaidSetup)wfServiceModel.InitialiseCase(6, WorkflowRequestType.Membership , request.Comment, request.CaseId);
            Institution institution = _institutionRepository.GetDbSet().Where(s => s.MembershipId == _case.Membership.Id).Single();
            //Generate template         
            ApproveDenyEmailTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == 
                (request.RequestMode  == RequestMode.Approval ? "ApprovePostPaidSetup" : "DenyPostPaidSetup")).SingleOrDefault();
            membership = _case.Membership;
            string ClientName = _institutionRepository.GetDbSet().Where(s => s.MembershipId == membership.Id).Single().Name;
            #endregion

            #region validation
            //Make sure this is an administrative account user
            if (!request.SecurityUser.IsAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }


            //Load the service request
            serRequest = _serviceRequestRepository.GetDbSet().Where(s => s.RequestNo == request.UniqueGuidForm).SingleOrDefault();

            //For only create, submit and resubmit, check that the service is unique
            if (this.ValidateUniqueServiceRequest(request.UniqueGuidForm, serRequest) == false)
            {
                response.SetResponseToResponseBase(this.ReturnAlreadyRequestedResponse()); return response;
            }

            //Make sure that the case is not close or u  know the workitem is not already handled.  Will have to do this in the case servicet
            if (_case == null)
            {
                throw new Exception("During an approval / deny / of client postpaid submission.  Case Id:" + request.CaseId);
            }
            else
            {
                if (_case.WorkItems.Where(s => s.Id == request.WorkItemId).Single().WorkitemStatus == "" || _case.CaseStatus == "CL")
                {
                    response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.AlreadyHandled;
                    response.MessageInfo.Message = "This item has already been handled or closed!";
                    return response;
                }


            }

            //Load the case and deal with it
            if (_case.MembershipAccountTypeId == MembershipAccountCategory.Regular)
            {
                _case.RepresentativeMembershipId = Constants.RegistryMembershipId;
            }
            else if (_case.MembershipAccountTypeId == MembershipAccountCategory.RegularRepresentative)
            {
                if (_case.RepresentativeMembershipId  == null)
                {
                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError, Message = "The representative bank was not included!" };
                }
            }
            else
            {
                throw new Exception("Membership type is not accepted here!");
            }


            Institution representativeInstitution = _institutionRepository.GetDbSet().Where(s => s.MembershipId == _case.RepresentativeMembershipId 
              && s.IsDeleted == false && s.IsActive == true).SingleOrDefault();
            if (representativeInstitution == null)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError, Message = "Cannot continue with this preocess because either the representative client has been deleted or is invalid!" };
                return response;
            }

             //We cannot submit if we are alreayd postpaid
            if (membership.HasValidPostpaidAccount())
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError, Message = "A valid postpaid account exists on this account" };
                return response;
            }

           


            #endregion

            //Lets process the workflow
            PerformWorkItemResponse wrkItemResponse = wfServiceModel.ProcessWorkItem(request.WorkItemId);
            if (wrkItemResponse.Success == false)
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;

            }


            membership.PendingPostpaidAccount = false;
            if (request.RequestMode == RequestMode.Approval)
            {
                MembershipView mv = new MembershipView
                {
                    AccountNumber = _case.AccountNumber,
                    MembershipAccountTypeId = _case.MembershipAccountTypeId,
                  
                };
                if   (_case.MembershipAccountTypeId == MembershipAccountCategory.RegularRepresentative)   
                   mv. RepresentativeMembershipId =   _case.RepresentativeMembershipId;

                membership.SetupPostpaidAccount(mv);
              
            }
            else if (request.RequestMode != RequestMode.Deny)
            {
                throw new Exception("Unknown request type was found when handling new client account.  Request type must be Approval Or Deny");
            }
           

            //Create the approval or deny mail
            Email ApproveDenyEmail = new Email();
            _emailRepository.Add(ApproveDenyEmail);
            auditTracker.Created.Add(ApproveDenyEmail);
            ApproveDenyEmail.EmailTo = _case.CreatedByUser.Address.Email;
            EmailTemplateGenerator.ApproveOrDenyPostpaid(request.PostpaidRequestMode == RequestMode.Approval,ApproveDenyEmail
                ,ApproveDenyEmailTemplate,request.Comment );

            //Assign email to the user
            EmailUserAssignment emailAssignment = new EmailUserAssignment();
            emailAssignment.AssignEmailToUser(ApproveDenyEmail, _case.CreatedByUser.Id);
            _emailUserAssignmentRepository.Add(emailAssignment);
            auditTracker.Created.Add(emailAssignment);


            #region auditing and committing
            //Audititng Processing
            AuditMessage = MembershipAuditMsgGenerator.ClientDetails(membership, ClientName);
            AuditAction = request.RequestMode  == RequestMode.Deny ? Model.FS.Enums.AuditAction.DenyPostpaidRequest  : AuditAction = Model.FS.Enums.AuditAction.AuthorizePostpaidRequest ;
            var responseAfterCommit = this.AuditCommitWithConcurrencyCheck(AuditMessage, request.RequestUrl, request.UserIP, AuditAction, request.SecurityUser.Id);
            if (responseAfterCommit.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(responseAfterCommit); return response; }
            #endregion

            //Return a success response
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;            
            response.MessageInfo.Message = request.RequestMode == RequestMode.Deny ? "You have denied the postpaid account setup.  An email will be sent to the client notifying them of the denial of their account" :
                "You have successfully approved the postpaid account setup.  An email will be sent to the client notifying them of the authorization!";
            return response;
            



 
        


        }

        public ResponseBase HandleTask(HandleWorkItemRequest request)
        {
            ////Load the workflow and case, note we must also know the workitem                 

            WFCasePostpaidSetup _case = _caseRepository.GetSpecializedWFCaseById<WFCasePostpaidSetup>(request.CaseId);

            ////Now let's get the workitem we chose
            WFWorkItem workItem = _case.WorkItems.Where(s => s.Id == request.WorkItemId).Single();

            ResponseBase response = null;
            SetupPostpaidAccountRequest request2 = new SetupPostpaidAccountRequest();
            request2.WorkItemId = request.WorkItemId;
            request2.CaseId = request.CaseId;
            request2.Comment = request.Comment;
            request2.SecurityUser = request.SecurityUser;
            request2.RequestUrl = request.RequestUrl;
            request2.UserIP = request.UserIP;
            if (workItem.TransitionId == 19)
                request2.RequestMode  = RequestMode.Approval;
            else if (workItem.TransitionId == 20)
                request2.RequestMode  = RequestMode.Deny;
            else
                throw new Exception("Unexpected code to be reached.  Possible workflow setup error.  Apart from deny and approve postpaid no other transiotion was defined during development");


            if (workItem.TransitionId == 19 || workItem.TransitionId == 20)
            {
                response = this.HandlePostpaidAccount(request2);
            }

            //else
            //{
            //    //Load the workflow and case, note we must also know the workitem                 

            //    List<Email> Emails = new List<Email>();

            //    //Now let's get the workitem we chose
            //    WorkflowFactory.PerformWorkItem(_case, request.WorkItemId, request.Comment, request.SecurityUser,
            //               _userRepository, _emailTemplateRepository, Emails, auditTracker, false);
            //    foreach (var mails in Emails)
            //    {
            //        _emailRepository.Add(mails);
            //    }
            //    _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
            //    _uow.Commit();

            //    response = new ResponseBase();
            //    //response.FSView   = request (financialStatement.Id);
            //    response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            //    response.MessageInfo.Message = "Success";

            //    return response;
            //}

            return response;
        }

        public ViewAuditsResponse ViewAudits(ViewAuditsRequest request)
        {
            ViewAuditsResponse response = new ViewAuditsResponse();
            //if (!request.SecurityUser.IsOwnerAdminRegistrySupport() && !request.SecurityUser .IsClientAdministrator () && !request.SecurityUser .IsUnitAdministrator())
            //{
            //    response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
            //    return response;
            //}
            if (!request.SecurityUser.IsAudit())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            response = _auditRepository.CustomViewAudits(request);
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
   
         
        }

        public ViewAuditDetailsResponse ViewAuditDetails(ViewAuditDetailsRequest request) 
        {
            ViewAuditDetailsResponse response = new ViewAuditDetailsResponse();
            if (!request.SecurityUser.IsAudit())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }
            response = _auditRepository.AuditDetails(request);
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }

        public ViewCreditActivitiesResponse ViewCreditActivities(ViewCreditActivitiesRequest request)
        {
            ViewCreditActivitiesResponse response = new ViewCreditActivitiesResponse();
            if (!request.SecurityUser.IsOwnerAdminRegistrySupport() && !request.SecurityUser.IsInRole("CRL Finance Officer") && !request.SecurityUser.IsClientAdministrator()
               && !request.SecurityUser.IsInRole("Finance Officer")  && !request.SecurityUser.IsUnitAdministrator())
            {
                response.SetResponseToResponseBase(this.ReturnUnAuthorizedResponse());
                return response;
            }

            response = _accountTransactionRepository.CustomViewCreditActivities(request);
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;

          
        }


    }
}
