using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Domain;
using CRL.Model.Notification;
using CRL.Model.Notification.IRepository;
using CRL.Model.ModelViews;

using CRL.Model.WorkflowEngine.IRepository;
using CRL.Infrastructure.Helpers;
using CRL.Model.WorkflowEngine.Enums;
using CRL.Model.Configuration;
using CRL.Model.Configuration.IRepository;
using CRL.Model.Memberships;
using CRL.Model.Memberships.IRepository;

namespace CRL.Model.WorkflowEngine
{
    public class PerformWorkItemResponse
    {
        public enum PerformWorkItemError { UnauthenticatedUser = 1 }
        public bool Success { get; set; }
        public PerformWorkItemError Error { get; set; }
    }
    public class WorkflowServiceModel
    {
        private IEmailTemplateRepository _emailTemplateRepository;
        private IWFWorkflowRepository _workflowRepository;
        private IEmailRepository _emailRepository;
        private IEmailUserAssignmentRepository _emailUserAssignmentRepository;
        private IUserRepository _userRepository;
        private IWFCaseRepository _caseRepository;
        private WFCase _case = null;
        private AuditingTracker _tracker;
        private SecurityUser _executingUser;
        private WFWorkflow wf;
        private WorkflowManager mgr;
        private WFTaskType wfTaskType;
        public WorkflowServiceModel(IWFWorkflowRepository workflowRepository, IUserRepository userRepository, SecurityUser user)
        {
            _workflowRepository = workflowRepository;
            _userRepository = userRepository;
            _executingUser = user;
        }
        public WorkflowServiceModel(IEmailTemplateRepository emailTemplateRepository,
             IWFWorkflowRepository workflowRepository,
             IEmailRepository emailRepository, IEmailUserAssignmentRepository emailUserAssignmentRepository, IUserRepository userRepository, IWFCaseRepository caseRepository,
            AuditingTracker tracker, SecurityUser user)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _emailUserAssignmentRepository = emailUserAssignmentRepository;
            _workflowRepository = workflowRepository;
            _emailRepository = emailRepository;
            _userRepository = userRepository;
            _caseRepository = caseRepository;
            _tracker = tracker;
            _executingUser = user;
        }

        public WFCase InitialiseCase(int WorkflowId, WorkflowRequestType caseType, string Comment, int? CaseId = null)
        {
            wf = _workflowRepository.GetWFWorkflowById(WorkflowId);

            if (CaseId == null || CaseId == 0)
            {
                if (caseType == WorkflowRequestType.FinancialStatement)
                {
                    _case = new WFCaseFS();
                    _case.CaseTitle = "Authorize Financing Statement";

                }
                else if (caseType == WorkflowRequestType.FinancialStatementActivity)
                {
                    _case = new WFCaseActivity();
                    _case.CaseTitle = "Authorize Financing Statement Change";

                }
                else if (caseType == WorkflowRequestType.Membership)
                {
                    _case = new WFCasePostpaidSetup();
                    _case.CaseTitle = "Authorize postpaid account";
                }

                else if (caseType == WorkflowRequestType.MembershipRegistration)
                {
                    _case = new WFCaseMembershipRegistration();
                    _case.CaseTitle = "Authorize client account";
                }

                else if (caseType == WorkflowRequestType.PaypointUserAssigment)
                {
                    _case = new WFCasePaypointUsersAssignment();
                }
                else
                {
                    throw new Exception("Illegal case type");
                }

                // _caseRepository.Add(_case);
                //_tracker.Created.Add(_case);
                _case.CaseType = caseType;
                _case.Workflow = wf;
                _case.WorkflowId = wf.Id;



            }
            else
            {
                if (caseType == WorkflowRequestType.FinancialStatement)
                {
                    _case = _caseRepository.GetSpecializedWFCaseById<WFCaseFS>((int)CaseId);
                }
                else if (caseType == WorkflowRequestType.FinancialStatementActivity)
                {
                    _case = _caseRepository.GetSpecializedWFCaseById<WFCaseActivity>((int)CaseId);
                }
                if (caseType == WorkflowRequestType.Membership)
                {
                    _case = _caseRepository.GetSpecializedWFCaseById<WFCasePostpaidSetup>((int)CaseId);
                }
                if(caseType == WorkflowRequestType.MembershipRegistration)
                {
                    _case = _caseRepository.GetSpecializedWFCaseById<WFCaseMembershipRegistration>((int)CaseId);
                }

            }

            mgr = new WorkflowManager(_case, _tracker);
            mgr.UserComment = Comment;
            mgr.ExecutingUser = _userRepository.FindBy(_executingUser.Id);
            return _case;

        }
        public void ProcessCase(WFTaskType wfTaskType, string ContextCode, int? membershipid = null)
        {

            List<EmailUserAssignment> emailAssignments = new List<EmailUserAssignment>();
            //Generate template
            EmailTemplate AssignTaskTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "NewTaskAssignment").SingleOrDefault();

