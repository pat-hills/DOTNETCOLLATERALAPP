using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Domain;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.FS.IRepository;
using CRL.Model.Messaging;
using CRL.Model.Notification.IRepository;
using CRL.Model.WorkflowEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelService.FS
{

    public class DischargeServiceModel : ChangeServiceModel
    {
        ActivityDischarge dischargeActivity = null;
        IFinancialStatementActivityRepository _fsActivityRepository { get; set; }
        FinancialStatementActivityCategory dischargeType { get; set; }

        public DischargeServiceModel(ISerialTrackerRepository serialTrackerRepository,
            IEmailRepository emailRepository, IEmailTemplateRepository emailTemplateRepository, IEmailUserAssignmentRepository emailUserAssignmentRepository, ILKFinancialStatementTransactionCategoryRepository financialStatementTransactionCategoryRepository,
            ILKCollateralCategoryRepository collateralCategoryRepository, IFinancialStatementRepository financialStatementRepository,IFinancialStatementActivityRepository fsActivityRepository,AuditingTracker tracker, SecurityUser user, string ServiceRequest)
        {
            _emailRepository = emailRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _emailUserAssignmentRepository = emailUserAssignmentRepository;
            _financialStatementTransactionCategoryRepository = financialStatementTransactionCategoryRepository;
            _collateralCategoryRepository = collateralCategoryRepository;
            _financialStatementRepository = financialStatementRepository ;
            _fsActivityRepository = fsActivityRepository;
            _serialTrackerRepository = serialTrackerRepository;
            _tracker = tracker;
            _executingUser = user;
            _serviceRequest = ServiceRequest;
            
            
        }

        public override void LoadInitialDataFromRepository(FSActivityRequest fs, WFCaseActivity _case = null)
        {
            DischargeFSRequest request = (DischargeFSRequest)fs;
            if (fs.RequestMode != CRL.Infrastructure.Messaging.RequestMode.Submit &&
              fs.RequestMode != CRL.Infrastructure.Messaging.RequestMode.Create)
            {
                dischargeActivity = (ActivityDischarge)_case.FinancialStatementActivity;
                submittedFS = dischargeActivity.FinancialStatement;
            }
            else
            {
                submittedFS = _financialStatementRepository.SelectFSById(request.FinancialStatementId);
            }
            
            TransactionTypeName = submittedFS.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName;
            CollateralTypeName = submittedFS.CollateralType.CollateralCategoryName;
            UniqeCodePrefix  = request.DischargeType == 1 ? "FDI" : "PDI";
        }

        public FinancialStatementActivity DenyDischarge(DischargeFSRequest  request, bool SetUniqueNoLater, WFCaseActivity _case = null)
        {
            FinancialStatementActivityCategory dischargeType = dischargeActivity.FinancialStatementActivityTypeId;
            if (dischargeType == FinancialStatementActivityCategory.PartialDischarge)
            {
                foreach (var col in submittedFS.Collaterals.Where(c => c.DischargeActivityId == dischargeActivity.Id))
                {

                    col.IsDischarged = false;
                    col.DischargeActivityId = null;
                    col.DischargeActivity = null;

                }
            }
            else
            {

                submittedFS.IsDischarged = false;
                submittedFS.DischargeActivityId = null;
                submittedFS.DischargeActivity = null;
            }

            return dischargeActivity;
        }

        public FinancialStatementActivity AuthorizeDischarge(DischargeFSRequest  request, bool SetUniqueNoLater, WFCaseActivity _case = null)
        {
            FinancialStatementActivityCategory dischargeType = dischargeActivity.FinancialStatementActivityTypeId;
            request.FinancialStatementActivityType = dischargeType;
            UniqeCodePrefix = dischargeType== FinancialStatementActivityCategory.FullDicharge  ? "FDI" : "PDI";
            
            
                //Set financing statement or collaterals to discharge
            if (dischargeType== FinancialStatementActivityCategory.PartialDischarge)
            {
                foreach (var col in submittedFS .Collaterals .Where (c=>c.DischargeActivityId == dischargeActivity .Id ))
                {
                   
                    col.IsDischarged =true;
                    
                }
            }
            else
            {
               
                submittedFS.IsDischarged = true ;            }

            if (SetUniqueNoLater == false)
            {
                this.AssignActivityNo(UniqeCodePrefix, dischargeActivity);
            }

            dischargeActivity.isApprovedOrDenied =1;
            dischargeActivity.ApprovedById = this._executingUser.Id;
            dischargeActivity.ApprovedDate = _tracker.Date;
            
            return dischargeActivity;
        }
        public FinancialStatementActivity SubmitDischarge(DischargeFSRequest  request, bool SetUniqueNoLater, WFCaseActivity _case = null)
        {
            UniqeCodePrefix = request.DischargeType  == 1 ? "FDI" : "PDI";
            FinancialStatementActivityCategory dischargeType =  request.DischargeType   == 1 ? FinancialStatementActivityCategory.FullDicharge : FinancialStatementActivityCategory.PartialDischarge;
        
            //Create new activity
            dischargeActivity = new ActivityDischarge();
            dischargeActivity.FinancialStatementActivityTypeId = dischargeType;
            dischargeActivity.PreviousActivity = submittedFS.FinancialStatementLastActivity;
            //Set the updated financing statement to the activity
            dischargeActivity.FinancialStatement = submittedFS;
            dischargeActivity.FinancialStatement.isPendingAmendment = true;

              //Set financing statement or collaterals to discharge
            if ( request.DischargeType   == 2)
            {
                foreach (var col in submittedFS .Collaterals .Where (c=>request.PartiallyDischargedCollateralIds.Contains (c.Id )))
                {
                   
                    col.DischargeActivity  = dischargeActivity ;
                }
            }
            else
            {
               
                submittedFS.DischargeActivity = dischargeActivity;
            }

              //Add activity to the tracker
            _tracker.Created.Add(dischargeActivity);

            if (SetUniqueNoLater == false)
            {
                this.AssignActivityNo(UniqeCodePrefix, dischargeActivity,_case );
            }

          
            dischargeActivity.IsActive = true;
            
            dischargeActivity.MembershipId = submittedFS.MembershipId;
            dischargeActivity.PerformedByRegistrar = _executingUser.IsOwnerUser;
            dischargeActivity.InstitutionUnitId  = submittedFS.InstitutionUnitId ;            
            _case.CaseTitle = "Authorize Financing Statement Cancellation - " + submittedFS.RegistrationNo;
            _tracker.Created.Add(dischargeActivity);
            _fsActivityRepository.Add(dischargeActivity);


            //We also need to load all activities and set the current to the current financing statement
            return dischargeActivity;

        }
        public FinancialStatementActivity Discharge(DischargeFSRequest  request, bool SetUniqueNoLater, WFCaseActivity _case = null)
        {

            UniqeCodePrefix = request.DischargeType == 1 ? "FDI" : "PDI";
            dischargeType = request.DischargeType  == 1 ? FinancialStatementActivityCategory.FullDicharge : FinancialStatementActivityCategory.PartialDischarge;
        
            //Create new activity
            dischargeActivity = new ActivityDischarge();
            dischargeActivity.FinancialStatementActivityTypeId = dischargeType;
            dischargeActivity.DischargeType = request.DischargeType;
            dischargeActivity.PerformedByRegistrar = _executingUser.IsOwnerUser;
      
            //Set the updated financing statement to the activity
            dischargeActivity.FinancialStatement = submittedFS;
            dischargeActivity.PreviousFinancialStatement = submittedFS;
           
           
            //Set financing statement or collaterals to discharge
            if (request.DischargeType  == 2)
            {
                foreach (var col in submittedFS .Collaterals .Where (c=>request.PartiallyDischargedCollateralIds.Contains (c.Id )))
                {
                    col.IsDischarged = true;
                    col.DischargeActivity  = dischargeActivity ;
                   
                }
            }
            else
            {
                submittedFS.IsDischarged = true;
                submittedFS.DischargeActivity = dischargeActivity;
               
            }

            //Add activity to the tracker
            _tracker.Created.Add(dischargeActivity);

            if (SetUniqueNoLater == false)
            {
                this.AssignActivityNo(UniqeCodePrefix, dischargeActivity);
            }

            dischargeActivity.isApprovedOrDenied =1;
            dischargeActivity.IsActive = true;         
            dischargeActivity.MembershipId = submittedFS.MembershipId;
            dischargeActivity.InstitutionUnitId  = submittedFS.InstitutionUnitId ;


            dischargeActivity.ApprovedById = this._executingUser.Id;
            dischargeActivity.ApprovedDate = _tracker.Date;
            _tracker.Created.Add(dischargeActivity);
            _fsActivityRepository.Add(dischargeActivity);


            //We also need to load all activities and set the current to the current financing statement
            return dischargeActivity;
        }

        public override FinancialStatementActivity Change(Messaging.FSActivityRequest request, bool SetUniqueNoLater, WFCaseActivity _fcase = null)
        {
            if (request.RequestMode == CRL.Infrastructure.Messaging.RequestMode.Create)
                return (this.Discharge((DischargeFSRequest)request, SetUniqueNoLater, _fcase ));
            else if (request.RequestMode == CRL.Infrastructure.Messaging.RequestMode.Submit)
                return (this.SubmitDischarge((DischargeFSRequest)request, SetUniqueNoLater, _fcase ));
            else if (request.RequestMode == CRL.Infrastructure.Messaging.RequestMode.Approval)
                return (this.AuthorizeDischarge((DischargeFSRequest)request, SetUniqueNoLater, _fcase ));
            else
                throw new Exception("Not implemented yet");
        }




        public override FinancialStatementActivity Deny()
        {
            submittedFS.isPendingAmendment = false;
            return dischargeActivity;
        }
    }
}
