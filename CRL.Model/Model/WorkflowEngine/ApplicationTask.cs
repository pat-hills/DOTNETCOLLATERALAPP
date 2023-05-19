using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;

namespace CRL.Model.WorkflowEngine
{
    public class ApplicationTask : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public string Name { get; set; }
        public string Task { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
