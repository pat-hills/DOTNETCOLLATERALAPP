using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model.Configuration;
using CRL.Model.Configuration.IRepository;

using CRL.Service.Interfaces;
using CRL.Service.Messaging.Configuration.Request;
using CRL.Service.Messaging.Configuration.Response;
using CRL.Service.Views.Configuration;
using CRL.Service.Mappings.Configuration;
using CRL.Infrastructure.Domain;
using CRL.Service.BusinessServices;
using CRL.Model;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelService;

using CRL.Service.QueryGenerator;

using CRL.Model.ModelViews;
using CRL.Model.Memberships.IRepository;
using CRL.Model.Memberships;
using CRL.Model.ModelCaching;
using CRL.Model.FS.IRepository;
using CRL.Infrastructure.Helpers;
using CRL.Model.Payments;
using CRL.Infrastructure.Configuration;

namespace CRL.Service.Implementations
{
    public class ConfigurationService : IConfigurationService
    {

        private readonly IConfigurationWorkflowRepository _configurationWorkflowRepository;
        private readonly IConfigurationFeeRepository _configurationFeeRepository;
        private readonly ILKServiceFeeCategoriesRepository _serviceFeeCategoryRepository;
        private readonly IBankVerificationCodeRepository _bankVerificationCodeRepository;
        private readonly IConfigurationTransactionPaymentRespository _configurationTransactionPaymentRespository;
        private readonly ILKSecuringPartyIndustryCategoryRepository _securingPartyCategoryRepository;

        private readonly IUnitOfWork _uow;
        public AuditingTracker auditTracker { get; set; }
        public DateTime AuditedDate { get; set; }
        public ConfigurationService(
            IConfigurationWorkflowRepository configurationWorkflowRepository,
             IConfigurationFeeRepository configurationFeeRepository,
            ILKServiceFeeCategoriesRepository serviceFeeCategoryRepository,
            IBankVerificationCodeRepository bankVerificationCodeRepository,
            IConfigurationTransactionPaymentRespository configurationTransactionPaymentRespository,
            ILKSecuringPartyIndustryCategoryRepository securingPartyCategoryRepository,
           IUnitOfWork uow)
        {

            _configurationWorkflowRepository = configurationWorkflowRepository;
            _configurationFeeRepository = configurationFeeRepository;
            _serviceFeeCategoryRepository = serviceFeeCategoryRepository;
            _bankVerificationCodeRepository = bankVerificationCodeRepository;
            _configurationTransactionPaymentRespository = configurationTransactionPaymentRespository;
            _securingPartyCategoryRepository = securingPartyCategoryRepository;
            _uow = uow;
            auditTracker = new AuditingTracker();
            AuditedDate = DateTime.Now;
        }


        public GetFeeResponse GetFeeForPublicSearch(RequestBase request)
        {
            GetFeeResponse response = new GetFeeResponse();
            if (request.SecurityUser == null)
            {

                response.fee  = _configurationFeeRepository.GetDbSetComplete( ServiceFees.PublicSearch , 360).SingleOrDefault();         
             
                    
            }

            return response;
        }

        public GetFeeConfigurationResponse GetFeeConfiguration(RequestBase request)
        {
            //GetFeeConfigurationResponse response = new GetFeeConfigurationResponse();
            //response.ConfigurationFee = _configurationFeeRepository.GetDbSetComplete().Where(s => s.IsDeleted  == false).ToList();
            GetFeeConfigurationResponse response = new GetFeeConfigurationResponse();

            if (!request.SecurityUser.IsInAnyRoles("Administrator (Owner)"))
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }

            ICollection<ConfigurationFee> configurationFees = new List<ConfigurationFee>();
          //  configurationFees = _configurationFeeRepository.GetDbSetComplete().ToList();

