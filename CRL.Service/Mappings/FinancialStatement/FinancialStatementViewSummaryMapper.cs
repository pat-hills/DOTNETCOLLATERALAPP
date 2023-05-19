using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.FS;
using CRL.Model.FS.Enums;
using CRL.Service.Common;
using CRL.Service.Views.FinancialStatement;

namespace CRL.Service.Mappings.FinancialStatement
{
    public static class FinancialStatementViewSummaryMapper
    {
        public static FSSummaryView ConvertToFinancialStatementSummaryView(this Model.FS.FinancialStatement FinancialStatement)
        {
            FSSummaryView fview = new FSSummaryView();
            fview.Id = FinancialStatement.Id;
            fview.RegistrationNo = FinancialStatement.RegistrationNo;
            fview.ExpiryDate  = FinancialStatement.ExpiryDate;
         
            fview.FinancialStatementTransactionTypeName = FinancialStatement.FinancialStatementTransactionType .FinancialStatementTransactionCategoryName ;
     
            fview.MaximumAmountSecured = FinancialStatement.MaximumAmountSecured;
            fview.MaximumAmountSecuredCurrencyId = FinancialStatement.MaximumAmountSecuredCurrencyId;
            fview.RegistrationDate = FinancialStatement.RegistrationDate;

            if (FinancialStatement.CreatedByUser.InstitutionId != null)
            {
                fview.Registrant = FinancialStatement.CreatedByUser.Institution.Name;
            }
            else
            {
                fview.Registrant = NameHelper.GetFullName(FinancialStatement.CreatedByUser.FirstName, FinancialStatement.CreatedByUser.MiddleName, FinancialStatement.CreatedByUser.Surname);
            }
           
            fview.SecurityAgreementDate = FinancialStatement.SecurityAgreementDate ;
            //fview.NumberOfCollaterals = FinancialStatement.Collaterals.Where(s => s.IsActive == true && s.IsDeleted == false && s.DischargeActivityId == null).ToList().Count();
            return fview;

        }

      
    }
}
