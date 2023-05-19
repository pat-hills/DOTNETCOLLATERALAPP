using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model.Payments.IRepository;
using CRL.Respositoy.EF.All.Common.Repository;
using Microsoft.SqlServer.Server;
using CRL.Model.Messaging;
using CRL.Model.ModelViews;

using System.Data.Entity;
using CRL.Model.ModelViews.Payments;
using CRL.Model.Payments;
using CRL.Infrastructure.Configuration;
using CRL.Model.Memberships;
using CRL.Infrastructure.Messaging;
using CRL.Infrastructure.Helpers;


namespace CRL.Repository.EF.All.Repository.Payments
{
    public class PaymentRepository : Repository<Payment, int>, IPaymentRepository
    {
        public PaymentRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "Payment";
        }

        public int CreateBatch(DateTime? StartDate, DateTime? EndDate, int? BatchType, string Batchcomment, int ExecutedBy, string UserIP, string RequestUrl)
        {
            //return this.GetDataContext.Database.ExecuteSqlCommand("", multiplier);
            Batchcomment = String.IsNullOrWhiteSpace(Batchcomment) ? "No comment" : Batchcomment;
            string sql = @"dbo.GenerateSettlement @DateStart ,  @DateEnd , @BatchType , @CreatedBy , @BatchComment, @UserIP, @RequestUrl";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            if (StartDate !=null)
                parameterList.Add(new SqlParameter("@DateStart", StartDate));
            else
                parameterList.Add(new SqlParameter("@DateStart", System.DBNull.Value));
               
            if (EndDate !=null)
                parameterList.Add(new SqlParameter("@DateEnd", EndDate));
            else
                parameterList.Add(new SqlParameter("@DateEnd", System.DBNull.Value));
            parameterList.Add(new SqlParameter("@BatchType", BatchType));
            parameterList.Add(new SqlParameter("@CreatedBy", ExecutedBy));
            parameterList.Add(new SqlParameter("@BatchComment", Batchcomment));
            parameterList.Add(new SqlParameter("@UserIP", UserIP));
            parameterList.Add(new SqlParameter("@RequestUrl", RequestUrl));
            SqlParameter[] parameters = parameterList.ToArray();
            int result = this.ctx.Database.ExecuteSqlCommand(sql, parameters);
            return result;
        }
        public int UndoBatch(int BatchId, int ExecutedBy, string UserIP, string RequestUrl)
        {
            //return this.GetDataContext.Database.ExecuteSqlCommand("", multiplier);
            
            string sql = "exec @MyID = dbo.UndoBatch  @BatchId,  @UserID, @UserIP, @RequestUrl";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            List<SqlDataRecord> paramValue = new List<SqlDataRecord>();
            parameterList.Add(new SqlParameter("@BatchId", BatchId));
            parameterList.Add(new SqlParameter("@UserID", ExecutedBy));
            parameterList.Add(new SqlParameter("@UserIP", UserIP));
            parameterList.Add(new SqlParameter("@RequestUrl", RequestUrl));
            SqlParameter parm = new SqlParameter()
            {
                ParameterName = "@MyID",
                SqlDbType = SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };
            parameterList .Add (parm);

            SqlParameter[] parameters = parameterList.ToArray();
            int result = this.ctx.Database.ExecuteSqlCommand(sql, parameters);






            return (int)parm.Value;
        }
        public int OwnerReconcilePayment(int BatchId, string ReconcileComment, int ExecutedBy, string UserIP, string RequestUrl)
        {
            //return this.GetDataContext.Database.ExecuteSqlCommand("", multiplier);
            ReconcileComment = String.IsNullOrWhiteSpace(ReconcileComment) ? "No comment" : ReconcileComment;
            string sql = "dbo.ReconcileBatch  @BatchId,  @UserID, @ReconcileComment, @UserIP, @RequestUrl";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            List<SqlDataRecord> paramValue = new List<SqlDataRecord>();
            parameterList.Add(new SqlParameter("@BatchId", BatchId));
            parameterList.Add(new SqlParameter("@UserID", ExecutedBy));
            parameterList.Add(new SqlParameter("@ReconcileComment", "Hello"));
            parameterList.Add(new SqlParameter("@UserIP", UserIP));
            parameterList.Add(new SqlParameter("@RequestUrl", RequestUrl));

            SqlParameter[] parameters = parameterList.ToArray();
            int result = this.ctx.Database.ExecuteSqlCommand(sql, parameters);
            return result;


        }
        public int ConfirmSubPostpaidClients(int BatchId, int[] RepresentativeInstitution, string ReconcileComment, int ExecutedBy, string UserIP, string RequestUrl)
        {
            //return this.GetDataContext.Database.ExecuteSqlCommand("", multiplier);
            ReconcileComment = String.IsNullOrWhiteSpace(ReconcileComment) ? "No comment" : ReconcileComment;
            string sql = "dbo.ConfirmSubPostpaidClient  @BatchId, @SelectedRepresentativeInstitutions, @UserID, @ReconcileComment, @UserIP, @RequestUrl";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            List<SqlDataRecord> paramValue = new List<SqlDataRecord>();


            foreach (var i in RepresentativeInstitution)
            {
                SqlDataRecord pval = new SqlDataRecord(new SqlMetaData("ID", SqlDbType.Int));
                pval.SetInt32(0, i);
                paramValue.Add(pval);


            }

            SqlParameter param = new SqlParameter();
            param.SqlDbType = System.Data.SqlDbType.Structured;
            param.TypeName = "dbo.ARRAYINT";
            param.Value = paramValue;
            param.ParameterName = "@SelectedRepresentativeInstitutions";


            parameterList.Add(new SqlParameter("@BatchId", BatchId));
            parameterList.Add(param);
            parameterList.Add(new SqlParameter("@UserID", ExecutedBy));
            parameterList.Add(new SqlParameter("@ReconcileComment", "Hello"));
            parameterList.Add(new SqlParameter("@UserIP", UserIP));
            parameterList.Add(new SqlParameter("@RequestUrl", RequestUrl));

            SqlParameter[] parameters = parameterList.ToArray();
            int result = this.ctx.Database.ExecuteSqlCommand(sql, parameters);
            return result;
        }

