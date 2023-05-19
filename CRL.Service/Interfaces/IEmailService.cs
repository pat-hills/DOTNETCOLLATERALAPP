using CRL.Service.Messaging.Configuration.Request;
using CRL.Service.Messaging.Configuration.Response;
using CRL.Service.Messaging.EmailAdministration.Request;
using CRL.Service.Messaging.EmailAdministration.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Service.Messaging.Memberships.Request;
using CRL.Service.Messaging.Memberships.Response;
using CRL.Model;

namespace CRL.Service.Interfaces
{
    public interface IEmailService
    {
        ViewEmailResponse GetAllEmails(ViewEmailRequest request);
        ViewClientEmailResponse ViewClientEmails(ViewClientEmailRequest request);
        ViewClientEmailResponse ViewEmailDetails(ViewClientEmailRequest request);
        GetDataForEmailAttachmentResponse GetDataForEmailAttachment(GetDataForEmailAttachmentRequest request);
        ViewGlobalMessagesResponse GetAllGlobalMessages(ViewGlobalMessagesModelRequest request);
        CreateGMResponse GetCreateDetails(CreateGMRequest request);
        CreateSubmitGlobalMessageResponse CreateSubmitGm(CreateSubmitGlobalMessageRequest request);
        CreateSubmitGlobalMessageResponse UpdateSubmitGm(CreateSubmitGlobalMessageRequest request);
        DeleteGMResponse DeletSubmitGm(DeleteGMRequest request);
    }
}
