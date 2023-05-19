using CRL.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Repository.EF.All;

namespace CRL.Respositoy.EF.All.Common.Repository
{
    public abstract class Repository<T, EntityKey> : IUnitOfWorkRepository where T : class , IAggregateRoot
    {


        private IUnitOfWork _uow;
        public CBLContext ctx
        {
            get
            {
                return ((EFUnitOfWork)_uow).GetDataContext;
            }

        }

        public Repository(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public void Add(T entity)
        {

            _uow.RegisterNew(entity, this);
        }

        public void Remove(T entity)
        {

            _uow.RegisterRemoved(entity, this);

        }

        public void Save(T entity)
        {
            // Do nothing as EF tracks changes
            this.Add(entity);
            Commit();
        }

        public void Commit()
        {
            // Do nothing as EF tracks changes
            ctx.SaveChanges();
        }

        public IQueryable<T> GetDbSet()
        {

            return ctx.Set<T>();


        }

        public IQueryable<T> GetNoTrackingDbSet()
        {

            return ctx.Set<T>().AsNoTracking();


        }

        public abstract string GetEntitySetName();


        public T FindBy(EntityKey Id)
        {

            return ctx.Set<T>().Find(Id);

        }


        public IEnumerable<T> FindAll()
        {
            return GetDbSet().ToList<T>();
        }

        public IEnumerable<T> FindAll(int index, int count)
        {
            return GetDbSet().Skip(index).Take(count).ToList<T>();
        }

        public IEnumerable<T> FindBy(IQueryable<T> query)
        {

            return query.ToList<T>();
        }

        public IEnumerable<T> FindBy(IQueryable<T> query, int index, int count)
        {


            return query.Skip(index).Take(count).ToList<T>();
        }
     

        public void PersistCreationOf(IAggregateRoot entity)
        {
            //DataContextFactory.GetDataContext().Set<T>().Add(entity);
            ctx.Set<T>().Add((T)entity);

        }

        public void PersistUpdateOf(IAggregateRoot entity)
        {
            //For non-sessional contextx we need to add this and set it's mode to update
        }

        public void PersistDeletionOf(IAggregateRoot entity)
        {
            ctx.Set<T>().Remove((T)entity);
        }
    }
}
