using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Domain;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.FS.IRepository;
using CRL.Model.Messaging;
using CRL.Model.Model.FS;
using CRL.Model.Notification.IRepository;
using CRL.Model.WorkflowEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelService.FS
{
    public class DischargeFSDueToErrorServiceModel: ChangeServiceModel
    {
        ActivityDischargeDueToError fsActivity = null;
        IFinancialStatementActivityRepository _fsActivityRepository { get; set; }
        FinancialStatementActivityCategory dischargeType { get; set; }

        public DischargeFSDueToErrorServiceModel(ISerialTrackerRepository serialTrackerRepository,
            IEmailRepository emailRepository, IEmailTemplateRepository emailTemplateRepository, IEmailUserAssignmentRepository emailUserAssignmentRepository, ILKFinancialStatementTransactionCategoryRepository financialStatementTransactionCategoryRepository,
            ILKCollateralCategoryRepository collateralCategoryRepository, IFinancialStatementRepository financialStatementRepository,IFinancialStatementActivityRepository fsActivityRepository,AuditingTracker tracker, SecurityUser user, string ServiceRequest)
        {
            _emailRepository = emailRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _emailUserAssignmentRepository = emailUserAssignmentRepository;
            _financialStatementTransactionCategoryRepository = financialStatementTransactionCategoryRepository;
            _collateralCategoryRepository = collateralCategoryRepository;
            _financialStatementRepository = financialStatementRepository ;
            _fsActivityRepository = fsActivityRepository;
            _serialTrackerRepository = serialTrackerRepository;
            _tracker = tracker;
            _executingUser = user;
            _serviceRequest = ServiceRequest;
            
            
        }

        public override void LoadInitialDataFromRepository(FSActivityRequest fs, WFCaseActivity _case = null)
        {
            DischargeFSDuetoErrorRequest request = (DischargeFSDuetoErrorRequest)fs;
            if (fs.RequestMode != CRL.Infrastructure.Messaging.RequestMode.Submit &&
              fs.RequestMode != CRL.Infrastructure.Messaging.RequestMode.Create)
            {
                fsActivity = (ActivityDischargeDueToError)_case.FinancialStatementActivity;
                submittedFS = fsActivity.FinancialStatement;
            }
            else
            {
                submittedFS = _financialStatementRepository.SelectFSById(request.FinancialStatementId);
            }
            
            TransactionTypeName = submittedFS.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName;
            CollateralTypeName = submittedFS.CollateralType.CollateralCategoryName;
            UniqeCodePrefix  =  "DTE";
        }

        public override FinancialStatementActivity Change(Messaging.FSActivityRequest request, bool SetUniqueNoLater, WFCaseActivity _case = null)
        {
            if (request.RequestMode == CRL.Infrastructure.Messaging.RequestMode.Create)
                return (this.Discharge((DischargeFSDuetoErrorRequest)request, SetUniqueNoLater, _case = null));
            else if (request.RequestMode == CRL.Infrastructure.Messaging.RequestMode.Submit)
                return (this.SubmitDischarge((DischargeFSDuetoErrorRequest)request, SetUniqueNoLater, _case));
            else if (request.RequestMode == CRL.Infrastructure.Messaging.RequestMode.Approval)
                return (this.AuthorizeDischarge((DischargeFSDuetoErrorRequest)request, SetUniqueNoLater, _case));
            else
                throw new Exception("Not implemented yet");
        }

        public FinancialStatementActivity AuthorizeDischarge(DischargeFSDuetoErrorRequest request, bool SetUniqueNoLater, WFCaseActivity _case = null)
        {

            fsActivity.isApprovedOrDenied = 1;
            fsActivity.ApprovedById = this._executingUser.Id;
            fsActivity.ApprovedDate = _tracker.Date;
            fsActivity.IsActive = true;
            
            submittedFS.IsActive = false;
            submittedFS.IsDeleted = true;

            return fsActivity;
        }
        public FinancialStatementActivity SubmitDischarge(DischargeFSDuetoErrorRequest request, bool SetUniqueNoLater, WFCaseActivity _case = null)
        {


            //Create new activity
            fsActivity = new ActivityDischargeDueToError();
            fsActivity.FinancialStatementActivityTypeId = FinancialStatementActivityCategory.DischargeDueToError;
            fsActivity.PreviousActivity = submittedFS.FinancialStatementLastActivity;
            fsActivity.PerformedByRegistrar = _executingUser.IsOwnerUser;

            fsActivity.FinancialStatement = submittedFS;
            fsActivity.FinancialStatement.isPendingAmendment = true;
            fsActivity.PreviousFinancialStatement = submittedFS;
            //Add activity to the tracker
            _tracker.Created.Add(fsActivity);

            if (SetUniqueNoLater == false)
            {
                this.AssignActivityNo(UniqeCodePrefix, fsActivity, _case);
            }


            fsActivity.IsActive = true;
            fsActivity.MembershipId = submittedFS.MembershipId;
            fsActivity.InstitutionUnitId = this._executingUser.InstitutionUnitId;

            _tracker.Created.Add(fsActivity);
            _fsActivityRepository.Add(fsActivity);

            _case.CaseTitle = "Authorize Financing Statement Cancellation Due To Error - " + submittedFS.RegistrationNo;

            //We also need to load all activities and set the current to the current financing statement
            return fsActivity;

        }
        public FinancialStatementActivity Discharge(DischargeFSDuetoErrorRequest request, bool SetUniqueNoLater, WFCaseActivity _case = null)
        {

            //Create new activity
            fsActivity = new ActivityDischargeDueToError();
            fsActivity.FinancialStatementActivityTypeId = FinancialStatementActivityCategory.DischargeDueToError;
            fsActivity.PerformedByRegistrar = _executingUser.IsOwnerUser;

            fsActivity.FinancialStatement = submittedFS;
            fsActivity.PreviousFinancialStatement = submittedFS;

            submittedFS.IsActive = false;
            submittedFS.IsDeleted = true;

            //Add activity to the tracker
            _tracker.Created.Add(fsActivity);

            if (SetUniqueNoLater == false)
            {
                this.AssignActivityNo(UniqeCodePrefix, fsActivity);
            }

            fsActivity.isApprovedOrDenied = 1;
            fsActivity.IsActive = true;
            fsActivity.MembershipId = this._executingUser.MembershipId;
            fsActivity.InstitutionUnitId = this._executingUser.InstitutionUnitId;
            fsActivity.ApprovedById = this._executingUser.Id;
            fsActivity.ApprovedDate = _tracker.Date;

            _tracker.Created.Add(fsActivity);
            _fsActivityRepository.Add(fsActivity);


            //We also need to load all activities and set the current to the current financing statement
            return fsActivity;
        }





        public override FinancialStatementActivity Deny()
        {
            submittedFS.isPendingAmendment = false;
            return fsActivity;
        }
    }
}
