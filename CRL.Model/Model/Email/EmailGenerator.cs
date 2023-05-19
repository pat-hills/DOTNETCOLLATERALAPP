using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.FS;
using CRL.Model.ModelViews;
using CRL.Model.WorkflowEngine;
using CRL.Model.Memberships;
using CRL.Model.Payments;
using CRL.Model.Model.FS;

namespace CRL.Model.Notification
{
    //Main job is to apply template to the email object
    public static class EmailTemplateGenerator
    {
        public static void CreateFinancingStatementMail(Email _email, FS.FinancialStatement fs, EmailTemplate _template, string Comment)
        {
            string BodyHTML = _template.EmailBodyHTML;
            BodyHTML = BodyHTML.Replace("#RegistrationNo", fs.RegistrationNo);
            string Message = "";
            if (String.IsNullOrWhiteSpace(Comment) == false)
            {
                Message = "<table><tr><td>Comment By User: </td><td>" + Comment + "</td></tr></table>";
            }
            else
            {
                Message = "<table><tr><td>Comment By User: </td><td> No comment was provided by user.</td></tr></table>";
            }
            BodyHTML = BodyHTML.Replace("#COMMENT", Message);
            _email.EmailSubject = _template.EmailSubject;
            if (fs.VerificationAttachment != null)
            {
                _email.EmailAttachments.Add(fs.VerificationAttachment);
                fs.VerificationAttachment.Email = _email;
            }
            _email.EmailBody = BodyHTML;




        }
        public static void CreateAmendmentMail(Email _email, FinancialStatementActivity fsActivity, EmailTemplate _template, FileUpload _attachment,string Comment)
        {
            string Amendment = fsActivity is ActivityUpdate ? "Update":
                fsActivity is ActivityDischarge && ((ActivityDischarge)fsActivity).DischargeType ==1 ? "Cancellation" :
                fsActivity is ActivityDischarge && ((ActivityDischarge)fsActivity).DischargeType ==2 ? "Partial Discharge" :
                fsActivity is ActivityDischargeDueToError ? "Cancellation Due To Error" :
                fsActivity is ActivitySubordination ? "Subordination" :
                 fsActivity is ActivityAssignment  && ((ActivityAssignment)fsActivity).AssignmentType  ==1 ? "Transfer" :
                 fsActivity is ActivityAssignment && ((ActivityAssignment)fsActivity).AssignmentType ==2 ? "Partial Assignment" : "N/A";
           
            string BodyHTML = _template.EmailBodyHTML;
            string Message = "";
            if (String.IsNullOrWhiteSpace(Comment) == false)
            {
                Message = "<table><tr><td>Comment By User: </td><td>" + Comment + "</td></tr></table>";
            }
            else
            {
                Message = "<table><tr><td>Comment By User: </td><td> No comment was provided by user.</td></tr></table>";
            }
            BodyHTML = BodyHTML.Replace("#COMMENT", Message);
            BodyHTML = BodyHTML.Replace("#RegistrationNo", fsActivity.FinancialStatement .RegistrationNo );
            BodyHTML = BodyHTML.Replace("#RequestNo", fsActivity.RequestNo);
            BodyHTML = BodyHTML.Replace("#AMENDMENT", Amendment.ToLower ());
            _email.EmailSubject = _template.EmailSubject.Replace("#AMENDMENT", Amendment);  
     
            _email.EmailAttachments.Add(_attachment);
            _email.EmailBody = BodyHTML;
            _attachment.Email = _email;



        }

