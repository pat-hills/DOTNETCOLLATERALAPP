using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews.Enums;

namespace CRL.Model.ModelViews.Memberships
{
    /// <summary>
    /// Every Institution or Individual registered in the system has a membership information
    /// Membership information also has an inbuilt CreditUnitAccount which can be used for internal op
    /// </summary>
    public class MembershipView 
    {
        
        public int Id { get; set; }
        public int MembershipGroup { get; set; }  //Determines membership groups, necessary if system support multiple owners
        [Display(Name = "Secured Creditor Code")]
        public string CreditCode { get; set; }    //Creditcode for credit transactions
     

        //Reltionships
      
        public MembershipCategory MembershipTypeId { get; set; } //Is this a developer membership, institution membership or individual membership


        public MembershipAccountCategory MembershipAccountTypeId { get; set;}

        
        [Display(Name = "Payment Integration")]
        public int? _MembershipAccountTypeId { get; set; }
      
      

        [Display(Name = "Payment Integration")]
        public string MembershipAccountTypeName{ get; set; }      

        public string MembershipType { get; set; }
        public bool RepresentativeMembership { get; set; }
          [Display(Name = "Representative Bank")]
        public int? RepresentativeMembershipId { get; set; }

          [Display(Name = "Representative Bank")]
          public string RepresentativeMembershipName { get; set; }

         [Display(Name = "Bank Account No")]
        public string AccountNumber { get; set; } //If regularregualr set this
         [Display(Name = "Major Role")]
          public short MajorRoleIsSecuredPartyOrAgent { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,###.00}")]
         [Display(Name = "Prepaid Account Bal")]
         public decimal PrepaidCreditBalance { get; set; } //Owners may never use credits and we will have only one owner
        [DisplayFormat(DataFormatString = "{0:#,##0.00}")]
        [Display(Name = "Postpaid Account Bal")]
         public decimal PostpaidCreditBalance { get; set; }
         [Display(Name = "Set client as Pay point")]
        public bool isPayPointClient { get; set; }
       
    }
}
