using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model;

namespace CRL.Service.Views
{
    public class MessageView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public MessageCategory MessageTypeId { get; set; }
        public string MessageTypeName { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public bool LimitToAdmin { get; set; }
        public short? LimitToClientOrOwners { get; set; }
        public short? LimitToInstitutionOrIndividual { get; set; }
        public ICollection<int> Roles { get; set; }
        public bool Read { get; set; }

    }
}
