using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Messaging;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.Messaging;
using CRL.Model.Model.FS;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.ModelViewValidators.FinancingStatement;
using CRL.Model.Notification;
using CRL.Model.Payments;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.WorkflowEngine;
using CRL.Model.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Model.ModelService.FS
{
    public abstract class ChangeServiceModel:FSServiceModelBase
    {
        public EmailTemplate FSActivityMailTemplate { get; set; }
        
        
       /// <summary>
       /// Validates the authorization requirement for performing a financing statement change
       /// </summary>
       /// <returns></returns>
        public bool ValidateSecurity(FinancialStatementActivityCategory  activityType , RequestMode request)
        {
            if (request == RequestMode.Create  && 
                activityType == FinancialStatementActivityCategory.FullDicharge
                && _executingUser.IsInRole("Registrar"))
            {
                return true;
            }
            else if (request == RequestMode.Submit || request == RequestMode.Create || request == RequestMode.Resend )
            {
                if (_executingUser.IsInAnyRoles("Client Officer", "Amend Officer"))
                    return true;
                if (activityType == FinancialStatementActivityCategory.Update && _executingUser.IsInRole("Update Officer"))
                    return true;
                if (activityType == FinancialStatementActivityCategory.FullDicharge && (_executingUser.IsInRole ("Cancellation Officer")))
                    return true;
                if (activityType == FinancialStatementActivityCategory.DischargeDueToError && (_executingUser.IsInRole("Cancellation Officer")))
                    return true;
                if (activityType == FinancialStatementActivityCategory.Subordination && _executingUser.IsInRole("Subordinate Officer"))
                    return true;
                if (activityType == FinancialStatementActivityCategory.FullAssignment && _executingUser.IsInRole("Transfer Officer"))
                    return true;
            }
            else if (request == RequestMode.Approval || request == RequestMode.Deny )
            {

                if (_executingUser.IsInAnyRoles("Client Authorizer", "Amend Authorizer"))
                    return true;
                if (activityType == FinancialStatementActivityCategory.Update && _executingUser.IsInRole("Update Authorizer"))
                    return true;
                if (activityType == FinancialStatementActivityCategory.FullDicharge && (_executingUser.IsInRole("Cancellation Authorizer")))
                    return true;
                if (activityType == FinancialStatementActivityCategory.DischargeDueToError && (_executingUser.IsInRole("Cancellation Authorizer")))
                    return true;
                if (activityType == FinancialStatementActivityCategory.Subordination && _executingUser.IsInRole("Subordinate Authorizer"))
                    return true;
                if (activityType == FinancialStatementActivityCategory.FullAssignment && _executingUser.IsInRole("Transfer Authorizer"))
                    return true;
            }
          
           return false;
        }
        public void InitialisePayment(PaymentServiceModel paymentServiceModel, FSActivityRequest request, int LenderType)
        {
            if (request is UpdateFSRequest)
            {
                paymentServiceModel.InitialisePayment(ServiceFees.UpdateOfFinancingStatement, LenderType);
            }
            else if (request is DischargeFSRequest)
            {
                paymentServiceModel.InitialisePayment(ServiceFees.DischargeofFinancingStatement, LenderType);
            }
            else if (request is SubordinateFSRequest)
            {
                paymentServiceModel.InitialisePayment(ServiceFees.SubordinationOfFinancingStatement, LenderType);
            }
            else if (request is AssignFSRequest )
            {
                paymentServiceModel.InitialisePayment(ServiceFees.AssignmentOfFinancingStatement, LenderType);
            }
        }
        public static ResponseBase ValidateFSViewState(FSView FSView, FinancialStatementActivityCategory selectedAmendmentType, SecurityUser user, bool IgnoreCheckingForPending=false, bool IsResubmit=false)
        {
            ResponseBase response = new ResponseBase();
            // Major validation checkup here
            if ((FSView.IsPendingAmendment == true && IgnoreCheckingForPending == false) || FSView.IsDischarged == true || FSView.IsExpired || (FSView.IsApproved == false && IsResubmit ==false)
                )
            {
                response.MessageInfo.MessageType = MessageType.Error;
                response.MessageInfo.Message = "Cannot perform any activity on this financing statement because of one or more of the reasons: Pending an exisiting activity, expired, discharged or unauthorized";
                return response;
            }

            if (FSView.MembershipId != user.MembershipId)
            {
                if (!(user.IsOwnerRegistry() && selectedAmendmentType == FinancialStatementActivityCategory.FullDicharge && selectedAmendmentType == FinancialStatementActivityCategory.DischargeDueToError) &&
                      !(selectedAmendmentType == FinancialStatementActivityCategory.FullAssignment && IgnoreCheckingForPending == true))
                {
                    response.GenerateDefaultUnauthorisedMessage();
                    return response;
                }
            }
            else
            {
                //Check that user is in same unit or in globa unit
                if (user.InstitutionUnitId != null && user.InstitutionUnitId != FSView.InstitutionUnitId)
                {
                    response.GenerateDefaultUnauthorisedMessage();
                    return response;
                }

            }

            response.GenerateDefaultSuccessMessage();
            return response;

        }
        public PaymentInfoResponse ValidatePayment(PaymentServiceModel paymentServiceModel, FSActivityRequest request)
        {
          
                return paymentServiceModel.ValidatePayment();
            

        }
        public abstract FinancialStatementActivity  Deny();
        public virtual int GetCurrentFSId()
        {
            return submittedFS.Id;
        }
        public virtual FinancialStatement  GetCurrentFS()
        {
            return submittedFS;
        }

        public void SetLastActivity(FinancialStatementActivityCategory category, bool Pending)
        {
            string PrefixPending = Pending ? "Pending " : "";
            this.GetCurrentFS().FinancialStatementLastActivity = category == FinancialStatementActivityCategory.Update ? PrefixPending+ "Update":
                category == FinancialStatementActivityCategory.Subordination ? PrefixPending+ "Subordination":
                category == FinancialStatementActivityCategory.FullAssignment ? PrefixPending+"Transfer" :
                category == FinancialStatementActivityCategory.DischargeDueToError ? PrefixPending + "Cancellation due to error" :
                    category == FinancialStatementActivityCategory.FullDicharge ? PrefixPending+ "Cancellation":"";
            this.GetCurrentFS().isPendingAmendment = Pending;
        }
       
        public void Deny(FinancialStatementActivity activity)
        {
            activity.FinancialStatement.isPendingAmendment = false;
            activity.FinancialStatement.FinancialStatementLastActivity = activity.PreviousActivity;
        }
        public abstract FinancialStatementActivity Change(FSActivityRequest request, bool SetUniqueNoLater, WFCaseActivity _case = null);
        public abstract void LoadInitialDataFromRepository(FSActivityRequest request, WFCaseActivity _case=null); //**We should let this call but the appropirate class will handle it
        public void GenerateFSActivityDenyEmail(FinancialStatementActivity fsActivity, ChangeServiceModel changeServiceModel,  List<UserEmailView> UserEmailView, string Comment)
        {
            EmailTemplate FSActivityEmailTemplate = changeServiceModel.FSActivityMailTemplate;

            Email Notification = new Email();
            Notification._emailUserAssignmentRepository = _emailUserAssignmentRepository;
            Notification.AddEmailAddresses(UserEmailView, _tracker);

         


            _emailRepository.Add(Notification);
            _tracker.Created.Add(Notification);
            //EmailTemplateGenerator.CreateFinancingStatementMail(Notification, submittedFS, FSActivityEmailTemplate);
            EmailTemplateGenerator.DenyAmendmentMail(Notification, fsActivity, FSActivityEmailTemplate,Comment);
        }
        public void GenerateFSActivityVerificationEmail(FinancialStatementActivity fsActivity, ChangeServiceModel changeServiceModel, FSActivityReportBuilder fsActivityVerificationBuilder, LookUpForFS lookUpForFS,List<UserEmailView> UserEmailView, string Comment)
        {
            List<ClientReportView> AssigneeNAssignor=null;
            if (changeServiceModel is AssignServiceModel)
            {
                AssigneeNAssignor = new List<ClientReportView>();
                AssigneeNAssignor.Add(((AssignServiceModel)(changeServiceModel)).AssignedClientReportView);
                AssigneeNAssignor.Add(((AssignServiceModel)(changeServiceModel)).AssignedFromClientReportView );

            }
            //Generate the veridication report         
            FileUpload _fileUpload = GenerateFSActivityVerificationReport(fsActivity, fsActivityVerificationBuilder, lookUpForFS,AssigneeNAssignor,  _tracker);
            EmailTemplate FSActivityEmailTemplate = changeServiceModel.FSActivityMailTemplate ;

            Email Notification = new Email();
            Notification._emailUserAssignmentRepository = _emailUserAssignmentRepository;
            Notification.AddEmailAddresses(UserEmailView,_tracker );

            //_emailRepository.Add(Notification);
            //_tracker.Created.Add(Notification);
            //EmailTemplateGenerator.CreateFinancingStatementMail(Notification, submittedFS, FSActivityEmailTemplate);


            _emailRepository.Add(Notification);
            _tracker.Created.Add(Notification);
            //EmailTemplateGenerator.CreateFinancingStatementMail(Notification, submittedFS, FSActivityEmailTemplate);
            EmailTemplateGenerator.CreateAmendmentMail(Notification, fsActivity, FSActivityEmailTemplate, _fileUpload,Comment);

        }
        /// <summary>
        /// Validation for an update
        /// </summary>
        /// <returns></returns>
        protected bool ValidateUpdateSecurityRole()
        {
            if (!(_executingUser.IsInAnyRoles("Client Officer", "Update Officer", "Amend Officer")))
            {
                return false;
            }



            return true;
        }
    }

  
}
