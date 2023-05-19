using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Respositoy.EF.All;
using CRL.Respositoy.EF.All.Common.Repository;
using CRL.Model.Messaging;
using CRL.Model.ModelViews;
using CRL.Model.Memberships;
using CRL.Model.Memberships.IRepository;
using CRL.Model.ModelViews.Memberships;


namespace CRL.Repository.EF.All.Repository.Memberships
{


    public class MembershipRepository : Repository<Membership, int>, IMembershipRepository
    {
        public MembershipRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public Membership GetMembershipDetailById(int id)
        {
            return ctx.Memberships.Include("MembershipType").Include("MembershipAccountType").Where(s => s.Id == id).Single();
        }
        public ViewPostpaidClientsResponse CustomViewPostpaidClients(ViewPostpaidClientsRequest request)
    {
        ViewPostpaidClientsResponse response = new ViewPostpaidClientsResponse();
        IQueryable<Membership > query = ctx.Memberships .AsNoTracking();
        query = query.Where(s => s.IsDeleted == false);
            query = query.Where(s => s.MembershipTypeId == Model.ModelViews.Enums.MembershipCategory.Client);
            
            if (request.SecurityUser.IsOwnerUser )
            {
                if (request .PostpaidFilter ==1)
                    query=query.Where (m=>m.MembershipAccountTypeId == Model.ModelViews.Enums.MembershipAccountCategory.Regular );
                else
                    query = query.Where(m => m.MembershipAccountTypeId == Model.ModelViews.Enums.MembershipAccountCategory.Regular ||
                        m.MembershipAccountTypeId == Model.ModelViews.Enums.MembershipAccountCategory.RegularRepresentative);
                if (request.RepresentativeMembershipId != null)
                    query = query.Where(m => m.RepresentativeId == request.RepresentativeMembershipId);
                
            }
            else
            {
                query=query.Where (m=>m.MembershipAccountTypeId == Model.ModelViews.Enums.MembershipAccountCategory.RegularRepresentative 
                    && m.RepresentativeId  == request .SecurityUser .MembershipId );
            }

            if (request.PageIndex > 0)
            {
                response.NumRecords = query.Count();
            }

            var query2 = query.Select(s => new ClientView
            {
                Id=s.Id ,
                Name =
                                (
                                s.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == s.Id && a.InstitutionId == null).Select(r => (r.FirstName + " " + r.MiddleName).TrimEnd() + " " + r.Surname).FirstOrDefault() :
                                s.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == s.Id).Select(r => r.Name).FirstOrDefault() : "Unknown"

                                ),
                AccountNumber = s.AccountNumber,
                ClientType =
                 (
                    s.isIndividualOrLegalEntity == 1 ? "Individual" :
                    s.isIndividualOrLegalEntity == 2 ? "Institution" : "Unknown"

                    ),

                RepresentativeClientId = s.RepresentativeId,
                RepresentativeClient = ctx.Institutions.Where(d => d.MembershipId == s.RepresentativeId).FirstOrDefault().Name ?? "NCRN"
            });

           
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "Name";
                    request.SortOrder = "ASC";
                }

                if (request.SortColumn == "Name")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.Name);
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Name);
                    }
                }



            

            if (request.PageIndex > 0 )
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }

            response.ClientView  = query2.ToList();
            return response;
         
    }
        public override string GetEntitySetName()
        {
            return "Membership";
        }
    }
}
