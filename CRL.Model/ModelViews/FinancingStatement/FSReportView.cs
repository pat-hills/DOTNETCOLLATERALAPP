using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViews.FinancingStatement
{
    public class FSReportView
    {
        public int Id { get; set; }
        public string RegistrationNo { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? SecurityAgreementDate { get; set; }
        public string MaximumAmountSecuredCurrencyName { get; set; }
        public decimal? MaximumAmountSecured { get; set; }
        public string CollateralTypeName { get; set; }
        public string FinancialStatementTransactionTypeName { get; set; }
        public string FinancialStatementLoanTypeName { get; set; }
        public bool IsDischarged { get; set; }
        public string InstitutionUnit { get; set; }

    }

    public class DebtorReportView
    {
        public int Id { get; set; }
        public int FinancialStatementId { get; set; }
        public int SerialNo { get; set; }
        public string ParticipantNo { get; set; }
        public string Title { get; set; }  //Individual Only
        public string Name { get; set; }
        public string CardNo { get; set; } //Individual Only
        public string PersonIdentificationTypename { get; set; } //Individual Only
        public string CardNo2 { get; set; } //Individual Only
        public string PersonIdentificationTypename2 { get; set; } //Individual Only
        public string OtherDocumentDescription { get; set; } //Individual Only
        public string Email { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        public string Nationality { get; set; }
        public bool HasOtherIdentificationInformation { get; set; }
        public string Gender { get; set; }  //Individual Only    
        public System.Nullable<DateTime> DOB { get; set; }   //Individual Only   
        public string SectorOfOperationTypes { get; set; }
        public string DebtorTypeName { get; set; }
        public string CompanyNo { get; set; }
        public string BusinessTin { get; set; }
        public string LGA { get; set; }
        public bool? DebtorIsAlreadyClientOfSecuredParty { get; set; }
        public short MajorityFemaleOrMaleOrBoth { get; set; }
        public string OwnerOfCompany { get; set; }
        public bool isIndividual { get; set; }
        public int UpdatedId { get; set; }
        public bool BeforeUpdate { get; set; }
        



    }

    public class SecuredPartyReportView
    {
        public int Id { get; set; }
        public int FinancialStatementId { get; set; }
        public int SerialNo { get; set; }
        public string ParticipantNo { get; set; }
        public string Title { get; set; }  //Individual Only
        public string CompanyNo { get; set; }
        public string Name { get; set; }
        public string CardNo { get; set; } //Individual Only
        public string PersonIdentificationTypename { get; set; } //Individual Only
        public string Email { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        public string Nationality { get; set; }
        public string OtherDocumentDescription { get; set; } //Individual Only
        public string Gender { get; set; }  //Individual Only   
        public System.Nullable<DateTime> DOB { get; set; }   //Individual Only 
        public string SecuringPartyIndustryTypename { get; set; }
        public string OwnerOfCompany { get; set; }
        public bool isIndividual { get; set; }
        public string LGA { get; set; }

    }

    public class CollateralReportView
    {
        public int FinancialStatementId { get; set; }
        public string CollateralNo { get; set; }
        public string Description { get; set; }
        public string SerialNo { get; set; }
        public string CollateralSubTypeName { get; set; }
        public bool IsDischarged { get; set; }
        public int? DischargedId { get; set; }


    }

    public class OtherIdentificationReportView
    {
        public int ParticipantId { get; set; }
        public string ParticipantNo { get; set; }
        public string Name { get; set; }
        public string CardNo { get; set; } //Individual Only
        public string PersonIdentificationTypename { get; set; } //Individual Only
        public string OtherDocumentDescription { get; set; }
    }

}
