using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.UI.MVC.Areas.Workflow.Models.ViewPageModels.Shared;
using CRL.UI.MVC.Common;

namespace CRL.UI.MVC.Areas.Workflow.Models.ViewPageModels
{
  

    public class TaskIndexViewModel : BaseDetailViewModel
    {
        public TaskIndexViewModel()
            : base()
        {
            _TaskJqGridViewModel = new _TaskJqGridViewModel();
        }

        public _TaskJqGridViewModel _TaskJqGridViewModel { get; set; }

    }
}