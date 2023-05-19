using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;


namespace CRL.Service.Messaging.Memberships.Request
{
    public class ViewMembershipRegistrationsRequest : PaginatedRequest
    {
        public DateRange CreatedRange { get; set; }
        public string Name { get; set; }
        
    }
}
