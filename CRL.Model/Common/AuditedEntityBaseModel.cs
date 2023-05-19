using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Infrastructure;
using CRL.Model.ModelViews;
using CRL.Infrastructure.Domain;
using CRL.Model.Memberships;

namespace CRL.Model.Common
{
    [Serializable]
    public abstract class AuditedEntityBaseModel<T>: AuditedEntityBase<T>
    {
        [ForeignKey("CreatedBy")]
        public virtual User CreatedByUser { get; set; }
        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedByUser { get; set; }

        public void Audit(AuditingTracker tracker)
        {
            if (this.State == RecordState.New)
                tracker.Created.Add(this);
            else if (this.State == RecordState.Modified || this.State == RecordState.Removed)
                tracker.Updated.Add(this);
        }
       
    }
}
