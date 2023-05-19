using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;

namespace CRL.Model.WorkflowEngine.IRepository
{
   
    public interface IWFCaseRepository : IWriteRepository<WFCase, int>
    {
        // IQueryable<Model.WorkflowEngine .WFWorkflow > GetDbSetWithCategoryNames();
        WFCase GetWFCaseById(int id);
        int GetNumTasksByUserId(int id);
        M GetSpecializedWFCaseById<M>(int id) where M : WFCase;
    }

    public interface IWFTokenRepository : IWriteRepository<WFToken, int>
    {
        IQueryable<WFToken> GetDbSetComplete();
    }

    
}
