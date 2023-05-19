using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Views.FinancialStatement
{
    public class FSActivityDetailSummaryView
    {
        public int Id { get; set; }
        [Display(Name = "Amendment No")]
        public string ActivityCode { get; set; }
        public string RequestNo { get; set; }
        [Display(Name = "Amendment Type")]
        public string FinancialStatementActivityTypeName { get; set; }
        public int FinancialStatementActivityTypeId { get; set; }
        [Display(Name = "Registration No")]
        public string RegistrationNo { get; set; }

        public int FinancialStatementId { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Activity Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}")]
        public DateTime ActivityDate { get; set; }
        public int MembershipId { get; set; }
        public string MembershipName { get; set; }
        public bool CurrentUserIsAssginedToRequest { get; set; }
        public int CaseId { get; set; }
        public bool ItemIsLocked { get; set; } 
    }

    public class RenewalFSActivitySummaryView : FSActivityDetailSummaryView
    {

        public DateTime BeforeExpiryDate { get; set; }
        public DateTime AfterExpiryDate { get; set; }
    }


  

    public class DischargeFSActivitySummaryView : FSActivityDetailSummaryView
    {

        public string DischargedTypeName { get; set; }
        public CollateralSummaryView[] PartialDischargedCollaterals;
    }

    public class DischargeFSDueToErrorActivitySummaryView : FSActivityDetailSummaryView
    {
    }

     public class UpdateFSActivitySummaryView : FSActivityDetailSummaryView
    {

         public ChangeDescriptionView[] UpdateChangeDescription { get; set; }
    }

     public class SubordinateFSActivitySummaryView : FSActivityDetailSummaryView
     {

         public SubordinatingPartyView SubordinatingPartyView { get; set; }
     }

     public class AssignmentFSActivitySummaryView : FSActivityDetailSummaryView
     {

       
         public string Assignor { get; set; }        
         public string Assignee {get; set; }         
         public string AssignmentType { get; set; }        
         public string AssignmentDescription { get; set; }
         public InstitutionView  AssignorDetails { get; set; }
         public InstitutionView  AssigneeDetails { get; set; }
         
     }
}


