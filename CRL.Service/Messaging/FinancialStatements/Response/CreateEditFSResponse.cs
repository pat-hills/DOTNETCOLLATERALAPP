using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Service.Views;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
    public class FSResponse : ResponseBase
    {
        public int Id{get;set;}
        public int FinancialStatementId { get; set; }
        public int AffectedAccountTransactionId { get; set; }
        public int AffectedApprovedEmailId { get; set; }
        public int AffectedCaseId { get; set; }        
        public bool? SuccessfullyGeneratedConfirmationReport { get; set; }
        
        public int FSActivity { get; set; }
        public byte[] FSRowVersion { get; set; }
    }
}
