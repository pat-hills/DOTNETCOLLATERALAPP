using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Service.Common;

namespace CRL.Model.ModelViews.FinancingStatement
{
    public class CollateralSummaryView:ViewBase
    {

        public int Id { get; set; }
        public string Description { get; set; }
        [Display(Name = "Serial No")]
        public string SerialNo { get; set; }
        public string CollateralSubTypeName {get;set;}
       
        
        
    }
}
