using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViews.Amendment
{
    public class InstitutionSubordinatingPartyView:SubordinatingPartyView
    {
   
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Company No")]
        public string CompanyNo { get; set; }
        [Required]
        [Display(Name = "Secured Party Type")]
        public int? SecuringPartyIndustryTypeId { get; set; }
        [Display(Name = "Secured Party Type")]
        public string SecuringPartyIndustryTypename { get; set; }
        public string OwnerOfCompany { get; set; }
        public int? RelatedInstitutionId { get; set; }
    
    }
}
