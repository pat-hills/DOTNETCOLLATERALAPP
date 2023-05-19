using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model;
using CRL.Model.Common.IRepository;
using CRL.Model.FS.Enums;
using CRL.Model.FS.IRepository;
using CRL.Service.Interfaces;
using CRL.Service.IOC;
using CRL.Model.WorkflowEngine.IRepository;
using CRL.Model.Notification.IRepository;
using CRL.Model.Configuration.IRepository;
using CRL.Infrastructure.Messaging;
using FluentValidation.Results;
using System.Data.Entity.Infrastructure;
using CRL.Model.FS;
using CRL.Service.BusinessServices;
using CRL.Model.ModelCaching;
using CRL.Model.Search.IRepository;
using CRL.Service.Messaging.Workflow.Request;
using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Model.Factories;
using CRL.Service.Interfaces.FinancialStatement;
using CRL.Model.WorkflowEngine;
using CRL.Model.WorkflowEngine.Enums;
using CRL.Model.Messaging;
using CRL.Model.ModelService;
using CRL.Model.Payments.IRepository;
using CRL.Model.Memberships.IRepository;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using CRL.Model.Model.Search.IRepository;
using CRL.Model.Notification;
using CRL.Model.ModelViews.Memberships;



namespace CRL.Service.Common
{
    public abstract class ServiceBase
    {
        public ServiceBase()
        {
            AuditedDate = DateTime.Now;
            auditTracker = new AuditingTracker();
        }
      
        private ILKCountryRepository countryRepository;
        private ILKCountyRepository countyRepository;
       
        private ILKNationalityRepository nationalityRepository;
        private ILKSecuringPartyIndustryCategoryRepository securingPartyCategoryRepository;
        private  ILKCollateralCategoryRepository collateralCategoryRepository;
        private  ILKCollateralSubTypeCategoryRepository collateralSubTypeCategoryRepository;
        private  ILKCurrencyRepository currencyRepository;
        private  ILKFinancialStatementCategoryRepository financialStatementCategoryRepository;
        private IFinancialStatementRepository  financialStatementRepository;
        private  ILKFinancialStatementTransactionCategoryRepository financialStatementTransactionCategoryRepository;
        private  ILKPersonIdentificationCategoryRepository personIdentificationCategoryRepository;
        private  ILKSectorOfOperationCategoryRepository sectorOfOperationCategoryRepository;
        private  ILKSecuringPartyIndustryCategoryRepository securingPartyIndustryCategoryRepository;
        private  ILKCompanyCategoryRepository companyCategoryRepository;       
        private  ILKFinancialStatementActivityCategoryRepository financialStatementActivityCategoryRepository;
        private IFinancialStatementActivityRepository financialStatementActivityRepository;
        private IMembershipRepository membershipRepository;
        private IInstitutionRepository institutionRepository;
        private IInstitutionUnitRepository institutionUnitRepository;
        private  IUserRepository userRepository;
        private  IFinancialStatementSnapshotRepository financialStatementSnapshotRepository;
        private  IFileUploadRepository fileUploadRepository;
        private  ICollateralRepository collateralRepository;
        private  IWFWorkflowRepository workflowRepository;
        private  IEmailTemplateRepository emailTemplateRepository;
        private  IEmailRepository emailRepository;
        private  IAccountTransactionRepository accountTransactionRepository;
        private  IConfigurationWorkflowRepository configurationWorkflowRepository;
        private  IConfigurationFeeRepository configurationFeeRepository;
        private  IServiceRequestRepository serviceRequestRepository;
        private  IParticipantRepository participantRepository;
        private  IPersonIdentificationRepository personIdentificationRepository;
        private ISearchFinancialStatementRepository searchFinancialStatementRepository;
        private ILKLGARepository lkgaRepository;
        private  IWFCaseRepository caseRepository;
        private IWFTokenRepository wfTokenRepository;
        private ISerialTrackerRepository serialTrackerRepository;
        private IAuditRepository auditRepository;
        private IEmailUserAssignmentRepository emailUserAssignmentRepository;
        private IMessageRepository messageRepository;
        private ILKRegistrationPrefixRepository registrationPrefixRepository;
        private IPaymentRepository paymentRepository;
        private IPublicUserSecurityCodeRepository publicUserSecurityCodeRepository;
        private IRoleRepository roleRepository;
        private ILKAuditCategoryRepository auditCategoryRepository;
        private IBankVerificationCodeRepository bankVerifcationCodeRepository;
        private IPasswordResetRequestRepository passwordResetRequestRepository;
        private IInterSwitchRepository interSwitchRepository;
        private IInterSwitchDirectPayRepository  interSwitchDirectPayRepository;
        private IAccountBatchRepository accountBatchRepository;
        private ILKAuditActionRepository auditActionRepository;
        private ITrackSearchResultRepository trackSearchResultRepository;
        
