using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViews.Amendment
{
    public class FSActivitySummaryReportView
    {
        public int Id { get; set; }
        public DateTime ActivityDate { get; set; }
        public string AmendmentCode { get; set; }
        public string TypeOfAmendment { get; set; }
        public string ActivityDescription { get; set; }
        public string Reference { get; set; }
        public int FinancialStatementId { get; set; }

    }
    public class ActivityReportView
    {
        public int Id { get; set; }
        public string ActivityCode { get; set; }
        public DateTime ActivityDate { get; set; }
        public int FinancialStatementId { get; set; }

    }

    public class UpdateActivityReportView : ActivityReportView
    {
        public int BeforeUpdateFinancialStatementId { get; set; }

    }

    public class DischargeActivityReportView : ActivityReportView
    {
        public int DischargeType { get; set; }
        public string DischargedTypeName { get; set; }
        public bool PerformedByRegistrar { get; set; }
        
    }

    public class AssignmentActivityReportView : ActivityReportView
    {
        public int AssignmentType { get; set; }
        public string AssignmentTypeName { get; set; }
        public int AssignedPartyId { get; set; }
        public int AssignedFromPartyId { get; set; }
    }

    public class SubordinationActivityReportView : ActivityReportView
    {
        public int SubordinatingParticipantId { get; set; }
    }


    public class SubordinatingPartyReportView : SubordinatingPartyView
    {

        public string Name { get; set; }
        public string CompanyNo { get; set; }
        public int? SecuringPartyIndustryTypeId { get; set; }
        public string SecuringPartyIndustryTypename { get; set; }
        public string OwnerOfCompany { get; set; }
        public string CardNo { get; set; }
        public int PersonIdentificationTypeId { get; set; }
        public string PersonIdentificationTypename { get; set; }
        public string OtherDocumentDescription { get; set; }      
        public string Title { get; set; }
        public string Gender { get; set; }
        public bool IsIndividual { get; set; }
        public System.Nullable<DateTime> DOB { get; set; }
    }

    public class ClientReportView
    {
        public int Id { get; set; }
        public string Title { get; set; }  //Individual Only
        public string Name { get; set; }
        public string CardNo { get; set; } //Individual Only        
        public string Email { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        public string Nationality { get; set; }
        public string LGA { get; set; }
        public int PersonIdentificationTypeId { get; set; }
        public string PersonIdentificationTypename { get; set; }
        public string OtherDocumentDescription { get; set; } //Individual Only
        public string Gender { get; set; }  //Individual Only   
        public System.Nullable<DateTime> DOB { get; set; }   //Individual Only 
        public string SecuringPartyIndustryTypename { get; set; }
        public bool IsIndividual { get; set; }
        public string CompanyNo { get; set; }


    }
}
