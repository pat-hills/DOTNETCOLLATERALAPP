using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Infrastructure;

namespace CRL.Model.Configuration
{
    [Serializable]
    public partial class LKCurrency : EntityBase<int>, IAggregateRoot
    {
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencySymbol { get; set; }
        public bool isLocal { get; set; }
        public decimal? SellingRateWithLocalCurrency { get; set; }




        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
