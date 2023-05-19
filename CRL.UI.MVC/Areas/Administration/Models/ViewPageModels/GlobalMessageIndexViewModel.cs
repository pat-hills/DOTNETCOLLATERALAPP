using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.UI.MVC.Common;

namespace CRL.UI.MVC.Areas.Administration.Models.ViewPageModels
{
    public class GlobalMessageIndexViewModel : BaseDetailViewModel
    {
        public GlobalMessageIndexViewModel()
        {
            _GlobalMessageJqGridViewModel =  new _GlobalMessageJqGridViewModel();
        }

        public _GlobalMessageJqGridViewModel _GlobalMessageJqGridViewModel { get; set; }
    }
}