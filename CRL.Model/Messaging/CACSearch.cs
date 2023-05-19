using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRL.Model.Messaging
{
    [Serializable]
    public class CACSearch
    {
        public string Name { get; set; }
        public string Deed_Title { get; set; }
        public string Asset { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Serial_No { get; set; }
        public string Amount { get; set; }
        public string In_Words { get;set;}
        public string Currency { get; set; }
        public string Bank { get; set; }
        public string Date { get; set; }
        public string BVN { get; set; }
    }
}