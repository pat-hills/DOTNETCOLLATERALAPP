using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews;

using CRL.Service.Messaging.User.Request;
using CRL.Model.Messaging;
using CRL.Model.Memberships;
using CRL.Model.Memberships.IRepository;

namespace CRL.Service.QueryGenerator
{
    

     public static class RolesFindQueryGenerator
    {
         public static IQueryable<Role> CreateQueryFor(
            ViewUserRolesRequest request, IRoleRepository _rpIn, IUserRepository _user)
         {

             IQueryable<Role> query = _rpIn.GetDbSet();
             query = query.Where(s => s.IsActive == true);
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
                 //Load for which user is credible for
                 User user = _user.FindBy(request.UserId);
                 //If user is individual then include else exclude
                 if (user.InstitutionId == null)
                 {
                     query = query.Where(s => s.LimitToIndividualOrInstitution != 2);
                 }
                 else
                 {
                     query = query.Where(s => s.LimitToIndividualOrInstitution !=1);
                 }

                 if (user.Membership.MembershipTypeId == Model.ModelViews.Enums.MembershipCategory.Client)
                 {
                     query = query.Where(s => s.MembershipCategoryId != Model.ModelViews.Enums.MembershipCategory.Owner);
                 }
                 else if (user.Membership.MembershipTypeId == Model.ModelViews.Enums.MembershipCategory.Owner)
                 {
                     query = query.Where(s => s.MembershipCategoryId != Model.ModelViews.Enums.MembershipCategory.Client);
                 }

                 if (user.InstitutionUnitId  == null)
                 {
                     query = query.Where(s => s.LimitToUnitOrEnterprise  != 1);
                 }
                 else
                 {
                     query = query.Where(s => s.LimitToUnitOrEnterprise != 2);
                 }


                
                 

             }

          
           

           

             //**Check if user is individual or not and run query
             //**Check if user is owner or not



             return query;
         }


        }
}
