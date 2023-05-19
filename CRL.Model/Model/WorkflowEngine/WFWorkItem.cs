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
    public partial class WFWorkItem : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public WFWorkItem()
        {
            AssignedUsers = new HashSet<User>();
        }
        public int CaseId { get; set; }
        //public int WorkflowId { get; set; }
        public int TransitionId { get; set; }
        public string TransitionTrigger { get; set; }
        public string TaskId { get; set; }
        public string Context { get; set; }
        public string WorkitemStatus { get; set; }
        public DateTime? EnabledDate { get; set; }
        public DateTime? CancelledDate { get; set; }
        public DateTime?  FinishedDate { get; set; }
        public DateTime? DeadLine { get; set; }
        public int? ExecutingUserId { get; set; }
        public virtual User ExecutingUser { get; set; }
        public virtual ICollection<User> AssignedUsers { get; set; }
        //public virtual WFWorkflow Workflow { get; set; }
        public virtual WFCase Case { get; set; }
        public virtual WFTransition  Transition { get; set; }
        public         string ExecutingUserComment { get; set; }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
