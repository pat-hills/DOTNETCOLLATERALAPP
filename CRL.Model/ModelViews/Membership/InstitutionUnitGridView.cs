using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViews.Memberships
{
    public class InstitutionUnitGridView
    {
        public int Id;
        public string Name { get; set; }      
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool HasUsers { get; set; }
        public bool IsActive { get; set; }
    }
}