            //WFCase _case = new WFCase();
            mgr.ProcessNewCase(ContextCode, _case);

            //Case should have membership
            _case.LimitedToOtherMembershipId = membershipid;
            _case.TaskType = wfTaskType;

            //Get the list of permissible users
            UserWorkflowAssignmentService uas = new UserWorkflowAssignmentService(_userRepository, _case, _executingUser, wfTaskType, true, membershipid);
            List<User> AssignedUsers = uas.Users ?? new List<User>();
            List<string> caseEmails = uas.UserMails;
            string AssignedEmails = "";

            //Add the emails of the permissble users to receive notification
            foreach (var mail in caseEmails)
            {
                AssignedEmails = AssignedEmails + mail + ";";
                _case.CaseMails.Add(new WFCaseMail { Email = mail, CreatedBy = _executingUser.Id, CreatedOn = DateTime.Now, IsActive = true });
            }



            AssignedEmails = String.IsNullOrWhiteSpace(AssignedEmails) ? AssignedEmails.TrimEnd(';') : AssignedEmails.TrimNull();

            //If no permissble user please send this to the Administrator of the institution sending out this email
            if (String.IsNullOrWhiteSpace(AssignedEmails))
            {

                List<User> users = _userRepository.GetDbSet().Where(s => s.MembershipId == _executingUser.MembershipId && s.Roles.Any(d => d.Id == Roles.AdminClient || d.Id == Roles.AdminOwner) && s.IsActive == true && s.IsDeleted == false && s.InBuiltUser == false).ToList();
                foreach (var usr in users)
                {
                    AssignedUsers.Add(usr);
                    AssignedEmails = AssignedEmails + usr.Address.Email + ";";

                }
                AssignedEmails = String.IsNullOrWhiteSpace(AssignedEmails) ? AssignedEmails.TrimEnd(';') : AssignedEmails.TrimNull();
            }

            Notification.Email AssignTaskMail = new Notification.Email();
            //We need to assign to all those assigned and also coppy the user who sent this mail

            //After approval please send to the client approving and creating

            AssignTaskMail.EmailCc = _executingUser.Email;  //Workflowemails not added but will be part of this list
            AssignTaskMail.IsSent = false;
            AssignTaskMail.NumRetries = 0;
            AssignTaskMail.EmailTo = AssignedEmails;
            //AssignTaskMail.EmailSubject = AssignTaskTemplate.EmailSubject;


            foreach (var usr in AssignedUsers)
            {
                EmailUserAssignment e = new EmailUserAssignment();
                e.User = usr;
                e.Email = AssignTaskMail;
                e.IsActive = true;
                _emailUserAssignmentRepository.Add(e);
                _tracker.Created.Add(e);
            }

            

            WorkflowTemplate ml = new WorkflowTemplate();
            ml.LoadTemplate(_case);
            EmailTemplateGenerator.AssignNewTaskMail(AssignTaskMail, ml, AssignTaskTemplate);
            _emailRepository.Add(AssignTaskMail);
            _tracker.Created.Add(AssignTaskMail);








