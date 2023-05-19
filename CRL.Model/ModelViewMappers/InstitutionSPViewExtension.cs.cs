using CRL.Infrastructure.Domain;
using CRL.Infrastructure.Helpers;
using CRL.Model.FS;
using CRL.Model.ModelViews.FinancingStatement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViewMappers
{
    public static class InstitutionSPViewExtension
    {
        public static InstitutionParticipant ConvertToNewInstitutionParticipant(this InstitutionSPView view)
        {

            InstitutionParticipant model = new InstitutionParticipant();
            model.State = RecordState.New;
            view.ConvertToNewInstitutionParticipant(model);
            return model;
        }
        public static void ConvertToNewInstitutionParticipant(this InstitutionSPView view, InstitutionParticipant model)
        {
            view.ConvertToNewParticipant(model);
            model.Name = view.Name.TrimNull();
            model.CompanyNo = view.CompanyNo.TrimNull();
            model.SecuringPartyIndustryTypeId = view.SecuringPartyIndustryTypeId;
            model.Id = view.Id;
            model.RegistrantInstitutionId = view.RegistrantInstitutionId;
        }
        public static void ConvertToUpdateInstitutionParticipant(this InstitutionSPView view, InstitutionParticipant model)
        {
            view.ConvertToNewParticipant(model);
            model.Name = view.Name.TrimNull();
            model.CompanyNo = view.CompanyNo.TrimNull();
            model.SecuringPartyIndustryTypeId = view.SecuringPartyIndustryTypeId;
            model.Id = view.Id;
        }
        public static void ConvertToInstitutionSPParticipantView(this InstitutionParticipant model, InstitutionSPView iview)
        {
            model.ConvertToParticipantView(iview);
            iview.Name = model.Name;
            iview.CompanyNo = model.CompanyNo;
            iview.SecuringPartyIndustryTypeId = model.SecuringPartyIndustryTypeId;
            iview.OwnerOfCompany = model.OwnerOfCompany;
            iview.SecuringPartyIndustryTypename = model.SecuringPartyIndustryType.SecuringPartyIndustryCategoryName;
            iview.Id = model.Id;
            iview.RegistrantInstitutionId = model.RegistrantInstitutionId;
        }

    }
}
