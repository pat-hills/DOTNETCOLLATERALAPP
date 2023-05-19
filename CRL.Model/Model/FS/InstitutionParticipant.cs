using CRL.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Infrastructure;
using CRL.Model.Common.Enum;
using System.ComponentModel.DataAnnotations;
using CRL.Model.ModelViews;
using CRL.Model.Memberships;

namespace CRL.Model.FS
{
    [Serializable]
    public partial class InstitutionParticipant : Participant,  IAggregateRoot
    {
        public string Name{get;set;}
        public System.Nullable<DebtorCategory> DebtorTypeId { get; set; }

        public int? BusinessRegistrationPrefixId { get; set; }
        public virtual LKRegistrationPrefix BusinessRegistrationPrefix { get; set; }
        public string CompanyNo { get; set; }

        public string BusinessTin { get; set; }
        /// <summary>
        /// Only meant for borrower information
        /// </summary>
        public short MajorityFemaleOrMaleOrBoth { get; set; } //For only debtor
        /// <summary>
        /// Only meant for borrower information
        /// </summary>
        //For only debtor particpant
        public int? SecuringPartyIndustryTypeId { get; set; } //For only secured party
        public virtual LKSecuringPartyIndustryCategory SecuringPartyIndustryType { get; set; }  //For only secured party
        public int? RegistrantInstitutionId { get; set; } //For only secured party
        public virtual Institution RegistrantInstitution { get; set; }  //For only secured party
        public virtual LKDebtorCategory DebtorType { get; set; } //For only debtor particpant
        public string OwnerOfCompany { get; set; }
        public string SearchableName { get; set; }

        public InstitutionParticipant Duplicate()
        {
            InstitutionParticipant _participant = new InstitutionParticipant
            {
                 Name = this.Name ,
                 DebtorTypeId = this.DebtorTypeId ,
                 CompanyNo = this.CompanyNo ,
                 BusinessRegistrationPrefix = this.BusinessRegistrationPrefix,
                 BusinessTin = this.BusinessTin,
                 MajorityFemaleOrMaleOrBoth = this.MajorityFemaleOrMaleOrBoth ,
                 DebtorIsAlreadyClientOfSecuredParty = this.DebtorIsAlreadyClientOfSecuredParty ,
                 SecuringPartyIndustryTypeId = this.SecuringPartyIndustryTypeId ,
                 OwnerOfCompany = this.OwnerOfCompany   ,
                 SearchableName = this.SearchableName 
                  
            };

         


            return _participant;




        }
        
    }

    [Serializable]
    public partial class LKDebtorCategory : EntityBase<DebtorCategory>, IAggregateRoot
    {
        [MaxLength(50)]
        public string CompanyCategoryName { get; set; }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public partial class LKSecuringPartyIndustryCategory : EntityBase<int>, IAggregateRoot
    {
        [MaxLength(50)]
        public string SecuringPartyIndustryCategoryName { get; set; }

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    
}
