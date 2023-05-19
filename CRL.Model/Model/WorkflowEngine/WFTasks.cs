using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;

namespace CRL.Model.WorkflowEngine
{
    public enum WFTaskType
    {
        CreateRegistration = 1, UpdateRegistration = 2, DischargeRegistration = 3,  SubordinateRegistration = 5,
        AssignRegistration = 7, PostpaidSetup=8, ClientRegistration = 9, PaypointUserAssigment=10, DischargeRegistrationDueToError=11}
    public class WFTask : AuditedEntityBaseModel<WFTaskType>, IAggregateRoot    
    {
        public string Name { get; set; }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
