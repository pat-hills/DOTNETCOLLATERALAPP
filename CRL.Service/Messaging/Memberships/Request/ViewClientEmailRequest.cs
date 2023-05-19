using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.Memberships.Request
{
    public class ViewClientEmailRequest : PaginatedRequest
    {
        public DateRange CreatedOn { get; set; }
        public String EmailSubject { get; set; }
        public int? MembershipId { get; set; }
        public int AssignmentId { get; set; }
        public Nullable<DateTime> NewCreatedOn { get; set; }
        public bool IsAdminMode { get; set; }
    }
}