        private IUnitOfWork uow;

        public ILKCollateralCategoryRepository _collateralCategoryRepository
        {
            get
            {
                if (collateralCategoryRepository == null) { collateralCategoryRepository = RepositoryFactory.LKCollateralCategoryRepository(_uow); return collateralCategoryRepository; } else return collateralCategoryRepository;
            }
            private set { ;}
        }
        public ILKCollateralSubTypeCategoryRepository _collateralSubTypeCategoryRepository
        {
            get
            {
                if (collateralSubTypeCategoryRepository == null) { collateralSubTypeCategoryRepository = RepositoryFactory.LKCollateralSubTypeCategoryRepository(_uow); return collateralSubTypeCategoryRepository; } else return collateralSubTypeCategoryRepository;
            }
            private set { ;}
        }
        public ILKCurrencyRepository _currencyRepository
        {
            get
            {
                if (currencyRepository == null) { currencyRepository = RepositoryFactory.LKCurrencyRepository(_uow); return currencyRepository; } else return currencyRepository;
            }
            private set { ;}
        }
        public ILKFinancialStatementCategoryRepository _financialStatementCategoryRepository
        {
            get
            {
                if (financialStatementCategoryRepository == null) { financialStatementCategoryRepository = RepositoryFactory.LKFinancialStatementCategoryRepository(_uow); return financialStatementCategoryRepository; } else return financialStatementCategoryRepository;
            }
            private set { ;}
        }
        public ILKFinancialStatementTransactionCategoryRepository _financialStatementTransactionCategoryRepository
        {
            get
            {
                if (financialStatementTransactionCategoryRepository  == null) { financialStatementTransactionCategoryRepository = RepositoryFactory.LKFinancialStatementTransactionCategoryRepository(_uow); return financialStatementTransactionCategoryRepository; } else return financialStatementTransactionCategoryRepository;
            }
            private set { ;}
        }
        public ILKPersonIdentificationCategoryRepository _personIdentificationCategoryRepository
        {
            get
            {
                if (personIdentificationCategoryRepository  == null) { personIdentificationCategoryRepository = RepositoryFactory.LKPersonIdentificationCategoryRepository(_uow); return personIdentificationCategoryRepository; } else return personIdentificationCategoryRepository;
            }
            private set { ;}
        }
        public ILKSectorOfOperationCategoryRepository _sectorOfOperationCategoryRepository
        {
            get
            {
                if (sectorOfOperationCategoryRepository  == null) { sectorOfOperationCategoryRepository = RepositoryFactory.LKSectorOfOperationCategoryRepository(_uow); return sectorOfOperationCategoryRepository; } else return sectorOfOperationCategoryRepository;
            }
            private set { ;}
        }
        public ILKCompanyCategoryRepository _companyCategoryRepository
        {
            get
            {
                if (companyCategoryRepository  == null) { companyCategoryRepository = RepositoryFactory.LKCompanyCategoryRepository(_uow); return companyCategoryRepository; } else return companyCategoryRepository;
            }
            private set { ;}
        }
        public IFinancialStatementActivityRepository _financialStatementActivityRepository
        {
            get
            {
                if (financialStatementActivityRepository  == null) { financialStatementActivityRepository = RepositoryFactory.FinancialStatementActivityRepository(_uow); return financialStatementActivityRepository; } else return financialStatementActivityRepository;
            }
            private set { ;}
        }
        public ILKFinancialStatementActivityCategoryRepository _financialStatementActivityCategoryRepository
        {
            get
            {
                if (financialStatementActivityCategoryRepository  == null) { financialStatementActivityCategoryRepository = RepositoryFactory.LKFinancialStatementActivityCategoryRepository(_uow); return financialStatementActivityCategoryRepository; } else return financialStatementActivityCategoryRepository;
            }
            private set { ;}
        }
        public ICollateralRepository _collateralRepository
        {
            get
            {
                if (collateralRepository  == null) { collateralRepository = RepositoryFactory.LKCollateralRepository(_uow); return collateralRepository; } else return collateralRepository;
            }
            private set { ;}
        }

