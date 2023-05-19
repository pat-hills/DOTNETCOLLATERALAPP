using CRL.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using System.ComponentModel.DataAnnotations;

namespace CRL.Model.Memberships
{
    /// <summary>
    /// A unit group; within a Financial Institution
    /// </summary>
    [Serializable]
    public class InstitutionUnit : AuditedEntityBaseModel<int>,  IAggregateRoot
    {
        public InstitutionUnit()
        {           
            this.People = new HashSet<Person>();       
        }
        
        public string Name{get;set;}
        public string Description { get; set; }
        public string Email { get; set; }
      

        //Relationships
        public int InstitutionId { get; set; }
        public virtual Institution Institution { get; set; }
        //public virtual ICollection<EnterpriseUnit> SubUnits { get; set; }
        //public virtual EnterpriseUnit ParentEnterpriseUnit { get; set; }
        //public virtual EnterpriseUnitLevel EnterpriseUnitLevel { get; set; }
        /// <summary>
        /// People in this unit group
        /// </summary>
        public virtual ICollection<Person> People { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
