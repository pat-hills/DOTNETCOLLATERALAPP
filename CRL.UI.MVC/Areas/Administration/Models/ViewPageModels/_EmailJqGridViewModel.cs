using CRL.UI.MVC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Areas.Administration.Models.ViewPageModels
{
    public class _EmailJqGridViewModel : BaseSearchFilterViewModel
    {
 
        public _EmailJqGridViewModel() : base() {
   
        
        }
        public int AllOrUnsent { get; set; }
        public EmailFilterList EmailFilterList { get; set; }
    }
    public enum EmailFilterList {SentMails=1, UnsentMails=2, FailedMails=3,AllMails=4 }
}