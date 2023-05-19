using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.FS;


namespace CRL.Model.ModelViewMappers
{
    public static class FSActivitySummaryReportViewBuilder
    {
        public static ICollection<FSActivitySummaryReportView> ConvertToFSActivitySummaryReportView(this IEnumerable<FinancialStatementActivity> models)
        {
            ICollection<FSActivitySummaryReportView> iviews = new List<FSActivitySummaryReportView>();
            foreach (var model in models)
            {
                FSActivitySummaryReportView iview = new FSActivitySummaryReportView();
                iview.Id = model.Id;
                iview.ActivityDate = model.CreatedOn;
                if (model is ActivityRenewal)
                {
                    ActivityRenewal renewalActivity = ((ActivityRenewal)model);
                    iview.ActivityDescription = "Changed new expiry date from " + renewalActivity.BeforeExpiryDate.ToShortDateString () + " to " + renewalActivity.AfterExpiryDate.ToShortDateString ();
                    iview.TypeOfAmendment = "Renewal";
                    iview.AmendmentCode = renewalActivity .ActivityCode ;
                    iview.ActivityDate = renewalActivity.CreatedOn;


                }
                else if (model is ActivityUpdate )
                {
                    ActivityUpdate updateActivity = ((ActivityUpdate)model);

                    string[] Description = updateActivity.GetChangeDescriptionAsArray();
                    iview.ActivityDescription = "";
                    foreach (string s in Description)
                    {
                        iview.ActivityDescription = iview.ActivityDescription + s + Environment.NewLine;
                    }
                    
                    
                    iview.TypeOfAmendment = "Update";
                    iview.AmendmentCode = updateActivity.ActivityCode;
                    iview.ActivityDate = updateActivity.CreatedOn;
                }
                else if (model is ActivityDischarge )
                {
                    ActivityDischarge dischargeActivity = ((ActivityDischarge)model);
                    if (dischargeActivity.DischargeType == 1)
                    {
                        string Description = "The financing statement was discharged";
                        iview.ActivityDescription = Description;
                        iview.TypeOfAmendment = "Discharge";
                        iview.AmendmentCode = dischargeActivity.ActivityCode;
                        iview.ActivityDate = dischargeActivity.CreatedOn;
                        
                    }
                    else
                    {
                        string Description = "The financing statement was partially discharged. " + dischargeActivity.Collaterals.Count() + " collaterals were discharged";
                        iview.ActivityDescription = Description;
                        iview.ActivityDescription = Description;
                        iview.TypeOfAmendment = "Partial Discharge";
                        iview.AmendmentCode = dischargeActivity.ActivityCode;
                        iview.ActivityDate = dischargeActivity.CreatedOn;
                    }
                }
                else if (model is ActivitySubordination )
                {
                    ActivitySubordination subordianteActivity = ((ActivitySubordination)model);
                    string Description = "Subordinated Financing Statement to " + subordianteActivity.MembershipId ;
                    iview.TypeOfAmendment = "Subordination";
                    iview.AmendmentCode = subordianteActivity.ActivityCode;
                    iview.ActivityDate = subordianteActivity.CreatedOn;
                }
                else if (model is ActivityAssignment)
                {
                    ActivityAssignment assignActivity = ((ActivityAssignment)model);
                    string Description = assignActivity.AssignmentType == 1 ? "Partially assigned this financing statement" : "Assigned this financing statement";
                    Description += assignActivity.MembershipId;
                    iview.TypeOfAmendment = "Transfer";
                    iview.AmendmentCode = assignActivity.ActivityCode;
                    iview.ActivityDate = assignActivity.CreatedOn;
                }
                iview.FinancialStatementId = model.FinancialStatementId;
                iviews.Add(iview);
            }

            return iviews;

        }


    }
}
