using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Messaging;
using CRL.Service.Messaging.FinancialStatements.Response;
using CRL.Service.Messaging.Workflow.Request;
using CRL.Model.Messaging;
using CRL.Infrastructure.Messaging;
using CRL.Service.Messaging.Workflow.Response;

namespace CRL.Service.Interfaces
{
    public interface IWidgetService
    {
        ViewMyTasksResponse ViewMy10LatestTasks(RequestBase request);
        ViewFSResponse ViewMy10AboutToExpireFinancingStatement(RequestBase request);
        ViewAuditsResponse ViewMy10Audits(RequestBase request);
        ViewMyMessagesResponse ViewMy10Messages(RequestBase request);
        ViewStatResponse ViewNoOfFinancingStatement(RequestBase request);
    }
}