        public static void ApproveFinancingStatementMail(Email _email,  FinancialStatement fs, EmailTemplate _template, FileUpload _attachment, string Comment)
        {
            _email.EmailSubject = _template.EmailSubject;
            string BodyHTML = _template.EmailBodyHTML;
            //BodyHTML = ReplaceWorkflowTags(_workflowTemplate, BodyHTML);
            BodyHTML = BodyHTML.Replace("#RegistrationNo", fs.RegistrationNo);
            //BodyHTML = BodyHTML.Replace("#ClientAuthorizer", fs..Amount.ToString());
            string Message = "";
            if (String.IsNullOrWhiteSpace(Comment) == false)
            {
                Message = "<table><tr><td>Comment By User: </td><td>" + Comment + "</td></tr></table>";
            }
            else
            {
                Message = "<table><tr><td>Comment By User: </td><td> No comment was provided by user.</td></tr></table>";
            }
            BodyHTML = BodyHTML.Replace("#COMMENT", Message);
            _email.EmailAttachments.Add(_attachment);
            _attachment.Email = _email;
            _email.EmailBody = BodyHTML;
        }
        public static void ApproveAmendmentMail(Email _email,   FinancialStatementActivity fsActivity, EmailTemplate _template, FileUpload _attachment, string Comment)
        {

            string Amendment = fsActivity is ActivityUpdate ? "Update" :
              fsActivity is ActivityDischarge && ((ActivityDischarge)fsActivity).DischargeType <2 ? "Cancellation" :
              fsActivity is ActivityDischarge && ((ActivityDischarge)fsActivity).DischargeType == 2 ? "Partial Discharge" :
              fsActivity is ActivityDischargeDueToError ? "Cancellation Due To Error" :
              fsActivity is ActivitySubordination ? "Subordination " :
               fsActivity is ActivityAssignment && ((ActivityAssignment)fsActivity).AssignmentType == 1 ? "Transfer" :
               fsActivity is ActivityAssignment && ((ActivityAssignment)fsActivity).AssignmentType == 2 ? "Partial Assignment" : "N/A";

            string BodyHTML = _template.EmailBodyHTML;
            string Message = "";
            if (String.IsNullOrWhiteSpace(Comment) == false)
            {
                Message = "<table><tr><td>Comment By User: </td><td>" + Comment + "</td></tr></table>";
            }
            else
            {
                Message = "<table><tr><td>Comment By User: </td><td> No comment was provided by user.</td></tr></table>";
            }
            BodyHTML = BodyHTML.Replace("#COMMENT", Message);
            BodyHTML = BodyHTML.Replace("#RegistrationNo", fsActivity.FinancialStatement.RegistrationNo);
            BodyHTML = BodyHTML.Replace("#RequestNo", fsActivity.RequestNo );
            BodyHTML = BodyHTML.Replace("#AMENDMENT", Amendment.ToLower());
            _email.EmailSubject = _template.EmailSubject.Replace("#AMENDMENT", Amendment);           
            _email.EmailAttachments.Add(_attachment);
            _attachment.Email = _email;
            _email.EmailBody = BodyHTML;
        }
        public static void DenyAmendmentMail(Email _email,  FinancialStatementActivity fsActivity, EmailTemplate _template, string Comment)
        {
            string Amendment = fsActivity is ActivityUpdate ? "Update" :
           fsActivity is ActivityDischarge && ((ActivityDischarge)fsActivity).DischargeType <2 ? "Cancellation" :
           fsActivity is ActivityDischarge && ((ActivityDischarge)fsActivity).DischargeType == 2 ? "Partial Discharge" :
           fsActivity is ActivityDischargeDueToError ? "Cancellation Due To Error" :
           fsActivity is ActivitySubordination ? "Subordination" :
            fsActivity is ActivityAssignment && ((ActivityAssignment)fsActivity).AssignmentType <2 ? "Transfer" :
            fsActivity is ActivityAssignment && ((ActivityAssignment)fsActivity).AssignmentType == 2 ? "Partial Assignment" : "N/A";


            string BodyHTML = _template.EmailBodyHTML;
            string Message = "";
            if (String.IsNullOrWhiteSpace(Comment) == false)
            {
                Message = "<table><tr><td>Comment By User: </td><td>" + Comment + "</td></tr></table>";
            }
            else
            {
                Message = "<table><tr><td>Comment By User: </td><td> No comment was provided by user.</td></tr></table>";
            }
            BodyHTML = BodyHTML.Replace("#COMMENT", Message);
            BodyHTML = BodyHTML.Replace("#RegistrationNo", fsActivity.FinancialStatement.RegistrationNo);
            BodyHTML = BodyHTML.Replace("#RequestNo", fsActivity.RequestNo);
            BodyHTML = BodyHTML.Replace("#AMENDMENT", Amendment.ToLower());
            _email.EmailSubject = _template.EmailSubject.Replace("#AMENDMENT", Amendment);  
            _email.EmailBody = BodyHTML;


        

            _email.EmailBody = BodyHTML;
        }

