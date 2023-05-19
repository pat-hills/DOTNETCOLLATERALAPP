using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Service.Views.Payments
{
    public class InterSwitchTransactionView : InterSwitchUserView
    {
        public int product_id { get; set; }
        public string site_redirect_url { get; set; }
        public string txn_ref { get; set; }
        public string hash { get; set; }
        public string pay_item_id { get; set; }
        public string cust_id { get; set; }
        public string pay_item_name { get; set; }
    }
}
