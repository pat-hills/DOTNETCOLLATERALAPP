using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Domain;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.FS.IRepository;
using CRL.Model.Messaging;
using CRL.Model.Notification.IRepository;
using CRL.Model.WorkflowEngine;
using CRL.Model.WorkflowEngine.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelService.FS
{
    class AuthorizeUpdateServiceModel : ChangeServiceModel 
    {
        ActivityUpdate updateActivity = null;
        FinancialStatement updatedFS = null;
        List<FinancialStatementActivity> oldActivities = null;
        IFinancialStatementActivityRepository _fsActivityRepository { get; set; }
        public AuthorizeUpdateServiceModel(ISerialTrackerRepository serialTrackerRepository, IFinancialStatementRepository financialStatementRepository,
            IEmailRepository emailRepository,IEmailUserAssignmentRepository emailUserAssignmentRepository, ILKFinancialStatementTransactionCategoryRepository financialStatementTransactionCategoryRepository,
            ILKCollateralCategoryRepository collateralCategoryRepository,ILKSectorOfOperationCategoryRepository sectorOfOperationCategoryRepository,
            IFinancialStatementActivityRepository fsActivityRepository,
            AuditingTracker tracker, SecurityUser user)
        {
            _serialTrackerRepository=serialTrackerRepository;
            _financialStatementRepository=financialStatementRepository;
            _financialStatementTransactionCategoryRepository = financialStatementTransactionCategoryRepository;
            _collateralCategoryRepository = collateralCategoryRepository;
            _emailUserAssignmentRepository = emailUserAssignmentRepository;
            _tracker = tracker;
            _executingUser = user;
            _emailRepository = emailRepository;
            _sectorOfOperationCategoryRepository = sectorOfOperationCategoryRepository;
            _fsActivityRepository = fsActivityRepository;

        }
      

        //public void LoadInitialDataFromRepository(int FinancialStatementTransactionTypeId, int CollateralTypeId,  WFCaseActivity _case)
        //{

        //    TransactionTypeName = _financialStatementTransactionCategoryRepository.FindBy((FinancialStatementTransactionCategory)FinancialStatementTransactionTypeId).FinancialStatementTransactionCategoryName;
        //    CollateralTypeName = _collateralCategoryRepository.FindBy((CollateralCategory)CollateralTypeId).CollateralCategoryName; ;
        //    SectorOfOperations = _sectorOfOperationCategoryRepository.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).ToList(); //**Cacheable and also stored procedure                                 
        //    updateActivity  = (ActivityUpdate)_case.FinancialStatementActivity;
        //    submittedFS = updateActivity.FinancialStatement;
        //    previousFS = updateActivity.PreviousFinancialStatement;





        //}



        public override FinancialStatementActivity  Deny()
        {
            submittedFS.isPendingAmendment = false;
            submittedFS.FinancialStatementLastActivity = updateActivity.PreviousActivity;
            return updateActivity;
        }

        public override FinancialStatementActivity Change(FSActivityRequest request, bool SetUniqueNoLater, WFCaseActivity _case = null)
        {
            //This will ensure that some missed values from the previous financing statement and other necessary
            //actions are taken
            updatedFS.Update(submittedFS, _executingUser.Id, _tracker,SectorOfOperations );

            //Set the previous financing statement to the activity
            updateActivity.PreviousFinancialStatement = submittedFS;
            //Set the updated financing statement to the activity
            updateActivity.FinancialStatement = updatedFS ;

            //Set the activity registration code here
            foreach (var olderActivity in oldActivities)
            {
                olderActivity.FinancialStatement = updateActivity.FinancialStatement;


            }

            //Assign registration no
            if (SetUniqueNoLater == false)
            {
                this.AssignActivityNo("UPD", updateActivity);
            }

           
            updateActivity.isApprovedOrDenied =1;
            updateActivity.IsActive = true;
            updateActivity.MembershipId = this._executingUser.MembershipId;
            updateActivity.ApprovedById = this._executingUser.Id;
            updateActivity.ApprovedDate = _tracker.Date;

            return updateActivity;  
        }

        public override void LoadInitialDataFromRepository(Messaging.FSActivityRequest request, WFCaseActivity _case = null)
        {
            UpdateFSRequest request_a = (UpdateFSRequest)request;
            SectorOfOperations = _sectorOfOperationCategoryRepository.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).ToList(); //**Cacheable and also stored procedure                                 
            updateActivity = (ActivityUpdate)_case.FinancialStatementActivity;
            updatedFS  = updateActivity.FinancialStatement;
            submittedFS  = updateActivity.PreviousFinancialStatement;
            TransactionTypeName = submittedFS.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName;
            CollateralTypeName = submittedFS.CollateralType.CollateralCategoryName;
            oldActivities = _fsActivityRepository.GetDbSet().Where(s => s.FinancialStatementId == submittedFS.Id && s.Id != updateActivity.Id && s.isApprovedOrDenied ==1).ToList();
            UniqeCodePrefix = "UPD";
            
        }

        public override int GetCurrentFSId()
        {
            return updatedFS.Id;
        }

        public override FinancialStatement GetCurrentFS()
        {
            return updatedFS;
        }
    }
}
