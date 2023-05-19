using CRL.UI.MVC.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Models
{
    public class LoginModel : BaseDetailViewModel
    {
        [Required]
        [RegularExpression("^[^+%'-]+$", ErrorMessage = "Invalid Login Id provided")]
        [Display(Name = "Login Id")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("^[^+%'-]+$", ErrorMessage = "Invalid Password provided")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}