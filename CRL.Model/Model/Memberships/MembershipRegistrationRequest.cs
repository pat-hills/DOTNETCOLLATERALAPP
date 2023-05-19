using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.Common.Enum;
using CRL.Model.FS;
using CRL.Model.ModelViews.Enums;

namespace CRL.Model.Memberships
{
    /// <summary>
    /// Represents request submitted by prospective clients
    /// </summary>
    public class MembershipRegistrationRequest:AuditedEntityBaseModel<int>,IAggregateRoot
       
       
    {
        public MembershipRegistrationRequest()
        {
            AdminAddress = new AddressInfo();
        }
        /// <summary>
        /// Indicates account number if central bank
        /// </summary>
        public string AccountNumberWithCentralBank { get; set; }
      

        /// <summary>
        /// If this is a relationship with a rep[resenative then the Id of the Financial Institution with which this relationship is formed with
        /// </summary>
        public int? RepresentativeInstitutionClientId { get; set; }

       
        /// <summary>
        /// Membership account type that must be stated before approval of this request
        /// </summary>
        public virtual LKMembershipAccountCategory MembershipAccountType { get; set; }

        /// <summary>
        /// Membership account type that must be stated before approval of this request
        /// </summary>
        public MembershipAccountCategory MembershipAccountTypeId { get; set; }

        //Name of the user to setup an account for
        public string RequestNo { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string Gender { get; set; }
        public AddressInfo AdminAddress { get; set; }
        public virtual LKNationality Nationality { get; set; }
        public virtual LKCountry Country { get; set; }
        public int? NationalityId { get; set; }
        public int? CountryId { get; set; }
        public virtual LKCounty County { get; set; }
        public int? CountyId { get; set; }
        /// <summary>
        /// Login name for Administrator to be created
        /// </summary>
        public string AccountName { get; set; }
        public string Password { get; set; }


        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Represents a Financial Institution membershiprequest
    /// </summary>
    public class LegalEntityMembershipRegistrationRequest : MembershipRegistrationRequest
    {
        public LegalEntityMembershipRegistrationRequest():base()
        {
            LegalEntityAddress = new AddressInfo();
        }
        //Name of the user to setup an account for
        public string Name { get; set; }
        public AddressInfo LegalEntityAddress { get; set; }
        public string CompanyNo { get; set; }
        public virtual LKNationality LegalNationality { get; set; }
        public virtual LKCountry LegalCountry { get; set; }
        public virtual LKCounty LegalCounty { get; set; }
        public int? SecuringPartyTypeId { get; set; }
        public virtual LKSecuringPartyIndustryCategory SecuringPartyType { get; set; }
        public int? LegalNationalityId { get; set; }
        public int? LegalCountryId { get; set; }
        public int? LegalCountyId { get; set; }
        public int? InstitutionId { get; set; }
        public virtual Institution Institution { get; set; }
        
        
        

    }
}
