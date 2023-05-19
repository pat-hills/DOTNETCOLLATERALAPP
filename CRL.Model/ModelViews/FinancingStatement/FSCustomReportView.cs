using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.FS.Enums;

namespace CRL.Model.ModelViews.FinancingStatement
{
    public class FSCustomReportView
    {
        public int Id { get; set; }
        public string RegistrationNo { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? SecurityAgreementDate { get; set; }
        public string MaximumAmountSecuredCurrencyName { get; set; }
        public decimal? MaximumAmountSecured { get; set; }
        public string CollateralTypeName { get; set; }
        public CollateralCategory? CollateralTypeId { get; set; }
        public string FinancialStatementTransactionTypeName { get; set; }
        public string FinancialStatementLoanTypeName { get; set; }
        public bool IsDischarged { get; set; }
        public FinancialStatementLoanCategory? FinancialStatementLoanTypeId { get; set; }
        public FinancialStatementTransactionCategory? FinancialStatementTransactionTypeId { get; set; }
        public int? MaximumAmountSecuredCurrencyId { get; set; }
        public string FinancialStatementLastActivity { get; set; }
        public bool IsPendingAmendment { get; set; }
        public bool IsExpired { get; set; }
        public int? MembershipId { get; set; }
        public bool HasFileAttachment { get; set; }
        public string MembershipName { get; set; }
        public string MembershipType { get; set; }

        public int? SecuredParty_Id { get; set; }
        public string SecuredParty_Title { get; set; }  //Individual Only
        public string SecuredParty_CompanyNo { get; set; }
        public string SecuredParty_Name { get; set; }
        public string SecuredParty_CardNo { get; set; } //Individual Only
        public string SecuredParty_PersonIdentificationTypename { get; set; } //Individual Only
        public string SecuredParty_Email { get; set; }
        public string SecuredParty_Address { get; set; }
        public string SecuredParty_Phone { get; set; }
        public string SecuredParty_City { get; set; }
        public string SecuredParty_Country { get; set; }
        public string SecuredParty_County { get; set; }
        public string SecuredParty_Nationality { get; set; }
        public string SecuredParty_OtherDocumentDescription { get; set; } //Individual Only
        public string SecuredParty_Gender { get; set; }  //Individual Only   
        public System.Nullable<DateTime> SecuredParty_DOB { get; set; }   //Individual Only 
        public string SecuredParty_SecuringPartyIndustryTypename { get; set; }
        public string SecuredParty_OwnerOfCompany { get; set; }
        public bool? SecuredParty_isIndividual { get; set; }

        public int? Debtor_Id { get; set; }

        public string Debtor_Title { get; set; }  //Individual Only
        public string Debtor_Name { get; set; }
        public string Debtor_CardNo { get; set; } //Individual Only
        public string Debtor_PersonIdentificationTypename { get; set; } //Individual Only
        public string Debtor_OtherDocumentDescription { get; set; } //Individual Only
        public string Debtor_Email { get; set; }
        public string Debtor_Address { get; set; }
        public string Debtor_Phone { get; set; }
        public string Debtor_City { get; set; }
        public string Debtor_Country { get; set; }
        public string Debtor_County { get; set; }
        public string Debtor_Nationality { get; set; }
        public bool? Debtor_HasOtherIdentificationInformation { get; set; }
        public string Debtor_Gender { get; set; }  //Individual Only    
        public System.Nullable<DateTime> Debtor_DOB { get; set; }   //Individual Only   
        public string Debtor_SectorOfOperationTypes { get; set; }
        public string Debtor_DebtorTypeName { get; set; }
        public string Debtor_CompanyNo { get; set; }
        public string Debtor_DebtorIsAlreadyClientOfSecuredParty { get; set; }
        public short Debtor_MajorityFemaleOrMale { get; set; }
        public string Debtor_OwnerOfCompany { get; set; }
        public bool? Debtor_isIndividual { get; set; }


        public string Collateral_CollateralNo { get; set; }
        public string Collateral_Description { get; set; }
        public string Collateral_SerialNo { get; set; }
        public string Collateral_SubTypeName { get; set; }
        public bool? Collateral_IsDischarged { get; set; }
        public int? Collateral_DischargedId { get; set; }
    }
}
