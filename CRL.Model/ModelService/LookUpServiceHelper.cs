using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.Helpers;
using CRL.Model;
using CRL.Model.Common.IRepository;
using CRL.Model.Configuration.IRepository;
using CRL.Model.FS.IRepository;

using CRL.Service.Common;
using CRL.Model.ModelCaching;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Enums;
using CRL.Model.Memberships.IRepository;
using CRL.Model.Memberships;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Model.ModelService
{
    public static class LookUpServiceModel
    {
        public static ICollection<LKLGAView> LGAs(ILKLGARepository _repository)
        {
            ICollection<LKLGAView> LGAs = new List<LKLGAView>();

            //Load all the institution types
            var query = _repository.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).OrderBy(s => s.Name);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LKLGAView lk = new LKLGAView();
                lk.Id = (int)c.Id;
                lk.Name = c.Name;
                lk.StateId = c.CountyId;
                LGAs.Add(lk);
            }

            return LGAs;

        }



        public static ICollection<LookUpView> LGAsLK(ILKLGARepository _repository)
        {
            ICollection<LookUpView> LGAs = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).OrderBy(s => s.Name);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.Name;
                LGAs.Add(lk);
            }

            return LGAs;
        }



        public static ICollection<LookUpView> BusinessRegPrefixes(ILKRegistrationPrefixRepository _repository)
        {
            ICollection<LookUpView> prefixes = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).OrderBy(s => s.Name);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.Name;
                prefixes.Add(lk);
            }

            return prefixes;
        }
        public static ICollection<LookUpView> Countys(ILKCountyRepository _repository)
        {
            ICollection<LookUpView> Countys = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).OrderBy(s => s.Name);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.Name;
                Countys.Add(lk);
            }
            CachedData.AddLookUpToCache(Countys, CachedData.CACHE_COUNTYS);
            return Countys;

        }
        public static ICollection<LookUpView> Countries(ILKCountryRepository _repository)
        {
            ICollection<LookUpView> Countries = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.Name;
                Countries.Add(lk);
            }

            CachedData.AddLookUpToCache(Countries, CachedData.CACHE_COUNTRIES);
            return Countries;

        }

        public static ICollection<LookUpView> Nationalities(ILKNationalityRepository _repository)
        {
            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.Name;
                lookups.Add(lk);
            }
            CachedData.AddLookUpToCache(lookups, CachedData.CACHE_NATIONALITIES);
            return lookups;

        }

        public static ICollection<LookUpView> RegularInstitutions(IInstitutionRepository _repository)
        {
            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.Membership.IsActive == true && s.IsDeleted == false && s.Membership.MembershipAccountTypeId == MembershipAccountCategory.Regular);  //**Parametrise Bank
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.MembershipId;
                lk.LkName = c.Name;
                lookups.Add(lk);
            }

            return lookups;

        }

        public static ICollection<LookUpView> SecuringPartyTypes(ILKSecuringPartyIndustryCategoryRepository _repository)
        {
            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.SecuringPartyIndustryCategoryName;
                lookups.Add(lk);

            }
            CachedData.AddLookUpToCache(lookups, CachedData.CACHE_SECURINGPARTYTYPES);
            return lookups;

        }

        public static ICollection<LookUpView> InstitutionUnits(IInstitutionUnitRepository _repository, int InstitutionId)
        {
            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false && s.InstitutionId == InstitutionId);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.Name;
                lookups.Add(lk);

            }

            return lookups;

        }

        public static ICollection<LookUpView> IdentificationCardTypes(ILKPersonIdentificationCategoryRepository _repository)
        {


            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.PersonIdentificationCardCategoryName;
                lookups.Add(lk);

            }
            CachedData.AddLookUpToCache(lookups, CachedData.CACHE_IDENTIFICATIONCARDTYPES);
            return lookups;

        }
        public static ICollection<LookUpView> FinancialStatementLoanType(ILKFinancialStatementCategoryRepository _repository)
        {


            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.FinancialStatementCategoryName;
                lookups.Add(lk);

            }

            CachedData.AddLookUpToCache(lookups, CachedData.CACHE_FSLOANTYPES);

            return lookups;

        }

        public static ICollection<LookUpView> FinancialStatementTransactionTypes(ILKFinancialStatementTransactionCategoryRepository _repository)
        {


            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.FinancialStatementTransactionCategoryName;
                lookups.Add(lk);

            }
            CachedData.AddLookUpToCache(lookups, CachedData.CACHE_TRANSACTIONTYPES);
            return lookups;

        }

        public static ICollection<LookUpView> Currencies(ILKCurrencyRepository _repository, bool UseCurrencyCode = false)
        {


            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = UseCurrencyCode == true ? c.CurrencyCode : c.CurrencyName;
                lookups.Add(lk);

            }
            CachedData.AddLookUpToCache(lookups, CachedData.CACHE_CURRENCYTYPES);
            return lookups;

        }

        public static ICollection<LookUpView> RatedCurrencies(ILKCurrencyRepository _repository)
        {


            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false && (s.isLocal || s.SellingRateWithLocalCurrency != null));
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.CurrencyName;
                lookups.Add(lk);

            }

            return lookups;

        }


        public static ICollection<LookUpView> DebtorTypes(ILKCompanyCategoryRepository _repository)
        {


            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.CompanyCategoryName;
                lookups.Add(lk);

            }
            CachedData.AddLookUpToCache(lookups, CachedData.CACHE_DEBTORTYPES);
            return lookups;

        }

        public static ICollection<LookUpView> CollateralTypes(ILKCollateralCategoryRepository _repository)
        {


            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.CollateralCategoryName;
                lookups.Add(lk);

            }
            CachedData.AddLookUpToCache(lookups, CachedData.CACHE_COLLATERALTYPES);
            return lookups;

        }

        public static ICollection<LookUpView> CollateralSubTypes(ILKCollateralSubTypeCategoryRepository _repository)
        {


            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).OrderBy(s => s.CollateralSubTypeCategoryName);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.CollateralSubTypeCategoryName;
                lookups.Add(lk);

            }
            CachedData.AddLookUpToCache(lookups, CachedData.CACHE_COLLATERALSUBTYPES);
            return lookups;

        }

        public static ICollection<LookUpView> SectorsOfOperation(ILKSectorOfOperationCategoryRepository _repository)
        {


            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).OrderBy(s => s.SectorOfOperationCategoryName); ;
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.SectorOfOperationCategoryName;
                lk.Tags = new { title = c.Description };
                lookups.Add(lk);

            }
            CachedData.AddLookUpToCache(lookups, CachedData.CACHE_SECTOROFOPERATIONS);
            return lookups;

        }

        public static ICollection<LookUpView> FinancialStatementActivityCategoryRepository(ILKFinancialStatementActivityCategoryRepository _repository)
        {


            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.FinancialStatementActivityCategoryName;
                lookups.Add(lk);

            }

            return lookups;

        }
        public static ICollection<LookUpView> LegalEntityClients(IInstitutionRepository _repository, int? MembershipId)
        {


            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            IQueryable<Institution> query;
            if (MembershipId != null)
            {
                query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false && s.Membership.IsActive == true && s.Membership.MembershipTypeId == MembershipCategory.Client && s.MembershipId != MembershipId);

            }
            else
            {
                query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false && s.Membership.IsActive == true && s.Membership.MembershipTypeId == MembershipCategory.Client);

            }
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.MembershipId;
                lk.LkName = c.Name;
                lookups.Add(lk);

            }

            return lookups;

        }
        public static ICollection<LookUpView> IndividualClients(IUserRepository _repository)
        {


            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false && s.InstitutionId == null && s.MembershipId != null && s.Membership.IsActive == true);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.MembershipId;
                lk.LkName = NameHelper.GetFullName(c.FirstName, c.MiddleName, c.Surname);
                lookups.Add(lk);

            }

            return lookups;

        }

        public static ICollection<LookUpView> ServiceFeeCategories(ILKServiceFeeCategoriesRepository _repository)
        {


            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsDeleted == false).OrderBy(s => s.Name);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.Name;
                lookups.Add(lk);
            }
            CachedData.AddLookUpToCache(lookups, CachedData.CACHE_SERVICEFEECATEGORIES);
            return lookups;

        }




        public static ICollection<LookUpView> AuditTypes(ILKAuditCategoryRepository _repository)
        {


            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsDeleted == false).OrderBy(s => s.Name);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.Name;
                lookups.Add(lk);
            }
            CachedData.AddLookUpToCache(lookups, CachedData.CACHE_AUDITTYPES);
            return lookups;

        }

        public static ICollection<AuditActionView> AuditActions(ILKAuditActionRepository _repository)
        {
            ICollection<AuditActionView> lookups = new List<AuditActionView>();

            //Load all audit action types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsDeleted == false).OrderBy(s => s.Name);
            var list = _repository.FindBy(query);

            foreach (var c in list)
            {
                AuditActionView lk = new AuditActionView();
                lk.Id = (int)c.Id;
                lk.Name = c.Name;
                lk.AuditTypeId = (int)c.AuditTypeId;
                lookups.Add(lk);
            }

            return lookups;
        }

        public static ICollection<LookUpView> Users(IUserRepository _repository, int InstitutionId)
        {


            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the institution types
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsDeleted == false && s.InstitutionId == InstitutionId).OrderBy(s => s.Username);
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.Username;
                lookups.Add(lk);
            }

            return lookups;

        }

        public static ICollection<LookUpView> Roles(IRoleRepository _repository)
        {
            ICollection<LookUpView> lookups = new List<LookUpView>();

            //Load all the roles
            var query = _repository.GetNoTrackingDbSet().Where(s => s.IsActive == true && s.IsDeleted == false).OrderBy(s => s.Name); ;
            var list = _repository.FindBy(query);
            foreach (var c in list)
            {
                LookUpView lk = new LookUpView();
                lk.LkValue = (int)c.Id;
                lk.LkName = c.Name;
                lookups.Add(lk);

            }
            return lookups;
        }


    }
}
