using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.UI.MVC.Common;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
  

    public class _PaymentJqGridViewModel : BaseSearchFilterViewModel
    {
        public _PaymentJqGridViewModel()
            : base()
        {

        }
        public Dictionary<string, string> PaymentType { get; set; }
        public int ShowType { get; set; }
        //public Dictionary<string, string> FinancialStatementLoanType { get; set; }
        //public Dictionary<string, string> CollateralTypes { get; set; }
        
    }
}