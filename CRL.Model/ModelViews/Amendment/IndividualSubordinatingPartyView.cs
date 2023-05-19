using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.Model.ModelViews.Amendment
{
    public class IndividualSubordinatingPartyView : SubordinatingPartyView
    {
        public IndividualSubordinatingPartyView()
        {
            Identification = new IdentificationView();
        }
        public string Title { get; set; }
        public IdentificationView Identification { get; set; }
        [Required]
        public string Gender { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        [Required]
        [Display(Name = "Date of birth")]
        public System.Nullable<DateTime> DOB { get; set; }
    }
}
