using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews;
using CRL.Repository.EF.All;
using CRL.Repository.EF.All.Repository.Payments;
using CRL.Service.Messaging.Payments.Request;
using CRL.Service.Views.Payments;
using CRL.Infrastructure.Configuration;
using System.Data.Entity;
using CRL.Model.Messaging;
using CRL.Model.ModelViews.Payments;
using CRL.Model.Payments;
using CRL.Model.Payments.IRepository;
using CRL.Model.Memberships;

namespace CRL.Service.QueryGenerator
{



    public static class PaymentQueryGenerator
    {
        public static IQueryable<AccountReconcilation> ViewAccountReconcilationQuery(ViewReconcilationsRequest request,
       IAccountTransactionRepository _accountTransactionRepository, bool DoCount = false)
        {
            CBLContext ctx = ((AccountTransactionRepository)_accountTransactionRepository).ctx;
            IQueryable<AccountReconcilation> query = ctx.AccountReconcilations;

            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "CreatedOn";
                    request.SortOrder = "desc";
                }

                if (request.SortColumn == "CreatedOn")
                {
                    if (request.SortOrder == "desc")
                    {
                        query = query.OrderByDescending(s => s.CreatedOn);
                    }
                    else
                    {
                        query = query.OrderBy(s => s.CreatedOn);
                    }
                }



            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query = query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }


