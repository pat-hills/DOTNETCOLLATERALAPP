using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.UI.MVC.Common;

namespace CRL.UI.MVC.Areas.Search.Models.ViewPageModels
{
    
    public class _FSSearchesViewModel : BaseSearchFilterViewModel
    {
        public _FSSearchesViewModel()
            : base()
        {

        }
        public string PublicUserSecurityCode { get; set; }
        //public Dictionary<string, string> FinancialStatementActivityType { get; set; }
        //public bool InRequestMode { get; set; }
    }
}