using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;
using CRL.Model.Memberships;

namespace CRL.Model.ModelViewMappers
{
    public static class MembershipMapper
    {
        public static MembershipView ConvertToMembershipView(this Membership Membership, string RepresentativeMembershipName=null)
        {
           
            MembershipView mv = new MembershipView()
            {
                Id = Membership.Id,
                MembershipGroup = Membership.MembershipGroup,
                MembershipType = Membership.MembershipType.MembershipCategoryName,
                MembershipTypeId = Membership.MembershipTypeId,
                MembershipAccountTypeId = Membership.MembershipAccountTypeId ,
                 _MembershipAccountTypeId= Convert.ToInt32(Membership .MembershipAccountTypeId ),
                 MembershipAccountTypeName  = Membership.MembershipAccountType .MembershipAccountCategoryName ,
                CreditCode = Membership.ClientCode,
                 PrepaidCreditBalance  = Membership.PrepaidCreditBalance,
                 PostpaidCreditBalance = Membership .PostpaidCreditBalance ,
                 RepresentativeMembershipId = Membership .RepresentativeId ,
                 AccountNumber = Membership .AccountNumber ,
                 RepresentativeMembership = Membership .RepresentativeId !=null,
                 MajorRoleIsSecuredPartyOrAgent = Membership .MajorRoleIsSecuredPartyOrAgent ,
                 isPayPointClient = Membership.isPayPointClient,
                RepresentativeMembershipName = RepresentativeMembershipName
                  
                  
                  
                  
            };
         
           

          

            return mv;
        }

        //public static Model.Membership.Membership  ConvertToMembership(this MembershipView mv)
        //{
        //    Model.Membership.Membership mb = new   Model.Membership.Membership()
        //    {
        //        Id = mv.Id,
        //        MembershipGroup = mv.MembershipGroup,
        //        MembershipTypeId = mv.MembershipTypeId,
        //        CreditCode = mv.CreditCode,
        //        CreditBalance = mv.CreditBalance



        //    };

        //    if (mv.AccountNumber != null)
        //    {

        //        RegularMembershipSetting rs = Membership.RegularMembershipSettings.Where(s => s.IsActive == true && s.IsDeleted == false).SingleOrDefault();
        //        if (rs != null)
        //        {
        //            mv.AccountNumber = rs.AccountNumber;

        //            mv.RepresentativeMembershipId = rs.RepresentativeId;
        //            if (rs.RepresentativeId != null)
        //                mv.RepresentativeMembership = true;
        //        }
        //    }

        //    return mv;
        //}
    }
}
