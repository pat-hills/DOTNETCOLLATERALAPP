using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.Payments
{
    public class InterSwitchUserView
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Name")] 
        public string Name { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Phone Number")] 
        public string Phone { get; set; }

        [Display(Name = "BVN")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "BVN must be 11 characters")]
        public string BVN { get; set; }
        
        [Display(Name = "Currency")]
        public string Currency { get; set; }

        [Required]
        [Display(Name = "Amount")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; }

        [Display(Name = "Date of Transaction")]
        public DateTime? TransactionDate { get; set; }

        [Required]
        [Display(Name = "Number of Searches")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Number of searches must be a number")]
        public int? Quantity { get; set; }

        [Display(Name="Fee Per Search")]
        public decimal? PricePerSearch { get; set; }

        [Required]
        [Display(Name="Top Up PIN Code")]
        public string TopUpCode { get; set; }

        [Required]
        [Display(Name = "Payment Type")]
        public bool IsTopUpPayment { get; set; }

        [Required]
        [Display(Name="Preferred Format")]
        public bool UsePaymentAmount { get; set; }

        [Display(Name = "Balance before Topup")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public decimal? BalanceBeforeTopUp { get; set; }

        [Display(Name="Balance after TopUp")]
        [DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public decimal? BalanceAfterTopUp { get; set; }

        public bool IsPending { get; set; }
        public bool? IsProcessed { get; set; }
        public bool IsTempError { get; set; }
        public bool IsSuccess { get; set; }

        public string Status { get; set; }
    }
}
