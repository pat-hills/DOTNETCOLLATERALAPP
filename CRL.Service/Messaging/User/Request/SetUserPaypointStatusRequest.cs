using CRL.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CRL.Service.Messaging.User.Request
{
    public class SetUserPaypointStatusRequest:RequestBase
    {
        public int Id { get; set; }
        public short SetPaypointOnOrOff { get; set; }
    }
}
