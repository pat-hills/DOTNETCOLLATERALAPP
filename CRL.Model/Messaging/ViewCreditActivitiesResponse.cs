using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Payments;
using CRL.Model.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.Messaging
{


    public class ViewCreditActivitiesResponse:ResponseBase
    {
        public ICollection<CreditActivityView> CreditActivityView { get; set; }
        public int NumRecords { get; set; }


    }
}
