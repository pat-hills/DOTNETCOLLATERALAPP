using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.Configuration
{
    public class ConfigurationWorkflowView
    {
        public int Id { get; set; }
        [Display(Name="Use workflow for Registration of financing statement")]
        public bool CreateNewFS { get; set; }
      [Display(Name = "Use workflow for Update of financing statement")]
        public bool UpdateFS { get; set; }
        [Display(Name = "Use workflow for Discharge of financing statement")]
        public bool DischargeFS { get; set; }
        [Display(Name = "Use workflow for Subordination of financing statement")]
        public bool SubordinateFS { get; set; }
        [Display(Name = "Use workflow for Transfer of financing statement")]
        public bool AssignmentFS { get; set; }
        [Display(Name = "Limit Task Assignments to Users within the Submitters Unit")]
        public bool UnitLimit { get; set; }
        [Display(Name = "Notify user when user is assigned a task")]
        public bool TaskAssignmentNotification { get; set; }
        [Display(Name = "Skip workflow when the submitter is an authorizer")]
        public bool SkipWhenSubmitterIsAuthroizer { get; set; }
        [Display(Name = "Notify all other users when a task assigned to them has been executed by another user")]
        public bool NotifyAllAssignedAfterTaskExecution { get; set; }
        [Display(Name = "Notify all officers and authorizers involved in a workflow when the workflow ends")]
        public bool NotifyAllAfterWorkflowClose { get; set; }
        [Display(Name = "Allow workflow to continue even when no authorizer has been found")]
        public bool AllowWorkflowWhenNoUserAssigned { get; set; }
        [Display(Name = "Notify Administrator when an authorizer or user is not found to handle task")]
        public bool NotifyAdministratorWhenNoUserAssigned { get; set; }
        [Display(Name = "Skip workflows for individuals")]
        public bool SkipWorkflowForIndividuals { get; set; }
        [Display(Name = "Notify Administrator when workflow delays")]
        public bool WorkflowDelayedWarning { get; set; }
        [Display(Name = "Number of days to mark workflow as delayed")]
        public int NumDaysWorkflowIsDelayed { get; set; }
        public bool AllowUsersToSelectAssignedUsers { get; set; }
        public bool AllowUsersToAddEmailsForTaskNotification { get; set; }
        public bool UseGlobalSettings { get; set; }
        public int? MembershipId { get; set; }
        [Display(Name="Enforce workflow for users with both officer and authorizer roles")]
        public bool EnforceWorkflowForUsersWithBothOfficerAndAuthorizerRoles { get; set; }
    }
}