        public static void DenyFinancingStatementMail(Email _email, FinancialStatement fs, EmailTemplate _template, string Comment)
        {
            _email.EmailSubject = _template.EmailSubject;
            string BodyHTML = _template.EmailBodyHTML;
            //BodyHTML = ReplaceWorkflowTags(_workflowTemplate, BodyHTML);
            //BodyHTML = BodyHTML.Replace("#RequestNo", fs.RequestNo );
            //BodyHTML = BodyHTML.Replace("#ClientAuthorizer", fs..Amount.ToString());
            string Message = "";
            if (String.IsNullOrWhiteSpace(Comment) == false)
            {
                Message = "<table><tr><td>Comment By User: </td><td>" + Comment + "</td></tr></table>";
            }
            else
            {
                Message = "<table><tr><td>Comment By User: </td><td> No comment was provided by user.</td></tr></table>";
            }
            BodyHTML = BodyHTML.Replace("#COMMENT", Message);
            _email.EmailBody = BodyHTML;
        }

        public static void LegalEntityMembershipRegistrationNotification(Email _email, Institution institution,string SecuredPartyType,string Country,string County,string State,EmailTemplate _template)
        {
            _email.EmailSubject = _template.EmailSubject;
            string BodyHTML = _template.EmailBodyHTML;
            BodyHTML = BodyHTML.Replace("#DATE", institution.CreatedOn.ToLongDateString());
            BodyHTML = BodyHTML.Replace("#CLIENTNAME", institution .Name );
            BodyHTML = BodyHTML.Replace("#COMPANYNO", institution.CompanyNo );
            BodyHTML = BodyHTML.Replace("#EMAIL", institution.Address .Email );
            //BodyHTML = BodyHTML.Replace("#ROLE", Role);
            BodyHTML = BodyHTML.Replace("#SECUREDPARTYTYPE", SecuredPartyType);
            BodyHTML = BodyHTML.Replace("#ADDRESS", institution.Address.Address  );
            BodyHTML = BodyHTML.Replace("#CITY", institution.Address.City );
            BodyHTML = BodyHTML.Replace("#PHONE", institution.Address .Phone );
            //BodyHTML = BodyHTML.Replace("#NATIONALITY", Nationality);
            BodyHTML = BodyHTML.Replace("#COUNTRY", Country);
            BodyHTML = BodyHTML.Replace("#COUNTY", County);
            BodyHTML = BodyHTML.Replace("#STATE", State);
            

            _email.EmailBody = BodyHTML;
        }
        public static void ConfirmClientRegistration(Email _email,  string UrlAction, string ClientSubjectName, string NameOfClient, string Username , EmailTemplate _template)
        {
            _email.EmailSubject = _template.EmailSubject;
            string BodyHTML = _template.EmailBodyHTML;
            
            BodyHTML = BodyHTML.Replace("#URLLINK", UrlAction);
            BodyHTML = BodyHTML.Replace("#CLIENTNAME", NameOfClient);
            BodyHTML = BodyHTML.Replace("#USERNAME", Username);
            _email.EmailSubject = _email.EmailSubject.Replace("#CLIENTNAME", ClientSubjectName);
            //BodyHTML = BodyHTML.Replace("#ClientAuthorizer", fs..Amount.ToString());
            _email.EmailBody = BodyHTML;
        }

