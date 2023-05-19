using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Messaging;
using CRL.Service.Messaging.Memberships.Request;
using CRL.Service.Messaging.Payments.Request;
using CRL.Service.Messaging.Payments.Response;
using CRL.Service.Messaging.Workflow.Request;
using CRL.Infrastructure.Messaging;
using CRL.Model.Messaging;

namespace CRL.Service.Interfaces
{
    public interface IClientService
    {
        ViewPostpaidClientsResponse ViewPostpaidClients(ViewPostpaidClientsRequest request);
        GetSubmitPostpaidResponse GetSubmitPostpaid(RequestBase request);
        ResponseBase SubmitPostpaidAccount(SetupPostpaidAccountRequest request);
        ResponseBase HandleTask(HandleWorkItemRequest request);
        ResponseBase RevokePostpaidAccount(SetupPostpaidAccountRequest request);
        ViewCreditActivitiesResponse ViewCreditActivities(ViewCreditActivitiesRequest request);
        ViewAuditsResponse ViewAudits(ViewAuditsRequest request);
        ViewAuditDetailsResponse ViewAuditDetails(ViewAuditDetailsRequest request);
        GetMembershipViewResponse GetDataForMembershipView(GetMembershipViewRequest request);
    }
}
