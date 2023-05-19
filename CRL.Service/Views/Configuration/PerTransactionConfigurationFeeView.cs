using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.Configuration
{
   public class PerTransactionConfigurationFeeView : ConfigurationFeeView
    {
       public PerTransactionConfigurationFeeView()
       {
       FeeLoanSetupsView = new List<FeeLoanSetupView>();
       
       }


       [Display(Name = "Loan Amount Filter")]
       public bool UseLoanAmountFilter { get; set; }
       public ICollection<FeeLoanSetupView> FeeLoanSetupsView { get; set; }
    }

   public class LenderTransactionFeeConfigurationView
   {
       public int LenderTypeId { get; set; }
       public string LenderType { get; set; }
       public ICollection<ConfigurationTransactionFeesView> ConfigurationFeeViews { get; set; }
   }
}
