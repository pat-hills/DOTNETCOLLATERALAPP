using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViews.Search
{
    public class SearchRequestGridView
    {
        public int Id { get; set; }
        public string SearchCode { get; set; }
        public string NameOfSearcher { get; set; }
        public string LoginId { get; set; }
        public DateTime SearchDate { get; set; }
        public DateTime? ReportGeneratedDate { get; set; }
        public bool HasCertifiedReport { get; set; }
        public bool HasUncertifiedReport { get; set; }
        public int? SearchReportId { get; set; }
        public int? MembershipId { get; set; }
        public string MembershipName { get; set; }
        public bool IsPublicUser { get; set; }     
        public string SearchRequestParameters { get; set; }
        public string SearchRequestParametersString { get; set; }
        public short IsLegalOrFlexible { get; set; }
        public string SearchType { get; set; }
        public string PublicUsrCode { get; set; }
        public int? SearchRequestResultId { get; set; }
        public int? SearchRequestCertifiedResultId { get; set; }
        public string FoundRegistrationNos { get; set; }
        public string FoundRegistrationNosString { get; set; }
        public int NumRecords { get; set; }
        public string ClientType { get; set; }
        public string InstitutionUnit { get; set; }
        public int? InstitutionUnitId { get; set; }
    }
}
