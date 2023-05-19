using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Service.Views.FinancialStatement;

namespace CRL.Service.Messaging.FinancialStatements.Request
{
    public class GetFileAttachmentResponse : ResponseBase
    {
        public byte[] AttachedFile { get; set; }
        public string AttachedFileName { get; set; }
        public string AttachedFileType { get; set; }
        public string AttachedFileSize { get; set; }
        public IEnumerable<FileUploadView> Attachments { get; set; } 
    }
}
