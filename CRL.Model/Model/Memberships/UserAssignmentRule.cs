using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;

namespace CRL.Model.Memberships
{

    //public class WFUserAssignmentConfigurationRule : AuditedEntityBaseModel<int>, IAggregateRoot
    //{
    //    public WFUserAssignmentConfigurationRule()
    //    {
    //        LimitToRoles = new HashSet<Role>();
    //        LimitToUsers = new HashSet<User>();
    //        LimitRuleToMembership = new HashSet<Membership>();
    //    }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public bool AssignPreviousSender { get; set; }
    //    public bool AssignCaseCreator { get; set; }
    //    public virtual ICollection<Role> LimitToRoles { get; set; }
    //    public bool LimitRoleToASpecificMembership { get; set; }
    //    public bool LimitRoleUsersToRefUsrMembership { get; set; }  //Limit to users in owner's institution
    //    public bool LimitRoleUsersToRefUsrUnit { get; set; } //Limit to owner's unit
    //    public virtual ICollection<User> LimitToUsers { get; set; }
    //    public virtual ICollection<Membership> LimitRuleToMembership { get; set; }
    //    protected override void CheckForBrokenRules()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class WFWorkflowNotificationConfigurationRule : AuditedEntityBaseModel<int>, IAggregateRoot
    //{
    //    public WFWorkflowNotificationConfigurationRule()
    //    {
    //        LimitToRoles = new HashSet<Role>();
    //        LimitToUsers = new HashSet<User>();
    //        LimitRuleToMembership = new HashSet<Membership>();
    //    }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public bool NotifyAssignedUsers { get; set; }
    //    public bool NotifySender { get; set; }
    //    public bool NotifyAllInvolvedEmails { get; set; }
    //    public virtual ICollection<Role> LimitToRoles { get; set; }
    //    public bool LimitRoleUsersToRefUsrMembership { get; set; }  //Limit to users in owner's institution
    //    public bool LimitRoleUsersToRefUsrUnit { get; set; } //Limit to owner's unit
    //    public virtual ICollection<User> LimitToUsers { get; set; }
    //    public virtual ICollection<Membership> LimitRuleToMembership { get; set; }
    //    protected override void CheckForBrokenRules()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

   
   

    ////Class that reads a business rule and gets users for that.  Should be in the service layer
    //public class UserAssignmentRuleProcessor
    //{

    //    public List<User> GetUsers(UserAssignmentConfigurationRule rule)
    //    {
    //        //if (rule is UserAssignmentFromUserListRule)
    //        //{
    //        //    return ((UserAssignmentFromUserListRule)rule).IncludeUsers.ToList();
    //        //}

    //        return null;
    //    }
    //}
}
