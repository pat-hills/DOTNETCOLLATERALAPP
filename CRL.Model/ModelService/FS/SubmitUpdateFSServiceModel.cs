using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Domain;
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
using CRL.Model.Messaging;

namespace CRL.Model.ModelService.FS
{
    public class SubmitUpdateFSServiceModel : ChangeServiceModel
    {
        FinancialStatementActivity previousUpdateActivity=null;
        FinancialStatement updatedFS = null;
        public bool Resubmit { get; set; }
        protected List<FileUpload> existingAttachment = new List<FileUpload> ();
        IFinancialStatementActivityRepository _fsActivityRepository { get; set; }
       
        ActivityUpdate updateActivity = null;
        public SubmitUpdateFSServiceModel(ISerialTrackerRepository serialTrackerRepository, IFinancialStatementRepository financialStatementRepository,
            IEmailRepository emailRepository,ILKFinancialStatementTransactionCategoryRepository financialStatementTransactionCategoryRepository,
            ILKCollateralCategoryRepository collateralCategoryRepository,ILKSectorOfOperationCategoryRepository sectorOfOperationCategoryRepository ,IFileUploadRepository fileUploadRepository,IFinancialStatementSnapshotRepository fsSnapShotRepository,
            IFinancialStatementActivityRepository fsActivityRepository, AuditingTracker tracker, SecurityUser user, string ServiceRequest)
        {
            _serialTrackerRepository=serialTrackerRepository;
            _financialStatementRepository=financialStatementRepository;
            _financialStatementTransactionCategoryRepository = financialStatementTransactionCategoryRepository;
            _collateralCategoryRepository = collateralCategoryRepository;
            _tracker = tracker;
            _executingUser = user;
            _emailRepository = emailRepository;
            _sectorOfOperationCategoryRepository = sectorOfOperationCategoryRepository;
            _fileUploadRepository = fileUploadRepository;
            _fsSnapshotRepository = fsSnapShotRepository;
            _fsActivityRepository = fsActivityRepository;
            _serviceRequest = ServiceRequest;

        }


            


             public override FinancialStatementActivity Change(Messaging.FSActivityRequest request, bool SetUniqueNoLater, WFCaseActivity _case = null)
             {
                 UpdateFSRequest urequest = (UpdateFSRequest)(request);
                 //Create the new financing statement
                 updatedFS  = urequest.FSView.ConvertToNewFS(SectorOfOperations,tempAttachment );
                 //Set membership
                 updatedFS.MembershipId = _executingUser.MembershipId;
                 updatedFS.InstitutionUnitId  = _executingUser.InstitutionUnitId ;
                 updatedFS.RegistrationDate = _tracker.Date;
                 updatedFS.EffectiveDate = _tracker.Date;

                 //Audit submitted fs
                 updatedFS.Submit(_tracker);
                 //Perform update of the submitted finacing statement

                 //Add to repository
                 _financialStatementRepository.Add(updatedFS);



                 //Add to repository


                 //Clean up drafts and temporary attachments related to this financing statement update
                 this.CleanupDraftAndTempAttachment();

                 //We also need to create the activity update and set it's current to the current financing statement and it's previous to the previous
                 //financing statement.  We cn get the after update by looking at the previous after update

                 if (request.RequestMode == CRL.Infrastructure.Messaging.RequestMode.Resend)
                 {
                     submittedFS .IsDeleted = true;
                     updateActivity.FinancialStatement = updatedFS;
                 }
                 else
                 {
                     //Set the activity type to update
                     updateActivity.FinancialStatementActivityTypeId = FinancialStatementActivityCategory.Update;

                     //Add activity to the tracker
                     _tracker.Created.Add(updateActivity);

                     //Set the activity request code here                     
                     if (SetUniqueNoLater == false)
                     {
                         this.AssignActivityNo("UPD", updateActivity, _case);
                     }

                     updateActivity.PreviousFinancialStatement = submittedFS ;
                     updateActivity.FinancialStatement = updatedFS ;

                     //Set to the case created for this submission
                     _case.FinancialStatementActivity = updateActivity;

                     updateActivity.PreviousActivity = submittedFS.FinancialStatementLastActivity;
                     updateActivity.isApprovedOrDenied = 0;
                     updateActivity.IsActive = true;
                     updateActivity.MembershipId = this._executingUser.MembershipId;
                     submittedFS.isPendingAmendment = true;
                     //submittedFS.FinancialStatementLastActivity = "Pending Update";
                     _tracker.Created.Add(updateActivity);
                     _fsActivityRepository.Add(updateActivity);

                     //Set to the case created for this submission
               
                       
                     
                 
                 }
                     updateActivity.CreateOperationDescription(_tracker, _lookUpFS);
                     if (String.IsNullOrWhiteSpace(updateActivity.UpdateXMLDescription))
                     {
                         updateActivity.NoChangeDetectedAfterPerformingChange = true;
                     }
                     _case.CaseTitle = "Authorize Financing Statement Update - " + submittedFS .RegistrationNo  ;
                  

                   
                       


                 return updateActivity;
             }

             public override void LoadInitialDataFromRepository(Messaging.FSActivityRequest request, WFCaseActivity _case = null)
             {
                 UpdateFSRequest urequest = (UpdateFSRequest)(request);
                 SectorOfOperations = _sectorOfOperationCategoryRepository.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).ToList(); //**Cacheable and also stored procedure                                            
                 
                 draft = _fsSnapshotRepository.GetDbSet().Where(s => (s.ServiceRequest == request.UniqueGuidForm
              || (s.AssociatedIdForNonNew == request.FSView.Id && request.FSView.Id != 0)) && s.CreatedBy == request.SecurityUser.Id).FirstOrDefault();
                 if (request.RequestMode == CRL.Infrastructure.Messaging.RequestMode.Resend)
                 {
                     updateActivity = (ActivityUpdate)_case.FinancialStatementActivity;
                     submittedFS  = updateActivity.FinancialStatement  ;
                 }
                 else
                 {
                     //Create a new activity
                     updateActivity = new ActivityUpdate();
                     submittedFS  = _financialStatementRepository.SelectFSById((int)urequest .FinancialStatementId );

                 }
                 TransactionTypeName = submittedFS.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName;
                 CollateralTypeName = submittedFS.CollateralType.CollateralCategoryName;
                 int[] Attachments = request.FSView.FileAttachments.Where(s => s.Id != 0).Select(s => s.Id).ToArray();
                 tempAttachment = _fileUploadRepository.GetDbSet().Where(s => Attachments.Contains(s.Id)).ToList(); 
                 UniqeCodePrefix = "UPD";
             }



             public override FinancialStatementActivity Deny()
             {
                 throw new NotImplementedException();
             }

             public override  int GetCurrentFSId()
             {
                 return updatedFS.Id;
             }
    }
}
