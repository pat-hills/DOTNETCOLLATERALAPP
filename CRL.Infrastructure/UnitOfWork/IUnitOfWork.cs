using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CRL.Infrastructure.Domain;

namespace CRL.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        void RegisterAmended(IAggregateRoot entity,
                             IUnitOfWorkRepository unitofWorkRepository);
        void RegisterNew(IAggregateRoot entity,
                         IUnitOfWorkRepository unitofWorkRepository);
        void RegisterRemoved(IAggregateRoot entity,
                             IUnitOfWorkRepository unitofWorkRepository);
           void AuditEntities (int UserId, DateTime auditedDate, AuditingTracker auditedtracker);
        void Commit();
    }
}

