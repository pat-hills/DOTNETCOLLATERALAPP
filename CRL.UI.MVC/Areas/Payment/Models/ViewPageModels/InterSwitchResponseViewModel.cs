using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class InterSwitchResponseViewModel
    {
        public string txnref { get; set; }
        public string payRef { get; set; }
        public string retRef { get; set; }
        public int cardNum { get; set; }
        public int apprAmt { get; set; }
        public string desc { get; set; }
        public string resp { get; set; }
    }
}