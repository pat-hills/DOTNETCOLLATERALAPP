using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Service.Common;
using CRL.Infrastructure.Messaging;
using CRL.Service.Views;
using CRL.Service.Views.FinancialStatement;
using CRL.Model.ModelViews;

using CRL.Service.Views.Workflow;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Messaging.Workflow.Response
{
    public class GetTaskHandleResponse : ResponseBase
    {
        public TaskHandleView TaskHandleView { get; set; }
        public FSActivityDetailSummaryView FSActivityDetailSummaryView { get; set; }
        public ICollection<LookUpView> SectorsOfOperation { get; set; }

        public FSView BeforeUpdateFSView { get; set; }
        public MembershipView MembershipView { get; set; }
        public InstitutionView InstitutionView { get; set; }
        public UserView UserView { get; set; }
        public FSView FSView { get; set; }
        public bool isPayable { get; set; }
        public decimal Amount { get; set; }
        public bool isPostpaid { get; set; }
        public decimal CurrentCredit { get; set; }
        public ICollection<UserGridView> UserGridView { get; set; }
    }
}
