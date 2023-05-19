using CRL.Model.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.Configuration
{
   public class PeriodicConfigurationFeeView: ConfigurationFeeView
    {
       public ReoccurencePeriod? ReoccurencePeriod { get; set; }

    }
}
