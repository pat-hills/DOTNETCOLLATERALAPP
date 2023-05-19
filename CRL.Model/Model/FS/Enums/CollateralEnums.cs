using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CRL.Model.FS.Enums
{
    public enum CollateralCategory
    {
        [XmlEnum("1")]
        ConsumerGoods = 1,
        [XmlEnum("2")]
        CommercialCollateral = 2,
        [XmlEnum("3")]
        Both = 3
    };
    public enum AssetCategory { Movable = 1, Immovable = 2, Intangible = 3 }


}
