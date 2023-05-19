using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;
using CRL.Service.Views.Memberships;

namespace CRL.Service.Messaging.Memberships.Response
{
    public class ViewClientEmailResponse : ResponseBase
    {
        public ICollection<ClientEmailView> EmailViews { get; set; }
        public ClientEmailView ClientEmailView { get; set; }
        public int NumRecords { get; set; }
    }
}
