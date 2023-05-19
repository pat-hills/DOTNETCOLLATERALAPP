using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.WorkflowEngine.Enums
{
    public enum WorkflowRequestType
    {
        FinancialStatement=1, FinancialStatementActivity=2, Membership=3,MembershipRegistration=4,
        UpdateFinancingStatement=5, SubordinateFinancingStatement=6, AssignFinancingStatement=7,CancelFinancingStatement=8,
        PaypointUserAssigment=9
    }
}
