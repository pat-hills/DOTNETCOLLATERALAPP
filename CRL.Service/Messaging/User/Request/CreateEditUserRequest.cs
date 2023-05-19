using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Views.Memberships;

using CRL.Model.ModelViews;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Messaging.User.Request
{
    public class CreateEditUserRequest:RequestBase
    {
        public UserView UserView { get; set; }
        public int NotifyUser { get; set; }
        public string UrlLink { get; set; }
    }

    public class EditUserRolesRequest : RequestBase
    {
        public ICollection<RoleGridView> RoleGridView { get; set; }
        public int UserId { get; set; }
      
    }
   
}
