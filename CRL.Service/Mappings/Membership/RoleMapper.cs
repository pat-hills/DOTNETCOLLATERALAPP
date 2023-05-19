using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CRL.Model.ModelViews;
using CRL.Service.Views.Memberships;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;
using CRL.Model.Memberships;

namespace CRL.Service.Mappings.Membership
{
    public static class RoleMapper
    {
        public static ICollection<RoleGridView> ConvertToRoleGridViews(
                                                this ICollection<Role> Roles)
        {
            ICollection<RoleGridView> iviews = new List<RoleGridView>();
            foreach (var role in Roles)
            {
                iviews.Add(role.ConvertToRoleGridView ());
            }

            return iviews;
        }

        public static RoleGridView ConvertToRoleGridView(this Role Role)
        {
            RoleGridView RoleGridView = new RoleGridView();


            RoleGridView.Id = Convert.ToInt32(Role.Id);
            RoleGridView.Description = Role.Description ;
            RoleGridView .Name = Role .Name ;
            RoleGridView .RoleCategoryId = Role.RoleCategoryId;
            RoleGridView.RoleCategory = Role.RoleCategory.RoleCategoryName;
 
        

            return RoleGridView;


           
        }

        public static ICollection<Role> ConvertToRoles(
                                               this ICollection<RoleGridView> RoleGridViews)
        {
            return Mapper.Map<ICollection<RoleGridView>,
                              ICollection<Role>>(RoleGridViews);
        }

        public static Role ConvertToRole(this RoleGridView RoleGridView)
        {
            return Mapper.Map<RoleGridView, Role>(RoleGridView);
        }

        public static void ConvertToRole(this RoleGridView RoleGridView, Role Role)
        {
            Mapper.Map<RoleGridView, Role>(RoleGridView, Role);
        }


        
    }
}
