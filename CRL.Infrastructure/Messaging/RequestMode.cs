using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Infrastructure.Messaging
{
    public enum RequestMode { Create = 1, Submit = 2, Resend = 3, Approval = 4, Deny = 5, SemiApproval = 6 }
}
