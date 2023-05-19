using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CRL.UI.MVC.Areas.Search.Models.ViewPageModels
{
    public class ViewSearchesViewModel
    {
        [Required]
        [Display(Name = "Security Code")]
        [RegularExpression("^[^+%'-]+$", ErrorMessage = "Invalid Public Security provided")]
        public string PublicUserSecurityCode { get; set; }
    }
}