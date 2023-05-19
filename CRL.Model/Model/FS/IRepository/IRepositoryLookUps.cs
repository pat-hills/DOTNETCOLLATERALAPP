using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Configuration;
using CRL.Model.FS.Enums;

namespace CRL.Model.FS.IRepository
{
    public interface ILKCollateralCategoryRepository : IWriteRepository<LKCollateralCategory, CollateralCategory>
    {
       
    }



    public interface ILKCollateralSubTypeCategoryRepository : IWriteRepository<LKCollateralSubTypeCategory, int>
    {
    }

    public interface ILKCurrencyRepository : IWriteRepository<LKCurrency, int>
    {
      
    }

    public interface ILKFinancialStatementCategoryRepository : IWriteRepository<LKFinancialStatementLoanCategory, FinancialStatementLoanCategory>
    {

    }

    public interface ILKFinancialStatementTransactionCategoryRepository : IWriteRepository<LKFinancialStatementTransactionCategory, FinancialStatementTransactionCategory>
    {

    }

    public interface ILKPersonIdentificationCategoryRepository : IWriteRepository<LKPersonIdentificationCategory, int>
    {
        
    }

    public interface ILKSectorOfOperationCategoryRepository : IWriteRepository<LKSectorOfOperationCategory, int>
    {
        
    }

    public interface ILKSecuringPartyIndustryCategoryRepository : IWriteRepository<LKSecuringPartyIndustryCategory, int>
    {
        
    }

    public interface ILKFinancialStatementActivityCategoryRepository : IWriteRepository<LKFinancialStatementActivityCategory, int>
    {

    }
}
