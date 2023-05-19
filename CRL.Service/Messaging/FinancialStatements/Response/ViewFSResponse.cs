using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using CRL.Service.Views.FinancialStatement;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Statistics;
using CRL.Infrastructure.Helpers;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
    public class PrepareViewStateResponse : ResponseBase 
    {
        public ICollection<LookUpView> Countys { get; set; }
        public ICollection<LKLGAView> LGAs { get; set; }
        public ICollection<LookUpView> Clients { get; set; }
        
    }  

    public class ViewStatResponse : ResponseBase
    {
        public ICollection<CountOfItemStatView> CountOfItemStatView { get; set; }
        public ICollection<ValueOfFSStatView> ValueOfFSStatView { get; set; }
        public ICollection<ValueOfFeeStatView> ValueOfFeeStatView { get; set; }
        public decimal Total { get; set; }
        
   


    }
}
