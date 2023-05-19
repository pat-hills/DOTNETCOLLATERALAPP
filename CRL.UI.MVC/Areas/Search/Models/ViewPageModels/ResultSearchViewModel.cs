using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.UI.MVC.Areas.FinancialStatement.Models.ViewPageModels;

namespace CRL.UI.MVC.Areas.Search.Models.ViewPageModels
{
    public class ResultSearchViewModel : SearchViewModel
    {
        public ResultSearchViewModel() : base()
        {
            fsViewModel = new FSViewModel();
        }
        public int SearchId { get; set; }
        public FSViewModel fsViewModel { get; set; }
    }
}