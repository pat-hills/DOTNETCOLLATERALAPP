using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews;
using CRL.Model.Messaging;
using CRL.Model.Search;
using CRL.Model.ModelViews.Search;
using CRL.Model.Payments;

using CRL.Model.ModelViews.FinancingStatement;
using CRL.Service.Views.Search;

namespace CRL.Model.Messaging
{

    public class GetSearchResponse : ResponseBase
    {
        public bool IsPayable { get; set; }        
        public ICollection<LookUpView> BusinessPrefixes { get; set; }
    }

    public class CACSearchresponse : ResponseBase
    {
        public string CACResults { get; set; }
    }
    public class SearchResponse:ResponseBase 
    {
        public int Id { get; set; }
        public string[] RegistrationNos { get; set; }
        public ICollection<SearchResultView> SearchResultView { get; set; }
        public int NumRecords { get; set; }
        //Add also finacial Activity
        public bool isPayable { get; set; }
        public decimal Amount { get; set; }
        public bool isPostpaid { get; set; }
        public decimal CurrentCredit { get; set; }
        public string publicUserEmail { get; set; }
        public FilterSearch FilterSearch { get; set; }
        public bool PaymentDeducted { get; set; }
        public ICollection<LookUpView> BusinessPrefixes { get; set; }
        public SearchParam SearchParam { get; set; }
        public DateTime SearchDate { get; set; }
        public int? AssignedFSId { get; set; }
        public bool HasSearchReport { get; set; }
        public string CACResults { get; set; }
        public CACSearch ResultsFromCAC { get; set; }
    }
    public class GenerateSearchReportResponse : ResponseBase
    {
        public int SearchId { get; set; }
        public bool ReportAlreadyExisted { get; set; }
        public bool PaymentDeducted { get; set; }
        public bool EmailSent { get; set; }
        public bool IsCertified { get; set; }
        public string PublicUserReceiptNo{get;set;}

    }

    public class DownloadSearchReportResponse : ResponseBase
    {
        public byte[] AttachedFile { get; set; }
        public string AttachedFileName { get; set; }
        public string AttachedFileType { get; set; }
        public string AttachedFileSize { get; set; }

    }

    public class ValidateSecurityCodeResponse : ResponseBase
    {
        public bool IsValid { get; set; }
        public bool HasInfo { get; set; }
        public decimal? Balance { get; set; }
        public PaymentSource PaymentType { get; set; }
    }

    public class AssignFSToSearchResponse : ResponseBase 
    {

    }

    public class TrackSearchResultResponse : ResponseBase
    {
        public bool HasSearchReport { get; set; }
    }

    public class GetExpiredSearchResultResponse:ResponseBase
    {
        public ICollection<string> ExpiredResultsRegNo { get; set; }
        public bool HasExpiredAllResults { get; set; }
        public ICollection<string> GeneratedSearchReportsRegNo{ get; set; }
        public ICollection<SearchReportView> SearchReports { get; set; }
        public bool HasSearchReports { get; set; }
    }
}
