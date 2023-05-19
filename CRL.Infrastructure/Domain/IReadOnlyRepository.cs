using System;
using System.Collections.Generic;
using System.Linq;


namespace CRL.Infrastructure.Domain
{
    public interface IReadOnlyRepository<T, TId> where T : IAggregateRoot
    {
        T FindBy(TId id);
        IEnumerable<T> FindAll();
        IEnumerable<T> FindBy(IQueryable<T> query);
        IEnumerable<T> FindBy(IQueryable<T> query, int index, int count);
        IQueryable<T> GetDbSet();
     
        IQueryable<T> GetNoTrackingDbSet();

    }
}
