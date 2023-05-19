using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews;

namespace CRL.Model.ModelViews.Memberships
{
    public class RoleGridView
    {
        public int Id;
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public bool isSelected { get; set; }
        public int RoleCategoryId { get; set; }
        public string RoleCategory { get; set; }  
    }
}
