using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Infrastructure;

namespace CRL.Infrastructure.Domain
{
   
       /// <summary>
    /// Stores a collection of newly created or updated IAuditedEntity entities
    /// </summary>
    public class AuditingTracker
    {
        public AuditingTracker()
        {
            Date = DateTime.Now;
            Created = new HashSet<IAuditedEntity>();
            Updated = new HashSet<IAuditedEntity>();
        }

        public void Refresh()
        {
            Created = new HashSet<IAuditedEntity>();
            Updated = new HashSet<IAuditedEntity>();
        }

        /// <summary>
        /// Stores a collection of newly created entities
        /// </summary>
        public ICollection<IAuditedEntity> Created { get; set; }
        /// <summary>
        /// Stores a collection of newly updated entities
        /// </summary>
        public ICollection<IAuditedEntity> Updated { get; set; }
        public DateTime Date { get; private set; }
    }
}
