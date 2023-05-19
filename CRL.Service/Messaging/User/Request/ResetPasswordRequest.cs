using CRL.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CRL.Service.Messaging.User.Request
{
    public class ResetPasswordRequest:RequestBase
    {
        public string Email { get; set; }
        public string UrlLink { get; set; }

    }

  
    public class ChangePasswordResetRequest : RequestBase
    {
        public int Id { get; set; }
        public string RequestCode { get; set; }
        public string Password { get; set; }
        

    }

    public class CheckPasswordResetCodeRequest : RequestBase
    {
        public string RequestCode { get; set; }
    }

    public class ForcePasswordChangeRequest : RequestBase
    {
        public string OldPassword { get; set; }
        public string PasswordResetCode { get; set; }
        public string Password { get; set; }
        public bool IsNotForced { get; set; }

    }
}
