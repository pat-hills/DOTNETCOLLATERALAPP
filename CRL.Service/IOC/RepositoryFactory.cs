using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model;
using CRL.Model.Common.IRepository;
using CRL.Model.FS.IRepository;

using CRL.Repository.EF.All;
using CRL.Repository.EF.All.Repository;
using CRL.Repository.EF.All.Repository.Common;
using CRL.Repository.EF.All.Repository.FinancialStatement;
using CRL.Repository.EF.All.Repository.Memberships;
using CRL.Infrastructure.Domain;
using CRL.Model.WorkflowEngine.IRepository;
using CRL.Model.Notification.IRepository;
using CRL.Repository.EF.All.Repository.Workflow;

using CRL.Repository.EF.All.Repository.Payments;
using CRL.Model.Configuration.IRepository;
using CRL.Repository.EF.All.Repository.Configuration;
using CRL.Repository.EF.All.Repository.Email;
using CRL.Model.Search.IRepository;
using CRL.Repository.EF.All.Repository.Search;
using CRL.Model.Memberships.IRepository;
using CRL.Model.Payments.IRepository;

using CRL.Model.Model.Search.IRepository;
using CRL.Repository.EF.All.Repository.Search;


namespace CRL.Service.IOC
{
    public static class RepositoryFactory
    {
        public static IUnitOfWork UnitOfWork()
        {
            return new EFUnitOfWork();
        }
       
        public static IInstitutionRepository InstitutionRepository(IUnitOfWork uow)
        {
            return new InstitutionRepository(uow);
        }
        public static IInstitutionUnitRepository InstitutionUnitRepository(IUnitOfWork uow)
        {
            return new InstitutionUnitRepository(uow);
        }
        public static ILKCountryRepository LKCountryRepository(IUnitOfWork uow)
        {
            return new LKCountryRepository(uow);
        }
        public static ILKCountyRepository LKCountyRepository(IUnitOfWork uow)
        {
            return new LKCountyRepository(uow);
        }
        public static IMembershipRepository MembershipRepository(IUnitOfWork uow)
        {
            return new MembershipRepository(uow);
        }
        public static ILKNationalityRepository LKNationalityRepository(IUnitOfWork uow)
        {
            return new LKNationalityRepository(uow);
        }
        public static ILKSecuringPartyIndustryCategoryRepository LKSecuringPartyIndustryCategoryRepository(IUnitOfWork uow)
        {
            return new LKSecuringPartyIndustryCategoryRepository(uow);
        }
        public static ISerialTrackerRepository SerialTrackerRepository(IUnitOfWork uow)
        {
            return new SerialTrackerRepository(uow);
        }
        public static IAuditRepository AuditRepository(IUnitOfWork uow)
        {
            return new AuditRepository(uow);
        }
        public static ILKAuditCategoryRepository AuditCategoryRepository(IUnitOfWork uow)
        {
            return new LKAuditCategoryRepository(uow);
        }

        public static ILKAuditActionRepository AuditActionRepository(IUnitOfWork uow)
        {
            return new LKAuditActionRepository(uow);
        }

        public static ILKCollateralCategoryRepository LKCollateralCategoryRepository(IUnitOfWork uow)
        {
            return new LKCollateralCategoryRepository(uow);
        }
        public static ILKCollateralSubTypeCategoryRepository LKCollateralSubTypeCategoryRepository(IUnitOfWork uow)
        {
            return new LKCollateralSubTypeCategoryRepository(uow);
        }
        public static ILKCurrencyRepository LKCurrencyRepository(IUnitOfWork uow)
        {
            return new LKCurrencyRepository(uow);
        }
        public static ILKFinancialStatementCategoryRepository LKFinancialStatementCategoryRepository(IUnitOfWork uow)
        {
            return new LKFinancialStatementCategoryRepository(uow);
        }
        public static ILKFinancialStatementTransactionCategoryRepository LKFinancialStatementTransactionCategoryRepository(IUnitOfWork uow)
        {
            return new LKFinancialStatementTransactionCategoryRepository(uow);
        }

        public static ILKPersonIdentificationCategoryRepository LKPersonIdentificationCategoryRepository(IUnitOfWork uow)
        {
            return new LKPersonIdentificationCategoryRepository(uow);
        }
        public static ILKSectorOfOperationCategoryRepository LKSectorOfOperationCategoryRepository(IUnitOfWork uow)
        {
            return new LKSectorOfOperationCategoryRepository(uow);
        }
        public static ILKCompanyCategoryRepository LKCompanyCategoryRepository(IUnitOfWork uow)
        {
            return new LKCompanyCategoryRepository(uow);
        }