            return query;

        }

        public static IQueryable<AccountBatch> ViewAccountBatchesQuery(ViewAccountBatchesRequest request,
        IAccountTransactionRepository _accountTransactionRepository, bool DoCount = false)
        {
            CBLContext ctx = ((AccountTransactionRepository)_accountTransactionRepository).ctx;

            IQueryable<AccountBatch> query = ctx.AccountBatches;

            if (!request.SecurityUser.IsOwnerUser)
            {
                query = query.Where(s => s.InstitutionId == request.SecurityUser.InstitutionId);
            }

            if (request.TransactionDate != null)
            {
                query = query.Where(s => s.CreatedOn >= request.TransactionDate.StartDate && s.CreatedOn < request.TransactionDate.EndDate);
            }

            if (request.Id != null)
            {
                query = query.Where(s => s.Id == request.Id);
            }

            if (request.BatchDate != null)
            {
                query = query.Where(s => DbFunctions.DiffDays(s.CreatedOn, request.BatchDate) == 0);
            }



            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "CreatedOn";
                    request.SortOrder = "desc";
                }

                if (request.SortColumn == "CreatedOn")
                {
                    if (request.SortOrder == "desc")
                    {
                        query = query.OrderByDescending(s => s.CreatedOn);
                    }
                    else
                    {
                        query = query.OrderBy(s => s.CreatedOn);
                    }
                }



            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query = query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }


            return query;

        }

        public static IQueryable<ClientSettlementSummaryView> CreateQueryForSettledClientsInBatchView(GetReconcileRequest request,
       IAccountTransactionRepository _accountTransactionRepository, bool DoCount = false)
        {

            CBLContext ctx = ((AccountTransactionRepository)_accountTransactionRepository).ctx;
            IQueryable<AccountTransaction> query = _accountTransactionRepository.GetDbSet().Where(s => s.AccountSourceTypeId == AccountSourceCategory.Postpaid && s.AccountReconcileId != null && s.AccountTypeTransactionId == AccountTransactionCategory.SettlementPayment
                );
            if (request.BatchId != null)
            {
                query = query.Where(s => s.AccountBatchId == request.BatchId);
            }


            var query2 = query.Select(s => new ClientSettlementSummaryView
            {


                Client = ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault(),
                Amount = s.Amount,
                ReconcilationNumber = (int)s.AccountReconcileId,
                SettledDate = s.CreatedOn


            });



            return query2;

        }
        public static IQueryable<ClientAmountSelectionView> CreateQueryForSelectBankPostpaidTransactions(GetReconcileRequest request,
        IAccountTransactionRepository _accountTransactionRepository, bool DoCount = false)
        {

            CBLContext ctx = ((AccountTransactionRepository)_accountTransactionRepository).ctx;
            IQueryable<AccountTransaction> query = _accountTransactionRepository.GetDbSet().Where(s => s.AccountSourceTypeId == AccountSourceCategory.Postpaid && s.AccountReconcileId == null);
            if (request.BatchId != null)
            {
                query = query.Where(s => s.AccountBatchId == request.BatchId);
            }


            var query2 = query.Select(s => new ClientAmountSelectionView
            {


                Client = (s.PostPaidRepresentativeMembership == null && s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() :
                  s.PostPaidRepresentativeMembership == null && s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId && a.InstitutionId == null).Select(r => r.FirstName + r.MiddleName + r.Surname).FirstOrDefault() :
                  ctx.Institutions.Where(a => a.MembershipId == s.PostPaidRepresentativeMembershipId).Select(r => r.Name).FirstOrDefault()),
                MembershipId = (s.PostPaidRepresentativeMembershipId == null ? (int)s.MembershipId : (int)s.PostPaidRepresentativeMembershipId),
                Amount = s.Amount,
                isSelected = true

            });

            query2 = query2.GroupBy(cm => new { RepresentativeClientName = cm.Client, RepresentativeMembershipId = cm.MembershipId }).Select(y => new ClientAmountSelectionView()
            {
                Client = y.Key.RepresentativeClientName,
                MembershipId = y.Key.RepresentativeMembershipId,
                Amount = y.Sum(p => p.Amount),
                isSelected = true

            }
                   );

            return query2;

        }
        public static IQueryable<ExpenditureByTransactionView> CreateQueryForClientExpenditureByTransaction(ViewClientExpenditureByTransactionRequest request,
           IAccountTransactionRepository _accountTransactionRepository, bool DoCount = false)
        {
            CBLContext ctx = ((AccountTransactionRepository)_accountTransactionRepository).ctx;
            IQueryable<AccountTransaction> query = _accountTransactionRepository.GetDbSet().Where(s => s.AccountTypeTransactionId == AccountTransactionCategory.CreditTransaction);

            if (request.SecurityUser.IsOwnerUser == false)
            {

                query = query.Where(m => m.MembershipId == request.SecurityUser.MembershipId);

            }
            else if (request.LimitToInstitutionId != null)
            {
                query = query.Where(m => m.MembershipId == request.LimitToInstitutionId);
            }
            bool LimitToDateRange = request.TransactionDate != null;

            if (LimitToDateRange)
            {
                query = query.Where(s => s.CreatedOn >= request.TransactionDate.StartDate && s.CreatedOn < request.TransactionDate.EndDate);
            }

            var query2 = query.Select(s => new ExpenditureByTransactionView
            {
                ServiceFeeType = s.ServiceFeeType.Name ?? "N/A",
                Fee = s.Amount,
                Quantity = 1,
                Amount = s.Amount



            });

            query2 = query2.GroupBy(cm => new { cm.ServiceFeeType, cm.Fee }).Select(y => new ExpenditureByTransactionView()
            {

                ServiceFeeType = y.Key.ServiceFeeType,
                Fee = y.Key.Fee,
                Quantity = y.Sum(p => p.Quantity),
                Amount = y.Sum(p => p.Amount)

            }
                );

            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "ServiceFeeType";
                    request.SortOrder = "asc";
                }

                if (request.SortColumn == "ServiceFeeType")
                {
                    if (request.SortOrder == "asc")
                    {
                        query2 = query2.OrderByDescending(s => s.ServiceFeeType);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.ServiceFeeType);
                    }
                }



            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }


            return query2;



        }
        public static IQueryable<SummarisedCreditActivityByRepresentativeBank> CreateQueryForSummarisedBankPostpaidTransactions(ViewSummaryPostpaidTransactionsRequest request,
            IAccountTransactionRepository _accountTransactionRepository, bool DoCount = false)
        {
            CBLContext ctx = ((AccountTransactionRepository)_accountTransactionRepository).ctx;
            IQueryable<AccountTransaction> query = _accountTransactionRepository.GetDbSet().Where(s => s.AccountSourceTypeId == AccountSourceCategory.Postpaid && s.AccountTypeTransactionId == AccountTransactionCategory.CreditTransaction);

            if (request.SecurityUser.IsOwnerUser == false)
            {
                if (request.LimitTo != null)
                {
                    if (request.LimitTo == 2)
                    {
                        query = query.Where(m => m.MembershipId == request.SecurityUser.MembershipId || m.PostPaidRepresentativeMembershipId == request.SecurityUser.MembershipId);
                    }
                    else if (request.LimitTo == 3)
                    {
                        query = query.Where(m => m.PostPaidRepresentativeMembershipId == request.SecurityUser.MembershipId);
                    }
                    else
                    {
                        query = query.Where(m => m.MembershipId == request.SecurityUser.MembershipId);
                    }
                }
                else
                {
                    query = query.Where(m => m.MembershipId == request.SecurityUser.MembershipId);
                }

            }


            if (request.BatchTypeId != null)
            {
                if (request.BatchTypeId == 1)
                {
                    query = query.Where(s => s.AccountBatchId == null);
                }
                else if (request.BatchTypeId == 2)
                {
                    query = query.Where(s => s.AccountBatchId != null);
                }
            }

            bool LimitToDateRange = request.TransactionDate != null;

            if (LimitToDateRange)
            {
                query = query.Where(s => s.CreatedOn >= request.TransactionDate.StartDate && s.CreatedOn < request.TransactionDate.EndDate);
            }

            if (request.BatchId != null)
            {

                query = query.Where(s => s.AccountBatchId == request.BatchId);
            }

            if (!(request.SettlementType == null))
            {
                if (request.SettlementType == 1)
                    query = query.Where(s => s.AccountReconcileId == null);
                else if (request.SettlementType == 2)
                    query = query.Where(s => s.AccountReconcileId != null);

            }
            else
            {
                query = query.Where(s => s.AccountReconcileId == null);
            }

            var query2 = query.Select(s => new SummarisedCreditActivityByRepresentativeBank
            {


                Client = (s.PostPaidRepresentativeMembership == null && s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() :
                s.PostPaidRepresentativeMembership == null && s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId && a.InstitutionId == null).Select(r => r.FirstName + " " + (r.MiddleName + " ") ?? "" + r.Surname).FirstOrDefault() :
                ctx.Institutions.Where(a => a.MembershipId == s.PostPaidRepresentativeMembershipId).Select(r => r.Name).FirstOrDefault()

               ),

                Amount = s.Amount

            });

            query2 = query2.GroupBy(cm => new { RepresentativeClientName = cm.Client }).Select(y => new SummarisedCreditActivityByRepresentativeBank()
            {

                Client = y.Key.RepresentativeClientName,
                Amount = y.Sum(p => p.Amount)

            }
                   );


            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "RepresentativeClientName";
                    request.SortOrder = "asc";
                }

                if (request.SortColumn == "RepresentativeClientName")
                {
                    if (request.SortOrder == "asc")
                    {
                        query2 = query2.OrderByDescending(s => s.Client);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Client);
                    }
                }



            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }


            return query2;


        }

        public static IQueryable<SummarisedClientExpenditureView> CreateQueryForSummarisedClientExpenditureView(ViewSummaryClientExpenditureRequest request, IAccountTransactionRepository _accountTransactionRepository, bool DoCount = false)
        {
            CBLContext ctx = ((AccountTransactionRepository)_accountTransactionRepository).ctx;
            IQueryable<AccountTransaction> query = _accountTransactionRepository.GetDbSet().Where(s => s.AccountTypeTransactionId == AccountTransactionCategory.CreditTransaction);

            bool LimitToDateRange = request.TransactionDate != null;

            if (LimitToDateRange)
            {
                query = query.Where(s => s.CreatedOn >= request.TransactionDate.StartDate && s.CreatedOn < request.TransactionDate.EndDate);
            }


            IQueryable<SummarisedClientExpenditureView> query2 = null;
            if (request.GroupBy == null)
            {
                query2 = query.Select(s => new SummarisedClientExpenditureView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    ClientType = s.ServiceFeeType.Name,
                    ClientName = s.Membership == null ? "Public User" : s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId && a.InstitutionId == null).Select(r => (r.FirstName + " " + r.MiddleName).TrimEnd() + " " + r.Surname).FirstOrDefault() : ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault(),
                    Amount = s.Amount
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.ByLenderType)
            {
                query2 = query.Select(s => new SummarisedClientExpenditureView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    ClientType = s.CreatedBy == Constants.PublicUser ? "Public User" : ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().SecuringPartyType.SecuringPartyIndustryCategoryName,
                    ClientName = s.Membership == null ? "Public User" : ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault(),
                    Amount = s.Amount
                });
            }
            else if (request.GroupBy == GroupByNoOfFSStat.BySecuredPartyCounty)
            {
                query2 = query.Select(s => new SummarisedClientExpenditureView
                {
                    Year = s.CreatedOn.Year,
                    MonthNum = s.CreatedOn.Month,
                    ClientType = s.CreatedBy == Constants.PublicUser ? "Unknown/NA" : ctx.Institutions.Where(m => m.MembershipId == s.CreatedByUser.MembershipId).FirstOrDefault().County.Name,
                    ClientName = s.Membership == null ? "Public User" : s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId && a.InstitutionId == null).Select(r => (r.FirstName + " " + r.MiddleName).TrimEnd() + " " + r.Surname).FirstOrDefault() : ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault(),
                    Amount = s.Amount
                });
            }
            //query2 = query.Select(s => new SummarisedClientExpenditureView
            //{
            //    Year = s.CreatedOn.Year,
            //    MonthNum = s.CreatedOn.Month,
            //    ClientType = s.Membership == null ? "Public User" : s.Membership.isIndividualOrLegalEntity == 1 ? "Individual" : ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.SecuringPartyType.SecuringPartyIndustryCategoryName).FirstOrDefault(),
            //    ClientName = (
            //                      s.Membership == null ? "Public User" :
            //                      s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId && a.InstitutionId == null).Select(r => (r.FirstName + " " + r.MiddleName).TrimEnd() + " " + r.Surname).FirstOrDefault() :
            //                      s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() : "Public user"),
            //    Amount = s.Amount

            //});



            query2 = query2.GroupBy(cm => new { cm.Year, cm.MonthNum, cm.ClientType, cm.ClientName }).Select(y => new SummarisedClientExpenditureView()
            {
                Year = y.Key.Year,
                MonthNum = y.Key.MonthNum,
                ClientType = y.Key.ClientType,
                ClientName = y.Key.ClientName,
                Amount = y.Sum(p => p.Amount)

            });


            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "RepresentativeClientName";
                    request.SortOrder = "asc";
                }

                if (request.SortColumn == "RepresentativeClientName")
                {
                    if (request.SortOrder == "asc")
                    {
                        query2 = query2.OrderByDescending(s => s.ClientType);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.ClientType);
                    }
                }



            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }


            return query2;


        }

        public static IQueryable<SummarisedCreditActivityView> CreateQueryForSummarisedPostpaidTransactions(ViewSummaryPostpaidTransactionsRequest request,
            IAccountTransactionRepository _accountTransactionRepository, bool DoCount = false)
        {
            CBLContext ctx = ((AccountTransactionRepository)_accountTransactionRepository).ctx;
            IQueryable<AccountTransaction> query = _accountTransactionRepository.GetDbSet().Where(s => s.AccountSourceTypeId == AccountSourceCategory.Postpaid && s.AccountTypeTransactionId == AccountTransactionCategory.CreditTransaction);

            bool LimitToDateRange = request.TransactionDate != null;

            if (LimitToDateRange)
            {
                query = query.Where(s => s.CreatedOn >= request.TransactionDate.StartDate && s.CreatedOn < request.TransactionDate.EndDate);
            }

            if (request.BatchTypeId != null)
            {
                if (request.BatchTypeId == 1)
                {
                    query = query.Where(s => s.AccountBatchId == null);
                }
                else if (request.BatchTypeId == 2)
                {
                    query = query.Where(s => s.AccountBatchId != null);
                }
            }




            if (request.BatchId != null)
            {

                query = query.Where(s => s.AccountBatchId == request.BatchId);
            }



            if (request.SecurityUser.IsOwnerUser == false)
            {
                if (request.LimitTo != null)
                {
                    if (request.LimitTo == 2)
                    {
                        query = query.Where(m => m.MembershipId == request.SecurityUser.MembershipId || m.PostPaidRepresentativeMembershipId == request.SecurityUser.MembershipId);
                    }
                    else if (request.LimitTo == 3)
                    {
                        query = query.Where(m => m.PostPaidRepresentativeMembershipId == request.SecurityUser.MembershipId);
                    }
                    else
                    {
                        query = query.Where(m => m.MembershipId == request.SecurityUser.MembershipId);
                    }
                }
                else
                {
                    query = query.Where(m => m.MembershipId == request.SecurityUser.MembershipId);
                }

            }

            if (!(request.SettlementType == null))
            {
                if (request.SettlementType == 1)
                    query = query.Where(s => s.AccountReconcileId == null);
                else if (request.SettlementType == 2)
                    query = query.Where(s => s.AccountReconcileId != null);

            }
            else
            {
                query = query.Where(s => s.AccountReconcileId == null);
            }

            IQueryable<SummarisedCreditActivityView> query2;

            if (request.ViewType != null && request.ViewType == 2)
            {
                query2 = query.Select(s => new SummarisedCreditActivityView
                {
                    RepresentativeClientName = (s.PostPaidRepresentativeMembershipId != null ? ctx.Institutions.Where(a => a.MembershipId == s.PostPaidRepresentativeMembershipId).Select(r => r.Name).FirstOrDefault() :
                     s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId && a.InstitutionId == null).Select(r => (r.FirstName + " " + r.MiddleName).TrimEnd() + " " + r.Surname).FirstOrDefault() :
                     s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() : "Public user"


                   ),
                    ClientAccountType = (int)s.Membership.MembershipAccountTypeId,
                    ClientName =
                                      (
                                      s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId && a.InstitutionId == null).Select(r => (r.FirstName + " " + r.MiddleName).TrimEnd() + " " + r.Surname).FirstOrDefault() :
                                      s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() : "Public user"

                                      )
                    ,

                    Amount = s.Amount

                });
            }
            else
            {


                query2 = query.Select(s => new SummarisedCreditActivityView
                {
                    RepresentativeClientName = (ctx.Institutions.Where(a => a.MembershipId == s.PostPaidRepresentativeMembershipId).Select(r => r.Name).FirstOrDefault()

                  ),
                    ClientAccountType = 1,

                    ClientName =
                                      (
                                      s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId && a.InstitutionId == null).Select(r => (r.FirstName + " " + r.MiddleName).TrimEnd() + " " + r.Surname).FirstOrDefault() :
                                      s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() : "Public user"

                                      ),


                    Amount = s.Amount

                });
            }

            query2 = query2.GroupBy(cm => new { cm.RepresentativeClientName, cm.ClientAccountType, cm.ClientName }).Select(y => new SummarisedCreditActivityView()
            {
                RepresentativeClientName = y.Key.RepresentativeClientName,
                ClientAccountType = y.Key.ClientAccountType,
                ClientName = y.Key.ClientName,
                Amount = y.Sum(p => p.Amount)

            }
                   );


            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "RepresentativeClientName";
                    request.SortOrder = "asc";
                }

                if (request.SortColumn == "RepresentativeClientName")
                {
                    if (request.SortOrder == "asc")
                    {
                        query2 = query2.OrderByDescending(s => s.RepresentativeClientName);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.RepresentativeClientName);
                    }
                }



            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }


            return query2;


        }

        public static IQueryable<CreditActivityView> CreateQueryForPostpaidTransactionDetials(ViewSummaryPostpaidTransactionsRequest request,
          IAccountTransactionRepository _accountTransactionRepository, bool DoCount = false)
        {
            CBLContext ctx = ((AccountTransactionRepository)_accountTransactionRepository).ctx;


            IQueryable<AccountTransaction> query = _accountTransactionRepository.GetDbSet().Where(s => s.AccountSourceTypeId == AccountSourceCategory.Postpaid && s.AccountReconcileId == null);



            if (request.SecurityUser.IsOwnerUser == false)
            {
                if (request.LimitTo != null)
                {
                    if (request.LimitTo == 2)
                    {
                        query = query.Where(m => m.MembershipId == request.SecurityUser.MembershipId || m.PostPaidRepresentativeMembershipId == request.SecurityUser.MembershipId);
                    }
                    else if (request.LimitTo == 3)
                    {
                        query = query.Where(m => m.PostPaidRepresentativeMembershipId == request.SecurityUser.MembershipId);
                    }
                    else
                    {
                        query = query.Where(m => m.MembershipId == request.SecurityUser.MembershipId);
                    }
                }
                else
                {
                    query = query.Where(m => m.MembershipId == request.SecurityUser.MembershipId);
                }

            }


            bool LimitToDateRange = request.TransactionDate != null;

            if (LimitToDateRange)
            {
                query = query.Where(s => s.CreatedOn >= request.TransactionDate.StartDate && s.CreatedOn < request.TransactionDate.EndDate);
            }
            if (request.BatchId != null)
            {

                request.BatchTypeId = 2;
                query = query.Where(s => s.AccountBatchId == request.BatchId.Value);
            }
            else
            {
                if (request.BatchTypeId != null)
                {
                    if (request.BatchTypeId == 1)
                    {
                        query = query.Where(s => s.AccountBatchId == null);
                    }
                    else if (request.BatchTypeId == 2)
                    {
                        query = query.Where(s => s.AccountBatchId != null);
                    }
                }
            }
            var query2 = query.Select(s => new CreditActivityView
            {
                Id = s.Id,
                Narration = s.Narration,
                ServiceFeeType = s.ServiceFeeType.Name ?? "N/A",
                AccountTransactionType = s.AccountTypeTransaction.FinancialStatementCategoryName,
                AccountTypeTransactionId = s.AccountTypeTransactionId,
                ServiceFeeTypeId = s.ServiceFeeTypeId,
                Amount = s.Amount,
                CreditOrDebit = (s.CreditOrDebit == CreditOrDebit.Credit ? "C" : "D"),
                NewPostpaidBalanceAfterTransaction = s.NewPostpaidBalanceAfterTransaction,
                NewPrepaidBalanceAfterTransaction = s.NewPrepaidBalanceAfterTransaction,
                AccountBatchId = s.AccountBatchId,
                AccountReconcileId = s.AccountReconcileId,
                MembershipId = s.MembershipId,
                PostPaidRepresentativeMembershipId = s.PostPaidRepresentativeMembershipId,
                SettlementAccountTransactionId = s.SettlementAccountTransactionId,
                IsReconciled = !(s.AccountReconcileId.Equals(null)),
                NameOfRepresnetative = ctx.Institutions.Where(a => a.MembershipId == s.PostPaidRepresentativeMembershipId).Select(r => r.Name).FirstOrDefault(),
                NameOfLegalEntity = ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() ?? (s.CreatedByUser.FirstName + " " + s.CreatedByUser.MiddleName).TrimEnd() +
            " " + s.CreatedByUser.Surname,
                NameOfUser = (s.CreatedByUser.FirstName + " " + s.CreatedByUser.MiddleName).TrimEnd() +
            " " + s.CreatedByUser.Surname,
                EntryDate = s.CreatedOn,


                //NameOfUser= ctx .People .Where (r=> r.Id == s.CreatedBy ).Select (t=>t.FirstName)         
            });

            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "EntryDate";
                    request.SortOrder = "desc";
                }

                if (request.SortColumn == "EntryDate")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.EntryDate);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.EntryDate);
                    }
                }



            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }


            return query2;

        }
        public static IQueryable<CreditActivityView> CreateQueryForCreditActivities(ViewCreditActivitiesRequest request,
            IAccountTransactionRepository _accountTransactionRepository, bool DoCount = false)
        {
            CBLContext ctx = ((AccountTransactionRepository)_accountTransactionRepository).ctx;
            IQueryable<AccountTransaction> query = _accountTransactionRepository.GetDbSet();

            if (!request.SecurityUser.IsOwnerUser)
            {
                if (request.LimitTo != null)
                {
                    if (request.LimitTo == 2)
                    {
                        query = query.Where(m => m.MembershipId == request.SecurityUser.MembershipId || m.PostPaidRepresentativeMembershipId == request.SecurityUser.MembershipId);
                    }
                    else if (request.LimitTo == 3)
                    {
                        query = query.Where(m => m.PostPaidRepresentativeMembershipId == request.SecurityUser.MembershipId);
                    }
                    else
                    {
                        query = query.Where(m => m.MembershipId == request.SecurityUser.MembershipId);
                    }
                }
                else
                {
                    query = query.Where(m => m.MembershipId == request.SecurityUser.MembershipId);
                }



            }
            else
            {

                if (request.MembershipId != null)
                {
                    query = query.Where(m => m.CreatedByUser.MembershipId == request.MembershipId);
                }

            }
            bool LimitToDateRange = request.TransactionDate != null;

            if (LimitToDateRange)
            {
                query = query.Where(s => s.CreatedOn >= request.TransactionDate.StartDate && s.CreatedOn < request.TransactionDate.EndDate);
            }
            if (!(request.CreditOrDebit == null))
            {
                query = query.Where(s => s.CreditOrDebit == request.CreditOrDebit);
            }
            if (!(request.AccountTypeTransactionId == null))
            {
                query = query.Where(s => s.AccountTypeTransactionId == request.AccountTypeTransactionId);
            }
            if (!(request.SettlementType == null))
            {
                if (request.SettlementType == 1)
                    query = query.Where(s => s.AccountReconcileId == null && s.AccountSourceTypeId == AccountSourceCategory.Postpaid);
                else
                    query = query.Where(s => s.AccountReconcileId != null && s.AccountSourceTypeId == AccountSourceCategory.Postpaid);
            }

            if (!(request.ServiceFeeTypeId == null))
            {
                query = query.Where(s => s.ServiceFeeTypeId == request.ServiceFeeTypeId);
            }
            if (!(request.AccountBatchId == null))
            {
                query = query.Where(s => s.AccountBatchId == request.AccountBatchId);
            }
            if (!(request.AccountReconcileId == null))
            {
                query = query.Where(s => s.AccountReconcileId == request.AccountReconcileId);
            }
            if (!String.IsNullOrWhiteSpace(request.Narration))
            {
                query = query.Where(m => m.Narration.ToLower().Contains(request.Narration.ToLower()));
            }
            //if (!String.IsNullOrWhiteSpace(request.NameOfUser))
            //{
            //    query = query.Where(m => ((m.CreatedByUser.FirstName.TrimEnd() + " " + m.CreatedByUser.MiddleName.TrimStart()).Trim() +
            //    " " + m.CreatedByUser.Surname.TrimStart()).Trim().ToLower().Contains(request.NameOfUser.ToLower()));
            //}
            if (!String.IsNullOrWhiteSpace(request.NameOfUserLogin))
            {
                //query = query.Where(m => m.CreatedByUser.Username.ToLower().StartsWith(request.NameOfUserLogin.ToLower()));
                query = query.Where(m => m.CreatedByUser.Username.ToLower().Contains(request.NameOfUserLogin.ToLower()));
            }

            var query2 = query.Select(s => new CreditActivityView
            {
                Narration = s.Narration,
                ServiceFeeType = s.ServiceFeeType.Name ?? "N/A",
                AccountTransactionType = s.AccountTypeTransaction.FinancialStatementCategoryName,
                AccountTypeTransactionId = s.AccountTypeTransactionId,
                ServiceFeeTypeId = s.ServiceFeeTypeId,
                Amount = s.Amount,
                CreditOrDebit = (s.CreditOrDebit == CreditOrDebit.Credit ? "C" : "D"),
                NewPostpaidBalanceAfterTransaction = s.NewPostpaidBalanceAfterTransaction,
                NewPrepaidBalanceAfterTransaction = s.NewPrepaidBalanceAfterTransaction,
                AccountBatchId = s.AccountBatchId,
                AccountReconcileId = s.AccountReconcileId,
                MembershipId = s.MembershipId,
                PrepaidOrPostpaid = s.AccountSourceTypeId == AccountSourceCategory.Postpaid ? "Postpaid" : "Prepaid",
                PostPaidRepresentativeMembershipId = s.PostPaidRepresentativeMembershipId,
                SettlementAccountTransactionId = s.SettlementAccountTransactionId,
                NameOfRepresnetative = ctx.Institutions.Where(a => a.MembershipId == s.PostPaidRepresentativeMembershipId).Select(r => r.Name).FirstOrDefault(),
                NameOfLegalEntity = ctx.Institutions.Where(a => a.MembershipId == s.CreatedByUser.MembershipId).Select(r => r.Name).FirstOrDefault(),
                NameOfUser = (s.CreatedByUser.FirstName + " " + s.CreatedByUser.MiddleName).TrimEnd() + " " + s.CreatedByUser.Surname,
                EntryDate = s.CreatedOn,
                //AffectedClient = s.CreatedByUser.Institution != null ? s.CreatedByUser.Institution.Name : "N/A",
                AffectedClient = (
                                   s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId && a.InstitutionId == null).Select(r => (r.FirstName + " " + r.MiddleName ?? "").Trim() + " " + r.Surname).FirstOrDefault() :
                                   s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() : "N/A"

                                   ),



                //NameOfUser= ctx .People .Where (r=> r.Id == s.CreatedBy ).Select (t=>t.FirstName)         
            });

            if (!(String.IsNullOrEmpty(request.ClientName)))
            {
                //query2 = query2.Where(s => s.AffectedClient .ToLower().StartsWith(request.ClientName.ToLower()));
                query2 = query2.Where(s => s.AffectedClient.ToLower().Contains(request.ClientName.ToLower()));
            }
            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "EntryDate";
                    request.SortOrder = "desc";
                }

                if (request.SortColumn == "EntryDate")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.EntryDate);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.EntryDate);
                    }
                }



            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }


            return query2;

        }
        public static IQueryable<PaypointPaymentView> CreateQueryForPaypointPaymentSummary(
          ViewPaypointPaymentsRequest request, IPaymentRepository _pyRps, bool DoCount)
        {

            CBLContext ctx = ((PaymentRepository)_pyRps).ctx;
            IQueryable<Payment> query = _pyRps.GetDbSet().Where(s => s.PaymentSource == PaymentSource.Paypoint);
            if (request.PaymentDate != null)
            {
                query = query.Where(s => s.PaymentDate >= request.PaymentDate.StartDate && s.PaymentDate < request.PaymentDate.EndDate);
            }

            var query2 = query.Select(s => new PaypointPaymentView
         {

             Paypoint = s.CreatedByUser.Institution.Name,
             Amount = s.Amount
         });

            query2 = query2.GroupBy(cm => new { cm.Paypoint }).Select(y => new PaypointPaymentView()
            {

                Paypoint = y.Key.Paypoint,

                Amount = y.Sum(p => p.Amount)

            });

            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "Paypoint";
                    request.SortOrder = "asc";
                }

                if (request.SortColumn == "Paypoint")
                {
                    if (request.SortOrder == "asc")
                    {
                        query2 = query2.OrderByDescending(s => s.Paypoint);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Paypoint);
                    }
                }



            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }


            return query2;

        }



        public static IQueryable<PaymentView> CreateQueryForFindPayment(
           ViewPaymentsRequest request, IPaymentRepository _pyRps, bool DoCount)
        {
            CBLContext ctx = ((PaymentRepository)_pyRps).ctx;
            IQueryable<Payment> query = _pyRps.GetDbSet();

            //    IQueryable<Model.Payment.Payment> query = _pyRps.GetDbSet().Where(s => s.IsDeleted == false && s.IsActive == true);

            if (!(String.IsNullOrEmpty(request.PaymentNo)))
            {
                query = query.Where(s => s.PaymentNo.ToLower() == request.PaymentNo.ToLower());
            }

            //if (!(String.IsNullOrEmpty(request.ClientName )))
            //{
            //    query = query.Where(s => s..ToLower() == request.RequestNo.ToLower());
            //}


            if (request.PaymentTypeId != null && request.PaymentTypeId > 0)
            {
                query = query.Where(s => s.PaymentType == (PaymentType)request.PaymentTypeId);
            }

            if (request.MembershipId != null && request.MembershipId > 0)
            {
                query = query.Where(s => s.MembershipId == request.MembershipId);
            }

            if (!(String.IsNullOrEmpty(request.TransactionNo)))
            {
                query = query.Where(s => s.T24TransactionNo.ToLower().StartsWith(request.TransactionNo.ToLower()));
            }

            if (!(String.IsNullOrEmpty(request.Payee)))
            {
                query = query.Where(s => s.Payee.ToLower().StartsWith(request.Payee.ToLower()));
            }
            if (request.PaymentSourceId != null)
            {
                query = query.Where(s => s.PaymentSource == (PaymentSource)request.PaymentSourceId);
            }
            if (!(String.IsNullOrEmpty(request.PaypointName)))
            {
                query = query.Where(s => s.CreatedByUser.Institution.Name.ToLower().StartsWith(request.PaypointName.ToLower()));
            }


            if (request.IsPublicUser != null || request.IsPublicUser > 0)
            {
                if (request.IsPublicUser.Value == 1)
                {
                    query = query.Where(s => s.MembershipId != null);
                }
                else if (request.IsPublicUser.Value == 2)
                {
                    query = query.Where(s => s.Membership.isIndividualOrLegalEntity == 2);
                }
                else if (request.IsPublicUser.Value == 3)
                {
                    query = query.Where(s => s.Membership.isIndividualOrLegalEntity == 1);
                }
                else if (request.IsPublicUser.Value == 4)
                {
                    query = query.Where(s => s.PublicUserSecurityCodeId != null);
                }
            }
            if (request.PaymentDate != null)
            {
                query = query.Where(s => s.PaymentDate >= request.PaymentDate.StartDate && s.PaymentDate < request.PaymentDate.EndDate);
            }




            //if (request.InstitutionId != null && request.InstitutionId > 0)
            //{
            //    //Get the institution membership

            //    query = query.Where(s => s.Membership  == (CollateralCategory)request.CollateralTypeId);
            //}
            if (request.ShowType == 1) //Payment received at our paypoint
            {
                if (request.SecurityUser.IsOwnerUser == false)
                {

                    if (!(request.SecurityUser.IsInAnyRoles("Administrator (Owner)", "CRL Finance Officer", "Administrator (Client)", "Finance Officer")))
                    {
                        query = query.Where(s => s.CreatedBy == request.SecurityUser.Id);
                    }
                    else
                    {
                        query = query.Where(s => s.CreatedByUser.MembershipId == request.SecurityUser.MembershipId); //For non paypoint users
                    }
                }



            }
            else //Our payments
            {

                query = query.Where(s => s.MembershipId == request.SecurityUser.MembershipId);

            }





            var query2 = query.Select(s => new PaymentView
            {
                Amount = s.Amount,
                Payee = s.Payee,
                PaymentDate = s.PaymentDate,
                PaymentNo = s.PaymentNo,
                ClientType = (s.CreatedByUser.Membership.MembershipTypeId == Model.ModelViews.Enums.MembershipCategory.Owner ? "Registry User" :
                  s.CreatedBy == 1 ? "Public User" :
                  s.CreatedByUser.Membership.isIndividualOrLegalEntity == 1 ? "Individual" :
                     s.CreatedByUser.Membership.isIndividualOrLegalEntity == 2 ? "Financial Institution User"
                      : "N/A"),
                MembershipId = s.MembershipId,
                PaymentType = s.PaymentType == PaymentType.Normal ? 1 : 2,
                PaymentTypeName = s.PaymentType == PaymentType.Normal ? "Normal" : "Reversal",
                T24TransactionNo = s.T24TransactionNo,
                PaymentSourceName = (s.PaymentSource == PaymentSource.Paypoint ? "Paypoint" : s.PaymentSource == PaymentSource.Settlement ? "Settlement" :
                s.PaymentSource == PaymentSource.InterSwitchWebPay ? "Interswtich WebPay" :
                s.PaymentSource == PaymentSource.InterswitchDirectPay ? "Interswitch DirectPay" : ""),
                PublicUserEmail = s.PublicUserSecurityCode.PublicUserEmail,
                SecurityCode = s.PublicUserSecurityCode.SecurityCode,
                IsPublicUser = s.PublicUserSecurityCode != null ? true : false,
                PaypointUser = ((s.CreatedByUser.FirstName.TrimEnd() + " " + s.CreatedByUser.MiddleName.TrimStart()).Trim() +
           " " + s.CreatedByUser.Surname.TrimStart()).Trim(),
                Paypoint = s.CreatedByUser.Institution.Name,
                //Client = s.CreatedByUser.Institution != null ? s.CreatedByUser.Institution.Name : "N/A",
                Client =
                                   (s.MembershipId == null ? s.Payee :
                                   s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId && a.InstitutionId == null).Select(r => (r.FirstName + " " + r.MiddleName).TrimEnd() + " " + r.Surname).FirstOrDefault() :
                                   s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() : "N/A"

                                   ),
                PaymentSource = s.PaymentSource,
                Id = s.Id,

                IsReversible = (s.PaymentType == PaymentType.Normal) && (request.SecurityUser.Id == s.CreatedBy) && (s.ReversedPaymentId == null)



            });


            if (!(String.IsNullOrEmpty(request.ClientName)))
            {
                query2 = query2.Where(s => s.Client.ToLower().StartsWith(request.ClientName.ToLower()));
            }


            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "PaymentDate";
                    request.SortOrder = "desc";
                }

                if (request.SortColumn == "PaymentDate")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.PaymentDate);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.PaymentDate);
                    }
                }

                if (request.SortColumn == "PaymentNo")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.PaymentNo);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.PaymentNo);
                    }
                }


            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }

            return query2;

        }


        public static IQueryable<InterSwitchTransactionView> CreateQueryForInterSwitchTransactions(GetAllInterSwitchTransactionsRequest request, IInterSwitchRepository _interSwitchRepository, bool DoCount)
        {
            CBLContext ctx = ((InterSwitchRepository)_interSwitchRepository).ctx;
            IQueryable<InterSwitchWebPayTransaction> query = _interSwitchRepository.GetDbSet();

            if (!String.IsNullOrWhiteSpace(request.Name))
            {
                query = query.Where(s => s.CustName.ToLower().Contains(request.Name.ToLower()));
            }

            if (!String.IsNullOrWhiteSpace(request.Email))
            {
                query = query.Where(s => s.CustEmail.ToLower().Contains(request.Email.ToLower()));
            }

            if (!String.IsNullOrWhiteSpace(request.Phone))
            {
                query = query.Where(s => s.CustPhone.ToLower().Contains(request.Phone.ToLower()));
            }

            if (request.TransactionDate != null)
            {
                query = query.Where(s => DbFunctions.DiffDays(s.InterSwitchWebPayTransactionQueryResponse.TransactionDate, request.TransactionDate) == 0);
            }



            if (!(String.IsNullOrEmpty(request.TransactionRefNo)))
            {
                query = query.Where(s => s.TransactionReference.ToLower().Equals(request.TransactionRefNo.ToLower().Trim()));
            }

            if (request.CreatedRange != null)
            {
                query = query.Where(s => s.InterSwitchWebPayTransactionQueryResponse.TransactionDate >= request.CreatedRange.StartDate && s.InterSwitchWebPayTransactionQueryResponse.TransactionDate < request.CreatedRange.EndDate);
            }

            if (request.TransactionLogType == 1)
            {
                query = query.Where(s => s.InterSwitchWebPayTransactionQueryResponse.ResponseCode.Equals(InterSwitchConfig.SuccessfulTransaction));
            }
            if (request.TransactionLogType == 2)
            {
                query = query.Where(s => s.InterSwitchWebPayTransactionQueryResponse.ResponseCode != InterSwitchConfig.SuccessfulTransaction && !s.InterSwitchWebPayTransactionQueryResponse.ResponseCode.Equals(null));
            }
            if (request.TransactionLogType == 3)
            {
                query = query.Where(s => s.InterSwitchWebPayTransactionQueryResponse.ResponseCode.Equals(InterSwitchConfig.UnknownTxnRefNo));
            }
            if (request.TransactionLogType == 4)
            {
                query = query.Where(s => s.InterSwitchWebPayTransactionQueryResponse.ResponseCode.Equals(null));
            }
            IQueryable<InterSwitchTransactionView> query2 = query.Select(s => new InterSwitchTransactionView()
            {
                cust_id = s.CustId,
                Name = s.CustName,
                Email = s.CustEmail,
                txn_ref = s.TransactionReference,
                Phone = s.CustPhone,
                Amount = s.Amount,
                TransactionDate = s.InterSwitchWebPayTransactionQueryResponse.TransactionDate,
                IsProcessed = !(s.InterSwitchWebPayTransactionQueryResponse.ResponseCode.Equals(InterSwitchConfig.UnknownTxnRefNo)),
                IsTempError = (InterSwitchConfig.TempErrorList.Contains(s.InterSwitchWebPayTransactionQueryResponse.ResponseCode)),
                IsSuccess = (s.InterSwitchWebPayTransactionQueryResponse.ResponseCode.Equals(InterSwitchConfig.SuccessfulTransaction)),
                IsPending = (s.InterSwitchWebPayTransactionQueryResponse.ResponseCode.Equals(null)),
                IsTopUpPayment = s.IsTopUpPayment
            });
            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "TransactionDate";
                    request.SortOrder = "desc";
                }

                if (request.SortColumn == "TransactionDate")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.TransactionDate);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.TransactionDate);
                    }
                }

                if (request.SortColumn == "Name")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.Name);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Name);
                    }
                }
                if (request.SortColumn == "Email")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.Email);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Email);
                    }
                }
                if (request.SortColumn == "txn_ref")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.txn_ref);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.txn_ref);
                    }
                }
                if (request.SortColumn == "Amount")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.Amount);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Amount);
                    }
                }
                if (request.SortColumn == "Phone")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.Phone);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Phone);
                    }
                }
            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }
            return query2;
        }

        public static IQueryable<InterSwitchDetailsView> CreateQueryForDirectPayTransactions(GetAllDirectPayTransactionsRequest request, IInterSwitchDirectPayRepository _interSwitchDirectPayRepository, bool DoCount)
        {
            CBLContext ctx = ((InterSwitchDirectPayRepository)_interSwitchDirectPayRepository).ctx;
            IQueryable<InterSwitchDirectPayTransaction> query = _interSwitchDirectPayRepository.GetDbSet();

            if (!String.IsNullOrWhiteSpace(request.Name))
            {
                query = query.Where(s => s.CustName.ToLower().Contains(request.Name.ToLower()));
            }

            if (!String.IsNullOrWhiteSpace(request.Email))
            {
                query = query.Where(s => s.CustEmail.ToLower().Contains(request.Email.ToLower()));
            }

            if (!String.IsNullOrWhiteSpace(request.Phone))
            {
                query = query.Where(s => s.CustPhone.ToLower().Contains(request.Phone.ToLower()));
            }

            if (request.TransactionDate != null)
            {
                query = query.Where(s => DbFunctions.DiffDays(s.InterSwitchDirectPayTransactionQueryResponse.CreatedOn, request.TransactionDate) == 0);
            }



            if (request.TransactionLogType == 0)
            {
                query = query.Where(s => s.InterSwitchDirectPayTransactionQueryResponse.StatusId.Equals(InterSwitchConfig.SuccessfulTransaction) && s.IsReversal == false);
            }
            if (request.TransactionLogType == 1)
            {
                query = query.Where(s => s.InterSwitchDirectPayTransactionQueryResponse.StatusId.Equals(null) && s.IsReversal == false);
            }
            if (request.TransactionLogType == 2)
            {
                query = query.Where(s => s.IsProcessed == true && s.IsReversal == true);
            }
            if (request.TransactionLogType == 3)
            {
                query = query.Where(s => s.InterSwitchDirectPayTransactionQueryResponse.StatusId != InterSwitchConfig.SuccessfulTransaction && !s.InterSwitchDirectPayTransactionQueryResponse.StatusId.Equals(null));
            }



            if (!(String.IsNullOrEmpty(request.PaymentVoucherCode)))
            {
                query = query.Where(s => s.DestAccount.ToLower().Equals(request.PaymentVoucherCode.ToLower().Trim()));
            }

            if (request.CreatedRange != null)
            {
                query = query.Where(s => s.InterSwitchDirectPayTransactionQueryResponse.CreatedOn >= request.CreatedRange.StartDate && s.InterSwitchDirectPayTransactionQueryResponse.CreatedOn < request.CreatedRange.EndDate);
            }

            IQueryable<InterSwitchDetailsView> query2 = query.Select(s => new InterSwitchDetailsView()
            {
                Id = s.Id,
                Name = s.CustName,
                Email = s.CustEmail,
                PaymentVoucherCode = s.DestAccount,
                Phone = s.CustPhone,
                Amount = s.Amount,
                TransactionDate = s.InterSwitchDirectPayTransactionQueryResponse.UpdatedOn ?? s.InterSwitchDirectPayTransactionQueryResponse.CreatedOn,
                IsProcessed = s.IsProcessed,
                IsSuccess = (s.InterSwitchDirectPayTransactionQueryResponse.StatusId.Equals(InterSwitchConfig.SuccessfulTransaction)),
                IsPending = (s.InterSwitchDirectPayTransactionQueryResponse.StatusId.Equals(null)),
                IsTopUpPayment = s.IsTopUpPayment
            });

            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "TransactionDate";
                    request.SortOrder = "desc";
                }

                if (request.SortColumn == "TransactionDate")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.TransactionDate);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.TransactionDate);
                    }
                }

                if (request.SortColumn == "Name")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.Name);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Name);
                    }
                }
                if (request.SortColumn == "Email")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.Email);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Email);
                    }
                }
                if (request.SortColumn == "PaymentVoucherCode")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.PaymentVoucherCode);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.PaymentVoucherCode);
                    }
                }
                if (request.SortColumn == "Amount")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.Amount);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Amount);
                    }
                }
                if (request.SortColumn == "Phone")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.Phone);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Phone);
                    }
                }
            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }


            return query2;
        }
    }
}
