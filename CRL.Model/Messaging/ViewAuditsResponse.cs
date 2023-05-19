using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Administration;

namespace CRL.Model.Messaging
{


    public class ViewAuditsResponse : ResponseBase
    {
        public ICollection<AuditView> AuditViews { get; set; }
        public int NumRecords { get; set; }


    }
}
