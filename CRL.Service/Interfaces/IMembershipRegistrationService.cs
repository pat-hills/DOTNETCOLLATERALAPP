using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Messaging.Memberships.Request;
using CRL.Service.Messaging.Memberships.Response;
using CRL.Service.Messaging.Workflow.Request;
using CRL.Model.Messaging;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Interfaces
{
    public interface IMembershipRegistrationService
    {
        GetCreateResponse GetCreate(GetCreateRequest request);
        //MembershipRegistrationResponse SubmitCreateMembershipRegistration(SubmitCreateEditMembershipRegistrationRequest request);       
        //ResponseBase Confirm(ConfirmMembershipRequest request);
        MembershipRegistrationResponse SubmitClientAccount(NewClientAccountRequest request);
        MembershipRegistrationResponse HandleNewClientAccount(HandleWorkItemRequest prequest);
        //MembershipRegistrationResponse SubmitClientForApproval(MembershipRegistrationRequest request);
        ResponseBase HandleTask(HandleWorkItemRequest request);
         GetDataforLGAResponse GetLGA(RequestBase request);
         GetBVCResponse VerifyBVC(GetBVCRequest request);
       
    }
}
