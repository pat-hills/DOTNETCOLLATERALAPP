using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.WorkflowEngine;
using CRL.Model.WorkflowEngine.Enums;
using CRL.Service.Common;

using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Views.Workflow
{
    public class TaskHandleView
    {

        public ICollection<LookUpView> Tasks { get; set; } //Lookup list of tasks
        public string[] TaskTransitions { get; set; }
        public int[] MandatoryCommentTasks { get; set; }
        public ICollection<UserView> UsersToAssign { get; set; }
        public string[] EmailsToNotify { get; set; }

        public UserView AssigningUser { get; set; }
        public string AssigningUserAction { get; set; }
        [Display(Name = "Comment by User")]
        public string AssingingUserComment { get; set; }
        [Display(Name = "Status")]
        public string CurrentStatus { get; set; }
        [Display(Name = "Title")]
        public string WorkflowName { get; set; }
        public string CaseTitle { get; set; }
        public WorkflowRequestType CaseType { get; set; }
        [Required]
        [Display(Name = "Select Action")]
        public int SelectedTask { get; set; }

        [Display(Name = "Add Comment")]
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }
        public bool TasksNotAllowedInHandleProcess { get; set; }
        public int? AssociatedItemId { get; set; }
        public int? ApprovingTask { get; set; }
        public int? CaseId { get; set; }
        public int? WorkflowId { get; set; }
        public WFTaskType WFTaskType { get; set; }
    }
}
