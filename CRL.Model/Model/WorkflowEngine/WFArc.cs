using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;

namespace CRL.Model.WorkflowEngine
{
    public partial class WFArc : AuditedEntityBaseModel<int>, IAggregateRoot
    {
      
      
        public int TransitionId { get; set; }
        public int PlaceId { get; set; }
        public string ArcDirections { get; set; }
        public string ArcType { get; set; }

      
        public virtual WFTransition Transition { get; set; }
        public virtual WFPlace  Place { get; set; }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
