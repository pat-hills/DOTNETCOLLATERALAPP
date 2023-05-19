using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Messaging;
using CRL.Model.Search;

namespace CRL.Model.Model.Search.IRepository
{
    public interface ITrackSearchResultRepository : IWriteRepository<SearchResultTracker, int>
    {
        IQueryable<SearchResultTracker> GetSearchResultTrackers(int statementId);
    }
}
