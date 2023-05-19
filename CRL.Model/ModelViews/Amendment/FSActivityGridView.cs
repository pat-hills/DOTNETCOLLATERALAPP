using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.FS.Enums;

namespace CRL.Model.ModelViews.Amendment
{
    [Serializable]
    public class FSActivityGridView
    {
        public int Id { get; set; }
        [Display(Name = "Amendment No")]
        public string ActivityCode { get; set; }
        [Display(Name="Request No")]
        public string RequestNo { get; set; }
        [Display(Name = "Amendment Type")]
        public string FinancialStatementActivityType { get; set; }
        public int FinancialStatementActivityTypeId { get; set; }
        [Display(Name = "Registration No")]
        public string RegistrationNo { get; set; }

        public int FinancialStatementId { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Activity Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}")]
        public string ActivityDate { get; set; }

        [Display(Name = "Activity Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? ApprovedDate { get; set; }
        public int MembershipId { get; set; }
        public int? InstitutionUnitId { get; set; }
        public string MembershipName { get; set; }
        public bool CurrentUserIsAssginedToRequest { get; set; }
        public int CaseId { get; set; }
        public bool ItemIsLocked { get; set; }
        public int? VerificationAttachmentId { get; set; }
        public int? IsApprovedOrDenied { get; set; }
    }
}
