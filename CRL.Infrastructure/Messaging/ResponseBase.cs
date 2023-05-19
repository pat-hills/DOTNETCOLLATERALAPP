using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.Messaging;

namespace CRL.Infrastructure.Messaging
{
    public class ResponseBase
    {
        public ResponseBase()
        {
            MessageInfo = new MessageInfo();
            ValidationErrors = new List<ValidationError>();
        }
        public ResponseBase(string message, MessageType messageType)
        {
            MessageInfo = new MessageInfo() { MessageType = messageType, Message = message };
            ValidationErrors = new List<ValidationError>();
        }
        public void GenerateDefaultSuccessMessage()
        {
            MessageInfo = new MessageInfo() { MessageType = MessageType.Success, Message = "Operation was successful" };
            
         
        }

        public void GenerateDefaultUnauthorisedMessage()
        {
            MessageInfo = new MessageInfo() { MessageType = MessageType.Unauthorized , Message = "You are not authorised to perform this operation" };
            
        }

        public void SetResponseToResponseBase(ResponseBase Base)
        {
            this.MessageInfo = Base.MessageInfo;
            this.ValidationErrors = Base.ValidationErrors;
            
        }

        public MessageType GetMessageType {get { return MessageInfo .MessageType ;} private  set {;}}
        public MessageInfo MessageInfo { get; set; }
        public List<ValidationError> ValidationErrors { get; set; }



        public bool HasPendingPostpaidAccount { get; set; }
    }

    public class ReportResponseBase : ResponseBase
    {
        public int NumRecords { get; set; }
    }
}
