using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;
using CRL.Service.Views;
using CRL.Service.Views.Payments;

namespace CRL.Service.Messaging.Payments.Request
{
    public class SubmitInterSwitchDetailsRequest:RequestBase
    {
        public InterSwitchUserView InterSwitchUserView { get; set; }
        public string TransactionReference { get; set; }
        public string SiteRedirectUrl { get; set; }
    }
}
