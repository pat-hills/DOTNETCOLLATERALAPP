using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;
namespace CRL.Service.Messaging.Memberships.Request
{
    public class GetSubmitPostpaidResponse:ResponseBase
    {
        public ICollection<LookUpView> RegularClientInstitutions { get; set; }
        public bool CurrentClientIsBank { get; set; }
        public bool HasPendingPostpaidAccount { get; set; }
    }

    public class GetMembershipViewResponse : ResponseBase
    {
        //public ICollection<LookUpView> RegularClientInstitutions { get; set; }
        public MembershipView MembershipView { get; set; }
    }

    public class GetMembershipViewRequest : RequestBase
    {
        //public ICollection<LookUpView> RegularClientInstitutions { get; set; }
        public int? MembershipId { get; set; }
    }
}
