using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Messaging;
using CRL.Model.ModelViews.FinancingStatement;
using CRL.Model.ModelViews.FinancingStatement;

namespace CRL.Model.FS.IRepository
{
    public interface IFinancialStatementRepository : IWriteRepository<FinancialStatement, int>
    {
        FinancialStatement SelectFSById(int id);
        FinancialStatement SelectFSDetailByRegNo(string RegNo);
        ViewFSResponse SelectFSGridViewCQ(ViewFSRequest request);
        FSView GetFSViewDetails(int Id, bool LoadActivities = true);
        IQueryable<FinancialStatement> GetDbSetComplete();
        IEnumerable<FileUploadView> SelectAttachments(int fsId);
    }

    public interface IFinancialStatementSnapshotRepository : IWriteRepository<FinancialStatementSnapshot, int>
    {

        ViewDraftsResponse DraftGridView(ViewDraftsRequest request);
    }

    public interface ITempAttachmentRepository : IWriteRepository<TempAttachment, int>
    {


    }

    public interface IFileUploadRepository : IWriteRepository<FileUpload, int>
    {
        FileUploadView GetLatestFileUpload(string uniqueGuidForm);
    }
}
