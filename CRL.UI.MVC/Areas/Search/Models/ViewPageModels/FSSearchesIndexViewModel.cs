using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.UI.MVC.Common;

namespace CRL.UI.MVC.Areas.Search.Models.ViewPageModels
{
   
    public class FSSearchesIndexViewModel : BaseDetailViewModel
    {
        public FSSearchesIndexViewModel()
            : base()
        {
            _FSSearchesViewModel = new _FSSearchesViewModel();
        }

        public _FSSearchesViewModel _FSSearchesViewModel { get; set; }
        
    }

}