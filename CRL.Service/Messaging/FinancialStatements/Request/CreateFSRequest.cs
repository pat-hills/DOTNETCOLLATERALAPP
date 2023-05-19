using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.FS.Enums;

using CRL.Service.Messaging.Workflow.Request;
using CRL.Service.Views;
using CRL.Model.Messaging;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Messaging.FinancialStatements.Request
{
    //public class CreateFSRequest:RequestBase
    //{
    //    public FSView FinancialStatementView { get; set; }

    //}

    public class CreateNewFSRequest : HandleWorkItemRequest
    {
        public FSView FSView { get; set; }
        public RequestMode NewFSRequestMode { get; set; }
      
        

    }


}
