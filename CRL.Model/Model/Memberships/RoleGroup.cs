using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;

namespace CRL.Model.Memberships
{
    [Serializable]
    public class RoleGroup : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public RoleGroup()
        {
            this.Roles = new HashSet<Role>();
        }
        [MaxLength(50)]
        public string RoleGroupName { get; set; } //Can individuals also create their own roles
        public Nullable<int> MembershipId { get; set; } //Here limit on if this group is owned by a particular mebership
        


        public virtual ICollection<Role> Roles { get; set; }
        public virtual Membership Membership { get; set; }
       
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
