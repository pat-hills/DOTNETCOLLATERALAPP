
using CRL.Infrastructure.Messaging;
using CRL.Service.Views.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Messaging.EmailAdministration.Response
{
  public  class ViewEmailResponse:ResponseBase
    {
      public ICollection<EmailView> EmailView { get; set; }
      public int NumRecords { get; set; }
    }
}
