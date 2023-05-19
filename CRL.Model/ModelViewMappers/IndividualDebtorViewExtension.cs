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
    public static class IndividualDebtorViewExtension
    {
        public static IndividualParticipant ConvertToNewIndividualParticipant(this IndividualDebtorView view, IEnumerable<LKSectorOfOperationCategory> sectors)
        {

            IndividualParticipant model = new IndividualParticipant();
            model.State = RecordState.New;
            view.ConvertToNewIndividualParticipant(model, sectors);
            return model;
        }
        public static void ConvertToNewIndividualParticipant(this IndividualDebtorView view, IndividualParticipant model, IEnumerable<LKSectorOfOperationCategory> sectors)
        {
            view.ConvertToNewParticipant(model);
            model.DOB = view.DOB;
            model.Identification.CardNo = view.CardNo.TrimNull();
            model.Identification.CardNo2 = view.CardNo2.TrimNull();
            model.Identification.OtherDocumentDescription = view.OtherDocumentDescription.TrimNull();
            model.Identification.FirstName = view.Identification.FirstName.TrimNull();
            model.Identification.MiddleName = view.Identification.MiddleName.TrimNull();
            model.Identification.Surname = view.Identification.Surname.TrimNull();
            model.DebtorIsAlreadyClientOfSecuredParty = view.DebtorIsAlreadyClientOfSecuredParty;

            model.Gender = view.Gender.TrimNull();
            model.Title = view.IndividualTitle.TrimNull();
            model.NationalityId = view.NationalityId;

            model.PersonIdentificationTypeId = Constants.BVNID;

            model.PersonIdentificationType2Id = Constants.PassportID;
            if (view.OtherIdentifications != null) { model.OtherPersonIdentifications = view.OtherIdentifications.ConvertToNewPersonIdentification(); }
            if (view.SectorOfOperationTypes != null) { model.AddSectorsOfOperationsForDebtor(view.SectorOfOperationTypes, sectors); }

            model.Address.Email = view.Email;
            model.Address.Phone = view.Phone;

        }
        public static void ConvertToUpdateIndividualParticipant(this IndividualDebtorView view, IndividualParticipant model, IEnumerable<LKSectorOfOperationCategory> sectors)
        {
            view.ConvertToUpdateParticipant(model);
            model.DOB = view.DOB;
            model.Identification.CardNo = view.CardNo.TrimNull();
            model.Identification.CardNo2 = view.CardNo2.TrimNull();
            model.Identification.OtherDocumentDescription = view.OtherDocumentDescription.TrimNull();

            model.Identification.FirstName = view.Identification.FirstName.TrimNull();
            model.Identification.MiddleName = view.Identification.MiddleName.TrimNull();
            model.Identification.Surname = view.Identification.Surname.TrimNull();
            model.NationalityId = view.NationalityId;
            model.DebtorIsAlreadyClientOfSecuredParty = view.DebtorIsAlreadyClientOfSecuredParty;



            model.Gender = view.Gender.TrimNull();
            model.Title = view.IndividualTitle.TrimNull();
            model.Address.Email = view.Email;
            model.Address.Phone = view.Phone;




            //We need to add new collaterals using the ConvertToNewCollateral, we use id==0 for an update process and collateral no for submitted updates since they will have an id already
            //We will add them to the financing statement to be modified
            //We will also set them to new
            //Note that the time of a collateral addition is when it is added
            foreach (IdentificationView i in view.OtherIdentifications.Where(s => s.OtherIdentificationId == 0 || String.IsNullOrWhiteSpace(s.UniqueCode)))
            {
                model.OtherPersonIdentifications.Add(i.ConvertToNewPersonIdentification());
            }

            //Updated Collaterals are handled here
            foreach (IdentificationView i in view.OtherIdentifications.Where(s => !String.IsNullOrWhiteSpace(s.UniqueCode)))
            {
                i.ConvertToUpdatePersonIdentification(model.OtherPersonIdentifications.Where(d => d.UniqueCode == i.UniqueCode).Single());
            }

            string[] submittedCollaterals = view.OtherIdentifications.Where(s => !String.IsNullOrWhiteSpace(s.UniqueCode)).Select(s => s.UniqueCode).ToArray();
            foreach (var remove in model.OtherPersonIdentifications.Where(s => !submittedCollaterals.Contains(s.UniqueCode)))
            {
                remove.IsDeleted = true;
            }




        }
        public static void ConvertToIndividualDebtorParticipantView(this IndividualParticipant model, IndividualDebtorView iview)
        {

            model.ConvertToParticipantView(iview);
            iview.DOB = model.DOB;
            iview.CardNo = model.Identification.CardNo;
            iview.CardNo2 = model.Identification.CardNo2;
            iview.OtherDocumentDescription = model.OtherDocumentDescription;
            iview.NationalityView = model.Nationality.Name;
            iview.NationalityId = model.NationalityId;
            iview.DebtorIsAlreadyClientOfSecuredParty = model.DebtorIsAlreadyClientOfSecuredParty;

            iview.PersonIdentificationTypeId = model.PersonIdentificationTypeId;
            iview.PersonIdentificationTypeId2 = model.PersonIdentificationType2Id;
            iview.PersonIdentificationTypename = model.PersonIdentificationType.PersonIdentificationCardCategoryName;
            iview.PersonIdentificationTypename2 = model.PersonIdentificationType2.PersonIdentificationCardCategoryName;
            IdentificationView _identification = new IdentificationView
            {

                FirstName = model.Identification.FirstName,
                MiddleName = model.Identification.MiddleName,
                Surname = model.Identification.Surname,


            };
            iview.Identification = _identification;
            iview.SectorOfOperationTypes = model.SectorOfOperationTypes.Select(s => s.Id).ToArray();
            iview.Gender = model.Gender;
            iview.IndividualTitle = model.Title;
            iview.DOB = model.DOB;
            iview.Id = model.Id;

            iview.Phone = model.Address.Phone;

            if (model.OtherPersonIdentifications.Count > 0)
            {
                iview.OtherIdentifications = (List<IdentificationView>)model.OtherPersonIdentifications.ConvertToPersonIdentificationViews();
            }




        }
    }
}
