using CRL.Infrastructure.Configuration;
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
    public static class InstitutionDebtorViewExtension
    {
        public static InstitutionParticipant ConvertToNewInstitutionParticipant(this InstitutionDebtorView view, IEnumerable<LKSectorOfOperationCategory> sectors)
        {

            InstitutionParticipant model = new InstitutionParticipant();
            model.State = RecordState.New;
            view.ConvertToNewInstitutionParticipant(model, sectors);
            return model;
        }
        public static void ConvertToNewInstitutionParticipant(this InstitutionDebtorView view, InstitutionParticipant model, IEnumerable<LKSectorOfOperationCategory> sectors)
        {
            view.ConvertToNewParticipant(model);
            model.Name = view.Name.TrimNull();

            model.SearchableName = Util.GetFlexibleBorrowerNameForSearch(view.Name);
            model.CompanyNo = view.CompanyNo.TrimNull();
            model.OtherSectorOfOperation = view.OtherSectorOp.TrimNull();
            model.BusinessTin = view.BusinessTin;
            model.BusinessRegistrationPrefixId = view.BusinessTinPrefix;
            model.DebtorTypeId = view.DebtorTypeId;
            model.DebtorIsAlreadyClientOfSecuredParty = view.DebtorIsAlreadyClientOfSecuredParty;
            model.Address.Phone = view.Phone;
            model.MajorityFemaleOrMaleOrBoth = view.MajorityFemaleOrMaleOrBoth;
            if (view.SectorOfOperationTypes != null) { model.AddSectorsOfOperationsForDebtor(view.SectorOfOperationTypes, sectors); }
        }
        public static void ConvertToUpdateInstitutionParticipant(this InstitutionDebtorView view, InstitutionParticipant model, IEnumerable<LKSectorOfOperationCategory> sectors)
        {
            view.ConvertToNewParticipant(model);
            model.Name = view.Name.TrimNull();
            model.SearchableName = Util.GetFlexibleBorrowerNameForSearch(view.Name);
            model.CompanyNo = view.CompanyNo.TrimNull();
            model.BusinessTin = view.BusinessTin;
            model.BusinessRegistrationPrefixId = view.BusinessTinPrefix;
            model.DebtorTypeId = view.DebtorTypeId;
            model.DebtorIsAlreadyClientOfSecuredParty = view.DebtorIsAlreadyClientOfSecuredParty;
            model.Address.Email = view.Email;
            model.Address.Phone = view.Phone;
            model.NationalityId = view.NationalityId;
            model.MajorityFemaleOrMaleOrBoth = view.MajorityFemaleOrMaleOrBoth;
            if (view.SectorOfOperationTypes != null) { model.AddSectorsOfOperationsForDebtor(view.SectorOfOperationTypes, sectors); }
        }

        public static void ConvertToInstitutionDebtorParticipantView(this InstitutionParticipant model, InstitutionDebtorView iview)
        {
            model.ConvertToParticipantView(iview);
            iview.Name = model.Name;
            iview.CompanyNo = model.CompanyNo;
            iview.DebtorIsAlreadyClientOfSecuredParty = model.DebtorIsAlreadyClientOfSecuredParty;
            iview.DebtorTypeId = model.DebtorTypeId;
            iview.OwnerOfCompany = model.OwnerOfCompany;
            iview.BusinessTinFullName = model.BusinessRegistrationPrefix.Name + model.CompanyNo;
            iview.MajorityFemaleOrMaleOrBoth = model.MajorityFemaleOrMaleOrBoth;
            iview.SectorOfOperationTypes = model.SectorOfOperationTypes.Select(s => s.Id).ToArray();
            iview.DebtorTypeName = model.DebtorType.CompanyCategoryName;
            iview.Email = model.Address.Email;
            iview.Phone = model.Address.Phone;
            iview.NationalityId = model.NationalityId;
            iview.OtherSectorOp = model.OtherSectorOfOperation;
            iview.BusinessTinPrefix = model.BusinessRegistrationPrefixId;
            
        }



    }
}