        public Payment GetWebPayPayment(int InterSwitchTransactionId)
        {
            return ctx.Payments.Include("PublicUserSecurityCode").SingleOrDefault(s => s.InterSwitchWebPayTransactionId == InterSwitchTransactionId);
        }

        public Payment GetPaymentBySecurityCode(int SecurityCodeId) 
        {
            return ctx.Payments.Include("InterSwitchDirectPayTransaction").Where(s => s.PublicUserSecurityCodeId == SecurityCodeId).OrderByDescending(s => s.Id).FirstOrDefault();
        }

        public Payment GetDirectPayPayment(int InterSwitchTransactionId) 
        {
            return ctx.Payments.Include("PublicUserSecurityCode").SingleOrDefault(s => s.InterSwitchDirectPayTransactionId == InterSwitchTransactionId);
        }

        public PaymentView GetPaymentView(int Id)
        {
            IQueryable<Payment> query = ctx.Payments.AsNoTracking().Where(s => s.Id == Id);

            var query2 = query.Select(s => new PaymentView
            {
                Amount = s.Amount,
                Payee = s.Payee,
                PaymentDate = s.PaymentDate,
                PaymentNo = s.PaymentNo,
                MembershipId = s.MembershipId,
                T24TransactionNo = s.T24TransactionNo,
                PaymentTypeName = s.PaymentType == PaymentType.Normal ? "Normal" : s.PaymentType == PaymentType.Reversal ? "Reversal" : "",
                PaymentType = s.PaymentType == PaymentType.Normal ? 1 : s.PaymentType == PaymentType.Reversal ? 2 : 0,
                PaymentSourceName = (s.PaymentSource == PaymentSource.Paypoint ? "Paypoint" : s.PaymentSource == PaymentSource.Settlement ? "Settlement" :
           s.PaymentSource == PaymentSource.InterSwitchWebPay ? "Interswitch WebPay" :
           s.PaymentSource == PaymentSource.InterswitchDirectPay ? "Interswitch DirectPay" : ""),
           PaymentSource = s.PaymentSource ,
                PublicUserEmail = s.PublicUserSecurityCode.PublicUserEmail,
                SecurityCode = s.PublicUserSecurityCode.SecurityCode,
                IsPublicUser = s.PublicUserSecurityCode != null ? true : false,
                Client = s.PublicUserSecurityCode != null ? s.Payee :
                        s.Membership.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.MembershipId && a.InstitutionId == null).Select(r => (r.FirstName + " " + r.MiddleName ?? "").Trim() + " " + r.Surname).FirstOrDefault() :
                        s.Membership.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.MembershipId).Select(r => r.Name).FirstOrDefault() : "N/A",
                PaypointUser = ((s.CreatedByUser.FirstName + " " + s.CreatedByUser.MiddleName ?? "").Trim() + " " + s.CreatedByUser.Surname),
                Paypoint = s.CreatedByUser.Institution.Name,
                SettlementId = s.PaymentSource == PaymentSource.Settlement ? ctx.AccountTransactions.OfType<PaymentAccountTransaction>().Where(a => a.PaymentId == s.Id).FirstOrDefault().AccountReconcileId : null,
                Id = s.Id,
                 CreatedBy = s.CreatedBy ,
                  CreatedByUserMembershipId = s.CreatedByUser .MembershipId 
                   
            });

            return query2.Single ();

        }   

    }


    public class PublicUserSecurityCodeRepository : Repository<PublicUserSecurityCode, int>, IPublicUserSecurityCodeRepository
    {
        public PublicUserSecurityCodeRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "PublicUserSecurityCode";
        }


            
            
         
          
        
        

    }



    public class AccountTransactionRepository : Repository<AccountTransaction, int>, IAccountTransactionRepository
    {
        public AccountTransactionRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "AccountTransaction";
        }

        public ViewCreditActivitiesResponse CustomViewCreditActivities(ViewCreditActivitiesRequest request)
        {
            ViewCreditActivitiesResponse response = new ViewCreditActivitiesResponse();

            IQueryable<AccountTransaction> query = ctx.AccountTransactions.AsNoTracking();

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

            if (request.PageIndex > 0)
            {
                response.NumRecords = query.Count();
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
                //AccountBatchId = s.AccountBatchId,
                AccountReconcileId = s.AccountReconcileId,
                MembershipId = s.MembershipId,
                PrepaidOrPostpaid = s.AccountSourceTypeId == AccountSourceCategory.Postpaid ? "Postpaid" : "Prepaid",
                PostPaidRepresentativeMembershipId = s.PostPaidRepresentativeMembershipId,
                SettlementAccountTransactionId = s.SettlementAccountTransactionId,
                NameOfRepresnetative = s.PostPaidRepresentativeMembershipId !=null ? ctx.Institutions.Where(a => a.MembershipId == s.PostPaidRepresentativeMembershipId).Select(r => r.Name).FirstOrDefault() : "N/A",
                NameOfLegalEntity = ctx.Institutions.Where(a => a.MembershipId == s.CreatedByUser.MembershipId).Select(r => r.Name).FirstOrDefault(),
                NameOfUser = (s.CreatedByUser.FirstName + " " + s.CreatedByUser.MiddleName).TrimEnd() + " " + s.CreatedByUser.Surname,
                UserLoginId = s.CreatedByUser.Username ,
                UserUnit = s.CreatedByUser .InstitutionUnit .Name != null ? s.CreatedByUser.InstitutionUnit.Name : "N/A",
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





            if (request.PageIndex > 0)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }


            response.CreditActivityView = query2.ToList();
            return response;
        }

        public ICollection<ClientAmountWithRepBankView> ViewClientExpenditureSummary(int BatchId)
        {
            var query = ctx.AccountTransactions.AsNoTracking().Where(s => s.AccountTypeTransactionId == AccountTransactionCategory.CreditTransaction & s.AccountBatchId == BatchId );

            var query2 = query.Select(s => new ClientAmountWithRepBankView
            {
                Client = ctx.Institutions.Where(m => m.MembershipId == s.MembershipId).FirstOrDefault().Name,
                MembershipId = (int)s.MembershipId,
                RepresentativeBank = ctx.Institutions.Where(m => m.MembershipId == s.PostPaidRepresentativeMembershipId).FirstOrDefault().Name,
                 RepresentativeBankMembershipID = (int)s.PostPaidRepresentativeMembershipId ,
                   Amount = s.Amount
                
            });

            query2 = query2.GroupBy(cm => new { cm.Client, cm.MembershipId,cm.RepresentativeBank , cm.RepresentativeBankMembershipID }).Select(y => new ClientAmountWithRepBankView()
            {

                Client = y.Key.Client,              
                MembershipId = y.Key.MembershipId,
                RepresentativeBank  = y.Key.RepresentativeBank ,
                RepresentativeBankMembershipID  = y.Key.RepresentativeBankMembershipID ,
                Amount = y.Sum(p => p.Amount),


            }
              );

            return query2.ToList();


        }
        public ICollection<ClientSettlementSummaryView> ViewClientSettlementSummary(int BatchId)
        {
            var query = ctx.AccountTransactions.OfType<PaymentAccountTransaction>().AsNoTracking();
            query = query.Where(s => s.AccountSourceTypeId == AccountSourceCategory.Postpaid &&
                                   s.AccountTypeTransactionId == AccountTransactionCategory.SettlementPayment &&
                                   s.AccountBatchId == BatchId);
            var query2 = query.Select(s => new ClientSettlementSummaryView
            {
                Client = ctx.Institutions.Where(m => m.MembershipId == s.MembershipId).FirstOrDefault().Name,
                Amount = s.Amount,
                MembershipId = (int)s.MembershipId,
                ReconcilationNumber = (int)s.AccountReconcileId,
                SettledDate = s.CreatedOn
            });

            return query2.ToList();

        }
        public ICollection<ExpenditureByTransactionView> ViewClientExpendituresByTransaction(int BatchId)
        {
            var query = ctx.AccountTransactions.AsNoTracking().Where(s => s.AccountTypeTransactionId == AccountTransactionCategory.CreditTransaction && s.AccountBatchId == BatchId );
            query = query.Where(s => s.PostPaidRepresentativeMembershipId == null || s.PostPaidRepresentativeMembershipId == 1);



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

            return query2.ToList();


        }


        public ICollection<ClientAmountSelectionView> GetClientForConfirmationInPostpaidBatch(int BatchId)
        {
            var query = ctx.AccountTransactions.OfType<PaymentAccountTransaction>().AsNoTracking();
            query = query.Where(s => s.AccountSourceTypeId == AccountSourceCategory.Postpaid &&
                                   s.AccountTypeTransactionId == AccountTransactionCategory.SettlementPayment &&
                                   s.AccountBatchId == BatchId && s.Payment.PendingConfirmation == true);
            var query2 = query.Select(s => new ClientAmountSelectionView
            {
                Client = ctx.Institutions.Where(m => m.MembershipId == s.MembershipId).FirstOrDefault().Name,
                Amount = s.Amount,
                MembershipId = (int)s.MembershipId
                
            });

            return query2.ToList();

        }
        /// <summary>
        /// Gets all postpaid clients (direct and indirect with the sums of their expenses)
        /// </summary>
        /// <param name="BatchId"></param>
        /// <returns></returns>
        public ICollection<ClientAmountView> GetClientAmountViewForReconcilation(int BatchId)
        {
            var query = ctx.AccountTransactions.OfType<PaymentAccountTransaction>().AsNoTracking();
            query = query.Where(s => s.AccountSourceTypeId == AccountSourceCategory.Postpaid &&
                                   s.AccountTypeTransactionId == AccountTransactionCategory.SettlementPayment &&
                                   s.AccountBatchId == BatchId && s.Payment.PendingConfirmation == true);
            var query2 = query.Select(s => new ClientAmountView
            {
                Client = ctx.Institutions.Where(m => m.MembershipId == s.MembershipId).Single().Name,
                Amount = s.Amount,
                MembershipId = (int)s.MembershipId

            });

            return query2.ToList();

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BatchId"></param>
        /// <returns></returns>
        public ICollection<ClientAmountView> GetMyPostpaidClientExpenditures(int BatchId)
        {
            var query = ctx.AccountTransactions.AsNoTracking().Where(s => s.AccountTypeTransactionId == AccountTransactionCategory.CreditTransaction && s.AccountBatchId == BatchId );
            query = query.Where(s => s.PostPaidRepresentativeMembershipId !=null && s.PostPaidRepresentativeMembershipId != 1);

            var query2 = query.Select(s => new ClientAmountView
            {
                Client = ctx.Institutions.Where(m => m.MembershipId == s.MembershipId).FirstOrDefault ().Name,
                Amount = s.Amount,
                MembershipId = (int)s.MembershipId
            });

            query2 = query2.GroupBy(cm => new { cm.Client, cm.MembershipId  }).Select(y => new ClientAmountView()
            {

                Client = y.Key.Client ,
                 Amount = y.Sum(p => p.Amount),
                MembershipId = y.Key .MembershipId ,
               

            }
              );

            return query2.ToList();

         

        }
       
    }


    public class AccountBatchRepository : Repository<AccountBatch, int>, IAccountBatchRepository
    {
        public AccountBatchRepository(IUnitOfWork uow)
            : base(uow) 
        {

        }

        public ViewAccountBatchesResponse GetAccountBatches(ViewAccountBatchesRequest request)
        {

            ViewAccountBatchesResponse response = new ViewAccountBatchesResponse();
            IQueryable<AccountBatch> query = ctx.AccountBatches.AsNoTracking().Where (s=>s.IsDeleted == false);
           

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


            if (request.PageIndex > 0)
            {
                response.NumRecords = query.Count();
            }
            var query2 = query.Select(s => new AccountBatchView
            {
                Id = s.Id,
                Name = s.Name,
                PeriodStartDate = s.PeriodStartDate,
                PeriodEndDate = s.PeriodEndDate,
                Comment = s.Comment,
                BatchStatus = s.isReconciled == false ? "Not reconciled" :
                             s.isReconciled == true && s.ConfirmSubPostpaidAccount == false ? "Reconciled" :
                             s.isReconciled == true && s.ConfirmSubPostpaidAccount == true ? "Pending Confirmation" : "N/A",

                CreatedOn = s.CreatedOn,
                InstitutionId = s.InstitutionId,
                InstitutionName = s.Institution .Name ,
                isReconciled = s.isReconciled ,
                ConfirmSubPostpaidAccount = s.ConfirmSubPostpaidAccount 
          

                 
               
            });

            if ((String.IsNullOrWhiteSpace(request.SortColumn)))
            {
                request.SortColumn = "CreatedOn";
                request.SortOrder = "desc";
            }

            if (request.SortColumn == "CreatedOn")
            {
                if (request.SortOrder == "desc")
                {
                    query2 = query2.OrderByDescending(s => s.CreatedOn);
                }
                else
                {
                    query2 = query2.OrderBy(s => s.CreatedOn);
                }
            }

            if (request.PageIndex > 0)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }

            response.AccountBatches = query2.ToList();
            return response;

           
        }

        public AccountBatchView GetAccountBatchView(int BatchID)
        {
              var query  = ctx.AccountBatches .Where(s=>s.Id == BatchID );
            
           var query2 = query.Select(s => new AccountBatchView
            {
                Id = s.Id,
                Name = s.Name,
                PeriodStartDate = s.PeriodStartDate,
                PeriodEndDate = s.PeriodEndDate,
                Comment = s.Comment,
                BatchStatus =  s.isReconciled == false ? "Not reconciled": 
                             s.isReconciled == true && s.ConfirmSubPostpaidAccount  ==false ? "Reconciled":
                             s.isReconciled == true && s.ConfirmSubPostpaidAccount == true ? "Pending Confirmation":"N/A",

                CreatedOn = s.CreatedOn,
                InstitutionId = s.InstitutionId,
                InstitutionName = s.Institution .Name ,
                isReconciled = s.isReconciled ,
                ConfirmSubPostpaidAccount = s.ConfirmSubPostpaidAccount,
                 TotalSettlement = ctx.AccountTransactions .Where (m=>m.AccountTypeTransactionId == AccountTransactionCategory.SettlementPayment  && m.AccountBatchId == BatchID ).Select (e=>e.Amount ).Sum (),
                TotalExpenses  = ctx.AccountTransactions.Where(m => m.AccountTypeTransactionId == AccountTransactionCategory.CreditTransaction && m.AccountBatchId == BatchID).Select(e => e.Amount).Sum(),

                  
          

                 
               
            });


           return query2.Single();
        }
        public override string GetEntitySetName()
        {
            return "AccountBatch";
        }
    }

    public class InterSwitchRepository : Repository<InterSwitchWebPayTransaction, int>, IInterSwitchRepository
    {
        public InterSwitchRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "InterSwitchWebPayTransaction";
        }

        public InterSwitchWebPayTransaction GetTransactionByRefNo(string tranRefNo)
        {
            return ctx.InterSwitchWebPayTransaction.Include("InterSwitchWebPayTransactionQueryResponse").SingleOrDefault(s => s.TransactionReference == tranRefNo && s.IsActive == true && s.IsDeleted == false);
        }

        public bool IsGeneratedReferenceCode(string referenceCode)
        {
            return ctx.InterSwitchWebPayTransaction.Any(s => s.TransactionReference == referenceCode && s.IsActive == true && s.IsDeleted == false);
        }
    }

    public class InterSwitchDirectPayRepository : Repository<InterSwitchDirectPayTransaction, int>, IInterSwitchDirectPayRepository 
    {
        public InterSwitchDirectPayRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "InterSwitchDirectPayTransaction";
        }

        public InterSwitchDirectPayTransaction GetTransactionBySeqNo(string sequenceNo)
        {
            return ctx.InterSwitchDirectPayTransaction.Include("InterSwitchDirectPayTransactionQueryResponse").SingleOrDefault(s => s.SequenceNo == sequenceNo && s.IsReversal == false & s.IsProcessed == null && s.IsActive == true && s.IsDeleted == false);
        }

        public InterSwitchDirectPayTransaction GetTransactionByDestAcc(string destAcc)
        {
            return ctx.InterSwitchDirectPayTransaction.Include("InterSwitchDirectPayTransactionQueryResponse").SingleOrDefault(s => s.DestAccount == destAcc && s.IsTopUpPayment == false && s.IsReversal == false & s.IsProcessed == null && s.IsActive == true && s.IsDeleted == false);
        }

        public bool IsReversed(string sequenceNo)
        {
            return ctx.InterSwitchDirectPayTransaction.Any(s => s.SequenceNo == sequenceNo && s.IsReversal == true && s.IsProcessed == true && s.IsActive == true && s.IsDeleted == false);
        }

        public bool IsGeneratedPaymentCode(string paymentCode)
        {
            return ctx.InterSwitchDirectPayTransaction.Any(s => s.DestAccount == paymentCode && s.IsActive == true && s.IsDeleted == false);
        }
    }

}
