using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.ModelViews;
using CRL.Model.Messaging;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Model.Memberships.IRepository
{
    public interface IRoleRepository : IWriteRepository<Role, Roles>
    {

        ICollection<RoleGridView> GetRolesGrid(ViewUserRolesRequest request);
        IQueryable<Role> GetDbSetComplete();
    }
}
