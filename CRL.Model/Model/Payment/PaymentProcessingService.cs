using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Configuration;
using CRL.Model.Configuration.IRepository;
using CRL.Model.FS;
using CRL.Model.ModelViews;

using CRL.Model.Search;
using CRL.Infrastructure.Messaging;
using CRL.Model.FS.IRepository;
using CRL.Infrastructure.Authentication;
using CRL.Model.Memberships.IRepository;
using CRL.Model.Payments.IRepository;
using CRL.Model.Memberships;
using CRL.Model.Model.FS;

namespace CRL.Model.Payments
{
    public class PaymentInfoResponse : ResponseBase
    {
        public bool isPayabale { get; set; }
        public decimal? Amount { get; set; }
        public bool AmountIsBasedOnLoanAmount { get; set; }
    }
    public class NotEnoughBalance : ApplicationException
    {
        public NotEnoughBalance()
            : base("You do not enough amount in your account to complete this transaction!")
        {

        }


    }
    public class NullAmountToPay : ApplicationException
    {
        public NullAmountToPay()
            : base("Cannot log account transaction with null amount")
        {

        }


    }


    public class PaymentServiceModel
    {


        private IAccountTransactionRepository _accountTransactionRepository;
        private IConfigurationFeeRepository _configurationFeeRepository;
        private IMembershipRepository _membershipRepository;
        private ILKCurrencyRepository _currencyRepository;
        private TransactionPaymentSetup fee;
        public Membership membership { get; set; }
        LKCurrency currency = null;
        public AuditingTracker auditTracker { get; set; }
        private SecurityUser _executingUser;


        public PaymentServiceModel()
        {

        }
        public PaymentServiceModel(IConfigurationFeeRepository configurationFeeRepository,
                                   IMembershipRepository membershipRepository,
                                   ILKCurrencyRepository currencyRepository,

           SecurityUser user)
        {

            _membershipRepository = membershipRepository;
            _configurationFeeRepository = configurationFeeRepository;
            _currencyRepository = currencyRepository;

            _executingUser = user;
        }
        public PaymentServiceModel(IConfigurationFeeRepository configurationFeeRepository,
                                   IMembershipRepository membershipRepository,
                                   ILKCurrencyRepository currencyRepository,
            IAccountTransactionRepository accountTransactionRepository,
             AuditingTracker _audtitracker, SecurityUser user)
        {
            _accountTransactionRepository = accountTransactionRepository;
            _membershipRepository = membershipRepository;
            _configurationFeeRepository = configurationFeeRepository;
            _currencyRepository = currencyRepository;
            auditTracker = _audtitracker;
            _executingUser = user;
        }


        public void InitialisePayment(ServiceFees feeTytpe, int LenderType, int? currencyType = null)
        {
            fee = _configurationFeeRepository.GetDbSetComplete(feeTytpe, LenderType).SingleOrDefault();


            if (this._executingUser != null)
                membership = _membershipRepository.FindBy(this._executingUser.MembershipId);
            if (currencyType != null) currency = _currencyRepository.FindBy((int)currencyType);
        }

        public PaymentInfoResponse QuickCheckPayment()
        {
            PaymentInfoResponse paymentInfoResponse = new PaymentInfoResponse();
            if (fee != null && fee.Fee > 0)
            {
                paymentInfoResponse.isPayabale = true;


                paymentInfoResponse.Amount = fee.Fee;

            }


            return paymentInfoResponse;




        }


        public PaymentInfoResponse ValidatePayment(decimal? LoanAmount = null, PublicUserSecurityCode publicUser = null)
        {

            PaymentInfoResponse paymentInfoResponse = new PaymentInfoResponse();
            //For registry users we will have to set this to false            
            if (publicUser == null && _executingUser.IsOwnerUser)
            {
                paymentInfoResponse.isPayabale = false;
            }
            else
            {
                //decimal AmountToPay = 0;
                if (fee != null && fee.Fee > 0)
                {



                    paymentInfoResponse.isPayabale = true;
                    paymentInfoResponse.Amount = fee.Fee;




                    if (membership != null)
                    {
                        if (!membership.HasValidPostpaidAccount())
                        {
                            paymentInfoResponse.MessageInfo = new MessageInfo { Message = "You do not have a valid postpaid account to perform this transaction!", MessageType = MessageType.Error };
                            return paymentInfoResponse;
                        }
                        if (membership.MembershipAccountTypeId == ModelViews.Enums.MembershipAccountCategory.NonRegular && membership.PrepaidCreditBalance < paymentInfoResponse.Amount)
                        {
                            paymentInfoResponse.MessageInfo = new MessageInfo { Message = "You do not have enough credit in your account to perform this transaction", MessageType = MessageType.Error };
                            return paymentInfoResponse;
                        }
                    }
                    else if (publicUser != null)
                    {
                        if (publicUser.Balance < paymentInfoResponse.Amount)
                        {
                            paymentInfoResponse.MessageInfo = new MessageInfo { Message = "You do not have enough credit in your account to perform this transaction", MessageType = MessageType.Error };
                            return paymentInfoResponse;
                        }
                    }
                    else
                    {
                        paymentInfoResponse.MessageInfo = new MessageInfo { Message = "You need to provide a security code before you can conduct a search!", MessageType = MessageType.Error };
                        return paymentInfoResponse;
                    }
                }
                else
                {
                    paymentInfoResponse.isPayabale = false;
                }
            }

            paymentInfoResponse.GenerateDefaultSuccessMessage();
            return paymentInfoResponse;


        }



