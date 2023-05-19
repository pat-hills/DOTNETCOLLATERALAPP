using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Configuration;
using CRL.Model.Configuration.IRepository;
using CRL.Respositoy.EF.All.Common.Repository;
using CRL.Model.Payments;

namespace CRL.Repository.EF.All.Repository.Configuration
{
    public class ConfigurationFeeRepository : Repository<TransactionPaymentSetup, int>, IConfigurationFeeRepository
    {
        public ConfigurationFeeRepository(CRL.Infrastructure.UnitOfWork.IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "TransactionPaymentSetup";
        }

        public IQueryable<TransactionPaymentSetup> GetDbSetComplete()
        {
            return ctx.TransactionPaymentSetup.Include("ServiceType").
            Where(s => s.IsActive == true && s.IsDeleted == false); //taken out isApproved

        }

        public IQueryable<TransactionPaymentSetup> GetDbSetComplete(ServiceFees Fee, int LenderType)
        {
            return ctx.TransactionPaymentSetup.Include("ServiceType").
             Where(s => s.IsActive == true && s.IsDeleted == false && s.ServiceTypeId == Fee && s.LenderType == LenderType);//taken out isApproved

        }
    }

    public class ConfigurationTransactionPaymentRespository : Repository<TransactionPaymentSetup, int>, IConfigurationTransactionPaymentRespository
    {
        public ConfigurationTransactionPaymentRespository(CRL.Infrastructure.UnitOfWork.IUnitOfWork uow)
            : base(uow)
        {
        }

        public override string GetEntitySetName()
        {
            return "TransactionPaymentSetup";
        }

        public IQueryable<TransactionPaymentSetup> GetDbSetComplete()
        {
            return ctx.TransactionPaymentSetup.Include("ServiceType").
            Where(s => s.IsActive == true && s.IsDeleted == false);
        }
    }
}