        public IFinancialStatementRepository _financialStatementRepository
        {
            get
            {
                if (financialStatementRepository == null) { financialStatementRepository = RepositoryFactory.FinancialStatementRepository(_uow); return financialStatementRepository; } else return financialStatementRepository;
            }
            private set { ;}
        }
        public IWFWorkflowRepository _workflowRepository
        {
            get
            {
                if (workflowRepository  == null) { workflowRepository = RepositoryFactory.WFWorkflowRepository(_uow); return workflowRepository; } else return workflowRepository;
            }
            private set { ;}
        }
        public IWFCaseRepository _caseRepository
        {
            get
            {
                if (caseRepository  == null) { caseRepository = RepositoryFactory.WFCaseRepository(_uow); return caseRepository; } else return caseRepository;
            }
            private set { ;}
        }
        public IWFTokenRepository  _wfTokenRepository
        {
            get
            {
                if (wfTokenRepository == null) { wfTokenRepository = RepositoryFactory.WFTokenRepository(_uow); return wfTokenRepository; } else return wfTokenRepository;
            }
            private set { ;}
        }
        public IUserRepository _userRepository
        {
            get
            {
                if (userRepository  == null) { userRepository = RepositoryFactory.UserRepository(_uow); return userRepository; } else return userRepository;
            }
            private set { ;}
        }
        public IFinancialStatementSnapshotRepository _financialStatementSnapshotRepository
        {
            get
            {
                if (financialStatementSnapshotRepository  == null) { financialStatementSnapshotRepository = RepositoryFactory.FinancialStatementSnapshotRepository(_uow); return financialStatementSnapshotRepository; } else return financialStatementSnapshotRepository;
            }
            private set { ;}
        }

        public IFileUploadRepository _fileUploadRepository
        {
            get
            {
                if (fileUploadRepository  == null) { fileUploadRepository = RepositoryFactory.FileUploadRepository(_uow); return fileUploadRepository; } else return fileUploadRepository;
            }
            private set { ;}
        }
        public IEmailTemplateRepository _emailTemplateRepository
        {
            get
            {
                if (emailTemplateRepository  == null) { emailTemplateRepository = RepositoryFactory.EmailTemplateRepository(_uow); return emailTemplateRepository; } else return emailTemplateRepository;
            }
            private set { ;}
        }

        public IEmailRepository _emailRepository
        {
            get
            {
                if (emailRepository == null) { emailRepository = RepositoryFactory.EmailRepository(_uow); return emailRepository; } else return emailRepository;
            }
            private set { ;}
        }
        public IMessageRepository _messageRepository
        {
            get
            {
                if (messageRepository == null) { messageRepository = RepositoryFactory.MessageRepository(_uow); return messageRepository; } else return messageRepository;
            }
            private set { ;}
        }
        public IAccountTransactionRepository _accountTransactionRepository
        {
            get
            {
                if (accountTransactionRepository == null) { accountTransactionRepository = RepositoryFactory.AccountTransactionRepository(_uow); return accountTransactionRepository; } else return accountTransactionRepository;
            }
            private set { ;}
        }



        public IConfigurationWorkflowRepository _configurationWorkflowRepository
        {
            get
            {
                if (configurationWorkflowRepository  == null) { configurationWorkflowRepository = RepositoryFactory.ConfigurationWorkflowRepository(_uow); return configurationWorkflowRepository; } else return configurationWorkflowRepository;
            }
            private set { ;}
        }

        public IConfigurationFeeRepository _configurationFeeRepository
        {
            get
            {
                if (configurationFeeRepository == null) { configurationFeeRepository = RepositoryFactory.ConfigurationFeeRepository(_uow); return configurationFeeRepository; } else return configurationFeeRepository;
            }
            private set { ;}
        }
        public IServiceRequestRepository _serviceRequestRepository
        {
            get
            {
                if (serviceRequestRepository  == null) { serviceRequestRepository = RepositoryFactory.ServiceRequestRepository(_uow); return serviceRequestRepository; } else return serviceRequestRepository;
            }
            private set { ;}
        }
        public IParticipantRepository _participantRepository
        {
            get
            {
                if (participantRepository  == null) { participantRepository = RepositoryFactory.ParticipantRepository(_uow); return participantRepository; } else return participantRepository;
            }
            private set { ;}
        }
        public IPersonIdentificationRepository _personIdentificationRepository
        {
            get
            {
                if (personIdentificationRepository == null) { personIdentificationRepository = RepositoryFactory.PersonIdentificationRepository(_uow); return personIdentificationRepository; } else return personIdentificationRepository;
            }
            private set { ;}
        }


