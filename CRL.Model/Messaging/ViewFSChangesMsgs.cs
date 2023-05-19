using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews.Amendment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Model.Messaging
{
    public class ViewFSActivityRequest : PaginatedRequest
    {

        public string RegistrationCode { get; set; }
        public int FSId { get; set; }
        public string ActivityCode { get; set; }
        public int FSActivityCategory { get; set; }
        public DateRange ActivityDate { get; set; }
        public string RequestNo { get; set; }
        public int? InstitutionId { get; set; }
        public int? MembershipId { get; set; }
        public bool inRequestMode { get; set; }
    }

    public class ViewFSActivityResponse : ResponseBase
    {
        public ICollection<FSActivityGridView> FSActivities { get; set; }
        public int NumRecords { get; set; }
    }
}
