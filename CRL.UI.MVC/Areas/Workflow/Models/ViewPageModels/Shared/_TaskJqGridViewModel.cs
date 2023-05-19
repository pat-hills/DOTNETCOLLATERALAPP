using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.UI.MVC.Common;

namespace CRL.UI.MVC.Areas.Workflow.Models.ViewPageModels.Shared
{


    public class _TaskJqGridViewModel : BaseSearchFilterViewModel
    {
        public _TaskJqGridViewModel()
            : base()
        {

        }
        //public Dictionary<string, string> FinancialStatementTransactionTypes { get; set; }
        //public Dictionary<string, string> FinancialStatementLoanType { get; set; }
        public Dictionary<string, string> TaskType { get; set; }
        //public bool InRequestMode { get; set; }
    }
}