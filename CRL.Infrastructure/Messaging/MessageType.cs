using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Infrastructure.Messaging
{
    public enum MessageType
    {
        Info,
        Warning,
        Error,
        BusinessValidationError,
        Success,
        Unauthorized,
        AlreadyHandled,
        DatabaseConcurrencyConflict,
        DatabaseUniqueKeyConflict

    }
}