        public static string  ReplaceWorkflowTags(WorkflowTemplate _workflowTemplate, string BodyHTML)
        {

            BodyHTML = BodyHTML.Replace("#ASSIGNEDDATE", _workflowTemplate.LastTaskDate);
            BodyHTML = BodyHTML.Replace("#EVENTNO", _workflowTemplate.EventNo );
            BodyHTML = BodyHTML.Replace("#EVENTTITLE", _workflowTemplate.EventTitle );            
            

           

            BodyHTML = BodyHTML.Replace("#EVENTSTATUS", _workflowTemplate .CurrentPendingStatus  );
            BodyHTML = BodyHTML.Replace("#CURRENTSTATUS", _workflowTemplate.CurrentPendingStatus);
            BodyHTML = BodyHTML.Replace("#TASKDESCRIPTION", _workflowTemplate.CurrentPendingStatus ?? "N/A");
            BodyHTML = BodyHTML.Replace("#ASSIGNEDTO", _workflowTemplate .CurrentTaskAssignedTo );




            BodyHTML = BodyHTML.Replace("#EVENTTASKDATE", _workflowTemplate.LastTaskDate);
            BodyHTML = BodyHTML.Replace("#EVENTPERFORMED", _workflowTemplate.LastTaskPerformed);
            BodyHTML = BodyHTML.Replace("#SUBMITTER", _workflowTemplate.LastTaskPerformedBy);
            BodyHTML = BodyHTML.Replace("#COMMENT", String.IsNullOrWhiteSpace (_workflowTemplate.LastTaskComment) ? "No comment by user" : _workflowTemplate.LastTaskComment );


         

            return BodyHTML ;
        }

        public static void AssignNewTaskMail(Email _email, WorkflowTemplate _workflowTemplate, EmailTemplate _template)
        {




            _email.EmailSubject = _workflowTemplate.EventTitle ;
            string BodyHTML = _template.EmailBodyHTML;
            BodyHTML = ReplaceWorkflowTags(_workflowTemplate, BodyHTML);
            _email.EmailBody = BodyHTML;
           



        }

        public static void ReversalPaymentNotification(Email _email, Payment payment, string ReceviedBy, string Paypoint, EmailTemplate _template)
        {
            string BodyHTML = _template.EmailBodyHTML;
            BodyHTML = BodyHTML.Replace("#REVPAYMENTNO", payment.PaymentNo);
            BodyHTML = BodyHTML.Replace("#PAYMENTDATE", payment.ReversedPayment .PaymentDate.ToString());
            BodyHTML = BodyHTML.Replace("#AMOUNT", "₦ " + payment.ReversedPayment.Amount.ToString("#,##0.00"));
            BodyHTML = BodyHTML.Replace("#PAYMENTNO", payment.ReversedPayment.PaymentNo);
            BodyHTML = BodyHTML.Replace("#PAYEE", payment.ReversedPayment  .Payee);
            BodyHTML = BodyHTML.Replace("#RECEIVEDBY", ReceviedBy);
            BodyHTML = BodyHTML.Replace("#PAYPOINT", Paypoint);
            _email.EmailSubject = _template.EmailSubject;            
            _email.EmailBody = BodyHTML;
            



        }


