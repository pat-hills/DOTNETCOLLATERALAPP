using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.Configuration
{
    public class FeeLoanSetupView
    {
        public int Id { get; set; }
        public int PerTransactionConfigurationFeeId { get; set; }
        public decimal LoanAmountLimit { get; set; }
        public decimal Fee { get; set; }
        public int IsLSDorUSD { get; set; }
    }
}
