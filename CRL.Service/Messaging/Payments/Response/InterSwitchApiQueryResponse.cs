using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;
using CRL.Service.Views.Payments;

namespace CRL.Service.Messaging.Payments.Response
{
    public class InterSwitchApiQueryResponse:ResponseBase
    {
        public string ResponseCode { get; set; }
        public bool IsConnectivityError { get; set; }
    }
}
