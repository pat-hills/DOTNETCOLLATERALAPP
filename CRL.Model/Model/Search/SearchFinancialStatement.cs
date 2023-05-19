using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.FS;
using CRL.Model.ModelViews;
using CRL.Model.Memberships;

namespace CRL.Model.Search
{
    public class SearchFinancialStatement : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public SearchFinancialStatement()
        {
            SearchResultTrackers = new HashSet<SearchResultTracker>();
            //GeneratedReport = new FileUpload();
        }
        public string SearchCode { get; set; }
        public string UniqueIdentifier { get; set; }
        public string SearchParamXML { get; set; }
        public string SearchParamString { get; set; }
        public short IsLegalOrFlexible { get; set; }
        public string PublicUsrCode { get; set; }
        public string CACResultsXML { get; set; }
        //public int? SearchRequestResultId { get; set; }
        public virtual FileUpload  GeneratedReport { get; set; }
        public int? GeneratedReportId { get; set; }
        public int? InstitutionUnitId { get; set; }
        public virtual InstitutionUnit InstitutionUnit{get;set;}
        public string FoundRegistrationXML { get; set; }
        public string FoundRegistrationString { get; set; }
        public int NumRecords { get; set; }
        public bool IsPublicUser { get; set; }
        public int? SelectedFSId { get; set; }
        public virtual ICollection<SearchResultTracker> SearchResultTrackers { get; set; }
        //We may need to get the result in PDF
        //For public users please provide an email which will be used to trace you with
        //It is getting the signed that should generate the payment aspect
        

        //User set to -1 will be used for public user we will use an id for the public user and the relationship will
        //not be too strict for this one

        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
