using CRL.Service.Views.Configuration;
using CRL.UI.MVC.Areas.Configuration.Models.Shared;
using CRL.UI.MVC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.UI.MVC.Areas.Configuration.Models.ViewPageModel
{
    public class FeeConfigurationDetailViewModel : BaseDetailViewModel
    {
     public FeeConfigurationDetailViewModel()
     {
         _ConfigurationFeeViewModel = new _ConfigurationFeeViewModel();
     }
     public _ConfigurationFeeViewModel _ConfigurationFeeViewModel { get; set; }
    }
}
