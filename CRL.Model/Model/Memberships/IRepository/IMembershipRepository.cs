using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Messaging;

namespace CRL.Model.Memberships.IRepository
{
    public interface IMembershipRepository : IWriteRepository<Membership, int>
    {
        Membership GetMembershipDetailById(int id);
        ViewPostpaidClientsResponse CustomViewPostpaidClients(ViewPostpaidClientsRequest request);
    }
}
