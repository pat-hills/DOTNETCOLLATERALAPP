using CRL.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CRL.Service.BusinessServices
{
    public static class ValidateSecurityOwnershipRule
    {
        public static ResponseBase Validate(RequestBase request,ResponseBase response,  int MembershipId)
        {
            if (!request.SecurityUser.IsInAnyRoles("Administrator (Owner)", "Administrator (Client)"))
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }

            

            //Check if we are not CBL then if we are the same membership
            if (request.SecurityUser.IsOwnerUser == false && MembershipId != request.SecurityUser.MembershipId)
            {
                response.MessageInfo = new Infrastructure.Messaging.MessageInfo { MessageType = Infrastructure.Messaging.MessageType.Unauthorized };
                return response;
            }


            return response;
        }

       
    }
}
