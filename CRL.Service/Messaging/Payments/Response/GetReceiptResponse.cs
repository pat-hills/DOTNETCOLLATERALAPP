using CRL.Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CRL.Service.Messaging.Payments.Response
{
    public class GetReceiptResponse:ResponseBase
    {
        public byte[] AttachedFile { get; set; }
        public string AttachedFileName { get; set; }
        public string AttachedFileType { get; set; }
        public string AttachedFileSize { get; set; }
    }
}
