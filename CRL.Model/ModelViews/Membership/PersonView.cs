using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViews.Memberships
{
    public class PersonView
    {
        public int Id { get; set; }
        [Display(Name="Title")]
        public string PersonTitle { get; set; }
        [Required]
        [Display(Name = "First Name")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,50}$", ErrorMessage = "Please enter a valid first name")]
        public string FirstName { get; set; }

        public string Fullname { get; set; }

        [Display(Name = "Middle Name")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,50}$", ErrorMessage = "Please enter a valid middle name")]
        public string MiddleName { get; set; }
        [Required]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,50}$", ErrorMessage = "Please enter a valid surname")]
        public string Surname { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [Required]
        [Display(Name = "City (Village,Town)")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,70}$", ErrorMessage = "Please enter a valid city")]
        public string City { get; set; }


        public string Country { get; set; }
        public string County { get; set; }   
        public string Nationality { get; set; }
        [Display(Name = "Institution Unit")]
        public string InstitutionUnit{ get; set; }

        [Required]
        [Display(Name = "Country")]
        public int? CountryId { get; set; }
        public int? CountyId { get; set; }

        [Required]
        [Display(Name = "Nationality")]
        public int? NationalityId { get; set; }
        public int? InstitutionId { get; set; }

        [Display(Name = "Institution Unit")]
        public int? InstitutionUnitId { get; set; }

       
    }
}
