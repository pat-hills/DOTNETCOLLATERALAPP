using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Infrastructure.Enums;
using CRL.UI.MVC.Common;

namespace CRL.UI.MVC.Areas.Administration.Models.ViewPageModels
{
    public class GlobalMessageDetailsViewModel : BaseDetailViewModel
    {
        public GlobalMessageDetailsViewModel()
        {
            _GlobalMessageDetailsViewModel = new _GlobalMessageDetailsViewModel();
        }
    
        public _GlobalMessageDetailsViewModel _GlobalMessageDetailsViewModel { get; set; }
        public bool InUpdateMode { get; set; }
    }
}