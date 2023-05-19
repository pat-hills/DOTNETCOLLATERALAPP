using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Payments;

namespace CRL.Model.ModelViews.Payments
{
    public class PaypointPaymentView
    {
        public string Paypoint { get; set; }
        public decimal Amount { get; set; }
    }
 
    public class PaymentView
    {
        public int Id { get; set; }



        [Required]
        [DisplayFormat(DataFormatString = "{0:#,###.00}")]
        //[Range(0.01, 9999999999999999.99, ErrorMessage = "The amount field must range from 0.01 to  9,999,999,999,999,999.99")]
        [Range(typeof(Decimal), "0.01", "9999999999999999.99", ErrorMessage = "{0} must be a decimal/number between {1} and {2}.")]
        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "The {0} must be a number without commas or a currency sign.")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Payment No")]
        public string PaymentNo { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        [Required]
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; }
        [Required]
        [Display(Name = "Paid by")]
        public string Payee { get; set; }
      
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string PublicUserEmail { get; set; }
         [Display(Name = "Security Code")]
        public string SecurityCode { get; set; }
        public int? MembershipId { get; set; }
        public bool IsPublicUser { get; set; }
        public string Client { get; set; }
        public string Paypoint { get; set; }
        [Display(Name = "Received by")]
        public string PaypointUser { get; set; }
        [Display(Name = "Payment Source")]
        public PaymentSource PaymentSource { get; set; }
        [Display(Name = "Payment Source")]
        public string PaymentSourceName { get; set; }
       
        
        [Display(Name = "Payment Type")]
        public int? PaymentType { get; set; }
         [Display(Name = "Payment Type")]
         public string PaymentTypeName { get; set; }

         public int? SettlementId { get; set; }
        [Required]
        [Display(Name = "Transaction No")]
        public string T24TransactionNo { get; set; }

        [Display(Name = "Client Type")]
        public string ClientType { get; set; }

        public bool IsReversible { get; set; }
        public int CreatedBy { get; set; }
        public int? CreatedByUserMembershipId { get; set; }

    }
}
