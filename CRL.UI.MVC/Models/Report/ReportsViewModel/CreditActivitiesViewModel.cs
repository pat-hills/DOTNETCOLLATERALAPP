using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Infrastructure.Helpers;

using CRL.Service.Views.Payments;

using CRL.Model.ModelViews.Payments;
using CRL.Model.Payments;

namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class CreditActivitiesViewModel : ReportBaseViewModel
    {
        public CreditActivitiesViewModel()
            : base()
        {
            AccountTypeTransactionList = new List<SelectListItem>();
            AccountTypeTransactionList.Add(new SelectListItem { Text = "Payment", Value = "1" });
            //AccountTypeTransactionList.Add(new SelectListItem { Text = "Reversal Payment", Value = "2" });
            AccountTypeTransactionList.Add(new SelectListItem { Text = "Settlement Payment", Value = "4" });
            AccountTypeTransactionList.Add(new SelectListItem { Text = "Charged Fee", Value = "3" });

            CreditDebitList = new List<SelectListItem>();
            CreditDebitList.Add(new SelectListItem { Text = "Cr", Value = "1" });
            CreditDebitList.Add(new SelectListItem { Text = "Dr", Value = "2" });

            SettlementList = new List<SelectListItem>();
            SettlementList.Add(new SelectListItem { Text = "Outstanding postpaid transactions", Value = "1" });
            SettlementList.Add(new SelectListItem { Text = "Settled postpaid transactions", Value = "2" });
            

              ServiceFeeList = new List<SelectListItem>();
            ServiceFeeList.Add(new SelectListItem { Text = "Registration of financing statement", Value = "1" });
            ServiceFeeList.Add(new SelectListItem { Text = "Upate of financing statement", Value = "2" });
            ServiceFeeList.Add(new SelectListItem { Text = "Obtain certified search result", Value = "6" });


            LimitToList = new List<SelectListItem>();
            LimitToList.Add(new SelectListItem { Text = "My transactions only", Value = "1" });
            LimitToList.Add(new SelectListItem { Text = "Include transactions of my postpaid clients", Value = "2" });
            LimitToList.Add(new SelectListItem { Text = "Transactions of my postpaid clients only", Value = "6" });
      
        }
        public ICollection<CreditActivityView> CreditActivitiesView { get; set; }
        public DateRange EntryDate { get; set; }
         [Display(Name = "Charged Fee")]
        public ServiceFees? ServiceFeeTypeId { get; set; }
         [Display(Name = "CR / DR")]
        public System.Nullable<CreditOrDebit> CreditOrDebit { get; set; } //Filter
        [Display(Name="Transaction Type")]
        public System.Nullable<AccountTransactionCategory> AccountTypeTransactionId { get; set; }
        public string Narration { get; set; }
        public int? MembershipId { get; set; }
        [Display(Name = "Batch No")]
        public int? AccountBatchId { get; set; }
        [Display(Name = "Reconcilation No")]
        public int? AccountReconcileId { get; set; }
         [Display(Name = "Client Name")]
        public string ClientName { get; set; }
         [Display(Name = "Postpaid Transaction Type")]
         public int? SettlementType { get; set; }
         [Display(Name = "Limit To")]
         public int LimitTo { get; set; }
         public bool InBatchMode { get; set; }
         [Display(Name = "Batch No")]
         public int? BatchId { get; set; }
        public List<SelectListItem> AccountTypeTransactionList { get; set; }
        public List<SelectListItem> CreditDebitList { get; set; }
        public List<SelectListItem> ServiceFeeList { get; set; }
        public List<SelectListItem> SettlementList { get; set; }
        public List<SelectListItem> LimitToList { get; set; }

    }
}