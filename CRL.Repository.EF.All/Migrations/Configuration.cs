namespace CRL.Repository.EF.All.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using CRL.Model;
    using CRL.Model.Common;
    using CRL.Model.Common.Enum;
    using CRL.Model.FS;
    using CRL.Model.FS.Enums;
    using CRL.Model.ModelViews;
    using CRL.Model.ModelViews.Enums;
    using CRL.Model.Memberships;


    internal sealed class Configuration : DbMigrationsConfiguration<CRL.Repository.EF.All.CBLContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "CRL.Repository.EF.All.CBLContext";
        }

        protected override void Seed(CRL.Repository.EF.All.CBLContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //SeedLookUps(context);
            //SeedEnterprises(context);
        }

        public void SeedEnterprises(CRL.Repository.EF.All.CBLContext context)
        {
            context.Set<Institution>().AddOrUpdate(
               p => p.Name,
               new Institution
               {
                   Name = "CBL",
                   Address = new Model.Common.AddressInfo { Email = "cbl@gmail.com", Address = "cbl enterprise address" },

                   IsActive = true,
                   IsDeleted = false,
                   CreatedOn = DateTime.Now,
                   CreatedBy = 1,
                   //CompanyTypeId = DebtorCategory.Government,

                   Membership = new Membership { MembershipGroup = 1, MembershipTypeId = MembershipCategory.Owner, MembershipAccountTypeId = MembershipAccountCategory.NonRegular, ClientCode = "10000", PrepaidCreditBalance = 0, CreatedBy = 1, CreatedOn = DateTime.Now, IsActive = true, IsDeleted = false },
                   People = new List<Person>() { new User { FirstName ="testuser", MiddleName ="test",Surname="test",Title ="Mr.",
                                         Address =new AddressInfo{Email="ngaboraa@gmail.com"},Username ="cbladmin",   CreatedBy =1, CreatedOn =DateTime.Now ,IsActive =true,IsDeleted =false}},
               }
               );

        }



        public void SeedLookUps(CRL.Repository.EF.All.CBLContext context)
        {
            context.Set<LKAssetCategory>().AddOrUpdate(
                p => p.AssetCategoryName,
                new LKAssetCategory { Id = AssetCategory.Movable, AssetCategoryName = "Movable" },
                new LKAssetCategory { Id = AssetCategory.Immovable, AssetCategoryName = "Immovable" },
                new LKAssetCategory { Id = AssetCategory.Intangible, AssetCategoryName = "Intangible" });

            context.Set<LKMembershipAccountCategory>().AddOrUpdate(
              p => p.MembershipAccountCategoryName,
              new LKMembershipAccountCategory { Id = MembershipAccountCategory.NonRegular, MembershipAccountCategoryName = "NonRegular" },
              new LKMembershipAccountCategory { Id = MembershipAccountCategory.Regular, MembershipAccountCategoryName = "Regular" },
              new LKMembershipAccountCategory { Id = MembershipAccountCategory.RegularRepresentative, MembershipAccountCategoryName = "RegularRepresentative" });

            context.Set<LKSecuringPartyIndustryCategory>().AddOrUpdate(
               p => p.SecuringPartyIndustryCategoryName,
               new LKSecuringPartyIndustryCategory { SecuringPartyIndustryCategoryName = "Bank", IsActive = true, IsDeleted = false },
               new LKSecuringPartyIndustryCategory { SecuringPartyIndustryCategoryName = "Financial Institution", IsActive = true, IsDeleted = false },
               new LKSecuringPartyIndustryCategory { SecuringPartyIndustryCategoryName = "Housing Finance Company", IsActive = true, IsDeleted = false },
               new LKSecuringPartyIndustryCategory { SecuringPartyIndustryCategoryName = "Non-Banking Finance Company", IsActive = true, IsDeleted = false },
               new LKSecuringPartyIndustryCategory { SecuringPartyIndustryCategoryName = "Intermediaries", IsActive = true, IsDeleted = false },
                new LKSecuringPartyIndustryCategory { SecuringPartyIndustryCategoryName = "Other", IsActive = true, IsDeleted = false });

            context.Set<LKCollateralCategory>().AddOrUpdate(
           p => p.CollateralCategoryName,
           new LKCollateralCategory { Id = CollateralCategory.ConsumerGoods, CollateralCategoryName = "Consumer Goods" },
           new LKCollateralCategory { Id = CollateralCategory.CommercialCollateral, CollateralCategoryName = "Commercial Collateral" },
           new LKCollateralCategory { Id = CollateralCategory.Both, CollateralCategoryName = "Both" }
   );

            //FINANCIAL STATEMENT

            context.Set<LKFinancialStatementLoanCategory>().AddOrUpdate(
              p => p.FinancialStatementCategoryName,
              new LKFinancialStatementLoanCategory { Id = FinancialStatementLoanCategory.Loan, FinancialStatementCategoryName = "Loan" },
              new LKFinancialStatementLoanCategory { Id = FinancialStatementLoanCategory.LineOfCredit, FinancialStatementCategoryName = "Line of Credit" },
              new LKFinancialStatementLoanCategory { Id = FinancialStatementLoanCategory.Both, FinancialStatementCategoryName = "Both" });

      

            context.Set<LKDebtorCategory>().AddOrUpdate(
              p => p.CompanyCategoryName,
              new LKDebtorCategory { Id = DebtorCategory.Sole, CompanyCategoryName = "Sole Proprietorship" },
              new LKDebtorCategory { Id = DebtorCategory.Micro, CompanyCategoryName = "Micro" },
              new LKDebtorCategory { Id = DebtorCategory.SME, CompanyCategoryName = "SME" },
                 new LKDebtorCategory { Id = DebtorCategory.Large, CompanyCategoryName = "Large Firm" },
                    new LKDebtorCategory { Id = DebtorCategory.Government, CompanyCategoryName = "Government" }
                       );



            context.Set<LKFinancialStatementTransactionCategory>().AddOrUpdate(
              p => p.FinancialStatementTransactionCategoryName,
              new LKFinancialStatementTransactionCategory { Id = FinancialStatementTransactionCategory.SecurityInterest, FinancialStatementTransactionCategoryName = "Security Interest" },
              new LKFinancialStatementTransactionCategory { Id = FinancialStatementTransactionCategory.FinancialLease, FinancialStatementTransactionCategoryName = "Finance Lease" },
              new LKFinancialStatementTransactionCategory { Id = FinancialStatementTransactionCategory.Lien, FinancialStatementTransactionCategoryName = "Lien" });



            context.Set<LKFinancialStatementActivityCategory>().AddOrUpdate(
            p => p.FinancialStatementActivityCategoryName,
            new LKFinancialStatementActivityCategory { Id = FinancialStatementActivityCategory.Update, FinancialStatementActivityCategoryName = "Update" },
            new LKFinancialStatementActivityCategory { Id = FinancialStatementActivityCategory.Renewal, FinancialStatementActivityCategoryName = "Renewal" },
            new LKFinancialStatementActivityCategory { Id = FinancialStatementActivityCategory.PartialDischarge, FinancialStatementActivityCategoryName = "Partial Discharge" }
            );

            context.Set<LKCollateralSubTypeCategory>().AddOrUpdate(
    p => p.CollateralSubTypeCategoryName,
    new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Industrial Equipment" },
    new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Construction Equipment" },
    new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Office Equipment" },
    new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Agriculture Equipment" },
    new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Other Equipment" },
    new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Inventory" },
    new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Farm products" },
    new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Motor vehicles" },
    new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Accessions" },
    new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Fixtures" },
    new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Minerals" },
    new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Timber" },
    new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Consumer / household goods" },
    new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Accounts receivable / other rights to payment" },
    new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Deposit Accounts" },
     new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Money" },
      new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Documents of title/Instruments/Chattel paper" },
       new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Intellectual property" },
        new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Investment securities" },
          new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "Other" },
            new LKCollateralSubTypeCategory { CollateralSubTypeCategoryName = "All Assets" }
    );

            context.Set<LKSectorOfOperationCategory>().AddOrUpdate(
 p => p.SectorOfOperationCategoryName,
 new LKSectorOfOperationCategory { SectorOfOperationCategoryName = "Wholesale / Retail trade" },
 new LKSectorOfOperationCategory { SectorOfOperationCategoryName = "Construction" },
 new LKSectorOfOperationCategory { SectorOfOperationCategoryName = "Manufacturing" },
 new LKSectorOfOperationCategory { SectorOfOperationCategoryName = "Transportation" },
 new LKSectorOfOperationCategory { SectorOfOperationCategoryName = "Agriculture" },
 new LKSectorOfOperationCategory { SectorOfOperationCategoryName = "Mining and Oil" },
 new LKSectorOfOperationCategory { SectorOfOperationCategoryName = "Forestry / Wood products" },
 new LKSectorOfOperationCategory { SectorOfOperationCategoryName = "Financial Services" },
 new LKSectorOfOperationCategory { SectorOfOperationCategoryName = "IT" },
 new LKSectorOfOperationCategory { SectorOfOperationCategoryName = "Health Care" },
 new LKSectorOfOperationCategory { SectorOfOperationCategoryName = "Professional services" },
 new LKSectorOfOperationCategory { SectorOfOperationCategoryName = "Government services" },
 new LKSectorOfOperationCategory { SectorOfOperationCategoryName = "Tourism" },
 new LKSectorOfOperationCategory { SectorOfOperationCategoryName = "Entertainment" },
     new LKSectorOfOperationCategory { SectorOfOperationCategoryName = "Other" }
     );
            context.Set<RoleCategory>().AddOrUpdate(
    p => p.RoleCategoryName,
    new RoleCategory {  RoleCategoryName = "Administration" ,  CreatedBy=3, CreatedOn =DateTime.Now },
    new RoleCategory { RoleCategoryName = "FinancialStatement", CreatedBy = 3, CreatedOn = DateTime.Now },
    new RoleCategory { RoleCategoryName = "FincancialStatement Change", CreatedBy = 3, CreatedOn = DateTime.Now },
    new RoleCategory { RoleCategoryName = "Search", CreatedBy = 3, CreatedOn = DateTime.Now },
    new RoleCategory { RoleCategoryName = "Configuration", CreatedBy = 3, CreatedOn = DateTime.Now },
     new RoleCategory { RoleCategoryName = "Menu", CreatedBy = 3, CreatedOn = DateTime.Now }


    );
            RoleCategory c = context.RoleCategories.Where(p => p.RoleCategoryName == "Administration").SingleOrDefault();

            if (c != null)
            {
                context.Set<Role>().AddOrUpdate(
      p => p.Name,
      
      new Role { Name = "Administrator (Owner)", RoleCategoryId = c.Id, MembershipCategoryId = MembershipCategory.Owner, LimitToIndividualOrInstitution = 2, LimitToUnitOrEnterprise = 2, CreatedBy = 3, CreatedOn = DateTime.Now },
      new Role { Name = "Administrator (Client)", RoleCategoryId = c.Id, MembershipCategoryId = MembershipCategory.Client, LimitToIndividualOrInstitution = 2, LimitToUnitOrEnterprise = 2, CreatedBy = 3, CreatedOn = DateTime.Now },
      new Role { Name = "Unit Administrator (Owner)", RoleCategoryId = c.Id, MembershipCategoryId = MembershipCategory.Owner, LimitToIndividualOrInstitution = 2, LimitToUnitOrEnterprise = 2, CreatedBy = 3, CreatedOn = DateTime.Now },
      new Role { Name = "Unit Administrator (Client)", RoleCategoryId = c.Id, MembershipCategoryId = MembershipCategory.Client, LimitToIndividualOrInstitution = 2, LimitToUnitOrEnterprise = 2, CreatedBy = 3, CreatedOn = DateTime.Now }
  );

        }




            context.Set<LKParticipationCategory>().AddOrUpdate(
          p => p.ParticipationCategoryName,
          new LKParticipationCategory { Id = ParticipationCategory.AsSecuredParty, ParticipationCategoryName = "Secured Party" },
          new LKParticipationCategory { Id = ParticipationCategory.AsBorrower, ParticipationCategoryName = "Borrower" }

          );

            context.Set<LKParticipantCategory>().AddOrUpdate(
          p => p.ParticipantCategoryName,
          new LKParticipantCategory { Id = ParticipantCategory.Individual, ParticipantCategoryName = "Individual" },
          new LKParticipantCategory { Id = ParticipantCategory.Insititution, ParticipantCategoryName = "Institution" }

          );

            context.Set<LKMembershipCategory>().AddOrUpdate(
         p => p.MembershipCategoryName,
         new LKMembershipCategory { Id = MembershipCategory.Developer, MembershipCategoryName = "Developer" },
         new LKMembershipCategory { Id = MembershipCategory.Owner, MembershipCategoryName = "Owner" },
         new LKMembershipCategory { Id = MembershipCategory.Client, MembershipCategoryName = "Client" }

         );

            context.Set<LKPersonIdentificationCategory>().AddOrUpdate(
   p => p.PersonIdentificationCardCategoryName,
   new LKPersonIdentificationCategory {  PersonIdentificationCardCategoryName = "Passport" },
   new LKPersonIdentificationCategory {  PersonIdentificationCardCategoryName = "Voter's Identification Card" },
   new LKPersonIdentificationCategory { PersonIdentificationCardCategoryName = "Driver's License" },
   new LKPersonIdentificationCategory { PersonIdentificationCardCategoryName = "Other" }


   );

            int fcount = context.FinancialStatements.Where(fc => fc.IsActive == true && fc.isApprovedOrDenied ==1 && fc.IsDeleted == false).ToList ().Count ;
            int fcount2 = context.FinancialStatementActivities .Where(fc => fc.IsActive == true && fc.IsDeleted == false).ToList().Count;
            
            
            fcount++; fcount2++;
            context.Set<SerialTracker>().AddOrUpdate(
 p => p.Name,
 new SerialTracker { Id = SerialTrackerEnum.Registration, Name  = "Registration",Value =fcount  },
 new SerialTracker { Id = SerialTrackerEnum.FSActivities , Name  = "FSActivities",Value =fcount2  },
 new SerialTracker { Id = SerialTrackerEnum.TempRegistration, Name = "TempRegistration", Value = 1 }
 

 );

            //public enum CollateralCategory { ConsumerGoods = 1, CommercialCollateral = 2, Both = 3 };
            //public enum AssetCategory { Movable = 1, Immovable = 2, Intangible = 3 }
            //public enum FinancialStatementCategory { SecurityInterest = 1, FinancialLease = 2, Lien = 3 }
            //public enum InterestRateCategory { Monthly = 1, Annual = 2 }
            //public enum FinancialStatementActivityCategory { Update = 1, Renewal = 2, Discharge = 3 };
            //public enum ParticipationCategory { AsSecuredParty = 1, AsBorrower = 2 }
            //public enum ParticipantCategory { Individual = 1, Insititution = 2 }
            //public enum MembershipCategory { Owner = 1, Client = 2 }

        }
    }
}
