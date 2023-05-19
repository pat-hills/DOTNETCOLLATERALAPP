using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Common;
using CRL.Service.Common;
using CRL.Infrastructure.Configuration;


namespace CRL.Model.ModelViews.Memberships
{
    public class InstitutionView : ViewBase
    {

        public int Id { get; set; }
        [Required]
        [Display(Name = "Company Name")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,150}$", ErrorMessage = "Please enter a valid company name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Secured Creditor Type")]
        public int? SecuringPartyTypeId { get; set; }


        [Display(Name = "Secured Creditor Type")]
        public string SecuringPartyType { get; set; }

        [Required]
        [Display(Name = "Incorporation Number")]
        public string CompanyNo { get; set; }

        //[Required]
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
        [Display(Name = "Telephone")]
        public string Phone { get; set; }

        [Display(Name = "City / Town")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,70}$", ErrorMessage = "Please enter a valid city")]
        public string City { get; set; }

        [Display(Name = Constants.RegionLabel)]
        public string County { get; set; }

        public string Country { get; set; }
        [Required]
        [Display(Name = Constants.RegionLabel)]
        public int? CountyId { get; set; }

        [Required]
        [Display(Name = "Country")]
        public int? CountryId { get; set; }

        [Display(Name = "Nationality")]
        public int? NationalityId { get; set; }
        public string Nationality { get; set; }


        [Display(Name = "Local Government Area")]
        public int? LGAId { get; set; }
        public string LGA { get; set; }
        
        public int? AuthorizedByUserId { get; set; }
        public string AuthorizedByUser { get; set; }
        public Nullable<System.DateTime> AuthorizedDate { get; set; }
        public string ClientCode { get; set; }
        public MembershipView MembershipView { get; set; }
        public ICollection<InstitutionUnitView> InstitutionUnitsView { get; set; }
        public ICollection<PersonView> PeopleView { get; set; }
    }
}
