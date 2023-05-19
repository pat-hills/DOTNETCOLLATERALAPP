using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.Messaging;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.FS.IRepository;
using CRL.Model.Notification.IRepository;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.WorkflowEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViewMappers;
using CRL.Model.Notification;
using CRL.Model.Messaging;


namespace CRL.Model.ModelService.FS
{
    public class CreateFSServiceModel : FSServiceModelBase
    {       
      


        public CreateFSServiceModel(ISerialTrackerRepository serialTrackerRepository, IFinancialStatementRepository financialStatementRepository,
            IEmailRepository emailRepository,IEmailUserAssignmentRepository emailUserAssignmentRepository ,ILKFinancialStatementTransactionCategoryRepository financialStatementTransactionCategoryRepository,
            ILKCollateralCategoryRepository collateralCategoryRepository,ILKSectorOfOperationCategoryRepository sectorOfOperationCategoryRepository ,IFileUploadRepository fileUploadRepository,IFinancialStatementSnapshotRepository fsSnapShotRepository,
            AuditingTracker tracker, SecurityUser user, string ServiceRequest)
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
            _fileUploadRepository = fileUploadRepository;
            _fsSnapshotRepository = fsSnapShotRepository;
            _serviceRequest = ServiceRequest;

        }

        public void LoadInitialDataFromRepository(NewFSRequest request)
        {

            TransactionTypeName = _financialStatementTransactionCategoryRepository.FindBy((FinancialStatementTransactionCategory)request.FSView .FinancialStatementTransactionTypeId ).FinancialStatementTransactionCategoryName;
            CollateralTypeName = _collateralCategoryRepository.FindBy((CollateralCategory)request.FSView .CollateralTypeId).CollateralCategoryName; ;
            SectorOfOperations = _sectorOfOperationCategoryRepository.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).ToList(); //**Cacheable and also stored procedure                                            
            draft = _fsSnapshotRepository.GetDbSet().Where(s => s.ServiceRequest == _serviceRequest).FirstOrDefault();
            int[] Attachments = request.FSView.FileAttachments.Where(s=>s.Id !=0).Select(s => s.Id).ToArray();
            tempAttachment = _fileUploadRepository.GetDbSet().Where(s =>Attachments.Contains (s.Id)).ToList(); 
        }

        public bool ValidateSecurity()
        {
            return (this.ValidateCreateSubmitResendSecurityRole());
        }

        public FinancialStatement CreateFinancingStatement(FSView fsview, bool SetUniqueNoLater = false)
        {
            //Create Financing Statement
            submittedFS = fsview.ConvertToNewFS(SectorOfOperations, tempAttachment);

            //Set membership
            submittedFS.MembershipId = _executingUser.MembershipId;
            //submittedFS.InstitutionUnitId = _executingUser.InstitutionUnitId;

            submittedFS.InstitutionUnitId = _executingUser.InstitutionUnitId;

            //Create
            submittedFS.Create(_executingUser.Id, _tracker);

            //Add to repository
            _financialStatementRepository.Add(submittedFS);

            //Assign registration no
            if (SetUniqueNoLater == false)
            {
                this.AssignRegistrationNo();
            }
            this.CleanupDraftAndTempAttachment();


            return submittedFS;               
            
        }

      
       

    }
}
