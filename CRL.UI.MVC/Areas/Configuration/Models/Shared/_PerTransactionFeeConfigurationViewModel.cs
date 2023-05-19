using CRL.Infrastructure.Helpers;
using CRL.Model.Configuration;
using CRL.Service.Views.Configuration;
using CRL.UI.MVC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Areas.Configuration.Models.Shared
{
    public class _PerTransactionFeeConfigurationViewModel : BaseDetailViewModel
    {
        public PerTransactionConfigurationFeeView PerTransactionFeeConfigurationViewModel { get; set; }
        public   ICollection<LookUpView> ServiceFeeType { get; set; }
        public int[] ServiceFeeCategories { get; set; }
    }
}