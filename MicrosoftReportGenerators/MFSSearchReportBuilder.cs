using CRL.Infrastructure.Configuration;
using CRL.Model.Model.Search;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftReportGenerators
{
    public class MFSSearchReportBuilder : SearchFSReportBuilder
    {
        void SubReportProcessingEventHandler(object sender, SubreportProcessingEventArgs e)
        {

            if (e.ReportPath == "_FSDetails")
            {
                ReportDataSource rdsFS = new ReportDataSource();
                ReportDataSource rdsLender = new ReportDataSource();
                ReportDataSource rdsDebtor = new ReportDataSource();
                ReportDataSource rdsCollateral = new ReportDataSource();
                rdsFS.Value = FS;
                rdsFS.Name = "FS";  //This refers to the dataset name in the RDLC file
                rdsLender.Name = "Lenders";  //This refers to the dataset name in the RDLC file
                rdsDebtor.Name = "Debtors";  //This refers to the dataset name in the RDLC file
                rdsCollateral.Name = "Collaterals";  //This refers to the dataset name in the RDLC file
                rdsFS.Value = FS;
                rdsLender.Value = Lenders;
                rdsDebtor.Value = Borrowers;
                rdsCollateral.Value = Collaterals;

                e.DataSources.Add(rdsFS);
                e.DataSources.Add(rdsLender);
                e.DataSources.Add(rdsDebtor);
                e.DataSources.Add(rdsCollateral);

            }

            else if (e.ReportPath == "_OtherIdentificationDetails")
            {
                ReportDataSource rdsOtherIdentification = new ReportDataSource();
                rdsOtherIdentification.Value = OtherIdentifications;
                rdsOtherIdentification.Name = "OtherIdentifications";
                e.DataSources.Add(rdsOtherIdentification);

            }

            else if (e.ReportPath == "_ChangeDescription")
            {
                ReportDataSource rdsChangeDescription = new ReportDataSource();
                rdsChangeDescription.Value = ChangeDescription;
                rdsChangeDescription.Name = "ChangeDescription";
                e.DataSources.Add(rdsChangeDescription);
            }

            else if (e.ReportPath == "_DischargedCollaterals")
            {
                ReportDataSource rdsDischargedCollaterals = new ReportDataSource();
                rdsDischargedCollaterals.Value = DischargedCollaterals;
                rdsDischargedCollaterals.Name = "DischargedCollaterals";
                e.DataSources.Add(rdsDischargedCollaterals);

            }
            else if (e.ReportPath == "_SubordinatingParty")
            {
                ReportDataSource rdsSubordinationParty = new ReportDataSource();
                rdsSubordinationParty.Value = SubordinatingParty;
                rdsSubordinationParty.Name = "SubordinatingParty";
                e.DataSources.Add(rdsSubordinationParty);
            }
            else if (e.ReportPath == "_AssignedClient")
            {
                ReportDataSource rdsAssignedClient = new ReportDataSource();
                rdsAssignedClient.Value = AssignedParty;
                rdsAssignedClient.Name = "AssignedClient";
                e.DataSources.Add(rdsAssignedClient);
            }
            else if (e.ReportPath == "_CACResults")
            {
                ReportDataSource rdsCACResults = new ReportDataSource();
                rdsCACResults.Value = CACSearches;
                rdsCACResults.Name = "CACResults";
                e.DataSources.Add(rdsCACResults);
            }




        }

        public override Byte[] GenerateVerificationReport(string IdentifierId)
        {
            LocalReport report = new LocalReport();
            report.SubreportProcessing += new
           SubreportProcessingEventHandler(SubReportProcessingEventHandler);

            ReportDataSource rdsSearchParam = new ReportDataSource();
            rdsSearchParam.Value = SearchParams;
            rdsSearchParam.Name = "SearchParam";
            report.DataSources.Add(rdsSearchParam);
            report.ReportPath = Constants.GetReportPath + "FinancingStatement\\SearchReport.rdlc";

            ReportDataSource rdsUpdates = new ReportDataSource();
            rdsUpdates.Value = Updates;
            rdsUpdates.Name = "Updates";
            report.DataSources.Add(rdsUpdates);

            ReportDataSource rdsDischarges = new ReportDataSource();
            rdsDischarges.Value = DischargeActivity;
            rdsDischarges.Name = "Discharges";
            report.DataSources.Add(rdsDischarges);

            ReportDataSource rdsSubordinations = new ReportDataSource();
            rdsSubordinations.Value = SubordinationActivity;
            rdsSubordinations.Name = "Subordinations";
            report.DataSources.Add(rdsSubordinations);



            ReportDataSource rdsAssignments = new ReportDataSource();
            rdsAssignments.Value = AssignmentActivity;
            rdsAssignments.Name = "Assignments";
            report.DataSources.Add(rdsAssignments);



            ReportDataSource rdsActivities = new ReportDataSource();
            rdsActivities.Value = Activities;
            rdsActivities.Name = "ActivitySummary";
            report.DataSources.Add(rdsActivities);

            ReportDataSource rdsCACResults = new ReportDataSource();
            rdsCACResults.Value = CACSearches;
            rdsCACResults.Name = "CACResults";
            report.DataSources.Add(rdsCACResults);

            if (CACSearches.Count < 1)
            {
                ReportParameter parameter = new ReportParameter("CACVisibility", "True");
                report.SetParameters(parameter);
            }
            else
            {
                ReportParameter parameter = new ReportParameter("CACVisibility", "False");
                report.SetParameters(parameter);
            }

            if (FS.Count < 1)
            {

                ReportParameter parameter = new ReportParameter("FinancialStatementId", "0");
                report.SetParameters(parameter);

                ReportParameter parameter1 = new ReportParameter("FSCount", "0");
                report.SetParameters(parameter1);

                ReportParameter parameter2 = new ReportParameter("IsEmpty", "True");
                report.SetParameters(parameter2);
            }
            else
            {
                ReportParameter parameter = new ReportParameter("FinancialStatementId", FS[0].Id.ToString());
                report.SetParameters(parameter);

                ReportParameter parameter1 = new ReportParameter("FSCount", FS.Count.ToString());
                report.SetParameters(parameter1);
            }

            ReportParameter parameter3 = new ReportParameter("IdentifierId", IdentifierId);
            report.SetParameters(parameter3);

            ReportParameter param = new ReportParameter("ReportGenereatedDateParameter", generatedDate.ToString("dd, MMMM, yyyy  h:mm:ss tt"));
            report.SetParameters(param);
            //Here we will add the search parameter
            Byte[] mybytes = report.Render("PDF");

            return mybytes;
        }
    }
}
