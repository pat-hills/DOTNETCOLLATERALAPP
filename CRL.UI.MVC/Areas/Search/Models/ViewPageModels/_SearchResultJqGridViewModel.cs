using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CRL.Infrastructure.Helpers;
using CRL.Service.Views.FinancialStatement;

using System.Web.Mvc;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.Search;
using CRL.Model.Messaging;
using CRL.Service.Common;
using CRL.Model.ModelViews.Search;
using CRL.Service.Views.Search;

namespace CRL.UI.MVC.Areas.Search.Models.ViewPageModels
{
    public class _SearchResultJqGridViewModel:ViewBase
    {
        public _SearchResultJqGridViewModel()
        {
            CACSearch = new CACSearch();
        }
        public string UniqueIdentifier { get; set; }       
        public int SearchId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SearchDate { get; set; }
        public int SearchResultCount { get; set; }
        public int CurrentPage { get; set; }
        public int Pages { get; set; }
        [Display(Name = "Payment Receipt No")]
        public string PublicUserReceiptNo { get; set; }
        public bool isPayable { get; set; }
        public bool isPayableGenerateReport { get; set; }
        [DataType(DataType.Date )]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> StartDate { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> EndDate { get; set; }
        public SearchParam SearchParam { get; set; }
        public CACSearch CACSearch { get; set; }
        public FSGridView[] FSGridView{get;set;}
        public SearchResultView[] SearchResultView { get; set; }
        public  int SelectedId { get; set; }
        public SelectList BusinessPrefixes { get; set; }
        public Dictionary<string, string> FinancialStatementTransactionTypes { get; set; }
        public Dictionary<string, string> FinancialStatementLoanType { get; set; }
        public Dictionary<string, string> CollateralTypes { get; set; }
        public bool Load { get; set; }
        public bool isNonLegalEffect { get; set; }
        public bool DoNotGenerateSearchAlert { get; set; }
        public bool IsAfterSearch { get; set; }
        public string RegistrationNo { get; set; }
        public int? AssignedFsId { get; set; }
        public bool IsPreviousSearch { get; set; }
        public bool HasGeneratedReport { get; set; }
        public ICollection<string> ExpiredSearchResults { get; set; }
        public bool HasExpiredAllResults { get; set; }
        public ICollection<string> GeneratedSearchReportsReNos { get; set; }
        public IEnumerable<SearchReportView> SearchReports { get; set; }

        [Display(Name = "Full name")]
        public string PublicUserName { get; set; }
     
        [Display(Name = "Email Address")]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter a correct email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [Required]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required]
        [StringLength(11,ErrorMessage="BVN must be 11 digits")]
        [Display(Name = "BVN")]
        public string BVN {get;set;}

        public DateRange GenerateDateRange()
        {
            if (this.StartDate != null && this.EndDate != null)
            {
                DateRange _dateRange = new Infrastructure.Helpers.DateRange((DateTime)this.StartDate, (DateTime)this.EndDate);
                return _dateRange;
            }
            else if (this.StartDate != null && this.EndDate == null)
            {
                DateRange _dateRange = new Infrastructure.Helpers.DateRange((DateTime)this.StartDate);
                return _dateRange;
            }
            else if (this.StartDate == null && this.EndDate != null)
            {
                DateRange _dateRange = new Infrastructure.Helpers.DateRange((DateTime)this.EndDate );
                return _dateRange;
            }
            else
                return null;
        }
    }
}