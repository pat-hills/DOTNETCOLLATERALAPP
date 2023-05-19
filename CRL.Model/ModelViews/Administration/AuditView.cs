using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.FS.Enums;

namespace CRL.Model.ModelViews.Administration
{
    public class AuditView
    {
        public int Id { get; set; }
        public DateTime AuditDate { get; set; } //(**)
        public string Message { get; set; } //jqgrid filter
        public string AuditAction { get; set; }
        public AuditAction  AuditActionId { get; set; }
        public string RequestUrl { get; set; }
        public string MachineName { get; set; }
        public string AuditType { get; set; } 
        public AuditCategory AuditTypeId { get; set; } //jqgrid filter - dropdown *we need lookup for this filed to filter by
        public string NameOfUser { get; set; }  //jqgrid filter (**)
        public string NameOfLegalEntity { get; set; } //Should show for cbl users
        public int? IdOfLegalEntity { get; set; }
        public string UserLoginId { get; set; }
    }
}
