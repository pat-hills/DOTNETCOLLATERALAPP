using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Messaging.EmailAdministration.Request
{
    public class ViewEmailRequest : PaginatedRequest
    {
        public int AllOrUnsent { get; set; }
        public DateRange SentDate { get; set; }
    }
}
