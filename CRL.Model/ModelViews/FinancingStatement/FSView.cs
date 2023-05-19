using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Service.Common;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.Common;
using System.Xml.Serialization;



namespace CRL.Model.ModelViews.FinancingStatement
{
    [Serializable]
    public partial class FSView : ViewBase
    {

        public FSView()
        {
            ParticipantsView = new List<ParticipantView>();
            CollateralsView = new List<CollateralView>();
            FileAttachments = new List<FileUploadView>();
            RecordState = RecordState.New;

        }
        public int Id { get; set; }
        [Display(Name = "Registration No"), XmlElement("GUID")]
        public string RegistrationNo { get; set; }
        public string RequestNo { get; set; }
        public bool IsApproved { get; set; }

        [Display(Name = "Registration Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; }

        [Required]
        //[XmlElement("LoanDueDate", DataType = "date")]
        [Display(Name = "Loan Due Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime SecurityAgreementDate { get; set; }

        [Required]
        [XmlElement("Currency")]
        [Display(Name = "Currency")]
        public int? MaximumAmountSecuredCurrencyId { get; set; } //*We should have a currency type      
        [Display(Name = "Currency")]
        public string CurrencyName { get; set; } //*We should have a currency type       

        [Required]
        [XmlElement("MaximumAmount")]
        [DisplayFormat(DataFormatString = "{0:#,###.00}")]
        //[Range(0.01, 9999999999999999.99, ErrorMessage = "The amount field must range from 0.01 to  9,999,999,999,999,999.99")]
        //[Range(typeof(Decimal), "0.01", "9999999999999999.99", ErrorMessage = "The maximum amount must be more than 0.")]
        //[RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "The {0} must be a number without commas or a currency sign.")]
        [Display(Name = "Maximum Amount")]
        public string MaximumAmountSecured { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)/*, XmlElement("DateofRegistrationExpiry", DataType = "date")*/]
        [Required]
        [Display(Name = "Date of Registration Expiry")]
        public DateTime ExpiryDate { get; set; }
        //public stri IsExpired { get; set; }

        //Relationship fields
        [Required, XmlElement("CollateralTypeId")]
        [Display(Name = "Collateral Type")]
        public CollateralCategory CollateralTypeId { get; set; }

        //Relationship fields
        [Display(Name = "Collateral Type")]
        public string CollateralTypeName { get; set; }


        [Required, XmlElement("FinancialStatementTransactionTypeId")]
        [Display(Name = "Transaction Type")]
        public FinancialStatementTransactionCategory FinancialStatementTransactionTypeId { get; set; }


        [Display(Name = "Transaction Type")]
        public string FinancialStatementTransactionTypeName { get; set; }

        [Required]
        [XmlElement("LoanType")]
        [Display(Name = "Loan Type")]
        public int? FinancialStatementLoanTypeId { get; set; }


        [Display(Name = "Unit")]
        public int? InstitutionUnitId { get; set; }

        [Display(Name = "Unit")]
        public string InstitutionUnit { get; set; }

        [Display(Name = "Loan Type")]
        public string FinancialStatementLoanTypeName { get; set; }

        //[XmlArray("ParticipantsView")]
        [XmlElement("IndividualDebtor", typeof(IndividualDebtorView))]
        [XmlElement("InstitutionalDebtor", typeof(InstitutionDebtorView))]
        public List<ParticipantView> ParticipantsView { get; set; }

        [XmlElement("Collateral")]
        public List<CollateralView> CollateralsView { get; set; }
        public List<FSActivityGridView> FSActivityGridView { get; set; }
        public List<FileUploadView> FileAttachments { get; set; }
        public int MembershipId { get; set; } //Only readOnly      
        public byte[] RowVersion { get; set; }

        public bool HasVerificationStatement { get; set; }
        public bool IsDischarged { get; set; }
        public bool IsExpired { get; set; }
        public bool IsPendingAmendment { get; set; }
        public int? AfterUpdateFinancialStatementId { get; set; }


        //We use this property to deserialize the loan due date
        [XmlElement("LoanDueDate")]
        public string SecurityAgreementDateString
        {
            get { return this.SecurityAgreementDate.ToString("yyyy-MM-dd"); }
            set { this.SecurityAgreementDate = DateTime.Parse(value); }
        }

        //We use this property to deserialize the expiry date
        [XmlElement("DateofRegistrationExpiry")]
        public string ExpiryDateString 
        {
            get { return this.ExpiryDate.ToString("yyyy-MM-dd"); }
            set { this.ExpiryDate = DateTime.Parse(value); }
        }
    }
}
