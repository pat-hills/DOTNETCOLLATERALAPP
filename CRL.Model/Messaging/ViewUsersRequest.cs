using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews;
using CRL.Model.Memberships;


namespace CRL.Model.Messaging
{
   
    public class ViewUsersRequest : PaginatedRequest
    {
        public DateRange CreatedRange { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? InstitutionUnitId { get; set; }
        public int? InstitutionId { get; set; }  //Required
        public int? MembershipId { get; set; }
        public bool IsPaypointUser { get; set; }
        public bool IsNonPayPointUser { get; set; }
        public bool LoadOnlyIndividualClients { get; set; }
        public int? UserListOption { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string Unit { get; set; }
        public bool? Status { get; set; }
        public bool IsReportRequest { get; set; }
        public int[] AccountStatus { get; set; }
    }

    public class ViewUserRolesRequest : PaginatedRequest
    {
      
        public string Name { get; set; }
        public int Category { get; set; }
        public int UserId { get; set; } //the userid of which we will check the roles that are accepted
        public bool LoadForEdit { get; set; }
        public User User { get; set; }
        
    }
}
