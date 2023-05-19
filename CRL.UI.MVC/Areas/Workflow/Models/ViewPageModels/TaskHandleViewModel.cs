using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Model.WorkflowEngine.Enums;
using CRL.Service.Messaging.Workflow.Request;
using CRL.UI.MVC.Areas.FinancialStatement.Models.ViewPageModels;
using CRL.UI.MVC.Areas.Membership.Models.ViewPageModels.Client;
using CRL.UI.MVC.Areas.Membership.Models.ViewPageModels.Shared.Institution;
using CRL.UI.MVC.Areas.Membership.Models.ViewPageModels.Shared.User;
using CRL.UI.MVC.Areas.Workflow.Models.ViewPageModels.Shared;
using CRL.UI.MVC.Common;
using CRL.Model.WorkflowEngine;
using CRL.Model.ModelViews;

namespace CRL.UI.MVC.Areas.Workflow.Models.ViewPageModels
{
  

    public class TaskHandleViewModel : BaseDetailViewModel
    {

        public _TaskHandleViewModel _TaskHandleViewModel { get; set; }
        public _SummaryViewAmendmentViewModel _SummaryViewAmendmentViewModel { get; set; }
        public NewAndPreviousFSViewModel NewAndPreviousFS { get; set; }
        public FSViewModel FSViewModel { get; set; }      
        public WorkflowRequestType CaseType { get; set; }
        public WFTaskType wfTaskType { get; set; }
        public int CaseId { get; set; }
        public _SetupPostpaidMembershipViewModel _SetupPostpaidMembershipViewModel { get; set; }
        public _InstitutionViewModel _InstitutionViewModel { get; set; }
        public _UserViewModel _UserViewModel { get; set; }
        public _UsersListViewModel _UsersListViewModel { get; set; }
        public int[] Actions { get; set; }     
       
    }
}