using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.MembershipRegistration
{
    public class MembershipRegistrationsGridView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SecuredPartyType { get; set; }
        public string AccountType { get; set; }
        public string AccountNo {get;set;}
        public System.Nullable<DateTime> CreatedOn { get; set; }
        public bool IsValid { get; set; }
    }
}
