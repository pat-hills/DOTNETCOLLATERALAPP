using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Enums;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.FinancialStatements.Request
{

    public class GetDraftRequest : RequestBase
    {

        public short RegistrationOrUpdate { get; set; }
        public EditMode EditMode { get; set; }
        public int AssociatedIdForNonNew { get; set; }
    }
}
