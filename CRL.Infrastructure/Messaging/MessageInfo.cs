using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Infrastructure.Messaging
{
    public class MessageInfo
    {
        private const string STR_BusinessValidationError="Business validation errors were detected!";
       
        public MessageInfo()
        {
            MessageType = MessageType.Info;
            Message = "";
        }

        public MessageInfo(MessageType messageType)
        {
            MessageType = messageType;
            if (MessageType == Messaging.MessageType.BusinessValidationError)
                Message = STR_BusinessValidationError;
        }
        public string Message { get; set; }


        public MessageType MessageType { get; set; }

        


    }
}
