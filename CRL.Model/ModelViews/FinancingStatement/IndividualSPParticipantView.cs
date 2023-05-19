using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Common;
using CRL.Model.FS.Enums;
using System.Xml.Serialization;


namespace CRL.Model.ModelViews.FinancingStatement
{
    [Serializable]
    public class IdentificationView
    {
        public int OtherIdentificationId { get; set; }
        [Required, XmlElement("FirstName")]
        [Display(Name = "First Name")]
        [MaxLength(50)]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,50}$", ErrorMessage = "Please enter a valid first name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [MaxLength(50), XmlElement("MiddleName")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,50}$", ErrorMessage = "Please enter a valid middle name")]
        public string MiddleName { get; set; }

        [Required, XmlElement("Surname")]
        [Display(Name = "Surname")]
        [MaxLength(50)]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,50}$", ErrorMessage = "Please enter a valid surname")]
        public string Surname { get; set; }    
         
        public string UniqueCode { get; set; }
    }
     [Serializable]
    public class IndividualSPView:ParticipantView
    {
        public IndividualSPView()
            : base()
        {
            Identification = new IdentificationView();
        }
    public string Title { get; set; }

        public IdentificationView Identification {get;set;}
 
        [Required]
        public string Gender { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}",ApplyFormatInEditMode = true)]
        [Required]
        [Display(Name = "Date of birth")]
        public DateTime DOB { get; set; }
        public int? RegistrantIndividualId { get; set; } 



       
    }
     

      
     

}
