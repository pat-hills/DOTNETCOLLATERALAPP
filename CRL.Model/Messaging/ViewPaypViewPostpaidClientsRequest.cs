using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Model.Messaging
{
    public class ViewPostpaidClientsRequest : PaginatedRequest
    {
        /// <summary>
        /// 1 - "All" , 2 - "My Postpaid Clients"
        /// </summary>
        public short? PostpaidFilter { get; set; }
        public int? RepresentativeMembershipId { get; set; }

    }

    public class ViewPostpaidClientsResponse:ResponseBase
    {
        public ICollection<ClientView> ClientView { get; set; }
        public int NumRecords { get; set; }

    }
}