        public static void PaymentNotification(Email _email, Payment   payment, string ReceviedBy , string Paypoint, EmailTemplate _template, bool isPublicPayment,string UrlLink)
        {
            string BodyHTML = _template.EmailBodyHTML;
            BodyHTML = BodyHTML.Replace("#PAYMENTDATE", payment.PaymentDate.ToString ());
            BodyHTML = BodyHTML.Replace("#AMOUNT", "₦ " + payment.Amount.ToString("#,##0.00"));
            BodyHTML = BodyHTML.Replace("#PAYMENTNO", payment.PaymentNo );
            BodyHTML = BodyHTML.Replace("#PAYEE", payment.Payee );
            BodyHTML = BodyHTML.Replace("#RECEIVEDBY", ReceviedBy);
            BodyHTML = BodyHTML.Replace("#PAYPOINT", Paypoint);
            
          
            if (isPublicPayment)
            {
                
                BodyHTML = BodyHTML.Replace("#SEARCHACCESSURL", UrlLink);
                BodyHTML = BodyHTML.Replace("#SECURITYCODE", "Your security code for making payment is " + payment.PublicUserSecurityCode.SecurityCode);
              
            }
          

            _email.EmailSubject = _template.EmailSubject;
            //_email.EmailAttachments.Add(_attachment);
            _email.EmailBody = BodyHTML;
            //_attachment.Email = _email;



        }

        public static void NotifyUserOfNewAccountCreated(Email _email, EmailTemplate _template, string LoginId,string pName, string pEmail, string Password,  string UrlLink,bool createdByUserAdmin = true)
        {
            string BodyHTML = _template.EmailBodyHTML;
            BodyHTML = BodyHTML.Replace("#LOGINID", LoginId);
            BodyHTML = BodyHTML.Replace("#NAME", pName);
            BodyHTML = BodyHTML.Replace("#EMAIL", pEmail);
            if (string.IsNullOrWhiteSpace(Password))
            {
                BodyHTML = BodyHTML.Replace("#PASSWORDLABEL", "Your password will be communicated to you shortly");
                BodyHTML = BodyHTML.Replace("#PASSWORD", "");

            }
            else
            {
                BodyHTML = BodyHTML.Replace("#PASSWORDLABEL", "Password : ");
                BodyHTML = BodyHTML.Replace("#PASSWORD", Password);

            }
            BodyHTML = BodyHTML.Replace("#LINK", UrlLink);
            _email.EmailSubject = _template.EmailSubject;
            _email.EmailBody = BodyHTML;




        }

        public static void SearchResponse(Email _email, EmailTemplate _template, FileUpload _attachment)
        {
            string BodyHTML = _template.EmailBodyHTML;
            BodyHTML = BodyHTML.Replace("#RequestDate", DateTime.Now.ToString () );
            _email.EmailSubject = _template.EmailSubject;
            _email.EmailAttachments.Add(_attachment);
            _email.EmailBody = BodyHTML;
            _attachment.Email = _email;



        }

    


        public static void ResetPasswordEmail(Email _email, User user, string UrlLink, string ResetRequestExpiryDays, EmailTemplate _template)
        {
            string BodyHTML = _template.EmailBodyHTML;
              BodyHTML = BodyHTML.Replace("#RESETPASSWORDACCESSURL", UrlLink);
              BodyHTML = BodyHTML.Replace("#NUMBEROFDAYS", ResetRequestExpiryDays);

            _email.EmailSubject = _template.EmailSubject;         
            _email.EmailBody = BodyHTML;         

        }

        public static void ApproveOrDenyClientAccountSetup(bool Approved, Email _email, EmailTemplate _template, string ClientName, string UrlAction, string Username,string Comment)
        {
            string BodyHTML = _template.EmailBodyHTML;
            _email.EmailSubject = _template.EmailSubject; 
            BodyHTML = BodyHTML.Replace("#URLLINK", UrlAction);
            BodyHTML = BodyHTML.Replace("#CLIENTNAME", ClientName);
            BodyHTML = BodyHTML.Replace("#USERNAME", Username);
            string Message = "";
            if (String.IsNullOrWhiteSpace (Comment)==false)
            {
                Message = "<table><tr><td>Comment By User: </td><td>" + Comment + "</td></tr></table>";
            }
            else
            {
                Message = "<table><tr><td>Comment By User: </td><td> No comment was provided by user.</td></tr></table>";
            }
            BodyHTML = BodyHTML.Replace("#COMMENT", Message);
            _email.EmailBody = BodyHTML;


        }

