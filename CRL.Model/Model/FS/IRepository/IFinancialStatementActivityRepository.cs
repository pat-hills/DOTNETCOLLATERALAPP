using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Messaging;

namespace CRL.Model.FS.IRepository
{
    public interface IFinancialStatementActivityRepository: IWriteRepository<FinancialStatementActivity, int>
    {

        IQueryable<FinancialStatementActivity> GetDbSetComplete();
        ViewFSActivityResponse SelectFSActivityGridViewCQ(ViewFSActivityRequest request);
    }
}
