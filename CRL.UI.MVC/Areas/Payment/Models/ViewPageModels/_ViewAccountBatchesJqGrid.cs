using CRL.UI.MVC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class _ViewAccountBatchesJqGrid : BaseSearchFilterViewModel
    {
        public _ViewAccountBatchesJqGrid()
            : base()
        {
            ReconciliationTypes = new List<ReconciliationType>();
        }
        public Dictionary<string, string> BatchStatusTypes { get; set; }
        public readonly ICollection<ReconciliationType> ReconciliationTypes;
        
    }

    public class ReconciliationType
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
}