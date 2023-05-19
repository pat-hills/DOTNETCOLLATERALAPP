using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Views.Memberships;
using CRL.Model.ModelViews;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Messaging.User.Response
{
    public class CreateEditUserResponse:ResponseBase
    {
        public UserView UserView { get; set; }
    }
}
