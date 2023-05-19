using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Administration;

namespace CRL.Model.Messaging
{
    public class ViewAuditDetailsResponse : ResponseBase
    {
        public AuditView AuditView { get; set; }
    }
}
