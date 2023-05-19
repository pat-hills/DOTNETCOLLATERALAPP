using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.Payments
{
    public class DirectPayResponseView
    {
        public string StatusId { get; set; }
        public string StatusMessage { get; set; }
        public string TxRefId { get; set; }
        public string ValueToken { get; set; }
        public string DealerNo { get; set; }
        public string DestAccount { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public decimal Amount { get; set; }
    }

    public class DirectPayReversalResponseView : DirectPayResponseView
    {

    }
}
