using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CRL.Service.Views.FinancialStatement;
using CRL.UI.MVC.Areas.Search.Models.ViewPageModels;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.Messaging;

namespace CRL.UI.MVC.Areas.Search.Models
{
    public class SearchViewModel
    {
        public SearchViewModel()
        {
            SendAsMail = true;
            _SearchResultJqGridViewModel = new _SearchResultJqGridViewModel();
            CACSearch = new CACSearch();
        }
        public _SearchResultJqGridViewModel _SearchResultJqGridViewModel { get; set; }
        public bool isNonLegalEffect { get; set; }       
        public bool IsCertified { get; set; }
        public bool SendAsMail { get; set; }      
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string PublicUserEmail { get; set; }
        public bool IsViewMode { get; set; }
        public string ViewAndGenerateReport { get; set; }
        public string RegistrationNo { get; set; }
        public bool IsCACResults { get; set; }
        public CACSearch CACSearch { get; set; }
        
    }
}