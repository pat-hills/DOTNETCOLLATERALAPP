using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Service.Messaging.FinancialStatements.Request;
using CRL.Service.Messaging.FinancialStatements.Response;
using CRL.Infrastructure.Messaging;

namespace CRL.Service.Interfaces
{
    public interface IFSBatchService
    {
        GetDataForFSByEditModeResponse GetFSFromBatchForView(GetFSFromBatchRequest request);
        GetFileAttachmentResponse LoadBatchedFSAttachment(GetFSFromBatchRequest request);
        ViewBatchedFSListResponse ViewBatchedFSList(GetFSFromBatchRequest request);
        ViewBatchesResponse ViewBatches(ViewBatchRequest request);
        ResponseBase UploadBatch(UploadBatchRequest request);
        ResponseBase SubmitBatch(SubmitBatchRequest request);
        ViewBatchResponse GetBatchDetail(RequestBase request);
        ResponseBase DeleteBatch(RequestBase request);
        int[] GetBatchDetailsIdFromBatchId(RequestBase request);

    }
}
