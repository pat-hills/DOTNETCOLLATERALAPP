using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.Infrastructure;
using CRL.Model.ModelViews;
using CRL.Model.Memberships;

namespace CRL.Model.Reporting
{
    public enum ReportCategory { AuditReport = 1 }
    
    public class Report: AuditedEntityBaseModel<int>,IAggregateRoot
    {
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Filename { get; set; }
        [MaxLength(50)]
        public string ViewModelHelper { get; set; }
        public bool IgnoreLimitForPaypointUser { get; set; }
        public bool LimitToAdmin { get; set; }
        public short? LimitToClientOrOwners { get; set; }
        public short? LimitToInstitutionOrIndividual { get; set; }
        public string Description { get; set; }
        public bool HasExcelExport { get; set; }
        public bool HasCSVExport { get; set; }
        public ICollection<Role> Roles {get;set;}
        public LKReportCategory ReportCategory { get; set; }
        public LKReportModule ReportModule { get; set; }
        public int? ReportModuleId { get; set; }
        public int? MaximumNumberOfRecords { get; set; }
        public int? DefaultNumberOfRecords { get; set; }
        public bool PaginateRecords { get; set; }
        public ReportCategory ReportCategoryId { get; set; }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public partial class LKReportCategory : EntityBase<ReportCategory>, IAggregateRoot
    {

        public string Name { get; set; }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public partial class LKReportModule : EntityBase<int>, IAggregateRoot
    {

        public string Name { get; set; }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
