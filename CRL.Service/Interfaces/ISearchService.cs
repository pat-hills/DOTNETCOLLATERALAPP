using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using CRL.Infrastructure.Messaging;
using CRL.Model.Messaging;


namespace CRL.Service.Interfaces
{
    public interface ISearchService
    {
        SearchResponse Search(SearchRequest request);
        GetSearchResponse GetSearch(GetSearchRequest request);
        GetSearchResponse GetSearchQ(GetSearchRequest request);
        SearchResponse FilterOrLoadSearch(SearchRequest request);
        GenerateSearchReportResponse GenerateSearchReport(GenerateSearchReportRequest request);
        ViewSearchesResponse ViewSearchFS(ViewSearchesFSRequest request);
        DownloadSearchReportResponse DownloadSearchReport(DownloadSearchReportRequest request);
        ValidateSecurityCodeResponse ValidateSecurityCode(ValidateSecurityCodeRequest request);
        AssignFSToSearchResponse AssignFSToSearch(AssignFSToSearchRequest request);
        TrackSearchResultResponse TrackSearchResult(TrackSearchResultRequest request);
        GetExpiredSearchResultResponse GetExpiredSearchResults(GetExpiredSearchResultRequest request);
        ResponseBase AuditCacLink(RequestBase request);
        CACSearchresponse GetCACSearchResults(CACSearchrequest request);
    }
}
