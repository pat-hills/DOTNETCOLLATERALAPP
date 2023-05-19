using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.ModelViews.Memberships
{
    public class InstitutionGridView
    {
        public int Id { get; set; }
        public string ClientCode { get; set; }
        public string Name { get; set; }
        public string CompanyNo { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string Nationality { get; set; }
        public string LGA { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string SecuredPartyType { get; set; }
        public string MembershipAccountType { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool HasUnits { get; set; }
        public bool HasUsers { get; set; }
        public string MajorityRole { get; set; }
        public int? MembershipId { get; set; }
        public int MembershipStatus { get; set; }
        public int? AuthorizedByUserId { get; set; }
        public string AuthorizedByUser { get; set; }
        public string RepresentativePostpaidClient { get; set; }
        public string Status { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> AuthorizedDate { get; set; }
    }
}
