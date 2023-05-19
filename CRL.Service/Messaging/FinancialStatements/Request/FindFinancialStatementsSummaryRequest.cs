using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.FS.Enums;
using CRL.Infrastructure.Messaging;


namespace CRL.Service.Messaging.FinancialStatements.Request
{
    public class FindFinancialStatementsSummaryRequest:RequestBase
    {
        public DateRange RegistrationDate { get; set; }
        public string RegistrationNo { get; set; }
        public int? FinancialStatementTransactionTypeId { get; set; }

    }
}
