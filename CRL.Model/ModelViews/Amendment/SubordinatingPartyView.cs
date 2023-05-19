using CRL.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViews.Amendment
{
    public class SubordinatingPartyView
    {
        public int Id { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        [Required]
        [MaxLength(255)]
        [Display(Name = "Address 1")]
        public string Address { get; set; }


        [MaxLength(255)]
        [Display(Name = "Address 2")]
        public string Address2 { get; set; }
        [Display(Name = "Telephone")]
        public string Phone { get; set; }


        [Display(Name = "City / Town")]
        public string City { get; set; }
        public string Country { get; set; }
        [Display(Name = Constants.RegionLabel)]
        public string County { get; set; }
        public string Nationality { get; set; }
        [Required]
        [Display(Name = "Country")]
        public int? CountryId { get; set; }
        [Required]
        [Display(Name = Constants.RegionLabel)]
        public int? CountyId { get; set; }
        [Required]
        [Display(Name = "Nationality")]
        public int? NationalityId { get; set; }

        public int SubordinatingPartyType { get; set; }

        [Display(Name = "Type")]
        public bool isBeneficiary { get; set; }


        [Required]
        [Display(Name = "LGA")]
        public int? LGAId { get; set; }


        public string LGA { get; set; }


    }
}
