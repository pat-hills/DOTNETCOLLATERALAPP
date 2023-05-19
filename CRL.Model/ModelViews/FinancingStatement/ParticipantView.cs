using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.FS.Enums;
using CRL.Service.Common;
using CRL.Infrastructure.Configuration;
using System.Xml.Serialization;

namespace CRL.Model.ModelViews.FinancingStatement
{
    [Serializable]
    public class ParticipantView : ViewBase
    {
        public ParticipantView()
        {
            //SectorOfOperationTypes = new List<int>();
            RecordState = RecordState.New;
        }

        public int Id { get; set; }


        [Required, XmlElement("Country")]
        [Display(Name = "Country")]
        public int? CountryId { get; set; }

        [Required]
        [XmlElement("State")]
        [Display(Name = Constants.RegionLabel)]
        public int? CountyId { get; set; }




        [Display(Name = "Local Government Area"), XmlElement("LocalGovernmentArea")]
        public int? LGAId { get; set; }

        [Required, XmlElement("CityOrTown")]
        [Display(Name = "City / Town")]
        [MaxLength(70)]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,70}$", ErrorMessage = "Please enter a valid city")]
        public string City { get; set; }

        [Required, XmlElement("Address1")]
        [MaxLength(255)]
        [Display(Name = "Address 1")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,255}$", ErrorMessage = "Please enter a valid address")]
        public string Address { get; set; }


        [MaxLength(255), XmlElement("Address2")]
        [Display(Name = "Address 2")]
        [RegularExpression("^(?=.*[a-zA-Z]).{0,255}$", ErrorMessage = "Please enter a valid address")]
        public string Address2 { get; set; }

        [Display(Name = "Country")]
        public string CountryView { get; set; }
        [Display(Name = Constants.RegionLabel)]
        public string County { get; set; }
     
        public string LGA { get; set; }

        [Display(Name = "If other specify")]
        public string OtherSectorOp { get; set; }

        [XmlElement("ParticipationCategory")]
        public ParticipationCategory ParticipationTypeId { get; set; }
        [XmlElement("ParticipantCategory")]
        public ParticipantCategory ParticipantTypeId { get; set; }
        public string ParticipantNo { get; set; }
        public int? RegistrantMembershipId { get; set; }
      
        
      
    }
}
