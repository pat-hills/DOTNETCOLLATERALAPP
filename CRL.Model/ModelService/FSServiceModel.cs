using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.FS.IRepository;
using CRL.Infrastructure.Messaging;
using CRL.Infrastructure.Authentication;
using CRL.Model.Notification;
using CRL.Model.Notification.IRepository;
using CRL.Model.FS.Enums;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViewMappers;
using CRL.Model.WorkflowEngine;
using CRL.Model.WorkflowEngine.IRepository;
using CRL.Model.Common.IRepository;
using CRL.Model.FS;
using CRL.Model.Memberships.IRepository;


namespace CRL.Model.ModelService
{
    
    public class FSServiceModel
    {
        private ISerialTrackerRepository _serialTrackerRepository ;
        private IFinancialStatementRepository _financialStatementRepository;
        private IEmailRepository  _emailRepository;
        private ILKFinancialStatementTransactionCategoryRepository _financialStatementTransactionCategoryRepository;
        private ILKCollateralCategoryRepository _collateralCategoryRepository;
        private ILKSectorOfOperationCategoryRepository _sectorOfOperationCategoryRepository;
        private IWFCaseRepository _caseRepository;
        private AuditingTracker _tracker;
        private SecurityUser _executingUser;

        public string TransactionTypeName { get; set; }
        public string CollateralTypeName { get; set; }
        public static DateTime ReportGeneratedDate { get; set; }
        List<LKSectorOfOperationCategory> SectorOfOperations = null;
       
       
      FinancialStatement financialStatement = null;
      FinancialStatement oldFS = null; //In the case this fs was resubmitted this variable will be used to remove the old fs
        public FSServiceModel ()
        {
        }
        public FSServiceModel(IFinancialStatementRepository financialStatementRepository, SecurityUser user)
        {
            _financialStatementRepository = financialStatementRepository;
            _executingUser = user;
        }
        public FSServiceModel (ISerialTrackerRepository serialTrackerRepository,IFinancialStatementRepository financialStatementRepository,
            IEmailRepository emailRepository,IWFCaseRepository caseRepository,ILKFinancialStatementTransactionCategoryRepository financialStatementTransactionCategoryRepository,
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
            _sectorOfOperationCategoryRepository = sectorOfOperationCategoryRepository;
        }


        public FSView GetFinancialViewStatementById(int Id)
        {
            //**Need to update so items are loaded indivdually to make it faster
            FinancialStatement  financialStatement = _financialStatementRepository.SelectFSById(Id);

            FSView fsview = financialStatement.ConvertToFinancialStatementView();
            fsview.CollateralsView = financialStatement.Collaterals.Where(s => s.IsActive == true && s.IsDeleted == false &&
                (s.DischargeActivityId != null || (s.DischargeActivityId == s.FinancialStatement.DischargeActivityId))).ConvertToCollateralViews();
            fsview.ParticipantsView = financialStatement.Participants.Where(s => s.IsActive == true && s.IsDeleted == false).ConvertToParticipantsView();

           
            //financialStatement.CompleteConvertToFinancialStatementView(fsview);
            return fsview;
        }

        
        /// <summary>
        /// Initialise service variables for a submit or resubmit oiperation.  The OldFinancingStatementID requires a value of more than
        /// 1 for resubmit to load the old submit
        /// </summary>
        /// <param name="FinancialStatementTransactionTypeId"></param>
        /// <param name="CollateralTypeId"></param>
        /// <param name="OldFinancingStatementId"></param>
        public void InitialiseSubmit(int FinancialStatementTransactionTypeId, int CollateralTypeId, int? OldFinancingStatementId=null)
        {

            TransactionTypeName = _financialStatementTransactionCategoryRepository.FindBy((FinancialStatementTransactionCategory )FinancialStatementTransactionTypeId).FinancialStatementTransactionCategoryName ;
            CollateralTypeName = _collateralCategoryRepository.FindBy((CollateralCategory)CollateralTypeId).CollateralCategoryName; ;
            SectorOfOperations = _sectorOfOperationCategoryRepository.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).ToList(); //**Cacheable and also stored procedure                                 
            financialStatement = new FinancialStatement();
            if (OldFinancingStatementId != null && OldFinancingStatementId > 1)
            {
                oldFS = _financialStatementRepository.SelectFSById((int)OldFinancingStatementId);
            }
           

        }
        /// <summary>
        /// Initialise service variables for a submit or resubmit oiperation.  The OldFinancingStatementID requires a value of more than
        /// 1 for resubmit to load the old submit
        /// </summary>
        /// <param name="FinancialStatementTransactionTypeId"></param>
        /// <param name="CollateralTypeId"></param>
        /// <param name="OldFinancingStatementId"></param>
        public FinancialStatement InitialiseCreate(int FinancialStatementTransactionTypeId, int CollateralTypeId, int? FinancingStatementId = null)
        {

            TransactionTypeName = _financialStatementTransactionCategoryRepository.FindBy((FinancialStatementTransactionCategory)FinancialStatementTransactionTypeId).FinancialStatementTransactionCategoryName;
            CollateralTypeName = _collateralCategoryRepository.FindBy((CollateralCategory)CollateralTypeId).CollateralCategoryName; ;
            SectorOfOperations = _sectorOfOperationCategoryRepository.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).ToList(); //**Cacheable and also stored procedure                                 

