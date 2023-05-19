using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViews.Memberships
{
    public class AuditActionView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AuditTypeId { get; set; }
    }   
}
