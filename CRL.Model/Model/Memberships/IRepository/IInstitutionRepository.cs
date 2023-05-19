using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Infrastructure.Helpers;
using CRL.Infrastructure.Messaging;
using CRL.Model.ModelViews;
using CRL.Model.ModelViews.Memberships;

namespace CRL.Model.Memberships.IRepository
{
    public class ViewClientInstitutionsRequest : PaginatedRequest
    {
        public DateRange CreatedRange { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? SecuredPartyType { get; set; }
        public int?[] SecuredPartyTypes { get; set; }
        public int MembershipGroupId { get; set; }  //Required
        public string ClientCode { get; set; }
        public int? MembershipStatus { get; set; }
        public int?[] InstitutionStatus { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public bool IsReportRequest { get; set; }


    }

    public class ViewClientInstitutionsResponse : ResponseBase
    {
        public ICollection<InstitutionGridView> InstitutionGridView { get; set; }
        public int NumRecords { get; set; }


    }

    public class GetCreateEditInstitutionResponse : ResponseBase
    {
        public GetCreateEditInstitutionResponse()
        {
            SecuringPartyTypes = new List<LookUpView>();
            RegularClientInstitutions = new List<LookUpView>();
            Nationalities = new List<LookUpView>();
            Countries = new List<LookUpView>();
            Countys = new List<LookUpView>();
        }
        public ICollection<LookUpView> SecuringPartyTypes { get; set; }  //Gets the type of institution
        public ICollection<LookUpView> RegularClientInstitutions { get; set; }
        public ICollection<LookUpView> Nationalities { get; set; }  //Gets the type of institution
        public ICollection<LookUpView> Countries { get; set; }
        public ICollection<LookUpView> Countys { get; set; }
        public InstitutionView InstitutionView { get; set; }


    }

    public interface IInstitutionRepository : IWriteRepository<Institution, int>
    {
        Institution GetInstitutionDetailById(int id);
        Institution GetInstitutionMembershipById(int id);
        /// <summary>
        /// Load institution by Id including the membership information
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        InstitutionView InstitutionMembershipViewById(int id);
        ViewClientInstitutionsResponse InstitutionGridView(ViewClientInstitutionsRequest request);
        GetCreateEditInstitutionResponse GetCreateEditInstitution();

        IQueryable<Institution> GetDbSetComplete();
    }
}