        public IInstitutionRepository _institutionRepository
        {
            get
            {
                if (institutionRepository == null) { institutionRepository = RepositoryFactory.InstitutionRepository(_uow); return institutionRepository; } else return institutionRepository;
            }
            private set{;}
        }
        public IInstitutionUnitRepository _institutionUnitRepository
        {
            get
            {
                if (institutionUnitRepository == null) { institutionUnitRepository = RepositoryFactory.InstitutionUnitRepository(_uow); return institutionUnitRepository; } else return institutionUnitRepository;
            }
            private set { ;}
        }

        public ILKCountryRepository _countryRepository
        {
            get
            {
                if (countryRepository == null) { countryRepository = RepositoryFactory.LKCountryRepository(_uow); return countryRepository; } else return countryRepository;
            }
            private set { ;}
        }

        public ILKCountyRepository _countyRepository
        {
            get
            {
                if (countyRepository == null) { countyRepository = RepositoryFactory.LKCountyRepository(_uow); return countyRepository; } else return countyRepository;
            }
            private set { ;}
        }

        public IMembershipRepository _membershipRepository
        {
            get
            {
                if (membershipRepository == null) {membershipRepository= RepositoryFactory.MembershipRepository(_uow);return membershipRepository;} else return membershipRepository;
            }
            private set { ;}
        }
        public ILKNationalityRepository _nationalityRepository
        {
            get
            {
                if (nationalityRepository == null) { nationalityRepository = RepositoryFactory.LKNationalityRepository(_uow); return nationalityRepository; } else return nationalityRepository;
            }
            private set { ;}
        }
        public ILKSecuringPartyIndustryCategoryRepository _securingPartyCategoryRepository
        {
            get
            {
                if (securingPartyCategoryRepository == null) { securingPartyCategoryRepository = RepositoryFactory.LKSecuringPartyIndustryCategoryRepository(_uow); return securingPartyCategoryRepository; } else return securingPartyCategoryRepository;
            }
            private set { ;}
        }
        public ISerialTrackerRepository _serialTrackerRepository
        {
            get
            {
                if (serialTrackerRepository == null) { serialTrackerRepository = RepositoryFactory.SerialTrackerRepository(_uow); return serialTrackerRepository; } else return serialTrackerRepository;
            }
            private set { ;}
        }



        public IAuditRepository _auditRepository
        {
            get
            {
                if (auditRepository == null) { auditRepository = RepositoryFactory.AuditRepository(_uow); return auditRepository; } else return auditRepository;
            }
            private set { ;}
        }

        public ILKAuditCategoryRepository _auditCategoryRepository
        {

            get
            {
                if (auditCategoryRepository == null) { auditCategoryRepository = RepositoryFactory.AuditCategoryRepository(_uow); return auditCategoryRepository; } else return auditCategoryRepository;
            }
            private set { ;}
        }

        public ISearchFinancialStatementRepository _searchFinancialStatementRepository
        {
            get
            {
                if (searchFinancialStatementRepository == null) { searchFinancialStatementRepository = RepositoryFactory.SearchFinancialStatementRepository (_uow); return searchFinancialStatementRepository; } else return searchFinancialStatementRepository;
            }
            private set { ;}
        }

        public IEmailUserAssignmentRepository  _emailUserAssignmentRepository
        {
            get
            {
                if (emailUserAssignmentRepository == null) { emailUserAssignmentRepository = RepositoryFactory.EmailUserAssignmentRepository(_uow); return emailUserAssignmentRepository; } else return emailUserAssignmentRepository;
            }
            private set { ;}
        }
          public ILKLGARepository  _lkgaRepository
        {
            get
            {
                if (lkgaRepository == null) { lkgaRepository = RepositoryFactory.LKLGARepository (_uow); return lkgaRepository; } else return lkgaRepository;
            }
            private set { ;}
        }

