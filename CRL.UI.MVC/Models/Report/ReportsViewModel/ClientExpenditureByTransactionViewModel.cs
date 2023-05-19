using CRL.Model.ModelViews.Payments;

using CRL.Service.Views.Payments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRL.UI.MVC.Models.Report.ReportsViewModel
{
    public class ClientExpenditureByTransactionViewModel : ReportBaseViewModel
    {
        public ClientExpenditureByTransactionViewModel()
            : base()
        {
        }
        public ICollection<ExpenditureByTransactionView> ClientExpenditureByTransaction { get; set; }
        [Display(Name = "Financial Institution")]
        public int? InstitutionId { get; set; }

        public SelectList ClientList { get; set; }
    }
}