        public static void ApproveOrDenyPostpaid(bool Approved, Email _email,EmailTemplate _template, string Comment)
        {
            string BodyHTML = _template.EmailBodyHTML;
            string Message = "";
            if (String.IsNullOrWhiteSpace(Comment) == false)
            {
                Message = "<table><tr><td>Comment By User: </td><td>" + Comment + "</td></tr></table>";
            }
            else
            {
                Message = "<table><tr><td>Comment By User: </td><td> No comment was provided by user.</td></tr></table>";
            }
            BodyHTML = BodyHTML.Replace("#COMMENT",  Message);
            _email.EmailSubject = _template.EmailSubject;
            _email.EmailBody = BodyHTML;


        }


        public static void General( Email _email,  EmailTemplate _template)
        {
            string BodyHTML = _template.EmailBodyHTML;
            _email.EmailSubject = _template.EmailSubject;
            _email.EmailBody = BodyHTML;


        }

        public static void ClientAccountSetupRequest(Email _email, EmailTemplate _template, string ClientCode)
        {
            string BodyHTML = _template.EmailBodyHTML;
            _email.EmailSubject = _template.EmailSubject;
            BodyHTML = BodyHTML.Replace("#CLIENTCODE", ClientCode);
            _email.EmailBody = BodyHTML;
        }

        public static void ApproveAmmendmentTransferee(Email _email, EmailTemplate _template, string TaskDate, string CurrentStatus, string Task, string TaskAssignedTo, string TaskSubmittedBy, string Comment)
        {
            string BodyHTML = _template.EmailBodyHTML;
            _email.EmailSubject = _template.EmailSubject;
            BodyHTML = BodyHTML.Replace("#TASKDATE", TaskDate);
            BodyHTML = BodyHTML.Replace("#CURRENTSTATUS", CurrentStatus);
            BodyHTML = BodyHTML.Replace("#TASK", Task);
            BodyHTML = BodyHTML.Replace("#TASKASSIGNEDTO", TaskAssignedTo);
            BodyHTML = BodyHTML.Replace("#TASKSUBMITTEDBY", TaskSubmittedBy);
            BodyHTML = BodyHTML.Replace("#COMMENT", String.IsNullOrWhiteSpace (Comment) ? "No comment by user" : Comment);
            _email.EmailBody = BodyHTML;
        }

        public static void InterSwitchPaymentNotification(Email _email, Payment  payment, EmailTemplate _template)
        {
            string BodyHTML = _template.EmailBodyHTML;
            _email.EmailSubject = _template.EmailSubject;
            BodyHTML = BodyHTML.Replace("#PAYMENTDATE", payment.PaymentDate.ToString ());
            BodyHTML = BodyHTML.Replace("#AMOUNT", "₦ " + payment.Amount.ToString("#,##0.00"));
            BodyHTML = BodyHTML.Replace("#PAYMENTNO", payment.PaymentNo );
            BodyHTML = BodyHTML.Replace("#PAIDBY", payment.Payee);
            BodyHTML = BodyHTML.Replace("#SECURITYCODE", payment.PublicUserSecurityCode.SecurityCode);            
            _email.EmailBody = BodyHTML;
        }

        public static void BatchReconciliationNotification(Email _email, AccountBatch accountBatch, EmailTemplate _template) 
        {
            string BodyHTML = _template.EmailBodyHTML;
            _email.EmailSubject = _template.EmailSubject;
            BodyHTML = BodyHTML.Replace("#BATCHDATE", accountBatch.CreatedOn.ToString());
            BodyHTML = BodyHTML.Replace("#BATCHNO", accountBatch.Id.ToString());
            BodyHTML = BodyHTML.Replace("#STARTDATE", accountBatch.PeriodStartDate.ToString());
            BodyHTML = BodyHTML.Replace("#ENDDATE", accountBatch.PeriodEndDate.ToString());
            _email.EmailBody = BodyHTML;
        }

    }
}
