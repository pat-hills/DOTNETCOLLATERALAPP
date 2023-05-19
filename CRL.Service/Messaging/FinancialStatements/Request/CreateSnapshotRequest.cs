using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Enums;

using CRL.Service.Views;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Authentication;
using CRL.Service.Views.FinancialStatement;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.Service.Messaging.FinancialStatements.Request
{
    [Serializable]
    public class CreateSnapshotRequest
    {
        public FSView FSView { get; set; }
        public EditMode EditMode { get; set; }
        public int SecurityUserId { get; set; }
        public int AssociatedIdForNonNew { get; set; }
        public int AssociatedCaseIdForEdit { get; set; }
        public int AssociatedWorkItemForEdit { get; set; }
        public int AssociatedFsActivityForUpdateEdit { get; set; }
        public int? AssociatedOriginalFinancialStatementIdForUpdateEdit { get; set; }
     
    }

    [Serializable]
    public class DraftRequest 
    {

        public FSView FSView { get; set; }
        public string DraftName { get; set; }
        public EditMode EditMode { get; set; }
        public int AssociatedIdForNonNew { get; set; }
        public int AssociatedCaseIdForEdit { get; set; }
        public int AssociatedWorkItemForEdit { get; set; }
        public int AssociatedFsActivityForUpdateEdit { get; set; }
        public int? AssociatedOriginalFinancialStatementIdForUpdateEdit { get; set; }
        public string UniqueGuidForm { get; set; }


    }

    public class SaveDraftRequest : DraftRequest
    {
        public List<FileUploadView> TempFileAttachments { get; set; }
        public SecurityUser SecurityUser { get; set; }
        public string RequestUrl { get; set; }
        public string UserIP { get; set; }
        public short RegistrationOrUpdate { get; set; }
        

    }

 


    public class SaveTempAttachmentRequest : RequestBase
    {
       public FileUploadView TempAttachment { get; set; }
    }

}
