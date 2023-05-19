using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
    public class GetRenewalActivitySummaryViewById : FSActivityResponseBase
    {
        public DateTime BeforeExpiryDate { get; set; }
        public DateTime AfterExpiryDate { get; set; }

    }
}
