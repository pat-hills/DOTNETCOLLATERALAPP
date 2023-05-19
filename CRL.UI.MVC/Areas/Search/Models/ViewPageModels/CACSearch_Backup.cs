using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.UI.MVC.Areas.Payment.Models.ViewPageModels
{
    public class CACSearch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Deed_Title { get; set; }
        public string Asset { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Serial_No { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string Bank { get; set; }
        public DateTime Date { get; set; }
        public string BVN { get; set; }
    }
}