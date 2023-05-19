using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Domain;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.FS.IRepository;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.WorkflowEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViewMappers;
using CRL.Model.Messaging;
using CRL.Model.Notification.IRepository;

namespace CRL.Model.ModelService.FS
{
    public class SubordinateServiceModel : ChangeServiceModel
    {
        ActivitySubordination  fsActivity = null;
        IFinancialStatementActivityRepository _fsActivityRepository { get; set; }

        public SubordinateServiceModel(ISerialTrackerRepository serialTrackerRepository,
            IEmailRepository emailRepository, IEmailTemplateRepository emailTemplateRepository, IEmailUserAssignmentRepository emailUserAssignmentRepository, ILKFinancialStatementTransactionCategoryRepository financialStatementTransactionCategoryRepository,
            ILKCollateralCategoryRepository collateralCategoryRepository, IFinancialStatementRepository financialStatementRepository, IFinancialStatementActivityRepository fsActivityRepository, AuditingTracker tracker, SecurityUser user, string ServiceRequest)
        {
            _emailRepository = emailRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _emailUserAssignmentRepository = emailUserAssignmentRepository;
            _financialStatementTransactionCategoryRepository = financialStatementTransactionCategoryRepository;
            _collateralCategoryRepository = collateralCategoryRepository;
            _financialStatementRepository = financialStatementRepository;
            _fsActivityRepository = fsActivityRepository;
            _serialTrackerRepository = serialTrackerRepository;
            _tracker = tracker;
            _executingUser = user;
            _serviceRequest = ServiceRequest;
          
            
            
        }

        public override void LoadInitialDataFromRepository(FSActivityRequest fs, WFCaseActivity _case = null)
        {
            SubordinateFSRequest request = (SubordinateFSRequest)fs;

            if (fs.RequestMode != CRL.Infrastructure.Messaging.RequestMode.Submit &&
                fs.RequestMode != CRL.Infrastructure.Messaging.RequestMode.Create )
            {
                fsActivity = (ActivitySubordination)_case.FinancialStatementActivity;
                submittedFS = fsActivity.FinancialStatement ;             
            }
            else
            {
                submittedFS = _financialStatementRepository.SelectFSById(request.FinancialStatementId);
            }
            TransactionTypeName = submittedFS.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName;
            CollateralTypeName = submittedFS.CollateralType.CollateralCategoryName;
            UniqeCodePrefix = "SUB";     
              
        }

     public override FinancialStatementActivity Change(Messaging.FSActivityRequest request, bool SetUniqueNoLater, WFCaseActivity _case = null)
        {
            if (request.RequestMode == CRL.Infrastructure.Messaging.RequestMode.Create)
                return (this.Subordinate((SubordinateFSRequest)request, SetUniqueNoLater, _case = null));
            else if (request.RequestMode == CRL.Infrastructure.Messaging.RequestMode.Submit)
                return (this.SubmitSubordination((SubordinateFSRequest)request, SetUniqueNoLater, _case));
            else if (request.RequestMode == CRL.Infrastructure.Messaging.RequestMode.Approval)
                return (this.AuthorizeSubordination((SubordinateFSRequest)request, SetUniqueNoLater, _case ));
            else
                throw new Exception("Not implemented yet");
        }

        public FinancialStatementActivity AuthorizeSubordination(SubordinateFSRequest  request, bool SetUniqueNoLater, WFCaseActivity _case = null)
        {

            fsActivity.isApprovedOrDenied =1;
            fsActivity.ApprovedById = this._executingUser.Id;
            fsActivity.ApprovedDate = _tracker.Date;
            fsActivity.IsActive = true;
            //submittedFS.FinancialStatementLastActivity = "Subordinated";

            return fsActivity;
        }
        public FinancialStatementActivity SubmitSubordination(SubordinateFSRequest  request, bool SetUniqueNoLater, WFCaseActivity _case = null)
        {
               
        
            //Create new activity
            fsActivity  = new ActivitySubordination();
            fsActivity.FinancialStatementActivityTypeId = FinancialStatementActivityCategory.Subordination ;
            fsActivity.PreviousActivity = submittedFS.FinancialStatementLastActivity;
            //submittedFS.FinancialStatementLastActivity = "Pending  Subordination";
            //Set the updated financing statement to the activity
            fsActivity.FinancialStatement = submittedFS;
            fsActivity.FinancialStatement.isPendingAmendment = true;
            fsActivity.PreviousFinancialStatement = submittedFS;
            fsActivity.SubordinatingParticipant = request.SubordinatingPartyView.ConvertToSubordinatingParty();
            _tracker.Created.Add(fsActivity.SubordinatingParticipant);
              //Add activity to the tracker
            _tracker.Created.Add(fsActivity);

            if (SetUniqueNoLater == false)
            {
                this.AssignActivityNo(UniqeCodePrefix, fsActivity, _case);
            }


            fsActivity.IsActive = true;
            fsActivity.MembershipId =  submittedFS.MembershipId;
            fsActivity.InstitutionUnitId = this._executingUser.InstitutionUnitId;

            _tracker.Created.Add(fsActivity);
            _fsActivityRepository.Add(fsActivity);

            _case.CaseTitle = "Authorize Financing Statement Subordination - "  + submittedFS .RegistrationNo ;
            
            //We also need to load all activities and set the current to the current financing statement
            return fsActivity;

        }
        public FinancialStatementActivity Subordinate(SubordinateFSRequest  request, bool SetUniqueNoLater, WFCaseActivity _case = null)
        {
    
            //Create new activity
            fsActivity = new ActivitySubordination();
            fsActivity.FinancialStatementActivityTypeId = FinancialStatementActivityCategory.Subordination;
            //submittedFS.FinancialStatementLastActivity = "Subordination";
      
            //Set the updated financing statement to the activity
            fsActivity.FinancialStatement = submittedFS;
            fsActivity.PreviousFinancialStatement = submittedFS;
            fsActivity.SubordinatingParticipant = request.SubordinatingPartyView.ConvertToSubordinatingParty();
            //**We need code to update to add the person we are subordinating to
           
           
            //Add activity to the tracker
            _tracker.Created.Add(fsActivity);

            if (SetUniqueNoLater == false)
            {
                this.AssignActivityNo(UniqeCodePrefix, fsActivity);
            }
            
            fsActivity.isApprovedOrDenied =1;
            fsActivity.IsActive = true;
            fsActivity.MembershipId = this._executingUser.MembershipId;
            fsActivity.InstitutionUnitId  = this._executingUser.InstitutionUnitId ;
            fsActivity.ApprovedById = this._executingUser.Id;
            fsActivity.ApprovedDate = _tracker.Date;
            fsActivity.SubordinatingParticipant.IsActive = true;

            _tracker.Created.Add(fsActivity);
            _tracker.Created.Add(fsActivity.SubordinatingParticipant);
            _fsActivityRepository.Add(fsActivity);


            //We also need to load all activities and set the current to the current financing statement
            return fsActivity;
        }





        public override FinancialStatementActivity Deny()
        {
            submittedFS .isPendingAmendment = false;
          
            return fsActivity;
        }
    }
}
