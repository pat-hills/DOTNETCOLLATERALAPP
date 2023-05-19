using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.FS;
using CRL.Model.ModelViews;

using CRL.Model.WorkflowEngine;

using CRL.Service.Views.FinancialStatement;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.Memberships.IRepository;
using CRL.Model.Memberships;


namespace CRL.Service.Mappings.FinancialStatement
{
  
    public static class FSActivitySummaryViewMapper
    {
        public static FSActivityGridView ConvertToFSActivitySummaryView(this FinancialStatementActivity fsActivity, IInstitutionRepository _institutionRepository, IUserRepository _userRepository, int? CurrentUserIdForRequestMode = null)
        {
            FSActivityGridView fview = new FSActivityGridView();
            fview.Id = fsActivity.Id;
            fview.ActivityCode = fsActivity.ActivityCode ;
            fview.ActivityDate = fsActivity.CreatedOn.ToString ()  ;
            fview.FinancialStatementActivityType = fsActivity.FinancialStatementActivityType .FinancialStatementActivityCategoryName ;
            fview.RegistrationNo = fsActivity.FinancialStatement .RegistrationNo;
            fview.FinancialStatementId = fsActivity.FinancialStatementId;
            


            //If we are in request mode and current user is not
            if (CurrentUserIdForRequestMode != null)
            {
                WFCaseActivity _case = fsActivity.Cases.Where(s => s.IsActive == true && s.IsDeleted == false && s.CaseStatus == "OP").FirstOrDefault();

                if (_case != null)
                {
                    if (_case.WorkItems.Any(w => w.IsActive == true && w.IsDeleted == false && w.WorkitemStatus == "EN" && w.AssignedUsers.Any(u => u.Id == CurrentUserIdForRequestMode)))
                    {
                        fview.CurrentUserIsAssginedToRequest = true;
                    }

                    fview.CaseId = _case.Id;
                    fview.ItemIsLocked = _case.LockItem;
                }             

            }

            fview.MembershipId = fsActivity.MembershipId;

            //Check if current user can handle

            //**This is bad and should be done in the query rather
            Institution institution = _institutionRepository.GetDbSet().Where(m => m.MembershipId == fsActivity.MembershipId).SingleOrDefault();
            if (institution != null)
                fview.MembershipName = institution.Name;
            else
            {
                User user = _userRepository.GetDbSet().Where(m => m.MembershipId == fsActivity.MembershipId).SingleOrDefault();
                fview.MembershipName = NameHelper.GetFullName(user.FirstName, user.MiddleName, user.Surname);
            }

       
 
            return fview;

        }

        public static IEnumerable<FSActivityGridView> ConvertToFSActivitysSummaryView(this IEnumerable<FinancialStatementActivity> FSActivitys,
             IInstitutionRepository _institutionRepository, IUserRepository _userRepository, int? CurrentUserIdForRequestMode = null)
        {
            ICollection<FSActivityGridView> fsviews = new List<FSActivityGridView>();
            foreach (var s in FSActivitys)
            {
                fsviews.Add(s.ConvertToFSActivitySummaryView(_institutionRepository, _userRepository, CurrentUserIdForRequestMode));
            }

            return fsviews;

        }


        public static ActivityReportView ConvertToActivityReportView(this FinancialStatementActivity fsActivity)
        {
            ActivityReportView fview= null;
            if (fsActivity is ActivityDischarge)
            {
                ActivityDischarge dischargeActivity = (ActivityDischarge)fsActivity;
                 fview = new DischargeActivityReportView();                
                ((DischargeActivityReportView)fview).DischargedTypeName = dischargeActivity.DischargeType == 1 ? "Cancellation" : "Partial Cancellation";
                ((DischargeActivityReportView)fview).DischargeType = dischargeActivity.DischargeType;                

            }
            else if (fsActivity is ActivityAssignment)
            {
                ActivityAssignment assignmentActivity = (ActivityAssignment)fsActivity;
                fview = new AssignmentActivityReportView();
                ((AssignmentActivityReportView)fview).AssignmentTypeName = assignmentActivity.AssignmentType == 1 ? "Transfer" : "Partial Transfer";
                ((AssignmentActivityReportView)fview).AssignmentType = assignmentActivity.AssignmentType;
                ((AssignmentActivityReportView)fview).AssignedPartyId = assignmentActivity.AssignedMembershipId;
                ((AssignmentActivityReportView)fview).AssignedFromPartyId  = assignmentActivity.AssignedMembershipFromId;
            }
            else if (fsActivity is ActivityUpdate)
            {
                fview = new UpdateActivityReportView();
            }
            else if (fsActivity is ActivitySubordination )
            {
                fview = new SubordinationActivityReportView ();
                ((SubordinationActivityReportView)fview).SubordinatingParticipantId = ((ActivitySubordination)fsActivity).SubordinatingParticipantId;
            }
            //fview.Id = fsActivity.Id;
            fview.ActivityCode = fsActivity.ActivityCode;
            fview.ActivityDate = fsActivity.CreatedOn ;
            fview.Id = fsActivity.Id;
            //fview.FinancialStatementActivityType = fsActivity.FinancialStatementActivityType.FinancialStatementActivityCategoryName;
            
            fview.FinancialStatementId = fsActivity.FinancialStatementId;
            return fview;


            ////If we are in request mode and current user is not
            //if (CurrentUserIdForRequestMode != null)
            //{
            //    WFCaseActivity _case = fsActivity.Cases.Where(s => s.IsActive == true && s.IsDeleted == false && s.CaseStatus == "OP").FirstOrDefault();

            //    if (_case != null)
            //    {
            //        if (_case.WorkItems.Any(w => w.IsActive == true && w.IsDeleted == false && w.WorkitemStatus == "EN" && w.AssignedUsers.Any(u => u.Id == CurrentUserIdForRequestMode)))
            //        {
            //            fview.CurrentUserIsAssginedToRequest = true;
            //        }

            //        fview.CaseId = _case.Id;
            //        fview.ItemIsLocked = _case.LockItem;
            //    }

            //}

            //fview.MembershipId = fsActivity.MembershipId;

            ////Check if current user can handle

            ////**This is bad and should be done in the query rather
            //Institution institution = _institutionRepository.GetDbSet().Where(m => m.MembershipId == fsActivity.MembershipId).SingleOrDefault();
            //if (institution != null)
            //    fview.MembershipName = institution.Name;
            //else
            //{
            //    User user = _userRepository.GetDbSet().Where(m => m.MembershipId == fsActivity.MembershipId).SingleOrDefault();
            //    fview.MembershipName = NameHelper.GetFullName(user.FirstName, user.MiddleName, user.Surname);
            //}



            //return fview;

        }

    }
}
