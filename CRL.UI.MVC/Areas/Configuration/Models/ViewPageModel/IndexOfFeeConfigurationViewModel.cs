using CRL.UI.MVC.Areas.Configuration.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.UI.MVC.Areas.Configuration.Models.ViewPageModel
{
   public  class IndexOfFeeConfigurationViewModel
    {
       public IndexOfFeeConfigurationViewModel()
       {
           _FeeConfigurationsJqgrid = new _FeeConfigurationsJqgrid();
          
       }

       public _FeeConfigurationsJqgrid _FeeConfigurationsJqgrid { get; set; }
    }
}
