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
    public class RoleCategory : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public RoleCategory()
        {
            this.Roles = new HashSet<Role>();
        }
        [MaxLength(50)]
        public string RoleCategoryName { get; set; }
    
        //Relationships
        public virtual ICollection<Role> Roles { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
