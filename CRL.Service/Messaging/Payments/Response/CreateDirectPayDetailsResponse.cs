using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.Payments.Response
{
    public class CreateDirectPayDetailsResponse : ResponseBase
    {
        public string PublicVoucherCode { get; set; }
    }
}
