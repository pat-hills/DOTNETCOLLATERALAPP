using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.ModelViews;
using CRL.Model.Memberships;

namespace CRL.Model.WorkflowEngine
{
    public partial class WFToken : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public WFToken()
        {
            this.TokenEnabledDate = DateTime.Now;
            this.TokenStatus = "FREE";
            AssignedUsers = new HashSet<User>();
           
        }
      
       
        public int CaseId { get; set; }
        //public int WorkflowId { get; set; }
        public int  PlaceId { get; set; }
        public string TokenContext { get; set; }
        public string TokenStatus { get; set; }
        public DateTime? TokenEnabledDate { get; set; }
        public DateTime? TokenCancelledDate { get; set; }
        public DateTime? TokenConsumedDate { get; set; }
        public int? TokenConsumedBy { get; set; }
        public int? TokennLockedBy { get; set; }
        public int? LimitToMembershipId { get; set; }
        public Membership LimitedToMembership { get; set; }
        public int? LimitToUnitId { get; set; }
        public InstitutionUnit LimitToUnit { get; set; }
        public virtual ICollection<User> AssignedUsers { get; set; }       
        //public virtual WFWorkflow Workflow { get; set; }
        public virtual WFCase  Case { get; set; }
        public virtual WFPlace Place { get; set; }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