        public static IFinancialStatementActivityRepository FinancialStatementActivityRepository(IUnitOfWork uow)
        {
            return new FinancialStatementActivityRepository(uow);
        }
        public static IFinancialStatementRepository FinancialStatementRepository(IUnitOfWork uow)
        {
            return new FinancialStatementRepository(uow);
        }
        public static ILKFinancialStatementActivityCategoryRepository LKFinancialStatementActivityCategoryRepository(IUnitOfWork uow)
        {
            return new LKFinancialStatementActivityCategoryRepository(uow);
        }
        public static ICollateralRepository LKCollateralRepository(IUnitOfWork uow)
        {
            return new CollateralRepository(uow);
        }


        public static IWFWorkflowRepository WFWorkflowRepository(IUnitOfWork uow)
        {
            return new WFWorkflowRepository(uow);
        }
        public static IWFCaseRepository WFCaseRepository(IUnitOfWork uow)
        {
            return new WFCaseRepository(uow);
        }
        public static IWFTokenRepository  WFTokenRepository(IUnitOfWork uow)
        {
            return new WFTokenRepository(uow);
        }
        public static IUserRepository UserRepository(IUnitOfWork uow)
        {
            return new UserRepository(uow);
        }


        public static IFinancialStatementSnapshotRepository FinancialStatementSnapshotRepository(IUnitOfWork uow)
        {
            return new FinancialStatementSnapshotRepository(uow);
        }
        public static IFileUploadRepository FileUploadRepository(IUnitOfWork uow)
        {
            return new FileUploadRepository(uow);
        }
        public static IEmailTemplateRepository EmailTemplateRepository(IUnitOfWork uow)
        {
            return new EmailTemplateRepository(uow);
        }
        public static IEmailRepository EmailRepository(IUnitOfWork uow)
        {
            return new EmailRepository(uow);
        }
        public static IMessageRepository  MessageRepository(IUnitOfWork uow)
        {
            return new MessageRepository(uow);
        }



        public static IAccountTransactionRepository AccountTransactionRepository(IUnitOfWork uow)
        {
            return new AccountTransactionRepository(uow);
        }
      
        public static IConfigurationWorkflowRepository ConfigurationWorkflowRepository(IUnitOfWork uow)
        {
            return new ConfigurationWorkflowRepository(uow);
        }
        public static IConfigurationFeeRepository ConfigurationFeeRepository(IUnitOfWork uow)
        {
            return new ConfigurationFeeRepository(uow);
        }




        public static IServiceRequestRepository ServiceRequestRepository(IUnitOfWork uow)
        {
            return new ServiceRequestRepository(uow);
        }
        public static IParticipantRepository ParticipantRepository(IUnitOfWork uow)
        {
            return new ParticipantRepository(uow);
        }
        public static IPersonIdentificationRepository PersonIdentificationRepository(IUnitOfWork uow)
        {
            return new PersonIdentificationRepository(uow);
        }
        public static ISearchFinancialStatementRepository SearchFinancialStatementRepository(IUnitOfWork uow)
        {
            return new SearchFinancialStatementRepository(uow);
        }
        public static IEmailUserAssignmentRepository EmailUserAssignmentRepository(IUnitOfWork uow)
        {
            return new EmailUserAssignmentRepository(uow);
        }
        public static ILKLGARepository LKLGARepository(IUnitOfWork uow)
        {
            return new LKLGARepository (uow);
        }
        public static ILKRegistrationPrefixRepository LKRegistrationPrefixRepository(IUnitOfWork uow)
        {
            return new LKRegistrationPrefixRepository  (uow);
        }
        public static IPublicUserSecurityCodeRepository PublicUserSecurityCodeRepository(IUnitOfWork uow)
        {
            return new PublicUserSecurityCodeRepository(uow);
        }
        public static IPaymentRepository PaymentRepository(IUnitOfWork uow)
        {
            return new PaymentRepository(uow);
        }
        public static IRoleRepository RoleRepository(IUnitOfWork uow)
        {
            return new RoleRepository(uow);
        }

        public static IBankVerificationCodeRepository BankVerificationCodeRepository(IUnitOfWork uow)
        {
            return new BankVerificationCodeRepository (uow);
        }

        public static IPasswordResetRequestRepository PasswordResetRequestRepository(IUnitOfWork uow)
        {
            return new PasswordResetRequestRepository(uow);
        }
        public static IInterSwitchRepository  InterSwitchRepository(IUnitOfWork uow)
        {
            return new InterSwitchRepository(uow);
        }
        public static IInterSwitchDirectPayRepository InterSwitchDirectPayRepository(IUnitOfWork uow)
        {
            return new InterSwitchDirectPayRepository(uow);
        }
        public static IAccountBatchRepository AccountBatchRepository(IUnitOfWork uow)
        {
            return new AccountBatchRepository(uow);
        }

        public static ITrackSearchResultRepository TrackSearchResultRepository(IUnitOfWork uow)
        {
            return new TrackSearchResultRepository(uow);
        }
    }
}
