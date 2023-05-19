using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews;

using CRL.Service.Messaging.Institution.Request;
using CRL.Service.Messaging.Memberships.Request;
using CRL.Model.Messaging;
using CRL.Model.Memberships;
using CRL.Model.Memberships.IRepository;

namespace CRL.Service.QueryGenerator
{
    public static class InstitutionUnitFindQueryGenerator
    {
        public static IQueryable<InstitutionUnit> CreateQueryFor(
           ViewInstitutionUnitsRequest request, IInstitutionUnitRepository _rpIn, bool DoCount = false)
        {

            IQueryable<InstitutionUnit> query = _rpIn.GetDbSet();

            if (!(String.IsNullOrEmpty(request.Name )))
            {
                query = query.Where(s => s.Name == request.Name);
            }
            if (request.InstitutionId  > 0)
            {
                query = query.Where(s => s.InstitutionId == request.InstitutionId );
            }

            
            if (!DoCount)
            {
                if ((String.IsNullOrWhiteSpace(request.SortColumn)))
                {
                    request.SortColumn = "Name";
                    request.SortOrder = "asc";
                }

                if (request.SortColumn == "Name")
                {
                    if (request.SortOrder == "desc")
                    {
                        query = query.OrderByDescending(s => s.Name);
                    }
                    else
                    {
                        query = query.OrderBy(s => s.Name);
                    }
                }

                
                if (request.SortColumn == "CreatedOn")
                {
                    if (request.SortOrder == "desc")
                    {
                        query = query.OrderByDescending(s => s.CreatedOn);
                    }
                    else
                    {
                        query = query.OrderBy(s => s.CreatedOn);
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
