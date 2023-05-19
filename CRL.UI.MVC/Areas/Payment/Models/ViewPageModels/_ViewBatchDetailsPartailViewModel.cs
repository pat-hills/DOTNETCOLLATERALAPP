using System;
using System.ComponentModel.DataAnnotations;


namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class _ViewBatchDetailsPartailViewModel
    {
        public int BatchNo { get; set; }
        public string BatchName { get; set; }
        public string BatchComment { get; set; }
        public string ReconcileComment { get; set; }
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public decimal? TotalOustandinBatchAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public decimal? TotalBatchExpenses { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public decimal? TotalSetltement { get; set; }

    }
}
