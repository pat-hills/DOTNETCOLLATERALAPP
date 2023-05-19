using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.Payments
{
    public class DirectPayRequestView
    {
        public string DestAccount { get; set; }
        public decimal Amount { get; set; }
        public string Msg { get; set; }
        public string SequenceNo { get; set; }
        public string DealerNo { get; set; }
        public string Passsword { get; set; }
        public string ProductCode { get; set; }
        public string Pin { get; set; }
        public string RequestTimestamp { get; set; }
        public string ReceiptNo { get; set; }
    }

    public class DirectPayReversalRequestView : DirectPayRequestView
    {
        
    }
}
