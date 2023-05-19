using CRL.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Messaging.FinancialStatements.Request
{
    public class GetFSIndexRequest : RequestBase
    {
        public bool IgnoreCheckForHasDraft { get; set; }
        public bool IgnoreLoadingAllLookUps { get; set; }
        public bool LoadAuditActions { get; set; }

    }
}
