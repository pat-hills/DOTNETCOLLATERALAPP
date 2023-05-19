using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Model.Search.IRepository;
using CRL.Model.Search;
using CRL.Respositoy.EF.All.Common.Repository;
using CRL.Infrastructure.UnitOfWork;

namespace CRL.Repository.EF.All.Repository.Search
{
    public class TrackSearchResultRepository : Repository<SearchResultTracker, int>, ITrackSearchResultRepository
    {
        public TrackSearchResultRepository(IUnitOfWork uow) : base(uow) 
        {

        }

        public override string GetEntitySetName()
        {
            return "SearchResultTracker";
        }

        public IQueryable<SearchResultTracker> GetSearchResultTrackers(int statementId) 
        {
            return ctx.SearchResultTracker.Include("FIleUpload").Where(s => s.SearchFinancialStatementId == statementId && s.IsActive == true && s.IsDeleted == false);
        }
    }
}
