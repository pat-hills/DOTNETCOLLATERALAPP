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
    public partial class WFWorkflow : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public WFWorkflow()
        {
            Places = new HashSet<WFPlace>();
            Transitions = new HashSet<WFTransition >();
            Cases = new HashSet<WFCase >();
           
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ObjectName { get; set; }  //Refers to the name of the business object    
        public int StartTaskId { get; set; }

        public virtual ICollection<WFPlace > Places { get; set; }
        public virtual ICollection<WFTransition > Transitions { get; set; }
        public virtual ICollection<WFCase> Cases { get; set; }
       

        public WFPlace GetStartingPlace()
        {
            return Places.Where(s => s.PlaceType == 1 ).Single();
        }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }


}
