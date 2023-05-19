using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;

namespace CRL.Model.ModelViews.Search
{
    public class SearchParamView
    {
     
        public string BorrowerType { get; set; }       
        public string BorrowerName { get; set; }      
        public string BorrowerFirstName { get; set; }        
        public string BorrowerMiddleName { get; set; }    
        public string BorrowerLastName { get; set; }       
        public string BorrowerIDNo { get; set; }            
        public string CollateralSerialNo { get; set; }     
        public DateTime? DebtorDateOfBirth { get; set; } 
        public string CollateralDescription { get; set; }       
        public string DebtorEmail { get; set; }
        public string IsNonLegalEffectSearch { get; set; }
        public string SearchCode { get; set; }
        public string ReportType { get; set; }
        public DateTime? DateOfSearch { get; set; }
        public string IdentifierId { get; set; }
        public DateTime ReportGeneratedDate { get; set; }
    }
}