            //foreach (var config in response.ConfigurationFee)
            //{
            //    if (config is PerTransactionConfigurationFee)
            //    {
            //        ((PerTransactionConfigurationFee)config).FeeLoanSetups.Count();
            //    }
            //}
            var lenderTypes = LookUpServiceModel.SecuringPartyTypes(_securingPartyCategoryRepository);
            response.ConfigurationFeesSetupView.ConfiguationFeesView = configurationFees.ConvertToConfigurationFeesView();
            var transactionFeesConfiguration = _configurationTransactionPaymentRespository.GetDbSetComplete().Where(s=>s.IsActive == true && s.IsDeleted == false);

            var publicUserLookup = new LookUpView() { LkValue = Constants.DefaultLenderTypeIdForPublicUsers, LkName = "Public Users" };
            lenderTypes.Add(publicUserLookup);

            var transactionFees = transactionFeesConfiguration.Select(s => new ConfigurationTransactionFeesView()
            {
                ServiceTypeId = s.ServiceTypeId,
                ServiceType = s.ServiceType,
                LenderTypeId = s.LenderType,
                Fee = s.Fee,
                Id = s.Id
            }).ToList();

            response.LenderTransactionFeeConfigurationViews = new List<LenderTransactionFeeConfigurationView>();

           

            foreach (var lender in lenderTypes)
            {
                if (lender.LkValue == Constants.DefaultLenderTypeIdForPublicUsers)
                {

                    response.LenderTransactionFeeConfigurationViews.Add(new LenderTransactionFeeConfigurationView()
                    {
                        LenderTypeId = lender.LkValue,
                        LenderType = lender.LkName,
                        ConfigurationFeeViews = transactionFees.Where(s => s.LenderTypeId == Constants.DefaultLenderTypeIdForPublicUsers).ToList()
                    });
                }
                else
                {                    
                    response.LenderTransactionFeeConfigurationViews.Add(new LenderTransactionFeeConfigurationView()
                    {
                        LenderTypeId = lender.LkValue,
                        LenderType = lender.LkName,
                        ConfigurationFeeViews = transactionFees.Where(s => s.LenderTypeId == lender.LkValue).ToList()
                    });
                }
            }

            response.PublicSearchId = (int)ServiceFees.PublicSearch;
            response.PublicGenerateCertificateId = (int)ServiceFees.CertifiedSearchResult;

            response.ConfigurationTransactionFeesViews = transactionFeesConfiguration.Where(q=>q.LenderType != Constants.DefaultLenderTypeIdForPublicUsers).GroupBy(p => p.ServiceTypeId).Select(m => m.FirstOrDefault()).Select(s => new ConfigurationTransactionFeesView()
            {
                ServiceTypeId = s.ServiceTypeId,
                ServiceType = s.ServiceType,
                //Fee = s.Fee,
                Id = s.Id
            }).ToList();

            response.NumRecords = response.ConfigurationFeesSetupView.ConfiguationFeesView.Count();
            response.ServiceFeeType = LookUpServiceModel.ServiceFeeCategories(_serviceFeeCategoryRepository);
            response.LenderType = lenderTypes;
            return response;
        }

        public SaveFeeConfigurationResponse SaveFeeConfiguration(SaveFeeConfigurationRequest request)
        {

            
            
            SaveFeeConfigurationResponse response = new SaveFeeConfigurationResponse();

            if (!request.SecurityUser.IsInAnyRoles("Administrator (Owner)"))
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }

            AuditedDate = DateTime.Now;

