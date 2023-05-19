using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Common.Enum;

using CRL.Model.FS.Enums;

using CRL.Model.ModelViews;


using CRL.Model.Search;
using CRL.Model.Search.IRepository;
using CRL.Repository.EF.All;
using CRL.Repository.EF.All.Repository.FinancialStatement;
using CRL.Repository.EF.All.Repository.Memberships;
using CRL.Repository.EF.All.Repository.Payments;
using CRL.Repository.EF.All.Repository.Search;

using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Service.Messaging.Reporting.Request;
using CRL.Service.Views.FinancialStatement;
using CRL.Model.FS;
using CRL.Model.FS.IRepository;
using CRL.Infrastructure.Messaging;
using CRL.Model.Memberships;
using CRL.Model.Payments.IRepository;
using CRL.Model.Payments;
using CRL.Model.Memberships.IRepository;
using CRL.Model.ModelViews.Statistics;
using CRL.Infrastructure.Configuration;

namespace CRL.Service.QueryGenerator
{
    public static class StatisticalQueryGenerator
    {
        public static IQueryable<CountOfItemStatView> NoOfAmendmentByYearMonthStat(
IFinancialStatementActivityRepository _rpFS, RequestByDateAndClient request)
        {
            CBLContext ctx = ((FinancialStatementActivityRepository)_rpFS).ctx;
            IQueryable<FinancialStatementActivity> query = _rpFS.GetDbSet().Where(s => s.IsDeleted == false && s.IsActive == true && s.isApprovedOrDenied == 1);

            if (!request.SecurityUser.IsOwnerUser)
            {
                query = query.Where(s => s.MembershipId == request.SecurityUser.MembershipId);


            }
            else
            {
                if (request.ClientId != null && request.ClientId != 0)
                {
                    query = query.Where(s => (ctx.Institutions.Where(o => o.Id == request.ClientId).Select(p => p.MembershipId).FirstOrDefault() == (s.MembershipId)));

                }
            }
            if (request.TransactionDate != null)
            {
                query = query.Where(s => s.CreatedOn >= request.TransactionDate.StartDate && s.CreatedOn < request.TransactionDate.EndDate);
            }

            var query2 = query.Select(s => new CountOfItemStatView
            {
                Year = s.CreatedOn.Year,
                MonthNum = s.CreatedOn.Month,
                CountOfItem = 1


            }
      );

            query2 = query2.GroupBy(cm => new { cm.Year, cm.MonthNum }).Select(y => new CountOfItemStatView()
            {
                Year = y.Key.Year,
                MonthNum = y.Key.MonthNum,
                CountOfItem = y.Count()

            }
             );

            return query2;

        }


        public static IQueryable<CountOfItemStatView> NoOfFSByYearMonthOnlyStat(
 IFinancialStatementRepository _rpFS, RequestByDate request)
        {
            CBLContext ctx = ((FinancialStatementRepository)_rpFS).ctx;
            IQueryable<Model.FS.FinancialStatement> query = _rpFS.GetDbSet().Where(s => s.IsDeleted == false && s.ClonedId == null && s.IsActive == true && s.isApprovedOrDenied == 1 && s.AfterUpdateFinancialStatementId == null);
            if (!request.SecurityUser.IsOwnerUser)
            {
                query = query.Where(s => s.MembershipId == request.SecurityUser.MembershipId);
            }

            if (request.TransactionDate != null)
            {
                query = query.Where(s => s.CreatedOn >= request.TransactionDate.StartDate && s.CreatedOn < request.TransactionDate.EndDate);
            }
            var query2 = query.Select(s => new CountOfItemStatView
            {
                Year = s.CreatedOn.Year,
                MonthNum = s.CreatedOn.Month,

                CountOfItem = 1


            }
      );

            query2 = query2.GroupBy(cm => new { cm.Year, cm.MonthNum }).Select(y => new CountOfItemStatView()
            {
                Year = y.Key.Year,
                MonthNum = y.Key.MonthNum,
                CountOfItem = y.Count()

            }
             );

            return query2;

        }

        public static IQueryable<CountOfItemStatView> NoOfDebtorStat(IParticipantRepository _rpFS, ViewFSStatRequest request)
        {
            CBLContext ctx = ((ParticipantRepository)_rpFS).ctx;

            IQueryable<Model.FS.Participant> query = _rpFS.GetDbSet().Where(s => s.FinancialStatement.IsDeleted == false && s.FinancialStatement.ClonedId == null && s.FinancialStatement.IsActive == true && s.FinancialStatement.isApprovedOrDenied == 1 && s.FinancialStatement.AfterUpdateFinancialStatementId == null && s.ParticipationTypeId == ParticipationCategory.AsBorrower);
            if (!request.SecurityUser.IsOwnerUser)
            {
                query = query.Where(s => s.FinancialStatement.MembershipId == request.SecurityUser.MembershipId);
            }

            if (request.LimitToWomenOwned)
            {
                query = query.Where(s => s.FinancialStatement.FinancialStatementTransactionTypeId != FinancialStatementTransactionCategory.Lien &&
                    (s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && d.IsActive == true && d.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().MajorityFemaleOrMaleOrBoth == 2 ||
                    s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && d.IsActive == true && d.IsDeleted == false).OfType<IndividualParticipant>().FirstOrDefault().Gender == "Female"));
            }

            if (request.RegistrationDate != null)
            {
                query = query.Where(s => s.FinancialStatement.RegistrationDate >= request.RegistrationDate.StartDate && s.FinancialStatement.RegistrationDate < request.RegistrationDate.EndDate);
            }

            if (request.FSState == 2)
            {
                //Limit to only active non expired and non discharged
                query = query.Where(s => s.FinancialStatement.IsDischarged == false && s.FinancialStatement.ExpiryDate > DateTime.Now);

            }

