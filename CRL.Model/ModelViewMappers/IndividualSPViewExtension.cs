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
    public static class IndividualSPViewExtension
    {
        public static IndividualParticipant ConvertToNewIndividualParticipant(this IndividualSPView view)
        {

            IndividualParticipant model = new IndividualParticipant();
            model.State = RecordState.New;
            view.ConvertToNewIndividualParticipant(model);
            return model;
        }
        public static void ConvertToNewIndividualParticipant(this IndividualSPView view, IndividualParticipant model)
        {
            view.ConvertToNewParticipant(model);
            model.DOB = view.DOB;           
            model.Identification.FirstName = view.Identification.FirstName.TrimNull();
            model.Identification.MiddleName = view.Identification.MiddleName.TrimNull();
            model.Identification.Surname = view.Identification.Surname.TrimNull();            
            model.Gender = view.Gender.TrimNull();
            model.Title = view.Title.TrimNull();
            model.RegistrantIndividualId = view.RegistrantIndividualId;
            



        }
        public static void ConvertToUpdateIndividualParticipant(this IndividualSPView view, IndividualParticipant model)
        {
            view.ConvertToNewParticipant(model);
            model.DOB = view.DOB;      
            model.Identification.FirstName = view.Identification.FirstName.TrimNull();
            model.Identification.MiddleName = view.Identification.MiddleName.TrimNull();
            model.Identification.Surname = view.Identification.Surname.TrimNull();            
            model.Gender = view.Gender.TrimNull();
            model.Title = view.Title.TrimNull();



        }

        public static void ConvertToIndividualSPParticipantView(this IndividualParticipant model, IndividualSPView iview)
        {
            model.ConvertToParticipantView(iview);
            iview.DOB = model.DOB;
            iview.Identification.FirstName = model.Identification.FirstName;
            iview.Identification.MiddleName = model.Identification.MiddleName;
            iview.Identification.Surname = model.Identification.Surname;
            iview.Gender = model.Gender;
            iview.Title = model.Title;
            iview.RegistrantIndividualId = model.RegistrantIndividualId;
 

        }
    }
}