        public bool ProcessNewFSPayment(FS.FinancialStatement financingStatement)
        {
            bool result = false;

            CreateFSAccountTransaction accountTransaction = new CreateFSAccountTransaction();
            accountTransaction.ServiceFeeTypeId = ServiceFees.NewFinancingStatement;
            accountTransaction.FinanacialStatement = financingStatement;
            LKCurrency currency = financingStatement.MaximumAmountSecuredCurrency;
            accountTransaction.Narration = "Loan Amount: " + currency.CurrencyCode + financingStatement.MaximumAmountSecured.Value.ToString("#,##0.00") + " | " + "Registration No: " + financingStatement.RegistrationNo;

            decimal Amount = 0;

            if (currency.isLocal || currency.CurrencyCode == "USD")

                Amount = (decimal)financingStatement.MaximumAmountSecured;


            //else if (currency.SellingRateWithLocalCurrency != null)                
            //Amount = ((decimal)financingStatement.MaximumAmountSecured) * (decimal)currency.SellingRateWithLocalCurrency;


            //Check the amount
            if (fee != null)
            {

                result = ProcessPerTransactionPayment(accountTransaction, membership, fee, currency, Amount);
            }




            return result;



        }
        public bool ProcessFSActivityPayment(FinancialStatementActivity activity)
        {
            bool result = false;
            ActivityAccountTransaction accountTransaction = new ActivityAccountTransaction();
            accountTransaction.FinancialStatementActivity = activity;

            if (activity is ActivityUpdate)
            {
                accountTransaction.ServiceFeeTypeId = ServiceFees.UpdateOfFinancingStatement;
            }
            else if (activity is ActivityDischarge)
            {
                accountTransaction.ServiceFeeTypeId = ServiceFees.DischargeofFinancingStatement;
            }
            else if (activity is ActivityDischargeDueToError)
            {
                accountTransaction.ServiceFeeTypeId = ServiceFees.DischargeofFinancingStatement;
            }
            else if (activity is ActivitySubordination)
            {
                accountTransaction.ServiceFeeTypeId = ServiceFees.SubordinationOfFinancingStatement;
            }
            else if (activity is ActivityAssignment)
            {
                accountTransaction.ServiceFeeTypeId = ServiceFees.AssignmentOfFinancingStatement;
            }
            //LKCurrency currency = activity.FinancialStatement.MaximumAmountSecuredCurrency;
            LKCurrency currency = _currencyRepository.FindBy((int)activity.FinancialStatement.MaximumAmountSecuredCurrencyId);
            decimal Amount = 0;
            if (currency.isLocal || currency.CurrencyCode == "USD")
                Amount = (decimal)activity.FinancialStatement.MaximumAmountSecured;
            //else if (currency.SellingRateWithLocalCurrency != null)
            //    Amount = ((decimal)activity.FinancialStatement.MaximumAmountSecured) * (decimal)currency.SellingRateWithLocalCurrency;
            accountTransaction.Narration = "Loan Amount: " + currency.CurrencyCode + activity.FinancialStatement.MaximumAmountSecured.Value.ToString("#,##0.00") + " | Activity No: " + activity.ActivityCode + " | Registration No: " + activity.FinancialStatement.RegistrationNo;
            //Check the amount
            if (fee != null)
            {

                result = ProcessPerTransactionPayment(accountTransaction, membership, fee, currency, Amount);


            }
            return result;

        }

