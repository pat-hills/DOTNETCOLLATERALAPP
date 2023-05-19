using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
    public class CreateFSAttachmentResponse:ResponseBase
    {
        public int Id { get; set; }
        public string filename { get; set; }
        public string filetype { get; set; }
        public string filesize { get; set; }
    }
}
