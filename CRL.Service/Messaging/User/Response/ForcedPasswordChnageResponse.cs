using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Authentication;

using CRL.Model.ModelViews;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.User.Response
{
    public class ForcedPasswordChangeResponse:ResponseBase
    {
        public SecurityUser User { get; set; }
    }
}
