using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model;
using CRL.Model.Common.IRepository;
using CRL.Model.Configuration.IRepository;

using CRL.Model.FS .IRepository;


using CRL.Model.Reporting.IRepository;
using CRL.Model.Search.IRepository;
using CRL.Model.WorkflowEngine.IRepository;
using CRL.Repository.EF.All;
using CRL.Repository.EF.All.Repository;
using CRL.Repository.EF.All.Repository.Common;
using CRL.Repository.EF.All.Repository.Configuration;
using CRL.Repository.EF.All.Repository.Email;
using CRL.Repository.EF.All.Repository.FinancialStatement;
using CRL.Repository.EF.All.Repository.Memberships;
using CRL.Repository.EF.All.Repository.Payments;
using CRL.Repository.EF.All.Repository.Reporting;
using CRL.Repository.EF.All.Repository.Search;
using CRL.Repository.EF.All.Repository.Workflow;
using CRL.Service.Implementations;
using CRL.Service.Interfaces;
using CRL.Service.Interfaces.FinancialStatement;
using StructureMap;
using StructureMap.Configuration.DSL;
using CRL.Model.Notification.IRepository;
using CRL.Model.Memberships.IRepository;
using CRL.Model.Payments.IRepository;

namespace CRL.WebServices
{
    public class BootStrapper
    {
        public static void ConfigureDependencies()
        {
            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry<ControllerRegistry>();

            });
        }

        public class ControllerRegistry : Registry
        {
            public ControllerRegistry()
            {
                // Repositories    

                For<IInstitutionRepository>().Use<InstitutionRepository>();
                For<IInstitutionUnitRepository>().Use<InstitutionUnitRepository>();
                For<ILKCompanyCategoryRepository>().Use<LKCompanyCategoryRepository>();
                For<IUserRepository>().Use<UserRepository>();
                For<IRoleRepository>().Use<RoleRepository>();
                For<IUnitOfWork>().Use<EFUnitOfWork>();
                For<IInstitutionService>().Use<InstitutionService>();
                For<IInstitutionUnitService>().Use<InstitutionUnitService>();
                For<IUserService>().Use<UserService>();
                For<IMembershipRegistrationService>().Use<MembershipRegistrationService>();
                For<IFinancialStatementRepository>().Use<FinancialStatementRepository>();
                For<IFinancialStatementActivityRepository>().Use<FinancialStatementActivityRepository>();
                For<ILKCollateralCategoryRepository>().Use<LKCollateralCategoryRepository>();
                For<ILKCollateralSubTypeCategoryRepository>().Use<LKCollateralSubTypeCategoryRepository>();
                For<ILKCurrencyRepository>().Use<LKCurrencyRepository>();
                For<ILKFinancialStatementCategoryRepository>().Use<LKFinancialStatementCategoryRepository>();
                For<ILKFinancialStatementTransactionCategoryRepository>().Use<LKFinancialStatementTransactionCategoryRepository>();
                For<ILKPersonIdentificationCategoryRepository>().Use<LKPersonIdentificationCategoryRepository>();
                For<ILKSectorOfOperationCategoryRepository>().Use<LKSectorOfOperationCategoryRepository>();
                For<ILKSecuringPartyIndustryCategoryRepository>().Use<LKSecuringPartyIndustryCategoryRepository>();

                For<ISerialTrackerRepository>().Use<SerialTrackerRepository>();
                For<ICollateralRepository>().Use<CollateralRepository>();
                For<ILKFinancialStatementActivityCategoryRepository>().Use<LKFinancialStatementActivityCategoryRepository>();
                For<IAuthenticationService>().Use<AuthenticatonService>();
                For<ILKCountryRepository>().Use<LKCountryRepository>();
                For<ILKCountyRepository>().Use<LKCountyRepository>();
                For<ILKLGARepository>().Use<LKLGARepository>();
                For<ILKNationalityRepository>().Use<LKNationalityRepository>();
                For<IMembershipRepository>().Use<MembershipRepository>();
                For<IWFWorkflowRepository>().Use<WFWorkflowRepository>();
                For<IWFCaseRepository>().Use<WFCaseRepository>();
                For<IUserService>().Use<UserService>();
                For<IMembershipRegistrationRequestRepository>().Use<MembershipRegistrationRequestRepository>();
                For<IFinancialStatementSnapshotRepository>().Use<FinancialStatementSnapshotRepository>();
                For<IFileUploadRepository>().Use<FileUploadRepository>();
                For<IEmailRepository>().Use<EmailRepository>();
                For<IEmailTemplateRepository>().Use<EmailTemplateRepository>();
                For<IParticipantRepository>().Use<ParticipantRepository>();
                For<ICollateralRepository>().Use<CollateralRepository>();
                For<IPaymentRepository>().Use<PaymentRepository>();
                //For<IFeeRepository >().Use<FeeRepository>();
                For<IAccountTransactionRepository>().Use<AccountTransactionRepository>();
                For<IAccountBatchRepository>().Use<AccountBatchRepository>();
                For<IPublicUserSecurityCodeRepository>().Use<PublicUserSecurityCodeRepository>();
                For<IConfigurationWorkflowRepository>().Use<ConfigurationWorkflowRepository>();
                For<IConfigurationFeeRepository>().Use<ConfigurationFeeRepository>();
                For<IPasswordResetRequestRepository>().Use<PasswordResetRequestRepository>();
                For<IWFTokenRepository>().Use<WFTokenRepository>();
                For<IWorkflowService>().Use<WorkflowService>();

                For<ISearchFinancialStatementRepository>().Use<SearchFinancialStatementRepository>();
                For<IAuditRepository>().Use<AuditRepository>();
                For<IServiceRequestRepository>().Use<ServiceRequestRepository>();
                For<IReportRepository>().Use<ReportRepository>();
                For<ILKAuditCategoryRepository>().Use<LKAuditCategoryRepository>();
                For<IPersonIdentificationRepository>().Use<PersonIdentificationRepository>();
                For<IEmailUserAssignmentRepository>().Use<EmailUserAssignmentRepository>();
                For<IMessageRepository>().Use<MessageRepository>();
                For<IInterSwitchRepository>().Use<InterSwitchRepository>();
                For<IInterSwitchDirectPayRepository>().Use<InterSwitchDirectPayRepository>();

                //For<IAmendmentService>().Use<AmendmentService>();
                For<IPaymentService>().Use<PaymentService>();
                For<IConfigurationService>().Use<ConfigurationService>();
                For<ILKServiceFeeCategoriesRepository>().Use<LKServiceFeeCategoriesRepository>();
                For<IClientService>().Use<ClientService>();
                For<IReportService>().Use<ReportService>();
                For<IEmailService>().Use<EmailService>();
                For<IWidgetService>().Use<WidgetService>();
                For<IFSBatchDetailRepository>().Use<FSBatchDetailRepository>();
                For<IFSBatchRepository>().Use<FSBatchRepository>();
                For<IFSBatchService>().Use<FSBatchService>();
                For<ITempAttachmentRepository>().Use<TempAttachmentRepository>();
                For<ILKRegistrationPrefixRepository>().Use<LKRegistrationPrefixRepository>();
                For<IBankVerificationCodeRepository>().Use<BankVerificationCodeRepository>();
                For<IConfigurationTransactionPaymentRespository>().Use<ConfigurationTransactionPaymentRespository>();




                //// Application Settings                 
                //ForRequestedType<IApplicationSettings>().TheDefault.Is.OfConcreteType
                //         <WebConfigApplicationSettings>();

                // Logger
                // For<ILogger>().Use<Log4NetAdapter>();

                //// Email Service                 
                //ForRequestedType<IEmailService>().TheDefault.Is.OfConcreteType
                //        <TextLoggingEmailService>();
            }
        }
    }
}