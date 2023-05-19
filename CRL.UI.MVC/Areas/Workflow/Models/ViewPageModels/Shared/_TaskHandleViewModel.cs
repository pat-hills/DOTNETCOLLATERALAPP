using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRL.Model.WorkflowEngine;
using CRL.Service.Views.Workflow;
using CRL.UI.MVC.Common;

namespace CRL.UI.MVC.Areas.Workflow.Models.ViewPageModels.Shared
{
    public class _TaskHandleViewModel:BaseDetailViewModel
    {
       public _TaskHandleViewModel()
       {
           
       }
        public TaskHandleView TaskHandleView { get; set; }
        public SelectList Tasks { get; set; }
        public bool isPayable { get; set; }
        public decimal Amount { get; set; }
        public bool isPostpaid { get; set; }
        public decimal CurrentCredit { get; set; }
      
    }
}