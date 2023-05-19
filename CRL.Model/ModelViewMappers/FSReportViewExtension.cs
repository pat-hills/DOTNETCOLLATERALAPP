using CRL.Model.FS;
using CRL.Model.ModelViews.FinancingStatement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViewMappers
{
    public static class FSReportViewExtension
    {
        public static FSReportView ConvertToFSReportView(this FinancialStatement model, LookUpForFS LookUps = null)
        {

            FSReportView iview = new FSReportView();
            model.ConvertToFSReportView(iview, LookUps);
            return iview;
        }



        public static void ConvertToFSReportView(this FinancialStatement model, FSReportView iview, LookUpForFS LookUps = null)
        {
            iview.Id = model.Id;
            iview.ExpiryDate = model.ExpiryDate;
            iview.MaximumAmountSecured = model.MaximumAmountSecured;
            iview.RegistrationDate = model.RegistrationDate;
            iview.RegistrationNo = model.RegistrationNo;
            iview.SecurityAgreementDate = model.SecurityAgreementDate;
            iview.IsDischarged = model.IsDischarged;
            if (LookUps != null)
            {
                iview.FinancialStatementTransactionTypeName = LookUps == null ? model.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName : LookUps.FinancialStatementTransactionTypes.Where(s => s.LkValue == (int)model.FinancialStatementTransactionTypeId).SingleOrDefault().LkName;
                if (model.FinancialStatementLoanTypeId != null)
                {
                    iview.FinancialStatementLoanTypeName = LookUps == null ? model.FinancialStatementLoanType.FinancialStatementCategoryName : LookUps.FinancialStatementLoanType.Where(s => s.LkValue == (int)model.FinancialStatementLoanTypeId).SingleOrDefault().LkName;
                }
                else
                {
                    iview.FinancialStatementLoanTypeName = "N/A";
                }
                iview.CollateralTypeName = LookUps == null ? model.CollateralType.CollateralCategoryName : LookUps.CollateralTypes.Where(s => s.LkValue == (int)model.CollateralTypeId).SingleOrDefault().LkName;
                iview.MaximumAmountSecuredCurrencyName = LookUps == null ? model.MaximumAmountSecuredCurrency.CurrencyName : LookUps.Currencies.Where(s => s.LkValue == (int)model.MaximumAmountSecuredCurrencyId).SingleOrDefault().LkName;
            }
            else
            {
                iview.FinancialStatementTransactionTypeName = model.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName;
                if (model.FinancialStatementLoanType != null)
                {
                    iview.FinancialStatementLoanTypeName = model.FinancialStatementLoanType.FinancialStatementCategoryName;
                }
                iview.CollateralTypeName = model.CollateralType.CollateralCategoryName;
                iview.MaximumAmountSecuredCurrencyName = model.MaximumAmountSecuredCurrency.CurrencyName;
            }



        }
    }
}
