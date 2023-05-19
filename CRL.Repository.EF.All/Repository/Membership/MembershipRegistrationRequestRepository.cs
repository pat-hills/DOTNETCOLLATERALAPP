using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model.ModelViews;
using CRL.Respositoy.EF.All.Common.Repository;
using CRL.Model.Memberships;
using CRL.Model.Memberships.IRepository;

namespace CRL.Repository.EF.All.Repository.Memberships
{
    public class MembershipRegistrationRequestRepository : Repository<MembershipRegistrationRequest, int>, IMembershipRegistrationRequestRepository
    {
        public MembershipRegistrationRequestRepository(IUnitOfWork uow)
            : base(uow)
        {
        }



        public override string GetEntitySetName()
        {
            return "MembershipRegistrationRequest";
        }
    }
}
