using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Views.FinancialStatement;
using CRL.Model.ModelViews.Amendment;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
    public class GetUpdateActivitySummaryViewById:FSActivityResponseBase
    {
        public ChangeDescriptionView[] UpdateChangeDescription { get; set; }

    
    }
}
