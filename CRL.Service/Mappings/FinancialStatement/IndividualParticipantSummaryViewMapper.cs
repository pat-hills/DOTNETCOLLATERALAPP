using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.FS;
using CRL.Service.Common;
using CRL.Service.Views.FinancialStatement;

namespace CRL.Service.Mappings.FinancialStatement
{
    public static class IndividualParticipantSummaryViewMapper
    {
        public static IndividualParticipantSummaryView ConvertToIndividualParticipantSummaryView(this IndividualParticipant IndividualParticipant, IndividualParticipantSummaryView fview )
        {
  
           
            fview.Name = NameHelper.GetFullName(IndividualParticipant.Identification.FirstName,
                                                  IndividualParticipant.Identification.MiddleName,
                                                  IndividualParticipant.Identification.Surname);
            fview.CardNo   =IndividualParticipant.Identification .CardNo ;
            fview.DOB =IndividualParticipant.DOB.ToString () ;
         
            fview.Gender  =IndividualParticipant.Gender ;
            //fview.Nationality  =IndividualParticipant.Nationality ;
            fview.PersonIdentificationTypeId =IndividualParticipant.PersonIdentificationTypeId ;
            fview.PersonIdentificationTypeName = IndividualParticipant.PersonIdentificationType.PersonIdentificationCardCategoryName;
            fview.Title =IndividualParticipant.Title ;
            
            return fview;

        }
    }
}
