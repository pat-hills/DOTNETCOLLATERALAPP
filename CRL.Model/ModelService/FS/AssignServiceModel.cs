using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Domain;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Model.FS.IRepository;

using CRL.Model.Messaging;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.Notification.IRepository;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.WorkflowEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViewMappers;
using CRL.Model.Memberships.IRepository;

namespace CRL.Model.ModelService.FS
{
    class AssignServiceModel:ChangeServiceModel
    {
        ActivityAssignment  fsActivity = null;
        IFinancialStatementActivityRepository _fsActivityRepository { get; set; }
        IMembershipRepository _membershipReposotiry { get; set; }
        IInstitutionRepository _institutionRepository { get; set; }
        IUserRepository  _userRepository { get; set; }

        public ClientReportView AssignedClientReportView { get; set; }
        public ClientReportView AssignedFromClientReportView { get; set; }
        

        public AssignServiceModel(ISerialTrackerRepository serialTrackerRepository,
            IEmailRepository emailRepository, IEmailTemplateRepository emailTemplateRepository, IEmailUserAssignmentRepository emailUserAssignmentRepository, ILKFinancialStatementTransactionCategoryRepository financialStatementTransactionCategoryRepository,
            ILKCollateralCategoryRepository collateralCategoryRepository, IFinancialStatementRepository financialStatementRepository, IFinancialStatementActivityRepository fsActivityRepository,
            IMembershipRepository membershipRepository, IInstitutionRepository institutionRepository, IUserRepository  userRepository, AuditingTracker tracker, SecurityUser user, string ServiceRequest)
        {
            _emailRepository = emailRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _emailUserAssignmentRepository = emailUserAssignmentRepository;
            _financialStatementTransactionCategoryRepository = financialStatementTransactionCategoryRepository;
            _collateralCategoryRepository = collateralCategoryRepository;
            _financialStatementRepository = financialStatementRepository;
            _fsActivityRepository = fsActivityRepository;
            _serialTrackerRepository = serialTrackerRepository;
            _tracker = tracker;
            _executingUser = user;
            _serviceRequest = ServiceRequest;
            _membershipReposotiry = membershipRepository;
            _institutionRepository = institutionRepository;
            _userRepository = userRepository;


        }

        public override void LoadInitialDataFromRepository(FSActivityRequest fs, WFCaseActivity _case = null)
        {
            AssignFSRequest request = (AssignFSRequest)fs;

            if (fs.RequestMode != CRL.Infrastructure.Messaging.RequestMode.Submit &&
                fs.RequestMode != CRL.Infrastructure.Messaging.RequestMode.Create)
            {
                fsActivity = (ActivityAssignment)_case.FinancialStatementActivity;
                submittedFS = fsActivity.FinancialStatement;
                AssignedClientReportView = (((ActivityAssignment)fsActivity).AssignedMembership.ConvertToClientReportView(_institutionRepository, _userRepository));
                AssignedFromClientReportView = (((ActivityAssignment)fsActivity).AssignedMembershipFrom.ConvertToClientReportView(_institutionRepository, _userRepository));
            }
            else
            {
               
                submittedFS = _financialStatementRepository.SelectFSById(request.FinancialStatementId);
                
            }
            TransactionTypeName = submittedFS.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName;
            CollateralTypeName = submittedFS.CollateralType.CollateralCategoryName;
            UniqeCodePrefix = "FSS";        

           
        }

     

        public FinancialStatementActivity AuthorizeAssignment(bool SetUniqueNoLater = false)
        {
            
            
            fsActivity.FinancialStatement.MembershipId = fsActivity.AssignedMembershipId;
            fsActivity.FinancialStatement.InstitutionUnitId = null;

            var _historyFinancialStatements = _financialStatementRepository.GetDbSet().Where(s => s.RegistrationNo == fsActivity.FinancialStatement.RegistrationNo && s.Id != fsActivity.FinancialStatementId && s.isApprovedOrDenied == 1 ).ToList();
            foreach (var historyFinancialStatement in _historyFinancialStatements)
            {
                historyFinancialStatement.MembershipId = fsActivity.AssignedMembershipId;
                historyFinancialStatement.InstitutionUnitId = null;

            }

            var _olderActivities = _fsActivityRepository .GetDbSet().Where(s => s.FinancialStatementId == fsActivity.FinancialStatementId && s.Id != fsActivity.Id && s.isApprovedOrDenied ==1).ToList();
            foreach (var olderActivity in _olderActivities)
            {
                olderActivity.MembershipId = fsActivity.AssignedMembershipId;
                olderActivity.InstitutionUnitId = null;


            }

            fsActivity.MembershipId = fsActivity.AssignedMembershipId;

            fsActivity.isApprovedOrDenied =1;
            fsActivity.ApprovedById = this._executingUser.Id;
            fsActivity.ApprovedDate = _tracker.Date;
            fsActivity.IsActive = true;
            //fsActivity.FinancialStatement.FinancialStatementLastActivity = "Transfer";

          

            return fsActivity;
        }

        public FinancialStatementActivity SubmitAssignment(AssignFSRequest request, bool SetUniqueNoLater, WFCaseActivity _case = null)
        {


            //Create new activity
            fsActivity = new ActivityAssignment();
            fsActivity.FinancialStatementActivityTypeId = FinancialStatementActivityCategory.FullAssignment ;
            fsActivity.PreviousActivity = submittedFS.FinancialStatementLastActivity;

            //submittedFS.FinancialStatementLastActivity = "Pending Transfer";
            //Set the updated financing statement to the activity
            fsActivity.FinancialStatement = submittedFS;
            fsActivity.PreviousFinancialStatement = submittedFS;
            fsActivity.FinancialStatement.isPendingAmendment = true;
            fsActivity.Description = request.AssignmentDescription;
            fsActivity.AssignedMembershipId = request.AssignToMembershipId;
            fsActivity.AssignedMembershipFromId = this._executingUser.MembershipId;
            

            fsActivity.AssignmentType = request.AssignType;

            //_case.LimitedToOtherMembershipId = request.AssignToMembershipId; ;

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

            
            _case.CaseTitle = "Authorize Financing Statement Transfer - " + submittedFS.RegistrationNo;
            //We also need to load all activities and set the current to the current financing statement
            return fsActivity;
        }

        public override FinancialStatementActivity Change(Messaging.FSActivityRequest request, bool SetUniqueNoLater, WFCaseActivity _case = null)
        {
            if (request.RequestMode == CRL.Infrastructure.Messaging.RequestMode.Create || request.RequestMode == CRL.Infrastructure.Messaging.RequestMode.Submit)
                return (this.SubmitAssignment((AssignFSRequest)request, SetUniqueNoLater, _case));           
            else if (request.RequestMode == CRL.Infrastructure.Messaging.RequestMode.Approval)
                return (this.AuthorizeAssignment(SetUniqueNoLater));
            else
                throw new Exception("Not implemented yet");
        }

       



        public override FinancialStatementActivity Deny()
        {
            submittedFS.isPendingAmendment = false;
            submittedFS.FinancialStatementLastActivity = fsActivity.PreviousActivity;
            return fsActivity;
        }
    }
}
