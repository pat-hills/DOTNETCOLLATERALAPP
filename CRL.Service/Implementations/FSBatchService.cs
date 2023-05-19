using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.FS.IRepository;
using CRL.Infrastructure.Messaging;
using CRL.Service.Interfaces;
using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Service.Messaging.FinancialStatements.Response;
using CRL.Service.QueryGenerator;
using CRL.Service.Views;
using CRL.Service.Mappings.FinancialStatement;
using CRL.Model.WorkflowEngine;
using CRL.Model.Notification;
using CRL.Model.WorkflowEngine.IRepository;

using CRL.Model.Notification.IRepository;
using System.Data.Entity.Infrastructure;

using CRL.Service.MicrosoftReportGenerator;

using CRL.Model.Configuration.IRepository;
using CRL.Service.BusinessServices;
using CRL.Model.Common.IRepository;
using CRL.Model.Configuration;
using CRL.Model.ModelViews;
using CRL.Service.Views.Workflow;
using CRL.Service.Mappings.Membership;
using CRL.Service.Messaging.Common.Request;
using CRL.Service.Views.FinancialStatement;
using CRL.Infrastructure.Configuration;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.WorkflowEngine.Enums;
using CRL.Model.ModelViewMappers;
using CRL.Model.Memberships.IRepository;
using CRL.Model.Payments.IRepository;
using CRL.Model.Payments;
using CRL.Model.Memberships;
using CRL.Model.ModelViewValidators.FinancingStatement;
using CRL.Service.Common;
using CRL.Infrastructure.Authentication;
using CRL.Model.ModelService.FS;
using CRL.Model.ModelService;
using MicrosoftReportGenerators;
using CRL.Model.ModelViews.Memberships;
using System.Xml;


namespace CRL.Service.Implementations
{
    public class FSBatchService : ServiceBase, IFSBatchService
    {
        private AuditingTracker auditTracker;
        private DateTime AuditedDate;
        private String AuditMessage;
        private AuditAction AuditAction;
        private readonly IUnitOfWork _uow;

        private readonly IFinancialStatementRepository _financialStatementRepository;
        private readonly IFSBatchRepository _fsBatchRepository;
        private readonly IFSBatchDetailRepository _fsBatchDetailRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly ILKSectorOfOperationCategoryRepository _sectorOfOperationCategoryRepository;
        private readonly ISerialTrackerRepository _serialTrackerRepository;
        private readonly IWFWorkflowRepository _workflowRepository;
        private readonly IWFCaseRepository _caseRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IEmailUserAssignmentRepository _emailUserAssignmentRepository;
        private readonly IEmailRepository _emailRepository;
        private readonly IAccountTransactionRepository _accountTransactionRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly ILKCurrencyRepository _currencyRepository;
        private readonly IConfigurationFeeRepository _configurationFeeRepository;

        private readonly ILKCollateralCategoryRepository _collateralCategoryRepository;
        private readonly ILKCollateralSubTypeCategoryRepository _collateralSubTypeCategoryRepository;

        private readonly ILKFinancialStatementCategoryRepository _financialStatementCategoryRepository;
        private readonly ILKFinancialStatementTransactionCategoryRepository _financialStatementTransactionCategoryRepository;
        private readonly ILKPersonIdentificationCategoryRepository _personIdentificationCategoryRepository;

        private readonly ILKSecuringPartyIndustryCategoryRepository _securingPartyIndustryCategoryRepository;
        private readonly ILKCompanyCategoryRepository _companyCategoryRepository;
        private readonly ILKFinancialStatementActivityCategoryRepository _financialStatementActivityCategoryRepository;
        private readonly ILKCountryRepository _countryRepository;
        private readonly ILKCountyRepository _countyRepository;
        private readonly ILKLGARepository _lgaRepository;
        private readonly ILKRegistrationPrefixRepository _businessRegPrefix;
        private readonly ILKNationalityRepository _nationalityRepository;
        private readonly IConfigurationWorkflowRepository _configurationWorkflowRepository;

        private readonly IInstitutionRepository _institutionRepository;

        public FSBatchService(IUnitOfWork uow, IFinancialStatementRepository financialStatementRepository,
            IFSBatchRepository fsBatchRepository, IFSBatchDetailRepository fsBatchDetailRepository,
            IAuditRepository auditRepository, ILKSectorOfOperationCategoryRepository sectorOfOperationCategoryRepository,
                ISerialTrackerRepository serialTrackerRepository,
         IWFWorkflowRepository workflowRepository,
         IWFCaseRepository caseRepository,
         IUserRepository userRepository,
         IEmailTemplateRepository emailTemplateRepository, IEmailUserAssignmentRepository emailUserAssignmentRepository,
         IEmailRepository emailRepository, ILKCollateralCategoryRepository collateralCategoryRepository,
          ILKCollateralSubTypeCategoryRepository collateralSubTypeCategoryRepository,
          ILKCurrencyRepository currencyRepository,
          ILKFinancialStatementCategoryRepository financialStatementCategoryRepository,

          ILKFinancialStatementTransactionCategoryRepository financialStatementTransactionCategoryRepository,
          ILKPersonIdentificationCategoryRepository personIdentificationCategoryRepository,

          ILKSecuringPartyIndustryCategoryRepository securingPartyIndustryCategoryRepository,
           ILKCompanyCategoryRepository companyCategoryRepository, ILKFinancialStatementActivityCategoryRepository financialStatementActivityCategoryRepository,
             ILKCountyRepository countyRepository, ILKCountryRepository countryRepository,
            ILKLGARepository lgaRepository,
            ILKRegistrationPrefixRepository businessRegPrefix,
          ILKNationalityRepository nationalityRepository,
            IConfigurationWorkflowRepository configurationWorkflowRepository,
               IMembershipRepository membershipRepository, IConfigurationFeeRepository configurationFeeRepository, IAccountTransactionRepository accountTransactionRepository,
             IInstitutionRepository institutionRepository
            )
        {
            _financialStatementRepository = financialStatementRepository;
            _fsBatchRepository = fsBatchRepository;
            _fsBatchDetailRepository = fsBatchDetailRepository;
            _auditRepository = auditRepository;
            _businessRegPrefix = businessRegPrefix;
            _serialTrackerRepository = serialTrackerRepository;
            _workflowRepository = workflowRepository;
            _caseRepository = caseRepository;
            _userRepository = userRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _emailRepository = emailRepository;
            _emailUserAssignmentRepository = emailUserAssignmentRepository;
            _sectorOfOperationCategoryRepository = sectorOfOperationCategoryRepository;
            _collateralCategoryRepository = collateralCategoryRepository;
            _collateralSubTypeCategoryRepository = collateralSubTypeCategoryRepository;
            _currencyRepository = currencyRepository;
            _financialStatementCategoryRepository = financialStatementCategoryRepository;
            _personIdentificationCategoryRepository = personIdentificationCategoryRepository;
            _securingPartyIndustryCategoryRepository = securingPartyIndustryCategoryRepository;
            _companyCategoryRepository = companyCategoryRepository;
            _financialStatementActivityCategoryRepository = financialStatementActivityCategoryRepository;
            _countyRepository = countyRepository;
            _lgaRepository = lgaRepository;
            _nationalityRepository = nationalityRepository;
            _countryRepository = countryRepository;
            _financialStatementTransactionCategoryRepository = financialStatementTransactionCategoryRepository;
            _configurationWorkflowRepository = configurationWorkflowRepository;
            _membershipRepository = membershipRepository;
            _configurationFeeRepository = configurationFeeRepository;
            _accountTransactionRepository = accountTransactionRepository;
            _institutionRepository = institutionRepository;

            AuditedDate = DateTime.Now;
            auditTracker = new AuditingTracker();
            _uow = uow;


        }

