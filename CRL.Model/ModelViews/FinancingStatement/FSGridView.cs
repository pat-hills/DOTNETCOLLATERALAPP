using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.FS.Enums;

namespace CRL.Model.ModelViews.FinancingStatement
{
    public class FSGridView
    {
        public int Id { get; set; }
        public string RegistrationNo { get; set; }
        public string RequestNo { get; set; }
         [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime RegistrationDate { get; set; }
        public int? MaximumAmountSecuredCurrencyId { get; set; }
        public string MaximumAmountSecuredCurrencyName { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.00}")]
        public decimal? MaximumAmountSecured { get; set; }
        public string FinancialStatementTransactionTypeName { get; set; }
        public string FinancialStatementLoanTypeName { get; set; }
        public string FinancialStatementLastActivity { get; set; }
        public FinancialStatementTransactionCategory  FinancialStatementTransactionTypeId { get; set; }
        public System.Nullable <FinancialStatementLoanCategory> FinancialStatementLoanTypeId { get; set; }
         [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ExpiryDate { get; set; }
        public CollateralCategory CollateralTypeId { get; set; }
        public string CollateralTypeName { get; set; }
        public int MembershipId { get; set; }
        public int? InstitutionUnitId { get; set; }
        public string InstitutionUnit { get; set; }
        public string MembershipName { get; set; }
        public bool HasFileAttachment { get; set; }
        public bool HasVerificationStatement { get; set; }
        public bool CurrentUserIsAssginedToRequest { get; set; }
        public int CaseId { get; set; }
        public bool ItemIsLocked { get; set; }  //Will not allow the item to be edited if it is locked
        public bool IsDischarged { get; set; }
        public bool IsExpired { get; set; }
        public bool IsPendingAmendment { get; set; }
        public string MembershipType { get; set; }
        public bool Uploaded { get; set; } //Used for financing statements in a batch
        public string FirstDebtorName { get; set; }


    }
}
