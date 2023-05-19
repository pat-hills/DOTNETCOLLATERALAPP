using CRL.Model.Memberships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.FS
{
    public class ActivityAssignment : FinancialStatementActivity
    {
        public int AssignmentType { get; set; }
        public virtual Membership AssignedMembership { get; set; }
        public int AssignedMembershipId { get; set; }
        public virtual Membership AssignedMembershipFrom { get; set; }
        public int AssignedMembershipFromId { get; set; }
        public string Description { get; set; }

          public new ActivityAssignment Duplicate()
        {
            ActivityAssignment activity = new ActivityAssignment();
            activity.AssignmentType   = this.AssignmentType ;
            activity.AssignedMembershipId  = this.AssignedMembershipId;
            activity.Description = this.Description;
            return activity;
        }
    }
}
