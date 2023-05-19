using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model.Infrastructure;
using CRL.Respositoy.EF.All;

namespace CRL.Repository.EF.All
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private CBLContext CBLContext { get; set; }

        public CBLContext GetDataContext
        {
            get
            {
                if (this.CBLContext == null)
                {
                    CBLContext = new CBLContext();
                    return CBLContext;

                }
                else
                    return CBLContext;
            }
            set
            {
                CBLContext = new CBLContext();
            }
        }


        public void Commit()
        {
            this.GetDataContext.SaveChanges();
        }
        public void AuditEntities(int UserId, DateTime auditedDate, AuditingTracker auditedtracker)
        {

            foreach (var c in auditedtracker.Created)
            {
                c.CreatedBy = UserId;
                c.CreatedOn = auditedDate;

            }
            foreach (var c in auditedtracker.Updated)
            {
                c.UpdatedBy = UserId;
                c.UpdatedOn = auditedDate;

            }
        }



        public void RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository unitofWorkRepository)
        {
            unitofWorkRepository.PersistUpdateOf(entity);
        }

        public void RegisterNew(IAggregateRoot entity, IUnitOfWorkRepository unitofWorkRepository)
        {
            unitofWorkRepository.PersistCreationOf(entity);
        }

        public void RegisterRemoved(IAggregateRoot entity, IUnitOfWorkRepository unitofWorkRepository)
        {
            unitofWorkRepository.PersistDeletionOf(entity);
        }


        public void Dispose()
        {
            if (CBLContext != null)
            {
                CBLContext.Dispose();
                CBLContext = null;
            }
        }
    }
}