        public ResponseBase UploadBatch(UploadBatchRequest request)
        {
            bool OldSupporedVersion = false;

            ResponseBase response = new ResponseBase();
            request.CurrentDesktopVersion = Constants.CurrentBatchDesktopVersion;
            request.LeastSupportedDesktopVersion = Constants.LeastSupportedBatchDesktopVersion;

            if (request.CurrentDesktopVersion == null)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Error, Message = "Current Desktop Version configuration not set" }; //**Error not to be shown to client
                return response;
            }

            if (request.LeastSupportedDesktopVersion == null)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Error, Message = "Least Supported Batch Desktop Version not set" }; //**Error not to be shown to client
                return response;
            }

            //Extract the batch upload file from the batch
            if (!(request.SecurityUser.IsInAnyRoles("Client Officer", "FS Officer")))
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }

            if (request.SecurityUser.IsOwnerUser)
            {
                {
                    response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                    return response;
                }
            }

            if (request.File == null)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Error, Message = "No file attachment found!" };
                return response;
            }

            //FSBatchModel fsBatchModel = null;
            CRL.Service.BusinessServices.FSBatch batchModel = null;
            try
            {
                batchModel = SerializerHelper.DeserializeBatch<CRL.Service.BusinessServices.FSBatch>(request.AttachedFile);
            }
            catch (Exception ex)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { Message = "Invalid file", MessageType = Infrastructure.Messaging.MessageType.Error };
                return response;

            }

            //DesktopVersion compatibility test

            //if (fsBatchModel.ApplicationVersionNumber < request.LeastSupportedDesktopVersion)
            //{
            //    response.MessageInfo = new Infrastructure.Messaging.MessageInfo { Message = "The Offline Batch Application used to generate this file is old.  Please upload a batch file generated with the current version of the Offline Batch Application", MessageType = Infrastructure.Messaging.MessageType.Error };
            //    return response;

            //}
            //else if (fsBatchModel.ApplicationVersionNumber < request.CurrentDesktopVersion)
            //{
            //    OldSupporedVersion = true;
            //}
            //else if (fsBatchModel.ApplicationVersionNumber > request.CurrentDesktopVersion)
            //    throw new Exception("Offline version number is later than the application version");


            //Create a new bacth
            CRL.Model.FS.FSBatch fsbatch = new CRL.Model.FS.FSBatch()
            {
                Name = batchModel.BatchName,
                IsActive = true,
                IsDeleted = false,
                CreatedBy = request.SecurityUser.Id,
                IsSettled = false,
                NumberOfFS = Convert.ToInt32(batchModel.NumberOFFS)
            };

            _fsBatchRepository.Add(fsbatch);
            auditTracker.Created.Add(fsbatch);



            //Get all the fs and remove the from them and save
            foreach (var fsview in batchModel.FinancingStatements)
            {
                FileUpload fileUpload = null;
                //Convert to the object form of fs
                //if (fsview.AttachedFile != null)
                //{
                //    //Create a new fileupload and add it
                //    fileUpload = new FileUpload()
                //    {
                //        AttachedFile = fsview.AttachedFile,
                //        AttachedFileName = fsview.AttachedFileName,
                //        AttachedFileSize = fsview.AttachedFileSize,
                //        AttachedFileType = fsview.AttachedFileType,
                //        CreatedBy = request.SecurityUser.Id,
                //        IsActive = true,
                //        IsDeleted = false

                //    };



                //    fsview.AttachedFile = null;
                //    auditTracker.Created.Add(fileUpload);
                //}
                
                foreach (var ind in fsview.ParticipantsView)
                {
                    if (ind is IndividualDebtorView)
                    {
                        ((IndividualDebtorView)ind).Identification.FirstName = ((IndividualDebtorView)ind).FirstName;
                        ((IndividualDebtorView)ind).Identification.MiddleName = ((IndividualDebtorView)ind).MiddleName;
                        ((IndividualDebtorView)ind).Identification.Surname = ((IndividualDebtorView)ind).Surname;
                    }
                }

                //var uniqueNumber = Util.GetNewValidationCode();
                //fsview.RegistrationNo = uniqueNumber;
                byte[] fs = SerializerHelper.Serialize<FSView>(fsview);

                FSBatchDetail fsBatchDetail = new FSBatchDetail() { FinancialStatementAttachment = fileUpload, FSBatch = fsbatch, FSView = fs, IsActive = true, IsDeleted = false, UniqueNo = fsview.RegistrationNo };

                _fsBatchDetailRepository.Add(fsBatchDetail);


            }

            AuditAction = Model.FS.Enums.AuditAction.UploadFSBatch;
            AuditMessage = "Batch Id:" + fsbatch.Id.ToString();
            Audit audit = new Audit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction);
            _auditRepository.Add(audit);
            auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);

            try
            {
                _uow.Commit();
            }
            catch (Exception ex)
            {
                //WFor exceptions that we can handle we will have a set of handled exceptions enumeration and 
                //pass them back to the response call
                throw ex;
            }


            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            response.MessageInfo.Message = "The batch file (" + fsbatch.Name + ") was successfully uploaded.  " + fsbatch.FSBatchDetail.Count().ToString() + " financing statement(s) were contained in the batch.";
            if (OldSupporedVersion)
                response.MessageInfo.Message += "Please note that the batch file was generated with an Offline Batch Application  Although this file is supported, it is recommended to download the current version of the Offline Batch Application.";


            return response;

        }
        public ViewBatchesResponse ViewBatches(ViewBatchRequest request)
        {

            ViewBatchesResponse response = new ViewBatchesResponse();
            //Let's get the query from the response
            if (request.PageIndex > 0)
            {
                var myquery = FSBatchQueryGenerator.ViewFSBatchesQuery(request, _fsBatchRepository, true);
                int TotalRecords = myquery.Count();
                response.NumRecords = TotalRecords;
            }


            var myquery2 = FSBatchQueryGenerator.ViewFSBatchesQuery(request, _fsBatchRepository, false);

            response.FSBatches = myquery2.ToList();
            return response;
        }

        public ViewBatchedFSListResponse ViewBatchedFSList(GetFSFromBatchRequest request)
        {

            ViewBatchedFSListResponse response = new ViewBatchedFSListResponse();
            //Let's get the query from the response
            if (request.PageIndex > 0)
            {
                var myquery = FSBatchQueryGenerator.CreateQueryForFindFSBatchDetail(request, _fsBatchRepository, true);
                int TotalRecords = myquery.Count();
                response.NumRecords = TotalRecords;
            }


            var myquery2 = FSBatchQueryGenerator.CreateQueryForFindFSBatchDetail(request, _fsBatchRepository, false);
            response.FSGridView = new List<FSGridView>();

            foreach (var batchDetail in myquery2.ToList())
            {
                FSView fs = SerializerHelper.DeSerilizeFSView<FSView>(batchDetail.FSView);
                response.FSGridView.Add(new FSGridView
                {
                    Id = batchDetail.Id,
                    //CollateralTypeName = _collateralCategoryRepository.FindBy(fs.CollateralTypeId).CollateralCategoryName,
                    FinancialStatementLoanTypeName = fs.FinancialStatementLoanTypeId != null ? _financialStatementCategoryRepository.FindBy((FinancialStatementLoanCategory)fs.FinancialStatementLoanTypeId).FinancialStatementCategoryName : "",
                    MaximumAmountSecured = Convert.ToDecimal(fs.MaximumAmountSecured),
                    //FinancialStatementTransactionTypeName = _financialStatementTransactionCategoryRepository.FindBy(fs.FinancialStatementTransactionTypeId).FinancialStatementTransactionCategoryName,
                    Uploaded = batchDetail.Uploaded,
                    MaximumAmountSecuredCurrencyName = fs.MaximumAmountSecuredCurrencyId != null ? _currencyRepository.FindBy((int)fs.MaximumAmountSecuredCurrencyId).CurrencyName : "",
                    ExpiryDate = fs.ExpiryDate,
                    HasFileAttachment = batchDetail.FinancialStatementAttachmentId == null ? false : true
                });

            }

            return response;
        }

        public ViewBatchResponse GetBatchDetail(RequestBase request)
        {
            ViewBatchResponse response = new ViewBatchResponse();
            response.FSBatch = _fsBatchRepository.GetDbSet().Where(s => s.Id == request.Id && s.CreatedByUser.MembershipId == request.SecurityUser.MembershipId).Select(d => new FSBatchView
            {
                Id = d.Id,
                IsSettled = d.IsSettled,
                Name = d.Name,
                NumberOfFS = d.FSBatchDetail.Count(),
                NumberOfUploadedFS = d.FSBatchDetail.Where(e => e.Uploaded == true).Count(),
                RemainingFSToBeUploaded = d.FSBatchDetail.Where(f => f.Uploaded == false).Count(),
                CreatedOn = d.CreatedOn,
                CreatedBy = d.CreatedBy
            }
        ).FirstOrDefault();

            if (response.FSBatch.CreatedBy != request.SecurityUser.Id && !request.SecurityUser.IsAdministrator())
            {
                response.CanEdit = false;
            }
            else
                response.CanEdit = true;
            ConfigurationWorkflow configWorkflow = _configurationWorkflowRepository.GetDbSet().Where(s => s.MembershipId == request.SecurityUser.MembershipId).SingleOrDefault();
            if (configWorkflow == null || configWorkflow.UseGlobalSettings)
            {
                configWorkflow = null;
                configWorkflow = _configurationWorkflowRepository.GetDbSet().Where(s => s.MembershipId == null).SingleOrDefault();
            }
            if (request.SecurityUser.InstitutionId != null)
            {

                if (configWorkflow != null && configWorkflow.CreateNewFS)
                {
                    response.CreateModeIsWorkflowOn = true;
                }
            }
            else
            {
                if (!configWorkflow.SkipWorkflowForIndividuals)
                    response.CreateModeIsWorkflowOn = true;


            }

            if (response.CreateModeIsWorkflowOn)  //for create mode only
            {
                AssignNewTaskView assignNewTaskView = new AssignNewTaskView();

                //Since this workflow is new we need to now what the first would be

                //This task starts a workflow so please load the workflow
                WFWorkflow wf = _workflowRepository.GetWFWorkflowById(4);
                List<WFPlace> _places = WorkflowManager.GetPlacesFromWorkflow(wf);
                if (_places.Count > 0)
                {

                    //We need to get users       //If no user is found then warn.  When we create users we will need to rassign roles  
                    UserWorkflowAssignmentService uaRps = new UserWorkflowAssignmentService(_userRepository, _places, request.SecurityUser, WFTaskType.CreateRegistration);
                    List<User> AssignedUsers = uaRps.Users;
                    if (uaRps.Users.Select(s => s.Id).ToArray().Contains(request.SecurityUser.Id) && configWorkflow.SkipWhenSubmitterIsAuthroizer)
                    {
                        response.CreateModeIsWorkflowOn = false;
                    }
                    else
                    {
                        response.AssignNewTaskView = new AssignNewTaskView();
                        response.AssignNewTaskView.AssignedToUser = AssignedUsers.ConvertToUserViews();
                    }
                }




            }

            return response;

        }

        public GetDataForFSByEditModeResponse GetFSFromBatchForView(GetFSFromBatchRequest request)
        {
            Institution institution = null;
            GetDataForFSByEditModeResponse response = new GetDataForFSByEditModeResponse();

            //First of all let's get the batch details and batch id from the 


            FSBatchDetail fsBatchDetail = _fsBatchDetailRepository.GetDbSet().Where(s => s.Id == request.FSBatchId && s.FSBatch.CreatedByUser.MembershipId == request.SecurityUser.MembershipId).FirstOrDefault();
            if (fsBatchDetail != null)
                response.FSView = SerializerHelper.DeSerilizeFSView<FSView>(fsBatchDetail.FSView);

            else
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Error, Message = "Cannot find the submitted batch" };
                return response;
            }

            if (fsBatchDetail.FSBatch.CreatedBy != request.SecurityUser.Id)
                response.IsNotOwnerOfBatch = true;

            LookUpForFS fsLookUps = this.GetLookUpsForFs();
            //response.FSView.FinancialStatementTransactionTypeName = fsLookUps.FinancialStatementTransactionTypes.Where(s => s.LkValue == (int)response.FSView.FinancialStatementTransactionTypeId).FirstOrDefault().LkName;
            response.FSView.FinancialStatementLoanTypeName = response.FSView.FinancialStatementLoanTypeId != null ? fsLookUps.FinancialStatementLoanType.Where(s => s.LkValue == (int)response.FSView.FinancialStatementLoanTypeId).FirstOrDefault().LkName : "";
            //response.FSView.CollateralTypeName = fsLookUps.CollateralTypes.Where(s => s.LkValue == (int)response.FSView.CollateralTypeId).FirstOrDefault().LkName;
            response.FSView.CurrencyName = fsLookUps.Currencies.Where(s => s.LkValue == (int)response.FSView.MaximumAmountSecuredCurrencyId).FirstOrDefault().LkName;
            //response.FSView .a = fsBatchDetail.FinancialStatementAttachmentId != null;

            foreach (var item in response.FSView.CollateralsView)
            {
                item.CollateralSubTypeName = fsLookUps.CollateralSubTypes.Where(s => s.LkValue == item.CollateralSubTypeId).FirstOrDefault().LkName;
            }
            foreach (var item in response.FSView.ParticipantsView)
            {
                item.CountryView = fsLookUps.Countries.Where(s => s.LkValue == item.CountryId).FirstOrDefault().LkName;

                item.County = fsLookUps.Countys.Where(s => s.LkValue == item.CountyId).FirstOrDefault().LkName;
                //item.Nationality = fsLookUps.Nationalities.Where(s => s.LkValue == item.NationalityId).FirstOrDefault().LkName;
                response.SectorsOfOperation = fsLookUps.SectorsOfOperation;

                if (item is IndividualDebtorView)
                {
                    IndividualDebtorView debtor = (IndividualDebtorView)item;

                    debtor.LGA = fsLookUps.LGAs.Where(s => s.LkValue == (int)debtor.LGAId).FirstOrDefault().LkName;
                    debtor.NationalityView = fsLookUps.Nationalities.Where(s => s.LkValue == (int)debtor.NationalityId).FirstOrDefault().LkName;
                    //debtor.Identification. = fsLookUps.IdentificationCardTypes.Where(s => s.LkValue == debtor.Identification.).FirstOrDefault().LkName;
                    if (debtor.OtherIdentifications != null && debtor.OtherIdentifications.Count() > 0)
                    {
                        foreach (var sub_item in debtor.OtherIdentifications)
                        {
                            // sub_item.PersonIdentificationTypename = fsLookUps.IdentificationCardTypes.Where(s => s.LkValue == sub_item.PersonIdentificationTypeId).FirstOrDefault().LkName;
                        }
                    }
                }

                if (item is IndividualSPView)
                {
                    IndividualSPView securedparty = (IndividualSPView)item;
                    //securedparty.Identification.PersonIdentificationTypename = fsLookUps.IdentificationCardTypes.Where(s => s.LkValue == securedparty.Identification.PersonIdentificationTypeId).FirstOrDefault().LkName;
                }

                if (item is InstitutionDebtorView)
                {
                    InstitutionDebtorView debtor = (InstitutionDebtorView)item;
                    debtor.DebtorTypeName = fsLookUps.DebtorTypes.Where(s => s.LkValue == (int)debtor.DebtorTypeId).FirstOrDefault().LkName;
                    debtor.BusinessTinFullName = fsLookUps.RegistrationNoPrefix.Where(s => s.LkValue == (int)debtor.BusinessTinPrefix).FirstOrDefault().LkName + debtor.CompanyNo;
                    debtor.LGA = fsLookUps.LGAs.Where(s => s.LkValue == (int)debtor.LGAId).FirstOrDefault().LkName;
                }

                if (item is InstitutionSPView)
                {
                    InstitutionSPView securedParty = (InstitutionSPView)item;
                    securedParty.SecuringPartyIndustryTypename = fsLookUps.SecuringPartyIndustryTypes.Where(s => s.LkValue == (int)securedParty.SecuringPartyIndustryTypeId).FirstOrDefault().LkName;

                }



            }
            institution = _institutionRepository.GetDbSet().Where(m => m.MembershipId == request.SecurityUser.MembershipId).SingleOrDefault();
            ParticipantView pv = new ParticipantView();
            //**This is bad and should be done in the query rather

            if (institution != null)
            {
                InstitutionSPView securedparty = new InstitutionSPView();
                securedparty.Address = institution.Address.Address;
                securedparty.Address2 = institution.Address.Address2;
                securedparty.CompanyNo = institution.CompanyNo;
                securedparty.Name = institution.Name;
                securedparty.ParticipantTypeId = Model.FS.Enums.ParticipantCategory.Insititution;
                securedparty.ParticipationTypeId = Model.FS.Enums.ParticipationCategory.AsSecuredParty;
                securedparty.SecuringPartyIndustryTypeId = institution.SecuringPartyTypeId;
                securedparty.City = institution.Address.City;
                securedparty.LGAId = institution.LGAId;
                securedparty.CountryId = institution.CountryId;
                securedparty.CountyId = institution.CountyId;
                securedparty.RegistrantInstitutionId = institution.Id;
                securedparty.RegistrantMembershipId = institution.MembershipId;
                response.FSView.ParticipantsView.Add(securedparty);

                // Map lookups to view
                securedparty.CountryView = institution.Country.Name;
                securedparty.County = institution.County.Name;
                securedparty.LGA = institution.LGA.Name;
                securedparty.SecuringPartyIndustryTypename = institution.SecuringPartyType.SecuringPartyIndustryCategoryName;
            }




            return response;
        }

        public GetFileAttachmentResponse LoadBatchedFSAttachment(GetFSFromBatchRequest request)
        {
            GetFileAttachmentResponse response = new GetFileAttachmentResponse();

            FileUpload fsAttachment = null;

            fsAttachment = _fsBatchDetailRepository.FindBy((int)request.FSBatchId).FinancialStatementAttachment;


            if (fsAttachment.CreatedByUser.MembershipId == request.SecurityUser.MembershipId)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }

            response.AttachedFile = fsAttachment.AttachedFile;
            response.AttachedFileName = fsAttachment.AttachedFileName;
            response.AttachedFileSize = fsAttachment.AttachedFileSize;
            response.AttachedFileType = fsAttachment.AttachedFileType;


            return response;



        }

        public int[] GetBatchDetailsIdFromBatchId(RequestBase request)
        {
            return _fsBatchDetailRepository.GetDbSet().Where(s => s.FSBatchId == request.Id && s.Uploaded == false).Select(s => s.Id).ToArray();

        }

        //public ResponseBase SubmitBatch(SubmitBatchRequest request)
        //{
        //    //Extract all the fsviews we need
        //    ResponseBase response = new ResponseBase();

        //    if (!(request.SecurityUser.IsInAnyRoles("Client Officer", "FS Officer")))
        //    {
        //        response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
        //        return response;
        //    }


        //    List<FSBatchDetail> fsBatchDetail = null;

        //    if (!request.SubmitOnlySelected)
        //    {
        //        fsBatchDetail = _fsBatchDetailRepository.GetDbSet().Where(s => s.FSBatchId == request.Id && s.Uploaded == false).ToList();
        //    }
        //    else
        //    {
        //        fsBatchDetail = _fsBatchDetailRepository.GetDbSet().Where(s => request.SubmittedFS.Contains(s.Id) && s.Uploaded == false).ToList();
        //    }

        //    CreateFSFromBatchRequest submitRequest= null;
        //      if (request.InWorkFlowMode ) //We are creating
        //    {
        //          submitRequest = new SubmitFSFromBatchRequest ();
        //        submitRequest.configurationFee = _configurationFeeRepository.GetDbSetComplete().Where(s => s.ServiceFeeType.Any(t => t.Id == ServiceFees.NewFinancingStatement) && s.IsActive == true && s.IsDeleted == false).FirstOrDefault();
        //    }
        //      else

        //      {
        //          submitRequest = new CreateFSFromBatchRequest ();
        //      }
        //    //Load sector of operations
        //    var query_sectorOfOperations = _sectorOfOperationCategoryRepository.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false);
        //    submitRequest.SectorOfOperationList = _sectorOfOperationCategoryRepository.FindBy(query_sectorOfOperations);


        //    submitRequest.Emails = new List<Email>();
        //    //Let's get those batch details with attachment and then setup so that 



        //    foreach (var item in fsBatchDetail)
        //    {
        //        if (item.FSBatch.CreatedBy != request .SecurityUser .Id )
        //        {
        //            response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
        //            return response;
        //        }

        //        FSView fsview = SerializerHelper.DeSerilizeFSView<FSView>(item.FSView);
        //        if (item.FinancialStatementAttachmentId != null)
        //            fsview.FinancialStatementAttachmentId = item.FinancialStatementAttachmentId; 


        //        submitRequest.FSView = fsview;
        //        submitRequest.SecurityUser = request.SecurityUser;

        //        if (request.InWorkFlowMode)
        //        {


        //            //Let's call the submit
        //            SubmitFS((SubmitFSFromBatchRequest)submitRequest);
        //        }
        //        else
        //        {
        //            response = CreateFS(submitRequest);
        //            if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Error)
        //                return response;
        //        }

        //        //Add all generated mails
        //        foreach (var mails in submitRequest.Emails)
        //        {

        //            _emailRepository.Add(mails);
        //            auditTracker.Created.Add(mails);


        //        }

        //        item.Uploaded = true;



        //    }
        //    AuditAction = Model.FS.Enums.AuditAction.SubmitOrCreatedFSBatch;
        //    AuditMessage = "Submiited transactions in the batch " + request.Id;


        //    Audit audit = new Audit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction);
        //    _auditRepository.Add(audit);
        //    auditTracker.Created.Add(audit);
        //    _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);

        //    try
        //    {
        //        _uow.Commit();
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        // Update original values from the database 
        //        var entry = ex.Entries.Single();
        //        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
        //        response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.DatabaseConcurrencyConflict;
        //        return response;


        //    }
        //    catch
        //    {

        //        throw;
        //    }

        //    response.MessageInfo.Message = "You have successfully submitted the financing statement(s) in the batch!";
        //    response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;

        //    return response;

        //}
        public ResponseBase SubmitBatch(SubmitBatchRequest request)
        {
            //Extract all the fsviews we need
            ResponseBase response = new ResponseBase();

            if (!(request.SecurityUser.IsInAnyRoles("Client Officer", "FS Officer")))
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }


            FSBatchDetail fsBatchDetail = null;

            fsBatchDetail = _fsBatchDetailRepository.GetDbSet().Where(s => request.SubmittedFS.Contains(s.Id) && s.Uploaded == false).SingleOrDefault();

            if (_fsBatchDetailRepository.GetDbSet().Where(s => s.UniqueNo == fsBatchDetail.UniqueNo && s.Uploaded == true).Count() > 0)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.BusinessValidationError, Message = " A financing statement with the same reference number has already been uploaded!" };
                return response;
            }

            if (fsBatchDetail.FSBatch.CreatedBy != request.SecurityUser.Id && !request.SecurityUser.IsClientAdministrator())
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }

            CreateFSFromBatchRequest submitRequest = null;
            if (request.InWorkFlowMode) //We are creating
            {
                var lenderType = _institutionRepository.FindBy((int)request.SecurityUser.InstitutionId).SecuringPartyTypeId;
                submitRequest = new SubmitFSFromBatchRequest();
                submitRequest.configurationFee = _configurationFeeRepository.GetDbSetComplete(ServiceFees.NewFinancingStatement, (int)lenderType).FirstOrDefault();
            }
            else
            {
                submitRequest = new CreateFSFromBatchRequest();
            }
            //Load sector of operations
            var query_sectorOfOperations = _sectorOfOperationCategoryRepository.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false);
            submitRequest.SectorOfOperationList = _sectorOfOperationCategoryRepository.FindBy(query_sectorOfOperations);


            submitRequest.Emails = new List<Email>();
            //Let's get those batch details with attachment and then setup so that 




            if (fsBatchDetail.FSBatch.CreatedBy != request.SecurityUser.Id)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }

            FSView fsview = SerializerHelper.DeSerilizeFSView<FSView>(fsBatchDetail.FSView);
            //if (fsBatchDetail.FinancialStatementAttachmentId != null)
            //    fsview.FinancialStatementAttachmentId = fsBatchDetail.FinancialStatementAttachmentId;

            var institution = _institutionRepository.GetDbSet().Where(m => m.MembershipId == request.SecurityUser.MembershipId).SingleOrDefault();
            ParticipantView pv = new ParticipantView();
            //**This is bad and should be done in the query rather

            if (institution != null)
            {
                InstitutionSPView securedparty = new InstitutionSPView();
                securedparty.Address = institution.Address.Address;
                securedparty.Address2 = institution.Address.Address2;
                securedparty.CompanyNo = institution.CompanyNo;
                securedparty.Name = institution.Name;
                securedparty.ParticipantTypeId = Model.FS.Enums.ParticipantCategory.Insititution;
                securedparty.ParticipationTypeId = Model.FS.Enums.ParticipationCategory.AsSecuredParty;
                securedparty.SecuringPartyIndustryTypeId = institution.SecuringPartyTypeId;
                securedparty.City = institution.Address.City;
                securedparty.LGAId = institution.LGAId;
                securedparty.CountryId = institution.CountryId;
                securedparty.CountyId = institution.CountyId;
                securedparty.RegistrantInstitutionId = institution.Id;
                securedparty.RegistrantMembershipId = institution.MembershipId;
                fsview.ParticipantsView.Add(securedparty);
            }


            submitRequest.FSView = fsview;
            submitRequest.SecurityUser = request.SecurityUser;

            FSViewValidator fsviewValidatior = new FSViewValidator();
            var result = fsviewValidatior.Validate(submitRequest.FSView);
            var responseHandleValidationResultResponse = this.HandleValidationResult(result);
            if (responseHandleValidationResultResponse.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(responseHandleValidationResultResponse); return response; }


            if (request.InWorkFlowMode)
            {
                //Let's call the submit
                response = SubmitFS((SubmitFSFromBatchRequest)submitRequest);
                if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Error)
                    return response;
            }
            else
            {
                response = CreateFS(submitRequest);
                if (response.MessageInfo.MessageType == Infrastructure.Messaging.MessageType.Error)
                    return response;
            }

            //Add all generated mails
            foreach (var mails in submitRequest.Emails)
            {

                _emailRepository.Add(mails);
                auditTracker.Created.Add(mails);


            }

            fsBatchDetail.Uploaded = true;




            AuditAction = Model.FS.Enums.AuditAction.SubmitOrCreatedFSBatch;
            AuditMessage = "Submitted financing statement with id " + fsBatchDetail.Id + " in batch " + request.Id;


            Audit audit = new Audit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction);
            _auditRepository.Add(audit);
            auditTracker.Created.Add(audit);
            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);

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
            catch
            {

                throw;
            }

            response.MessageInfo.Message = "You have successfully submitted the financing statement(s) in the batch!";
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;

            return response;

        }

        private ResponseBase SubmitFS(SubmitFSFromBatchRequest request)
        {

            //Bind fsview to the fs
            ResponseBase response = new ResponseBase();
            PaymentServiceModel paymentServiceModel = new PaymentServiceModel(_configurationFeeRepository, _membershipRepository, _currencyRepository, _accountTransactionRepository, auditTracker, request.SecurityUser);
            Model.FS.FinancialStatement fs = request.FSView.ConvertToNewFS(request.SectorOfOperationList);
            fs.Collaterals = request.FSView.CollateralsView.ConvertToNewCollaterals();
            fs.Participants = request.FSView.ParticipantsView.ConvertToNewParticipants(request.SectorOfOperationList);
            fs.MembershipId = request.SecurityUser.MembershipId;

            _financialStatementRepository.Add(fs);
            auditTracker.Created.Add(fs);

            //PAYMENT PROCESSING
            var LenderTypeId = _institutionRepository.FindBy((int)request.SecurityUser.InstitutionId).SecuringPartyTypeId;
            paymentServiceModel.InitialisePayment(ServiceFees.NewFinancingStatement, (int)LenderTypeId, request.FSView.MaximumAmountSecuredCurrencyId);

            var Amount = Convert.ToDecimal(request.FSView.MaximumAmountSecured);
            var paymentValidationResponse = paymentServiceModel.ValidatePayment(Amount);
            if (paymentValidationResponse.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(paymentValidationResponse); return response; }


            if (paymentValidationResponse.isPayabale == true)
            {
                paymentServiceModel.ProcessNewFSPayment(fs);
            }


            //create the case to handle this
            fs.RegistrationNo = "";
            fs.RequestNo = "TRG" + SerialTracker.GetSerialValue(_serialTrackerRepository, SerialTrackerEnum.TempRegistration);
            fs.EffectiveDate = AuditedDate;
            fs.RegistrationDate = AuditedDate;
            //fs.FinancialStatementAttachmentId = request.FSView.FinancialStatementAttachmentId;



            //Setup collaterals
            short Indx = 0;
            foreach (Collateral _collateral in fs.Collaterals)
            {
                auditTracker.Created.Add(_collateral);
            }

            Indx = 0;
            //Audit participants and add particpant no
            foreach (Participant participant in fs.Participants)
            {
                auditTracker.Created.Add(participant);
                if (participant is IndividualParticipant)
                {
                    foreach (var identity in ((IndividualParticipant)participant).OtherPersonIdentifications)
                    {
                        auditTracker.Created.Add(identity);
                    }
                }
            }




            //Setup the case

            WorkflowServiceModel wfServiceModel = new WorkflowServiceModel(_emailTemplateRepository, _workflowRepository, _emailRepository, _emailUserAssignmentRepository, _userRepository, _caseRepository, auditTracker, request.SecurityUser);
            WFCaseFS _case = (WFCaseFS)wfServiceModel.InitialiseCase(4, WorkflowRequestType.FinancialStatement, request.Comment);
            wfServiceModel.ProcessCase(WFTaskType.CreateRegistration, null);
            //  WorkflowService.SetupNewCase(_case, 4, fs.RequestNo, request.Comment, request.SecurityUser,
            //     _workflowRepository, _userRepository, WFTaskType.CreateRegistration, _emailTemplateRepository, request.Emails, auditTracker);
            _case.FinancialStatement = fs;
            _caseRepository.Add(_case);
            auditTracker.Created.Add(_case);


            return response;

        }

        private ResponseBase CreateFS(CreateFSFromBatchRequest request)
        {
            ResponseBase response = new ResponseBase();
            CreateFSServiceModel fsServiceModel = new CreateFSServiceModel(_serialTrackerRepository, _financialStatementRepository, _emailRepository, _emailUserAssignmentRepository, _financialStatementTransactionCategoryRepository, _collateralCategoryRepository, _sectorOfOperationCategoryRepository, _fileUploadRepository, _financialStatementSnapshotRepository, auditTracker, request.SecurityUser, request.UniqueGuidForm);
            FSServiceModel.ReportGeneratedDate = AuditedDate;
            PaymentServiceModel paymentServiceModel = new PaymentServiceModel(_configurationFeeRepository, _membershipRepository, _currencyRepository, _accountTransactionRepository, auditTracker, request.SecurityUser);
            //Bind fsview to the fs
            Model.FS.FinancialStatement fs = request.FSView.ConvertToNewFS(request.SectorOfOperationList);
            fs.Collaterals = request.FSView.CollateralsView.ConvertToNewCollaterals();
            fs.Participants = request.FSView.ParticipantsView.ConvertToNewParticipants(request.SectorOfOperationList);
            fs.MembershipId = request.SecurityUser.MembershipId;
            _financialStatementRepository.Add(fs);
            auditTracker.Created.Add(fs);

            fs.HandledById = request.SecurityUser.Id;
            fs.isApprovedOrDenied = 1;
            fs.RegistrationNo = "REG" + SerialTracker.GetSerialValue(_serialTrackerRepository, SerialTrackerEnum.Registration);
            fs.RegistrationDate = AuditedDate;
            fs.EffectiveDate = fs.RegistrationDate;

            string UniqueCode = Util.GetNewValidationCode() + request.SecurityUser.Id.ToString("00000");

            //Setup collaterals
            short Indx = 0;
            foreach (Collateral _collateral in fs.Collaterals)
            {
                Indx++;
                _collateral.CollateralNo = UniqueCode + Indx.ToString("000");
                auditTracker.Created.Add(_collateral);
            }

            Indx = 0;
            //Audit participants and add particpant no
            foreach (Participant participant in fs.Participants)
            {
                Indx++;
                participant.ParticipantNo = UniqueCode + Indx.ToString("000");
                auditTracker.Created.Add(participant);

                short OtherIndex = 0;
                if (participant is IndividualParticipant)
                {
                    foreach (var identity in ((IndividualParticipant)participant).OtherPersonIdentifications)
                    {
                        OtherIndex++;
                        identity.UniqueCode = UniqueCode + OtherIndex.ToString("000");
                        auditTracker.Created.Add(identity);
                    }
                }
            }

            //fs.FinancialStatementAttachmentId = request.FSView.FinancialStatementAttachmentId;


            //PAYMENT PROCESSING
            var LenderTypeId = _institutionRepository.FindBy((int)request.SecurityUser.InstitutionId).SecuringPartyTypeId;
            paymentServiceModel.InitialisePayment(ServiceFees.NewFinancingStatement, (int)LenderTypeId, request.FSView.MaximumAmountSecuredCurrencyId);

            var Amount = Convert.ToDecimal(request.FSView.MaximumAmountSecured);
            var paymentValidationResponse = paymentServiceModel.ValidatePayment(Amount);
            if (paymentValidationResponse.GetMessageType != MessageType.Success) { response.SetResponseToResponseBase(paymentValidationResponse); return response; }


            if (paymentValidationResponse.isPayabale == true)
            {
                paymentServiceModel.ProcessNewFSPayment(fs);
            }
            //Approval email
            Email ApproveEmail = new Email();
            ApproveEmail.IsSent = false;
            ApproveEmail.NumRetries = 0;
            EmailTemplate ApproveEmailTemplate;
            ApproveEmailTemplate = _emailTemplateRepository.GetDbSet().Where(s => s.EmailAction == "CreateFinancingStatement").SingleOrDefault(); //Load from above
            ApproveEmail.EmailTo = request.SecurityUser.Email;  //Workflowemails not added but will be part of this list

            ////Verification statement
            //List<FSReportView> FS = new List<FSReportView>();
            //LookUpForFS fsLookUps = this.GetLookUpsForFs();
            //FS.Add(fs.ConvertToFSReportView(fsLookUps));
            //List<SecuredPartyReportView> Lenders = new List<SecuredPartyReportView>();
            //Lenders.AddRange(fs.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsSecuredParty).ConvertToSecuredPartyReportView(fsLookUps));
            //List<DebtorReportView> Borrowers = new List<DebtorReportView>();
            //Borrowers.AddRange(fs.Participants.Where(s => s.ParticipationTypeId == ParticipationCategory.AsBorrower).ConvertToDebtorReportView(fsLookUps));
            //List<CollateralReportView> Collaterals = new List<CollateralReportView>();
            //Collaterals.AddRange(fs.Collaterals.ConvertToCollateralReportViews(fsLookUps));
            //List<OtherIdentificationReportView> OtherIdentifications = new List<OtherIdentificationReportView>();

            //OtherIdentifications.AddRange(fs.Participants.OfType<IndividualParticipant>().Where(s => s.OtherPersonIdentifications.Count > 0).GetOtherIdentificationReportView(fsLookUps));

            MFSVerificationReportBuilder verificationBuilder = new MFSVerificationReportBuilder();
            List<UserEmailView> UserEmails = new List<UserEmailView>();
            UserEmails.Add(new UserEmailView { Id = request.SecurityUser.Id, Email = request.SecurityUser.Email, RecepientType = MailReceipientType.To });
            fsServiceModel.submittedFS = fs;
            fsServiceModel.GenerateVerificationEmail(ApproveEmailTemplate, verificationBuilder, this.GetLookUpsForFs(), UserEmails, "", AuditedDate);

            //Byte[] mybytes = report.GenerateReport();

            //FileUpload _fileUpload = new FileUpload();
            //_fileUpload.AttachedFile = mybytes;
            //_fileUpload.AttachedFileName = fs.RegistrationNo + ".pdf";
            //_fileUpload.AttachedFileSize = mybytes.Length.ToString();
            //_fileUpload.AttachedFileType = "application/pdf";
            //fs.VerificationAttachment = _fileUpload;
            //_emailRepository.Add(ApproveEmail);
            //auditTracker.Created.Add(_fileUpload);
            //auditTracker.Created.Add(ApproveEmail);

            //EmailTemplateGenerator.CreateFinancingStatementMail(ApproveEmail, fs, ApproveEmailTemplate, _fileUpload);

            //request.Emails.Add(ApproveEmail);

            return response;
        }

        private LookUpForFS GetLookUpsForFs()
        {
            LookUpForFS response = new LookUpForFS();
            response.IdentificationCardTypes = LookUpServiceHelper.IdentificationCardTypes(_personIdentificationCategoryRepository);
            response.DebtorTypes = LookUpServiceHelper.DebtorTypes(_companyCategoryRepository); //**caching may be necessary
            response.Countries = LookUpServiceHelper.Countries(_countryRepository); //**caching may be necessary
            response.Countys = LookUpServiceHelper.Countys(_countyRepository);
            response.LGAs = LookUpServiceHelper.LGAsLK(_lgaRepository);
            response.RegistrationNoPrefix = LookUpServiceHelper.BusinessRegPrefixes(_businessRegPrefix);
            response.Nationalities = LookUpServiceHelper.Nationalities(_nationalityRepository); //**caching maybe necessary           
            response.SecuringPartyIndustryTypes = LookUpServiceHelper.SecuringPartyTypes(_securingPartyIndustryCategoryRepository);
            response.SectorsOfOperation = LookUpServiceHelper.SectorsOfOperation(_sectorOfOperationCategoryRepository); //**caching maybe necessary            
            response.CollateralTypes = LookUpServiceHelper.CollateralTypes(_collateralCategoryRepository);
            response.CollateralSubTypes = LookUpServiceHelper.CollateralSubTypes(_collateralSubTypeCategoryRepository); //**caching maybe necessary            
            response.Currencies = LookUpServiceHelper.Currencies(_currencyRepository); //**caching may be necessary                            
            response.FinancialStatementLoanType = LookUpServiceHelper.FinancialStatementLoanType(_financialStatementCategoryRepository); //**caching maybe necessary           
            response.FinancialStatementTransactionTypes = LookUpServiceHelper.FinancialStatementTransactionTypes(_financialStatementTransactionCategoryRepository);
            return response;
        }

        public ResponseBase DeleteBatch(RequestBase request)
        {
            ResponseBase response = new ResponseBase();


            CRL.Model.FS.FSBatch batch = _fsBatchRepository.FindBy(request.Id);
            if (batch.CreatedBy != request.SecurityUser.Id && !request.SecurityUser.IsAdministrator())
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }
            batch.IsDeleted = true;

            //Load all the unloaded batch details
            List<FSBatchDetail> batchDetail = batch.FSBatchDetail.Where(s => s.Uploaded == false && s.FinancialStatementAttachmentId != null).ToList();

            foreach (var item in batchDetail)
            {
                FileUpload f = item.FinancialStatementAttachment;
                f.IsDeleted = true;
                auditTracker.Updated.Add(f);

            }


            AuditMessage = "Batch Id:" + batch.Id.ToString();

            Audit audit = new Audit(AuditMessage, request.RequestUrl, request.UserIP, AuditAction.DeleteBatch);

            _auditRepository.Add(audit);
            auditTracker.Updated.Add(batch);
            auditTracker.Created.Add(audit);

            _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
            _uow.Commit();
            response.MessageInfo.Message = "Batch Deletion Successful Successful";
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            return response;
        }

    }
}