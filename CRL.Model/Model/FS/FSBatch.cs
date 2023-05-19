using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Common;
using CRL.Model.Infrastructure;

namespace CRL.Model.FS
{
    [Serializable]
    public class FSBatch:AuditedEntityBaseModel<int>,IAggregateRoot
    {
        public FSBatch()
        {
            this.FSBatchDetail = new HashSet<FSBatchDetail>();
        }
        public string Name { get; set; }
        public bool IsSettled { get; set; }
        public int NumberOfFS { get; set; }
        public ICollection<FSBatchDetail> FSBatchDetail { get; set; }


        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class FSBatchDetail: EntityBase<int>, IAggregateRoot
    {
        public byte[] FSView { get; set; }        
        public int? FinancialStatementAttachmentId { get; set; }
        public virtual FileUpload FinancialStatementAttachment { get; set; }        
        public int FSBatchId{get;set;}
        public virtual FSBatch FSBatch { get; set; }
        public bool Uploaded { get; set; }
        public string UniqueNo { get; set; }



        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
