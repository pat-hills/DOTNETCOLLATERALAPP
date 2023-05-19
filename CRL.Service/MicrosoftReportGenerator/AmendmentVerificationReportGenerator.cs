using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Configuration;
using CRL.Model.FS.Enums;

using CRL.Service.Views.FinancialStatement;
using Microsoft.Reporting.WebForms;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViews.Amendment;

namespace CRL.Service.MicrosoftReportGenerator
{

    public class AmendmentVerificationReportGenerator
    {
        List<FSReportView> FS;
        List<DischargeActivityReportView> DischargeActivity;
        List<AssignmentActivityReportView> AssignmentActivity;
        List<ActivityReportView> Activity;
        List<SecuredPartyReportView> Lenders;
        List<DebtorReportView> Borrowers;
        List<CollateralReportView> Collaterals;
        List<FSActivitySummaryReportView> Activities;
        List<OtherIdentificationReportView> OtherIdentifications;
        List<ChangeDescriptionView> ChangeDescription;
        List<CollateralReportView> DischargedCollaterals;
        List<SubordinatingPartyReportView> SubordinatingParty;
        List<ClientReportView> AssignedParty;
        
        FinancialStatementActivityCategory activityType;



        public AmendmentVerificationReportGenerator(List<FSReportView> _fs, List<SecuredPartyReportView> _lenders, List<DebtorReportView> _borrowers,
            List<CollateralReportView> _collaterals, List<ChangeDescriptionView> _changedescription, List<OtherIdentificationReportView> _otheridentifications, List<ActivityReportView> _activity)
        {
            FS = _fs;
            Lenders = _lenders;
            Borrowers = _borrowers;
            Collaterals = _collaterals;
            OtherIdentifications = _otheridentifications;
            ChangeDescription = _changedescription;
            Activity = _activity;
            activityType = FinancialStatementActivityCategory.Update;
            //Activities = _activities;


        }

        public AmendmentVerificationReportGenerator(List<FSReportView> _fs, List<SecuredPartyReportView> _lenders, List<DebtorReportView> _borrowers,
            List<CollateralReportView> _collaterals, List<OtherIdentificationReportView> _otheridentifications, List<DischargeActivityReportView> _dischargeactivity, List<CollateralReportView> _dischargedcoll)
        {
            FS = _fs;
            Lenders = _lenders;
            Borrowers = _borrowers;
            Collaterals = _collaterals;
            OtherIdentifications = _otheridentifications;
            DischargedCollaterals = _dischargedcoll;
            DischargeActivity = _dischargeactivity;
            activityType = _dischargeactivity[0].DischargeType == 1 ? FinancialStatementActivityCategory.FullDicharge  : FinancialStatementActivityCategory.PartialDischarge;
            //Activities = _activities;


        }
        public AmendmentVerificationReportGenerator(List<FSReportView> _fs, List<SecuredPartyReportView> _lenders, List<DebtorReportView> _borrowers,
           List<CollateralReportView> _collaterals, List<OtherIdentificationReportView> _otheridentifications, List<SubordinatingPartyReportView> _subordinatingparty, List<ActivityReportView > _activity)
        {
            FS = _fs;
            Lenders = _lenders;
            Borrowers = _borrowers;
            Collaterals = _collaterals;
            OtherIdentifications = _otheridentifications;
            SubordinatingParty  = _subordinatingparty;
            Activity = _activity;
            activityType = FinancialStatementActivityCategory.Subordination;
            


        }
        public AmendmentVerificationReportGenerator(List<FSReportView> _fs, List<SecuredPartyReportView> _lenders, List<DebtorReportView> _borrowers,
          List<CollateralReportView> _collaterals, List<OtherIdentificationReportView> _otheridentifications, List<ClientReportView > _assigningparty,List<AssignmentActivityReportView> _assignmentactivity)
        {
            FS = _fs;
            Lenders = _lenders;
            Borrowers = _borrowers;
            Collaterals = _collaterals;
            OtherIdentifications = _otheridentifications;
            AssignedParty = _assigningparty;
            
            AssignmentActivity  = _assignmentactivity ;
            activityType = _assignmentactivity[0].AssignmentType  == 1 ? FinancialStatementActivityCategory.FullAssignment  : FinancialStatementActivityCategory.PartialAssignment ;



        }
        void SubReportProcessingEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            //C:\Users\Kwaku\Source\Workspaces\CollateralRegistryLiberia\\Reporting\FinancingStatement\FSDetails.rdlc
            //We need to check wether we are dealing with the otheridentification or 
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

        }

