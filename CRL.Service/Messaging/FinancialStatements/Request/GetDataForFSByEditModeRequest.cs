using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Enums;

using CRL.Service.Messaging.Common.Request;
using CRL.Service.Views;
using CRL.Model.FS.Enums;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.Service.Messaging.FinancialStatements.Request
{

    public class GetFSViewRequest : RequestBase
    {
        public bool IsRequestFromSearchResult { get; set; }
    }
    public class GetFSEditRequest : RequestBase
    {
        public bool IsRequestFromSearchResult { get; set; }
        public int CaseId { get; set; }
        public bool WorkflowOnForRebuild { get; set; }
        public FSView FSView { get; set; }

    }
    public class GetFSCreateRequest : RequestBase
    {
        public int? Id { get; set; }
        public bool WorkflowOnForRebuild { get; set; }
        public FinancialStatementTransactionCategory? CreateTransactionCategory { get; set; }
        public bool? CreateAsRegistrant { get; set; }
        public FSView FSView { get; set; }
        //public bool CreateOrEditModeIsUpdate { get; set; }
        public bool CreateLien { get; set; }
        //public bool ExternalFSViewIsLien { get; set; }




    }
}
