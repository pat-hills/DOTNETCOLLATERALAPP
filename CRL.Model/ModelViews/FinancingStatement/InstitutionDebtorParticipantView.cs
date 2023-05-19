using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Common.Enum;
using System.Xml.Serialization;


namespace CRL.Model.ModelViews.FinancingStatement
{
    [Serializable ]
    public class InstitutionDebtorView:ParticipantView 
    {
        [Required, XmlElement("DebtorType")]
        [Display(Name = "Debtor Type")]
        public System.Nullable<DebtorCategory> DebtorTypeId { get; set; }
        [Display(Name = "Debtor Type")]
        public string DebtorTypeName { get; set; }
        [Required]
        [Display(Name = "Business Reg No"), XmlElement("BusinessRegNo")]
        [MaxLength(10)]
        public string CompanyNo { get; set; }

        [Required, XmlElement("BusinessRegPrefix")]
        public int? BusinessTinPrefix { get; set; }

        [Display(Name = "Business Tax ID")]
        public string BusinessTinFullName { get; set; }

        [Display(Name = "Business Tax ID")]
        [MaxLength(50)]
        public string BusinessTin { get; set; }


        [Required, XmlElement("OwnerComposition")]
        [Display(Name = "Owner Composition")]
        public short MajorityFemaleOrMaleOrBoth { get; set; }
        public string OwnerOfCompany { get; set; }
        [Required, XmlElement("Name")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,150}$", ErrorMessage = "Please enter a valid company name")]
        public string Name { get; set; }
        [Display(Name = "Sector of Operation")]
        [XmlElement("SectorOfOperations")]
        public int[] SectorOfOperationTypes { get; set; }



        //[Required]
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
    }
}