        public bool ProcessSearchPayment(SearchFinancialStatement search, short IsSearchOrGenerateResult, PublicUserSecurityCode publicUser = null)
        {
            bool ProcessPayment = _executingUser == null ? this.ProcessUnRegisteredUserSearch(publicUser, search, IsSearchOrGenerateResult) :
                      _executingUser.IsOwnerUser == false ? this.ProcessRegisteredUserSearch(search, IsSearchOrGenerateResult) : false;
            return ProcessPayment;
        }
        private bool ProcessRegisteredUserSearch(SearchFinancialStatement search, short IsSearchOrGenerateResult)
        {
            bool result = false;
            SearchAccountTransaction accountTransaction = new SearchAccountTransaction();

            accountTransaction.ServiceFeeTypeId = IsSearchOrGenerateResult == 1 ? ServiceFees.Search : ServiceFees.CertifiedSearchResult;
            accountTransaction.SearchFinancialStatement = search;
            accountTransaction.Narration = "Search Date: " + auditTracker.Date;
            if (fee != null)
            {
                result = ProcessPerTransactionPayment(accountTransaction, membership, fee);

            }
            return result;
        }

        private bool ProcessUnRegisteredUserSearch(PublicUserSecurityCode publicUser, SearchFinancialStatement search, short IsSearchOrGenerateResult)
        {
            bool result = false;
            SearchAccountTransaction accountTransaction = new SearchAccountTransaction();
            accountTransaction.ServiceFeeTypeId = IsSearchOrGenerateResult == 1 ? ServiceFees.PublicSearch : ServiceFees.CertifiedSearchResultPublic;
            accountTransaction.SearchFinancialStatement = search;
            accountTransaction.Narration = "Search Date: " + auditTracker.Date;
            if (fee != null)
            {

                result = ProcessPerTransactionPayment(accountTransaction, publicUser, fee);
            }

            return result;
        }
        public bool ProcessPerTransactionPayment(AccountTransaction accountTransaction, PublicUserSecurityCode publicUser, TransactionPaymentSetup configurationFee)
        {
            if (publicUser.Balance < configurationFee.Fee)
            {
                throw new NotEnoughBalance();
            }

            accountTransaction.Amount = configurationFee.Fee;
            accountTransaction.CreditOrDebit = CreditOrDebit.Debit;
            accountTransaction.AccountTypeTransactionId = AccountTransactionCategory.CreditTransaction;
            accountTransaction.NewPrepaidBalanceAfterTransaction = publicUser.Balance - configurationFee.Fee;
            publicUser.Balance = publicUser.Balance - configurationFee.Fee;
            if (auditTracker == null)
                throw new Exception("Audit tracker cannot be null");
            auditTracker.Created.Add(accountTransaction);
            _accountTransactionRepository.Add(accountTransaction);
            auditTracker.Updated.Add(publicUser);
            return true;
        }
        public bool ProcessPerTransactionPayment(AccountTransaction accountTransaction, Membership membership, TransactionPaymentSetup configurationFee, LKCurrency currency = null, decimal? LoanAmount = null)
        {

            //Get the amount to pay
            PaymentInfoResponse paymentInfoResponse = this.ValidatePayment(LoanAmount);

            //Validate membership account

            accountTransaction.Membership = membership;
            if (membership.MembershipAccountTypeId == ModelViews.Enums.MembershipAccountCategory.NonRegular) //** || configurationFee.AllowPostpaidIfClientIsPostpaid == false)
            {
                if (membership.PrepaidCreditBalance < paymentInfoResponse.Amount)
                    throw new NotEnoughBalance();


                accountTransaction.AccountSourceTypeId = AccountSourceCategory.Prepaid;
                accountTransaction.NewPrepaidBalanceAfterTransaction = membership.PrepaidCreditBalance - (decimal)paymentInfoResponse.Amount;
                membership.PrepaidCreditBalance = membership.PrepaidCreditBalance - (decimal)paymentInfoResponse.Amount;
                accountTransaction.NewPostpaidBalanceAfterTransaction = membership.PostpaidCreditBalance;



            }
            else
            {
                accountTransaction.AccountSourceTypeId = AccountSourceCategory.Postpaid;
                accountTransaction.PostPaidRepresentativeMembership = membership.Representative;
                accountTransaction.NewPostpaidBalanceAfterTransaction = membership.PostpaidCreditBalance - (decimal)paymentInfoResponse.Amount;
                accountTransaction.NewPrepaidBalanceAfterTransaction = membership.PrepaidCreditBalance;
                membership.PostpaidCreditBalance = membership.PostpaidCreditBalance - (decimal)paymentInfoResponse.Amount;
            }
            if (paymentInfoResponse.Amount == null)
            {
                throw new NullAmountToPay();
            }

            //Process the account
            accountTransaction.Amount = (decimal)paymentInfoResponse.Amount;
            accountTransaction.CreditOrDebit = CreditOrDebit.Debit;
            accountTransaction.AccountTypeTransactionId = AccountTransactionCategory.CreditTransaction;



            if (auditTracker == null)
                throw new Exception("Audit tracker cannot be null");
            auditTracker.Created.Add(accountTransaction);
            _accountTransactionRepository.Add(accountTransaction);
            auditTracker.Updated.Add(membership);

            return true;
        }