          public ILKRegistrationPrefixRepository _registrationPrefixRepository
          {
              get
              {
                  if (registrationPrefixRepository == null) { registrationPrefixRepository = RepositoryFactory.LKRegistrationPrefixRepository  (_uow); return registrationPrefixRepository; } else return registrationPrefixRepository;
              }
              private set { ;}
          }

          public IPublicUserSecurityCodeRepository _publicUserSecurityCodeRepository
          {
              get
              {
                  if (publicUserSecurityCodeRepository == null) { publicUserSecurityCodeRepository = RepositoryFactory.PublicUserSecurityCodeRepository(_uow); return publicUserSecurityCodeRepository; } else return publicUserSecurityCodeRepository;
              }
              private set { ;}
          }

        public IPaymentRepository _paymentRepository
        {
            get
            {
                if (paymentRepository == null) { paymentRepository = RepositoryFactory.PaymentRepository(_uow); return paymentRepository;} else { return paymentRepository;}
            }
            private set { ;}
        }

          public IRoleRepository _roleRepository
          {
              get
              {
                  if (roleRepository == null) { roleRepository = RepositoryFactory.RoleRepository(_uow); return roleRepository; } else return roleRepository;
              }
              private set { ;}
          }

          public IBankVerificationCodeRepository _bankVerifcationCodeRepository
          {
              get
              {
                  if (bankVerifcationCodeRepository == null) { bankVerifcationCodeRepository = RepositoryFactory.BankVerificationCodeRepository(_uow); return bankVerifcationCodeRepository; } else return bankVerifcationCodeRepository;
              }
              private set { ;}
          }

          public IPasswordResetRequestRepository _passwordResetRequestRepository
          {
              get
              {
                  if (passwordResetRequestRepository == null) { passwordResetRequestRepository = RepositoryFactory.PasswordResetRequestRepository(_uow); return passwordResetRequestRepository; } else return passwordResetRequestRepository;
              }
              private set { ;}
          }

          public IInterSwitchRepository  _interSwitchRepository
          {
              get
              {
                  if (interSwitchRepository == null) { interSwitchRepository = RepositoryFactory.InterSwitchRepository(_uow); return interSwitchRepository; } else return interSwitchRepository;
              }
              private set { ;}
          }
          public IInterSwitchDirectPayRepository _interSwitchDirectPayRepository
          {
              get
              {
                  if (interSwitchDirectPayRepository == null) { interSwitchDirectPayRepository = RepositoryFactory.InterSwitchDirectPayRepository(_uow); return interSwitchDirectPayRepository; } else return interSwitchDirectPayRepository;
              }
              private set { ;}
          }

        public IAccountBatchRepository _accountBatchRepository
        {
            get
            {
                if (accountBatchRepository == null) { accountBatchRepository = RepositoryFactory.AccountBatchRepository(_uow); return accountBatchRepository; } else return accountBatchRepository;
            }
            private set { ;}
        }

        public ILKAuditActionRepository _auditActionRepository 
        {
            get
            {
                if (auditActionRepository == null) { auditActionRepository = RepositoryFactory.AuditActionRepository(_uow); return auditActionRepository; } else return auditActionRepository;
            }
            private set { ;}
        }

        public ITrackSearchResultRepository _trackSearchResultRepository 
        {
            get 
            {
                if (trackSearchResultRepository == null) { trackSearchResultRepository = RepositoryFactory.TrackSearchResultRepository(_uow); return trackSearchResultRepository; } else return trackSearchResultRepository;
            }
            private set { ;}
        }


        
        public IUnitOfWork _uow
        {
            get
            {
                if (uow == null) { uow = RepositoryFactory.UnitOfWork(); return uow; } else return uow;
            }
            private set { ;}
        }

        protected bool ValidateUniqueServiceRequest(string UniqueGuidForm)
        {
            
            //Make sure service request is unique
            if (String.IsNullOrWhiteSpace(UniqueGuidForm)==false)
            {
                ServiceRequest serRequest = _serviceRequestRepository.GetDbSet().Where(s => s.RequestNo == UniqueGuidForm).SingleOrDefault();
                return this.ValidateUniqueServiceRequest(UniqueGuidForm, serRequest);
            }

            return true;
               

                
            
        }
        protected bool ValidateUniqueServiceRequest(string UniqueGuidForm, ServiceRequest serRequest)
        {
           
         
            if (!String.IsNullOrWhiteSpace(UniqueGuidForm))
            {

                if (serRequest != null)
                {
                    return false;

                }
                else
                {
                    serRequest = new ServiceRequest();
                    serRequest.RequestNo = UniqueGuidForm;
                    _serviceRequestRepository.Add(serRequest);
                    return true;

                }
            }

            return true;
        }

