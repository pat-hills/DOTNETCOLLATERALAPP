using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.Messaging;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.FS.IRepository;
using CRL.Model.ModelViews;
using CRL.Model.Model.FS;
using CRL.Model.Model.Search;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.Notification;
using CRL.Model.Notification.IRepository;
using CRL.Model.Search;
using CRL.Model.WorkflowEngine;
using CRL.Model.WorkflowEngine.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Model.ModelService.FS
{
    public class FSServiceModelBase
    {
        
        public FinancialStatement submittedFS = null;
        public string UniqeCodePrefix = null;
        protected FinancialStatementSnapshot  draft = null;
        protected ICollection<FileUpload>  tempAttachment = null;
        public LookUpForFS _lookUpFS { get; set; }
        protected ISerialTrackerRepository _serialTrackerRepository;
        protected IFinancialStatementRepository _financialStatementRepository;
        protected IEmailRepository _emailRepository;
        protected ILKFinancialStatementTransactionCategoryRepository _financialStatementTransactionCategoryRepository;
        protected ILKCollateralCategoryRepository _collateralCategoryRepository;
        protected ILKSectorOfOperationCategoryRepository _sectorOfOperationCategoryRepository;
        protected IWFCaseRepository _caseRepository;
        protected IFileUploadRepository _fileUploadRepository;
        protected IEmailTemplateRepository _emailTemplateRepository;
        protected IEmailUserAssignmentRepository _emailUserAssignmentRepository;       
        protected IFinancialStatementSnapshotRepository _fsSnapshotRepository;
        protected AuditingTracker _tracker;
        protected SecurityUser _executingUser;
        protected string _serviceRequest { get; set; }
        public string TransactionTypeName { get; set; }
        public string CollateralTypeName { get; set; }
        public static DateTime ReportGeneratedDate { get; set; }
        
        protected List<LKSectorOfOperationCategory> SectorOfOperations = null;

        public FSServiceModelBase()
        {

        }

       
        protected bool ValidateCreateSubmitResendSecurityRole()
        {

            if (!(_executingUser.IsInAnyRoles("Client Officer", "FS Officer")))
            {
                return false;
            }



            return true;

        }

        protected  bool ValidateApproveDenySecurityRole()
        {

            if (!(_executingUser.IsInAnyRoles("Client Authorizer", "FS Authorizer")))
            {
                return false;
            }
            return true;
        }

        public void AssignRegistrationNo()
        {
            submittedFS.RegistrationNo = "REG" + SerialTracker.GetSerialValue(_serialTrackerRepository, SerialTrackerEnum.Registration);
        }
        public void AssignActivityNo(string activityPrefix, FinancialStatementActivity activity, WFCaseActivity _case = null)
        {
            if (_case != null)
            {
                activity.RequestNo = activityPrefix + SerialTracker.GetSerialValue(_serialTrackerRepository, SerialTrackerEnum.TempFSActivities);
                _case.SetContextNo(activity.RequestNo);
            }
            else
                activity.ActivityCode = activityPrefix + SerialTracker.GetSerialValue(_serialTrackerRepository, SerialTrackerEnum.FSActivities);
        }

        public static FileUpload GenerateCurrentFSReport(FinancialStatement fs, FSDetailReportBuilder fsVerificationBuilder, LookUpForFS lookUpForFS, AuditingTracker _tracker)
        {
            FileUpload _fileUpload = new FileUpload();
            DateTime generatedDate = ReportGeneratedDate;
            fsVerificationBuilder.BuildCurrentReport(fs, lookUpForFS, generatedDate);
            Byte[] mybytes = fsVerificationBuilder.GenerateCurrentReport(_fileUpload.IdentifierId);
            _fileUpload.AttachedFile = mybytes;
            _fileUpload.AttachedFileName = fs.RegistrationNo + ".pdf";
            _fileUpload.AttachedFileSize = mybytes.Length.ToString();
            _fileUpload.AttachedFileType = "application/pdf";
            return _fileUpload;
         
        }

        public static FileUpload GenerateVerificationReport(FinancialStatement fs, FSDetailReportBuilder fsVerificationBuilder, LookUpForFS lookUpForFS, AuditingTracker _tracker, DateTime verificationReportDate)
        {
            FileUpload _fileUpload = new FileUpload();
            //DateTime generatedDate = fsVerificationBuilder.generatedDate;
            fsVerificationBuilder.BuildVerificationReport(fs, lookUpForFS, verificationReportDate);
            Byte[] mybytes = fsVerificationBuilder.GenerateVerificationReport(_fileUpload.IdentifierId);
            _fileUpload.AttachedFile = mybytes;
            _fileUpload.AttachedFileName = fs.RegistrationNo + ".pdf";
            _fileUpload.AttachedFileSize = mybytes.Length.ToString();
            _fileUpload.AttachedFileType = "application/pdf";
            fs.VerificationAttachment = _fileUpload;
            _tracker.Created.Add(_fileUpload);
        
            return _fileUpload;
        }

        public static FileUpload GenerateFSActivityVerificationReport(FinancialStatementActivity  fs, FSActivityReportBuilder  fsVerificationBuilder, LookUpForFS lookUpForFS, List<ClientReportView> AssigneeNAssignor, AuditingTracker _tracker)
        {
            FileUpload _fileUpload = new FileUpload();
            DateTime generatedDate = ReportGeneratedDate;
            fsVerificationBuilder.BuildVerificationReport(fs,AssigneeNAssignor, lookUpForFS, generatedDate);
            Byte[] mybytes = fsVerificationBuilder.GenerateVerificationReport(_fileUpload.IdentifierId);
            _fileUpload.AttachedFile = mybytes;
            _fileUpload.AttachedFileName = fs.ActivityCode  + ".pdf";
            _fileUpload.AttachedFileSize = mybytes.Length.ToString();
            _fileUpload.AttachedFileType = "application/pdf";
            fs.VerificationAttachment = _fileUpload;
            _tracker.Created.Add(_fileUpload);
            
            return _fileUpload;
        }


        public static FileUpload GenerateSearchFSReport(SearchFinancialStatement sch, List<FinancialStatement> SelectedFS, SearchFSReportBuilder fsVerificationBuilder, List<ClientReportView> AssigneeNAssignor, LookUpForFS lookUpForFS, AuditingTracker _tracker)
        {
            FileUpload _fileUpload = new FileUpload();
            DateTime generatedDate = ReportGeneratedDate;
            fsVerificationBuilder.BuildSearchReport(sch, true, SelectedFS, AssigneeNAssignor, lookUpForFS, generatedDate);
            Byte[] mybytes = fsVerificationBuilder.GenerateVerificationReport(_fileUpload.IdentifierId);
            _fileUpload.AttachedFile = mybytes;
            _fileUpload.AttachedFileName = sch.SearchCode  + ".pdf";
            _fileUpload.AttachedFileSize = mybytes.Length.ToString();
            _fileUpload.AttachedFileType = "application/pdf";
            sch.GeneratedReport = _fileUpload;
            _tracker.Created.Add(_fileUpload);

            return _fileUpload;
        }
        public void GenerateFSDenyEmail(EmailTemplate template, List<UserEmailView> UserEmailView, string Comment)
        {
           

            Email Notification = new Email();
            Notification._emailUserAssignmentRepository = _emailUserAssignmentRepository;
            Notification.AddEmailAddresses(UserEmailView, _tracker);




            _emailRepository.Add(Notification);
            _tracker.Created.Add(Notification);
            //EmailTemplateGenerator.CreateFinancingStatementMail(Notification, submittedFS, FSActivityEmailTemplate);
            EmailTemplateGenerator.DenyFinancingStatementMail(Notification, submittedFS, template, Comment );
        }

        public void GenerateVerificationEmail(EmailTemplate CreateFSEmailTemplate, FSDetailReportBuilder fsVerificationBuilder, LookUpForFS lookUpForFS,  List<UserEmailView> UserEmailView, string Comment, DateTime reportDate)
        {
            //Generate the veridication report         
            FileUpload _fileUpload = GenerateVerificationReport(submittedFS, fsVerificationBuilder, lookUpForFS, _tracker, reportDate);

            

            Email CreateFSNotification = new Email();
            CreateFSNotification._emailUserAssignmentRepository = _emailUserAssignmentRepository;
            CreateFSNotification.AddEmailAddresses(UserEmailView, _tracker  );   
            
            _emailRepository.Add(CreateFSNotification);        
            _tracker.Created.Add(CreateFSNotification);
            EmailTemplateGenerator.CreateFinancingStatementMail(CreateFSNotification, submittedFS, CreateFSEmailTemplate,  Comment);


        }
        
        public void CleanupDraftAndTempAttachment()
        {

            if (draft != null)
            {
                draft.IsDeleted = true;
            }

            //var tempAttachments = _fileUploadRepository.GetDbSet().Where(s => s.ServiceRequest == _serviceRequest);
            //foreach (var c in tempAttachments)
            //{
            //    if (c!= tempAttachment)
            //        _fileUploadRepository.Remove(c);
            //}
          
            

        }
        public FinancialStatement DenyFinancingStatement()
        {
           
            submittedFS.IsDeleted = true;
            _tracker.Updated.Add(submittedFS);

            foreach (Collateral c in submittedFS.Collaterals)
            {
                _tracker.Updated.Add(c);
                c.IsDeleted = true;
            }

            foreach (Participant p in submittedFS.Participants)
            {
                _tracker.Updated.Add(p);
                p.IsDeleted = true;
            }

            
            return submittedFS;
        }
        
    }
}
