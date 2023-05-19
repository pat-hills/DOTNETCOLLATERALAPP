using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.FS;
using CRL.Model.ModelViews.Amendment;


namespace CRL.Model.ModelViewMappers
{
    public static class IndividualSubordinatingPartyMapper
    {
        public static IndividualSubordinatingPartyView ConvertToIndividualSubordinatingPartyView(this IndividualSubordinatingParty model)
        {
            IndividualSubordinatingPartyView iview = new IndividualSubordinatingPartyView();
            model.ConvertToIndividualSubordinatingPartyView(iview);
            return iview;
            
           
        }

        public static void ConvertToIndividualSubordinatingPartyView(this IndividualSubordinatingParty model, IndividualSubordinatingPartyView iview)
        {

          
            iview.DOB = model.DOB;
            //iview.Identification.CardNo = model.Identification.CardNo;
            iview.Identification.FirstName = model.Identification.FirstName;
            iview.Identification.MiddleName = model.Identification.MiddleName;
            iview.Identification.Surname = model.Identification.Surname;
            //iview.Identification.OtherDocumentDescription = model.Identification.OtherDocumentDescription;
            //iview.Identification.PersonIdentificationTypeId = model.PersonIdentificationTypeId;
            iview.Gender = model.Gender;
            iview.Title = model.Title;
            iview.Id = model.Id;

        }
        public static IndividualSubordinatingParty ConvertToIndividualSubordinatingParty(this IndividualSubordinatingPartyView iview)
        {

            IndividualSubordinatingParty model = new IndividualSubordinatingParty();
            iview.ConvertToIndividualSubordinatingParty(model);
            return model;
        }
        public static void ConvertToIndividualSubordinatingParty(this IndividualSubordinatingPartyView iview, IndividualSubordinatingParty model)
        {
            //Determine the type of MembershipRegistration this is
            model.DOB = iview.DOB;
            //model.Identification.CardNo = iview.Identification.CardNo;
            model.Identification.FirstName = iview.Identification.FirstName;
            model.Identification.MiddleName = iview.Identification.MiddleName;
            model.Identification.Surname = iview.Identification.Surname;
            //model.Identification.OtherDocumentDescription = iview.Identification.OtherDocumentDescription;
            model.Gender = iview.Gender;
            //model.PersonIdentificationTypeId = iview.Identification .PersonIdentificationTypeId  ;
            model.Title = iview.Title;

        }



       

     
    }
}