            var LenderTypeId = request.LenderTypeId;
            var ConfigurationTransactionFeesViews = request.ConfigurationTransactionFeesViews;
            var message = "";

            
            if (LenderTypeId == Constants.DefaultLenderTypeIdForPublicUsers)
            {
                var configurations = _configurationFeeRepository.GetDbSet().Where(s => s.LenderType == Constants.DefaultLenderTypeIdForPublicUsers);
                if (configurations.Count() > 0)
                {
                    var transactionFee =
                            configurations.SingleOrDefault(s => s.ServiceTypeId == ServiceFees.PublicSearch && s.LenderType == LenderTypeId);
                    transactionFee.Fee = request.PubilcSearch;
                    auditTracker.Updated.Add(transactionFee);


                    var transactionFee1 = 
                            configurations.SingleOrDefault(s => s.ServiceTypeId == ServiceFees.CertifiedSearchResultPublic && s.LenderType == LenderTypeId);
                    transactionFee1.Fee = request.PublicGenerateSearchResult;
                    auditTracker.Updated.Add(transactionFee1);
                    message = "Successfully updated Configuration";
                }
                else
                {
                    var transactionFee1 = new TransactionPaymentSetup() {
                        Fee = request.PubilcSearch,
                        ServiceTypeId = ServiceFees.PublicSearch,
                        LenderType = Constants.DefaultLenderTypeIdForPublicUsers,
                    };
                    _configurationFeeRepository.Add(transactionFee1);
                    auditTracker.Created.Add(transactionFee1);

                    var transactionFee2 = new TransactionPaymentSetup()
                    {
                        Fee = request.PublicGenerateSearchResult,
                        ServiceTypeId = ServiceFees.CertifiedSearchResultPublic,
                        LenderType = Constants.DefaultLenderTypeIdForPublicUsers,
                    };
                    _configurationFeeRepository.Add(transactionFee2);
                    auditTracker.Created.Add(transactionFee2);
                    message = "Successfully created Configuration";
                }
            }
            else
            {
                var configurations = _configurationFeeRepository.GetDbSet().Where(s => s.LenderType == LenderTypeId && s.LenderType != Constants.DefaultLenderTypeIdForPublicUsers);
                if (configurations.Count() > 0)
                {
                    foreach (var configurationTransactionFee in ConfigurationTransactionFeesViews)
                    {
                        var transactionFee =
                            configurations.SingleOrDefault(s => s.ServiceTypeId == configurationTransactionFee.ServiceTypeId && s.LenderType == LenderTypeId);
                        transactionFee = configurationTransactionFee.ConvertToTransactionConfiguration(transactionFee, LenderTypeId, true);
                        auditTracker.Updated.Add(transactionFee);
                    }
                    message = "Successfully updated Configuration";
                }
                else
                {
                    foreach (var configurationTransactionFee in ConfigurationTransactionFeesViews)
                    {
                        var transactionFee = new TransactionPaymentSetup();
                        transactionFee = configurationTransactionFee.ConvertToTransactionConfiguration(transactionFee, LenderTypeId, false);
                        _configurationFeeRepository.Add(transactionFee);
                        auditTracker.Created.Add(transactionFee);
                    }
                    message = "Successfully created Configuration";
                }
            }
            //ConfigurationFee  = new ConfigurationFee();
           // var query_serviceFees = _serviceFeeCategoryRepository.GetDbSet().Where(s => s.IsDeleted == false);
           // var _serviceFees = _serviceFeeCategoryRepository.FindBy(query_serviceFees);
            //ConfigurationFees = request.ConfigurationFeesView.ConfiguationFeesView.ConvertToConfigurationFees(_serviceFees, auditTracker).ToList();

        
            //if (request.CreateNewConfiguration)
            //{
            //    //ConfigurationFee ConfigurationFee = request.ConfigurationFeesView.ConvertToConfigurationFee( _serviceFees, auditTracker);
            //    ConfigurationFee ConfigurationFee = new ConfigurationFee();//Temporarl correction above is correct
            //    //_configurationFeeRepository.Add(ConfigurationFee);
            //    auditTracker.Created.Add(ConfigurationFee);
            //}
            //else
            //{
            //    //ConfigurationFee ConfigurationFees = _configurationFeeRepository.FindBy(request.ConfigurationFeesView.Id);
            //    ConfigurationFee ConfigurationFees = new ConfigurationFee();//Temporarl correction above is correct
            //    request.ConfigurationFeesView.ConvertToConfigurationFee(ConfigurationFees, _serviceFees,auditTracker);
            //    auditTracker.Updated.Add(ConfigurationFees);
            //}
                      

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
            response.MessageInfo.Message = message;

