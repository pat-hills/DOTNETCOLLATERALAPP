using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Configuration;
using CRL.Service.Views.FinancialStatement;
using Microsoft.Reporting.WebForms;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.Search;
using CRL.Model.ModelViews.Search;

namespace CRL.Service.MicrosoftReportGenerator
{
    public class SearchReportGenerator
    {
        List<FSReportView> FS;
        List<SecuredPartyReportView> Lenders;
        List<DebtorReportView> Borrowers;
        List<CollateralReportView> Collaterals;
        List<FSActivitySummaryReportView> Activities;
        List<ChangeDescriptionView> ChangeDescriptions;
        List<UpdateActivityReportView> Updates = new List<UpdateActivityReportView>();
        List<OtherIdentificationReportView> OtherIdentifications;

        List<DischargeActivityReportView> DischargeActivity = null;
        List<AssignmentActivityReportView> AssignmentActivity = null;
        List<SubordinationActivityReportView> SubordinationActivity = null;
        List<ClientReportView> AssignedParty = null;
        List<SubordinatingPartyReportView> SubordinatingParty = null;
        List<CollateralReportView> DischargedCollaterals = null;


        List<SearchParamView> SearchParam;

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
                rdsChangeDescription.Value = ChangeDescriptions;
                rdsChangeDescription.Name = "ChangeDescription";
                e.DataSources.Add(rdsChangeDescription);
            }

            else if (e.ReportPath == "_DischargedCollaterals")
            {
                ReportDataSource rdsDischargedCollaterals = new ReportDataSource();
                rdsDischargedCollaterals.Value = DischargedCollaterals ;
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

         
         

          
        }

        public  SearchReportGenerator( List<SearchParamView> _searchParam, List<FSReportView> _fs, List<SecuredPartyReportView> _lenders, List<DebtorReportView> _borrowers,
            List<CollateralReportView> _collaterals, List<FSActivitySummaryReportView> _activities, List<UpdateActivityReportView> _updates,  List<ChangeDescriptionView> _changedescriptions,
              List<OtherIdentificationReportView> _otherIdentifications, 
            List<DischargeActivityReportView> _dischargedActivities,   List<AssignmentActivityReportView> _assignementActivities, List<SubordinationActivityReportView> _subordinatingActivties,
              List<ClientReportView> _assignedParties,List<SubordinatingPartyReportView> _subordinatingparties, List<CollateralReportView> _dischargedCollaterls)
        {


            FS = _fs;
            Lenders = _lenders;
            Borrowers = _borrowers;
            Collaterals = _collaterals;
            Activities = _activities;
            ChangeDescriptions = _changedescriptions;
            OtherIdentifications = _otherIdentifications;
            Updates = _updates;
            SearchParam= _searchParam;
            DischargeActivity = _dischargedActivities;
            AssignmentActivity = _assignementActivities;
            SubordinationActivity = _subordinatingActivties;
            AssignedParty = _assignedParties;
            SubordinatingParty = _subordinatingparties;
            DischargedCollaterals =  _dischargedCollaterls;

         
        }

        public Byte[] GenerateReport()
        {
            LocalReport report = new LocalReport();
            report.SubreportProcessing += new
           SubreportProcessingEventHandler(SubReportProcessingEventHandler);

            ReportDataSource rdsSearchParam = new ReportDataSource();
            rdsSearchParam.Value = SearchParam;
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


                if (FS.Count < 1)
                {
                    ReportParameter parameter = new ReportParameter("FinancialStatementId", "0");
                    report.SetParameters(parameter);

                    ReportParameter parameter2 = new ReportParameter("IsEmpty", "True");
                    report.SetParameters(parameter2);
                }
                else
                {
                    ReportParameter parameter = new ReportParameter("FinancialStatementId", FS[0].Id.ToString());
                    report.SetParameters(parameter);
                }
           
           
           
            
            //Here we will add the search parameter
            Byte[] mybytes = report.Render("PDF");

            return mybytes;
        }
    }
}
