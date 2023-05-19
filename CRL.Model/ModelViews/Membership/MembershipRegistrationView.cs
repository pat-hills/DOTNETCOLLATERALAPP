using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews.Enums;

namespace CRL.Model.ModelViews.Memberships
{
    public class MembershipRegistrationView
    {
      
        public string AccountNumberWithCentralBank { get; set; }
        /// <summary>
        /// If there is a representative client then name of that client
        /// </summary>
    
        [Display(Name = "Representative Bank")]
        public int? RepresentativeInstitutionClientId { get; set; }

     
        /// <summary>
        /// Membership account type that must be stated before approval of this request
        /// </summary>
        [Required]
        [Display(Name = "Bank Account Integration")]
        public MembershipAccountCategory MembershipAccountTypeId { get; set; }

        public string MembershipAccountType { get; set; }


        //Name of the user to setup an account for
        public string RequestNo { get; set; }
        public string Title { get; set; }
        [Required]
        [Display(Name = "First name")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,50}$", ErrorMessage = "Please enter a valid first name")]
        public string FirstName { get; set; }
     
        [Display(Name = "Middle name")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,50}$", ErrorMessage = "Please enter a valid middle name")]
        public string MiddleName { get; set; }
        [Required]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,50}$", ErrorMessage = "Please enter a valid surname")]
        public string Surname { get; set; }

        [Required]
        public string Gender { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(255)]
        [Display(Name = "Address 1")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,255}$", ErrorMessage = "Please enter a valid address")]
        public string Address { get; set; }

        [MaxLength(255)]
        [Display(Name = "Address 2")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,255}$", ErrorMessage = "Please enter a valid address")]
        public string Address2 { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        [Display(Name = "City / Town")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,70}$", ErrorMessage = "Please enter a valid city")]
        public string City { get; set; }
        
        public string Country { get; set; }
        [Required]
        [Display(Name = "Country")]
        public int? CountryId { get; set; }

        [Required]
        [Display(Name = "County")]
        public int? CountyId { get; set; }

        public string County { get; set; }

        [Required]
        [Display(Name = "Local Government Area")]
        public int? LgaId { get; set; }

        public string Lga { get; set; }


        public string Nationality { get; set; }
        [Required]
        [Display(Name = "Nationality")]
        public int? NationalityId { get; set; }
        public int IndividualOrMembership { get; set; }
        /// <summary>
        /// Login name for Administrator to be created
        /// </summary>
        ///   [Required]
         [Required]
        [Display(Name = "Login Id")]
        public string AccountName { get; set; }
           [Required]
        [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long and less than 18 characters.", MinimumLength = 8)]
        [RegularExpression("(^(?=.*[_\\W])(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{6,32}$)?(^(?=.*\\d)(?=.*[a-z])(?=.*[@#$%^&+=]).{6,32}$)?(^(?=.*\\d)(?=.*[A-Z])(?=.*[@#$%^&+=]).{6,32}$)?(^(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).{6,32}$)?",
            ErrorMessage = "The password must have at least one uppercase character, one numeric character and one non alpha numeric character")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

           [Required]
           [DataType(DataType.Password)]
           [Display(Name = "Confirm Password")]
           [Compare("Password")]
           public string ConfirmPassword { get; set; }

        public int Id { get; set; }
        [Required]
         [Display(Name = "Major Role")]
        public short MajorRoleIsSecuredPartyOrAgent { get; set; }

        public string BankOrFinancialInstitutionCode { get; set; }

    }
}