            if (request.FSPeriod != 3)
            {
                //if (request.FSPeriod == 1)
                //{

                //    query = query.Where(s => s.FinancialStatement.IsPriorFS == false);
                //}
                //else if (request.FSPeriod == 2)
                //{ query = query.Where(s => s.FinancialStatement.IsPriorFS == true); }

            }
            IQueryable<CountOfItemStatView> query2 = null;
            if (request.GroupBy == GroupByNoOfFSStat.ByBorrowerType)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.FinancialStatement.RegistrationDate.Year,
                    MonthNum = s.FinancialStatement.RegistrationDate.Month,
                    //GroupingColumn = s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ? "Individual" :
                    //s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().DebtorType.CompanyCategoryName,
                    GroupingColumn = s.ParticipantTypeId == ParticipantCategory.Individual ? "Individual" : ctx.Participants.Where(m => m.Id == s.Id).OfType<InstitutionParticipant>().FirstOrDefault().DebtorType.CompanyCategoryName,
                    CountOfItem = 1,

                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByLenderType)
            {

                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.FinancialStatement.RegistrationDate.Year,
                    MonthNum = s.FinancialStatement.RegistrationDate.Month,
                    //GroupingColumn = s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ? "Individual" :
                    //s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().SecuringPartyIndustryType.SecuringPartyIndustryCategoryName,
                    //GroupingColumn = s.ParticipantTypeId == ParticipantCategory.Individual ? "Individual" : ctx.Participants.Where(m => m.FinancialStatementId == s.FinancialStatementId).OfType<InstitutionParticipant>().FirstOrDefault().SecuringPartyIndustryType.SecuringPartyIndustryCategoryName,
                    GroupingColumn =
                        (
                        s.FinancialStatement.Membership.isIndividualOrLegalEntity == 1 ? "Individual" :
                        s.FinancialStatement.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.FinancialStatement.MembershipId).FirstOrDefault().SecuringPartyType.SecuringPartyIndustryCategoryName : "Unknown"
                        ),
                    CountOfItem = 1,


                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByOwner)
            {

                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.FinancialStatement.RegistrationDate.Year,
                    MonthNum = s.FinancialStatement.RegistrationDate.Month,
                    GroupingColumn =
                         (
                         s.FinancialStatement.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.FinancialStatement.MembershipId && a.InstitutionId == null).Select(r => r.FirstName + " " + r.MiddleName ?? "" + " " + r.Surname).FirstOrDefault() :
                         s.FinancialStatement.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.FinancialStatement.MembershipId).Select(r => r.Name).FirstOrDefault() : "Unknown"
                         ),
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByOwnerType)
            {

                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.FinancialStatement.RegistrationDate.Year,
                    MonthNum = s.FinancialStatement.RegistrationDate.Month,
                    GroupingColumn =
                         (
                         s.FinancialStatement.Membership.isIndividualOrLegalEntity == 1 ? "Individual" :
                         s.FinancialStatement.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.FinancialStatement.MembershipId).FirstOrDefault().SecuringPartyType.SecuringPartyIndustryCategoryName : "Unknown"

                         ),
                    CountOfItem = 1,


                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByCurrency)
            {

                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.FinancialStatement.RegistrationDate.Year,
                    MonthNum = s.FinancialStatement.RegistrationDate.Month,
                    GroupingColumn = s.FinancialStatement.MaximumAmountSecuredCurrency.CurrencyCode,
                    CountOfItem = 1,


                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByDebtorCounty)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.FinancialStatement.RegistrationDate.Year,
                    MonthNum = s.FinancialStatement.RegistrationDate.Month,
                    GroupingColumn = s.County != null ? s.County.Name : "N/A",
                    CountOfItem = 1,


                });



            }

            else if (request.GroupBy == GroupByNoOfFSStat.BySecuredPartyCounty)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.FinancialStatement.RegistrationDate.Year,
                    MonthNum = s.FinancialStatement.RegistrationDate.Month,
                    GroupingColumn = s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().County.Name,
                    CountOfItem = 1,


                });


            }

            query2 = query2.GroupBy(cm => new { cm.Year, cm.MonthNum, cm.GroupingColumn }).Select(y => new CountOfItemStatView()
            {
                Year = y.Key.Year,
                MonthNum = y.Key.MonthNum,
                GroupingColumn = y.Key.GroupingColumn,
                CountOfItem = y.Count()

            }
              );
            query2 = query2.OrderBy(s => s.GroupingColumn);
            return query2;

        }

        public static IQueryable<CountOfItemStatView> TotalNoOfFinancingStatement(
IFinancialStatementRepository _rpFS, RequestBase request)
        {

            CBLContext ctx = ((FinancialStatementRepository)_rpFS).ctx;
            IQueryable<Model.FS.FinancialStatement> query = _rpFS.GetDbSet().Where(s => s.IsDeleted == false && s.ClonedId == null && s.IsActive == true && s.isApprovedOrDenied == 1 && s.AfterUpdateFinancialStatementId == null);
            if (!request.SecurityUser.IsOwnerUser)
            {
                query = query.Where(s => s.MembershipId == request.SecurityUser.MembershipId);
            }

            var query2 = query.Select(s => new CountOfItemStatView
           {

               CountOfItem = 1

           });

            return query2;



        }


        public static IQueryable<CountOfItemStatView> NoOfSearchesByYearMonthOnlyStat(
ISearchFinancialStatementRepository _rpFS, RequestByDate request)
        {
            CBLContext ctx = ((SearchFinancialStatementRepository)_rpFS).ctx;
            IQueryable<SearchFinancialStatement> query = _rpFS.GetDbSet().Where(s => s.IsDeleted == false && s.IsActive == true);
            if (!request.SecurityUser.IsOwnerUser)
            {
                query = query.Where(s => s.CreatedByUser.MembershipId == request.SecurityUser.MembershipId);
            }
            if (request.TransactionDate != null)
            {
                query = query.Where(s => s.CreatedOn >= request.TransactionDate.StartDate && s.CreatedOn < request.TransactionDate.EndDate);
            }
            var query2 = query.Select(s => new CountOfItemStatView
            {
                Year = s.CreatedOn.Year,
                MonthNum = s.CreatedOn.Month,
                CountOfItem = 1


            }
      );

            query2 = query2.GroupBy(cm => new { cm.Year, cm.MonthNum }).Select(y => new CountOfItemStatView()
            {
                Year = y.Key.Year,
                MonthNum = y.Key.MonthNum,
                CountOfItem = y.Count()

            }
             );




            return query2;

        }



        public static IQueryable<CountOfItemStatView> NoOfFSStat(IFinancialStatementRepository _rpFS, ViewFSStatRequest request)
        {
            CBLContext ctx = ((FinancialStatementRepository)_rpFS).ctx;
            IQueryable<Model.FS.FinancialStatement> query = _rpFS.GetDbSet().Where(s => s.IsDeleted == false && s.ClonedId == null && s.IsActive == true && s.isApprovedOrDenied == 1);
            if (request.FSStateType == 1)
            {
                query = query.Where(s => s.AfterUpdateFinancialStatementId == null);
            }
            else
            {
                query = query.Where(s => !(ctx.FinancialStatements.Where(p => p.RegistrationNo == s.RegistrationNo && p.AfterUpdateFinancialStatementId != null).Select(c => c.AfterUpdateFinancialStatementId).Contains(s.Id)));

            }
            if (!request.SecurityUser.IsOwnerUser)
            {
                query = query.Where(s => s.MembershipId == request.SecurityUser.MembershipId);
            }
            else
            {
                if (request.ClientId != null && request.ClientId != 0)
                {
                    query = query.Where(s => (ctx.Institutions.Where(o => o.Id == request.ClientId).Select(p => p.MembershipId).FirstOrDefault() == (s.MembershipId)));

                }
            }

            if (request.LimitToWomenOwned)
            {
                query = query.Where(s => (s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && d.IsActive == true && d.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().MajorityFemaleOrMaleOrBoth == 2 ||
                    s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && d.IsActive == true && d.IsDeleted == false).OfType<IndividualParticipant>().FirstOrDefault().Gender == "Female"));
            }

            if (request.RegistrationDate != null)
            {
                query = query.Where(s => s.RegistrationDate >= request.RegistrationDate.StartDate && s.RegistrationDate < request.RegistrationDate.EndDate);
            }

            if (request.FSState == 2)
            {
                //Limit to only active non expired and non discharged
                query = query.Where(s => s.IsDischarged == false && s.ExpiryDate > DateTime.Now);

            }
            IQueryable<CountOfItemStatView> query2 = null;
            if (request.GroupBy == GroupByNoOfFSStat.ByBorrowerType)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    GroupingColumn = s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ? "Individual" :
                    s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().DebtorType.CompanyCategoryName,
                    CountOfItem = 1,


                });
            }

            else if (request.GroupBy == GroupByNoOfFSStat.ByCurrency)
            {
                query2 = query.Select(s => new CountOfItemStatView
                    {
                        Year = s.RegistrationDate.Year,
                        MonthNum = s.RegistrationDate.Month,
                        GroupingColumn = s.MaximumAmountSecuredCurrency.CurrencyName,
                        CountOfItem = 1,
                    });
            }

            else if (request.GroupBy == GroupByNoOfFSStat.ByLenderType)
            {

                query2 = query.Select(s => new CountOfItemStatView
               {
                   Year = s.RegistrationDate.Year,
                   MonthNum = s.RegistrationDate.Month,
                   GroupingColumn = s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ? "Individual" :
                   s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().SecuringPartyIndustryType.SecuringPartyIndustryCategoryName,
                   CountOfItem = 1,


               });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByOwner)
            {

                query2 = query.Select(s => new CountOfItemStatView
              {
                  Year = s.RegistrationDate.Year,
                  MonthNum = s.RegistrationDate.Month,
                  GroupingColumn =
                       (
                       s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId && a.InstitutionId == null).Select(r => r.FirstName + " " + r.MiddleName ?? "" + " " + r.Surname).FirstOrDefault() :
                       s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() : "Unknown"

                       ),
                  CountOfItem = 1,


              });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByOwnerType)
            {

                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    GroupingColumn =
                         (
                         s.Membership.isIndividualOrLegalEntity == 1 ? "Individual" :
                         s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).FirstOrDefault().SecuringPartyType.SecuringPartyIndustryCategoryName : "Unknown"

                         ),
                    CountOfItem = 1,


                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByTransactionType)
            {

                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    GroupingColumn = s.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName,
                    CountOfItem = 1,


                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByDebtorCounty)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    GroupingColumn = s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().County.Name,
                    CountOfItem = 1,


                });



            }

            else if (request.GroupBy == GroupByNoOfFSStat.BySecuredPartyCounty)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    GroupingColumn = s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().County.Name,
                    CountOfItem = 1,


                });


            }

            query2 = query2.GroupBy(cm => new { cm.Year, cm.MonthNum, cm.GroupingColumn }).Select(y => new CountOfItemStatView()
            {
                Year = y.Key.Year,
                MonthNum = y.Key.MonthNum,
                GroupingColumn = y.Key.GroupingColumn,
                CountOfItem = y.Count()

            }
              );
            query2 = query2.OrderBy(s => s.GroupingColumn);
            return query2;

        }

        public static IQueryable<ValueOfFSStatView> ValueOfFSStat(
 IFinancialStatementRepository _rpFS, ViewFSStatRequest request)
        {
            CBLContext ctx = ((FinancialStatementRepository)_rpFS).ctx;
            IQueryable<Model.FS.FinancialStatement> query = _rpFS.GetDbSet().Where(s => s.IsDeleted == false && s.ClonedId == null && s.IsActive == true && s.isApprovedOrDenied == 1);
            var sum = query.Sum(s => s.MaximumAmountSecured);
            if (request.FSStateType == 1)
            {
                query = query.Where(s => s.AfterUpdateFinancialStatementId == null);
            }
            else
            {
                query = query.Where(s => !(ctx.FinancialStatements.Where(p => p.RegistrationNo == s.RegistrationNo && p.AfterUpdateFinancialStatementId != null).Select(c => c.AfterUpdateFinancialStatementId).Contains(s.Id)));

            }
            if (!request.SecurityUser.IsOwnerUser)
            {
                query = query.Where(s => s.MembershipId == request.SecurityUser.MembershipId);
            }
            else
            {
                if (request.ClientId != null && request.ClientId != 0)
                {
                    query = query.Where(s => (ctx.Institutions.Where(o => o.Id == request.ClientId).Select(p => p.MembershipId).FirstOrDefault() == (s.MembershipId)));

                }
            }

            if (request.LimitToWomenOwned)
            {
                query = query.Where(s => (s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && d.IsActive == true && d.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().MajorityFemaleOrMaleOrBoth == 2 ||
                    s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && d.IsActive == true && d.IsDeleted == false).OfType<IndividualParticipant>().FirstOrDefault().Gender == "Female"));
            }

            if (request.RegistrationDate != null)
            {
                query = query.Where(s => s.RegistrationDate >= request.RegistrationDate.StartDate && s.RegistrationDate < request.RegistrationDate.EndDate);
            }

            if (request.FSState == 2)
            {
                //Limit to only active non expired and non discharged
                query = query.Where(s => s.IsDischarged == false && s.ExpiryDate > DateTime.Now);

            }
            IQueryable<ValueOfFSStatView> query2 = null;
            if (request.GroupBy == GroupByNoOfFSStat.ByBorrowerType)
            {
                query2 = query.Select(s => new ValueOfFSStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    GroupingColumn = s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ? "Individual" :
                    s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().DebtorType.CompanyCategoryName,
                    Currency = s.MaximumAmountSecuredCurrency.CurrencyCode,
                    SumOfLoanAmount = (decimal)s.MaximumAmountSecured


                }

          );
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByLenderType)
            {

                query2 = query.Select(s => new ValueOfFSStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    GroupingColumn = s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ? "Individual" :
                    s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().SecuringPartyIndustryType.SecuringPartyIndustryCategoryName,
                    Currency = s.MaximumAmountSecuredCurrency.CurrencyCode,
                    SumOfLoanAmount = (decimal)s.MaximumAmountSecured


                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByOwner)
            {

                query2 = query.Select(s => new ValueOfFSStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    GroupingColumn =
                         (
                         s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId && a.InstitutionId == null).Select(r => r.FirstName + " " + r.MiddleName ?? "" + " " + r.Surname).FirstOrDefault() :
                         s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() : "Unknown"

                         ),
                    Currency = s.MaximumAmountSecuredCurrency.CurrencyCode,
                    SumOfLoanAmount = (decimal)s.MaximumAmountSecured


                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByOwnerType)
            {

                query2 = query.Select(s => new ValueOfFSStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    GroupingColumn =
                         (
                         s.Membership.isIndividualOrLegalEntity == 1 ? "Individual" :
                         s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).FirstOrDefault().SecuringPartyType.SecuringPartyIndustryCategoryName : "Unknown"

                         ),
                    Currency = s.MaximumAmountSecuredCurrency.CurrencyCode,
                    SumOfLoanAmount = (decimal)s.MaximumAmountSecured


                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByTransactionType)
            {

                query2 = query.Select(s => new ValueOfFSStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    GroupingColumn = s.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName,
                    Currency = s.MaximumAmountSecuredCurrency.CurrencyCode,
                    SumOfLoanAmount = (decimal)s.MaximumAmountSecured


                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByDebtorCounty)
            {
                query2 = query.Select(s => new ValueOfFSStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    GroupingColumn = s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().County.Name,
                    Currency = s.MaximumAmountSecuredCurrency.CurrencyCode,
                    SumOfLoanAmount = (decimal)s.MaximumAmountSecured
                });
            }

            else if (request.GroupBy == GroupByNoOfFSStat.BySecuredPartyCounty)
            {
                query2 = query.Select(s => new ValueOfFSStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    GroupingColumn = s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().County.Name,
                    Currency = s.MaximumAmountSecuredCurrency.CurrencyCode,
                    SumOfLoanAmount = (decimal)s.MaximumAmountSecured
                });
            }

            else if (request.GroupBy == GroupByNoOfFSStat.ByCurrency)
            {
                query2 = query.Select(s => new ValueOfFSStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    GroupingColumn = s.MaximumAmountSecuredCurrency.CurrencyName,
                    Currency = s.MaximumAmountSecuredCurrency.CurrencyCode,
                    SumOfLoanAmount = (decimal)s.MaximumAmountSecured


                });


            }


            query2 = query2.GroupBy(cm => new { cm.Year, cm.MonthNum, cm.GroupingColumn, cm.Currency }).Select(y => new ValueOfFSStatView()
            {
                Year = y.Key.Year,
                MonthNum = y.Key.MonthNum,
                GroupingColumn = y.Key.GroupingColumn,
                Currency = y.Key.Currency,
                SumOfLoanAmount = y.Sum(mo => mo.SumOfLoanAmount)

            }
              );
            query2 = query2.OrderBy(s => s.GroupingColumn);
            return query2;

        }



        public static IQueryable<CountOfItemStatView> NoOfLendersStat(IInstitutionRepository _rpFS, ViewStatRequest request)
        {
            //CBLContext ctx = ((MembershipRepository)_rpFS).ctx;
            //IQueryable<Membership> query = _rpFS.GetDbSet().Where(s => s.IsActive == true && s.IsDeleted == false && s.Id != 1);

            CBLContext ctx = ((InstitutionRepository)_rpFS).ctx;
            IQueryable<Institution> query = _rpFS.GetDbSet().Where(s => s.Id != 1);
            //IQueryable<Institution> query = ctx.Institutions.AsNoTracking();

            if (!(request.Status == null))
            {
                switch (request.Status)
                {
                    case 0:
                        query = query.Where(s => s.Membership.IsActive && !s.Membership.IsDeleted && s.IsActive && !s.IsDeleted);
                        break;
                    case 1:
                        query = query.Where(s => s.Membership.IsActive && !s.Membership.IsDeleted && !s.IsActive && !s.IsDeleted);
                        break;
                    case 2:
                        query = query.Where(s => s.Membership.IsActive && !s.Membership.IsDeleted && (s.IsActive || !s.IsActive) && s.IsDeleted);
                        break;
                    case 3:
                        query = query.Where(s => !s.Membership.IsActive && !s.Membership.IsDeleted && s.IsActive && !s.IsDeleted);
                        break;
                    case 4:
                        query = query.Where(s => s.Membership.IsActive && !s.Membership.IsDeleted && s.IsActive && !s.IsDeleted || (!s.IsActive && !s.IsDeleted));
                        break;
                    case 5:
                        query = query.Where(s => !s.Membership.IsActive && s.Membership.IsDeleted && s.IsActive && s.IsDeleted);
                        break;
                }

            }

            if (request.RegistrationDate != null)
            {
                query = query.Where(s => s.CreatedOn >= request.RegistrationDate.StartDate && s.CreatedOn < request.RegistrationDate.EndDate);
            }

            #region Group By
            IQueryable<CountOfItemStatView> query2 = null;
            if (request.GroupBy == GroupByNoOfFSStat.ByLenderType)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = ctx.Institutions.Where(m => m.MembershipId == s.Id).FirstOrDefault().SecuringPartyType.SecuringPartyIndustryCategoryName == null ? "Unknown/NA" : ctx.Institutions.Where(m => m.MembershipId == s.Id).FirstOrDefault().SecuringPartyType.SecuringPartyIndustryCategoryName,
                    CountOfItem = 1
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.BySecuredPartyCounty)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = ctx.Institutions.Where(m => m.MembershipId == s.Id).FirstOrDefault().County.Name == null ? "Unknown/NA" : ctx.Institutions.Where(m => m.MembershipId == s.Id).FirstOrDefault().County.Name,
                    CountOfItem = 1,
                });
            }

            #endregion



            query2 = query2.GroupBy(cm => new { cm.Year, cm.MonthNum, cm.GroupingColumn }).Select(y => new CountOfItemStatView()
            {
                Year = y.Key.Year,
                MonthNum = y.Key.MonthNum,
                GroupingColumn = y.Key.GroupingColumn,
                CountOfItem = y.Count()

            });
            query2 = query2.OrderBy(s => s.GroupingColumn);
            return query2;

        }


        public static IQueryable<CountOfItemStatView> NoOfDischargeStat(IFinancialStatementActivityRepository _rpFS, ViewStatRequest request)
        {
            CBLContext ctx = ((FinancialStatementActivityRepository)_rpFS).ctx;
            IQueryable<FinancialStatementActivity> query = _rpFS.GetDbSet().Where(s => s.IsDeleted == false && s.IsActive == true && s.isApprovedOrDenied == 1 && (new[] { FinancialStatementActivityCategory.PartialDischarge, FinancialStatementActivityCategory.FullDicharge }.ToList().Contains(s.FinancialStatementActivityTypeId)));
            if (!request.SecurityUser.IsOwnerUser)
            {
                query = query.Where(s => s.MembershipId == request.SecurityUser.MembershipId);
            }

            if (request.RegistrationDate != null)
            {
                query = query.Where(s => s.FinancialStatement.RegistrationDate >= request.RegistrationDate.StartDate && s.FinancialStatement.RegistrationDate < request.RegistrationDate.EndDate);
            }

            #region Group By
            IQueryable<CountOfItemStatView> query2 = null;
            if (request.GroupBy == GroupByNoOfFSStat.ByBorrowerType)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ? "Individual" : s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().DebtorType.CompanyCategoryName,
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByLenderType)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().SecuringPartyIndustryType.SecuringPartyIndustryCategoryName,
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByOwner)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId && a.InstitutionId == null).Select(r => r.FirstName + " " + r.MiddleName ?? "" + " " + r.Surname).FirstOrDefault() : s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() : "Unknown",
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByOwnerType)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = s.Membership.isIndividualOrLegalEntity == 1 ? "Individual" : s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).FirstOrDefault().SecuringPartyType.SecuringPartyIndustryCategoryName : "Unknown",
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByTransactionType)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = s.FinancialStatement.FinancialStatementTransactionTypeId == null ? "Unknown" : s.FinancialStatement.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName,
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByDebtorCounty)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().County.Name,
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.BySecuredPartyCounty)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().County.Name,
                    CountOfItem = 1,
                });
            }
            #endregion

            query2 = query2.GroupBy(cm => new { cm.Year, cm.MonthNum, cm.GroupingColumn }).Select(y => new CountOfItemStatView()
            {
                Year = y.Key.Year,
                MonthNum = y.Key.MonthNum,
                GroupingColumn = y.Key.GroupingColumn,
                CountOfItem = y.Count()

            });
            //query2 = query2.OrderBy(s => s.GroupingColumn);
            return query2;

        }

        public static IQueryable<CountOfItemStatView> NoOfCollateralsStat(ICollateralRepository _rpFS, ViewStatRequest request)
        {
            CBLContext ctx = ((CollateralRepository)_rpFS).ctx;
            IQueryable<Collateral> query = _rpFS.GetDbSet().Where(s => s.FinancialStatement.isApprovedOrDenied == 1 && s.FinancialStatement.IsActive == true && s.FinancialStatement.ClonedId == null && s.FinancialStatement.AfterUpdateFinancialStatementId == null);

            if (request.RegistrationDate != null)
            {
                query = query.Where(s => s.FinancialStatement.RegistrationDate >= request.RegistrationDate.StartDate && s.FinancialStatement.RegistrationDate < request.RegistrationDate.EndDate);
            }

            if (!request.SecurityUser.IsOwnerUser)
            {
                query = query.Where(s => s.FinancialStatement.MembershipId == request.SecurityUser.MembershipId);
            }
            else
            {
                if (request.ClientId != null && request.ClientId != 0)
                {
                    query = query.Where(s => (ctx.Institutions.Where(o => o.Id == request.ClientId).Select(p => p.MembershipId).FirstOrDefault() == (s.FinancialStatement.MembershipId)));

                }
            }

            #region Group By
            IQueryable<CountOfItemStatView> query2 = null;
            if (request.GroupBy == null)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.FinancialStatement.RegistrationDate.Year,
                    MonthNum = s.FinancialStatement.RegistrationDate.Month,
                    GroupingColumn = s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ? "Individual" : s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().DebtorTypeId == null ? "Unknown/NA" : s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && d.ParticipantTypeId == ParticipantCategory.Insititution && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().DebtorType.CompanyCategoryName,
                    PrimaryGroup = s.CollateralSubTypeType.CollateralSubTypeCategoryName,
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByBorrowerType)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.FinancialStatement.RegistrationDate.Year,
                    MonthNum = s.FinancialStatement.RegistrationDate.Month,
                    GroupingColumn = s.CollateralSubTypeType.CollateralSubTypeCategoryName,
                    PrimaryGroup = s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ? "Individual" : s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().DebtorTypeId == null ? "Unknown/NA" : s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && d.ParticipantTypeId == ParticipantCategory.Insititution && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().DebtorType.CompanyCategoryName,
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByLenderType)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.FinancialStatement.RegistrationDate.Year,
                    MonthNum = s.FinancialStatement.RegistrationDate.Month,
                    GroupingColumn = s.CollateralSubTypeType.CollateralSubTypeCategoryName,
                    PrimaryGroup = s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().SecuringPartyIndustryTypeId == null ? "Unknown/NA" : s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().SecuringPartyIndustryType.SecuringPartyIndustryCategoryName,
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByOwner)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.FinancialStatement.RegistrationDate.Year,
                    MonthNum = s.FinancialStatement.RegistrationDate.Month,
                    GroupingColumn = s.CollateralSubTypeType.CollateralSubTypeCategoryName,
                    PrimaryGroup = s.FinancialStatement.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.FinancialStatement.MembershipId).Select(r => r.Name).FirstOrDefault() : "Unknown",
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByOwnerType)
            {

                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.FinancialStatement.RegistrationDate.Year,
                    MonthNum = s.FinancialStatement.RegistrationDate.Month,
                    GroupingColumn = s.CollateralSubTypeType.CollateralSubTypeCategoryName,
                    PrimaryGroup = ctx.Institutions.Where(a => a.MembershipId == s.FinancialStatement.MembershipId).FirstOrDefault().SecuringPartyTypeId == null ? "Unknown/NA" : ctx.Institutions.Where(a => a.MembershipId == s.FinancialStatement.MembershipId).FirstOrDefault().SecuringPartyType.SecuringPartyIndustryCategoryName,
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByDebtorCounty)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.FinancialStatement.RegistrationDate.Year,
                    MonthNum = s.FinancialStatement.RegistrationDate.Month,
                    GroupingColumn = s.CollateralSubTypeType.CollateralSubTypeCategoryName,
                    PrimaryGroup = s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().CountyId == null ? "Unknown/NA" : s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().County.Name,
                    CountOfItem = 1,
                });
            }

            else if (request.GroupBy == GroupByNoOfFSStat.BySecuredPartyCounty)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.FinancialStatement.RegistrationDate.Year,
                    MonthNum = s.FinancialStatement.RegistrationDate.Month,
                    GroupingColumn = s.CollateralSubTypeType.CollateralSubTypeCategoryName,
                    PrimaryGroup = s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().CountyId == null ? "Unknown/NA" : s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().County.Name,
                    CountOfItem = 1,
                });
            }
            #endregion



            query2 = query2.GroupBy(cm => new { cm.Year, cm.MonthNum, cm.GroupingColumn, cm.PrimaryGroup }).Select(y => new CountOfItemStatView()
            {
                Year = y.Key.Year,
                MonthNum = y.Key.MonthNum,
                GroupingColumn = y.Key.GroupingColumn,
                PrimaryGroup = y.Key.PrimaryGroup,
                CountOfItem = y.Count()
            });
            query2 = query2.OrderBy(s => s.PrimaryGroup);
            return query2;

        }

        public static IQueryable<CountOfItemStatView> NoOfAmendmentStat(IFinancialStatementActivityRepository _rpFS, ViewStatRequest request)
        {
            CBLContext ctx = ((FinancialStatementActivityRepository)_rpFS).ctx;
            IQueryable<FinancialStatementActivity> query = request.ReportId != 15 ? _rpFS.GetDbSet().Where(s => s.IsDeleted == false && s.IsActive == true && s.isApprovedOrDenied == 1 && new[] { FinancialStatementActivityCategory.Update, FinancialStatementActivityCategory.FullAssignment, FinancialStatementActivityCategory.Subordination }.ToList().Contains(s.FinancialStatementActivityTypeId)) : _rpFS.GetDbSet().Where(s => s.IsDeleted == false && s.IsActive == true && s.isApprovedOrDenied == 1);

            if (!request.SecurityUser.IsOwnerUser)
            {
                query = query.Where(s => s.MembershipId == request.SecurityUser.MembershipId);
            }
            else
            {
                if (request.ClientId != null && request.ClientId != 0)
                {
                    query = query.Where(s => (ctx.Institutions.Where(o => o.Id == request.ClientId).Select(p => p.MembershipId).FirstOrDefault() == (s.MembershipId)));

                }
            }
            if (request.RegistrationDate != null)
            {
                query = query.Where(s => s.CreatedOn >= request.RegistrationDate.StartDate && s.CreatedOn < request.RegistrationDate.EndDate);
            }

            //var query2 = query.Select(s => new CountOfItemStatView
            //{
            //    Year = s.CreatedOn.Year,
            //    MonthNum = s.CreatedOn.Month,
            //    GroupingColumn = s.FinancialStatementActivityType.FinancialStatementActivityCategoryName,

            //    CountOfItem = 1


            //}
            //);
            #region Group By
            IQueryable<CountOfItemStatView> query2 = null;

            if (request.GroupBy == GroupByNoOfFSStat.ByBorrowerType)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = request.ReportId != 15 ? s.FinancialStatementActivityType.FinancialStatementActivityCategoryName : s.FinancialStatementActivityTypeId == null ? "Unknown" : s.FinancialStatementActivityType.FinancialStatementActivityCategoryName,
                    PrimaryGroup = s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ? "Individual" : s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().DebtorType.CompanyCategoryName,
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByLenderType)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = request.ReportId != 15 ? s.FinancialStatementActivityType.FinancialStatementActivityCategoryName : s.FinancialStatementActivityTypeId == null ? "Unknown" : s.FinancialStatementActivityType.FinancialStatementActivityCategoryName,
                    PrimaryGroup = s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().SecuringPartyIndustryType.SecuringPartyIndustryCategoryName,
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByOwner)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = request.ReportId != 15 ? s.FinancialStatementActivityType.FinancialStatementActivityCategoryName : s.FinancialStatementActivityTypeId == null ? "Unknown" : s.FinancialStatementActivityType.FinancialStatementActivityCategoryName,
                    PrimaryGroup = s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() : "Unknown",
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByOwnerType)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = request.ReportId != 15 ? s.FinancialStatementActivityType.FinancialStatementActivityCategoryName : s.FinancialStatementActivityTypeId == null ? "Unknown" : s.FinancialStatementActivityType.FinancialStatementActivityCategoryName,
                    PrimaryGroup = s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).FirstOrDefault().SecuringPartyType.SecuringPartyIndustryCategoryName : "Unknown",
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByDebtorCounty)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = request.ReportId != 15 ? s.FinancialStatementActivityType.FinancialStatementActivityCategoryName : s.FinancialStatementActivityTypeId == null ? "Unknown" : s.FinancialStatementActivityType.FinancialStatementActivityCategoryName,
                    PrimaryGroup = s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().County.Name,
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.BySecuredPartyCounty)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = request.ReportId != 15 ? s.FinancialStatementActivityType.FinancialStatementActivityCategoryName : s.FinancialStatementActivityTypeId == null ? "Unknown" : s.FinancialStatementActivityType.FinancialStatementActivityCategoryName,
                    PrimaryGroup = s.FinancialStatement.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().County.Name,
                    CountOfItem = 1,
                });
            }
            #endregion

            query2 = query2.GroupBy(cm => new { cm.Year, cm.MonthNum, cm.GroupingColumn, cm.PrimaryGroup }).Select(y => new CountOfItemStatView()
            {
                Year = y.Key.Year,
                MonthNum = y.Key.MonthNum,
                GroupingColumn = y.Key.GroupingColumn,
                PrimaryGroup = y.Key.PrimaryGroup,
                CountOfItem = y.Count()
            });

            return query2;

        }


        public static IQueryable<CountOfItemStatView> NoOfSearchesStat(ISearchFinancialStatementRepository _rpFS, ViewStatRequest request)
        {
            CBLContext ctx = ((SearchFinancialStatementRepository)_rpFS).ctx;
            IQueryable<SearchFinancialStatement> query = _rpFS.GetDbSet().Where(s => s.IsDeleted == false && s.IsActive == true);
            if (!request.SecurityUser.IsOwnerUser)
            {
                query = query.Where(s => s.CreatedByUser.MembershipId == request.SecurityUser.MembershipId);
            }
            else
            {
                if (request.ClientId != null && request.ClientId != 0)
                {
                    query = query.Where(s => (ctx.Institutions.Where(o => o.Id == request.ClientId).Select(p => p.MembershipId).FirstOrDefault() == (s.CreatedByUser.MembershipId)));

                }
            }
            if (request.RegistrationDate != null)
            {
                query = query.Where(s => s.CreatedOn >= request.RegistrationDate.StartDate && s.CreatedOn < request.RegistrationDate.EndDate);
            }

            #region Group By
            IQueryable<CountOfItemStatView> query2 = null;
            if (request.GroupBy == null)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = s.CreatedBy == Constants.PublicUser ? "Public User" : s.CreatedByUser.MembershipId == 1 ? "Registry User" : "Secured Creditor",
                    CountOfItem = 1
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByLenderType)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = s.CreatedBy == Constants.PublicUser ? "Public User" : s.CreatedByUser.MembershipId == 1 ? "Registry User" : ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().SecuringPartyTypeId == null ? "Unknown/NA" : ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().SecuringPartyType.SecuringPartyIndustryCategoryName,
                    CountOfItem = 1
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByOwner)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = s.CreatedBy == Constants.PublicUser ? "Public User" : new[] { " ", null, "" }.ToList().Contains(ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().Name) ? "UNKNOWN CLIENT" : ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().Name,
                    CountOfItem = 1,
                });
            }
            #endregion

            //query2 = query.Select(s => new CountOfItemStatView
            //{
            //    Year = s.CreatedOn.Year,
            //    MonthNum = s.CreatedOn.Month,
            //    GroupingColumn = s.CreatedBy == 1 ? "Public User" :
            //    s.CreatedByUser.Membership.isIndividualOrLegalEntity == 2 ? "Financial Institution" : "Individual",
            //    CountOfItem = 1
            //});

            query2 = query2.GroupBy(cm => new { cm.Year, cm.MonthNum, cm.GroupingColumn }).Select(y => new CountOfItemStatView()
            {
                Year = y.Key.Year,
                MonthNum = y.Key.MonthNum,
                GroupingColumn = y.Key.GroupingColumn,
                CountOfItem = y.Count()
            });

            return query2;

        }

        public static IQueryable<CountOfItemStatView> NoOfServiceStat(IAccountTransactionRepository _rpFS, ViewStatRequest request)
        {
            CBLContext ctx = ((AccountTransactionRepository)_rpFS).ctx;
            IQueryable<AccountTransaction> query = _rpFS.GetDbSet().Where(s => s.IsDeleted == false && s.IsActive == true && s.ServiceFeeType != null);

            if (!request.SecurityUser.IsOwnerUser)
            {
                query = query.Where(s => s.MembershipId == request.SecurityUser.MembershipId);
            }
            if (request.RegistrationDate != null)
            {
                query = query.Where(s => s.CreatedOn >= request.RegistrationDate.StartDate && s.CreatedOn < request.RegistrationDate.EndDate);
            }
            #region Group By
            IQueryable<CountOfItemStatView> query2 = null;
            if (request.GroupBy == GroupByNoOfFSStat.ByLenderType)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    PrimaryGroup = s.ServiceFeeType.Name,
                    GroupingColumn = s.CreatedBy == Constants.PublicUser ? "Public user" : s.CreatedByUser.MembershipId == 1 || ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().SecuringPartyTypeId == null ? "Unknown/NA" : ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().SecuringPartyType.SecuringPartyIndustryCategoryName,
                    CountOfItem = 1
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.BySecuredPartyCounty)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    PrimaryGroup = s.ServiceFeeType.Name,
                    GroupingColumn = s.CreatedBy == Constants.PublicUser ? "Public User" : s.CreatedByUser.MembershipId == 1 || ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().CountyId == null ? "Unknown/NA" : ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().County.Name,
                    CountOfItem = 1,
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByOwner)
            {
                query2 = query.Select(s => new CountOfItemStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    PrimaryGroup = s.ServiceFeeType.Name,
                    GroupingColumn = s.CreatedBy == Constants.PublicUser ? "Public User" : new[] { " ", null, "" }.ToList().Contains(ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().Name) ? "UNKNOWN SECURED CREDITOR" : ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().Name,
                    CountOfItem = 1,
                });
            }
            #endregion

            query2 = query2.GroupBy(cm => new { cm.Year, cm.MonthNum, cm.PrimaryGroup, cm.GroupingColumn }).Select(y => new CountOfItemStatView()
            {
                Year = y.Key.Year,
                MonthNum = y.Key.MonthNum,
                PrimaryGroup = y.Key.PrimaryGroup,
                GroupingColumn = y.Key.GroupingColumn,
                CountOfItem = y.Count()
            });

            return query2;
        }


        public static IQueryable<ValueOfFeeStatView> ValueOfFeesStatView(IAccountTransactionRepository _rpFS, ViewStatRequest request)
        {
            CBLContext ctx = ((AccountTransactionRepository)_rpFS).ctx;
            IQueryable<AccountTransaction> query = _rpFS.GetDbSet().Where(s => s.IsDeleted == false && s.IsActive == true && s.AccountTypeTransactionId == AccountTransactionCategory.CreditTransaction
                );
            if (!request.SecurityUser.IsOwnerUser)
            {
                query = query.Where(s => s.MembershipId == request.SecurityUser.MembershipId);
            }
            else
            {
                if (request.ClientId != null && request.ClientId != 0)
                {
                    query = query.Where(s => (ctx.Institutions.Where(o => o.Id == request.ClientId).Select(p => p.MembershipId).FirstOrDefault() == (s.MembershipId)));

                }
            }
            if (request.RegistrationDate != null)
            {
                query = query.Where(s => s.CreatedOn >= request.RegistrationDate.StartDate && s.CreatedOn < request.RegistrationDate.EndDate);
            }

            #region Group By
            IQueryable<ValueOfFeeStatView> query2 = null;
            if (request.GroupBy == null)
            {
                query2 = query.Select(s => new ValueOfFeeStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    GroupingColumn = s.ServiceFeeType.Name,
                    SumOfLoanAmount = (decimal)s.Amount
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByLenderType)
            {
                query2 = query.Select(s => new ValueOfFeeStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    PrimaryGroup = s.ServiceFeeType.Name,
                    GroupingColumn = s.CreatedBy == Constants.PublicUser ? "Public User" : s.CreatedByUser.MembershipId == 1 || ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().SecuringPartyTypeId == null ? "Unknown/NA" : ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().SecuringPartyType.SecuringPartyIndustryCategoryName,
                    SumOfLoanAmount = (decimal)s.Amount
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.BySecuredPartyCounty)
            {
                query2 = query.Select(s => new ValueOfFeeStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    PrimaryGroup = s.ServiceFeeType.Name,
                    GroupingColumn = s.CreatedBy == Constants.PublicUser || s.CreatedByUser.MembershipId == 1 || ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().CountyId == null ? "Unknown/NA" : ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().County.Name,
                    SumOfLoanAmount = (decimal)s.Amount
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByOwner)
            {
                query2 = query.Select(s => new ValueOfFeeStatView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    PrimaryGroup = s.ServiceFeeType.Name,
                    GroupingColumn = s.CreatedBy == Constants.PublicUser ? "Public User" : new[] { " ", null, "" }.ToList().Contains(ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().Name) ? "UNKNOWN SECURED CREDITOR" : ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().Name,
                    SumOfLoanAmount = (decimal)s.Amount
                });
            }
            #endregion

            query2 = query2.GroupBy(cm => new { cm.Year, cm.MonthNum, cm.PrimaryGroup, cm.GroupingColumn }).Select(y => new ValueOfFeeStatView()
            {
                Year = y.Key.Year,
                MonthNum = y.Key.MonthNum,
                PrimaryGroup = y.Key.PrimaryGroup,
                GroupingColumn = y.Key.GroupingColumn,
                SumOfLoanAmount = y.Sum(mo => mo.SumOfLoanAmount)

            });

            return query2;
        }


        public static IQueryable<FSCustomQueryStatView> CreateQueryForFSCustomQuery(
         ViewFSCustomQueryRequest request, IFinancialStatementRepository _rpFS, bool DoCount)
        {
            CBLContext ctx = ((FinancialStatementRepository)_rpFS).ctx;
            IQueryable<Model.FS.FinancialStatement> query = _rpFS.GetDbSet().Where(s => s.IsDeleted == false && s.ClonedId == null && s.isApprovedOrDenied == 1 && s.IsActive == true && s.IsDischarged == false);
            if (request.FSStateType == 1)
            {
                query = query.Where(s => s.AfterUpdateFinancialStatementId == null);
            }
            else
            {
                query = query.Where(s => !(ctx.FinancialStatements.Where(p => p.RegistrationNo == s.RegistrationNo && p.AfterUpdateFinancialStatementId != null).Select(c => c.AfterUpdateFinancialStatementId).Contains(s.Id)));

            }
            if (request.FinancialStatementTransactionTypeId != null && request.FinancialStatementTransactionTypeId.Count() > 0)
            {
                query = query.Where(s => request.FinancialStatementTransactionTypeId.Contains(s.FinancialStatementTransactionTypeId));
            }
            if (request.FinancialStatementLoanTypeId != null && request.FinancialStatementLoanTypeId.Count() > 0)
            {
                query = query.Where(s => request.FinancialStatementLoanTypeId.Contains(s.FinancialStatementLoanTypeId));
            }
            if (request.MaximumCurrency != null && request.MaximumCurrency.Count() > 0)
            {
                query = query.Where(s => request.MaximumCurrency.Contains(s.MaximumAmountSecuredCurrencyId));
            }
            if (request.CollateralTypeId != null && request.CollateralTypeId.Count() > 0)
            {
                query = query.Where(s => request.CollateralTypeId.Contains(s.CollateralTypeId));
            }
            if (request.CollateralSubTypeId != null && request.CollateralSubTypeId.Count() > 0)
            {
                query = query.Where(s => s.Collaterals.Any(p => request.CollateralSubTypeId.Contains(p.CollateralSubTypeId))); //Let's use first if elsa agrres for only first to be listed
            }

            if (request.DebtorTypeId != null && request.DebtorTypeId.Count() > 0)
            {
                if (!request.LimitToFirstDebtor)
                {

                    if (request.DebtorTypeId.Contains(DebtorCategory.Individual))
                    {
                        query = query.Where(s =>
                            s.Participants.OfType<IndividualParticipant>().Any(p => p.ParticipationTypeId == ParticipationCategory.AsBorrower && p.IsActive == true && p.IsDeleted == false)
                            || s.Participants.OfType<InstitutionParticipant>().Any(o => request.DebtorTypeId.Contains(o.DebtorTypeId) && o.IsActive == true && o.IsDeleted == false));
                    }
                    else
                    {
                        query = query.Where(s => s.IsActive == true && s.IsDeleted == false && s.Participants.OfType<InstitutionParticipant>().Any(o => request.DebtorTypeId.Contains(o.DebtorTypeId)));
                    }
                }
                else
                {

                    if (request.DebtorTypeId.Contains(DebtorCategory.Individual))
                    {
                        query = query.Where(s =>
                            (s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && d.IsActive == true && d.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ||
                           (s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && d.IsActive == true && d.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Insititution
                        && request.DebtorTypeId.Contains(s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && d.IsActive == true && d.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().DebtorTypeId))));
                    }
                    else
                    {
                        query = query.Where(s => request.DebtorTypeId.Contains(s.Participants.Where(o => o.ParticipationTypeId == ParticipationCategory.AsBorrower && o.IsActive == true && o.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().DebtorTypeId));
                    }

                }

            }
            if (request.DebtorCountryId != null && request.DebtorCountryId.Count() > 0)
            {
                if (!request.LimitToFirstDebtor)
                {

                    query = query.Where(s => s.Participants.Any(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower && request.DebtorCountryId.Contains(o.CountryId)));
                }
                else
                {
                    query = query.Where(s => request.DebtorCountryId.Contains(s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).FirstOrDefault().CountryId));
                }

            }
            if (request.MajorityOwnershipId != null && request.MajorityOwnershipId != 0)
            {


                if (!request.LimitToFirstDebtor)
                {
                    query = query.Where(s => s.Participants.OfType<IndividualParticipant>().Any(p => p.IsActive == true && p.IsDeleted == false && p.ParticipationTypeId == ParticipationCategory.AsBorrower && (request.MajorityOwnershipId == 1 || request.MajorityOwnershipId == 2) && p.Gender == (request.MajorityOwnershipId == 1 ? "Male" : "Female"))
                         || s.Participants.OfType<InstitutionParticipant>().Any(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower && o.MajorityFemaleOrMaleOrBoth == request.MajorityOwnershipId)
                         );
                }
                else
                {


                    query = query.Where(s => s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).OfType<InstitutionParticipant>().FirstOrDefault().MajorityFemaleOrMaleOrBoth == request.MajorityOwnershipId
                        || ((request.MajorityOwnershipId == 1 || request.MajorityOwnershipId == 2) && s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).OfType<IndividualParticipant>().FirstOrDefault().Gender == (request.MajorityOwnershipId == 1 ? "Male" : "Female")));
                }
            }
            if (request.ExistingRelationshipId != null)
            {


                if (!request.LimitToFirstDebtor)
                {

                    query = query.Where(s => s.FinancialStatementTransactionTypeId != FinancialStatementTransactionCategory.Lien && s.Participants.Any(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower && o.DebtorIsAlreadyClientOfSecuredParty == (request.ExistingRelationshipId == 1 ? true : false)));
                }
                else
                {
                    query = query.Where(s => s.FinancialStatementTransactionTypeId != FinancialStatementTransactionCategory.Lien && s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).FirstOrDefault().DebtorIsAlreadyClientOfSecuredParty == (request.ExistingRelationshipId == 1 ? true : false));
                }
            }
            if (request.DebtorNationalityId != null && request.DebtorNationalityId.Count() > 0)
            {


                if (!request.LimitToFirstDebtor)
                {

                    query = query.Where(s => s.Participants.Any(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower && request.DebtorNationalityId.Contains(o.NationalityId)));
                }
                else
                {
                    query = query.Where(s => request.DebtorNationalityId.Contains(s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).FirstOrDefault().NationalityId));
                }
            }
            if (request.SectorOfOperationId != null && request.SectorOfOperationId.Count() > 0)
            {


                if (!request.LimitToFirstDebtor)
                {

                    query = query.Where(s => s.Participants.Any(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower && o.SectorOfOperationTypes.Any(m => request.SectorOfOperationId.Contains(m.Id))));
                }
                else
                {
                    query = query.Where(s => s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).FirstOrDefault().SectorOfOperationTypes.Any(m => request.SectorOfOperationId.Contains(m.Id)));
                }

            }
            if (request.SecuredPartyTypeId != null && request.SecuredPartyTypeId.Count() > 0)
            {
                if (!request.LimitToFirstSecuredParty)
                {

                    if (request.SecuredPartyTypeId.Contains(0))
                    {
                        query = query.Where(s => s.Participants.OfType<IndividualParticipant>().Any(p => p.IsActive == true && p.IsDeleted == false && p.ParticipationTypeId == ParticipationCategory.AsSecuredParty)
                          || s.Participants.OfType<InstitutionParticipant>().Any(o => o.IsActive == true && o.IsDeleted == false && request.SecuredPartyTypeId.Contains(o.SecuringPartyIndustryTypeId)));
                    }
                    else
                    {
                        query = query.Where(s => s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsSecuredParty).OfType<InstitutionParticipant>().Any(o => request.SecuredPartyTypeId.Contains(o.SecuringPartyIndustryTypeId)));
                    }
                }
                else
                {

                    if (request.SecuredPartyTypeId.Contains(0))
                    {
                        query = query.Where(s => s.Participants.Where(d => d.IsActive == true && d.IsDeleted == false && d.ParticipationTypeId == ParticipationCategory.AsSecuredParty).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ||
                           (s.Participants.Where(d => d.IsActive == true && d.IsDeleted == false && d.ParticipationTypeId == ParticipationCategory.AsSecuredParty).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Insititution
                        && request.SecuredPartyTypeId.Contains(s.Participants.Where(d => d.IsActive == true && d.IsDeleted == false && d.ParticipationTypeId == ParticipationCategory.AsSecuredParty).OfType<InstitutionParticipant>().FirstOrDefault().SecuringPartyIndustryTypeId)));
                    }
                    else
                    {
                        query = query.Where(s => request.DebtorTypeId.Contains(s.Participants.Where(o => o.IsActive == true && o.IsDeleted == false && o.ParticipationTypeId == ParticipationCategory.AsBorrower).OfType<InstitutionParticipant>().FirstOrDefault().DebtorTypeId));
                    }

                }


            }


            if (request.SecurityUser.IsOwnerUser == false)
            {
                query = query.Where(s => s.MembershipId == request.SecurityUser.MembershipId);
            }
            else
            {

                if (request.ClientId != null && request.ClientId != 0)
                {
                    query = query.Where(s => (ctx.Institutions.Where(o => o.Id == request.ClientId).Select(p => p.MembershipId).FirstOrDefault() == (s.MembershipId)));

                }
            }

            if (request.FSState == 2)
            {
                //Limit to only active non expired and non discharged
                query = query.Where(s => s.IsDischarged == false && s.ExpiryDate > DateTime.Now);

            }

            IQueryable<FSCustomQueryStatView> query3 = null;

            if (request.GroupBy == GroupByNoOfFSStat.ByBorrowerType)
            {
                query3 = query.Select(s => new FSCustomQueryStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    TransactionType = s.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName,
                    LoanType = s.FinancialStatementLoanType.FinancialStatementCategoryName,
                    GroupingColumn = s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ? "Individual" :
                    s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().DebtorType.CompanyCategoryName,
                    Currency = s.MaximumAmountSecuredCurrency.CurrencyCode,
                    SumOfLoanAmount = (decimal)s.MaximumAmountSecured,
                    CountOfItem = 1


                }


          );
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByLenderType)
            {

                query3 = query.Select(s => new FSCustomQueryStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    TransactionType = s.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName,
                    LoanType = s.FinancialStatementLoanType.FinancialStatementCategoryName,
                    GroupingColumn = s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().ParticipantTypeId == ParticipantCategory.Individual ? "Individual" :
                    s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).OfType<InstitutionParticipant>().FirstOrDefault().SecuringPartyIndustryType.SecuringPartyIndustryCategoryName,
                    Currency = s.MaximumAmountSecuredCurrency.CurrencyCode,
                    SumOfLoanAmount = (decimal)s.MaximumAmountSecured,
                    CountOfItem = 1


                }
              );
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByDebtorCounty)
            {

                query3 = query.Select(s => new FSCustomQueryStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    TransactionType = s.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName,
                    LoanType = s.FinancialStatementLoanType.FinancialStatementCategoryName,
                    GroupingColumn = s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsBorrower && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().County.Name,
                    Currency = s.MaximumAmountSecuredCurrency.CurrencyCode,
                    SumOfLoanAmount = (decimal)s.MaximumAmountSecured,
                    CountOfItem = 1


                }
              );
            }
            else if (request.GroupBy == GroupByNoOfFSStat.BySecuredPartyCounty)
            {

                query3 = query.Select(s => new FSCustomQueryStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    TransactionType = s.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName,
                    LoanType = s.FinancialStatementLoanType.FinancialStatementCategoryName,
                    GroupingColumn = s.Participants.Where(d => d.ParticipationTypeId == ParticipationCategory.AsSecuredParty && s.IsActive == true && s.IsDeleted == false).FirstOrDefault().County.Name,
                    Currency = s.MaximumAmountSecuredCurrency.CurrencyCode,
                    SumOfLoanAmount = (decimal)s.MaximumAmountSecured,
                    CountOfItem = 1


                }
              );
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByOwner)
            {

                query3 = query.Select(s => new FSCustomQueryStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    TransactionType = s.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName,
                    LoanType = s.FinancialStatementLoanType.FinancialStatementCategoryName,
                    GroupingColumn =
                      (
                      s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId && a.InstitutionId == null).Select(r => r.FirstName + " " + r.MiddleName ?? "" + " " + r.Surname).FirstOrDefault() :
                      s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() : "Unknown"

                      ),

                    Currency = s.MaximumAmountSecuredCurrency.CurrencyCode,
                    SumOfLoanAmount = (decimal)s.MaximumAmountSecured,
                    CountOfItem = 1


                }
              );
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByOwnerType)
            {

                query3 = query.Select(s => new FSCustomQueryStatView
                {
                    Year = s.RegistrationDate.Year,
                    MonthNum = s.RegistrationDate.Month,
                    TransactionType = s.FinancialStatementTransactionType.FinancialStatementTransactionCategoryName,
                    LoanType = s.FinancialStatementLoanType.FinancialStatementCategoryName,
                    GroupingColumn =
                     (
                     s.Membership.isIndividualOrLegalEntity == 1 ? "Individual" :
                     s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).FirstOrDefault().SecuringPartyType.SecuringPartyIndustryCategoryName : "Unknown"

                     ),
                    Currency = s.MaximumAmountSecuredCurrency.CurrencyCode,
                    SumOfLoanAmount = (decimal)s.MaximumAmountSecured,
                    CountOfItem = 1


                }
              );
            }


            query3 = query3.GroupBy(cm => new { cm.Year, cm.MonthNum, cm.TransactionType, cm.LoanType, cm.GroupingColumn, cm.Currency }).Select(y => new FSCustomQueryStatView()
            {
                Year = y.Key.Year,
                MonthNum = y.Key.MonthNum,
                TransactionType = y.Key.TransactionType,
                LoanType = y.Key.LoanType,
                GroupingColumn = y.Key.GroupingColumn,
                Currency = y.Key.Currency,
                SumOfLoanAmount = y.Sum(mo => mo.SumOfLoanAmount),
                CountOfItem = y.Count()

            }
            ).OrderBy(s => s.Year).OrderBy(s => s.MonthNum);
            return query3;


        }


    }
}
