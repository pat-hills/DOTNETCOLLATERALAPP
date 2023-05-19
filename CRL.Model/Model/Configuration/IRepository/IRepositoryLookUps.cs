using CRL.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.Configuration.IRepository
{
    public interface ILKServiceFeeCategoriesRepository : IWriteRepository<LKServiceFeeCategory, int>
    {

    }

    public interface ILKCurrenciesRepository : IWriteRepository<LKCurrency, int>
    {

    }
}
