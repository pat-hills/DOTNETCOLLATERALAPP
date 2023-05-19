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
    public class CreateFSAttachmentRequest:RequestBase
    {
        public FileUploadView FileUploadView { get; set; }
    }
}
