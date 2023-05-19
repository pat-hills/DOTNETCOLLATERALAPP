using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Messaging;

namespace CRL.Model
{

    public interface IAuditRepository : IWriteRepository<Audit, int>
    {

       ViewAuditsResponse CustomViewAudits(ViewAuditsRequest request);
       ViewAuditDetailsResponse AuditDetails(ViewAuditDetailsRequest request);
    }
    public interface IServiceRequestRepository : IWriteRepository<ServiceRequest , int>
    {

    }

    public interface ILKAuditActionRepository : IWriteRepository<LKAuditAction, int> 
    {

    }

    public interface ILKAuditCategoryRepository : IWriteRepository<LKAuditCategory , int>
    {

    }
   
}
