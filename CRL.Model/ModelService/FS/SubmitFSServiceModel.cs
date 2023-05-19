using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.Messaging;
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
using CRL.Model.ModelViewMappers;
using CRL.Model.Messaging;

namespace CRL.Model.ModelService.FS
{
    public class SubmitFSServiceModel:FSServiceModelBase
    {

        protected ICollection<FileUpload> existingAttachment = null;
         FinancialStatement submittedFS = null;
         FinancialStatement oldSubmittedFS = null; //In the case this fs was resubmitted this variable will be used to remove the old fs

        public SubmitFSServiceModel(ISerialTrackerRepository serialTrackerRepository, IFinancialStatementRepository financialStatementRepository,
            IEmailRepository emailRepository,IEmailUserAssignmentRepository emailUserAssignmentRepository,IWFCaseRepository caseRepository,ILKFinancialStatementTransactionCategoryRepository financialStatementTransactionCategoryRepository,
            ILKCollateralCategoryRepository collateralCategoryRepository, ILKSectorOfOperationCategoryRepository sectorOfOperationCategoryRepository, 
            IFileUploadRepository fileUploadRepository,IFinancialStatementSnapshotRepository fsSnapShotRepository,
            AuditingTracker tracker, SecurityUser user, String ServiceRequest)
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
            _fileUploadRepository = fileUploadRepository;
            _fsSnapshotRepository = fsSnapShotRepository;
            _serviceRequest = ServiceRequest;

        }

        public bool ValidateSecurity()
        {
           return( this.ValidateCreateSubmitResendSecurityRole());
        }

        public void LoadInitialDataFromRepository(NewFSRequest request, int? OldFinancingStatementId)
        {

            TransactionTypeName = _financialStatementTransactionCategoryRepository.FindBy((FinancialStatementTransactionCategory)request.FSView.FinancialStatementTransactionTypeId).FinancialStatementTransactionCategoryName;
            CollateralTypeName = _collateralCategoryRepository.FindBy((CollateralCategory)request.FSView.CollateralTypeId).CollateralCategoryName; ;
            SectorOfOperations = _sectorOfOperationCategoryRepository.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).ToList(); //**Cacheable and also stored procedure                                            
            draft = _fsSnapshotRepository.GetDbSet().Where(s => (s.ServiceRequest == request.UniqueGuidForm
                || (s.AssociatedIdForNonNew == request.FSView.Id  && request.FSView.Id  != 0)) && s.CreatedBy == request.SecurityUser.Id).FirstOrDefault();
            if (OldFinancingStatementId != null && OldFinancingStatementId > 0)
            {
                oldSubmittedFS = _financialStatementRepository.SelectFSById((int)OldFinancingStatementId);
               
            }
            int[] Attachments = request.FSView.FileAttachments.Where(s => s.Id != 0).Select(s => s.Id).ToArray();
            tempAttachment = _fileUploadRepository.GetDbSet().Where(s => Attachments.Contains(s.Id)).ToList(); 
        }

        public FinancialStatement SubmitFinancingStatement(FSView fsview, RequestMode mode, WFCaseFS _case = null, bool SetUniqueNoLater = false)
        {
            //Create fs model from fsview

            submittedFS = fsview.ConvertToNewFS(SectorOfOperations, tempAttachment);
            //Set membership
            submittedFS.MembershipId = _executingUser.MembershipId;            
            submittedFS.InstitutionUnitId = _executingUser.InstitutionUnitId;
            submittedFS.RegistrationDate = _tracker.Date;
            submittedFS.EffectiveDate  = _tracker.Date;

            //Audit submitted fs
            submittedFS.Submit(_tracker);

            //Add to repository
            _financialStatementRepository.Add(submittedFS);
            
            //Set to the case created for this submission
            _case.FinancialStatement = submittedFS;
         
            if (mode == RequestMode.Submit)
            {
                _caseRepository.Add(_case);
                if (SetUniqueNoLater == false)
                {
                    this.AssignRequestNo(_case);
                }
            }
            else
            {
                //Remove the financing statement, hope this will also remove all it's collaterals , participants and other identities and also fileuploads
                oldSubmittedFS.IsDeleted = true;
                submittedFS.RequestNo = _case.CaseContext;

            }

          //Assign name of the first debtor to the case title
            var firstDebtor = submittedFS.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsBorrower ).First();
           _case .CaseTitle +=" - "+ firstDebtor .GetName (true);

            this.CleanupDraftAndTempAttachment();

            return submittedFS;
        }

       

        public void AssignRequestNo(WFCaseFS _case=null)
        {
            if (oldSubmittedFS == null)
            {
                submittedFS.RequestNo = "TRG" + SerialTracker.GetSerialValue(_serialTrackerRepository, SerialTrackerEnum.TempRegistration);
                
                
                _case.SetContextNo(submittedFS.RequestNo);

            }
            else
            {
                submittedFS.RequestNo = oldSubmittedFS.RequestNo;
            }



        }     
    }
}
