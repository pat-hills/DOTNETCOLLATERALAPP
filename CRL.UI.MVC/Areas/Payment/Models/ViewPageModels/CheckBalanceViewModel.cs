using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class CheckBalanceViewModel
    {
        [Required]
        [Display(Name="Security Code")]
        public string PublicUserSecurityCode { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.00}", ApplyFormatInEditMode = true)]
        public decimal? Balance { get; set; }
        public bool HasResponse { get; set; }
    }
}