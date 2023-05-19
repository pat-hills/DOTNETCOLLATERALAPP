using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.Messaging;
using CRL.Model.FS.Enums;
using CRL.Model.FS.IRepository;

using CRL.Model.ModelService.FS;
using CRL.Model.Notification.IRepository;
using CRL.Model.WorkflowEngine;
using CRL.Model.WorkflowEngine.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Memberships.IRepository;

namespace CRL.Model.Factories
{
    //**We can beautifull passed an interface with all the repositories instead of this massive constructors
    public static class FactoryChangeServiceModel
    {
        public static ChangeServiceModel CreateChangeServiceModel(
            ISerialTrackerRepository _serialTrackerRepository, IFinancialStatementRepository _financialStatementRepository,
            IEmailRepository _emailRepository,IEmailTemplateRepository _emailTemplateRepository, IEmailUserAssignmentRepository _emailUserAssignmentRepository, ILKFinancialStatementTransactionCategoryRepository _financialStatementTransactionCategoryRepository,
            ILKCollateralCategoryRepository _collateralCategoryRepository,ILKSectorOfOperationCategoryRepository _sectorOfOperationCategoryRepository ,IFileUploadRepository _fileUploadRepository,IFinancialStatementSnapshotRepository _fsSnapShotRepository,
            IFinancialStatementActivityRepository _fsActivityRepository,IMembershipRepository _membershipRepository ,IInstitutionRepository _institutionRepository, IUserRepository _userRepository,
            RequestMode FinancialActivityRequestMode ,  FinancialStatementActivityCategory FinancialStatementActivityType, AuditingTracker tracker, SecurityUser user, string ServiceRequest)
        {
            if (FinancialStatementActivityType == FinancialStatementActivityCategory.Update)
            {
                if (FinancialActivityRequestMode == RequestMode.Submit || FinancialActivityRequestMode == RequestMode.Resend)
                {
                    return new SubmitUpdateFSServiceModel (_serialTrackerRepository, _financialStatementRepository, _emailRepository,
                                                           _financialStatementTransactionCategoryRepository, _collateralCategoryRepository,
                                                           _sectorOfOperationCategoryRepository, _fileUploadRepository,_fsSnapShotRepository,
                                                           _fsActivityRepository, tracker, user, ServiceRequest);
                }
                else if (FinancialActivityRequestMode == RequestMode.Create)
                {
                    return new UpdateFSServiceModel (_serialTrackerRepository, _financialStatementRepository, _emailRepository,_emailTemplateRepository,_emailUserAssignmentRepository,
                                                          _financialStatementTransactionCategoryRepository, _collateralCategoryRepository,
                                                          _sectorOfOperationCategoryRepository, _fileUploadRepository, _fsSnapShotRepository,
                                                          _fsActivityRepository, tracker, user, ServiceRequest);
                }
                else if (FinancialActivityRequestMode == RequestMode.Approval || FinancialActivityRequestMode == RequestMode.Deny )
                {
                    return new AuthorizeUpdateServiceModel (_serialTrackerRepository, _financialStatementRepository, _emailRepository,_emailUserAssignmentRepository ,
                                                         _financialStatementTransactionCategoryRepository, _collateralCategoryRepository,
                                                         _sectorOfOperationCategoryRepository,_fsActivityRepository, tracker, user);
                }

            }
            else if (FinancialStatementActivityType == FinancialStatementActivityCategory.PartialDischarge ||
                FinancialStatementActivityType == FinancialStatementActivityCategory.FullDicharge)
            {
                return new DischargeServiceModel (_serialTrackerRepository,_emailRepository ,_emailTemplateRepository ,_emailUserAssignmentRepository , _financialStatementTransactionCategoryRepository, _collateralCategoryRepository,
                                                    _financialStatementRepository ,_fsActivityRepository, tracker, user, ServiceRequest);
            }
            else if (FinancialStatementActivityType == FinancialStatementActivityCategory.Subordination)
            {
                return new SubordinateServiceModel(_serialTrackerRepository, _emailRepository, _emailTemplateRepository, _emailUserAssignmentRepository, _financialStatementTransactionCategoryRepository, _collateralCategoryRepository,
                                                    _financialStatementRepository, _fsActivityRepository, tracker, user, ServiceRequest);
            }
            else if (FinancialStatementActivityType == FinancialStatementActivityCategory.FullAssignment ||
                FinancialStatementActivityType == FinancialStatementActivityCategory.PartialAssignment )
            {
                return new AssignServiceModel (_serialTrackerRepository, _emailRepository, _emailTemplateRepository, _emailUserAssignmentRepository, _financialStatementTransactionCategoryRepository, _collateralCategoryRepository,
                                                    _financialStatementRepository, _fsActivityRepository,_membershipRepository ,_institutionRepository ,_userRepository  ,tracker, user, ServiceRequest);
            }
            else if (FinancialStatementActivityType == FinancialStatementActivityCategory.DischargeDueToError)
            {
                return new DischargeFSDueToErrorServiceModel(_serialTrackerRepository, _emailRepository, _emailTemplateRepository, _emailUserAssignmentRepository, _financialStatementTransactionCategoryRepository, _collateralCategoryRepository,
                                                    _financialStatementRepository, _fsActivityRepository, tracker, user, ServiceRequest);
            }
          
                throw new Exception("Change service model undeifined");
            

        }

        public static WFTaskType GetTaskType(FinancialStatementActivityCategory FinancialStatementActivityType)
        {
            if (FinancialStatementActivityType == FinancialStatementActivityCategory.Update) return WFTaskType.UpdateRegistration;
            else if (FinancialStatementActivityType == FinancialStatementActivityCategory.Subordination) return WFTaskType.SubordinateRegistration;
            else if (FinancialStatementActivityType == FinancialStatementActivityCategory.FullDicharge) return WFTaskType.DischargeRegistration;
            else if (FinancialStatementActivityType == FinancialStatementActivityCategory.PartialDischarge) return WFTaskType.DischargeRegistration;
            else if (FinancialStatementActivityType == FinancialStatementActivityCategory.FullAssignment) return WFTaskType.AssignRegistration;
            else if (FinancialStatementActivityType == FinancialStatementActivityCategory.PartialAssignment) return WFTaskType.AssignRegistration;
            else if (FinancialStatementActivityType == FinancialStatementActivityCategory.DischargeDueToError) return WFTaskType.DischargeRegistrationDueToError;
            else
                throw new Exception("Invalid FinancialStatementActivityType");


        }
    }
}
