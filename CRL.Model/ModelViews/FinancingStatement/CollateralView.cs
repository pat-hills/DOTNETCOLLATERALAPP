using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.FS.Enums;
using CRL.Service.Common;
using System.Xml.Serialization;


namespace CRL.Model.ModelViews.FinancingStatement
{
    [Serializable]
    public partial class CollateralView : ViewBase
    {
        public CollateralView()
        {
            RecordState = RecordState.New;
            AssetTypeId = AssetCategory.Movable;
            //CollateralSubTypes = new List<LookUpView>();
        }


        
        public int Id { get; set; }

        [XmlElement("Description")]
        [Required(ErrorMessage="Please provide collateral desciption for one or more of the supplied collaterals")]
        [DataType(DataType.MultilineText)]
        [MaxLength(255)]
        public string Description { get; set; }
        
        [MaxLength(100)]
        [XmlElement("SerialNo")]
        [Display(Name = "Serial No")]
        public string SerialNo { get; set; }

      
        [Required]
        [XmlElement("CollateralTypes")]
        [Display(Name = "Collateral Types")]
        public int CollateralSubTypeId{ get; set; }

    
        [Display(Name = "Collateral Type")]
        public string CollateralSubTypeName { get; set; }

        public string CollateralNo { get; set; }
        public AssetCategory AssetTypeId { get; set; }
        
        
    }


}
