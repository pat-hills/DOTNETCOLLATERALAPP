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
    public class PasswordResetRequest : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        
        public string RequestCode { get; set; }
        public int? ResetUserId { get; set; }
        public virtual User ResetUser { get; set; }
        public bool RequestHandled { get; set; }
        public string RequestCodeSalt { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
