using CRL.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;

namespace CRL.Model.Memberships
{
    /// <summary>
    /// Peoples in the system.  People may or may not belong to a Financial Institution.   People may also have or not have a membership account in the system.  This means they cannot login into the system
    /// </summary>
    [Serializable]
    public class Person : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public Person()
        {
            Address = new AddressInfo();
        }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }        
        public string Gender { get; set; }       
        public AddressInfo Address { get; set; }
        public virtual LKNationality Nationality { get; set; }
        public virtual LKCountry Country { get; set; }
        public virtual LKCounty County { get; set; }
        public int? NationalityId { get; set; }
        public int? CountryId { get; set; }
        public int? CountyId { get; set; }
        public int? LGAId { get; set; }
        //Relationships
        public Nullable<int> InstitutionUnitId { get; set; }
        public Nullable<int> InstitutionId { get; set; }
        public virtual InstitutionUnit InstitutionUnit { get; set; }
        public virtual Institution Institution { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
