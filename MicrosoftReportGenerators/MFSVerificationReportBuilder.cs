using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Reporting.WebForms;
using CRL.Model.FS;
using CRL.Infrastructure.Configuration;


namespace MicrosoftReportGenerators
{

    public class MFSVerificationReportBuilder : FSDetailReportBuilder
    {
       
        public void SubReportProcessingEventHandler(object sender, SubreportProcessingEventArgs e)
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
        public override Byte[] GenerateCurrentReport(string IdentifierId)
        {
            //Generate the fileupload
            LocalReport report = new LocalReport();

            report.SubreportProcessing += new
           SubreportProcessingEventHandler(SubReportProcessingEventHandler);

            ReportDataSource rdsActivities = new ReportDataSource();
            rdsActivities.Value = Activities;
            rdsActivities.Name = "ActivitySummary";
            report.DataSources.Add(rdsActivities);


            report.ReportPath = Constants.GetReportPath + "FinancingStatement\\CurrentFS.rdlc";
            //Set the report parameters

            ReportParameter parameter = new ReportParameter("FinancialStatementId", FS[0].Id.ToString());
            report.SetParameters(parameter);


            ReportParameter parameter3 = new ReportParameter("IdentifierId", IdentifierId);
            report.SetParameters(parameter3);

            ReportParameter param = new ReportParameter("ReportGenereatedDateParameter", generatedDate.ToString("dd, MMMM, yyyy  h:mm:ss tt"));
            report.SetParameters(param);

            Byte[] mybytes = report.Render("PDF");
            return mybytes;


        }
        public override  Byte[] GenerateVerificationReport(string IdentfierId)
        {
            //Generate the fileupload
            LocalReport report = new LocalReport();
            
            report.SubreportProcessing += new
           SubreportProcessingEventHandler(SubReportProcessingEventHandler);
            report.ReportPath = Constants.GetReportPath + "FinancingStatement\\VerificationReportFSRegistration.rdlc"; 
            //Set the report parameters

            string SubmittedDateTime = DateTime.Now.ToString(); ; //Please set this to the approved date            
            ReportParameter parameter = new ReportParameter("SubmittedDate", SubmittedDateTime);
            ReportParameter parameter2 = new ReportParameter("FinancialStatementId", FS[0].Id.ToString());
            ReportParameter parameter3 = new ReportParameter("IdentifierId", IdentfierId);
            ReportParameter param = new ReportParameter("ReportGenereatedDateParameter", generatedDate.ToString("dd, MMMM, yyyy  h:mm:ss tt"));
            report.SetParameters(param);
            report.SetParameters(parameter);
            report.SetParameters(parameter2);
            report.SetParameters(parameter3);
            Byte[] mybytes = report.Render("PDF");
            return mybytes; 

           
        }
    }
}
