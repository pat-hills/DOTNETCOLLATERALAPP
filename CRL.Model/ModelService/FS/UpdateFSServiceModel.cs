using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Domain;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.FS.IRepository;
using CRL.Model.Notification.IRepository;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViewMappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Messaging;
using CRL.Model.WorkflowEngine;


namespace CRL.Model.ModelService.FS
{
    public class UpdateFSServiceModel : ChangeServiceModel
    {
        FinancialStatement updatedFS = null; //In the case this fs was resubmitted this variable will be used to remove the old fs  //**Kwesi
        protected List<FileUpload> existingAttachment = null;
        IFinancialStatementActivityRepository _fsActivityRepository { get; set; }

        ActivityUpdate updateActivity = null;
        List<FinancialStatementActivity> oldActivities = null;
        public UpdateFSServiceModel(ISerialTrackerRepository serialTrackerRepository, IFinancialStatementRepository financialStatementRepository,
       IEmailRepository emailRepository, IEmailTemplateRepository emailTemplateRepository, IEmailUserAssignmentRepository emailUserAssignmentRepository, ILKFinancialStatementTransactionCategoryRepository financialStatementTransactionCategoryRepository,
       ILKCollateralCategoryRepository collateralCategoryRepository, ILKSectorOfOperationCategoryRepository sectorOfOperationCategoryRepository, IFileUploadRepository fileUploadRepository, IFinancialStatementSnapshotRepository fsSnapShotRepository,
       IFinancialStatementActivityRepository fsActivityRepository, AuditingTracker tracker, SecurityUser user, string ServiceRequest)
        {
            _serialTrackerRepository = serialTrackerRepository;
            _financialStatementRepository = financialStatementRepository;
            _financialStatementTransactionCategoryRepository = financialStatementTransactionCategoryRepository;
            _collateralCategoryRepository = collateralCategoryRepository;
            _tracker = tracker;
            _executingUser = user;
            _emailRepository = emailRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _emailUserAssignmentRepository = emailUserAssignmentRepository;
            _sectorOfOperationCategoryRepository = sectorOfOperationCategoryRepository;
            _fileUploadRepository = fileUploadRepository;
            _fsSnapshotRepository = fsSnapShotRepository;
            _fsActivityRepository = fsActivityRepository;
            _serviceRequest = ServiceRequest;

        }
        public override void LoadInitialDataFromRepository(FSActivityRequest request, WFCaseActivity _case = null)
        {
            UpdateFSRequest updRequest = (UpdateFSRequest)request;
            TransactionTypeName = _financialStatementTransactionCategoryRepository.FindBy(updRequest.FSView.FinancialStatementTransactionTypeId).FinancialStatementTransactionCategoryName;
            CollateralTypeName = _collateralCategoryRepository.FindBy(updRequest.FSView.CollateralTypeId).CollateralCategoryName; ;
            SectorOfOperations = _sectorOfOperationCategoryRepository.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).ToList(); //**Cacheable and also stored procedure                                            

            draft = _fsSnapshotRepository.GetDbSet().Where(s => (s.ServiceRequest == request.UniqueGuidForm
              || (s.AssociatedIdForNonNew == request.FSView.Id && request.FSView.Id != 0)) && s.CreatedBy == request.SecurityUser.Id).FirstOrDefault();
            submittedFS = _financialStatementRepository.SelectFSById(updRequest.FSView.Id);
            FSActivityMailTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "UpdateFinancingStatement").SingleOrDefault();
            UniqeCodePrefix = "UPD";
            int[] Attachments = request.FSView.FileAttachments.Where(s => s.Id != 0).Select(s => s.Id).ToArray();
            tempAttachment = _fileUploadRepository.GetDbSet().Where(s => Attachments.Contains(s.Id)).ToList();
            oldActivities = _fsActivityRepository.GetDbSet().Where(s => s.FinancialStatementId == submittedFS.Id && s.isApprovedOrDenied == 1).ToList();

        }
        public override FinancialStatementActivity Change(FSActivityRequest request, bool SetUniqueNoLater, WFCaseActivity _case = null)
        {

            //Create the new financing statement
            updatedFS = ((UpdateFSRequest)request).FSView.ConvertToNewFS(SectorOfOperations, tempAttachment);
            //Perform update of the submitted finacing statement

            //This will ensure that some missed values from the previous financing statement and other necessary
            //actions are taken
            updatedFS.Update(submittedFS, _executingUser.Id, _tracker, SectorOfOperations);

            updatedFS.MembershipId = _executingUser.MembershipId;
            //updatedFS.InstitutionUnitId = _executingUser.InstitutionUnitId;

            //Add to repository
            _financialStatementRepository.Add(updatedFS);

            //Clean up drafts and temporary attachments related to this financing statement update
            this.CleanupDraftAndTempAttachment();

            //We also need to create the activity update and set it's current to the current financing statement and it's previous to the previous
            //financing statement.  We cn get the after update by looking at the previous after update


            //Create a new activity
            updateActivity = new ActivityUpdate();
            //Set the previous financing statement to the activity
            updateActivity.PreviousFinancialStatement = submittedFS;
            //Set the updated financing statement to the activity
            updateActivity.FinancialStatement = updatedFS;


            foreach (var olderActivity in oldActivities)
            {
                olderActivity.FinancialStatement = updateActivity.FinancialStatement;


            }

            //Set the activity type to update
            updateActivity.FinancialStatementActivityTypeId = FinancialStatementActivityCategory.Update;
            //Add activity to the tracker
            _tracker.Created.Add(updateActivity);

            //Set the activity registration code here
            //Assign registration no
            //*Here is where we must look for a true change if there was really one
            if (SetUniqueNoLater == false)
            {
                this.AssignActivityNo("UPD", updateActivity);
            }
            updateActivity.CreateOperationDescription(_tracker, _lookUpFS);
            if (String.IsNullOrWhiteSpace(updateActivity.UpdateXMLDescription))
            {
                updateActivity.NoChangeDetectedAfterPerformingChange = true;
            }
            //updatedFS .FinancialStatementLastActivity = "Updated";
            updateActivity.isApprovedOrDenied = 1;
            updateActivity.IsActive = true;
            updateActivity.MembershipId = this._executingUser.MembershipId;
            updateActivity.ApprovedById = this._executingUser.Id;
            updateActivity.ApprovedDate = _tracker.Date;
            updateActivity.FinancialStatementActivityTypeId = FinancialStatementActivityCategory.Update;
            _tracker.Created.Add(updateActivity);
            _fsActivityRepository.Add(updateActivity);


            //We also need to load all activities and set the current to the current financing statement

            return updateActivity;
        }
        public override FinancialStatementActivity Deny()
        {
            throw new NotImplementedException();
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
