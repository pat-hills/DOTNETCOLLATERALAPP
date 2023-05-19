using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.UI.MVC.Common;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class _InterSwitchJqGrid : BaseSearchFilterViewModel
    {

        public _InterSwitchJqGrid():base()
        {
            TransactionLogTypes = new List<TransactionLogType>();
        }
        
        public readonly ICollection<TransactionLogType> TransactionLogTypes;
        public int SelectedTransactionLogTypeId { get; set; }
        public IEnumerable<SelectListItem> TransactionLogTypeItems
        {
            get { return new SelectList(TransactionLogTypes, "Id", "Name"); }
        }
    }

    public class TransactionLogType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}