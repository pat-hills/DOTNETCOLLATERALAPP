using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Service.Common;

using CRL.Service.Views.Memberships;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Messaging.Memberships.Response
{
   

    public class GetCreateResponse:ResponseBase
    {
        public ICollection<LookUpView> SecuringPartyTypes { get; set; }  //Gets the type of institution
        public ICollection<LookUpView> RegularClientInstitutions { get; set; }
        public ICollection<LookUpView> Nationalities { get; set; }  //Gets the type of institution
        public ICollection<LookUpView> Countries { get; set; }
        public ICollection<LookUpView> Countys { get; set; }
        public ICollection<LookUpView> LGAs { get; set; }
        public MembershipRegistrationView MembershipRegistrationView { get; set; }
        public string SecuredCreditorCodeName { get; set; }
        

    }
}
