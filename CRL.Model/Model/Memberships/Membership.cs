using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.Infrastructure;
using CRL.Model.ModelViews.Enums;
using CRL.Model.WorkflowEngine.IRepository;
using CRL.Model.ModelViews;
using CRL.Infrastructure.Configuration;
using CRL.Infrastructure.Helpers;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Model.Memberships
{
    /// <summary>
    /// Every client registered in the system has a membership account.  Legal clients and their users share the same membership account.  All membership accounts which are involved in a relationship with a developer and owner have the same group no
    /// </summary>
    [Serializable]
    public class Membership : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public Membership()
        {
            //this.RegularMembershipSettings = new HashSet<RegularMembershipSetting>();
          
          
        }
        /// <summary>
        /// Group number assigned to all related memberships.  Used where there would be multiple owners
        /// </summary>
        public int MembershipGroup { get; set; }
        /// <summary>
        /// Code assigned to membership when it was created
        /// </summary>
        public string ClientCode { get; set; }
        /// <summary>
        /// The current credit balance
        /// </summary>
        [ConcurrencyCheck]
        public decimal PrepaidCreditBalance { get; set; } //Owners may never use credits and we will have only one owner
        [ConcurrencyCheck]
        public decimal PostpaidCreditBalance { get; set; }
        //Reltionships
        public MembershipCategory MembershipTypeId { get; set; }
        /// <summary>
        /// States wether this membership is a developer, owner or client
        /// </summary>
        public virtual LKMembershipCategory MembershipType { get; set; }
        /// <summary>
        /// States the financial relationship of this membership account
        /// </summary>
        public MembershipAccountCategory MembershipAccountTypeId { get; set; }
        /// <summary>
        /// States the financial relationship of this membership account
        /// </summary>
        public virtual LKMembershipAccountCategory MembershipAccountType { get; set; }
        /// <summary>
        /// If financial relationship is regular then this is the settings for that relationship
        /// </summary>
        //public virtual ICollection<RegularMembershipSetting> RegularMembershipSettings { get; set; }
        public short MajorRoleIsSecuredPartyOrAgent { get; set; }

        public string AccountNumber { get; set; }
        public int? RepresentativeId { get; set; }
        public virtual Membership Representative { get; set; }
        public short isIndividualOrLegalEntity { get; set; }
        public bool isPayPointClient { get; set; }
        //public bool IsApproved { get; set; }
        public string BankOrFinancialInstitutionCode { get; set; }
        public bool PendingPostpaidAccount { get; set; }

        public void SetupPostpaidAccount(MembershipView membershipView)
        {

            if (membershipView.MembershipAccountTypeId == MembershipAccountCategory.RegularRepresentative)
            {
                
                this.SetRegularRepresentativePostpaidAccount(membershipView.AccountNumber,(int)membershipView.RepresentativeMembershipId);
            }
            else
            {
                this.SetRegularPostpaidAccount(membershipView.AccountNumber);
                
            }

        }
        public bool HasValidPostpaidAccount()
        {
            bool ValidPostpaidAccount = (MembershipAccountTypeId == MembershipAccountCategory.Regular && !String.IsNullOrWhiteSpace(AccountNumber)) ||
                (MembershipAccountTypeId == MembershipAccountCategory.RegularRepresentative 
                && RepresentativeId !=null);

            return ValidPostpaidAccount;           

        }

        public void SetRegularPostpaidAccount(string pAccountNumber)
        {
             MembershipAccountTypeId = MembershipAccountCategory.Regular ;
            AccountNumber =         pAccountNumber.TrimNull();
          

        }
         public void SetRegularRepresentativePostpaidAccount(string pAccountNumber,int RepresentativeClientId)
        {
             MembershipAccountTypeId = MembershipAccountCategory.RegularRepresentative  ;
            RepresentativeId = RepresentativeClientId ;
            AccountNumber = pAccountNumber;

        }

        /// <summary>
        /// For regulare postpaid rememebr to revoke all the childrens or let the children check if their main memebrship is still
        /// </summary>
        public void RevokePostpaidAccount()
        {
            MembershipAccountTypeId = MembershipAccountCategory.NonRegular;
            AccountNumber = null;
            RepresentativeId = null;
            
        }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
    ///// <summary>
    ///// Stands for those recurrent users who have a bank account with their owners
    ///// </summary>
    //public class RegularMembershipSetting : AuditedEntityBaseModel<int>, IAggregateRoot
    //{
    //    /// <summary>
    //    /// Account number with the central bank if there exists a relationship with the central bank
    //    /// </summary>
    //    public string AccountNumber { get; set; }
    //    public int MembershipId { get; set; }
    //    /// <value>Related membership</value>
    //    public virtual Membership Membership { get; set; }
    //    public int? RepresentativeId { get; set; }
    //    /// <summary>
    //    /// If this is a relationship with a repsenative client then the membership of that representative client
    //    /// </summary>
    //    public virtual Membership Represenative { get; set; }
    //    //public virtual ICollection<RegularRepresentativeMembership> RepresentingMembers { get; set; }

    //    protected override void CheckForBrokenRules()
    //    {
    //        //AccountNumber and RepresentativeID cannot be set
    //        throw new NotImplementedException();
    //    }
    //}


    /// <summary>
    /// Represents lookup values for MembershipAccount Types
    /// </summary>
    [Serializable ]
    public class LKMembershipAccountCategory : EntityBase<MembershipAccountCategory>, IAggregateRoot
    {
        [MaxLength(50)]
        public string MembershipAccountCategoryName { get; set; }



        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Represents LookUp value for Membership Types
    /// </summary>
    [Serializable ]
    public class LKMembershipCategory:EntityBase<MembershipCategory>, IAggregateRoot
    {
        [MaxLength(50)]
        public string MembershipCategoryName { get; set; }



        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

   
}
