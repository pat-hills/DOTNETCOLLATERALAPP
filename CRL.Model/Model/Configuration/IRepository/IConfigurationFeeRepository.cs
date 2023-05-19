using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Payments;

namespace CRL.Model.Configuration.IRepository
{
    public interface IConfigurationFeeRepository : IWriteRepository<TransactionPaymentSetup, int>
    {

        IQueryable<TransactionPaymentSetup> GetDbSetComplete();
        IQueryable<TransactionPaymentSetup> GetDbSetComplete(ServiceFees Fee, int LenderType);
    }

    public interface IConfigurationTransactionPaymentRespository : IWriteRepository<TransactionPaymentSetup,int>
    {
        IQueryable<TransactionPaymentSetup> GetDbSetComplete();
    }
}
