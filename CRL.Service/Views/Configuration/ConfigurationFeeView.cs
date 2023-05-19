using CRL.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CRL.Service.Views.Configuration
{
     public  class ConfigurationFeeView
    {
         public int Id { get; set; }
         public string Name { get; set; }

         [Display(Name="Choose transaction types for this fee")]
         public int[] ServiceFeeCategory { get; set; }


         public int PerTransactionOrReoccurence { get; set; }
         public decimal Amount { get; set; }

         [Display(Name = "Affect postpaid payment for this fee setup")]
         public bool AllowPostpaidIfClientIsPostpaid { get; set; }

         [Display(Name = "Enabled")]
         public bool IsActive { get; set; }

         [Display(Name = "Fee Setup")]
         public bool UseFlatAmountForAllCurrencies { get; set; }
    }
}