        protected ResponseBase HandleValidationResult(ValidationResult result)
        {
            ResponseBase response =null;
            if (!result.IsValid)  //If there are errors then we need to map them to the ModelState
            { 
                response = new ResponseBase();
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError;
                response.MessageInfo.Message = "Business validation errors detected!";//**Add the validation error here
                foreach (var failure in result.Errors)
                {
                    String propertyname = failure.PropertyName;
                   
                    response.ValidationErrors.Add(new ValidationError { ErrorMessage = failure.ErrorMessage, PropertyName = failure.PropertyName });
                  

                }
                
             
                return response;
            }

            response = new ResponseBase();
            response.GenerateDefaultSuccessMessage();
            return response;
        }

        public ResponseBase ReturnAlreadyRequestedResponse()
        {
            ResponseBase response = new ResponseBase();
            response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError, Message = "This request has already been submitted successfully." };          
            return response;
        }
        public ResponseBase ReturnUnAuthorizedResponse()
        {
            ResponseBase response = new ResponseBase();
            response.GenerateDefaultUnauthorisedMessage ();
            return response;
        }

        public ResponseBase AuditCommitWithConcurrencyCheck(string AuditMessage, string RequestUrl, string UserIP, AuditAction auditAction, int UserId)
        {
            ResponseBase response = new ResponseBase();
            Audit audit = new Audit(AuditMessage, RequestUrl, UserIP, auditAction);
            _auditRepository.Add(audit);
            auditTracker.Created.Add(audit);
            _uow.AuditEntities(UserId, AuditedDate, auditTracker);
            //Commit
            try
            {
                
                _uow.Commit();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Update original values from the database 
                var entry = ex.Entries.Single();
                entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.DatabaseConcurrencyConflict;
                return response;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                var sqlException = ex.InnerException as SqlException;

                if (sqlException != null && sqlException.Errors.OfType<SqlError>()
                    .Any(se => se.Number == 2601 || se.Number == 2627 /* PK/UKC violation */))
                {

                    response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.DatabaseUniqueKeyConflict;
                    return response;
                }
                else
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            response.GenerateDefaultSuccessMessage();
            return response;

        }

        public ResponseBase AuditCommit(string AuditMessage, string RequestUrl, string UserIP, AuditAction AuditAction, int UserId)
        {
            ResponseBase response = new ResponseBase();
            Audit audit = new Audit(AuditMessage, RequestUrl, UserIP, AuditAction);
            _auditRepository.Add(audit);
            auditTracker.Created.Add(audit);
            _uow.AuditEntities(UserId, AuditedDate, auditTracker);
            //Commit
            _uow.Commit();         
           
            response.GenerateDefaultSuccessMessage();
            return response;
        }

        protected LookUpForFS GetLookUpsForFs(bool LoadAllInActives=false)
        {
            LookUpForFS response = new LookUpForFS();
            response.IdentificationCardTypes = CachedData.GetLookUpData(CachedData.CACHE_IDENTIFICATIONCARDTYPES) ?? LookUpServiceModel.IdentificationCardTypes(_personIdentificationCategoryRepository);
            response.DebtorTypes = CachedData.GetLookUpData(CachedData.CACHE_DEBTORTYPES) ?? LookUpServiceModel.DebtorTypes(_companyCategoryRepository); //**caching may be necessary
            response.Countries = CachedData.GetLookUpData(CachedData.CACHE_COUNTRIES) ?? LookUpServiceModel.Countries(_countryRepository); //**caching may be necessary
            response.Countys = CachedData.GetLookUpData(CachedData.CACHE_COUNTYS) ?? LookUpServiceModel.Countys(_countyRepository);
            response.Nationalities = CachedData.GetLookUpData(CachedData.CACHE_NATIONALITIES) ?? LookUpServiceModel.Nationalities(_nationalityRepository); //**caching maybe necessary  
            response.SecuringPartyIndustryTypes = CachedData.GetLookUpData(CachedData.CACHE_SECURINGPARTYTYPES) ?? LookUpServiceModel.SecuringPartyTypes(_securingPartyCategoryRepository);
            response.SectorsOfOperation = CachedData.GetLookUpData(CachedData.CACHE_SECTOROFOPERATIONS) ?? LookUpServiceModel.SectorsOfOperation(_sectorOfOperationCategoryRepository); //**caching maybe necessary
            response.CollateralTypes = CachedData.GetLookUpData(CachedData.CACHE_COLLATERALTYPES) ?? LookUpServiceModel.CollateralTypes(_collateralCategoryRepository);
            response.CollateralSubTypes = CachedData.GetLookUpData(CachedData.CACHE_COLLATERALSUBTYPES) ?? LookUpServiceModel.CollateralSubTypes(_collateralSubTypeCategoryRepository); //**caching maybe necessary
            response.Currencies = CachedData.GetLookUpData(CachedData.CACHE_CURRENCYTYPES) ?? LookUpServiceModel.Currencies(_currencyRepository); //**caching may be necessary                
            response.FinancialStatementLoanType = CachedData.GetLookUpData(CachedData.CACHE_LOANTYPES) ?? LookUpServiceModel.FinancialStatementLoanType(_financialStatementCategoryRepository); //**caching maybe necessary
            response.FinancialStatementTransactionTypes = CachedData.GetLookUpData(CachedData.CACHE_TRANSACTIONTYPES) ?? LookUpServiceModel.FinancialStatementTransactionTypes(_financialStatementTransactionCategoryRepository);
            response.RegistrationNoPrefix = LookUpServiceHelper.BusinessRegPrefixes(_registrationPrefixRepository);
            response.LGAs = LookUpServiceHelper.LGAsLK(_lkgaRepository );
            return response;
        }

        /// <summary>
        /// **Review to include activities
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual ResponseBase HandleTask(HandleWorkItemRequest request)
        {    
            //Create the response class
            ResponseBase response = null;
            string ItemLabel="";
            WorkflowRequestType workflowRequestType= new WorkflowRequestType ();
            HandleWorkItemRequest itemRequest = HandleWorkItemRequestFactory.CreateHandleWorkItemRequest(request);
            request.CloneTo(itemRequest );

            //Create the correct
            if (itemRequest is NewFSRequest && (request.TransitionId == 4 || request.TransitionId == 5))
            {
                ItemLabel = "financing statement";
                workflowRequestType = WorkflowRequestType.FinancialStatement;
                IFinancingStatementService fs = (IFinancingStatementService)(this);
                if (request.TransitionId == 4)
                {
                    itemRequest.RequestMode = RequestMode.Approval;
                    response = fs.Authorize(itemRequest);
                }
                else if (request.TransitionId == 5)
                {
                    itemRequest.RequestMode = RequestMode.Deny;
                    response = fs.Deny(itemRequest);
                }
            }
            else if (itemRequest is UpdateFSRequest && (request.TransitionId == 4 || request.TransitionId == 5))
            {
                ItemLabel = "financing statement update";
                workflowRequestType = WorkflowRequestType.UpdateFinancingStatement ;
                IFinancingStatementService fs = (IFinancingStatementService)(this);
                if (request.TransitionId == 4)
                {
                    itemRequest.RequestMode = RequestMode.Approval;
                    response = fs.AuthorizeChange (itemRequest);
                }
                else if (request.TransitionId == 5)
                {
                    itemRequest.RequestMode = RequestMode.Deny;
                    response = fs.DenyChange (itemRequest);
                }
            }
           
            else if (((request.TransitionId == 15|| request.TransitionId == 17) && itemRequest.wfTaskType == WFTaskType.AssignRegistration )
                || request.TransitionId == 19 || request.TransitionId == 20 || request.TransitionId == 10)
            {
                ItemLabel = "financing statement change";
                workflowRequestType = WorkflowRequestType.FinancialStatementActivity ;
                IFinancingStatementService fs = (IFinancingStatementService)(this);

                if (request.TransitionId == 15  ) 
                {

                    itemRequest.RequestMode = RequestMode.Approval;
                    response = fs.AuthorizeChange(itemRequest);
                   
                }
                else if (request.TransitionId == 17) 
                {

                    itemRequest.RequestMode = RequestMode.Deny ;
                    response = fs.DenyChange(itemRequest);
                }
                else if ( request.TransitionId == 19) 
                {

                    itemRequest.RequestMode = RequestMode.Approval;
                    response = fs.AuthorizeChange(itemRequest);
                    
                }
                else if (request.TransitionId == 20 || request .TransitionId ==10)
                {
                    itemRequest.RequestMode = RequestMode.Deny ;
                    response = fs.DenyChange(itemRequest);
                }

            }
            else
            {
                if (request.wfTaskType == WFTaskType.CreateRegistration)
                {
                    workflowRequestType = WorkflowRequestType.FinancialStatement;
                    ItemLabel = "financing statement";
                }
                else if (request.wfTaskType == WFTaskType.UpdateRegistration )
                {
                    workflowRequestType = WorkflowRequestType.FinancialStatementActivity  ;
                    ItemLabel = "financing statement";
                }
                else if (request.wfTaskType == WFTaskType.AssignRegistration )
                {
                     workflowRequestType = WorkflowRequestType.FinancialStatementActivity  ;
                    ItemLabel = "financing statement";
                }

                //**This section must be handled in the workflow service class               
                WorkflowServiceModel wfServiceModel = new WorkflowServiceModel(_emailTemplateRepository, _workflowRepository, _emailRepository, _emailUserAssignmentRepository, _userRepository, _caseRepository, auditTracker, request.SecurityUser);
                var _case = wfServiceModel.InitialiseCase(request.WorkflowId, workflowRequestType, request.Comment, request.CaseId); //**Revise this so that it is general
              
                WFWorkItem workItem = _case.WorkItems.Where(s => s.Id == request.WorkItemId).Single();
                int? LimitToMembershipId = null;
                if (request.TransitionId == 9 )
                {
                    ActivityAssignment AS = (ActivityAssignment)((WFCaseActivity)_case).FinancialStatementActivity;
                    LimitToMembershipId = AS.AssignedMembershipId;
                    _case.LimitedToOtherMembershipId = AS.AssignedMembershipId;
                    _case.LimitedToOtherUnitId = null;
                    auditTracker.Updated.Add(_case);
                }

                //else if (request.TransitionId == 10)
                //{
                //    ActivityAssignment AS = (ActivityAssignment)((WFCaseActivity)_case).FinancialStatementActivity;
                //    AS.FinancialStatement.isPendingAmendment = false;
                //    AS.FinancialStatement.FinancialStatementLastActivity = AS.PreviousActivity;

                //    Email closeCaseMail = new Email();
                //    UserEmailView caseEmailCreator  = new UserEmailView
                //    {
                //        Email = _case.CreatedByUser.Address.Email,
                //        Id = _case.CreatedBy,
                //    };
                //    closeCaseMail._emailUserAssignmentRepository = _emailUserAssignmentRepository;
                //    closeCaseMail.EmailTo = caseEmailCreator.Email;
                //    closeCaseMail.IsSent = false;
                //    closeCaseMail.NumRetries = 0;
                //    closeCaseMail.AddEmailAddresses(caseEmailCreator, auditTracker);                 

                //    //Generate template
                //    EmailTemplate DenyAmendmentEmailTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "DenyAmendment").SingleOrDefault();
                //    EmailTemplateGenerator.DenyAmendmentMail (closeCaseMail ,AS,DenyAmendmentEmailTemplate,request .Comment );

                  
                //    _emailRepository.Add(closeCaseMail);
                //    auditTracker.Created.Add(closeCaseMail);
                  
                    

                //}
                PerformWorkItemResponse wrkItemResponse = wfServiceModel.ProcessWorkItem(request.WorkItemId,false,LimitToMembershipId );
                if (wrkItemResponse.Success == false)
                {
                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                    return response;
                }
                var responseAfterCommit = this.AuditCommit(workItem.Transition.ActionTakenName.Replace("item", ItemLabel), request.RequestUrl, request.UserIP, AuditAction.HandledTask, request.SecurityUser.Id);
                if (responseAfterCommit.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(responseAfterCommit); return response; }




                response = new ResponseBase();
                //response.FSView   = request (financialStatement.Id);
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
                response.MessageInfo.Message = "Successfully " + workItem.Transition.ActionTakenName;
            }

            return response;
        }
       
        public AuditingTracker auditTracker { get; set; }
        public DateTime AuditedDate { get; set; }
        public String AuditMessage;
        public AuditAction AuditAction { get; set; }

       
       

    }
}

