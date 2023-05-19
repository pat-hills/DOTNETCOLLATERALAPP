using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.FS
{
    [Serializable]
    public partial class ActivityRenewal : FinancialStatementActivity
    {
        public DateTime  BeforeExpiryDate { get; set; }
        public DateTime AfterExpiryDate { get; set; }

        public new ActivityRenewal Duplicate()
        {
            ActivityRenewal activity = new ActivityRenewal();
            activity.BeforeExpiryDate = this.BeforeExpiryDate;
            activity.AfterExpiryDate = this.AfterExpiryDate;
            return activity;
        }

    }
}
