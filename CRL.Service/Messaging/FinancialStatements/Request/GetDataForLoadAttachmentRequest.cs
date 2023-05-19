using CRL.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CRL.Service.Messaging.FinancialStatements.Request
{
    public enum AttachmentType { Verification = 1, FileAttachment = 2, TemporaryFileAttachment = 3 }
    public class GetFileAttachmentRequest : RequestBase
    {
        public int FinancialStatmentId { get; set; }
        public AttachmentType FileAttachmentType { get; set; }
    }
}
