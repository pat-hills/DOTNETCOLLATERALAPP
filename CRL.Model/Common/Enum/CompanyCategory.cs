using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CRL.Model.Common.Enum
{
    public enum DebtorCategory
    {
        [XmlEnum("0")]
        Individual = 0,

        [XmlEnum("1")]
        Sole = 1,

        [XmlEnum("2")]
        Micro = 2,

        [XmlEnum("3")]
        SME = 3,

        [XmlEnum("4")]
        Large = 4,

        [XmlEnum("5")]
        Government = 5,

        [XmlEnum("6")]
        Other = 6
    }

}
