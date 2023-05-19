using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Infrastructure.Helpers
{
    public class LookUpView
    {
        public int LkValue { get; set; }
        public string LkName { get; set; }
        //Boolean value to select a checkbox
        //on the list
        public bool IsSelected { get; set; }

        //Object of html tags to be applied
        //to checkbox, e.g.:'new{tagName = "tagValue"}'
        public object Tags { get; set; }

    }

    public class AuditActionTypeView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? AuditCategoryId { get; set; }
    }
}