        public Byte[] GenerateReport()
        {
            //Generate the fileupload
            LocalReport report = new LocalReport();

            report.SubreportProcessing += new
           SubreportProcessingEventHandler(SubReportProcessingEventHandler);
            if (activityType == FinancialStatementActivityCategory.Update)
            {
                report.ReportPath = report.ReportPath = Constants.GetReportPath + "FinancingStatement\\VerificationReportFSUpdate.rdlc";
                //Set the report parameters
                ReportDataSource rdsActivity = new ReportDataSource();
                rdsActivity.Name = "Activity";
                rdsActivity.Value = Activity;
                ReportDataSource rdsChangeDescription = new ReportDataSource();
                rdsChangeDescription.Value = ChangeDescription;
                rdsChangeDescription.Name = "ChangeDescription";
                report.DataSources.Add(rdsChangeDescription);
                report.DataSources.Add(rdsActivity);
            }
            else if (activityType == FinancialStatementActivityCategory.FullDicharge || activityType == FinancialStatementActivityCategory.PartialDischarge)
            {

                report.ReportPath = report.ReportPath = Constants.GetReportPath + "FinancingStatement\\VerificationReportFSDischarge.rdlc";
                //Set the report parameters
                ReportDataSource rdsDischargeActivity = new ReportDataSource();
                ReportDataSource rdsDischargedCollaterals = new ReportDataSource();
                rdsDischargeActivity.Name = "DischargeActivity";
                rdsDischargeActivity.Value = DischargeActivity;
                rdsDischargedCollaterals.Value = DischargedCollaterals;
                rdsDischargedCollaterals.Name = "DischargedCollaterals";
                ReportParameter parameter = new ReportParameter("DischargeType", DischargeActivity[0].DischargeType.ToString() );
                ReportParameter parameter2 = new ReportParameter("DischargeTypeName", DischargeActivity[0].DischargedTypeName );
                ReportParameter parameter3 = new ReportParameter("FinancialStatementId", FS[0].Id.ToString());
            
                report.SetParameters(parameter);
                report.SetParameters(parameter2);
                report.SetParameters(parameter3);
                report.DataSources.Add(rdsDischargeActivity);
                report.DataSources.Add(rdsDischargedCollaterals);

            }
            else if (activityType == FinancialStatementActivityCategory.FullAssignment || activityType == FinancialStatementActivityCategory.PartialAssignment )
            {
                report.ReportPath = report.ReportPath = Constants.GetReportPath + "FinancingStatement\\VerificationReportFSAssignment.rdlc";
                //Set the report parameters
                ReportDataSource rdsAssignmentActivity = new ReportDataSource();
                ReportDataSource rdsAssignedParty = new ReportDataSource();
                ReportParameter parameter = new ReportParameter("AssignmentType", AssignmentActivity[0].AssignmentType.ToString());
                ReportParameter parameter2 = new ReportParameter("AssignmentTypeName", AssignmentActivity[0].AssignmentTypeName);
                ReportParameter parameter3 = new ReportParameter("FinancialStatementId", FS[0].Id.ToString());
                ReportParameter parameter4 = new ReportParameter("AssignedToId", AssignmentActivity[0].AssignedPartyId.ToString () );
                ReportParameter parameter5 = new ReportParameter("AssignedFromId", AssignmentActivity[0].AssignedFromPartyId .ToString());
                rdsAssignmentActivity.Name = "AssignmentActivity";
                rdsAssignmentActivity.Value = AssignmentActivity;
                rdsAssignedParty.Value = AssignedParty;
                rdsAssignedParty.Name ="AssignedParty";
                report.SetParameters(parameter);
                report.SetParameters(parameter2);
                report.SetParameters(parameter3);
                report.SetParameters(parameter4);
                report.SetParameters(parameter5);
                report.DataSources.Add(rdsAssignmentActivity);
                report.DataSources.Add(rdsAssignedParty);

            }
            else if (activityType == FinancialStatementActivityCategory.Subordination)
            {
                report.ReportPath = report.ReportPath = Constants.GetReportPath + "FinancingStatement\\VerificationReportFSSubordinate.rdlc";
                //Set the report parameters
                ReportDataSource rdsActivity = new ReportDataSource();
                ReportDataSource rdsSubordinateParty = new ReportDataSource();
                ReportParameter parameter = new ReportParameter("FinancialStatementId", FS[0].Id.ToString());
                rdsActivity.Name = "Activity";
                rdsActivity.Value = Activity;
                rdsSubordinateParty.Value = SubordinatingParty ;
                rdsSubordinateParty.Name = "SubordinatingParty";
                report.SetParameters(parameter);
                report.DataSources.Add(rdsActivity);
                report.DataSources.Add(rdsSubordinateParty);

            }
           

            string SubmittedDateTime = DateTime.Now.ToString(); ; //Please set this to the approved date            
            ReportParameter parameterSubmittedDateTime = new ReportParameter("SubmittedDate", SubmittedDateTime);
            report.SetParameters(parameterSubmittedDateTime);
            Byte[] mybytes = report.Render("PDF");
            return mybytes;










        }
    }
}
