using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.Configuration;

namespace CRL.Model.WorkflowEngine
{
    public partial class WFPlace : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public WFPlace()
        {
            Arcs = new HashSet<WFArc>();
            WorkflowPlaceAssignmentConfigurations = new HashSet<WorkflowPlaceAssignmentConfiguration>();
         
        }
        public int WorkflowId { get; set; }
        public int PlaceType { get; set; }
        public bool LockItemInThisPlace { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool TasksNotAllowedInHandleProcess { get; set; }
        public bool ReAssignToPreviousSender { get; set; }
        public bool ReAssignToCaseCreator { get; set; }
        public bool ForReAssignsIncludeOtherRoles { get; set; }
        public virtual WFWorkflow  Workflow { get; set; }
        public virtual ICollection<WorkflowPlaceAssignmentConfiguration> WorkflowPlaceAssignmentConfigurations { get; set; }
        public virtual ICollection<WFArc > Arcs { get; set; }
        
        public List<WFArc>  GetInWardArcs()
        {  
            return Arcs.Where(s => s.ArcDirections =="IN" ).ToList ();
        }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
