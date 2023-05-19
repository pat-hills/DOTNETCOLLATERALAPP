using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CRL.Model.ModelViews.Memberships
{
    public class UserView:PersonView
    {
        [Required]
        [Display(Name = "Login Id")]
        [RegularExpression("^[^+%'-]+$", ErrorMessage = "Invalid Login Id provided")]
        public string Username { get; set; }

        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long and less than 18 characters.", MinimumLength = 8)]
        [RegularExpression("(^(?=.*[_\\W])(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{6,32}$)?(^(?=.*\\d)(?=.*[a-z])(?=.*[@#$%^&+=]).{6,32}$)?(^(?=.*\\d)(?=.*[A-Z])(?=.*[@#$%^&+=]).{6,32}$)?(^(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).{6,32}$)?",
            ErrorMessage = "The password must have at least one uppercase character, one numeric character and one non alpha numeric character")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }    
        public  MembershipView MembershipView { get; set; }
        public  ICollection<RoleView> RoleViews { get; set; }
         [Display(Name = "Paypoint User")]
        public bool isPayPointUser { get; set; }

    }

    public enum MailReceipientType { To, CC, BCC };

    public class UserEmailView
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public MailReceipientType RecepientType { get; set; }
    }
}
