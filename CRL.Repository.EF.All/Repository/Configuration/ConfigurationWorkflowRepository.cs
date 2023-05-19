using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Configuration;
using CRL.Model.Configuration.IRepository;
using CRL.Respositoy.EF.All.Common.Repository;

namespace CRL.Repository.EF.All.Repository.Configuration
{
  
    public class ConfigurationWorkflowRepository: Repository<ConfigurationWorkflow, int>, IConfigurationWorkflowRepository
    {
        public ConfigurationWorkflowRepository(CRL.Infrastructure.UnitOfWork.IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "ConfigurationWorkflow";
        }
    }
}
