using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace CRL.Model.ModelViews.FinancingStatement
{


    [Serializable]
    public class IndividualDebtorView : ParticipantView
    {
        public IndividualDebtorView()
            : base()
        {
            ;
            Identification = new IdentificationView();
            OtherIdentifications = new List<IdentificationView>();
        }
        [Display(Name = "Title"), XmlElement("Title")]
        public string IndividualTitle { get; set; }

        [XmlElement("Identification")]
        public IdentificationView Identification { get; set; }
        public List<IdentificationView> OtherIdentifications { get; set; }
        [Required, XmlElement("Gender")]
        public string Gender { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        [Required]
        [Display(Name = "Date of birth")]
        public DateTime DOB { get; set; }

        [Required]
        [Display(Name = "Sector of Operation")]
        [XmlElement("SectorOfOperations")]
        public int[] SectorOfOperationTypes { get; set; }

        [Required, XmlElement("BVN")]
        [Display(Name = "BVN")]
        [MaxLength(11)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a valid BVN")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "BVN must be 11 characters")]
        public string CardNo { get; set; }

        [Display(Name = "Passport No."), XmlElement("Passport")]
        public string CardNo2 { get; set; }

        [Required]
        [Display(Name = "BVN")]
        public int? PersonIdentificationTypeId { get; set; }

        [Display(Name = "Passport")]
        public int? PersonIdentificationTypeId2 { get; set; }



        [Display(Name = "BVN")]
        public string PersonIdentificationTypename { get; set; }
        [Display(Name = "Passport")]
        public string PersonIdentificationTypename2 { get; set; }

        [Display(Name = "Other Official Document")]
        public string OtherDocumentDescription { get; set; }

        [Display(Name = "Nationality")]
        public string NationalityView { get; set; }

        [Required, XmlElement("Nationality")]
        [Display(Name = "Nationality")]
        public int? NationalityId { get; set; }

        [MaxLength(254)]
        [EmailAddress]
        public string Email { get; set; }

        [Required, XmlElement("Telephone")]
        [Display(Name = "Telephone")]
        public string Phone { get; set; }

        [Required, XmlElement("RelationshipWithDebtor")]
        [Display(Name = "Relationship with debtor")]
        public bool? DebtorIsAlreadyClientOfSecuredParty { get; set; }

        [XmlElement("FirstName")]
        public string FirstName { get; set; }

        [XmlElement("MiddleName")]
        public string MiddleName { get; set; }

        [XmlElement("Surname")]
        public string Surname { get; set; }


        [XmlElement("DateOfBirth")]
        public string DOBString
        {
            get { return this.DOB.ToString("yyyy-MM-dd"); }
            set { this.DOB = DateTime.Parse(value); }
        }
    }

}
