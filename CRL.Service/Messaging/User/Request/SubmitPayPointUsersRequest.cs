using CRL.Infrastructure.Messaging;
using CRL.Model.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Messaging.User.Request
{
    public class SubmitPayPointUsersRequest : HandleWorkItemRequest
    {
        public int[] PaypointUsersIds { get; set; }
        public RequestMode PostpaidRequestMode { get; set; }
    }
}
