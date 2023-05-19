using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Service.Common;

using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
    public class GetDataForFSViewResponse:ResponseBase
    {
        public ICollection<LookUpView> FinancialStatementTransactionTypes { get; set; }
        public ICollection<LookUpView> FinancialStatementLoanType { get; set; }
        public ICollection<LookUpView> CollateralTypes { get; set; }
       

    }
    public class GetFSIndexResponse : ResponseBase
    {
        public ICollection<LookUpView> FinancialStatementTransactionTypes { get; set; }
        public ICollection<LookUpView> FinancialStatementLoanType { get; set; }
        public ICollection<LookUpView> CollateralTypes { get; set; }
        public ICollection<LookUpView> AuditType { get; set; }
        public ICollection<AuditActionView> AuditActions { get; set; }
        public bool HasDraftLoad { get; set; }


    }

    
}
