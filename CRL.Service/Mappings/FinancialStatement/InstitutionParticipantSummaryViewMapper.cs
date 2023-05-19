using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.FS;
using CRL.Service.Views.FinancialStatement;

namespace CRL.Service.Mappings.FinancialStatement
{
   

    public static class InstitutionParticipantSummaryViewMapper
    {
        public static InstitutionParticipantSummaryView ConvertToInstitutionParticipantSummaryView(this InstitutionParticipant InstitutionParticipant, InstitutionParticipantSummaryView fview )
        {
          
          
            fview.Name =InstitutionParticipant.Name ;
            fview.CompanyNo   =InstitutionParticipant.CompanyNo;
            fview.BusinessTin = InstitutionParticipant.BusinessTin;

          
            if (InstitutionParticipant.DebtorType != null)
            {
                fview.CompanyTypeId = (int)InstitutionParticipant.DebtorTypeId ;

                fview.CompanyTypeName = InstitutionParticipant.DebtorType.CompanyCategoryName;
            }
            fview.GenderComposition = InstitutionParticipant.MajorityFemaleOrMaleOrBoth == 3 ? "Equal Distribution" : InstitutionParticipant.MajorityFemaleOrMaleOrBoth == 2 ? "Male" : "False";
            //fview.IncorporationDate =InstitutionParticipant.IncorporationDate.ToString () ;
       
            if (InstitutionParticipant.SecuringPartyIndustryType !=null)
                fview.SecuringPartyIndustryName  =InstitutionParticipant.SecuringPartyIndustryType .SecuringPartyIndustryCategoryName ; ;
            fview.SecuringPartyIndustryTypeId  =InstitutionParticipant.SecuringPartyIndustryTypeId  ;
       
            
            return fview;

        }
    }
}