            return response;
        }

        public ResponseBase ToggleEnableDisableConfiguration(RequestBase request)
        {

            ResponseBase response = new ResponseBase();
            if (!request.SecurityUser.IsInAnyRoles("Administrator (Owner)"))
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }
            string message;
            if (request.Id != null)
            {
                //ConfigurationFee ConfigurationFee = _configurationFeeRepository.FindBy(request.Id);
                ConfigurationFee ConfigurationFee = new ConfigurationFee();//Temporarl correction above is correct

                if (ConfigurationFee.IsActive)
                {
                    ConfigurationFee.IsActive = false;
                    message = "You have successfully disabled the fee configuration";
                }
                else
                {
                    ConfigurationFee.IsActive = true;
                    message = "You have successfully  the postpaid account of the client";
                }
                //Audit audit = new Audit("Fee Configuration:" + membership.ClientCode, request.RequestUrl, request.UserIP, AuditAction.RevokePostpaidRequest);
                //audit.Message = MembershipAuditMsgGenerator.ClientDetails(membership, ClientName);
                //_auditRepository.Add(audit);
                //auditTracker.Created.Add(audit);
                _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
                try
                {
                    _uow.Commit();
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                return new ResponseBase(message, Infrastructure.Messaging.MessageType.Success);
            }

            else
            {

                return new ResponseBase("There was an error performing this operaton", Infrastructure.Messaging.MessageType.Error);
            
            
            }

           

        
        }
 


        public ResponseBase DeleteFeeConfiguration(RequestBase request)
        {

            ResponseBase response = new ResponseBase();
            if (!request.SecurityUser.IsInAnyRoles("Administrator (Owner)"))
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }
            string message;
            if (request.Id != null)
            {
              //  ConfigurationFee ConfigurationFee = _configurationFeeRepository.FindBy(request.Id);

                ConfigurationFee ConfigurationFee = new ConfigurationFee();//Temporarl correction above is correct


                    ConfigurationFee.IsDeleted = true;
                    message = "You have successfully deleted the fee configuration";
    
                //Audit audit = new Audit("Fee Configuration:" + membership.ClientCode, request.RequestUrl, request.UserIP, AuditAction.RevokePostpaidRequest);
                //audit.Message = MembershipAuditMsgGenerator.ClientDetails(membership, ClientName);
                //_auditRepository.Add(audit);
                //auditTracker.Created.Add(audit);
                _uow.AuditEntities(request.SecurityUser.Id, AuditedDate, auditTracker);
                try
                {
                    _uow.Commit();
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                return new ResponseBase(message, Infrastructure.Messaging.MessageType.Success);
            }

            else
            {

                return new ResponseBase("There was an error performing this operaton", Infrastructure.Messaging.MessageType.Error);


            }




        }
        public GetWorkflowConfigurationResponse GetDataForConfiguration(GetDataForConfigurationRequest request)
        {
            GetWorkflowConfigurationResponse response = new GetWorkflowConfigurationResponse();

            //We need to determine which page we need and return that and we need to do the mapping which should be simple
            if (request.ConfigurationPage == ConfigurationPage.Workflow)
            {
                // Get the personalised settings and global  default  settings for the user if they exist
                List<ConfigurationWorkflow> ConfigurationWorkflows = _configurationWorkflowRepository.GetDbSet().Where(s => s.MembershipId == null || s.MembershipId == request.SecurityUser.MembershipId).ToList();
                if (!request.SecurityUser.IsOwnerUser)
                {
                    ConfigurationWorkflow config = null;
                    ConfigurationWorkflow GlobalConfig = new ConfigurationWorkflow();
                    GlobalConfig = ConfigurationWorkflows.Where(s => s.MembershipId == null).Single();
                    //if the personalised settings does not exist
                    if (ConfigurationWorkflows.Count() < 2)
                    {
                        config = new ConfigurationWorkflow()
                        {
                            CreateNewFS = GlobalConfig.CreateNewFS,
                            UpdateFS = GlobalConfig.UpdateFS,
                            DischargeFS = GlobalConfig.DischargeFS,
                            SubordinateFS = GlobalConfig.SubordinateFS,
                            AssignmentFS = GlobalConfig.AssignmentFS,
                            SkipWhenSubmitterIsAuthroizer = GlobalConfig.SkipWhenSubmitterIsAuthroizer,
                            SkipWorkflowForIndividuals = GlobalConfig.SkipWorkflowForIndividuals,
                            UseGlobalSettings = true,
                            IsActive = true,
                            IsDeleted = false,
                            MembershipId = request.SecurityUser.MembershipId,
                            CreatedBy = request.SecurityUser.Id,
                            CreatedOn = DateTime.Now,
                            AllowUsersToAddEmailsForTaskNotification = GlobalConfig.AllowUsersToAddEmailsForTaskNotification,
                            AllowUsersToSelectAssignedUsers = GlobalConfig.AllowUsersToSelectAssignedUsers,
                            AllowWorkflowWhenNoUserAssigned = GlobalConfig.AllowWorkflowWhenNoUserAssigned,
                            NotifyAdministratorWhenNoUserAssigned = GlobalConfig.NotifyAdministratorWhenNoUserAssigned,
                            NotifyAllAfterWorkflowClose = GlobalConfig.NotifyAllAfterWorkflowClose,
                            WorkflowDelayedWarning = GlobalConfig.WorkflowDelayedWarning,
                            UnitLimit = GlobalConfig.UnitLimit,
                            TaskAssignmentNotification = GlobalConfig.TaskAssignmentNotification,
                            NumDaysWorkflowIsDelayed = GlobalConfig.NumDaysWorkflowIsDelayed,
                            NotifyAllAssignedAfterTaskExecution = GlobalConfig.NotifyAllAssignedAfterTaskExecution,
                            EnforceWorkflowForUsersWithBothOfficerAndAuthorizerRoles =GlobalConfig.EnforceWorkflowForUsersWithBothOfficerAndAuthorizerRoles
                        };
                        _configurationWorkflowRepository.Add(config);//Add the second
                        _uow.Commit();
                    }

                    else
                    {
                        config = ConfigurationWorkflows.Where(s => s.MembershipId == request.SecurityUser.MembershipId).Single();
                    }


                    response.MemberConfigurationWorkflowView = config.ConvertToConfigurationWorkflowView();
                }



                response.GlobalConfigurationWorkflowView = ConfigurationWorkflows.Where(s => s.MembershipId == null).Single().ConvertToConfigurationWorkflowView();
            }




            return response;


        }

        public SaveConfigurationResponse SaveConfiguration(SaveConfigurationRequest request)
        {
            SaveConfigurationResponse response = new SaveConfigurationResponse();
            AuditedDate = DateTime.Now;

            if (request.SecurityUser != null && !request.SecurityUser.IsAdministrator())
            {
                response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Unauthorized;
                response.MessageInfo.Message = "Unauthorized Access";
                return response;
            }

            //We need to determine which page we need and return that and we need to do the mapping which should be simple
            if (request.ConfigurationPage == ConfigurationPage.Workflow)
            {
                if (request.SecurityUser.IsOwnerUser)
                {
                    ConfigurationWorkflow ConfigurationWorkflow = _configurationWorkflowRepository.FindBy(request.GlobalConfigurationWorkflowView.Id);
                    request.GlobalConfigurationWorkflowView.ConvertToConfigurationWorkflow(ConfigurationWorkflow);
                    auditTracker.Updated.Add(ConfigurationWorkflow);

                }
                else if (!request.SecurityUser.IsOwnerUser && request.GlobalConfigurationWorkflowView == null)
                {
                    ConfigurationWorkflow ConfigurationWorkflow = null;
                    ConfigurationWorkflow = _configurationWorkflowRepository.FindBy(request.MemberConfigurationWorkflowView.Id);
                    request.MemberConfigurationWorkflowView.ConvertToConfigurationWorkflow(ConfigurationWorkflow);
                    auditTracker.Updated.Add(ConfigurationWorkflow);

                }

                else if (!request.SecurityUser.IsOwnerUser && request.MemberConfigurationWorkflowView == null)
                {

                    ConfigurationWorkflow ConfigurationWorkflow = null;
                    ConfigurationWorkflow = _configurationWorkflowRepository.GetDbSet().Where(s => s.MembershipId == request.SecurityUser.MembershipId).SingleOrDefault();
                    ConfigurationWorkflow.UseGlobalSettings = true;
                    auditTracker.Updated.Add(ConfigurationWorkflow);

                }




            }






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
            response.MessageInfo.Message = "Successfully Changed Configuration";

            if (request.ConfigurationPage == ConfigurationPage.Workflow)
            {
                //Set them to response and return response

            }
            return response;

        }

        public ResponseBase CreateBVCData(CreateNewBVCDataRequest request)
        {
            ResponseBase response = new ResponseBase();
            _bankVerificationCodeRepository.TruncateAllData();

            var bvcList = request.BankVerificationCodesView.Select(s => new BankVerificationCode
            {
                Name = s.Name,
                Code = s.Code,
                Type = s.Type,
                level = s.Level
            });

            foreach (var item in bvcList)
            {
                _bankVerificationCodeRepository.Add(item);
                auditTracker.Created.Add(item);
            }

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
            response.MessageInfo.Message = "Successfully Updated Bank Verification Code data";

            return response;

        }


        public GetBvcDataResponse GetBVCData(GetBvcDataRequest request)
        {
            GetBvcDataResponse response = new GetBvcDataResponse();
            if (request.PageIndex > 0)
            {
            var myquery = BVCQueryGenerator.CreateQueryFor(request, _bankVerificationCodeRepository, true);
            int TotalRecords = myquery.Count();
            response.NumRecords = TotalRecords;
            }
            var myquery2 = BVCQueryGenerator.CreateQueryFor(request, _bankVerificationCodeRepository);
            response.BvcCodes = myquery2.ToList();
            return response;
        }

        public GetDataForConfigurationTransactionFeesResponse GetDataForConfigurationTransactionFees(GetDataForConfigurationTransactionFeesRequest request)
        {
            GetDataForConfigurationTransactionFeesResponse response =
                new GetDataForConfigurationTransactionFeesResponse();

            var transactionFees = _configurationTransactionPaymentRespository.GetDbSetComplete();
            //fee = _configurationFeeRepository.GetDbSetComplete(ServiceFees ., LenderType).SingleOrDefault();
            response.ConfigurationTransactionFeesViews = transactionFees.Select(s => new ConfigurationTransactionFeesView()
            {
                ServiceTypeId = s.ServiceTypeId,
                ServiceType = s.ServiceType,
                Fee = s.Fee,
                Id = s.Id
            }).ToList();

            return response;
        }

        public SaveFeesConfigurationResponse SaveFeesConfiguration(SaveFeesConfigurationRequest request)
        {
            SaveFeesConfigurationResponse response = new SaveFeesConfigurationResponse();
            var configurationTransactionFees = _configurationTransactionPaymentRespository.GetDbSetComplete();

            var ConfigurationTransactionFees = request.ConfigurationTransactionFees;
            foreach (var configurationTransactionFee in ConfigurationTransactionFees)
            {
                var transactionFee =
                    configurationTransactionFees.SingleOrDefault(s => s.ServiceTypeId == configurationTransactionFee.ServiceTypeId);
                transactionFee = configurationTransactionFee.ConvertToTransactionConfiguration(transactionFee);
                auditTracker.Updated.Add(transactionFee);
            }

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
            var transactionFees = _configurationTransactionPaymentRespository.GetDbSetComplete();
            response.ConfigurationTransactionFees = transactionFees.Select(s => new ConfigurationTransactionFeesView()
            {
                ServiceTypeId = s.ServiceTypeId,
                ServiceType = s.ServiceType,
                Fee = s.Fee,
                Id = s.Id
            }).ToList();
            response.MessageInfo.MessageType = Infrastructure.Messaging.MessageType.Success;
            response.MessageInfo.Message = "Successfully saved Fee Configuration";
            return response;
        }
    }

}
