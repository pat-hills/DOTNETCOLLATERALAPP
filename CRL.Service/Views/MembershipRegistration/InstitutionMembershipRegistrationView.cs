using CRL.Infrastructure.Configuration;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.Memberships
{
    public class InstitutionMembershipRegistrationView : MembershipRegistrationView
    {
        //Name of the user to setup an account for
        [Required]
        [Display(Name = "Secured Creditor Name")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,150}$", ErrorMessage = "Please enter a valid name")]
        public string Name { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string LegalEmail { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Address 1")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,255}$", ErrorMessage = "Please enter a valid address")]
        public string LegalAddress { get; set; }

        [MaxLength(255)]
        [Display(Name = "Address 2")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,255}$", ErrorMessage = "Please enter a valid address")]
        public string LegalAddress2 { get; set; }
       
        [Required]
        [Display(Name = "Telephone")]
        public string LegalPhone { get; set; }

        [Required]
        [Display(Name = "City / Town")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,70}$", ErrorMessage = "Please enter a valid city")]
        public string LegalCity { get; set; }
        public string LegalCountry { get; set; }
        
        [Required]
        [Display(Name = "Country")]
        public int? LegalCountryId { get; set; }

         [Required]
        [Display(Name = Constants.RegionLabel)]
        public int? LegalCountyId { get; set; }


        [Display(Name = Constants.RegionLabel)]
        public string LegalCounty { get; set; }

        
        [Display(Name = "Nationality")]
        public int? LegalNationalityId { get; set; }
        public string LegalNationality { get; set; }
        
        [Required]
        [Display(Name = "Secured Creditor Type")]
        public int? SecuringPartyTypeId { get; set; }
        public string SecuringPartyType { get; set; }

        [Required]
        [Display(Name = "Incorporation Number")]
        public string CompanyNo { get; set; }

        //

     

        //
    }
}
