using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Configuration;

using Microsoft.Reporting.WebForms;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViews.Amendment;

namespace CRL.Service.ReportGenerator
{
   

    public class FSVerificationReportGenerator
    {
        List<FSReportView> FS;
        List<SecuredPartyReportView> Lenders;
        List<DebtorReportView> Borrowers;
        List<CollateralReportView> Collaterals;
        List<FSActivitySummaryReportView> Activities;
        List<OtherIdentificationReportView > OtherIdentifications;



        public FSVerificationReportGenerator(List<FSReportView> _fs, List<SecuredPartyReportView> _lenders, List<DebtorReportView> _borrowers,
            List<CollateralReportView> _collaterals,List<OtherIdentificationReportView > _otheridentifications=null, List<FSActivitySummaryReportView> _activities=null)
        {
            FS = _fs;
            Lenders = _lenders;
            Borrowers = _borrowers;
            Collaterals = _collaterals;
            OtherIdentifications = _otheridentifications;
            Activities = _activities;


        }
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

        public Byte[] GenerateReport()
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
            
            report.SetParameters(parameter);
            report.SetParameters(parameter2);
            Byte[] mybytes = report.Render("PDF");
            return mybytes;

           
         


         

          

           
        }
    }
}
