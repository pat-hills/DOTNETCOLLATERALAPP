using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Views.Memberships;
using CRL.Service.Messaging.Workflow.Request;
using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Model.Messaging;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Messaging.Memberships.Request
{
    public class SubmitCreateEditMembershipRegistrationRequest : RequestBase
    {
        public MembershipRegistrationView MembershipRegistrationView { get; set; }
        public string UrlLink { get; set; }
    }

    public class NewClientAccountRequest : HandleWorkItemRequest
    {
        public MembershipRegistrationView MembershipRegistrationView { get; set; }
        public RequestMode MembershipRegRequestMode { get; set; }
    }
}
