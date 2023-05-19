using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Common.Enum;

namespace CRL.Model.ModelViews.FinancingStatement
{
    [Serializable ]
    public class InstitutionSPView:ParticipantView
    {
        public InstitutionSPView()
            : base()
        {

        }
       
        [Required]
        public string Name { get; set; }
        //[Required] **Nigeria
        [Display(Name = "Incorporation Number")]
        [MaxLength(50)]
        public string CompanyNo { get; set; }

        [Required]
        [Display(Name = "Secured Creditor Type")]
        public int? SecuringPartyIndustryTypeId { get; set; }

        [Display(Name = "Secured Creditor Type")]
        public string SecuringPartyIndustryTypename { get; set; }

        public string OwnerOfCompany { get; set; }

        public int? RegistrantInstitutionId { get; set; }
      
    }
}
