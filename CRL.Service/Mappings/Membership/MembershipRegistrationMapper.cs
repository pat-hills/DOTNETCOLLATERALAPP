using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.ModelViews;
using CRL.Service.Common;
using CRL.Service.Views.Memberships;
using CRL.Service.Views.MembershipRegistration;
using CRL.Model.ModelViews;
using CRL.Model.Memberships;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Service.Mappings.Membership
{
    public static class MembershipRegistrationMapper
    {
        public static MembershipRegistrationView ConvertToMembershipRegistrationView(this MembershipRegistrationRequest model)
        {

            MembershipRegistrationView iview = new MembershipRegistrationView();
            model.ConvertToMembershipRegistrationView(iview);
            return iview;
        }

        public static void ConvertToMembershipRegistrationView(this MembershipRegistrationRequest model, MembershipRegistrationView iview)
        {
            //Determine the type of MembershipRegistration this is
            iview.FirstName  = model.FirstName ;
            iview.Email = model.AdminAddress.Email;
            iview.Phone = model.AdminAddress.Phone;
            iview.Address = model.AdminAddress .Address;
            iview.Address2 = model.AdminAddress.Address2;
            iview.City = model.AdminAddress .City;
            iview.Country  = model.Country .Name ;
            iview.County = model.County.Name;
            iview.CountryId = model.CountryId;
            iview.CountyId = model.CountyId;
            iview.NationalityId = model.NationalityId;
            iview.Nationality = model.Nationality.Name;
            iview.AccountName = model.AccountName;
            iview.AccountNumberWithCentralBank = model.AccountNumberWithCentralBank;
            iview.RepresentativeInstitutionClientId = model.RepresentativeInstitutionClientId;
            iview.MiddleName = model.MiddleName;
            iview.Gender = model.Gender;
            iview.MembershipAccountType = model.MembershipAccountType.MembershipAccountCategoryName;
            iview.MembershipAccountTypeId = model.MembershipAccountTypeId;
            iview.Password = model.Password;
            iview.RequestNo = model.RequestNo;
            iview.Surname = model.Surname;
            iview.Title = model.Title;
            iview.Id = model.Id;

        }
        public static InstitutionMembershipRegistrationView ConvertToInstitutionMembershipRegistrationView(this LegalEntityMembershipRegistrationRequest model)
        {

            InstitutionMembershipRegistrationView iview = new InstitutionMembershipRegistrationView();
            model.ConvertToInstitutionMembershipRegistrationView(iview);
            return iview;
        }

        public static void ConvertToInstitutionMembershipRegistrationView(this LegalEntityMembershipRegistrationRequest model, InstitutionMembershipRegistrationView iview)
        {
            model.ConvertToMembershipRegistrationView(iview);
            iview.LegalEmail = model.LegalEntityAddress .Email ;
            iview.LegalCity = model.LegalEntityAddress.City;
            iview.LegalAddress = model.LegalEntityAddress.Address;
            iview.LegalAddress2 = model.LegalEntityAddress.Address2;
            iview.LegalPhone = model.LegalEntityAddress.Phone;
            iview.LegalCountry = model.LegalCountry.Name;
            iview.LegalCounty = model.LegalCounty.Name;
            iview.LegalCountryId = model.LegalCountryId;
            iview.LegalCountyId = model.LegalCountyId;
            iview.LegalNationality = model.LegalNationality.Name;
            iview.LegalNationalityId = model.LegalNationalityId;
            iview.SecuringPartyType = model.SecuringPartyType.SecuringPartyIndustryCategoryName;
            iview.SecuringPartyTypeId = model.SecuringPartyTypeId;

        }


        public static MembershipRegistrationRequest ConvertToMembershipRegistrationRequest(this MembershipRegistrationView iview)
        {

            MembershipRegistrationRequest model = new MembershipRegistrationRequest();
            iview.ConvertToMembershipRegistrationRequest(model);
            return model;
        }

        public static void ConvertToMembershipRegistrationRequest(this MembershipRegistrationView iview, MembershipRegistrationRequest model)
        {
            //Determine the type of MembershipRegistration this is
            model.FirstName =  iview.FirstName ;
            model.AdminAddress.Email = iview.Email ;
            model.AdminAddress.Phone =  iview.Phone ;
            model.AdminAddress.Address = iview.Address ;
             model.AdminAddress.City = iview.City;
             
             model.CountryId = iview.CountryId;
             model.NationalityId = iview.NationalityId;
             model.CountyId  = iview.CountyId ;
            
            model.AccountName = iview.AccountName ;
             model.AccountNumberWithCentralBank = iview.AccountNumberWithCentralBank;
             model.RepresentativeInstitutionClientId = iview.RepresentativeInstitutionClientId;
             model.MiddleName = iview.MiddleName;
             model.Gender = iview.Gender;
             
             model.MembershipAccountTypeId = iview.MembershipAccountTypeId;
            model.Password = iview.Password ;
             model.RequestNo = iview.RequestNo;
             model.Surname = iview.Surname;
             model.Title = iview.Title;
            model.Id = iview.Id;

        }

        public static LegalEntityMembershipRegistrationRequest ConvertToLegalEntityMembershipRegistrationRequest(this InstitutionMembershipRegistrationView iview)
        {

            LegalEntityMembershipRegistrationRequest model = new LegalEntityMembershipRegistrationRequest();
            iview.ConvertToLegalEntityMembershipRegistrationRequest(model);
            return model;
        }

        public static void ConvertToLegalEntityMembershipRegistrationRequest(this InstitutionMembershipRegistrationView iview, LegalEntityMembershipRegistrationRequest model)
        {
            iview.ConvertToMembershipRegistrationRequest(model);
            model.LegalEntityAddress.Email = iview.LegalEmail;
            model.LegalEntityAddress.City = iview.LegalCity;
            model.LegalEntityAddress.Address = iview.LegalAddress;
            model.LegalEntityAddress.Phone = iview.LegalPhone;
            model.LegalCountryId = iview.LegalCountryId;
            model.LegalCountyId = iview.LegalCountyId;
            model.LegalNationalityId = iview.LegalNationalityId;
            model.SecuringPartyTypeId = iview.SecuringPartyTypeId;
            model.Name = iview.Name;

        }

        public static  MembershipRegistrationsGridView  ConvertToMembershipRegistrationGridView(this MembershipRegistrationRequest model)
        {
            MembershipRegistrationsGridView view = new MembershipRegistrationsGridView();
            view.Id = model.Id;
            view.Name = NameHelper.GetFullName(model.FirstName, model.MiddleName, model.Surname);
            view.SecuredPartyType = "Individual";
            view.AccountType = model.MembershipAccountType.MembershipAccountCategoryName;
            view.AccountNo = model.AccountNumberWithCentralBank;
            view.CreatedOn = model.CreatedOn;
            view.IsValid = true;
          

            return view;
         
            
        }

        public static MembershipRegistrationsGridView ConvertToMembershipRegistrationGridViewFromLegal(this LegalEntityMembershipRegistrationRequest model)
        {

            MembershipRegistrationsGridView view = new MembershipRegistrationsGridView();
            view.Id = model.Id;
            view.Name = model.Name;
            view.SecuredPartyType = model.SecuringPartyType .SecuringPartyIndustryCategoryName ;
            view.AccountType = model.MembershipAccountType.MembershipAccountCategoryName;
            view.AccountNo = model.AccountNumberWithCentralBank;
            view.CreatedOn = model.CreatedOn;
            if (model.MembershipAccountTypeId == Model.ModelViews.Enums.MembershipAccountCategory.Regular)
            {
                if (!String.IsNullOrEmpty(model.AccountNumberWithCentralBank))
                {
                    view.IsValid = true;
                }
                else
                {
                    view.IsValid = false;
                }
            }

            return view;
        }




        public static IEnumerable<MembershipRegistrationsGridView> ConvertToMembershipRegistrationsGridView(
                                              this IEnumerable<MembershipRegistrationRequest> models)
        {
            ICollection<MembershipRegistrationsGridView> iviews = new List<MembershipRegistrationsGridView>();
            foreach (var i in models)
            {
                if (i is LegalEntityMembershipRegistrationRequest)
                {
                    iviews.Add(((LegalEntityMembershipRegistrationRequest)(i)).ConvertToMembershipRegistrationGridViewFromLegal ());
                }
                else
                {
                     iviews.Add(i.ConvertToMembershipRegistrationGridView());
                }
               
            }

            return iviews;
        }

    }
}
