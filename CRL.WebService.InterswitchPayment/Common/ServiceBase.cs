using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRL.Infrastructure.Messaging;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace CRL.WebService.InterswitchPayment.Common
{
    public class ServiceBase
    {
        protected void HttpRequestToServiceRequest(RequestBase request)
        {
            RemoteEndpointMessageProperty messageProperty = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            request.UserIP = messageProperty.Address;
            request.RequestUrl = OperationContext.Current.RequestContext.RequestMessage.Headers.To.ToString();                 
        }
    }
}