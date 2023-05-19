using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using CRL.Infrastructure.Domain;

namespace CRL.Model.Infrastructure
{
    public interface IAuditedEntity
    {
        int CreatedBy { get; set; }
        Nullable<int> UpdatedBy { get; set; }
        System.DateTime CreatedOn { get; set; }
        Nullable<System.DateTime> UpdatedOn { get; set; }
        RecordState State { get; set; }


    }

    [Serializable]
    public abstract class AuditedEntityBase<T> : EntityBase<T>, IAuditedEntity
    {
        public int CreatedBy { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        [NotMapped]
        public RecordState State { get; set; }
    }

    [Serializable]
    public abstract class EntityBase<T>
    {
        private T _id;
        private IList<string> _brokenRules = new List<string>();
        private bool _idHasBeenSet = false;

        public EntityBase()
        {
            IsActive = true;
            IsDeleted = false;
        }

        public EntityBase(T id)
        {
            this.Id = id;
        }
        [Column]
        [Key]
        public T Id
        {
            get { return _id; }
            set
            {
                if (_idHasBeenSet)
                    ThrowExceptionIfOverwritingAnId();
                _id = value;
                _idHasBeenSet = true;
            }
        }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        private void ThrowExceptionIfOverwritingAnId()
        {
            // throw new ApplicationException("You cannot change the id of an entity.");
        }
        public bool IsValid()
        {
            ClearCollectionOfBrokenRules();
            CheckForBrokenRules();
            return _brokenRules.Count() == 0;
        }
        protected abstract void CheckForBrokenRules();

        private void ClearCollectionOfBrokenRules()
        {
            _brokenRules.Clear();
        }
        public IEnumerable<string> GetBrokenBusinessRules()
        {
            return _brokenRules;
        }
        protected void AddBrokenRule(string brokenRule)
        {
            _brokenRules.Add(brokenRule);
        }

        public object ShallowClone()
        {
            //grab the type and create a new instance of that type
            object opSource = this;
            Type opSourceType = opSource.GetType();
            object opTarget = Activator.CreateInstance(opSourceType);

            //grab the properties
            PropertyInfo[] opPropertyInfo = opSourceType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            //iterate over the properties and if it has a 'set' method assign it from the source TO the target
            foreach (PropertyInfo item in opPropertyInfo)
            {
                if (item.CanWrite)
                {
                    //value types can simply be 'set'
                    if (item.PropertyType.IsValueType || item.PropertyType.IsEnum || item.PropertyType.Equals(typeof(System.String)))
                    {
                        item.SetValue(opTarget, item.GetValue(opSource, null), null);
                    }
                }
            }
            //return the new item
            return opTarget;
        }
    }

}
