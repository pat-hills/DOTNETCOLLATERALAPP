using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;

namespace CRL.Model.Configuration.IRepository
{
   

    public interface IConfigurationWorkflowRepository : IWriteRepository<ConfigurationWorkflow, int>
    {

    }
}
