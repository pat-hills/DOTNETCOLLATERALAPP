
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
        public class LKServiceFeeCategoriesRepository : Repository<LKServiceFeeCategory, int>, ILKServiceFeeCategoriesRepository
        {
            public LKServiceFeeCategoriesRepository(IUnitOfWork uow)
                : base(uow)
            {
            }
            public override string GetEntitySetName()
            {
                return "LKServiceFeeCategories";
            }
        }
    
}
