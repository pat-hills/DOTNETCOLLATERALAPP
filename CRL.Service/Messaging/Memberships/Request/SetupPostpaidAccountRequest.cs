using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Service.Messaging.Workflow.Request;
using CRL.Infrastructure.Messaging;
using CRL.Model.Messaging;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Messaging.Memberships.Request
{
    public class SetupPostpaidAccountRequest : HandleWorkItemRequest
    {
        public int Id { get; set; }
        public MembershipView MembershipView { get; set; }
        public RequestMode PostpaidRequestMode { get; set; }
    }

    public class RevokePostpaidAccountRequest :RequestBase
    {
        public int Id { get; set; }
       
    }
}
