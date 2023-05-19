using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Service.Views.FinancialStatement;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.Messaging;

namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class FSListViewModel : ReportBaseViewModel
    {
        public FSListViewModel()
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
            FSLoanTypeList.Add(new SelectListItem { Text = "Loan", Value = "1" });
            FSLoanTypeList.Add(new SelectListItem { Text = "Line of Credit", Value = "2" });
            FSLoanTypeList.Add(new SelectListItem { Text = "Both", Value = "3" });

            ShowTypeList = new List<SelectListItem>();
            ShowTypeList.Add(new SelectListItem { Text = "Active financing statements", Value = "1" });
            ShowTypeList.Add(new SelectListItem { Text = "Active financing statements (10 days to expire)", Value = "3" });
            ShowTypeList.Add(new SelectListItem { Text = "Discharged financing statements", Value = "2" });
            ShowTypeList.Add(new SelectListItem { Text = "Expired financing statements", Value = "4" });
            ShowTypeList.Add(new SelectListItem { Text = "All approved financing statements", Value = "5" });
            ShowTypeList.Add(new SelectListItem { Text = "All denied financing statements", Value = "7" });
            ShowTypeList.Add(new SelectListItem { Text = "All pending financing statements", Value = "6" });



        }
        public ICollection<FSGridView> FSGridView { get; set; }
        [Display(Name = "Registration No")]
        public string RegistrationNo { get; set; }
        [Display(Name = "Transaction Type")]
        public int? FinancialStatementTransactionTypeId { get; set; }
        [Display(Name = "Loan Type")]
        public int? FinancialStatementLoanTypeId { get; set; }
        [Display(Name = "Collateral Type")]
        public int? CollateralTypeId { get; set; }
        [Display(Name = "Limit to")]
        public FilterFinancingStatement ShowType { get; set; }

        public List<SelectListItem> CollateralTypeList { get; set; }
        public List<SelectListItem> FSTransactionTypeList { get; set; }
        public List<SelectListItem> FSLoanTypeList { get; set; }
        public List<SelectListItem> ShowTypeList { get; set; }
        public List<SelectListItem> Clients { get; set; }
        [Display(Name = "Financial Institution")]
        public int? ClientId { get; set; }


    }
}