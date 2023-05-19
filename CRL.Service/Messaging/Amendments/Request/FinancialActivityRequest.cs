using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Enums;
using CRL.Service.Views;
using CRL.Model.FS.Enums;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.Service.Messaging.FinancialStatements.Request
{

    public class GetDataForAmendRequest : RequestBase
    {
        public int Id { get; set; }
        public FinancialStatementActivityCategory selectedAmendmentType { get; set; }
        public bool SubordinationToIndividual { get; set; }
        public bool CreateModeIsWorkflowOn { get; set; }
        public int? OptionalClientType { get; set; }
        public int? MembershipId { get; set; }
        

        public EditMode EditMode { get; set; }

    }

    public class GetCreateUpdateRequest : RequestBase
    {
        public bool WorkflowOnForRebuild { get; set; }
        public EditMode EditMode { get; set; }
        public bool IgnoreReloadingOfFSView { get; set; }
        public int CaseId { get; set; }
        public FSView FSView { get; set; }
    }

    public class GetFSUpdateRequest : RequestBase
    {
        public EditMode EditMode { get; set; }
        public int FinancialStatementId { get; set; }
        public int EditUpdateActivityId { get; set; }
        public FSView FSView { get; set; }
        public bool IgnoreReloadingOfFSView { get; set; }
        public int CaseId { get; set; }

    }
   

   

}
