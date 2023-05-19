using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Model.ModelViews;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Model.Messaging
{
  
    public class ViewUsersResponse:ResponseBase
    {
        public ICollection<UserGridView> UserGridView { get; set; }
        public int NumRecords { get; set; }


    }

    public class ViewUserRolesResponse:ResponseBase
    {
        public ICollection<RoleGridView> RoleGridView { get; set; }
        public int NumRecords { get; set; }


    }
}
