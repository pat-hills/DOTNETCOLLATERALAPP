using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using CRL.Service.Views;
using CRL.Service.Views.FinancialStatement;
using CRL.Service.Views.Workflow;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.Service.Messaging.FinancialStatements.Response
{
    public class ViewBatchesResponse:ResponseBase
    {
        public ICollection<FSBatchView> FSBatches { get; set; }
        public int NumRecords { get; set; }
    }

    public class ViewBatchResponse : ResponseBase
    {
        public FSBatchView FSBatch { get; set; }
        public bool CreateModeIsWorkflowOn { get; set; }
        public AssignNewTaskView AssignNewTaskView { get; set; }
        public bool CanEdit { get; set; }
    }

    public class ViewBatchedFSListResponse : ResponseBase
    {
        public List<FSGridView> FSGridView { get; set; }
        public int NumRecords { get; set; }
    }
}
