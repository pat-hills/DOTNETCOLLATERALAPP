using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Model.ModelViews.FinancingStatement;
using CRL.Infrastructure.Messaging;
using CRL.Model.Messaging;

namespace CRL.Model.Messaging
{
    public class CreateEditFSRequest:RequestBase
    {
        public FSView FSView { get; set; }
    }

    public class SubmitFSRequest : RequestBase
    {
        public FSView FSView { get; set; }
        public string WFComment { get; set; }  //Reprresents the comment the assigned user will see in their email and also when viewing in the taskhandler page
        public int[] AssignedUsers { get; set; } //Represent the users 
        public string[] NotifiedEmails { get; set; }
    }

    public class CreateFSRequest : RequestBase
    {
        public FSView FinancialStatementView { get; set; }

    }

  

    public class NewFSRequest : FSRequest
    {
      
 public bool DebtorConsentAgreed { get; set; }


    }

}
