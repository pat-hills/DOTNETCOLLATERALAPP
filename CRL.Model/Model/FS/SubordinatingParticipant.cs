using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.Common.Enum;
using CRL.Model.ModelViews;
using CRL.Model.Memberships;

namespace CRL.Model.FS
{
   
    public class SubordinatingParty :AuditedEntityBaseModel<int>,IAggregateRoot
    {
        public SubordinatingParty()
        {
            Address = new AddressInfo();
        }
        
        public AddressInfo Address { get; set; }
        public virtual LKNationality Nationality { get; set; }
        public int? NationalityId { get; set; }
        public virtual LKCountry Country { get; set; }        
        public int? CountryId { get; set; }
        public virtual LKCounty County { get; set; }
        public int? CountyId { get; set; }
        public int? LGAId { get; set; }
        public virtual LKLGA LGA { get; set; }
        public bool isBeneficiary { get; set; }

        public int? RelatedMembershipId { get; set; }
        public virtual Membership RelatedMembership { get; set; }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    public class IndividualSubordinatingParty:SubordinatingParty 
    {
        public IndividualSubordinatingParty()
        {
            Identification = new PersonIdentificationInfo();
        }
        public string Title { get; set; }
        public PersonIdentificationInfo Identification { get; set; }
        public string Gender { get; set; }
        public System.Nullable<DateTime> DOB { get; set; }
        public string OtherDocumentDescription { get; set; }
        public int PersonIdentificationTypeId { get; set; }
        public int? RelatedIndividualUserId { get; set; }
        public virtual User RelatedIndividualUser { get; set; }
    }

    public class InstitutionSubordinatingParty:SubordinatingParty 
    {
        public string Name { get; set; }
        public string CompanyNo { get; set; }
        public virtual LKSecuringPartyIndustryCategory SecuringPartyIndustryType { get; set; }  
        public int? SecuringPartyIndustryTypeId { get; set; }
        public int? RelatedInstitutionId { get; set; }
        public virtual Institution RelatedInstitution { get; set; }
        
        
    }
}
