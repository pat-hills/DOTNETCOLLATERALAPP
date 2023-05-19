using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Domain;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.FS.IRepository;
using CRL.Model.Notification.IRepository;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.WorkflowEngine;
using CRL.Model.WorkflowEngine.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelService.FS
{
    public class HandleFSServiceModel: FSServiceModelBase 
    {
        public HandleFSServiceModel(ISerialTrackerRepository serialTrackerRepository, IFinancialStatementRepository financialStatementRepository,
            IEmailRepository emailRepository,IEmailUserAssignmentRepository emailUserAssignmentRepository,IWFCaseRepository caseRepository,ILKFinancialStatementTransactionCategoryRepository financialStatementTransactionCategoryRepository,
            ILKCollateralCategoryRepository collateralCategoryRepository,ILKSectorOfOperationCategoryRepository sectorOfOperationCategoryRepository,
            AuditingTracker tracker, SecurityUser user)
        {
            _serialTrackerRepository=serialTrackerRepository;
            _financialStatementRepository=financialStatementRepository;
            _financialStatementTransactionCategoryRepository = financialStatementTransactionCategoryRepository;
            _collateralCategoryRepository = collateralCategoryRepository;
            _caseRepository = caseRepository;
            _tracker = tracker;
            _executingUser = user;
            _emailRepository = emailRepository;
            _emailUserAssignmentRepository = emailUserAssignmentRepository;
            _sectorOfOperationCategoryRepository = sectorOfOperationCategoryRepository;
            

        }
        public bool ValidateSecurity()
        {
            return (this.ValidateApproveDenySecurityRole());
        }

        public void LoadInitialDataFromRepository(int FinancialStatementTransactionTypeId, int CollateralTypeId,  WFCaseFS _case)
        {

            TransactionTypeName = _financialStatementTransactionCategoryRepository.FindBy((FinancialStatementTransactionCategory)FinancialStatementTransactionTypeId).FinancialStatementTransactionCategoryName;
            CollateralTypeName = _collateralCategoryRepository.FindBy((CollateralCategory)CollateralTypeId).CollateralCategoryName; ;
            SectorOfOperations = _sectorOfOperationCategoryRepository.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).ToList(); //**Cacheable and also stored procedure                                 
            submittedFS = _case.FinancialStatement;
            





        }
        public FinancialStatement AuthorizeFinancingStatement(bool SetUniqueNoLater = false)
        {
            submittedFS.Authorize(_executingUser.Id, _tracker);
            submittedFS.RegistrationDate = _tracker.Date;
            submittedFS.EffectiveDate = _tracker.Date;

            //Assign registration no
            if (SetUniqueNoLater == false)
            {
                this.AssignRegistrationNo();
            }

            
            return submittedFS;  
        }
        public FinancialStatement DenyFinancingStatement(bool SetUniqueNoLater = false)
        {
            //Load the related fs

            submittedFS.IsDeleted = false;
            submittedFS.isApprovedOrDenied = 2;
            submittedFS.HandledById = _executingUser.Id ;
            _tracker.Updated.Add(submittedFS);

            foreach (Collateral c in submittedFS.Collaterals)
            {
                _tracker.Updated.Add(c);
                //c.IsDeleted = true;
            }

            foreach (Participant p in submittedFS.Participants)
            {
                _tracker.Updated.Add(p);
                //p.IsDeleted = true;
            }

            return submittedFS;

          
          
        }
     

    }
}
