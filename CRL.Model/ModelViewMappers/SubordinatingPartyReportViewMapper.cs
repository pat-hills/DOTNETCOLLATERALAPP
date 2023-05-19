using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.FS;
using CRL.Model.ModelViews.Amendment;


namespace CRL.Model.ModelViewMappers
{
    public static class SubordinatingPartyReportViewMapper
    {
        public static SubordinatingPartyReportView ConvertToSubordinatingPartyReportView(this SubordinatingParty model, LookUpForFS lookUps=null)
        {

            SubordinatingPartyReportView iview = new SubordinatingPartyReportView();
            model.ConvertToSubordinatingPartyReportView(iview, lookUps );
            return iview;
        }

        public static void ConvertToSubordinatingPartyReportView(this SubordinatingParty model, SubordinatingPartyReportView iview, LookUpForFS lookUps=null)
        {
            iview.Address = model.Address.Address;
            iview.City = model.Address.City;
            iview.CountryId = model.CountryId;
            iview.CountyId = model.CountyId;
          

                iview.Country = lookUps == null ? model.Country.Name : lookUps.Countries .Where(s => s.LkValue == (int)model.CountryId).SingleOrDefault().LkName;
                iview.County = lookUps == null ? model.County.Name : lookUps.Countys .Where(s => s.LkValue == (int)model.CountyId ).SingleOrDefault().LkName;
                iview.Nationality = lookUps == null ? model.Nationality .Name : lookUps.Nationalities.Where(s => s.LkValue == (int)model.NationalityId ).SingleOrDefault().LkName;
                if (model.LGAId !=null )iview.LGA = lookUps == null ? model.LGA.Name : lookUps.LGAs.Where(s => s.LkValue == (int)model.LGAId ).SingleOrDefault().LkName;
            
           
          
            iview.Email = model.Address.Email;
            iview.Phone = model.Address.Phone;
            iview.isBeneficiary = model.isBeneficiary;
            iview.NationalityId = model.NationalityId;
            
            iview.isBeneficiary = model.isBeneficiary;
            iview.Id = model.Id;
            

            if (model is IndividualSubordinatingParty)
            {
                IndividualSubordinatingParty individualModel = model as IndividualSubordinatingParty;
                iview.DOB = individualModel.DOB;
                iview.CardNo = individualModel.Identification.CardNo;
                iview.Name = NameHelper.GetFullName(individualModel.Identification.FirstName, individualModel.Identification.MiddleName, individualModel.Identification.Surname);
                iview.OtherDocumentDescription = individualModel.Identification.OtherDocumentDescription;
                iview.PersonIdentificationTypeId = individualModel.PersonIdentificationTypeId;
                iview.Gender = individualModel.Gender;
                iview.Title = individualModel.Title;
                iview.IsIndividual = true;

            }
            else
            {
                InstitutionSubordinatingParty institueModel = model as InstitutionSubordinatingParty;
                iview.Name = institueModel.Name;
                iview.CompanyNo = institueModel.CompanyNo;
                iview.SecuringPartyIndustryTypeId = institueModel.SecuringPartyIndustryTypeId;
                iview.SecuringPartyIndustryTypename = lookUps == null ? institueModel .SecuringPartyIndustryType .SecuringPartyIndustryCategoryName  : lookUps.SecuringPartyIndustryTypes .Where(s => s.LkValue == (int)institueModel.SecuringPartyIndustryTypeId ).SingleOrDefault().LkName;                
                iview.IsIndividual = false;
            }

        }

       
     

       
    }
}
