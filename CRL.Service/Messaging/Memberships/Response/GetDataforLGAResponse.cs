using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Service.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Messaging.Memberships.Response
{
  public  class GetDataforLGAResponse:ResponseBase
    {
     public ICollection<LKLGAView> LGAs { get; set; }
    }
}
