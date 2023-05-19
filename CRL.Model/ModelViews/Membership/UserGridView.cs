using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViews.Memberships
{
    public class UserGridView
    {
        public int Id;
        public string ClientCode { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string Nationality { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool isPayPoint { get; set; }
        public string Institution { get; set; }
        public int? MembershipId { get; set; }
        public int? InstitutionId { get; set; }
        public bool IsActive { get; set; }
        public string MajorityRole { get; set; }
        public string Unit { get; set; }
        public string Status { get; set; }
        public string CreatedByUser { get; set; }
        public string CreatedByUserFinancingInstitution { get; set; }
        public List<string> Roles { get; set; }
        public bool IsLocked { get; set; }
        public string FinancialInstitutionType { get; set; }
    }
}
