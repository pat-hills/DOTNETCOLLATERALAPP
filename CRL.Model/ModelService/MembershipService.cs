using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Authentication;
using CRL.Infrastructure.Domain;
using CRL.Model.ModelViews.Enums;

using CRL.Model.Memberships;
using CRL.Model.Memberships.IRepository;

namespace CRL.Model.ModelViews
{
    public class MembershipServiceModel
    {
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IUserRepository  _userRepository;
        private readonly IRoleRepository _roleRepository;
        public AuditingTracker AuditTracker { get; set; }
        public SecurityUser ExecutingUser { get; set; }

        public MembershipServiceModel(IInstitutionRepository institutionRepository)
        {
            _institutionRepository =institutionRepository;
        }
        public MembershipServiceModel()
        {
           
        }
        public MembershipServiceModel(IUserRepository userRepository, IInstitutionRepository institutionRepository,IRoleRepository roleRepository,AuditingTracker tracker, SecurityUser user)
        {
            _userRepository = userRepository;
            _institutionRepository = institutionRepository;
            _roleRepository = roleRepository;
            AuditTracker = tracker;
            ExecutingUser = user;
        }

        public void SetupUserPassword(User user)
        {
            user.PasswordSalt = SecurityHelper.GenerateSaltValue();
            user.Password = SecurityHelper.HashPassword(user.Password, user.PasswordSalt, new SHA256Managed());
            user.LastActivityDate = DateTime.Now;
            user.LastPasswordChangeDate = DateTime.Now; ;      

        }

        public void SetupUserAccount(User user)
        {
            SetupUserPassword(user);
            user.ResetPasswordNextLogin = true;
            user.BuildUserSettingOnLogin = true;
            if (user.InstitutionId != null) //No membership assigned
            {

                user.MembershipId = _institutionRepository.FindBy((int)user.InstitutionId).MembershipId ; 
            }

            _userRepository.Add(user);

        }



      
        public void CreateClientIndividualMembership(User user, string MembershipAccountNumber, int? RepresentativeMembershipId, short MajorRoleIsSecuredPartyOrAgent, bool IsApproved)
        {
            Membership membership = new Membership();
            AuditTracker.Created.Add(membership);
            //We need to create membership
            if (RepresentativeMembershipId != null)
            {

                membership.RepresentativeId = RepresentativeMembershipId;
                membership.MembershipAccountTypeId = MembershipAccountCategory.RegularRepresentative;
                membership.AccountNumber = MembershipAccountNumber; //**Check that this is not empty

            }
            else
            {
                membership.MembershipAccountTypeId = MembershipAccountCategory.NonRegular;
            }
           
            //Generate a credit code which is not that too necessary
            membership.PrepaidCreditBalance = 0;              
            membership.MembershipGroup = 1;
            membership.MembershipTypeId = MembershipCategory.Client;
            membership.isIndividualOrLegalEntity = 1;
            membership.MajorRoleIsSecuredPartyOrAgent = MajorRoleIsSecuredPartyOrAgent;
            
           
            membership.IsActive = IsApproved;
            
            user.Membership = membership;
            
            
           

        }

        public void EditClientIndividualMembership(User user, string MembershipAccountNumber, int? RepresentativeMembershipId)
        {
            Membership activeSettings = user.Membership;
            //**Cannot change just like that we need to know if there are existing postpaid transaction according to the business rules
            if (RepresentativeMembershipId == null)//Another change is when we had something but we have decided that we do not need anymore then we do not 
            {
                
                activeSettings.MembershipAccountTypeId = MembershipAccountCategory.NonRegular;
                activeSettings.AccountNumber = MembershipAccountNumber;
                AuditTracker.Updated.Add(user.Membership);
            }
            else
            {

                activeSettings.RepresentativeId = RepresentativeMembershipId;
                activeSettings.AccountNumber = MembershipAccountNumber;
                user.Membership.MembershipAccountTypeId = MembershipAccountCategory.RegularRepresentative;
                AuditTracker.Updated.Add(user.Membership);
                

                
            }

        }
        public void GenerateSubmittedClientMembership(Institution institution, User user, Role AdminRole)
        {
            institution.People.Add((Person)user); //Add user to institution

            //Create the new membership and audit it
            Membership membership = new Membership();
            AuditTracker.Created.Add(membership);

            //Set the membership to client and make it inactive
            membership.MembershipTypeId = MembershipCategory.Client;
            membership.IsActive =false;
            membership.MajorRoleIsSecuredPartyOrAgent = 1;
            membership.MembershipAccountTypeId = MembershipAccountCategory.NonRegular;
            membership.isIndividualOrLegalEntity = 2;
            

            //Assign membership 
            institution.Membership = membership;

            //Setup user password and set the build user settings after login to true
            SetupUserPassword(user);
            user.BuildUserSettingOnLogin = true;
            user.Institution = institution;
            user.Membership = membership;
            
            user.Roles.Add(AdminRole);
          
        }

       

