using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using CRL.Model.ModelViews;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Messaging.Memberships.Request
{
    public class GetClientSummaryViewResponse:ResponseBase
    {
        public ClientSummaryView ClientSummaryView { get; set; }
    }
}
