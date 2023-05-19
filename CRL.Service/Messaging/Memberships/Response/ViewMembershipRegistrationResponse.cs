using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;

using CRL.Service.Views.MembershipRegistration;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.Memberships.Response
{
    public class ViewMembershipRegistrationsResponse : ResponseBase
    {
        public ICollection<MembershipRegistrationsGridView> MembershipRegistrationsGridView { get; set; }
        public int NumRecords { get; set; }
    }
}
