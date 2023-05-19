using CRL.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CRL.Service.Messaging.Authentication.Request
{
    public class LoginRequest:RequestBase
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LogoffRequest : RequestBase 
    {

    }
}
