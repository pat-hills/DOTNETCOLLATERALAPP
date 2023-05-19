using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Areas.Administration.Models.ViewPageModels
{
    public class EmailViewModel
    {
        public int Id { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string EmailTo { get; set; }
        public string EmailFrom { get; set; }
        public bool IsSent { get; set; }
        public bool IsActive { get; set; }
    }
}