using CRL.UI.MVC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Areas.Administration.Models.ViewPageModels
{
    public class EmailsIndexViewModel : BaseDetailViewModel
    {

        public EmailsIndexViewModel(){
        
            _EmailJqGridViewModel = new _EmailJqGridViewModel();
        }

        public _EmailJqGridViewModel _EmailJqGridViewModel { get; set; }
    }
}