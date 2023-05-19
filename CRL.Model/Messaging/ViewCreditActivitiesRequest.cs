using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.Payments;
using CRL.Infrastructure.Messaging;


namespace CRL.Model.Messaging
{
    public enum GroupByNoOfFSStat
    {
        ByBorrowerType = 1, ByLenderType = 2, ByCollateralClass = 3, ByDebtorCounty = 4, BySecuredPartyCounty = 5, ByOwner = 6, ByDebtorCountry = 7,
        ByOwnerType = 8, ByTransactionType = 9
    }

    public class ViewSummaryPostpaidTransactionsRequest : PaginatedRequest
    {
        public DateRange TransactionDate { get; set; }
        public int? BatchTypeId { get; set; }
        public int? BatchId { get; set; }
        public int? SettlementType { get; set; }
        public int? LimitTo { get; set; }
        public int? ViewType { get; set; }

    }

    public class ViewSummaryClientExpenditureRequest : PaginatedRequest
    {
        public DateRange TransactionDate { get; set; }
        public GroupByNoOfFSStat? GroupBy { get; set; }

    }

    public class ViewClientExpenditureByTransactionRequest : PaginatedRequest
    {
        public DateRange TransactionDate { get; set; }
        public int? LimitToInstitutionId { get; set; }
        public int? LimitToUnitId { get; set; }

    }

    public class ViewClientExpenditureRequest : PaginatedRequest
    {
        public int? BatchId { get; set; }
    }



    public class ViewCreditActivitiesRequest : PaginatedRequest
    {


        public System.Nullable<CreditOrDebit> CreditOrDebit { get; set; } //Filter
        public System.Nullable<AccountTransactionCategory> AccountTypeTransactionId { get; set; }
        public ServiceFees? ServiceFeeTypeId { get; set; }
        public int? MembershipId { get; set; }
        public string Narration { get; set; }
        public int? AccountBatchId { get; set; }
        public int? AccountReconcileId { get; set; }
        public string NameOfUser { get; set; }
        public string NameOfUserLogin { get; set; }
        public string ClientName { get; set; } //For CBL users only
        public int? SettlementType { get; set; } //For CBL users only
        public int LimitTo { get; set; }
        public DateRange TransactionDate { get; set; }

    }

    public class ViewAccountBatchesRequest : PaginatedRequest
    {
        public int? Id { get; set; }
        public DateRange TransactionDate { get; set; }
        public string BatchStatus { get; set; }
        public DateTime? BatchDate { get; set; }

    }

    public class ViewReconcilationsRequest : PaginatedRequest
    {
        public int? Id { get; set; }
        public DateRange ReconciledDate { get; set; }


    }



}
