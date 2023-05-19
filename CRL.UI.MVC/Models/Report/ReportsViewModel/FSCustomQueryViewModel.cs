using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CRL.Service.Views.FinancialStatement;
using Microsoft.Reporting.WebForms;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViews.Statistics;

namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class FSCustomQueryViewModel : ReportBaseViewModel
    {
        public FSCustomQueryViewModel()
        {
            CollateralTypeList = new List<SelectListItem>();
            CollateralTypeList.Add(new SelectListItem { Text = "Consumer Goods", Value = "1" });
            CollateralTypeList.Add(new SelectListItem { Text = "Commercial Collateral", Value = "2" });
            CollateralTypeList.Add(new SelectListItem { Text = "Both", Value = "3" });

            FSTransactionTypeList = new List<SelectListItem>();
            FSTransactionTypeList.Add(new SelectListItem { Text = "Security Interest", Value = "1" });
            FSTransactionTypeList.Add(new SelectListItem { Text = "Finance Lease", Value = "2" });
            FSTransactionTypeList.Add(new SelectListItem { Text = "Lien", Value = "3" });

            FSLoanTypeList = new List<SelectListItem>();
            FSLoanTypeList.Add(new SelectListItem { Text = "Consumer Loan", Value = "1" });
            FSLoanTypeList.Add(new SelectListItem { Text = "Personal Loan", Value = "2" });
            FSLoanTypeList.Add(new SelectListItem { Text = "Commercial Loan", Value = "3" });
            FSLoanTypeList.Add(new SelectListItem { Text = "Line of Credit", Value = "4" });
            FSLoanTypeList.Add(new SelectListItem { Text = "Facility", Value = "5" });
            FSLoanTypeList.Add(new SelectListItem { Text = "Agricultural Loan", Value = "6" });
            FSLoanTypeList.Add(new SelectListItem { Text = "Federal Government Loan (Agriculture, MSME, Mining, etc.)", Value = "7" });
            FSLoanTypeList.Add(new SelectListItem { Text = "State Government Loan (Agriculture, MSME, Mining, etc.)", Value = "8" });
            FSLoanTypeList.Add(new SelectListItem { Text = "Local Government Loan (Agriculture, MSME, Mining, etc.)", Value = "9" });
            FSLoanTypeList.Add(new SelectListItem { Text = "Financial Statement Own Loan (Agriculture, MSME, Mining, etc.)", Value = "10" });

            MajorityOwnershipList = new List<SelectListItem>();
            MajorityOwnershipList.Add(new SelectListItem { Text = "Majority Male", Value = "1" });
            MajorityOwnershipList.Add(new SelectListItem { Text = "Majority Female", Value = "2" });
            MajorityOwnershipList.Add(new SelectListItem { Text = "Equal Distribution", Value = "3" });


            ExistingRelationshipList = new List<SelectListItem>();
            ExistingRelationshipList.Add(new SelectListItem { Text = "Existing relationship with debtor", Value = "1" });
            ExistingRelationshipList.Add(new SelectListItem { Text = "New relationship with debtor", Value = "2" });

            ReportTypeList = new List<SelectListItem>();
            ReportTypeList.Add(new SelectListItem { Text = "Financing Statement View", Value = "1" });
            ReportTypeList.Add(new SelectListItem { Text = "Statistical View", Value = "2" });
              ReportTypeList.Add(new SelectListItem { Text = "Raw Statistical View", Value = "3" });
           
            StatisticalReportTypeList = new List<SelectListItem>();
            StatisticalReportTypeList.Add(new SelectListItem { Text = "Value and No of Financing Statement", Value = "1" });
            StatisticalReportTypeList.Add(new SelectListItem { Text = "No of Financing Statement only", Value = "2" });
            StatisticalReportTypeList.Add(new SelectListItem { Text = "Value of Financing Statement only", Value = "3" });

            ShowSectionList = new List<SelectListItem>();
            ShowSectionList.Add(new SelectListItem { Text = "Secured Creditor", Value = "1" , Selected=true});
            ShowSectionList.Add(new SelectListItem { Text = "Debtor", Value = "2" , Selected =true});
            ShowSectionList.Add(new SelectListItem { Text = "Collateral", Value = "3", Selected =true });

            LimitToFirstItemList = new List<SelectListItem>();
            LimitToFirstItemList.Add(new SelectListItem { Text = "Limit debtor filter(s) to first debtor of financing statement", Value = "1", Selected = true });
            LimitToFirstItemList.Add(new SelectListItem { Text = "Limit secured creditor filter(s) to first secured creditor of  financing statement", Value = "2", Selected = true });
            

            GroupByList = new List<SelectListItem>();
            GroupByList.Add(new SelectListItem { Text = "Debtor Type", Value = "1" });
            GroupByList.Add(new SelectListItem { Text = "Secured Creditor Type", Value = "2" });
            GroupByList.Add(new SelectListItem { Text = "Debtor State", Value = "4" });
            GroupByList.Add(new SelectListItem { Text = "Secured Creditor State", Value = "5" });
            GroupByList.Add(new SelectListItem { Text = "Registrant", Value = "6" });
            GroupByList.Add(new SelectListItem { Text = "Registrant Type", Value = "8" });

            FSStateList = new List<SelectListItem> ();
            
            FSStateList.Add(new SelectListItem{ Text = "All registered financing statements", Value = "1" ,Selected=true });
             FSStateList.Add(new SelectListItem{ Text = "Exclude discharged and expired financing statements", Value = "2" });

             FSStateTypeList = new List<SelectListItem>();
             FSStateTypeList.Add(new SelectListItem { Text = "Current Values of FS", Value = "1", Selected = true });
             FSStateTypeList.Add(new SelectListItem { Text = "Original Values of FS", Value = "2" });
        }
     
        public ICollection<FSGridView> FSGridView { get; set; }
        public ICollection<FSCustomReportView> FSCustomReportView { get; set; }
        public ICollection<FSCustomQueryStatView> FSCustomQueryStatView { get; set; }
        public List<SelectListItem> Clients { get; set; }
        [Display(Name = "Financial Institution")]
        public int? ClientId { get; set; }

        [Display(Name = "Financing Statement State")]
        public int FSStateType { get; set; }
        public List<SelectListItem> FSStateTypeList { get; set; }

        [Display(Name = "Registration No")]
        public string RegistrationNo { get; set; }
        [Display(Name = "Transaction Type")]
        public int[] FinancialStatementTransactionTypeId { get; set; }
        [Display(Name = "Loan Type")]
        public int[] FinancialStatementLoanTypeId { get; set; }
        [Display(Name = "Collateral Type")]
        public int[] CollateralTypeId { get; set; }
        [Display(Name = "Secured Creditor Type")]
        public int[] SecuredPartyTypeId { get; set; }
        [Display(Name = "Debtor Type")]
        public int[] DebtorTypeId { get; set; }
        [Display(Name = "Maximum Amount Currency")]
        public int[] MaximumCurrency { get; set; }
        [Display(Name = "Debtor Country")]
        public int?[] DebtorCountryId { get; set; }
        [Display(Name = "Debtor State")]
        public int[] DebtorCountyId { get; set; }
        [Display(Name = "Debtor Nationality")]
        public int[] DebtorNationalityId { get; set; }
        [Display(Name = "Collateral Type")]
        public int[] CollateralSubtypeId { get; set; }
        [Display(Name = "Sector of Operation")]
        public int[] SectorOfOperationId { get; set; }
         [Display(Name = "Majority Ownership")]
        public short? MajorityOwnershipId { get; set; }
         [Display(Name = "Existing Relationship With Debtor")]
        public short? ExistingRelationshipId { get; set; }
         [Display(Name = "Select one or more item to show in report details")]
         public int[] ShowSection { get; set; }
            [Display(Name = "Select one or more items below to filter financing statements with multiple debtors or secured parties by only the first debtor or secured creditor respectively")]
         public int[] LimitToFirstItem { get; set; }
         //public bool ShowLender { get; set; }
         //public bool ShowCollateral { get; set; }
         //public bool ShowDebtor { get; set; }

         [Display(Name = "Group by")]
         public int GroupedBy { get; set; }
        [Display(Name = "Statistical report type")]
         public int StatisticalReportType { get; set; }
        [Display(Name = "Report type")]
         public int ReportType { get; set; }

         [Display(Name = "Limit to Financing Statement Status")]
        public int FSState { get; set; }
       
        //[Display(Name = "Limit to")]
        //public short ShowType { get; set; }

        public List<SelectListItem> GroupByList { get; set; }
        public List<SelectListItem> FSStateList { get; set; }
        public List<SelectListItem> ShowSectionList { get; set; }
        public List<SelectListItem> LimitToFirstItemList { get; set; }
        public List<SelectListItem> CollateralTypeList { get; set; }
        public List<SelectListItem> ReportTypeList { get; set; }
        public List<SelectListItem> StatisticalReportTypeList { get; set; }
        public List<SelectListItem> FSTransactionTypeList { get; set; }
        public List<SelectListItem> FSLoanTypeList { get; set; }
        public List<SelectListItem> MaximumAmountSecuredCurrencyList { get; set; }
        public List<SelectListItem> IdentificationCardTypeList { get; set; }
        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Countys { get; set; }
        public List<SelectListItem> Nationalities { get; set; }
        public List<SelectListItem> CollateralSubTypesList { get; set; }
        public List<SelectListItem> SectorOfOperationList { get; set; }
        public List<SelectListItem> DebtorTypes { get; set; }
        public List<SelectListItem> SecuredPartyList { get; set; }
        public List<SelectListItem> MajorityOwnershipList { get; set; }
        public List<SelectListItem> ExistingRelationshipList { get; set; }

      

      


    }
}