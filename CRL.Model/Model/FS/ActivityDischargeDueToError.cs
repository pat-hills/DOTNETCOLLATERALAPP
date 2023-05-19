using CRL.Model.FS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.Model.FS
{
    [Serializable]
    public partial class ActivityDischargeDueToError : FinancialStatementActivity
    {
        public bool PerformedByRegistrar { get; set; }
    }
}
