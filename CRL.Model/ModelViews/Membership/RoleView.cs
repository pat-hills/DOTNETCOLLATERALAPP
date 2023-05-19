using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews.Enums;
using CRL.Service.Common;

namespace CRL.Model.ModelViews.Memberships
{
    public class RoleView:ViewBase
    {
        public int Id;
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public bool isSelected { get; set; }
        //Relationships
        public int RoleCategoryId { get; set; }
        public string RoleCategory { get; set; }      
        public MembershipCategory MembershipCategoryId { get; set; }  //Limit role to enterprise category
        public MembershipCategory MembershipCategory { get; set; } 
     

    }
}
