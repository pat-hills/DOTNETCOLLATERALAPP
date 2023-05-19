using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Model.ModelViews;

using CRL.Repository.EF.All;
using CRL.Repository.EF.All.Repository.Memberships;
using CRL.Service.Messaging.Institution.Request;
using CRL.Service.Messaging.Memberships.Request;


using CRL.Model.Memberships.IRepository;
using CRL.Model.ModelViews.Memberships;
using CRL.Model.Memberships;

namespace CRL.Service.QueryGenerator
{
   

    public static class InstitutionFindQueryGenerator
    {
        public static IQueryable<InstitutionGridView> CreateQueryFor(
            ViewClientInstitutionsRequest request, IInstitutionRepository _rpIn, bool DoCount=false)
        {
            CBLContext ctx = ((InstitutionRepository)_rpIn).ctx;

            IQueryable<Institution> query = _rpIn.GetDbSet();
            query = query.Where(s => s.IsDeleted == false && s.Membership.IsActive   == true);
            if (!(String.IsNullOrEmpty(request .CompanyName )))
            {
                query = query.Where(s=>s.Name.ToLower ().StartsWith(request.CompanyName.ToLower ()) );
            }
            if (!(String.IsNullOrEmpty(request.Email )))
            {
                query = query.Where(s => s.Address.Email.ToLower().StartsWith(request.Email.ToLower()));
            }
            if (!(String.IsNullOrEmpty(request.Phone)))
            {
                query = query.Where(s => s.Address.Phone.ToLower().StartsWith(request.Phone.ToLower()));
            }
            if (!(String.IsNullOrEmpty(request.ClientCode )))
            {
                query = query.Where(s => s.Membership.ClientCode.ToLower().Contains(request.ClientCode.ToLower()));
            }
            if (request.MembershipGroupId >0)
            {
                query = query.Where(s => s.Membership.MembershipGroup == request.MembershipGroupId );
            }
            if (request.SecuredPartyType !=null)
            {
                query = query.Where(s => s.SecuringPartyTypeId == request.SecuredPartyType);
            }

            if (request.CreatedRange  != null)
            {
                query = query.Where(s => s.CreatedOn >= request.CreatedRange.StartDate && s.CreatedOn < request.CreatedRange.EndDate);
            }
            query=query.Where (s=>s.Membership.MembershipTypeId == Model.ModelViews.Enums.MembershipCategory.Client );

            var query2 = query.Select(s => new InstitutionGridView
            {
                Id = s.Id,
                ClientCode = s.Membership.ClientCode,
                Email = s.Address.Email,
                Address = s.Address .Address ,
                City = s.Address .City ,
                 CompanyNo= s.CompanyNo ,
                  Country = s.Country.Name  ,
                   County = s.County.Name  ,
                    Phone= s.Address .Phone ,
                    Name = s.Name ,
                     Nationality = s.Nationality.Name  ,     
                     SecuredPartyType = s.SecuringPartyType .SecuringPartyIndustryCategoryName ,
                      MembershipAccountType = s.Membership .MembershipAccountType .MembershipAccountCategoryName ,
                CreatedOn = s.CreatedOn,
                IsActive = s.IsActive,
                MajorityRole = s.Membership.MajorRoleIsSecuredPartyOrAgent==1 ?"Secured Party": "Agent" 

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
                        query2 = query2.OrderByDescending(s => s.Name );
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Name );
                    }
                }

                if (request.SortColumn == "Phone")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.Phone );
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Phone);
                    }
                }

                if (request.SortColumn == "Email")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.Email );
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.Email);
                    }
                }

                if (request.SortColumn == "CreatedOn")
                {
                    if (request.SortOrder == "desc")
                    {
                        query2 = query2.OrderByDescending(s => s.CreatedOn );
                    }
                    else
                    {
                        query2 = query2.OrderBy(s => s.CreatedOn);
                    }
                }
            }

            if (request.PageIndex > 0 && DoCount == false)
            {
                ////We are doing clientside filtering            
                query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

            }
            return query2;


        }
    }
}
