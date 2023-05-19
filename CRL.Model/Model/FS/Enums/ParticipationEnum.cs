using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CRL.Model.FS.Enums
{
    public enum ParticipationCategory
    {
        [XmlEnum("1")]
        AsSecuredParty = 1,
        [XmlEnum("2")]
        AsBorrower = 2
    }
    public enum ParticipantCategory
    {
        [XmlEnum("1")]
        Individual = 1,
        [XmlEnum("2")]
        Insititution = 2
    }




}
