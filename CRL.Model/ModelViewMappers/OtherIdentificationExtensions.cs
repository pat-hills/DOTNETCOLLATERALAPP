using CRL.Model.FS;
using CRL.Model.ModelViews.FinancingStatement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;

namespace CRL.Model.ModelViewMappers
{
    public static class PersonIdentificationExtensions
    {
        public static PersonIdentification ConvertToNewPersonIdentification(this IdentificationView view)
        {

            PersonIdentification model = new PersonIdentification();
            model.State = CRL.Infrastructure.Domain.RecordState.New;
            view.ConvertToNewPersonIdentification(model);
            return model;
        }
        public static void ConvertToNewPersonIdentification(this IdentificationView view, PersonIdentification model)
        {
            //model.Identification.CardNo = view.CardNo.TrimNull();
            //model.Identification.OtherDocumentDescription = view.OtherDocumentDescription.TrimNull();
            model.Identification.FirstName = view.FirstName.TrimNull();
            model.Identification.MiddleName = view.MiddleName.TrimNull();
            model.Identification.Surname = view.Surname.TrimNull();
            //model.PersonIdentificationTypeId = view.PersonIdentificationTypeId;
        }

        public static void ConvertToUpdatePersonIdentification(this IdentificationView view, PersonIdentification model)
        {
            //model.Identification.CardNo = view.CardNo.TrimNull();
            //model.Identification.OtherDocumentDescription = view.OtherDocumentDescription.TrimNull();
            model.Identification.FirstName = view.FirstName.TrimNull();
            model.Identification.MiddleName = view.MiddleName.TrimNull();
            model.Identification.Surname = view.Surname.TrimNull();
            //model.PersonIdentificationTypeId = view.PersonIdentificationTypeId;
        }
      

        /// <summary>
        /// Convert list of 
        /// </summary>
        /// <param name="views"></param>
        /// <returns></returns>
        public static ICollection<PersonIdentification> ConvertToNewPersonIdentification(
                                              this ICollection<IdentificationView> views)
        {
            
            List<PersonIdentification> models = new List<PersonIdentification>();
            foreach (var view in views)
            {
                models.Add(view.ConvertToNewPersonIdentification ());
            }

            return models;
            
        }


        public static IdentificationView ConvertToPersonIdentificationView(this PersonIdentification model)
        {

            IdentificationView view = new IdentificationView();           
            model.ConvertToPersonIdentificationView (view);
            return view;
        }
        public static void ConvertToPersonIdentificationView(this  PersonIdentification model,IdentificationView view)
        {
            view.OtherIdentificationId = model.Id;
            //view.CardNo = model.Identification.CardNo;
            //view.OtherDocumentDescription = model.Identification.OtherDocumentDescription.TrimNull();
            view.FirstName = model.Identification.FirstName;
            view.MiddleName = model.Identification.MiddleName;
            view.Surname = model.Identification.Surname;
            //view.PersonIdentificationTypeId = model.PersonIdentificationTypeId;
            view.UniqueCode = model.UniqueCode;
        }

        /// <summary>
        /// Convert list of 
        /// </summary>
        /// <param name="views"></param>
        /// <returns></returns>
        public static ICollection<IdentificationView> ConvertToPersonIdentificationViews(
                                              this ICollection<PersonIdentification> models)
        {

            List<IdentificationView> views = new List<IdentificationView>();
            foreach (var model in models)
            {
                views.Add(model.ConvertToPersonIdentificationView ());
            }

            return views;

        }
    }
}
