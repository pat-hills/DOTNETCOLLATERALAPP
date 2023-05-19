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
    public interface IUserRepository : IWriteRepository<User, int>
    {

        UserView GetUserViewById(int id);
        User GetUserById(int id);
        IQueryable<User> GetDbSetComplete();
        IQueryable<User> GetNonDeletedUserByLoginId(string LoginId);
        IQueryable<User> GetNonDeletedUserByEmail(string Email);
        ViewUsersResponse GetUserGrid(ViewUsersRequest request);
    }
}
