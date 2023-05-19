using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Messaging.Common.Request;
using CRL.Service.Messaging.User.Request;
using CRL.Service.Messaging.User.Response;
using CRL.Infrastructure.Messaging;
using CRL.Model.Messaging;

namespace CRL.Service.Interfaces
{
    public interface IUserService
    {
        GetUserCreateResponse GetCreate(GetUserCreateRequest request);
        GetUserEditResponse GetEdit(GetUserEditRequest request);
        GetUserViewResponse GetView(GetUserViewRequest request);
        CreateEditUserResponse CreateUser(CreateEditUserRequest request);
        CreateEditUserResponse EditUser(CreateEditUserRequest request);
        ResponseBase EditUserRolesList(EditUserRolesRequest request);
        ViewUserRolesResponse ViewUserRoles(ViewUserRolesRequest request);
        ViewUsersResponse ViewUsers(ViewUsersRequest request);
        ResponseBase ResetPassword(ResetPasswordRequest request);
        ResponseBase ChangePasswordReset(ChangePasswordResetRequest request);
        ResponseBase SetUserPaypointStatus(SetUserPaypointStatusRequest request);
        ResponseBase RevokePaypointStatus(RequestBase request);
        ForcedPasswordChangeResponse ForcePasswordChange(ForcePasswordChangeRequest request);
        ResponseBase ChangeUserStatus(ChangeItemStatusRequest request);
        ResponseBase DeleteUser(DeleteItemRequest request);
        ResponseBase ProcessPayPointUsersRequest(SubmitPayPointUsersRequest request);
        ResponseBase UnlockUser(UnlockItemRequest request);
        ResponseBase CheckPasswordResetCode(CheckPasswordResetCodeRequest request);

    }
}
