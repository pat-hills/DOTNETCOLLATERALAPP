using CRL.Infrastructure.UnitOfWork;
using CRL.Model.Memberships;
using CRL.Model.Memberships.IRepository;
using CRL.Model.ModelViews;

using CRL.Respositoy.EF.All.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Repository.EF.All.Repository.Memberships
{
    public class BankVerificationCodeRepository : Repository<BankVerificationCode, int>, IBankVerificationCodeRepository
    {
        public BankVerificationCodeRepository(IUnitOfWork uow)
            : base(uow)
        {


        }

        public override string GetEntitySetName()
        {
            return "BankVerificationCode";
        }

        public BankVerificationCode GetBankVerificationByCode(string code)
        {
            return ctx.BankVerificationCodes.Where(c => c.Code == code).SingleOrDefault();
        }

        public IQueryable<BankVerificationCode> GetDbSetComplete()
        {
            return ctx.BankVerificationCodes;
        }

        public void TruncateAllData()
        {
            var newctx = ctx.BankVerificationCodes;
            //var ctxList = ctx.ToList();
            newctx.RemoveRange(newctx);
        }
    }
}
