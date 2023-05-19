using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews;

using CRL.Service.Messaging.Memberships.Request;
using CRL.Model.Memberships;
using CRL.Model.Memberships.IRepository;

namespace CRL.Service.QueryGenerator
{
    public static class MembershipRegistrationFindQueryGenerator
    {
        public static IQueryable<MembershipRegistrationRequest> CreateQueryFor(
         ViewMembershipRegistrationsRequest request, IMembershipRegistrationRequestRepository _rpIn, bool DoCount = false)
        {

            IQueryable<MembershipRegistrationRequest> query = _rpIn.GetDbSet();

            //if (!(String.IsNullOrEmpty(request.Name)))
            //{
            //    query = query.Where(s => s.Na == request.Name);
            //}
          

            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "CreatedOn";
                    request.SortOrder = "ASC";
                }

                if (request.SortColumn == "CreatedOn")
                {
                    if (request.SortOrder == "desc")
                    {
                        query = query.OrderByDescending(s => s.CreatedOn);
                    }
                    else
                    {
                        query = query.OrderBy(s => s.CreatedOn );
                    }
                }


            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query = query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }
            return query;

        }
    }
}
