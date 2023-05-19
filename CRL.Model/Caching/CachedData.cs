using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Caching;

namespace CRL.Model.ModelCaching
{
    
    public static class CachedData
    {
        public  const string CACHE_COUNTRIES = "Countries";
        public  const string CACHE_COUNTYS = "Countys";
        public const string CACHE_LGAS = "LGAs";
        public  const string CACHE_NATIONALITIES = "Nationalities";
        public  const string CACHE_SECURINGPARTYTYPES = "SecuringPartyTypes";
        public const string CACHE_FSLOANTYPES = "FSLoanTypes";
        public const string CACHE_TRANSACTIONTYPES = "TransactionTypes";
        public const string CACHE_COLLATERALTYPES = "CollateralTypes";
        public const string CACHE_CURRENCYTYPES = "CurrencyTypes";
        public const string CACHE_LOANTYPES = "LoanTypes";
        public const string CACHE_COLLATERALSUBTYPES = "CollateralSubTypes";
        public const string CACHE_IDENTIFICATIONCARDTYPES = "IdentificationCardTypes";
        public const string CACHE_SECTOROFOPERATIONS = "SectorOfOperations";
        public const string CACHE_SERVICEFEECATEGORIES = "ServiceFeeCategories";
        public const string CACHE_DEBTORTYPES = "DebtorTypes";
        public const string CACHE_AUDITTYPES = "AuditTypes";
        public const string CACHE_WORKFLOWS = "Workflow";
        
        public static ICollection <LookUpView>  GetLookUpData(string LookUpItem)
        {
            Cache cache = new Cache();
            ICollection<LookUpView> lkUp = cache.GetMyCachedItem(LookUpItem) as ICollection<LookUpView>;
            return lkUp;       
       
        }

        public static void AddLookUpToCache(ICollection <LookUpView> lkUp, string lkUpName)
        {
            Cache cache = new Cache();
            cache.AddToMyCache(lkUpName, lkUp, CachePriority.Default); 
        }

       

        
    }
}