            ////We need to get users       //If no user is found then warn.  When we create users we will need to rassign roles  
            //   UserAssignmentRuleProcessorService uaRps = new UserAssignmentRuleProcessorService(_userRepository );
            //   UserNotificationRuleProcessorService unRps = new UserNotificationRuleProcessorService(_userRepository);
            //   List<User> AssignedUsers = uaRps.GenerateAssignedUsersForWorkItems(_case, ExecutingUserId,membershipid );
            //   List<string> caseEmails = unRps.GetEmailsFromRule(_case, ExecutingUserId, AssignedUsers);
            //   foreach (var mail in caseEmails)
            //   {
            //       _case.CaseMails.Add(new WFCaseMail { Email = mail, CreatedBy =ExecutingUserId, CreatedOn = DateTime.Now, IsActive = true });
            //   }



        }
        public WFCase GetCase()
        {
            return _case;
        }
        public PerformWorkItemResponse ProcessWorkItem(int WorkItemId, bool skipAssignedUser = false, int? MembershipIdLimit=null)
        {
            PerformWorkItemResponse response = new PerformWorkItemResponse();
            //Validate that only assigned users can be here

          
            var token =
                _case.Tokens.Where(s => s.TokenStatus == "FREE" && s.IsActive == true && s.IsDeleted == false).
                    SingleOrDefault();

                
                   var ids = token.AssignedUsers.Select(s => s.Id);
                   if (!ids.Contains(_executingUser.Id))
                   {
                       response.Success = false;
                       response.Error = PerformWorkItemResponse.PerformWorkItemError.UnauthenticatedUser;
                       return response;
                   }
           

            //WorkflowManager mgr = new WorkflowManager(_case);
            //mgr.ExecutingUser = _userRepository.FindBy(_executingUser.Id);
            //mgr.UserComment = Comment;
            //Now let's get the workitem we chose
            mgr.PerformWorkItem(_case.WorkItems.Where(s => s.Id == WorkItemId).Single());


            if (skipAssignedUser)
            {
                response.Success = true;
                return response;
            }
            //**In case final does not send mail then we will do so here
            if (_case.CaseStatus != "CL")
            {
                UserWorkflowAssignmentService uas = new UserWorkflowAssignmentService(_userRepository, _case, _executingUser, _case.TaskType, true, MembershipIdLimit);

                List<User> AssignedUsers = uas.Users;
                List<string> caseEmails = uas.UserMails;
                string AssignedEmails = "";

                foreach (var mail in caseEmails)
                {
                    AssignedEmails = AssignedEmails + mail + ";";
                    _case.CaseMails.Add(new WFCaseMail { Email = mail, CreatedBy = _executingUser.Id, CreatedOn = DateTime.Now, IsActive = true });
                }
                AssignedEmails = String.IsNullOrWhiteSpace(AssignedEmails) ? AssignedEmails.TrimEnd(';') : AssignedEmails.TrimNull();
                if (String.IsNullOrWhiteSpace(AssignedEmails))
                {

                    List<User> users = _userRepository.GetDbSet().Where(s => s.MembershipId == _executingUser.MembershipId && s.Roles.Any(d => d.Id == Roles.AdminClient || d.Id == Roles.AdminOwner) && s.IsActive == true && s.IsDeleted == false && s.InBuiltUser == false).ToList();
                    foreach (var usr in users)
                    {
                        AssignedEmails = AssignedEmails + usr.Address.Email + ";";

                    }
                    AssignedEmails = String.IsNullOrWhiteSpace(AssignedEmails) ? AssignedEmails.TrimEnd(';') : AssignedEmails.TrimNull();
                }
                //**Send mail to Administrator telling him that no user has been assigned to this task when it was expecting

                Notification.Email AssignTaskMail = new Notification.Email();
                //We need to assign to all those assigned and also coppy the user who sent this mail

                //After approval please send to the client approving and creating
                AssignTaskMail.EmailCc = _executingUser.Email;  //Workflowemails not added but will be part of this list
                AssignTaskMail.IsSent = false;
                AssignTaskMail.NumRetries = 0;

                //Generate template
                EmailTemplate AssignTaskTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "NewTaskAssignment").SingleOrDefault();

                WorkflowTemplate ml = new WorkflowTemplate();
                ml.LoadTemplate(_case);
                EmailTemplateGenerator.AssignNewTaskMail(AssignTaskMail, ml, AssignTaskTemplate);
                _emailRepository.Add(AssignTaskMail);
                _tracker.Created.Add(AssignTaskMail);







            }
            //else
            //{
            //    Email.Email closeCaseMail = new Email.Email();
            //    closeCaseMail.EmailTo = securityUser.Email;  //Workflowemails not added but will be part of this list
            //    closeCaseMail.IsSent = false;
            //    closeCaseMail.NumRetries = 0;

            //    //Generate template
            //    EmailTemplate AssginTaskTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "AssignTaskTemplate").SingleOrDefault();
            //    EmailTemplateGenerator.FinancingStatementApproval(AssginTaskMail, financialStatement, AssginTaskMail);
            //    Emails.Add(closeCaseMail);

            //}

            response.Success = true;
            return response;
        }
        public void InitializeWorkflow(WFTaskType taskType, bool WorkflowOn=true)
        {
            int WorkflowId;
            wfTaskType = taskType;
            if (taskType == WFTaskType.CreateRegistration || taskType == WFTaskType.UpdateRegistration)
                WorkflowId = 4;
            else if (taskType == WFTaskType.SubordinateRegistration || taskType == WFTaskType.DischargeRegistration || taskType == WFTaskType.DischargeRegistrationDueToError)
                WorkflowId = 6;
            else if (taskType == WFTaskType.AssignRegistration && WorkflowOn == true)
                WorkflowId = 5;
            else if (taskType == WFTaskType.AssignRegistration && WorkflowOn == false)
                WorkflowId = 6;
            else
                throw new Exception();
            wf = _workflowRepository.GetWFWorkflowById(WorkflowId);
        }
        public List<User> LoadAssignedUsersFromWorkflow(int? LimitToInstitutionId=null)
        {
            List<User> AssignedUsers;
            //This task starts a workflow so please load the workflow           
            List<WFPlace> _places = WorkflowManager.GetPlacesFromWorkflow(wf);
            if (_places.Count > 0)
            {

                //We need to get users       //If no user is found then warn.  When we create users we will need to rassign roles  
                UserWorkflowAssignmentService uaRps = new UserWorkflowAssignmentService(_userRepository, _places, _executingUser, wfTaskType,LimitToInstitutionId );
                AssignedUsers = uaRps.Users;
                return AssignedUsers;

            }
            else
                return null;



        }

    }
}
