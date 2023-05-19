using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Administration;

namespace CRL.Service.Messaging.Configuration.Response
{
    public class CreateGMResponse : ResponseBase
    {
        public GlobalMessageDetailsView GlobalMessageDetailsView { get; set; }
        public int[] Roles { get; set; }
        public ICollection<LookUpView> MessageRolesList { get; set; }
        public ICollection<LookUpView> SelectedMessageRolesList { get; set; }
    }
}
