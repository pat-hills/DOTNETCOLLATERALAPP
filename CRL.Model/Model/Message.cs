using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.Infrastructure;
using CRL.Model.ModelViews;
using CRL.Model.Memberships;

namespace CRL.Model
{
    public enum MessageCategory
    {
        Alert=1, Feed=2
    }

    public class Message : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public virtual LKMessageCategory MessageType { get; set; }
        public virtual ICollection<User> ReadBy { get; set; }
        public MessageCategory MessageTypeId { get; set; }
        public bool LimitToAdmin { get; set; }
        public short? LimitToClientOrOwners { get; set; }
        public short? LimitToInstitutionOrIndividual { get; set; }
        public ICollection<Role> Roles { get; set; }
        public Message()
        {
            
            Roles = new HashSet<Role>();
            ReadBy = new HashSet<User>();

        }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

   

    public partial class LKMessageCategory : EntityBase<MessageCategory>, IAggregateRoot
    {
        [MaxLength(100)]
        public string Name { get; set; }
      

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
