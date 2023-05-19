using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;

namespace CRL.Model.WorkflowEngine.IRepository
{


    public interface IWFWorkflowRepository : IWriteRepository<WFWorkflow, int>
    {
       // IQueryable<Model.WorkflowEngine .WFWorkflow > GetDbSetWithCategoryNames();
        WFWorkflow GetWFWorkflowById(int id);
    }
}
