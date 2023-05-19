using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.Infrastructure;
using CRL.Model.Payments;

namespace CRL.Model.Configuration
{ 
    public enum ReoccurencePeriod { Daily = 1, Monthly = 2, Quarterly = 3, SemiAnually=4, Yearly = 5 }
    public class ConfigurationFee : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public ConfigurationFee()
        {
            ServiceFeeType = new HashSet<LKServiceFeeCategory>();
        }
        public string Name { get; set; }
        public virtual ICollection<LKServiceFeeCategory> ServiceFeeType { get; set; }
        public int PerTransactionOrReoccurence { get; set; }      
        public decimal FlatAmount { get; set; }
        public bool UseFlatAmountForAllCurrencies { get; set; }
        public bool AllowPostpaidIfClientIsPostpaid { get; set; }    


        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    public class PerTransactionConfigurationFee : ConfigurationFee
    {
        public PerTransactionConfigurationFee()
        {
            FeeLoanSetups = new HashSet<FeeLoanSetup>();
        }
        
        public virtual ICollection<FeeLoanSetup> FeeLoanSetups { get; set; }

    }
    public class PeriodicConfigurationFee : ConfigurationFee
    {
        public ReoccurencePeriod? ReoccurencePeriod { get; set; }
    }


    public class FeeLoanSetup : AuditedEntityBaseModel<ServiceFees>, IAggregateRoot
    {
        public PerTransactionConfigurationFee PerTransactionConfigurationFee { get; set; }
        public int PerTransactionConfigurationFeeId { get; set; }
        public decimal LoanAmountLimit { get; set; }
        public decimal Fee { get; set; }
        public short isLSDorUSD { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }


    public partial class LKServiceFeeCategory : AuditedEntityBaseModel<ServiceFees>, IAggregateRoot
    {

        public string Name { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    public partial class TransactionPaymentSetup : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public LKServiceFeeCategory ServiceType { get; set; }
        public ServiceFees ServiceTypeId { get; set; }
        public decimal Fee { get; set; }
        public int LenderType { get; set; }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
