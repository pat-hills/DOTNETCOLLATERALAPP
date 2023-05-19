using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Service.Views.FinancialStatement;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
    public class GetDischargeActivitySummaryViewById : FSActivityResponseBase
    {
        public string DischargedTypeName { get; set; }
       

        public CollateralSummaryView[] PartialDischargedCollaterals;
    



    }
}
