using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class AccountBatchesIndexViewModel
    {
        public AccountBatchesIndexViewModel()
        {
            _ViewAccountBatchesJqGrid = new _ViewAccountBatchesJqGrid();
        }

        public _ViewAccountBatchesJqGrid _ViewAccountBatchesJqGrid { get; set; }

    }
}