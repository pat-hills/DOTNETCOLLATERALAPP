using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Views.Memberships;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Messaging.Memberships.Response
{
    public class MembershipRegistrationResponse : ResponseBase
    {
        public MembershipRegistrationView MembershipRegistrationView { get; set; }
        public bool RegistrationWasCreatedOrSubmitted { get; set; }
        public string ClientCode { get; set; }
    }
}
