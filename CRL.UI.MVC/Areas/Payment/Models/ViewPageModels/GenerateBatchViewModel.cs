using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class GenerateBatchViewModel
    {
        public GenerateBatchViewModel()
        {
            _ListOfTransactionsJqGrid = new _ListOfTransactionsJqGrid();

            ActivateGrid = false;
        }
        public _ListOfTransactionsJqGrid _ListOfTransactionsJqGrid { get; set; }
        public bool ActivateGrid { get; set; }
       
    }
}