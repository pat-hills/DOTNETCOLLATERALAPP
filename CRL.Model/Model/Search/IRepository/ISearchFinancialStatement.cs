using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Messaging;

namespace CRL.Model.Search.IRepository
{
    public interface ISearchFinancialStatementRepository : IWriteRepository<SearchFinancialStatement, int>
    {
        string[] Search(SearchRequest request);
        ViewSearchesResponse SelectSearchesGridViewCQ(ViewSearchesFSRequest request);
    }
}
