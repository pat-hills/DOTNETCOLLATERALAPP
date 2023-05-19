using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.UnitOfWork;

namespace CRL.Model.Memberships.IRepository
{
   

    public interface IMembershipRegistrationRequestRepository : IWriteRepository<MembershipRegistrationRequest , int>
    {
      
    }

    
}
