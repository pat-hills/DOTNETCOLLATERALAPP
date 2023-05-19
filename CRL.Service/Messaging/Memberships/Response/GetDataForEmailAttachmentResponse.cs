using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.Memberships.Response
{
    public class GetDataForEmailAttachmentResponse : ResponseBase
    {
        public byte[] AttachedFile { get; set; }
        public string AttachedFileName { get; set; }
        public string AttachedFileType { get; set; }
        public string AttachedFileSize { get; set; }
    }
}
