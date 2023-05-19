using CRL.Infrastructure.Configuration;
using CRL.Model.Payments;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftReportGenerators
{
    public class MFSBatchDetailsReportBuilder : BatchDetailsReportBuilder
    {
        public override byte[] GenerateVerificationReport()
        {
            //Generate the fileupload
            LocalReport report = new LocalReport();



            report.ReportPath = report.ReportPath = Constants.GetReportPath + "Payment\\BatchDetails.rdlc";
                //Set the report parameters
            ReportDataSource rdsPostpaidClientTotalExpenditure = new ReportDataSource() { Name = "dsPostpaidClientTotalExpenditure", Value = dsPostpaidClientTotalExpenditure };
            ReportDataSource rdsSettledPayments = new ReportDataSource() { Name = "dsSettledPayments", Value = dsSettledPayments };
            ReportDataSource rdsClientExpenditureByTransaction = new ReportDataSource() { Name = "dsClientExpenditureByTransaction", Value = dsClientExpenditureByTransaction };
            ReportDataSource rdsAccountBatchView = new ReportDataSource() { Name = "dsAccountBatchView", Value = dsAccountBatchView };


            report.DataSources.Add(rdsPostpaidClientTotalExpenditure);
            report.DataSources.Add(rdsSettledPayments);
            report.DataSources.Add(rdsClientExpenditureByTransaction);
            report.DataSources.Add(rdsAccountBatchView);
           
           

            //string SubmittedDateTime = DateTime.Now.ToString(); ; //Please set this to the approved date            
            //ReportParameter parameterSubmittedDateTime = new ReportParameter("SubmittedDate", SubmittedDateTime);
            //report.SetParameters(parameterSubmittedDateTime);
            Byte[] mybytes = report.Render("PDF");
            return mybytes;
        }
    }
}