        //public bool ProcessNewFSPayment(Membership.Membership membership, ConfigurationFee fee, FinancialStatement.FinancialStatement financingStatement)
        //{
        //    bool result;
        //    CreateFSAccountTransaction accountTransaction = new CreateFSAccountTransaction();
        //    accountTransaction.FinanacialStatement = financingStatement;
        //    //accountTransaction.Narration = "New Financing Statement";
        //    result = ProcessFee(fee, membership, accountTransaction);
        //    if (result)
        //    {
        //        if (auditTracker == null)
        //            throw new Exception("Audit tracker cannot be null");
        //        auditTracker.Created.Add(accountTransaction);
        //        _accountTransactionRepository.Add(accountTransaction);
        //        auditTracker.Updated.Add(membership);
        //    }

        //    return result;
        //}




        //public bool ProcessFee(ConfigurationFee fee, PublicUserSecurityCode publicUser, AccountTransaction accountTransaction)
        //{


        //        if (!CheckFees(fee.FlatAmount, publicUser.Balance ))
        //        {

        //            return false;
        //        }
        //        accountTransaction.Amount = fee.FlatAmount;
        //        accountTransaction.CreditOrDebit = CreditOrDebit.Debit;
        //        accountTransaction.AccountTypeTransactionId = AccountTransactionCategory.CreditTransaction;
        //        accountTransaction.NewBalanceAfterTransaction = publicUser.Balance - fee.FlatAmount;


        //        publicUser.Balance = publicUser.Balance - fee.FlatAmount;
        //    return true;
        //    //
        //}
        //public bool ProcessFee(ConfigurationFee fee, Membership.Membership membership, AccountTransaction accountTransaction)
        //{

        //    //Check if this is pospaid or prepaid memebership
        //    if ((membership.MembershipAccountTypeId == Membership.Enums.MembershipAccountCategory.Regular
        //        || membership.MembershipAccountTypeId == Membership.Enums.MembershipAccountCategory.RegularRepresentative)
        //        && fee.AllowPostpaidIfClientIsPostpaid)
        //    {
        //        ////Get the account number of the postpaid transaction
        //        //RegularMembershipSetting activeSettings = null;
        //        //if (membership.MembershipAccountTypeId == Membership.Enums.MembershipAccountCategory.Regular)
        //        //{
        //        //    activeSettings = membership.RegularMembershipSettings.Where(s => s.IsActive == true).Single();
        //        //}
        //        //else
        //        //{
        //        //    activeSettings = membership.RegularMembershipSettings.Where(s => s.IsActive == true).Single().Represenative.RegularMembershipSettings.Where(s => s.IsActive == true).SingleOrDefault();
        //        //}

        //        accountTransaction.Amount = fee.FlatAmount;
        //        accountTransaction.CreditOrDebit  = CreditOrDebit.Debit ;
        //        accountTransaction.AccountTypeTransactionId = AccountTransactionCategory.PostpaidPaidTransaction;
        //        accountTransaction.NewBalanceAfterTransaction = membership.PrepaidCreditBalance - fee.FlatAmount;
        //        //accountTransaction.PostpaidMembershipSettings = activeSettings;

        //    }
        //    else
        //    {
        //        if (!CheckFees(fee.FlatAmount, membership.PrepaidCreditBalance))
        //        {

        //            return false;
        //        }
        //        accountTransaction.Amount = fee.FlatAmount;
        //        accountTransaction.CreditOrDebit = CreditOrDebit.Debit;
        //        accountTransaction.AccountTypeTransactionId = AccountTransactionCategory.CreditTransaction;
        //        accountTransaction.NewBalanceAfterTransaction = membership.PrepaidCreditBalance - fee.FlatAmount;


        //    }
        //    membership.PrepaidCreditBalance = membership.PrepaidCreditBalance - fee.FlatAmount;
        //    return true;
        //    //
        //}

        /// <summary>
        /// Check if we have a valid payment balance
        /// </summary>
        /// <param name="ChargeableService"></param>
        /// <param name="CreditBalance"></param>
        /// <returns></returns>
        //public bool CheckFees(ConfigurationFee fee, decimal CreditBalance)
        //{

        //    if (CreditBalance > fee.FlatAmount)
        //        return true;
        //    else
        //        return false;

        //}

        //public bool CheckFees(decimal FeeAmount, decimal CreditBalance)
        //{

        //    if (CreditBalance > FeeAmount)
        //        return true;
        //    else
        //        return false;

        //}

    }
}
