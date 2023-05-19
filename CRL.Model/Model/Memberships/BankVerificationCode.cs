using CRL.Infrastructure.Domain;
using CRL.Infrastructure.Messaging;
using CRL.Model.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.Memberships
{
    [Serializable]
    public class BankVerificationCode : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string level { get; set; }

        public static ResponseBase VerifyCode(BankVerificationCode verificationCode,   Membership  membership )
        {
            ResponseBase response = new ResponseBase();

            //Verify that code is in our database
            if (verificationCode == null)
            {
                response.MessageInfo.MessageType = MessageType.Error;
                response.MessageInfo.Message = "The bank or financial institution code does not exist!";
                return response;
            }

            //Verify that code is available for use
            if (membership != null)
            {
                response.MessageInfo.MessageType = MessageType.Error;
                response.MessageInfo.Message = "The bank or financial institution code is unavailable for use";
                return response;

            }
                     
            response.GenerateDefaultSuccessMessage();
            return response;          


        }
        
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }

    }
    
}
