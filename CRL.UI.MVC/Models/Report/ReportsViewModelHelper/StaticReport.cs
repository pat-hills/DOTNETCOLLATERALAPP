using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Reporting.WebForms;
using ReportViewerForMvc;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.UI.MVC.Models.Report.ReportsViewModelHelper
{
    public  class FSCustomQuerySubReportEvent : SubreportEventClass
    {
        public   ICollection<SecuredPartyReportView> SecuredParties { get; set; }
        public  ICollection<DebtorReportView> Debtors { get; set; }
        public  ICollection<CollateralReportView> Collaterals { get; set; }

        public override void SubReportProcessingEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            //C:\Users\Kwaku\Source\Workspaces\CollateralRegistryLiberia\\Reporting\FinancingStatement\FSDetails.rdlc
            //We need to check wether we are dealing with the otheridentification or 
            if (e.ReportPath == "_SecuredParties")
            {

                ReportDataSource rdsLender = new ReportDataSource();
                rdsLender.Name = "dsSecuredParties";  //This refers to the dataset name in the RDLC file               
                rdsLender.Value = SecuredParties;
                e.DataSources.Add(rdsLender);


            }
            else if (e.ReportPath == "_Debtors")
            {
                ReportDataSource rdsDebtor = new ReportDataSource();
                rdsDebtor.Name = "dsDebtors";  //This refers to the dataset name in the RDLC file               
                rdsDebtor.Value = Debtors;
                e.DataSources.Add(rdsDebtor);

            }
            else
            {

                ReportDataSource rdsCollateral = new ReportDataSource();
                rdsCollateral.Name = "dsCollaterals";  //This refers to the dataset name in the RDLC file               
                rdsCollateral.Value = Collaterals;
                e.DataSources.Add(rdsCollateral);
            }

        }
    }
}