using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Model.FS;
using CRL.Model.ModelViews.Amendment;


namespace CRL.Model.ModelViewMappers
{
    public static  class InstitutionSubordinatingPartyMapper
    {
        public static InstitutionSubordinatingPartyView ConvertToInstitutionSubordinatingPartyView(this InstitutionSubordinatingParty model)
        {
            InstitutionSubordinatingPartyView iview = new InstitutionSubordinatingPartyView();
            model.ConvertToInstitutionSubordinatingPartyView(iview);
            return iview;
        }

        public static void ConvertToInstitutionSubordinatingPartyView(this InstitutionSubordinatingParty model, InstitutionSubordinatingPartyView iview)
        {
            iview.Name  = model.Name ;
            iview.CompanyNo = model.CompanyNo;
            iview.SecuringPartyIndustryTypeId = model.SecuringPartyIndustryTypeId;
            iview.SecuringPartyIndustryTypename = model.SecuringPartyIndustryType.SecuringPartyIndustryCategoryName;
            iview.RelatedInstitutionId = model.RelatedInstitutionId;
            iview.Id = model.Id;
        }

        public static InstitutionSubordinatingParty ConvertToInstitutionSubordinatingParty(this InstitutionSubordinatingPartyView iview)
        {

            InstitutionSubordinatingParty model = new InstitutionSubordinatingParty();
            iview.ConvertToInstitutionSubordinatingParty(model);
            return model;
        }
        public static void ConvertToInstitutionSubordinatingParty(this InstitutionSubordinatingPartyView iview, InstitutionSubordinatingParty model)
        {
            model.Name = iview.Name;
            model.CompanyNo = iview.CompanyNo;
            model.SecuringPartyIndustryTypeId = iview.SecuringPartyIndustryTypeId;            
            model.Id = iview.Id;

        }        
    }
}
