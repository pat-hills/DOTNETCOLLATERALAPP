using CRL.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.Memberships.IRepository
{
    public interface IBankVerificationCodeRepository : IWriteRepository<BankVerificationCode, int>
    {
        BankVerificationCode GetBankVerificationByCode(string code);
        IQueryable<BankVerificationCode> GetDbSetComplete();
        void TruncateAllData();
    }
}
