using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.Memberships;

namespace CRL.Model.Configuration
{
    public class ConfigurationWorkflow: AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public bool CreateNewFS{ get; set; }
        public bool UpdateFS { get; set; }
        public bool DischargeFS { get; set; }
        public bool SubordinateFS { get; set; }
        public bool AssignmentFS { get; set; }
        public bool UnitLimit { get; set; }
        public bool TaskAssignmentNotification { get; set; }
        public bool SkipWhenSubmitterIsAuthroizer { get; set; }
        public bool NotifyAllAssignedAfterTaskExecution { get; set; }
        public bool NotifyAllAfterWorkflowClose { get; set; }
        public bool AllowWorkflowWhenNoUserAssigned { get; set; }
        public bool NotifyAdministratorWhenNoUserAssigned { get; set; }
        public bool SkipWorkflowForIndividuals { get; set; }
        public bool WorkflowDelayedWarning { get; set; }
        public int NumDaysWorkflowIsDelayed { get; set; }
        public bool AllowUsersToSelectAssignedUsers { get; set; }
        public bool AllowUsersToAddEmailsForTaskNotification { get; set; }
        public bool UseGlobalSettings { get; set; }
        public bool EnforceWorkflowForUsersWithBothOfficerAndAuthorizerRoles { get; set; }

        public int? MembershipId { get; set; }
        public Membership Membership { get; set; }

        
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
