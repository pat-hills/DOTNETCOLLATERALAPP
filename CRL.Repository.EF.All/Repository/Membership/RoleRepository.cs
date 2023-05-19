using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model.ModelViews;
using CRL.Respositoy.EF.All.Common.Repository;
using CRL.Model.Messaging;
using CRL.Model.Memberships;
using CRL.Model.Memberships.IRepository;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Repository.EF.All.Repository.Memberships
{


    public class RoleRepository : Repository<Role, Roles>, IRoleRepository
    {
        public RoleRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "Role";
        }

        public ICollection<RoleGridView> GetRolesGrid(ViewUserRolesRequest request)
        {
           
            var query = ctx.Roles.AsNoTracking().Where(s => s.IsDeleted == false );
            if (!(String.IsNullOrEmpty(request.Name)))
             {
                 query = query.Where(s => s.Name == request.Name);
             }
             if (request.Category > 0)
             {
                 query = query.Where(s => s.RoleCategoryId == request.Category);
             }

             if (!request.LoadForEdit)
             {

                 query = query.Where(s => s.Users.Any(a => a.Id == request.UserId));
             }
            else
            {
                 User user;
                 if (request.User != null)
                     user = request.User;
                 else
                     user = ctx.Set<User>().Where(s => s.Id == request.UserId).Single();
               
                //If user is individual then include else exclude
                if (user.InstitutionId == null)
                {
                    query = query.Where(s => s.LimitToIndividualOrInstitution != 2);
                }
                else
                {
                    query = query.Where(s => s.LimitToIndividualOrInstitution != 1);
                }

                if (user.Membership.MembershipTypeId == Model.ModelViews.Enums.MembershipCategory.Client)
                {
                    query = query.Where(s => s.MembershipCategoryId != Model.ModelViews.Enums.MembershipCategory.Owner);
                }
                else if (user.Membership.MembershipTypeId == Model.ModelViews.Enums.MembershipCategory.Owner)
                {
                    query = query.Where(s => s.MembershipCategoryId != Model.ModelViews.Enums.MembershipCategory.Client);
                }

                if (user.InstitutionUnitId == null)
                {
                    query = query.Where(s => s.LimitToUnitOrEnterprise != 1);
                }
                else
                {
                    query = query.Where(s => s.LimitToUnitOrEnterprise != 2);
                }





            }

             var query2 = query.Select(s => new RoleGridView 
             {
                 Id = (int)s.Id, 
                  Description = s.Description ,
                   Name = s.Name ,
                    Label = s.Label ,
                    RoleCategory = s.RoleCategory .RoleCategoryName ,
                     RoleCategoryId = s.RoleCategoryId               
               
             });
             if (String.IsNullOrWhiteSpace(request.SortColumn) == false)
             {

                 if (request.SortColumn == "Name")
                 {
                     if (request.SortOrder == "desc")
                     {
                         query2 = query2.OrderByDescending(s => s.Name );
                     }
                     else
                     {
                         query2 = query2.OrderBy(s => s.Name);
                     }
                 }
             }


             if (request.PageIndex > 0)
             {
                 ////We are doing clientside filtering            
                 query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

             }

            ICollection <RoleGridView >  RoleGridView = query2.ToList();
            return RoleGridView;


        }
        public IQueryable<Role> GetDbSetComplete()
        {
            throw new NotImplementedException();
        }
    }
}
