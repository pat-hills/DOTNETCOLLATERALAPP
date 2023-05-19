using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.Infrastructure;
using CRL.Model.ModelViews;
using CRL.Model.WorkflowEngine;
using CRL.Model.Memberships;

namespace CRL.Model.Configuration
{
    public class RoleLimitRule : EntityBase<int>, IAggregateRoot
    {
        public Role Role {get;set;}
        public Roles RoleId { get; set; }
        public bool LimitToInstitution {get;set;}
        public short AllUnitsOrLimitToUnit { get; set; }
        public virtual WorkflowPlaceAssignmentConfiguration WorkflowPlaceAssignmentConfiguration { get; set; }
        public int WorkflowPlaceAssignmentConfigurationId { get; set; }


        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
    public class WorkflowPlaceAssignmentConfiguration : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public WorkflowPlaceAssignmentConfiguration()
        {
            RoleLimitRules = new HashSet<RoleLimitRule>();
        }
        public virtual ICollection<RoleLimitRule> RoleLimitRules { get; set; }
        public bool ReAssignToPreviousSender { get; set; }
        public bool ReAssignToCaseCreator { get; set; }
        public bool ForReAssignsIncludeOtherRoles { get; set; }
        public int PlaceId { get; set; }
        public WFPlace Place {get;set;}
        public System.Nullable <WFTaskType> TaskId { get; set; }
        public WFTask Task { get; set; }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
