using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Enums;
using CRL.Service.Common;

using CRL.Model.ModelViews;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Messaging.User.Request
{

    public class GetUserRequest : RequestBase
    {
        public int? Id { get; set; }
        public int? InstitutionId { get; set; }
        public int? InstitutionUnitId { get; set; }
        public UserView UserView { get; set; }
        //public EditMode EditMode { get; set; }
        public bool IgnoreLoadingUserView { get; set; }
    }
    public class GetUserCreateRequest : GetUserRequest
    {
      
    }
    public class GetUserEditRequest : GetUserRequest
    {

    }
    public class GetUserViewRequest : GetUserRequest
    {

    }

    public class GetDataForEditUserRolesListResponse : RequestBase
    {
        public UserView UserView { get; set; }
        public TypeOfUser TypeOfUser { get; set; }
    }


}
