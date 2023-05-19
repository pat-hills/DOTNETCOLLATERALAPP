using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Views.FinancialStatement;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
    public class FindFinancialStatementsSummaryResponse:ResponseBase
    {
        
        public FSSummaryView[] FinancialStatements { get; set; }

    }
}
