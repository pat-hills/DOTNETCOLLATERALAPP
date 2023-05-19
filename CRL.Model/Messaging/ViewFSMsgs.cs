using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViews.Search;
using CRL.Model.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.Messaging
{
    public enum FilterFinancingStatement { Active = 1, ActiveButTenDays = 2, Discharged = 3, Expired = 4, Approved = 5, Denied = 6, Pending = 7 }
    public class ViewFSRequest : PaginatedRequest
    {

        public DateRange RegistrationDate { get; set; }
        public string RegistrationNo { get; set; }
        public string RequestNo { get; set; }
        public int? FinancialStatementTransactionTypeId { get; set; }
        public int? FinancialStatementLoanTypeId { get; set; }
        public int? CollateralTypeId { get; set; }
        public string FinancialStatementLastActivity { get; set; }
        public int? InstitutionId { get; set; }
        public int? MembershipId { get; set; }
        public string MembershipName { get; set; }
        public bool inRequestMode { get; set; }
        public Nullable<DateTime> NewCreatedOn { get; set; }
        public FilterFinancingStatement ShowActive { get; set; }

    }

    public class ViewDraftsRequest : PaginatedRequest
    {

        public DateRange DraftDate { get; set; }
        public string DraftName { get; set; }
        public string RegistrationNo { get; set; }
        public Nullable<DateTime> NewCreatedOn { get; set; }


    }

    public class ViewSearchesFSRequest : PaginatedRequest
    {
        public DateRange SearchDate { get; set; }
        public string SearchCode { get; set; }
        public int? MembershipId { get; set; }
        public string PublicUserCode { get; set; }
        public string ClientName { get; set; }
        public string UserName { get; set; }
        public int? ClientType { get; set; }
        public short? GeneratedReportType { get; set; }
        public short? SearchType { get; set; }
        public short? ReturnedResults { get; set; }
        public bool LimitTo { get; set; }
        public bool IsReportRequest { get; set; }
        public Nullable<DateTime> NewCreatedOn { get; set; }

    }
    public class ViewFSResponse : ReportResponseBase
    {
        public ICollection<FSReportView> FSReportView { get; set; }
        public ICollection<DebtorReportView> DebtorReportView { get; set; }
        public ICollection<SecuredPartyReportView> SecuredPartyReportView { get; set; }
        public ICollection<CollateralReportView> CollateralReportView { get; set; }
        public ICollection<FSGridView> FSGridView { get; set; }


    }

    public class ViewDraftsResponse : ResponseBase
    {
        public ICollection<DraftView> DraftView { get; set; }
        public int NumRecords { get; set; }
    }

    public class ViewSearchesResponse : ResponseBase
    {
        public ICollection<SearchRequestGridView> SearchRequestGridView { get; set; }
        public int NumRecords { get; set; }
    }
}
