using CRL.Infrastructure.UnitOfWork;
using CRL.Model.Configuration;
using CRL.Model.Configuration.IRepository;
using CRL.Respositoy.EF.All.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Repository.EF.All.Repository.Configuration
{
 
        public class LKCurrenciesRepository : Repository<LKCurrency, int>, ILKCurrenciesRepository
        {
            public LKCurrenciesRepository(IUnitOfWork uow)
                : base(uow)
            {
            }
            public override string GetEntitySetName()
            {
                return "LKCurrencies";
            }
        }
    
}