            if (FinancingStatementId != null && FinancingStatementId > 1)
            {
                financialStatement  = _financialStatementRepository.SelectFSById((int)FinancingStatementId);
            }

            return financialStatement;

        }
        public FinancialStatement RemoveFinancingStatement()
        {
            //Load the related fs
     
            financialStatement.IsDeleted = true;
            _tracker.Updated.Add(financialStatement);

            foreach (Collateral c in financialStatement.Collaterals)
            {
                _tracker.Updated.Add(c);
                c.IsDeleted = true;



            }

            foreach (Participant p in financialStatement.Participants)
            {
                _tracker.Updated.Add(p);
                p.IsDeleted = true;
            }

            return financialStatement;
        }
        public FinancialStatement CreateFinancingStatement(FSView fsview, RequestMode mode, WFCaseFS _case = null, bool SetUniqueNoLater = false)
        {
          
            
            if (mode == RequestMode.Create )
            {
                //financialStatement = fsview.ConvertToNewFS(SectorOfOperations);
                financialStatement.Create(_executingUser.Id, _tracker);
                _financialStatementRepository.Add(financialStatement);
                financialStatement.MembershipId = _executingUser.MembershipId;
                if (SetUniqueNoLater == false) this.LateAssignRegistrationNo(financialStatement );
            }
            else if (mode == RequestMode.Submit || mode == RequestMode.Resend)
            {
                //financialStatement = fsview.ConvertToNewFS(SectorOfOperations);
                financialStatement.Submit(_tracker);
                _financialStatementRepository.Add(financialStatement);
                financialStatement.MembershipId = _executingUser.MembershipId;
                _case.FinancialStatement = financialStatement;    
            }
            if (mode != RequestMode.Approval)
            {
                financialStatement.Create(_executingUser.Id, _tracker);
                if (SetUniqueNoLater == false) this.LateAssignRegistrationNo(financialStatement);
            }
            if (mode == RequestMode.Submit)
            {
                _caseRepository.Add(_case);
            }
            if (mode  == RequestMode.Resend)
            {
                //Remove the financing statement, hope this will also remove all it's collaterals , participants and other identities and also fileuploads
              
                _financialStatementRepository.Remove(oldFS);
                _financialStatementRepository.Add(financialStatement);
               
            }
          

         

         
            return financialStatement;
        }



        public void LateAssignRegistrationNo(FinancialStatement fs)
        {
            
            fs.RegistrationNo = "REG" + SerialTracker.GetSerialValue(_serialTrackerRepository, SerialTrackerEnum.Registration);
        }

        public void LateAssignRequestNo(FinancialStatement fs, WFCaseFS _case)
        {
            if (oldFS == null)
            {
                fs.RequestNo = "TRG" + SerialTracker.GetSerialValue(_serialTrackerRepository, SerialTrackerEnum.TempRegistration);
                _case.SetContextNo(fs.RequestNo);
                
            }
            else
            {
                fs.RequestNo = oldFS.RequestNo;
            }

            

        }

        public void GenerateVerificationEmail(FinancialStatement fs, EmailTemplate CreateFSEmailTemplate, FSDetailReportBuilder fsVerificationBuilder,LookUpForFS lookUpForFS)
        {
            FileUpload _fileUpload = new FileUpload();
            DateTime generatedDate = ReportGeneratedDate;
            Email  CreateFSNotification = new Email ();
            CreateFSNotification.EmailTo = _executingUser.Email;  //Workflowemails not added but will be part of this list
            _emailRepository.Add(CreateFSNotification);
            _tracker.Created.Add(CreateFSNotification);
            EmailTemplateGenerator.CreateFinancingStatementMail(CreateFSNotification, fs, CreateFSEmailTemplate,"");

            //Generate the veridication report         
            fsVerificationBuilder.BuildVerificationReport(fs, lookUpForFS, generatedDate);
            Byte[] mybytes = fsVerificationBuilder.GenerateVerificationReport (_fileUpload.IdentifierId);            
            _fileUpload.AttachedFile = mybytes;
            _fileUpload.AttachedFileName = fs.RegistrationNo + ".pdf";
            _fileUpload.AttachedFileSize = mybytes.Length.ToString();
            _fileUpload.AttachedFileType = "application/pdf";
            fs.VerificationAttachment = _fileUpload;
            _tracker.Created.Add(_fileUpload);
        }
        
        public bool ValidateSecurityRole(RequestMode mode, SecurityUser user)
        {
            if (mode == RequestMode.Create || mode == RequestMode.Submit || mode == RequestMode.Resend )
            {
                if (!(user.IsInAnyRoles("Client Officer", "FS Officer")))
                {
                    return false;
                }
            }
            else if (mode == RequestMode.Approval || mode == RequestMode.Deny)
            {
                if (!(user.IsInAnyRoles("Client Authorizer", "FS Authorizer")))
                {
                    return false;
                }
            }

            return true;
           
        }


        public static LookUpForFS GetLookUpsForFs(ILKPersonIdentificationCategoryRepository _personIdentificationCategoryRepository,
            ILKCompanyCategoryRepository _companyCategoryRepository,
            ILKCountryRepository _countryRepository,
             ILKCountyRepository _countyRepository,
            ILKNationalityRepository _nationalityRepository,
            ILKSecuringPartyIndustryCategoryRepository _securingPartyCategoryRepository,
            ILKSectorOfOperationCategoryRepository _sectorOfOperationCategoryRepository,
            ILKCollateralCategoryRepository _collateralCategoryRepository,
            ILKCollateralSubTypeCategoryRepository _collateralSubTypeCategoryRepository,
            ILKCurrencyRepository _currencyRepository,
            ILKFinancialStatementCategoryRepository _financialStatementCategoryRepository,
            ILKFinancialStatementTransactionCategoryRepository _financialStatementTransactionCategoryRepository)
        {
            LookUpForFS response = new LookUpForFS();
            response.IdentificationCardTypes = LookUpServiceModel.IdentificationCardTypes(_personIdentificationCategoryRepository);
            response.DebtorTypes = LookUpServiceModel.DebtorTypes(_companyCategoryRepository); //**caching may be necessary
            response.Countries = LookUpServiceModel.Countries(_countryRepository); //**caching may be necessary
            response.Countys = LookUpServiceModel.Countys(_countyRepository);
            response.Nationalities = LookUpServiceModel.Nationalities(_nationalityRepository); //**caching maybe necessary           
            response.SecuringPartyIndustryTypes = LookUpServiceModel.SecuringPartyTypes(_securingPartyCategoryRepository);
            response.SectorsOfOperation = LookUpServiceModel.SectorsOfOperation(_sectorOfOperationCategoryRepository); //**caching maybe necessary            
            response.CollateralTypes = LookUpServiceModel.CollateralTypes(_collateralCategoryRepository);
            response.CollateralSubTypes = LookUpServiceModel.CollateralSubTypes(_collateralSubTypeCategoryRepository); //**caching maybe necessary            
            response.Currencies = LookUpServiceModel.Currencies(_currencyRepository); //**caching may be necessary                            
            response.FinancialStatementLoanType = LookUpServiceModel.FinancialStatementLoanType(_financialStatementCategoryRepository); //**caching maybe necessary           
            response.FinancialStatementTransactionTypes = LookUpServiceModel.FinancialStatementTransactionTypes(_financialStatementTransactionCategoryRepository);
            return response;
        }

        public static FinancialStatement CreateFinancialStatementHistoryAfterUpdate(FinancialStatement  financialStatement, IFinancialStatementRepository _financialStatementRepository, IEnumerable<LKSectorOfOperationCategory> _sectorOfOperations,
            AuditingTracker auditTracker, bool AsSubmitted)
        {
             //We need to make a duplicate
            FinancialStatement financialStatementCloned = financialStatement.Duplicate(_sectorOfOperations);

            ////If there was a last activity then we need to set the clone to be the last effect of the last activity.  This sill not be necessary for all activities such as subordination

            if (!AsSubmitted)
            {   
                FinancialStatement lastActivityFinancialStatement = _financialStatementRepository.GetDbSet().Where(s => s.RegistrationNo == financialStatement.RegistrationNo && s.IsActive == false && s.IsDeleted == false && s.ClonedId ==null).OrderByDescending(s => s.CreatedOn).FirstOrDefault();
                if (lastActivityFinancialStatement != null)
                {
                    lastActivityFinancialStatement.AfterUpdateFinancialStatement = financialStatementCloned;
                }
           
            
            //Set the old cloned next version to the new finance statement
            financialStatementCloned.AfterUpdateFinancialStatement = financialStatement;
            financialStatementCloned.IsActive = false;


           
            }

            
            ////CLEAN UP AFTER CLONING
            //foreach (Collateral c in financialStatementCloned.Collaterals)
            //{
            //    c.IsActive = false; //make the collaterals not active meaning this is an old record
               
            //}

            //foreach (Participant c in financialStatementCloned.Participants)
            //{
              
            //    c.IsActive = false;            

            //}

            //Audit track cloned
            auditTracker.Updated.Add(financialStatementCloned);
            foreach (Collateral c in financialStatementCloned.Collaterals)
            {
                auditTracker.Updated.Add(c);
            }
            foreach (Participant p in financialStatementCloned.Participants)
            {
                if (p is IndividualParticipant)
                {
                    foreach (var z in ((IndividualParticipant)p).OtherPersonIdentifications)
                    {
                        auditTracker.Updated.Add(z);
                    }
                }
                auditTracker.Updated.Add(p);
            }

            return financialStatementCloned;
        }

     
    }
}
