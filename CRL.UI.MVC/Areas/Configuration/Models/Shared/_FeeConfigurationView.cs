using CRL.Service.Views.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Model.Configuration;
using CRL.Infrastructure.Helpers;

namespace CRL.UI.MVC.Areas.Configuration.Models.Shared
{
    public class _FeeConfigurationViewModel
    {
        public _FeeConfigurationViewModel()
        {
            _ConfigurationFeeViewModel = new List<_ConfigurationFeeViewModel>();
        
        }
        public List<_ConfigurationFeeViewModel> _ConfigurationFeeViewModel { get; set; }
     
       
    }
}