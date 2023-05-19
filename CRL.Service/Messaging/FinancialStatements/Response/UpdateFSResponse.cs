using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Views;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
    public class UpdateFSResponse : ResponseBase
    {
        public FSView FinancialStatementView { get; set; }
        public byte[] FSRowVersion { get; set; }

    }
}
