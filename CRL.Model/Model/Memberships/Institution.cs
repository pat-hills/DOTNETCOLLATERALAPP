using CRL.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Infrastructure;
using CRL.Model.ModelViews.Enums;
using CRL.Infrastructure.Domain;
using CRL.Model.FS;
using CRL.Model.Common.Enum;

namespace CRL.Model.Memberships
{
    /// <summary>
    /// Represents a Financial Institution client
    /// </summary>
    [Serializable]
    public class Institution : AuditedEntityBaseModel<int>,  IAggregateRoot
    {
        public Institution()
        {
            this.Address = new AddressInfo();
            this.InstitutionUnits = new HashSet<InstitutionUnit>();
            this.People = new HashSet<Person>();         
        }
        public string Name{get;set;}
        public AddressInfo Address { get; set; }
        public string CompanyNo { get; set; }      
        //Relationships
        public int? SecuringPartyTypeId { get; set; }
        public virtual LKSecuringPartyIndustryCategory  SecuringPartyType { get; set; }
        public virtual LKNationality Nationality { get; set; }
        public virtual LKCountry Country { get; set; }
        public int? NationalityId { get; set; }
        public int? CountryId { get; set; }
        public virtual LKCounty County { get; set; }
        public int? CountyId { get; set; }
        public int? LGAId { get; set; }
        public virtual LKLGA LGA { get; set; }
        public int? MembershipId { get; set; }
        public virtual Membership Membership { get; set; }       
        public virtual ICollection<InstitutionUnit> InstitutionUnits { get; set; }
        public int? AuthorizedByUserId { get; set; }
        public User AuthorizedByUser { get; set; }
        public Nullable<System.DateTime> AuthorizedDate { get; set; }
        /// <summary>
        /// All people in this Financial Institution Client
        /// </summary>
        public virtual ICollection<Person> People { get; set; }
        
        

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

  



    
}
