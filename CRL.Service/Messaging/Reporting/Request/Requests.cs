using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Service.Views.FinancialStatement;
using CRL.Model.Messaging;
using CRL.Model.ModelViews.Statistics;


namespace CRL.Service.Messaging.Reporting.Request
{
    public enum GroupByNoOfFSStat { ByBorrowerType = 1, ByLenderType = 2, ByCollateralClass = 3 ,ByDebtorCounty=4, BySecuredPartyCounty=5, ByOwner=6, ByDebtorCountry = 7,  
    ByOwnerType=8, ByTransactionType=9, ByCurrency = 10}

    public class PrepareViewFSStatRequest : RequestBase
    {
      
    }

    public class ViewFSStatRequest:RequestBase
    {
        public GroupByNoOfFSStat? GroupBy { get; set; }
        public bool LimitToWomenOwned { get; set; }
        public DateRange RegistrationDate { get; set; }
        public int FSState { get; set; }
        public int FSStateType { get; set; }
        public int FSPeriod { get; set; }
        public int? ReportId { get; set; }
        public int? ClientId { get; set; }
    }

    public class ViewStatRequest : RequestBase
    {
        public GroupByNoOfFSStat? GroupBy { get; set; }
        public bool LimitToWomenOwned { get; set; }
        public DateRange RegistrationDate { get; set; }
        public int? ReportId { get; set; }
        public int? ClientId { get; set; }
        public int? Status { get; set; }
    }

    public class RequestByDate : RequestBase
    {
      
        public DateRange TransactionDate { get; set; }
    }

    public class RequestByDateAndClient : RequestByDate
    {

        public int? ClientId { get; set; }
    }

    public class ViewFSCustomQueryResponse : ViewFSResponse
    {
        public ICollection<FSCustomReportView> FSCustomReportView { get; set; }
        public ICollection<FSCustomQueryStatView> FSCustomQueryStatView { get; set; }
    }



}