        public void CreateClientInstitutionMembership(Institution client, string MembershipAccountNumber, int? RepresentativeMembershipId, short MajorRoleIsSecuredPartyOrAgent, bool IsApproved)
        {

            Membership membership = new Membership();
            AuditTracker.Created.Add(membership);
           // We need to create membership
           
                //if (RepresentativeMembershipId != null && !String.IsNullOrEmpty(MembershipAccountNumber))
                //{

                //    membership.RepresentativeId = RepresentativeMembershipId;
                //    membership.MembershipAccountTypeId = MembershipAccountCategory.RegularRepresentative;

                //}
                //else if (!String.IsNullOrEmpty(MembershipAccountNumber))
                //{



                //    membership.AccountNumber = MembershipAccountNumber;
                //    membership.MembershipAccountTypeId = MembershipAccountCategory.Regular;

                //}
                //else
                //{
                //    membership.AccountNumber = MembershipAccountNumber;
                //    membership.MembershipAccountTypeId = MembershipAccountCategory.NonRegular;
                //}
           
            //Generate a credit code which is not that too necessary
            membership.PrepaidCreditBalance = 0;
            membership.isIndividualOrLegalEntity = 2;            
            membership.MembershipGroup = 1;
            membership.MajorRoleIsSecuredPartyOrAgent = MajorRoleIsSecuredPartyOrAgent;
            membership.MembershipAccountTypeId = MembershipAccountCategory.NonRegular;
            membership.MembershipTypeId = MembershipCategory.Client;
            membership.IsActive = IsApproved;
            client.Membership = membership;
            _institutionRepository.Add(client);


        }

       

        //**We need to remodify this guy
        public void EditClientInstitutionMembership(Institution client, MembershipAccountCategory membershipAccountTypeId, string MembershipAccountNumber, int? RepresentativeMembershipId, short MajorRoleIsSecuredPartyOrAgent, bool isPayPointUser)
        {
            //RegularMembershipSetting activeSettings = client.Membership.RegularMembershipSettings.Where(s => s.IsActive == true).SingleOrDefault();
            //**Cannot change just like that we need to know if there are existing postpaid transaction according to the business rules
            client.Membership.MajorRoleIsSecuredPartyOrAgent = MajorRoleIsSecuredPartyOrAgent;
            client.Membership.isPayPointClient = isPayPointUser;
            //((String.IsNullOrEmpty(MembershipAccountNumber) && RepresentativeMembershipId == null))
            //    if (membershipAccountTypeId == MembershipAccountCategory.NonRegular )//Another change is when we had something but we have decided that we do not need anymore then we do not 
            //    {
            //        //We have changed from regular or regular representative to nonregular


            //        client.Membership.MembershipAccountTypeId = MembershipAccountCategory.NonRegular;
            //        client.Membership.RepresentativeId = null;
            //        client.Membership.Representative = null;
            //        client.Membership.AccountNumber = null;
            //        AuditTracker.Updated.Add(client.Membership); 
            //    }
            //    else if (membershipAccountTypeId == MembershipAccountCategory.Regular )
            //    {

            //            client.Membership.AccountNumber = MembershipAccountNumber;
            //            client.Membership.RepresentativeId = null;
            //            client.Membership.Representative = null;
            //            client.Membership.MembershipAccountTypeId = MembershipAccountCategory.Regular;

            //            AuditTracker.Updated.Add(client.Membership); 
            //        }
            //        else
            //        {

            //            client.Membership.AccountNumber = MembershipAccountNumber;      
            //        client .Membership .RepresentativeId = RepresentativeMembershipId ;
            //            client.Membership.MembershipAccountTypeId = MembershipAccountCategory.RegularRepresentative;
            //            AuditTracker.Updated.Add(client.Membership); 

            //        }

            //}
        }
    }
     
}
