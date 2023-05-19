using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.Search;
using CRL.Model.FS;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;

namespace CRL.Model.Search
{
    public class SearchResultTracker : AuditedEntityBaseModel<int>, IAggregateRoot
    {
        public SearchResultTracker() 
        {            
            
        }
        public int? FileUploadId { get; set; }
        public int SearchFinancialStatementId { get; set; }
        public string RegistrationNo { get; set; }
        public DateTime? ReportGenerationDate { get; set; }
        public int ReportGenerationCount { get; set; }
        public int SearchResultViewCount { get; set; }
        public SearchFinancialStatement SearchFinancialStatement { get; set; }
        public FileUpload FileUpload { get; set; }
        //Methods
        protected override void CheckForBrokenRules()
        {

            throw new NotImplementedException();
        }
    }
}
