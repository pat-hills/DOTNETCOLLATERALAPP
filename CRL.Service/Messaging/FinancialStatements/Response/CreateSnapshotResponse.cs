using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Views;
using CRL.Service.Views.FinancialStatement;
using CRL.Infrastructure.Helpers;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
    public  class CreateSnapshotResponse:ResponseBase
    {
        public FSView FSView { get; set; }
    }

    public class SaveDraftResponse : ResponseBase
    {
        public FSView FSView { get; set; }
    }
 
   
}
