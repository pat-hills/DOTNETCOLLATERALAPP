using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Service.Views.FinancialStatement;
using CRL.Model.ModelViews.Statistics;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class FSStatViewModel: ReportBaseViewModel
    {
        public FSStatViewModel()
        {
            GroupByList = new List<SelectListItem>();
            GroupByList.Add(new SelectListItem { Text = "Debtor Type", Value = "1" });
            GroupByList.Add(new SelectListItem { Text = "Secured Creditor Type", Value = "2" });
            GroupByList.Add(new SelectListItem { Text = "Registrant", Value = "6" });
            GroupByList.Add(new SelectListItem { Text = "Registrant Type", Value = "8" });
            GroupByList.Add(new SelectListItem { Text = "Debtor State", Value = "4" });
            GroupByList.Add(new SelectListItem { Text = "Secured Creditor State", Value = "5" });
            GroupByList.Add(new SelectListItem { Text = "Currency", Value = "10" });
            //GroupByList.Add(new SelectListItem { Text = "Transaction Type", Value = "9" });

            FSStateList = new List<SelectListItem>();

            FSStateList.Add(new SelectListItem { Text = "All registered financing statements", Value = "1", Selected = true });
            FSStateList.Add(new SelectListItem { Text = "Exclude discharged and expired financing statements", Value = "2" });
            
            FSStateTypeList = new List<SelectListItem>();
            FSStateTypeList.Add(new SelectListItem { Text = "Current Values of FS", Value = "1", Selected = true });
            FSStateTypeList.Add(new SelectListItem { Text = "Original Values of FS", Value = "2" });
        }
        public ICollection<CountOfItemStatView> FSStatView { get; set; }
        public ICollection<ValueOfFSStatView> ValueOfFSStatView { get; set; }
        public List<SelectListItem> FSStateList { get; set; }

        public SelectList Countys { get; set; }
        public ICollection<LKLGAView> Lgas { get; set; }
        public List<SelectListItem> GroupByList { get; set; }
           [Display(Name = "Group by")]
        public int GroupedBy{get;set;}
        [Display(Name="Women Owned")]
        public bool LimitToWomenOwned { get; set; }
        [Display(Name = "Include Graph")]
        public bool IncludeGraph { get; set; }
        [Display(Name = "Limit to Financing Statement Status")]
        public int FSState { get; set; }
        [Display(Name = "State")]
        public int? County { get; set; }
        [Display(Name = "LGA")]
        public int? Lga { get; set; }
        [Display(Name = "Financing Statement State")]
        public int FSStateType { get; set; }
        public List<SelectListItem> FSStateTypeList { get; set; }
        public List<SelectListItem> Clients { get; set; }
        [Display(Name = "Financial Institution")]
        public int? ClientId { get; set; }

      
        
    }
}