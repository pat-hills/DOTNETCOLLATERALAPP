using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.FinancialStatement
{
    public class FSSummaryView
    {
        public int Id { get; set; }

        [Display(Name = "Registration No")]
        public string RegistrationNo { get; set; }

        [DataType(DataType.DateTime )]
        [Display(Name = "Registration Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}")]
        public DateTime RegistrationDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Expiry Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}")]
        public DateTime? ExpiryDate { get; set; }


        [DataType(DataType.DateTime)]
        [Display(Name = "Security Agreement Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}")]
        public DateTime? SecurityAgreementDate { get; set; }
      
        [Display(Name = "Maximum Amount Currency")]
        public int? MaximumAmountSecuredCurrencyId { get; set; } //*We should have a currency type
     
        [Display(Name = "Maximum Amount")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00}")]
        public decimal? MaximumAmountSecured { get; set; }
    

      
    
        [Display(Name = "Transaction Type")]
        public string FinancialStatementTransactionTypeName { get; set; }

          [Display(Name = "Security Type")]
        public string FinancialStatementLoanTypeName { get; set; }

         [Display(Name = "Last Activity")]
        public string LastActivity { get; set; }

      
         [Display(Name = "Registrant")]
         public string Registrant { get; set; }

         //Relationship fields
         [Display(Name = "Collateral ID Type")]
         public string CollateralTypeName { get; set; }
         //Relationship fields
         [Display(Name = "Collateral Type")]
         public int CollateralTypeId { get; set; }
        
        
    }
}
