using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews;

using CRL.Repository.EF.All;
using CRL.Repository.EF.All.Repository.Memberships;
using CRL.Service.Messaging.Memberships.Request;
using CRL.Service.Views.Memberships;
using CRL.Model.Messaging;
using CRL.Model.ModelViews.Memberships;
using CRL.Model.Memberships.IRepository;
using CRL.Model.Memberships;

namespace CRL.Service.QueryGenerator
{
  
    public static class ClientQueryGenerator
    {
        public static IQueryable <ClientView> SelectPostpaidClient(
            ViewPostpaidClientsRequest request, IMembershipRepository _membershipRepository, bool DoCount = false)
        {
            CBLContext ctx = ((MembershipRepository)_membershipRepository).ctx;
            IQueryable <Membership> query = _membershipRepository .GetDbSet ();

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
                    && m.RepresentativeId == request .SecurityUser .MembershipId );
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
                RepresentativeClient = ctx.Institutions.Where(d => d.MembershipId == s.RepresentativeId).FirstOrDefault().Name ?? "CBL"
            });

            if (!DoCount)
            {
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



            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }

         
         


            /*
            select new ClientView
                        {
                            Name =
                                (
                                m.isIndividualOrLegalEntity == 1 ? ctx.People.OfType<User>().Where(a => a.MembershipId == m.Id && a.InstitutionId == null).Select(r => r.FirstName + r.MiddleName + r.Surname).SingleOrDefault() :
                                m.isIndividualOrLegalEntity == 2 ? ctx.Institutions.Where(a => a.MembershipId == m.Id).Select(r => r.Name).SingleOrDefault() : "Unknown"

                                ),
                            AccountNumber = m.AccountNumber,
                            ClientType =
                             (
                                m.isIndividualOrLegalEntity == 1 ? "Individual" :
                                m.isIndividualOrLegalEntity == 2 ? "Institution" : "Unknown"

                                ),

                            RepresentativeClientId = m.RepresentativeId,
                            RepresentativeClient = ctx.Institutions.Where(d => d.MembershipId == m.RepresentativeId).SingleOrDefault().Name
                        };



            */
          
            return query2;


        }
    }
}
