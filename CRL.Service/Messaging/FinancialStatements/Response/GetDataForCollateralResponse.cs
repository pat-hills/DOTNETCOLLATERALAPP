using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Service.Common;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
    public class GetAddCollateralResponse : ResponseBase
    {
        public ICollection<LookUpView> CollateralSubTypes { get; set; }
    }
}
