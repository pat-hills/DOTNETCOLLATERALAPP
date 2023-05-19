using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CRL.Model.Common;
using CRL.Model.ModelViews;
using CRL.Service.Views.Memberships;
using CRL.Infrastructure.Helpers;
using CRL.Model.ModelViews;
using CRL.Infrastructure.Configuration;
using CRL.Model.ModelViews.Memberships;
using CRL.Model.Memberships;

namespace CRL.Service.Mappings.Membership
{
    
    public static class InstitutionMapper
    {

        public static InstitutionView ConvertToInstitutionView(this InstitutionMembershipRegistrationView membershipRegView)
        {
            InstitutionView InstitutionView = new InstitutionView()
            {

                Address = membershipRegView.LegalAddress.Trim(),
                Address2 = membershipRegView.LegalAddress2.TrimNull(),
                City = membershipRegView.LegalCity.Trim(),
                CompanyNo = membershipRegView.CompanyNo.Trim(),
                CountryId = Constants.DefaultCountry ,
                CountyId = membershipRegView.LegalCountyId,
                LGAId = membershipRegView.LgaId,
                NationalityId = membershipRegView.LegalNationalityId ,
                Email = membershipRegView.LegalEmail.TrimNull(),
                Phone = membershipRegView.LegalPhone.Trim(),
                SecuringPartyTypeId = membershipRegView.SecuringPartyTypeId,
                MembershipView = new MembershipView()
                {
                    MembershipTypeId = Model.ModelViews.Enums.MembershipCategory.Client,
                    MajorRoleIsSecuredPartyOrAgent = membershipRegView.MajorRoleIsSecuredPartyOrAgent
                },
                Name = membershipRegView.Name

            };

            return InstitutionView;
        }
      

        public static InstitutionView ConvertToInstitutionView(this Institution Institution)
        {

            InstitutionView iview = new InstitutionView();
            Institution.ConvertToInstitutionView(iview);
            return iview;
        }

        public static void  ConvertToInstitutionView(this Institution Institution, InstitutionView iview)
        {
            iview.Name = Institution.Name;
            iview.ClientCode = Institution.Membership.ClientCode;
            iview.Email = Institution.Address.Email;
            iview.Phone = Institution.Address.Phone;
            iview.Address = Institution.Address.Address;
            iview.Address2 = Institution.Address.Address2;            
            iview.City = Institution.Address.City;            
            iview.CountryId = Institution.CountryId ;
            iview.County = Institution.County.Name;
            iview.CountyId = Institution.CountyId;
            iview.LGAId = Institution.LGAId;
            iview.LGA = (Institution.LGA !=null)? Institution.LGA.Name:"N/A";
            iview.Country = Institution.Country.Name;            
            iview.CompanyNo = Institution.CompanyNo;
            iview.SecuringPartyTypeId = Institution.SecuringPartyTypeId;
            iview.SecuringPartyType = Institution.SecuringPartyType.SecuringPartyIndustryCategoryName;
            iview.NationalityId = Institution.NationalityId;
            iview.Nationality = (Institution.Nationality  != null) ? Institution.Nationality .Name : "";         
            iview.Id = Institution.Id;
            
        }

        public static Institution ConvertToInstitution(this InstitutionView InstitutionView)
        {

            Institution i = new Institution();
            InstitutionView.ConvertToInstitution(i);
            return i;
        }

        public static void ConvertToInstitution(this InstitutionView InstitutionView, Institution i)
        {

            i.Address.Email = InstitutionView.Email.TrimNull ();
            i.Address.Phone = InstitutionView.Phone.TrimNull(); ;
            i.Address.Address = InstitutionView.Address.TrimNull(); ;
            i.Address.Address2 = InstitutionView.Address2.TrimNull(); ;
            i.Address.City = InstitutionView.City.TrimNull(); ;
            i.CountryId = InstitutionView.CountryId;
           
            i.CountyId = InstitutionView.CountyId;
            i.LGAId = InstitutionView.LGAId;
            i.Name = InstitutionView.Name.TrimNull(); 
            i.CompanyNo = InstitutionView.CompanyNo.TrimNull();
            i.SecuringPartyTypeId = InstitutionView.SecuringPartyTypeId;            
            i.NationalityId = InstitutionView.NationalityId;            
            i.Id = InstitutionView.Id;

        }

        public static IEnumerable<InstitutionView> ConvertToInstitutionViews(
                                              this IEnumerable<Institution> Institutions)
        {
            ICollection<InstitutionView> iviews = new List<InstitutionView>();
            foreach (Institution i in Institutions)
            {
                iviews.Add(i.ConvertToInstitutionView());
            }

            return iviews;


        }

        public static IEnumerable<Institution> ConvertToInstitutions(
                                               this IEnumerable<InstitutionView> InstitutionViews)
        {
            ICollection<Institution> ins = new List<Institution>();
            foreach (InstitutionView iview in InstitutionViews)
            {
                ins.Add(iview.ConvertToInstitution ());
            }

            return ins;
          
        }


        public static InstitutionGridView ConvertToInstitutionGridView(this Institution Institution)
        {

            InstitutionGridView iview = new InstitutionGridView();
            Institution.ConvertToInstitutionGridView(iview);
            return iview;
        }

        public static void ConvertToInstitutionGridView(this Institution Institution, InstitutionGridView iview)
        {
            iview.ClientCode = Institution.Membership.ClientCode;
            iview.Name = Institution.Name;
            iview.Email = Institution.Address.Email;
            iview.Phone = Institution.Address.Phone;           
            iview.Id = Institution.Id;
            iview.Address = Institution.Address.Address;            
            iview.City = Institution.Address.City;
            iview.CompanyNo = Institution.CompanyNo;
            iview.Country = Institution.Country.Name ;
            iview.County = Institution.County.Name ;
            iview.CreatedOn = Institution.CreatedOn;
            iview.IsActive = Institution.IsActive;
            iview.HasUsers = Institution.People.Where (p=>p.IsDeleted ==false).Count() > 0;
            iview.HasUnits = Institution.InstitutionUnits.Where ( (p=>p.IsDeleted ==false)).Count() > 0;
            iview.MembershipAccountType = Institution.Membership.MembershipAccountType.MembershipAccountCategoryName;
            if (Institution.SecuringPartyType != null)
            {
                iview.SecuredPartyType = Institution.SecuringPartyType.SecuringPartyIndustryCategoryName;
            }
            

        }

     
      

        public static IEnumerable <InstitutionGridView> ConvertToInstitutionGridViews(
                                              this IEnumerable<Institution> Institutions)
        {
            ICollection <InstitutionGridView> iviews = new List<InstitutionGridView>();
            foreach (Institution i in Institutions)
            {
                iviews.Add (i.ConvertToInstitutionGridView());
            }

            return iviews;
        }



      
    }

}
