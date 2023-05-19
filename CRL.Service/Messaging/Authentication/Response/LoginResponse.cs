using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Messaging;


namespace CRL.Service.Messaging.Authentication.Response
{
    public class LoginResponse:ResponseBase
    {
        public LoginResponse() : base() { }
        public LoginResponse(string message, MessageType messageType) : base(message, messageType) { }
        public  SecurityUser User { get; set; }
        public bool PasswordChangeRequired { get; set; }
        public bool PasswordWarnOfChangeRequired { get; set; }
        public int NumPasswordChangeDaysRemaining { get; set; }
        public string ResetPasswordNextLoginCode { get; set; }
    }

    public class LogoffResponse : ResponseBase 
    {

    }
}
