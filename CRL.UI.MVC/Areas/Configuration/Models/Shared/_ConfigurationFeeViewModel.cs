using CRL.Infrastructure.Helpers;
using CRL.Model.Configuration;
using CRL.Service.Views.Configuration;
using CRL.UI.MVC.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Areas.Configuration.Models.Shared
{
    public class _ConfigurationFeeViewModel : BaseDetailViewModel
    {
        public _ConfigurationFeeViewModel()
        {
            ServiceFeeType = new List<LookUpView>();
            SelectedServiceFeeType = new List<LookUpView>();
            ConfigurationFeeView = new PerTransactionConfigurationFeeView();
            //
            LenderTypes = new List<LookUpView>();
            ConfigurationTransactionFeesViews = new List<ConfigurationTransactionFeesView>();
            LenderTransactionFeeConfigurationViews = new List<LenderTransactionFeeConfigurationView>();
        }

        public ConfigurationFeeView ConfigurationFeeView { get; set; }
        public ICollection<LookUpView> SelectedServiceFeeType { get; set; }
        public ICollection<LookUpView> ServiceFeeType { get; set; }
        //
        [Required (ErrorMessage="Please select a client type")]
        public int LenderTypeId { get; set; }
        [Display (Name="Public Search")]
        public decimal PublicSearch { get; set; }
        public int PublicSearchId { get; set; }
        [Display(Name = "Public Generate Search Certificate")]
        public decimal PublicGenerateCertificate { get; set; }
        public int PublicGenerateCertificateId { get; set; }
        public ICollection<LookUpView> LenderTypes { get; set; }
        public IList<ConfigurationTransactionFeesView> ConfigurationTransactionFeesViews { get; set; }
        public IList<LenderTransactionFeeConfigurationView> LenderTransactionFeeConfigurationViews { get; set; }
    }
}