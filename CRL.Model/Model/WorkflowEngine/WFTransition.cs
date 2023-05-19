using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.ModelViews;

namespace CRL.Model.WorkflowEngine
{
    public enum TaskType { FinalApproval = 1, FinalDeny = 2, Resubmit = 3 }
    public partial class WFTransition : AuditedEntityBaseModel<int>, IAggregateRoot
    {

        public WFTransition()
        {
            Arcs = new HashSet<WFArc>();
            //TransitionAssignments = new HashSet<WFTransitionAssignment>();
         
        }
        public int WorkflowId { get; set; }
        public string Name { get; set; }
        public string ActionTakenName { get; set; }
        public string Description { get; set; }
        public string TaskTrigger { get; set; }
        public TaskType TaskType { get; set; }
        public virtual WFWorkflow Workflow { get; set; }
        public virtual ICollection<WFArc > Arcs { get; set; }
        public bool CommentMandatory { get; set; }
        //public virtual ICollection<WFTransitionAssignment> TransitionAssignments { get; set; }
       
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
