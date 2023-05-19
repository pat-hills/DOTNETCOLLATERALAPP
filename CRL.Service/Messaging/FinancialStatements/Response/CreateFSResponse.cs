using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Views;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
    public class CreateFSResponse:ResponseBase
    {
        public FSView FinancialStatementView { get; set; }

    }
}
