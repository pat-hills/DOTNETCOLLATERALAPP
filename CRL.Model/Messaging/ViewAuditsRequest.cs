using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.FS.Enums;
using CRL.Infrastructure.Messaging;


namespace CRL.Model.Messaging
{
   
    public class ViewAuditsRequest : PaginatedRequest
    {
        public DateRange CreatedRange { get; set; }
        public string Message { get; set; }
        public int? MembershipId { get; set; }
        public string NameOfUser { get; set; }
        public string NameOfUserLogin { get; set; }
        public int? UserId { get; set; }
        public string AuditAction { get; set; }
        public string MachineName { get; set; }
        public int? ClientId { get; set; }
        public string ClientName { get; set; }
        public string GridRequestUrl { get; set; }
        public bool LimitTo { get; set; }
        public Nullable<DateTime> NewCreatedOn { get; set; }
        public string NameOfLegalEntity { get; set; }
        public string AuditCategoryType { get; set; }
        public int?[] AuditTypeId { get; set; }
        public int?[] AuditActionTypes { get; set; }

        public int? AuditCategory { get; set; }
        public int? AuditActionType { get; set; }

    }
}
