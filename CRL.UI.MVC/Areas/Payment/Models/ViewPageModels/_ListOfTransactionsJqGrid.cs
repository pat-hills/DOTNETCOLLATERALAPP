using CRL.UI.MVC.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class _ListOfTransactionsJqGrid : BaseSearchFilterViewModel
    {

     public   _ListOfTransactionsJqGrid()
        {
           
        
     }


        /// <summary>All = 1, Batched= 2, UnBatched = 3
        /// 
        /// </summary>
        public int? AllOrBatchedOrUnBatched { get; set; }
        public string BatchComment { get; set; }
        public int NumberOfRows { get; set; }

        public bool SelectAllOustandingTransactions { get; set; }

        public int? BatchId { get; set; }

    }
}