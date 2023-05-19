using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.FS
{
    public class ActivitySubordination: FinancialStatementActivity
    {
        public string SubordinationComment { get; set; }
        public bool SubmitterIsSubordinatingParty { get; set; }
        public virtual SubordinatingParty SubordinatingParticipant { get; set; }
        public int SubordinatingParticipantId { get; set; }
       
        public  new ActivitySubordination  Duplicate()
        {
            ActivitySubordination activity = new ActivitySubordination();
            activity.SubordinatingParticipantId = SubordinatingParticipantId;
            activity.SubmitterIsSubordinatingParty = SubmitterIsSubordinatingParty;
            activity.SubordinationComment = SubordinationComment;
            return activity;
        }
    }
}
