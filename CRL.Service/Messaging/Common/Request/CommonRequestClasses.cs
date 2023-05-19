using CRL.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CRL.Service.Messaging.Common.Request
{
    public class ChangeItemStatusRequest : RequestBase
    {
        
        public bool Activate { get; set; }
    }

    public class DeleteItemRequest : RequestBase
    {
        

    }

    public class UnlockItemRequest : RequestBase 
    {

    }
}
