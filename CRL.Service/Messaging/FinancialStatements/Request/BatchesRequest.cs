using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Model.Configuration;


using CRL.Service.Views;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.FS;
using CRL.Model.Notification;
using CRL.Model.Memberships;
using System.Web;

namespace CRL.Service.Messaging.FinancialStatements.Request
{
    public class ViewBatchRequest : PaginatedRequest
    {
      
        public DateRange BatchDate { get; set; }
        public int? ShowType { get; set; }
    }

    public class GetFSFromBatchRequest : PaginatedRequest
    {

        public int? FSBatchId { get; set; }
        
    }

    public class UploadBatchRequest : RequestBase
    {
        public byte[] AttachedFile { get; set; }
        public int? CurrentDesktopVersion { get; set; }
        public int? LeastSupportedDesktopVersion { get; set; }
        public HttpPostedFileBase File { get; set; }
    }

    public class SubmitBatchRequest : RequestBase
    {
        public bool SubmitOnlySelected { get; set; }
        public int[] SubmittedFS { get; set; }
        public bool InWorkFlowMode { get; set; }
        public string WorkflowComment { get; set; }
    }

     public class CreateFSFromBatchRequest : RequestBase
    {
        public FSView FSView { get; set; }
        public IEnumerable<LKSectorOfOperationCategory> SectorOfOperationList { get; set; }
        public ICollection<Email> Emails { get; set; }
        public TransactionPaymentSetup configurationFee { get; set; }
        public Membership Membership { get; set; }
        
        
    }

     public class SubmitFSFromBatchRequest : CreateFSFromBatchRequest
    {      
        
        public string Comment { get; set; }
        
        
    }


   


}
   

