using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;

using CRL.Model.ModelViews;


using CRL.Model.FS;
using CRL.Model.ModelViews.Amendment;
using CRL.Model.Memberships;
using CRL.Model.Memberships.IRepository;

namespace CRL.Model.ModelViewMappers
{
    public static class ClientReportViewMapper
    {


        public static ClientReportView ConvertToClientReportView(this  Membership model, IInstitutionRepository _institutionReporsitory,
            IUserRepository _userRepository, LookUpForFS lookUps=null)
        {

            ClientReportView iview = new ClientReportView();
            model.ConvertToClientReportView(iview, _institutionReporsitory, _userRepository,lookUps);
            return iview;
        }

        public static void ConvertToClientReportView(this Membership model,
            ClientReportView iview, IInstitutionRepository _institutionRepository,
            IUserRepository _userRepository,LookUpForFS LookUps=null)
        {
            //**This is bad and should be done in the query rather
            Institution institution = _institutionRepository.GetDbSet().Where(m => m.MembershipId == model.Id ).SingleOrDefault();
            if (institution != null)
            {
                iview.Name = institution.Name;
                iview.Address = institution.Address .Address ;
                iview.Address2  = institution.Address.Address2 ;
                iview.LGA = LookUps != null ? LookUps.LGAs.Where(s => s.LkValue == institution.LGAId ).SingleOrDefault().LkName : institution.LGA .Name;
                
                iview.City = institution.Address.City;
                iview.Country = LookUps != null? LookUps.Countries.Where(s => s.LkValue == institution .CountryId).SingleOrDefault().LkName : institution.Country.Name ;
                iview.County =  LookUps != null?LookUps.Countys .Where(s => s.LkValue == institution.CountyId ).SingleOrDefault().LkName : institution .County .Name ;
                iview.Email = institution.Address.Email;
                iview.Phone = institution.Address.Phone;
                iview.Nationality =  LookUps != null?LookUps.Nationalities .Where(s => s.LkValue == institution.NationalityId ).SingleOrDefault().LkName: institution .Nationality .Name ;
                iview.CompanyNo  = institution.CompanyNo;
                iview.SecuringPartyIndustryTypename =  LookUps != null?LookUps.SecuringPartyIndustryTypes .Where(s => s.LkValue == institution.SecuringPartyTypeId ).SingleOrDefault().LkName: institution .SecuringPartyType .SecuringPartyIndustryCategoryName ;
                iview.IsIndividual = false;
              
            }
            else
            {
                User user = _userRepository.GetDbSet().Where(m => m.MembershipId == model.Id).SingleOrDefault();
                iview.Name = NameHelper.GetFullName(user.FirstName, user.MiddleName, user.Surname);
                //iview.DOB = user.d;
                //iview.CardNo = user.ca.CardNo;
                iview.Address = user.Address.Address;
                iview.City = user.Address.City;
                iview.Country =  LookUps != null?LookUps.Countries.Where(s => s.LkValue == user.CountryId).SingleOrDefault().LkName : user.Country .Name ;
                iview.County =  LookUps != null?LookUps.Countys.Where(s => s.LkValue == user.CountyId).SingleOrDefault().LkName : user.County .Name ;
                //iview.PersonIdentificationTypename  = LookUps.IdentificationCardTypes.Where(s => s.LkValue == user.).SingleOrDefault().LkName;
                iview.Email = user.Address.Email;
                iview.Phone = user.Address.Phone;
                iview.Nationality =  LookUps != null?LookUps.Nationalities.Where(s => s.LkValue == user.NationalityId).SingleOrDefault().LkName: user.Nationality .Name ;
                iview.Gender = user.Gender;
                iview.Title = user.Title;
                iview.IsIndividual = true;
            }

            iview.Id = model.Id;

            
         

        }
    }
}
