using CRL.Model.ModelViews.Search;
using CRL.Model.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class SearchesViewModel: ReportBaseViewModel
    {
        public SearchesViewModel()
            : base()
        {
            GeneratedReportTypeList = new List<SelectListItem>();
            GeneratedReportTypeList.Add(new SelectListItem { Text = "Uncertified Report", Value = "1" });
            GeneratedReportTypeList.Add(new SelectListItem { Text = "Certified Report", Value = "2" });

            SearchTypeList = new List<SelectListItem>();
            SearchTypeList.Add(new SelectListItem { Text = "Legally Effective Search", Value = "1" });
            SearchTypeList.Add(new SelectListItem { Text = "Flexible Search", Value = "2" });

            ReturnedResultsList = new List<SelectListItem>();
            ReturnedResultsList.Add(new SelectListItem { Text = "Records Found", Value = "1" });
            ReturnedResultsList.Add(new SelectListItem { Text = "Empty Result", Value = "2" });

           

            ClientTypeList = new List<SelectListItem>();
            ClientTypeList.Add(new SelectListItem { Text = "Registry Users", Value = "1" });
            ClientTypeList.Add(new SelectListItem { Text = "Financial Institution Users", Value = "2" });
            ClientTypeList.Add(new SelectListItem { Text = "Public Users", Value = "3" });

            
        }
        [Display(Name="Search Code")]
        public string SearchCode { get; set; }
        public int? MembershipId { get; set; }
        [Display(Name = "Public User Code")]
        public string PublicUserCode { get; set; }
         [Display(Name = "Financial Institution")]
        public string ClientName { get; set; }
         [Display(Name = "User")]
         public string Username { get; set; }
        [Display(Name = "User Type")]
        public int? ClientType { get; set; }
        [Display(Name = "Search report type")]
        public short? GeneratedReportType { get; set; }
        [Display(Name = "Search type")]
        public short? SearchType { get; set; }
        [Display(Name = "Search Result Type")]
        public short? ReturnedResults { get; set; }
        [Display(Name = "Limit to")]
        public bool LimitTo { get; set; }

        public List<SelectListItem> GeneratedReportTypeList { get; set; }
        public List<SelectListItem> SearchTypeList { get; set; }
        public List<SelectListItem> ReturnedResultsList { get; set; }
        public List<SelectListItem> ClientTypeList { get; set; }
        public List<SelectListItem> LimitToList { get; set; }

        public ICollection<SearchRequestGridView> SearchesView { get; set; }

        
    }
}